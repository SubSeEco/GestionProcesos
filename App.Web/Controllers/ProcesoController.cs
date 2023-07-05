using App.Core.Interfaces;
using App.Core.UseCases;
using App.Model.Cometido;
using App.Model.Core;
using App.Util;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
    public class ProcesoController : Controller
    {
        public class DTODelete
        {
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
                Select = new HashSet<Model.DTO.DTOSelect>();
                Result = new HashSet<Proceso>();
            }

            [Display(Name = "Texto de búsqueda")]
            public string TextSearch { get; set; }

            [Display(Name = "Desde")]
            [DataType(DataType.Date)]
            public DateTime? Desde { get; set; }

            [Display(Name = "Hasta")]
            [DataType(DataType.Date)]
            public DateTime? Hasta { get; set; }

            [Display(Name = "Tipos de proceso")]
            public IEnumerable<Model.DTO.DTOSelect> Select { get; set; }
            public IEnumerable<Proceso> Result { get; set; }

            [Display(Name = "Estado")]
            public int? EstadoProcesoId { get; set; }

            [Display(Name = "Unidad")]
            public string UnidadCodigo { get; set; }
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

        private readonly IGestionProcesos _repository;
        private readonly IEmail _email;
        private readonly ISigper _sigper;

        public ProcesoController(IGestionProcesos repository, IEmail email, ISigper sigper)
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
                backgroundColor = new[] { "#2ecc71", "#3498db", "#e74c3c", "#f1c40f", "#9b59b6", "#34495e" }
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
                data = result.ToList().OrderByDescending(q => q.Value).Select(q => q.Value).ToArray(),
                backgroundColor = new[] { "#2ecc71", "#3498db", "#e74c3c", "#f1c40f", "#9b59b6", "#34495e" }
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
                data = result.ToList().OrderByDescending(q => q.Value).Select(q => q.Value).ToArray(),
                backgroundColor = new[] { "#2ecc71", "#3498db", "#e74c3c", "#f1c40f", "#9b59b6", "#34495e" }
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
                Select = _repository.Get<DefinicionProceso>(q => q.Habilitado).OrderBy(q => q.Nombre).ToList().Select(q => new Model.DTO.DTOSelect() { Id = q.DefinicionProcesoId, Descripcion = q.Nombre, Selected = false }),
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

            return View(new Proceso());
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

        public ActionResult ReporteProceso()
        {
            ViewBag.UnidadCodigo = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            var user = _sigper.GetUserByEmail(User.Email());
            var model = new DTOFilter()
            {
                UnidadCodigo = _sigper.GetUnidad(user.Unidad.Pl_UndCod).Pl_UndCod.ToString()
            };

            return View(model);
        }

        public ActionResult ReporteGenerico(DTOFilter model)
        {
            using (var context = new Infrastructure.GestionProcesos.AppContext())
            {
                //if (model.Desde == null)
                //{
                //    ModelState.AddModelError(string.Empty, "Debe ingresar fecha Desde.");
                //}
                //if (model.Hasta == null)
                //{
                //    ModelState.AddModelError(string.Empty, "Debe ingresar fecha Hasta.");
                //}

                if (ModelState.IsValid)
                {
                    var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\PROCESOS.xlsx");
                    var fileInfo = new FileInfo(file);
                    var excel = new ExcelPackage(fileInfo);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    model.Hasta = DateTime.Today.AddDays(1).AddTicks(-1);
                    if (model.UnidadCodigo.IsNullOrWhiteSpace())
                    {
                        var procesos = context
                        .Proceso
                        .AsNoTracking()
                        .Where(q => model.Hasta >= q.FechaCreacion)
                        .Select(proceso => new
                        {
                            proceso.ProcesoId,
                            proceso.DefinicionProceso.Nombre,
                            proceso.FechaCreacion,
                            proceso.FechaVencimiento,
                            proceso.FechaTermino,
                            procesoEmail = proceso.Reservado ? "" : proceso.Email,
                            proceso.EstadoProceso.Descripcion,
                            Observacion = proceso.Reservado ? "" : proceso.Observacion,
                            Reservado = proceso.Reservado ? "SI" : "NO"
                        })
                        .ToList();

                        var workflows = context
                            .Workflow
                            .AsNoTracking()
                            .Where(q => model.Hasta >= q.FechaCreacion)
                            .Select(workflow => new
                            {
                                workflow.Proceso.ProcesoId,
                                workflow.Proceso.DefinicionProceso.Nombre,
                                workflow.Proceso.FechaCreacion,
                                workflow.Proceso.FechaVencimiento,
                                workflow.Proceso.FechaTermino,
                                workflowProcesoEmail = workflow.Proceso.Reservado ? "" : workflow.Proceso.Email,
                                workflow.Proceso.EstadoProceso.Descripcion,
                                observacion = workflow.Proceso.Reservado ? "" : workflow.Proceso.Observacion,
                                workflow.WorkflowId,
                                WorkflowDefinicion = workflow.DefinicionWorkflow.Nombre,
                                Pl_UndDes = workflow.Proceso.Reservado ? "" : workflow.Pl_UndDes,
                                WorkflowEmail = workflow.Proceso.Reservado ? "" : workflow.Email,
                                WorkflowFechaCreacion = workflow.FechaCreacion,
                                WorkflowFechaCreacionFechaTermino = workflow.FechaTermino,
                                WorkflowTipoAprobacion = workflow.Proceso.Reservado ? "" : workflow.TipoAprobacion.Nombre,
                                WorkflowObservacion = workflow.Proceso.Reservado ? "" : workflow.Observacion,
                            })
                            .ToList();

                        excel.Workbook.Worksheets[0].Cells[2, 1].LoadFromCollection(procesos);
                        excel.Workbook.Worksheets[1].Cells[2, 1].LoadFromCollection(workflows);
                    }
                    else
                    {
                        var procesos = context
                        .Proceso
                        .AsNoTracking()
                        .Where(q => model.Hasta >= q.FechaCreacion && q.Pl_UndCod.Value.ToString() == model.UnidadCodigo)
                        .Select(proceso => new
                        {
                            proceso.ProcesoId,
                            proceso.DefinicionProceso.Nombre,
                            proceso.FechaCreacion,
                            proceso.FechaVencimiento,
                            proceso.FechaTermino,
                            procesoEmail = proceso.Reservado ? "" : proceso.Email,
                            proceso.EstadoProceso.Descripcion,
                            Observacion = proceso.Reservado ? "" : proceso.Observacion,
                            Reservado = proceso.Reservado ? "SI" : "NO"
                        })
                        .ToList();

                        var workflows = context
                            .Workflow
                            .AsNoTracking()
                            .Where(q => model.Hasta >= q.FechaCreacion && q.Pl_UndCod.Value.ToString() == model.UnidadCodigo)
                            .Select(workflow => new
                            {
                                workflow.Proceso.ProcesoId,
                                workflow.Proceso.DefinicionProceso.Nombre,
                                workflow.Proceso.FechaCreacion,
                                workflow.Proceso.FechaVencimiento,
                                workflow.Proceso.FechaTermino,
                                workflowProcesoEmail = workflow.Proceso.Reservado ? "" : workflow.Proceso.Email,
                                workflow.Proceso.EstadoProceso.Descripcion,
                                observacion = workflow.Proceso.Reservado ? "" : workflow.Proceso.Observacion,
                                workflow.WorkflowId,
                                WorkflowDefinicion = workflow.DefinicionWorkflow.Nombre,
                                Pl_UndDes = workflow.Proceso.Reservado ? "" : workflow.Pl_UndDes,
                                WorkflowEmail = workflow.Proceso.Reservado ? "" : workflow.Email,
                                WorkflowFechaCreacion = workflow.FechaCreacion,
                                WorkflowFechaCreacionFechaTermino = workflow.FechaTermino,
                                WorkflowTipoAprobacion = workflow.Proceso.Reservado ? "" : workflow.TipoAprobacion.Nombre,
                                WorkflowObservacion = workflow.Proceso.Reservado ? "" : workflow.Observacion,
                            })
                            .ToList();

                        excel.Workbook.Worksheets[0].Cells[2, 1].LoadFromCollection(procesos);
                        excel.Workbook.Worksheets[1].Cells[2, 1].LoadFromCollection(workflows);
                    }

                    return File(excel.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "Reporte Génerico " + DateTime.Now.ToString("yyyy-MM-dd hhmmss") + ".xlsx");
                }
                else
                {
                    var help = new List<string>();
                    help.Add("En Reporte Procesos Génerico: ");
                    foreach (var error in ModelState.Values)
                    {
                        for (int i = 0; i < error.Errors.Count; i++)
                        {
                            var errorModel = error.Errors[i];
                            if (errorModel != null)
                            {
                                help.Add(errorModel.ErrorMessage);
                                TempData["Error"] = help;
                            }
                        }
                    }
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }
            }
        }
        #region Desarrollo Reportes Pendientes a la Fecha

        //public ActionResult ReportePendientesFecha(DTOFilter model)
        //{
        //    using (var context = new Infrastructure.GestionProcesos.AppContext())
        //    {
        //        var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\PROCESOS.xlsx");
        //        var fileInfo = new FileInfo(file);
        //        var excel = new ExcelPackage(fileInfo);
        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //        model.Hasta = DateTime.Today.AddDays(1).AddTicks(-1);
        //        var help = model.UnidadCodigo;

        //        var procesos = context
        //            .Proceso
        //            .AsNoTracking()
        //            .Where(q => model.Hasta >= q.FechaCreacion && q.EstadoProcesoId == (int)App.Util.Enum.EstadoProceso.EnProceso &&
        //            q.Pl_UndCod.Value.ToString() == model.UnidadCodigo)
        //            .Select(proceso => new
        //            {
        //                proceso.ProcesoId,
        //                proceso.DefinicionProceso.Nombre,
        //                proceso.FechaCreacion,
        //                proceso.FechaVencimiento,
        //                proceso.FechaTermino,
        //                procesoEmail = proceso.Reservado ? "" : proceso.Email,
        //                proceso.EstadoProceso.Descripcion,
        //                Observacion = proceso.Reservado ? "" : proceso.Observacion,
        //                Reservado = proceso.Reservado ? "SI" : "NO"
        //            })
        //            .ToList();

        //        var workflows = context
        //            .Workflow
        //            .AsNoTracking()
        //            .Where(q => model.Hasta >= q.FechaCreacion && !q.Anulada && !q.Terminada && q.Pl_UndCod.Value.ToString() == model.UnidadCodigo)
        //            .Select(workflow => new
        //            {
        //                workflow.Proceso.ProcesoId,
        //                workflow.Proceso.DefinicionProceso.Nombre,
        //                workflow.Proceso.FechaCreacion,
        //                workflow.Proceso.FechaVencimiento,
        //                workflow.Proceso.FechaTermino,
        //                workflowProcesoEmail = workflow.Proceso.Reservado ? "" : workflow.Proceso.Email,
        //                workflow.Proceso.EstadoProceso.Descripcion,
        //                observacion = workflow.Proceso.Reservado ? "" : workflow.Proceso.Email,
        //                workflow.WorkflowId,
        //                WorkflowDefinicion = workflow.DefinicionWorkflow.Nombre,
        //                Pl_UndDes = workflow.Proceso.Reservado ? "" : workflow.Pl_UndDes,
        //                WorkflowEmail = workflow.Proceso.Reservado ? "" : workflow.Email,
        //                WorkflowFechaCreacion = workflow.FechaCreacion,
        //                WorkflowFechaCreacionFechaTermino = workflow.FechaTermino,
        //                WorkflowTipoAprobacion = workflow.Proceso.Reservado ? "" : workflow.TipoAprobacion.Nombre,
        //                WorkflowObservacion = workflow.Proceso.Reservado ? "" : workflow.Observacion,
        //            })
        //            .ToList();

        //        excel.Workbook.Worksheets[0].Cells[2, 1].LoadFromCollection(procesos);
        //        excel.Workbook.Worksheets[1].Cells[2, 1].LoadFromCollection(workflows);

        //        return File(excel.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet,
        //            "Reporte Pendientes " + DateTime.Now.ToString("yyy-MM-dd HHmmss") + ".xlsx");
        //    }
        //}
        #endregion
        public FileResult ReporteGenericoGD(DTOFilter model)
        {
            using (var context = new Infrastructure.GestionProcesos.AppContext())
            {
                var procesos = context
                    .Proceso
                    .AsNoTracking()
                    .Where(q => q.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.GDInterno || q.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.GDOficParte)
                    .Select(proceso => new
                    {
                        proceso.ProcesoId,
                        proceso.DefinicionProceso.Nombre,
                        proceso.FechaCreacion,
                        proceso.FechaVencimiento,
                        proceso.FechaTermino,
                        procesoEmail = proceso.Reservado ? "" : proceso.Email,
                        proceso.EstadoProceso.Descripcion,
                        Observacion = proceso.Reservado ? "" : proceso.Observacion,
                        Reservado = proceso.Reservado ? "SI" : "NO"
                    })
                    .ToList();

                var workflows = context
                    .Workflow
                    .AsNoTracking()
                    .Where(q => q.FechaCreacion >= model.Desde && q.FechaCreacion <= model.Hasta && q.Proceso.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.GDInterno
                    || q.Proceso.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.GDOficParte)
                    .Select(workflow => new
                    {
                        workflow.Proceso.ProcesoId,
                        workflow.Proceso.DefinicionProceso.Nombre,
                        workflow.Proceso.FechaCreacion,
                        workflow.Proceso.FechaVencimiento,
                        workflow.Proceso.FechaTermino,
                        workflowProcesoEmail = workflow.Proceso.Reservado ? "" : workflow.Proceso.Email,
                        workflow.Proceso.EstadoProceso.Descripcion,
                        observacion = workflow.Proceso.Reservado ? "" : workflow.Proceso.Observacion,
                        workflow.WorkflowId,
                        WorkflowDefinicion = workflow.DefinicionWorkflow.Nombre,
                        Pl_UndDes = workflow.Proceso.Reservado ? "" : workflow.Pl_UndDes,
                        WorkflowEmail = workflow.Proceso.Reservado ? "" : workflow.Email,
                        WorkflowFechaCreacion = workflow.FechaCreacion,
                        WorkflowFechaCreacionFechaTermino = workflow.FechaTermino,
                        WorkflowTipoAprobacion = workflow.Proceso.Reservado ? "" : workflow.TipoAprobacion.Nombre,
                        WorkflowObservacion = workflow.Proceso.Reservado ? "" : workflow.Observacion,
                    })
                    .ToList();

                var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\PROCESOS.xlsx");
                var fileInfo = new FileInfo(file);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var excel = new ExcelPackage(fileInfo);

                excel.Workbook.Worksheets[0].Cells[2, 1].LoadFromCollection(procesos);
                excel.Workbook.Worksheets[1].Cells[2, 1].LoadFromCollection(workflows);

                //excel.Workbook.Worksheets[0].Cells.AutoFitColumns();
                //excel.Workbook.Worksheets[1].Cells.AutoFitColumns();

                return File(excel.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "Reporte Gestión Documental " + DateTime.Now.ToString("yyyy-MM-dd hhmmss") + ".xlsx");
            }
        }

        public ActionResult ReporteUnidad(DTOFilter model)
        {
            using (var context = new Infrastructure.GestionProcesos.AppContext())
            {
                if (model.Desde == null)
                {
                    ModelState.AddModelError(string.Empty, "Debe ingresar fecha Desde.");
                }
                if (model.Hasta == null)
                {
                    ModelState.AddModelError(string.Empty, "Debe ingresar fecha Hasta.");
                }
                if (model.UnidadCodigo.IsNullOrWhiteSpace())
                {
                    ModelState.AddModelError(string.Empty, "Debe seleccionar Unidad.");
                }

                if (ModelState.IsValid)
                {
                    model.Hasta = model.Hasta.Value.AddDays(1).AddTicks(-1);
                    var procesos = context
                        .Proceso
                        .AsNoTracking()
                        .Where(q => q.FechaCreacion >= model.Desde && q.FechaCreacion <= model.Hasta && q.Pl_UndCod.Value.ToString() == model.UnidadCodigo)
                        .Select(proceso => new
                        {
                            proceso.ProcesoId,
                            proceso.DefinicionProceso.Nombre,
                            proceso.FechaCreacion,
                            proceso.FechaVencimiento,
                            proceso.FechaTermino,
                            procesoEmail = proceso.Reservado ? "" : proceso.Email,
                            proceso.EstadoProceso.Descripcion,
                            Observacion = proceso.Reservado ? "" : proceso.Observacion,
                            Reservado = proceso.Reservado ? "SI" : "NO",
                            proceso.Pl_UndDes
                        })
                        .ToList();

                    var workflows = context
                        .Workflow
                        .AsNoTracking()
                        .Where(q => q.FechaCreacion >= model.Desde && q.FechaCreacion <= model.Hasta && q.Proceso.Pl_UndCod.Value.ToString() == model.UnidadCodigo)
                        .Select(workflow => new
                        {
                            workflow.Proceso.ProcesoId,
                            workflow.Proceso.DefinicionProceso.Nombre,
                            workflow.Proceso.FechaCreacion,
                            workflow.Proceso.FechaVencimiento,
                            workflow.Proceso.FechaTermino,
                            workflowProcesoEmail = workflow.Proceso.Reservado ? "" : workflow.Proceso.Email,
                            workflow.Proceso.EstadoProceso.Descripcion,
                            observacion = workflow.Proceso.Reservado ? "" : workflow.Proceso.Observacion,
                            workflow.WorkflowId,
                            WorkflowDefinicion = workflow.DefinicionWorkflow.Nombre,
                            Pl_UndDes = workflow.Proceso.Reservado ? "" : workflow.Pl_UndDes,
                            WorkflowEmail = workflow.Proceso.Reservado ? "" : workflow.Email,
                            WorkflowFechaCreacion = workflow.FechaCreacion,
                            WorkflowFechaCreacionFechaTermino = workflow.FechaTermino,
                            WorkflowTipoAprobacion = workflow.Proceso.Reservado ? "" : workflow.TipoAprobacion.Nombre,
                            WorkflowObservacion = workflow.Proceso.Reservado ? "" : workflow.Observacion,
                        })
                        .ToList();
                    var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\PROCESOSUNIDAD.xlsx");
                    var fileInfo = new FileInfo(file);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var excel = new ExcelPackage(fileInfo);

                    excel.Workbook.Worksheets[0].Cells[2, 1].LoadFromCollection(procesos);
                    excel.Workbook.Worksheets[1].Cells[2, 1].LoadFromCollection(workflows);

                    //excel.Workbook.Worksheets[0].Cells.AutoFitColumns();
                    //excel.Workbook.Worksheets[1].Cells.AutoFitColumns();

                    return File(excel.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "Reporte " + procesos.Last().Pl_UndDes + " " + DateTime.Now.ToString("yyyy-MM-dd hhmmss") + ".xlsx");
                }
                else
                {
                    var help = new List<string>();
                    help.Add("En Reporte Unidad: ");
                    foreach (var error in ModelState.Values)
                    {
                        for (int i = 0; i < error.Errors.Count; i++)
                        {
                            var errorModel = error.Errors[i];
                            if (errorModel != null)
                            {
                                help.Add(errorModel.ErrorMessage);
                                TempData["Error"] = help;
                            }
                        }
                    }
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }
            }
        }

        public ActionResult ReportePendientes(DTOFilter model)
        {
            using (var context = new Infrastructure.GestionProcesos.AppContext())
            {
                if (model.Desde == null)
                {
                    ModelState.AddModelError(string.Empty, "Debe ingresar fecha Desde.");
                }
                if (model.Hasta == null)
                {
                    ModelState.AddModelError(string.Empty, "Debe ingresar fecha Hasta.");
                }

                if (ModelState.IsValid)
                {
                    model.Hasta = model.Hasta.Value.AddDays(1).AddTicks(-1);
                    var procesos = context
                        .Proceso
                        .AsNoTracking()
                        .Where(q => q.FechaCreacion >= model.Desde && q.FechaCreacion <= model.Hasta && q.Pl_UndCod.Value.ToString() ==
                        model.UnidadCodigo && q.EstadoProcesoId == (int)App.Util.Enum.EstadoProceso.EnProceso)
                        .Select(proceso => new
                        {
                            proceso.ProcesoId,
                            proceso.DefinicionProceso.Nombre,
                            proceso.FechaCreacion,
                            proceso.FechaVencimiento,
                            proceso.FechaTermino,
                            procesoEmail = proceso.Reservado ? "" : proceso.Email,
                            proceso.EstadoProceso.Descripcion,
                            Observacion = proceso.Reservado ? "" : proceso.Observacion,
                            Reservado = proceso.Reservado ? "SI" : "NO",
                            proceso.Pl_UndDes
                        })
                        .ToList();

                    var workflows = context
                        .Workflow
                        .AsNoTracking()
                        .Where(q => q.FechaCreacion >= model.Desde && q.FechaCreacion <= model.Hasta &&
                        q.Proceso.Pl_UndCod.Value.ToString() == model.UnidadCodigo && !q.Terminada && !q.Anulada)
                        .Select(workflow => new
                        {
                            workflow.Proceso.ProcesoId,
                            workflow.Proceso.DefinicionProceso.Nombre,
                            workflow.Proceso.FechaCreacion,
                            workflow.Proceso.FechaVencimiento,
                            workflow.Proceso.FechaTermino,
                            workflowProcesoEmail = workflow.Proceso.Reservado ? "" : workflow.Proceso.Email,
                            workflow.Proceso.EstadoProceso.Descripcion,
                            observacion = workflow.Proceso.Reservado ? "" : workflow.Proceso.Observacion,
                            workflow.WorkflowId,
                            WorkflowDefinicion = workflow.DefinicionWorkflow.Nombre,
                            Pl_UndDes = workflow.Proceso.Reservado ? "" : workflow.Pl_UndDes,
                            WorkflowEmail = workflow.Proceso.Reservado ? "" : workflow.Email,
                            WorkflowFechaCreacion = workflow.FechaCreacion,
                            WorkflowFechaCreacionFechaTermino = workflow.FechaTermino,
                            WorkflowTipoAprobacion = workflow.Proceso.Reservado ? "" : workflow.TipoAprobacion.Nombre,
                            WorkflowObservacion = workflow.Proceso.Reservado ? "" : workflow.Observacion,
                        })
                        .ToList();
                    var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\PROCESOSUNIDAD.xlsx");
                    var fileInfo = new FileInfo(file);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var excel = new ExcelPackage(fileInfo);

                    excel.Workbook.Worksheets[0].Cells[2, 1].LoadFromCollection(procesos);
                    excel.Workbook.Worksheets[1].Cells[2, 1].LoadFromCollection(workflows);

                    //excel.Workbook.Worksheets[0].Cells.AutoFitColumns();
                    //excel.Workbook.Worksheets[1].Cells.AutoFitColumns();

                    return File(excel.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "Reporte " + procesos.Last().Pl_UndDes + " " + DateTime.Now.ToString("yyyy-MM-dd hhmmss") + ".xlsx");
                }
                else
                {
                    var help = new List<string>();
                    help.Add("En Reporte Unidad: ");
                    foreach (var error in ModelState.Values)
                    {
                        for (int i = 0; i < error.Errors.Count; i++)
                        {
                            var errorModel = error.Errors[i];
                            if (errorModel != null)
                            {
                                help.Add(errorModel.ErrorMessage);
                                TempData["Error"] = help;
                            }
                        }
                    }
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }
            }
        }

        public ActionResult Header(int id)
        {
            var model = _repository.GetById<Proceso>(id);
            return View(model);
        }
    }
}