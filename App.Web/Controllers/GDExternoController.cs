﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using App.Core.Interfaces;
using App.Core.UseCases;
using App.Model.Core;
using App.Model.GestionDocumental;
using App.Model.Sigper;
using App.Util;
using ExpressiveAnnotations.Attributes;
using OfficeOpenXml;
using AppContext = App.Infrastructure.GestionProcesos.AppContext;
using Enum = App.Util.Enum;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
    public class GDExternoController : Controller
    {
        public class DTOReport
        {
            [Display(Name = "Desde")]
            [DataType(DataType.Date)]
            [Required(ErrorMessage = "Es necesario especificar este dato")]
            public DateTime? Desde { get; set; }

            [Display(Name = "Hasta")]
            [DataType(DataType.Date)]
            [Required(ErrorMessage = "Es necesario especificar este dato")]
            public DateTime? Hasta { get; set; }

            [Display(Name = "Unidad")]
            public string UnidadCodigo { get; set; }
        }

        public class DTOFileUploadFEA
        {
            public int ProcesoId { get; set; }
            public int WorkflowId { get; set; }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Archivo")]
            [DataType(DataType.Upload)]
            public HttpPostedFileBase[] File { get; set; }

            [Display(Name = "Requiere firma electrónica?")]
            public bool RequiereFirmaElectronica { get; set; } 

            [Display(Name = "Es documento oficial?")]
            public bool EsOficial { get; set; } 

            [Display(Name = "Tiene firma electrónica?")]
            public bool TieneFirmaElectronica { get; set; }

            [RequiredIf("RequiereFirmaElectronica", ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Unidad del firmante")]
            public string FirmanteUnidadCodigo { get; set; }

            [Display(Name = "Unidad del firmante")]
            public string FirmanteUnidadDescripcion { get; set; }

            [RequiredIf("RequiereFirmaElectronica", ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Usuario firmante")]
            public string FirmanteEmail { get; set; }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Tipo documento")]
            public string TipoDocumentoCodigo { get; set; }

            [Display(Name = "Descripcion")]
            public string Descripcion { get; set; }
        }

        private readonly IGestionProcesos _repository;
        private readonly ISigper _sigper;
        private readonly IFile _file;
        private readonly IFolio _folio;
        private readonly IEmail _email;
        public GDExternoController(IGestionProcesos repository, ISigper sigper, IFile file, IFolio folio, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _email = email;
        }

        public ActionResult Details(int id)
        {
            //es autoridad
            var email = User.Email();
            var autoridades = _repository.GetFirst<Configuracion>(q => q.Nombre == Enum.Configuracion.autoridades.ToString());
            if (autoridades != null && !string.IsNullOrWhiteSpace(autoridades.Valor) && autoridades.Valor.Contains(email))
                return RedirectToAction("DetailsAutoridad", new { id });

            var model = _repository.GetById<GD>(id);
            return View(model);
        }

        public ActionResult DetailsAutoridad(int id)
        {
            var model = _repository.GetById<GD>(id);
            return View(model);
        }

        public ActionResult View(int id)
        {
            var model = _repository.GetFirst<GD>(q => q.ProcesoId == id);
            if (model == null)
                return RedirectToAction("View", "Proceso", new { id });

            return View(model);
        }

        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<GD>(id);
            return View(model);
        }

        public ActionResult Create(int? WorkFlowId)
        {
            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new GD
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
                IngresoExterno = true
            };

            ViewBag.GDOrigenId = new SelectList(_repository.GetAll<GDOrigen>(), "GDorigenId", "Descripcion");
            ViewBag.DestinoUnidadCodigo = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.DestinoFuncionarioEmail = new SelectList(new List<PEDATPER>().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList(), "Email", "Nombre").OrderBy(q => q.Text);
            ViewBag.DestinoUnidadCodigo2 = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.DestinoFuncionarioEmail2 = new SelectList(new List<PEDATPER>().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList(), "Email", "Nombre").OrderBy(q => q.Text);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GD model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseGD(_repository, _file, _sigper, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.Insert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.GDOrigenId = new SelectList(_repository.GetAll<GDOrigen>(), "GDorigenId", "Descripcion");
            ViewBag.DestinoUnidadCodigo = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.DestinoFuncionarioEmail = new SelectList(new List<PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.DestinoUnidadCodigo != null && model.DestinoUnidadCodigo.IsInt())
                ViewBag.DestinoFuncionarioEmail = new SelectList(_sigper.GetUserByUnidad(model.DestinoUnidadCodigo.ToInt()).Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList(), "Email", "Nombre").OrderBy(q => q.Text);

            ViewBag.DestinoUnidadCodigo2 = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.DestinoFuncionarioEmail2 = new SelectList(new List<PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.DestinoUnidadCodigo2 != null && model.DestinoUnidadCodigo.IsInt())
                ViewBag.DestinoFuncionarioEmail2 = new SelectList(_sigper.GetUserByUnidad(model.DestinoUnidadCodigo.ToInt()).Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList(), "Email", "Nombre").OrderBy(q => q.Text);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<GD>(id);
            ViewBag.GDOrigenId = new SelectList(_repository.GetAll<GDOrigen>(), "GDorigenId", "Descripcion");

            ViewBag.DestinoUnidadCodigo = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.DestinoFuncionarioEmail = new SelectList(new List<PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.DestinoUnidadCodigo != null && model.DestinoUnidadCodigo.IsInt())
                ViewBag.DestinoFuncionarioEmail = new SelectList(_sigper.GetUserByUnidad(model.DestinoUnidadCodigo.ToInt()).Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList(), "Email", "Nombre", model.DestinoFuncionarioEmail).OrderBy(q => q.Text);

            ViewBag.DestinoUnidadCodigo2 = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.DestinoFuncionarioEmail2 = new SelectList(new List<PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.DestinoUnidadCodigo2 != null && model.DestinoUnidadCodigo.IsInt())
                ViewBag.DestinoFuncionarioEmail2 = new SelectList(_sigper.GetUserByUnidad(model.DestinoUnidadCodigo2.ToInt()).Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList(), "Email", "Nombre", model.DestinoFuncionarioEmail2).OrderBy(q => q.Text);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GD model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseGD(_repository, _file, _sigper, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.Update(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.GDOrigenId = new SelectList(_repository.GetAll<GDOrigen>(), "GDorigenId", "Descripcion");
            ViewBag.DestinoUnidadCodigo = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.DestinoUnidadCodigo);
            if (model.DestinoUnidadCodigo != null && model.DestinoUnidadCodigo.IsInt())
                ViewBag.DestinoFuncionarioEmail = new SelectList(_sigper.GetUserByUnidad(model.DestinoUnidadCodigo.ToInt()).Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList(), "Email", "Nombre").OrderBy(q => q.Text);
            ViewBag.DestinoUnidadCodigo2 = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.DestinoUnidadCodigo);
            if (model.DestinoUnidadCodigo2 != null && model.DestinoUnidadCodigo.IsInt())
                ViewBag.DestinoFuncionarioEmail2 = new SelectList(_sigper.GetUserByUnidad(model.DestinoUnidadCodigo.ToInt()).Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList(), "Email", "Nombre").OrderBy(q => q.Text);

            return View(model);
        }

        public ActionResult FEAUpload(int ProcesoId, int WorkflowId)
        {
            ViewBag.TipoDocumentoCodigo = new SelectList(_folio.GetTipoDocumento().Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion");
            ViewBag.FirmanteUnidadCodigo = new SelectList(_sigper.GetUnidadesFirmantes(_repository.Get<Rubrica>(q => q.HabilitadoFirma).Select(q => q.Email.Trim()).ToList()), "Pl_UndCod", "Pl_UndDes");
            ViewBag.FirmanteEmail = new SelectList(new List<PEDATPER>().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList(), "Email", "Nombre");

            var model = new DTOFileUploadFEA { ProcesoId = ProcesoId, WorkflowId = WorkflowId };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FEAUpload(DTOFileUploadFEA model)
        {
            ViewBag.TipoDocumentoCodigo = new SelectList(_folio.GetTipoDocumento().Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion");
            ViewBag.FirmanteUnidadCodigo = new SelectList(_sigper.GetUnidadesFirmantes(_repository.Get<Rubrica>(q => q.HabilitadoFirma).Select(q => q.Email.Trim()).ToList()), "Pl_UndCod", "Pl_UndDes");
            ViewBag.FirmanteEmail = new SelectList(new List<PEDATPER>().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList(), "Email", "Nombre");

            var email = User.Email();

            if (Request.Files.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Debe adjuntar un archivo.");
            }

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var documento = new Documento();
                documento.Fecha = DateTime.Now;
                documento.Email = email;
                documento.ProcesoId = model.ProcesoId;
                documento.WorkflowId = model.WorkflowId;
                documento.TipoPrivacidadId = (int)Enum.Privacidad.Privado;
                documento.TipoDocumentoFirma = model.TipoDocumentoCodigo;
                documento.EsOficial = model.EsOficial;
                documento.Signed = model.TieneFirmaElectronica;
                documento.RequiereFirmaElectronica = model.RequiereFirmaElectronica;
                documento.FirmanteUnidad = model.FirmanteUnidadCodigo;
                documento.FirmanteEmail = !string.IsNullOrWhiteSpace(model.FirmanteEmail) ? model.FirmanteEmail.Trim() : null;
                documento.Descripcion = model.Descripcion;

                //contenido
                var file = Request.Files[i];
                if (file != null)
                {
                    var target = new MemoryStream();
                    file.InputStream.CopyTo(target);
                    documento.FileName = file.FileName;
                    documento.File = target.ToArray();

                    //metadata
                    var metadata = _file.BynaryToText(target.ToArray());
                    if (metadata != null)
                    {
                        documento.Texto = metadata.Text;
                        documento.Metadata = metadata.Metadata;
                        documento.Type = metadata.Type;
                    }
                    
                    var size = target.Length;
                    if (size > 62914560)
                    {
                        ModelState.AddModelError(string.Empty, "El archivo " + file.FileName + " excede el maximo de 50 MB.");
                    }
                }

                _repository.Create(documento);
            }

            if (ModelState.IsValid)
            {
                _repository.Save();
                TempData["Success"] = "Operación terminada correctamente.";
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }
            else
            {
                foreach (var error in ModelState.Values)
                {
                    for (int i = 0; i < error.Errors.Count; i++)
                    {
                        var errorModel = error.Errors[i];
                        if (errorModel != null)
                        {
                            var help = new List<string>();
                            help.Add(errorModel.ErrorMessage);
                            TempData["Error"] = help;
                        }
                    }
                }
                return Redirect(Request.UrlReferrer.PathAndQuery);

            }

            return View(model);
        }

        public ActionResult FEADocumentos(int ProcesoId)
        {
            var email = User.Email();
            var model = _repository.Get<Documento>(q => q.ProcesoId == ProcesoId && q.Activo);
            foreach (var item in model)
            {
                item.AutorizadoParaFirma = item.FirmanteEmail == email;
                item.AutorizadoParaEliminar = item.Email == email;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteDocument(int id)
        {
            var document = _repository.GetById<Documento>(id);
            if (document != null)
            {
                document.Activo = false;
                _repository.Save();
                TempData["Success"] = "Operación terminada correctamente.";
            }

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public PartialViewResult Row(int ProcesoId)
        {
            var model = _repository.GetFirst<GD>(q => q.ProcesoId == ProcesoId);
            return PartialView(model);
        }

        public PartialViewResult Workflow(int ProcesoId)
        {
            var model = _repository.Get<Workflow>(q => q.ProcesoId == ProcesoId);
            return PartialView(model);
        }

        public ActionResult Documents(int ProcesoId)
        {
            var model = _repository.Get<Documento>(q => q.ProcesoId == ProcesoId);
            return View(model);
        }

        public PartialViewResult WorkflowAutoridad(int ProcesoId)
        {
            var model = _repository.Get<Workflow>(q => q.ProcesoId == ProcesoId && q.TipoAprobacionId == (int)Enum.TipoAprobacion.Aprobada);
            return PartialView(model);
        }

        [HttpGet]
        public ActionResult Report()
        {
            ViewBag.UnidadCodigo = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            return View(new DTOReport { Desde = DateTime.Now });
        }

        public FileResult ReportGenerico()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excel = new ExcelPackage(new FileInfo(string.Concat(Request.PhysicalApplicationPath, @"App_Data\GDGenerico.xlsx")));

            using (var context = new AppContext())
            {
                var resumen = context
                .GD
                .AsNoTracking()
                .OrderBy(q => q.ProcesoId)
                .Select(item => new
                {
                    item.ProcesoId,
                    item.Proceso.DefinicionProceso.Nombre,
                    item.Proceso.Email,
                    Pl_UndDes = item.Proceso.Reservado ? string.Empty : item.Proceso.Pl_UndDes,
                    item.Proceso.EstadoProceso.Descripcion,
                    item.Proceso.FechaCreacion,
                    item.Proceso.FechaTermino,
                    item.Fecha,
                    item.FechaIngreso,
                    Materia = item.Proceso.Reservado ? string.Empty : item.Materia,
                    Referencia = item.Proceso.Reservado ? string.Empty : item.Referencia,
                    Observacion = item.Proceso.Reservado ? string.Empty : item.Observacion,
                    NumeroExterno = item.Proceso.Reservado ? string.Empty : item.NumeroExterno,
                    Origen = item.GDOrigen != null && !item.Proceso.Reservado ? item.GDOrigen.Descripcion : string.Empty,
                    DestinoFuncionarioNombre = item.Proceso.Reservado ? string.Empty : item.DestinoFuncionarioNombre,
                    DestinoUnidadDescripcion = item.Proceso.Reservado ? string.Empty : item.DestinoUnidadDescripcion,
                    Reservado = item.Proceso.Reservado ? "SI" : "NO"
                }).ToList();

                var idsProcesos =
                    resumen.Select(q => q.ProcesoId).Distinct();

                var detalle = context
                    .Workflow
                    .AsNoTracking()
                    .Where(q => q.Proceso.EstadoProcesoId != (int)Enum.EstadoProceso.Anulado/* && idsProcesos.Contains(q.ProcesoId)*/ && q.Proceso.FechaCreacion.Year == DateTime.Now.Year)
                    .OrderBy(q => q.ProcesoId)
                    .ThenBy(q => q.WorkflowId)
                    .Select(item => new
                    {
                        item.ProcesoId,
                        item.WorkflowId,
                        Proceso = item.Proceso.DefinicionProceso.Nombre,
                        Workflow = item.DefinicionWorkflow.Nombre,
                        TipoAprobacion = item.TipoAprobacion != null ? item.TipoAprobacion.Nombre : string.Empty,
                        item.FechaCreacion,
                        item.FechaTermino,
                        item.NombreFuncionario,
                        item.Pl_UndDes,
                        item.Observacion,
                    });

                excel.Workbook.Worksheets[0].Cells[2, 1].LoadFromCollection(resumen);
                excel.Workbook.Worksheets[0].Cells.AutoFitColumns();

                excel.Workbook.Worksheets[1].Cells[2, 1].LoadFromCollection(detalle);
                excel.Workbook.Worksheets[1].Cells.AutoFitColumns();

                return File(excel.GetAsByteArray(), MediaTypeNames.Application.Octet, DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
            }
        }

        public FileResult ReportMinutaDocumentosOP(DTOReport model)
        {
            var predicate = PredicateBuilder.True<GD>();

            predicate = predicate.And(q => q.Fecha.Value.Year == model.Desde.Value.Year && q.Fecha.Value.Month == model.Desde.Value.Month && q.Fecha.Value.Day == model.Desde.Value.Day);
            predicate = predicate.And(q => q.Proceso.EstadoProcesoId != (int)Enum.EstadoProceso.Anulado);
            predicate = predicate.And(q => q.IngresoExterno);

            if (!string.IsNullOrWhiteSpace(model.UnidadCodigo))
                predicate = predicate.And(q => q.DestinoUnidadCodigo == model.UnidadCodigo);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var excel = new ExcelPackage(new FileInfo(string.Concat(Request.PhysicalApplicationPath, @"App_Data\GDMinutaDocumentosOP.xlsx")));

            //documentos desde oficina de partes hacia las unidades

            using (var context = new AppContext())
            {
                var docs = context
                    .GD
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderBy(q => q.DestinoUnidadDescripcion)
                    .ThenBy(q => q.GDId)
                    .Select(item => new
                    {
                        item.DestinoUnidadDescripcion,
                        item.DestinoFuncionarioNombre,
                        item.NumeroExterno,
                        item.Fecha,
                        item.FechaIngreso,
                        item.ProcesoId,
                        Origen = item.GDOrigen != null ? item.GDOrigen.Descripcion : string.Empty,
                        item.Materia,
                        item.Referencia,
                        item.Observacion,
                    });

                excel.Workbook.Worksheets[0].Cells[2, 1].LoadFromCollection(docs);
                excel.Workbook.Worksheets[0].Cells.AutoFitColumns();

                return File(excel.GetAsByteArray(), MediaTypeNames.Application.Octet, DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
            }
        }

        public FileResult ReportPermanencia(DTOReport model)
        {
            var unidad = _sigper.GetUnidad(model.UnidadCodigo.ToInt());
            if (unidad == null)
                throw new Exception("No se encontró la unidad en sigper.");

            using (var context = new AppContext())
            {
                var procesosCerradosEnUnidad = context
                    .Workflow
                    .AsNoTracking()
                    .Where(q => q.EsTareaCierre && q.Pl_UndCod == unidad.Pl_UndCod).Select(q => q.ProcesoId)
                    .ToList();

                var desglose = context
                .Workflow
                .AsNoTracking()
                .Where(q =>
                    q.Proceso.Pl_UndCod != unidad.Pl_UndCod &&  //procesos originados fuera de mi unidad
                    q.Pl_UndCod == unidad.Pl_UndCod && // tareas atendidas por mi unidad
                    q.Terminada && //solo tareas terminadas
                    q.FechaTermino.HasValue && // solo tareas con fecha de termino
                    (q.FechaCreacion >= model.Desde && q.FechaCreacion <= model.Hasta) && //solo tareas atendidas dentro del periodo
                    q.Proceso.DefinicionProceso.Entidad.Codigo.Contains("GD") && //solo procesos gd
                    q.Proceso.EstadoProcesoId != (int)Enum.EstadoProceso.Anulado && //descartar procesos anulados
                    !procesosCerradosEnUnidad.Contains(q.ProcesoId) // descartar proceso cerrados en la unidad
                ).
                GroupBy(g => new
                {
                    g.ProcesoId,
                    g.Proceso.Pl_UndDes,
                }).
                Select(grupo => new
                {
                    grupo.Key.ProcesoId,
                    grupo.Key.Pl_UndDes,
                    workflows = grupo.ToList()
                }).
                ToList().
                Select(proceso => new
                {
                    proceso.Pl_UndDes,
                    proceso.ProcesoId,
                    documentosCreados = proceso.workflows.Count(),
                    minutosPermanencia = proceso.workflows.Sum(w => w.MinutosPermanencia) / 1440,
                }).ToList();


                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var excel = new ExcelPackage(new FileInfo(string.Concat(Request.PhysicalApplicationPath, @"App_Data\GDPermanencia.xlsx")));

                excel.Workbook.Worksheets[0].Cells[2, 2].Value = unidad.Pl_UndDes.Trim();
                excel.Workbook.Worksheets[0].Cells[3, 2].Value = model.Desde;
                excel.Workbook.Worksheets[0].Cells[4, 2].Value = model.Hasta;
                excel.Workbook.Worksheets[0].Cells[7, 1].LoadFromCollection(desglose);
                excel.Workbook.Worksheets[0].Cells.AutoFitColumns();

                return File(excel.GetAsByteArray(), MediaTypeNames.Application.Octet, DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
            }
        }
    }
}