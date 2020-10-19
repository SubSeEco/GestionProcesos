using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using App.Model.Core;
using App.Core.Interfaces;
using App.Util;
using App.Core.UseCases;
using System.IO;
using OfficeOpenXml;
using App.Model.Cometido;
using System.Xml;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class ProcesoController : Controller
    {
        public class DTODelete
        {
            public DTODelete()
            {

            }

            public int ProcesoId { get; set; }


            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Justificación")]
            [DataType(DataType.MultilineText)]
            public string JustificacionAnulacion { get; set; }
        }

        public class DTOFilter
        {
            public DTOFilter()
            {
                TextSearch = string.Empty;
                Select = new HashSet<App.Model.DTO.DTOSelect>();
                Result = new HashSet<Proceso>();
            }

            [Display(Name = "Texto de búsqueda")]
            public string TextSearch { get; set; }

            [Display(Name = "Desde")]
            [DataType(DataType.Date)]
            public System.DateTime? Desde { get; set; }

            [Display(Name = "Hasta")]
            [DataType(DataType.Date)]
            public System.DateTime? Hasta { get; set; }

            [Display(Name = "Tipos de proceso")]
            public IEnumerable<App.Model.DTO.DTOSelect> Select { get; set; }
            public IEnumerable<Proceso> Result { get; set; }

            [Display(Name = "Estado")]
            public int? EstadoProcesoId { get; set; }
        }

        public class Chart
        {
            public Chart()
            {
                datasets = new List<Datasets>();
            }
            public string[] labels { get; set; }
            public List<Datasets> datasets { get; set; }
        }

        public class Datasets
        {
            public string label { get; set; }
            public string[] backgroundColor { get; set; }
            public string[] borderColor { get; set; }
            public string borderWidth { get; set; }
            public double[] data { get; set; }
            public bool fill { get; set; }
        }

        protected readonly IGestionProcesos _repository;
        protected readonly IEmail _email;
        protected readonly ISIGPER _sigper;

        public ProcesoController(IGestionProcesos repository, IEmail email, ISIGPER sigper)
        {
            _repository = repository;
            _email = email;
            _sigper = sigper;
        }

        public JsonResult ProcesosPorTipo()
        {
            var result = _repository.GetAll<Proceso>().GroupBy(q => q.DefinicionProceso.Nombre).Select(y => new { Text = y.Key, Value = y.Count() });
            var _chart = new Chart()
            {
                labels = result.ToList().Select(q => q.Text).ToArray(),
            };

            _chart.datasets.Add(new Datasets()
            {
                label = "N° de procesos",
                data = result.ToList().OrderByDescending(q => q.Value).Select(q => (double)q.Value).ToArray(),
                backgroundColor = new string[] { "#2ecc71", "#3498db", "#e74c3c", "#f1c40f", "#9b59b6", "#34495e" }
            });

            return Json(_chart, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProcesosPorTiempoEjecucion()
        {
            var result = _repository.Get<Proceso>(q => q.FechaTermino.HasValue)
                      .GroupBy(a => a.DefinicionProceso.Nombre)
                      .Select(g => new { Text = g.Key, Value = g.Average(q => (q.FechaTermino - q.FechaCreacion).Value.TotalDays) });

            var _chart = new Chart()
            {
                labels = result.ToList().Select(q => q.Text).ToArray(),
            };

            _chart.datasets.Add(new Datasets()
            {
                label = "Días promedio",
                data = result.ToList().OrderByDescending(q => q.Value).Select(q => (double)q.Value).ToArray(),
                backgroundColor = new string[] { "#2ecc71", "#3498db", "#e74c3c", "#f1c40f", "#9b59b6", "#34495e" }
            });

            return Json(_chart, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TiempoRespuestaGrupo()
        {
            var result = _repository.Get<Workflow>(q => q.Terminada && q.Pl_UndDes != null)
                      .GroupBy(a => a.Pl_UndDes)
                      .Select(g => new { Text = g.Key, Value = g.Average(q => (q.FechaTermino - q.FechaCreacion).Value.TotalDays) });

            var _chart = new Chart()
            {
                labels = result.Take(10).ToList().Select(q => q.Text.Trim()).ToArray(),
            };

            _chart.datasets.Add(new Datasets()
            {
                label = "Días promedio",
                data = result.ToList().OrderByDescending(q => q.Value).Select(q => (double)q.Value).ToArray(),
                backgroundColor = new string[] { "#2ecc71", "#3498db", "#e74c3c", "#f1c40f", "#9b59b6", "#34495e" }
            });

            return Json(_chart, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProcesosPorDia()
        {
            var result = from p in _repository.GetAll<Proceso>()
                         group p by new { day = p.FechaCreacion.Date } into d
                         select new { Text = string.Format("{0:dd-MM-yyyy}", d.Key.day), Value = d.Count() };

            var _chart = new Chart()
            {
                labels = result.ToList().Select(q => q.Text).ToArray(),
            };

            _chart.datasets.Add(new Datasets()
            {
                label = "N° de procesos",
                data = result.ToList().Select(q => (double)q.Value).ToArray(),
            });

            return Json(_chart, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            ViewBag.EstadoProcesoId = new SelectList(_repository.Get<EstadoProceso>(), "EstadoProcesoId", "Descripcion");

            var model = new DTOFilter()
            {
                Select = _repository.GetAll<DefinicionProceso>().Where(q => q.Habilitado).OrderBy(q => q.Nombre).ToList().Select(q => new App.Model.DTO.DTOSelect() { Id = q.DefinicionProcesoId, Descripcion = q.Nombre, Selected = false }),
                Result = _repository.Get<Proceso>().ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DTOFilter model)
        {
            var predicate = PredicateBuilder.True<Proceso>();

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(model.TextSearch))
                    predicate = predicate.And(q => q.ProcesoId.ToString().Contains(model.TextSearch) || q.Observacion.Contains(model.TextSearch) || q.Email.Contains(model.TextSearch));

                if (model.Desde.HasValue)
                    predicate = predicate.And(q =>
                        q.FechaCreacion.Year >= model.Desde.Value.Year &&
                        q.FechaCreacion.Month >= model.Desde.Value.Month &&
                        q.FechaCreacion.Day >= model.Desde.Value.Day);

                if (model.Hasta.HasValue)
                    predicate = predicate.And(q =>
                        q.FechaCreacion.Year <= model.Desde.Value.Year &&
                        q.FechaCreacion.Month <= model.Desde.Value.Month &&
                        q.FechaCreacion.Day <= model.Desde.Value.Day);

                var DefinicionProcesoId = model.Select.Where(q => q.Selected).Select(q => q.Id).ToList();
                if (DefinicionProcesoId.Any())
                    predicate = predicate.And(q => DefinicionProcesoId.Contains(q.DefinicionProcesoId));

                if (model.EstadoProcesoId.HasValue)
                    predicate = predicate.And(q => q.EstadoProcesoId == model.EstadoProcesoId);

                model.Result = _repository.Get(predicate);
            }

            ViewBag.EstadoProcesoId = new SelectList(_repository.Get<EstadoProceso>(), "EstadoProcesoId", "Descripcion", model.EstadoProcesoId);

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Proceso>(id);
            return View(model);
        }

        public ActionResult View(int id)
        {
            var model = _repository.GetById<Proceso>(id);
            return View(model);
        }


        public ActionResult Create()
        {
            ViewBag.DefinicionProcesoId = new SelectList(_repository.Get<DefinicionProceso>(q => q.Habilitado && q.DefinicionWorkflows.Any(i => i.Habilitado)).OrderBy(q => q.Nombre), "DefinicionProcesoId", "Nombre");

            return View(new App.Model.Core.Proceso());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Proceso model)
        {
            var Email = UserExtended.Email(User);

            model.FechaCreacion = DateTime.Now;
            model.Email = Email;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository, _email, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.ProcesoInsert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";

                    //ver si proceso se ejecuta inmediatamente
                    var proceso = _repository.GetById<Proceso>(_UseCaseResponseMessage.EntityId);
                    if (proceso != null && proceso.DefinicionProceso.EjecutarInmediatamente)
                    {
                        var workflow = proceso.Workflows.OrderByDescending(q => q.WorkflowId).FirstOrDefault(q => !q.Terminada && !q.Anulada);
                        if (workflow != null)
                            return RedirectToAction("Execute", "Workflow", new { id = workflow.WorkflowId });
                    }

                    return RedirectToAction("Index", "Workflow");
                }
                else
                    TempData["Error"] = _UseCaseResponseMessage.Errors;

            }

            ViewBag.DefinicionProcesoId = new SelectList(_repository.Get<DefinicionProceso>(q => q.Habilitado).OrderBy(q => q.Nombre), "DefinicionProcesoId", "Nombre", model.DefinicionProcesoId);
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<Proceso>(id);
            return View(new DTODelete { ProcesoId = model.ProcesoId });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DTODelete model)
        {
            var _useCaseInteractor = new UseCaseCore(_repository, _email, _sigper);
            var _UseCaseResponseMessage = _useCaseInteractor.ProcesoDelete(model.ProcesoId, model.JustificacionAnulacion);
            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            var p = _repository.GetById<Proceso>(model.ProcesoId).DefinicionProcesoId;
            if (p == 13)
                return RedirectToAction("SeguimientoGP", "Cometido");

            var com = _repository.GetFirst<Cometido>(c => c.ProcesoId == model.ProcesoId);
            if (com != null)
                return RedirectToAction("View", "Cometido", new { id = com.CometidoId });

            return RedirectToAction("Index");
        }

        public ActionResult Dashboard()
        {
            var result = _repository.GetAll<Proceso>();
            ViewBag.EnCurso = result.Count(q => !q.Terminada);
            ViewBag.Terminados = result.Count(q => q.Terminada);
            ViewBag.Anulados = result.Count(q => q.Anulada);
            ViewBag.Totales = result.Count();

            return View();
        }

        public FileResult Report()
        {
            using (var context = new App.Infrastructure.GestionProcesos.AppContext())
            {
                var procesos = context.Proceso.Select(proceso => new
                {
                    proceso.ProcesoId,
                    proceso.DefinicionProceso.Nombre,
                    proceso.FechaCreacion,
                    proceso.FechaVencimiento,
                    proceso.FechaTermino,
                    proceso.Email,
                    proceso.EstadoProceso.Descripcion,
                    proceso.Observacion,
                    Reservado = proceso.Reservado ? "SI" : "NO"
                }).ToList();

                var workflows = context.Workflow.Select(workflow => new
                {
                    workflow.Proceso.ProcesoId,
                    workflow.Proceso.DefinicionProceso.Nombre,
                    workflow.Proceso.FechaCreacion,
                    workflow.Proceso.FechaVencimiento,
                    workflow.Proceso.FechaTermino,
                    workflow.Proceso.Email,
                    workflow.Proceso.EstadoProceso.Descripcion,
                    workflow.Proceso.Observacion,
                    workflow.WorkflowId,
                    WorkflowDefinicion = workflow.DefinicionWorkflow.Nombre,
                    workflow.Pl_UndDes,
                    WorkflowEmail = workflow.Email,
                    WorkflowFechaCreacion = workflow.FechaCreacion,
                    WorkflowFechaCreacionFechaTermino = workflow.FechaTermino,
                    WorkflowTipoAprobacion = workflow.TipoAprobacion.Nombre,
                    WorkflowObservacion = workflow.Observacion,
                }).ToList();

                var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\PROCESOS.xlsx");
                var fileInfo = new FileInfo(file);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var excel = new ExcelPackage(fileInfo);

                excel.Workbook.Worksheets[0].Cells[2, 1].LoadFromCollection(procesos);
                excel.Workbook.Worksheets[1].Cells[2, 1].LoadFromCollection(workflows);

                return File(excel.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");

            }
        }

        public ActionResult Header(int id)
        {
            var model = _repository.GetById<Proceso>(id);
            return View(model);
        }
    }
}