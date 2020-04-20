using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using App.Model.Core;
using App.Core.Interfaces;
using App.Infrastructure.Extensions;
using App.Core.UseCases;
using App.Model.Comisiones;
using App.Model.Cometido;
using App.Model.Pasajes;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class ProcesoController : Controller
    {
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

            public IEnumerable<App.Model.DTO.DTOSelect> Select { get; set; }
            public IEnumerable<Proceso> Result { get; set; }
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
            var model = new DTOFilter()
            {
                Select = _repository.GetAll<DefinicionProceso>().OrderBy(q => q.Nombre).ToList().Select(q => new App.Model.DTO.DTOSelect() { Id = q.DefinicionProcesoId, Descripcion = q.Nombre, Selected = false }),
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
                    predicate = predicate.And(q => q.Observacion.Contains(model.TextSearch));

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

                model.Result = _repository.Get(predicate);
            }

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Proceso>(id);
            return View(model);
        }

        public ActionResult Create()
        {
            ViewBag.DefinicionProcesoId = new SelectList(_repository.Get<DefinicionProceso>(q => q.Habilitado && q.DefinicionWorkflows.Any(i => i.Habilitado)).OrderBy(q => q.Nombre), "DefinicionProcesoId", "Nombre");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Proceso model)
        {
            var Email = UserExtended.Email(User);

            model.FechaCreacion = DateTime.Now;
            model.Email = UserExtended.Email(User);

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository, _email, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.ProcesoInsert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
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
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseCore(_repository, _email);
            var _UseCaseResponseMessage = _useCaseInteractor.ProcesoDelete(id);

            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            return RedirectToAction("Index");
        }


        //public ActionResult Archive(int id)
        //{
        //    var model = _repository.GetById<Workflow>(id);
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Archive(Workflow model)
        //{
        //    model.Email = UserExtended.Email(User);

        //    if (ModelState.IsValid)
        //    {
        //        var _useCaseInteractor = new UseCaseCore(_repository, _email, _sigper);
        //        ResponseMessage _UseCaseResponseMessageDelete = new ResponseMessage();

        //        var workflow = _repository.Get<Workflow>(c => c.WorkflowId == model.WorkflowId).FirstOrDefault();
        //        var Entity = _repository.Get<DefinicionWorkflow>(d => d.DefinicionWorkflowId == workflow.DefinicionWorkflowId).FirstOrDefault().DefinicionProceso.EntidadId;
        //        if (Entity == 8)/*Se eliminan datos del destino, luego el cometido*/
        //        {
        //            var cometido = _repository.Get<Cometido>(c => c.CometidoId == workflow.EntityId.Value).FirstOrDefault();
        //            var destinos = _repository.Get<Destinos>(c => c.CometidoId == cometido.CometidoId);
        //            var cdp = _repository.Get<GeneracionCDP>(c => c.CometidoId == cometido.CometidoId);
        //            if (destinos != null)
        //            {
        //                foreach (var des in destinos)
        //                {
        //                    //_UseCaseResponseMessageDelete = _useCaseInteractor.DestinosDelete(des.DestinoId);
        //                }
        //            }

        //            if (cdp != null)
        //            {
        //                foreach (var c in cdp)
        //                {
        //                    //_UseCaseResponseMessageDelete = _useCaseInteractor.GeneracionCDPDelete(c.GeneracionCDPId);
        //                }
        //            }
        //            //_UseCaseResponseMessageDelete = _useCaseInteractor.CometidoDelete(cometido.CometidoId);
        //        }

        //        /*se elimina datos del proceso*/
        //        var _UseCaseResponseMessage = _useCaseInteractor.WorkflowArchive(model);
        //        if (_UseCaseResponseMessage.IsValid && _UseCaseResponseMessageDelete.IsValid)
        //        {
        //            TempData["Success"] = "Operación terminada correctamente.";
        //            return RedirectToAction("Index", "Workflow");
        //        }
        //        else
        //            TempData["Error"] = _UseCaseResponseMessage.Errors;
        //    }

        //    return View(model);
        //}



        public ActionResult Dashboard()
        {
            var result = _repository.GetAll<Proceso>();
            ViewBag.EnCurso = result.Count(q => !q.Terminada);
            ViewBag.Terminados = result.Count(q => q.Terminada);
            ViewBag.Anulados = result.Count(q => q.Anulada);
            ViewBag.Totales = result.Count();

            return View();
        }

        //public FileResult Download()
        //{
        //    var result = _repository.GetAll<Proceso>();

        //    var file = string.Concat(Request.PhysicalApplicationPath, @"App_Data\PROCESOS.xlsx");
        //    var fileInfo = new FileInfo(file);
        //    var excelPackage = new ExcelPackage(fileInfo);

        //    var fila = 1;
        //    var worksheet = excelPackage.Workbook.Worksheets[1];
        //    foreach (var proceso in result.ToList())
        //    {
        //        fila++;
        //        worksheet.Cells[fila, 1].Value = proceso.ProcesoId;
        //        worksheet.Cells[fila, 2].Value = proceso.DefinicionProceso != null ? proceso.DefinicionProceso.Nombre : string.Empty;
        //        worksheet.Cells[fila, 3].Value = proceso.FechaCreacion;
        //        worksheet.Cells[fila, 4].Value = proceso.FechaVencimiento;
        //        worksheet.Cells[fila, 5].Value = proceso.FechaTermino;
        //        worksheet.Cells[fila, 6].Value = proceso.Email;
        //        worksheet.Cells[fila, 7].Value = proceso.Terminada ? "SI" : "NO";
        //        worksheet.Cells[fila, 8].Value = proceso.Observacion;
        //    }

        //    fila = 1;
        //    worksheet = excelPackage.Workbook.Worksheets[2];
        //    foreach (var proceso in result.ToList())
        //    {
        //        foreach (var workflow in proceso.Workflows)
        //        {
        //            fila++;
        //            worksheet.Cells[fila, 1].Value = proceso.ProcesoId;
        //            worksheet.Cells[fila, 2].Value = proceso.DefinicionProceso != null ? proceso.DefinicionProceso.Nombre : string.Empty;
        //            worksheet.Cells[fila, 3].Value = proceso.FechaCreacion;
        //            worksheet.Cells[fila, 4].Value = proceso.FechaVencimiento;
        //            worksheet.Cells[fila, 5].Value = proceso.FechaTermino;
        //            worksheet.Cells[fila, 6].Value = proceso.Email;
        //            worksheet.Cells[fila, 7].Value = proceso.Terminada ? "SI" : "NO";
        //            worksheet.Cells[fila, 8].Value = proceso.Observacion;
        //            worksheet.Cells[fila, 9].Value = workflow.WorkflowId;
        //            worksheet.Cells[fila, 10].Value = workflow.DefinicionWorkflow.Nombre;
        //            worksheet.Cells[fila, 11].Value = workflow.Pl_UndDes;
        //            worksheet.Cells[fila, 12].Value = workflow.Email;
        //            worksheet.Cells[fila, 13].Value = workflow.FechaCreacion;
        //            worksheet.Cells[fila, 14].Value = workflow.FechaTermino;
        //            worksheet.Cells[fila, 15].Value = workflow.TipoAprobacion != null ? workflow.TipoAprobacion.Nombre : string.Empty;
        //            worksheet.Cells[fila, 16].Value = workflow.Observacion;
        //            worksheet.Cells[fila, 17].Value = workflow.Terminada ? "SI" : "NO";
        //        }
        //    }


        //    return File(excelPackage.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
        //}


        public ActionResult Search()
        {
            var model = new DTOFilter()
            {
                Select = _repository.GetAll<DefinicionProceso>().OrderBy(q => q.Nombre).ToList().Select(q => new App.Model.DTO.DTOSelect() { Id = q.DefinicionProcesoId, Descripcion = q.Nombre, Selected = false }),
                Result = _repository.Get<Proceso>().ToList()
            };

            foreach (var res in model.Result)//.Where(p => p.DefinicionProcesoId == 13))
            {
                switch (res.DefinicionProcesoId)
                {
                    case 13:
                        var com = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId);
                        if (com.Count() > 0)
                        {
                            model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().NroSolicitud = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().CometidoId.ToString();
                        }
                        break;
                    case 10:
                        var come = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId);
                        if (come.Count() > 0)
                        {
                            model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().NroSolicitud = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().CometidoId.ToString();
                        }
                        break;
                    case 12:
                        var comision = _repository.Get<Comisiones>(c => c.ProcesoId == res.ProcesoId);
                        if (comision.Count() > 0)
                        {
                            model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().NroSolicitud = _repository.Get<Comisiones>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().ComisionesId.ToString();
                        }
                        break;
                    //case 11:
                    //    var pasaje = _repository.Get<Pasaje>(c => c.ProcesoId == res.ProcesoId);
                    //    if (pasaje.Count() > 0)
                    //    {
                    //        model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().NroSolicitud = _repository.Get<Pasaje>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().PasajeId.ToString();
                    //    }
                    //    break;
                    //case 9:
                    //    var taxi = _repository.Get<RadioTaxi>(c => c.ProcesoId == res.ProcesoId);
                    //    if (taxi.Count() > 0)
                    //    {
                    //        model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().NroSolicitud = _repository.Get<RadioTaxi>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().RadioTaxiId.ToString();
                    //    }
                    //    break;
                }

                //var com = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId);
                //if (com.Count() > 0)
                //{
                //    model.Result.Where(p =>p.ProcesoId == res.ProcesoId).FirstOrDefault().NroSolicitud = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().CometidoId.ToString();
                //}
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Search(DTOFilter model)
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

                model.Result = _repository.Get(predicate);
            }

            foreach (var res in model.Result)//.Where(p => p.DefinicionProcesoId == 13))
            {
                switch (res.DefinicionProcesoId)
                {
                    case 13:
                        var com = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId);
                        if (com.Count() > 0)
                        {
                            model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().NroSolicitud = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().CometidoId.ToString();
                        }
                        break;
                    case 10:
                        var come = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId);
                        if (come.Count() > 0)
                        {
                            model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().NroSolicitud = _repository.Get<Cometido>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().CometidoId.ToString();
                        }
                        break;
                    case 12:
                        var comision = _repository.Get<Comisiones>(c => c.ProcesoId == res.ProcesoId);
                        if (comision.Count() > 0)
                        {
                            model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().NroSolicitud = _repository.Get<Comisiones>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().ComisionesId.ToString();
                        }
                        break;
                    case 11:
                        var pasaje = _repository.Get<Pasaje>(c => c.ProcesoId == res.ProcesoId);
                        if (pasaje.Count() > 0)
                        {
                            model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().NroSolicitud = _repository.Get<Pasaje>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().PasajeId.ToString();
                        }
                        break;
                    //case 9:
                    //    var taxi = _repository.Get<RadioTaxi>(c => c.ProcesoId == res.ProcesoId);
                    //    if (taxi.Count() > 0)
                    //    {
                    //        model.Result.Where(p => p.ProcesoId == res.ProcesoId).FirstOrDefault().NroSolicitud = _repository.Get<RadioTaxi>(c => c.ProcesoId == res.ProcesoId).FirstOrDefault().RadioTaxiId.ToString();
                    //    }
                    //    break;
                }
            }

            return View(model);
        }


        public ActionResult Me()
        {
            var email = UserExtended.Email(User);
            var model = new DTOFilter()
            {
                Select = _repository.GetAll<DefinicionProceso>().OrderBy(q => q.Nombre).ToList().Select(q => new App.Model.DTO.DTOSelect() { Id = q.DefinicionProcesoId, Descripcion = q.Nombre, Selected = false }),
                Result = _repository.Get<Proceso>(q => q.Email == email).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Me(DTOFilter model)
        {
            var email = UserExtended.Email(User);

            var predicate = PredicateBuilder.True<Proceso>();

            if (ModelState.IsValid)
            {
                predicate = predicate.And(q => q.Email == email);

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

                model.Result = _repository.Get(predicate);
            }

            return View(model);
        }
    }
}