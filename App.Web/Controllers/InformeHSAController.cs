using System;
using System.Web.Mvc;
using App.Model.Core;
using App.Model.InformeHSA;
using App.Core.Interfaces;
using Rotativa.MVC;
using App.Core.UseCases;
using System.Linq;
using System.IO;
using OfficeOpenXml;
using System.Collections.Generic;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
    public class DTOInformeHSA
    {
        public DTOInformeHSA()
        {
            DTOArchivos = new List<DTOArchivo>();
        }

        public int InformHSAId { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public DateTime FechaDesde { get; set; }

        public DateTime FechaHasta { get; set; }

        public int RUT { get; set; }

        public string Nombre { get; set; }

        public string Unidad { get; set; }

        public string NombreJefatura { get; set; }

        public bool ConJornada { get; set; } = false;

        public string Funciones { get; set; }

        public string Actividades { get; set; }

        public string Observaciones { get; set; }

        public string NumeroBoleta { get; set; }

        public DateTime FechaBoleta { get; set; }

        public string Email { get; set; }

        public string Estado { get; set; }

        public List<DTOArchivo> DTOArchivos { get; set; }
    }

    public class DTOArchivo
    {
        public DTOArchivo()
        {
        }

        public string FileString { get; set; }
        public string Filename { get; set; }
        public string Filetype { get; set; }
    }

    [Audit]
    [Authorize]
    public class InformeHSAController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IEmail _email;

        public InformeHSAController(IGestionProcesos repository, ISIGPER sigper, IFile file, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _email = email;
        }

        public ActionResult Create(int WorkFlowId)
        {
            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new InformeHSA()
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
            };

            var persona = _sigper.GetUserByEmail(User.Email());
            if (persona.Funcionario == null)
                ModelState.AddModelError(string.Empty, "No se encontró información del funcionario en SIGPER");
            if (persona.Unidad == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la unidad del funcionario en SIGPER");
            if (persona.Jefatura == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la jefatura del funcionario en SIGPER");

            if (ModelState.IsValid)
            {
                model.RUT = persona.Funcionario.RH_NumInte;
                model.Nombre = persona.Funcionario.PeDatPerChq;
                model.Unidad = persona.Unidad.Pl_UndDes;
                model.NombreJefatura = persona.Jefatura.PeDatPerChq;
                model.FechaDesde = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                model.FechaHasta = model.FechaDesde.Value.AddMonths(1).AddDays(-1);
                model.FechaBoleta = model.FechaHasta;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InformeHSA model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseInformeHSA(_repository, _sigper, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.Insert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return View(model);
        }

        public ActionResult View(int id)
        {
            var model = _repository.GetFirst<InformeHSA>(q => q.ProcesoId == id);
            if (model == null)
                return RedirectToAction("View", "Proceso", new { id });

            return View(model);
        }

        public ActionResult Pdf(int id)
        {
            var model = _repository.GetById<InformeHSA>(id);
            model.QR = _file.CreateQR(id.ToString());
            return new ViewAsPdf("pdf", model);
        }

        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<InformeHSA>(id);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<InformeHSA>(id);

            return View(model);
        }
        public ActionResult Details(int id)
        {
            var model = _repository.GetById<InformeHSA>(id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InformeHSA model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseInformeHSA(_repository, _sigper, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.Update(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return View(model);
        }

        [AllowAnonymous]
        public JsonResult GetHonorarioByRUT(int id)
        {
            var persona = _sigper.GetUserByRut(id);
            if (persona == null)
                return Json(new { ok = false, error = "Error al consultar el sistema SIGPER" }, JsonRequestBehavior.AllowGet);

            if (persona != null && persona.Funcionario == null)
                return Json(new { ok = false, error = "No se encontró información del funcionario en SIGPER" }, JsonRequestBehavior.AllowGet);

            if (persona != null && persona.DatosLaborales != null && persona.DatosLaborales.RH_ContCod != 10)
                return Json(new { ok = false, error = "El funcionario no tiene calidad jurídica de honorario a suma alzada" }, JsonRequestBehavior.AllowGet);

            if (persona != null && persona.DatosLaborales == null)
                return Json(new { ok = false, error = "No se encontró información laboral en SIGPER" }, JsonRequestBehavior.AllowGet);

            if (persona != null && persona.Jefatura == null)
                return Json(new { ok = false, error = "No se encontró información de la jefatura en SIGPER" }, JsonRequestBehavior.AllowGet);

            if (persona != null && persona.Unidad == null)
                return Json(new { ok = false, error = "No se encontró información de la unidad en SIGPER" }, JsonRequestBehavior.AllowGet);

            return Json(new { ok = true, error = "", Nombre = persona.Funcionario.PeDatPerChq, Unidad = persona.Unidad.Pl_UndDes, NombreJefatura = persona.Jefatura.PeDatPerChq, Email = persona.Funcionario.Rh_Mail }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult StartProcess(DTOInformeHSA model)
        {
            //validaciones
            if (model == null)
                return Json(new { ok = false, id = 0, error = "Problemas al crear proceso: informe vacio" }, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(model.Funciones))
                return Json(new { ok = false, id = 0, error = "Problemas al crear proceso: debe indicar las funciones" }, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(model.Actividades))
                return Json(new { ok = false, id = 0, error = "Problemas al crear proceso: debe indicar las actividades" }, JsonRequestBehavior.AllowGet);
            if (!model.DTOArchivos.Any())
                return Json(new { ok = false, id = 0, error = "Problemas al crear proceso: debe adjuntar archivos" }, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(model.NumeroBoleta))
                return Json(new { ok = false, id = 0, error = "Problemas al crear proceso: debe indicar el número de boleta" }, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(model.Email))
                return Json(new { ok = false, id = 0, error = "Problemas al crear proceso: debe indicar el email de funcionario" }, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(model.Nombre))
                return Json(new { ok = false, id = 0, error = "Problemas al crear proceso: debe indicar el nombre de funcionario" }, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(model.NombreJefatura))
                return Json(new { ok = false, id = 0, error = "Problemas al crear proceso: debe indicar el nombre de la jefatura" }, JsonRequestBehavior.AllowGet);
            if (model.RUT == 0)
                return Json(new { ok = false, id = 0, error = "Problemas al crear proceso: debe indicar un RUT válido de funcionario" }, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(model.Unidad))
                return Json(new { ok = false, id = 0, error = "Problemas al crear proceso: debe indicar la unidad del funcionario" }, JsonRequestBehavior.AllowGet);

            //crear proceso
            var _useCaseInformHSA = new UseCaseInformeHSA(_repository, _sigper, _email);
            var _UseCaseCrearInformeHSAResponseMessage = _useCaseInformHSA.InicioExterno(new Proceso
            {
                DefinicionProcesoId = (int)Util.Enum.DefinicionProceso.InformeHSA,
                Email = model.Email
            });

            if (!_UseCaseCrearInformeHSAResponseMessage.IsValid)
                return Json(new { ok = false, id = 0, error = "Problemas al crear proceso" }, JsonRequestBehavior.AllowGet);

            //crear workflow
            var workflow = _repository.GetFirst<Workflow>(q => q.ProcesoId == _UseCaseCrearInformeHSAResponseMessage.EntityId);
            if (workflow == null)
                return Json(new { ok = false, id = 0, error = "Problemas al obtener el workflow inicial" }, JsonRequestBehavior.AllowGet);

            //asociar boleta
            foreach (var file in model.DTOArchivos)
            {
                workflow.Documentos.Add(new Documento
                {
                    WorkflowId = workflow.WorkflowId,
                    ProcesoId = workflow.ProcesoId,
                    FileName = file.Filename,
                    Type = file.Filetype,
                    File = Convert.FromBase64String(file.FileString),
                    Email = model.Email
                });
            }

            //crear informe hsa
            var _UseCaseDocumentoResponseMessage = _useCaseInformHSA.Insert(new InformeHSA
            {
                Actividades = model.Actividades,
                ConJornada = model.ConJornada,
                FechaBoleta = model.FechaBoleta,
                FechaDesde = model.FechaDesde,
                FechaHasta = model.FechaHasta,
                FechaSolicitud = model.FechaSolicitud,
                Funciones = model.Funciones,
                Nombre = model.Nombre,
                NombreJefatura = model.NombreJefatura,
                NumeroBoleta = model.NumeroBoleta,
                Observaciones = model.Observaciones,
                RUT = model.RUT,
                Unidad = model.Unidad,
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
            });

            if (!_UseCaseDocumentoResponseMessage.IsValid)
                return Json(new { ok = false, id = 0, error = "Problemas al crear informe HSA" }, JsonRequestBehavior.AllowGet);

            //enviar tarea a siguiente paso
            var _useCaseCore = new UseCaseCore(_repository, _email, _sigper);

            var _UseCaseCoreResponseMessage = _useCaseCore.WorkflowUpdate(workflow);
            if (!_UseCaseCoreResponseMessage.IsValid)
                return Json(new { ok = false, id = 0, error = "Problemas al enviar informe a la siguiente tarea" }, JsonRequestBehavior.AllowGet);

            return Json(new { ok = true, id = _UseCaseCrearInformeHSAResponseMessage.EntityId }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetInformes(int id)
        {
            var persona = _sigper.GetUserByRut(id);
            if (persona == null)
                return Json(new { ok = false, error = "Error al consultar el sistema SIGPER" }, JsonRequestBehavior.AllowGet);
            if (persona != null && persona.Funcionario == null)
                return Json(new { ok = false, error = "No se encontró información del funcionario en SIGPER" }, JsonRequestBehavior.AllowGet);

            int definicionProcesoId = (int)Util.Enum.DefinicionProceso.InformeHSA;

            var DTOInformeHSA = _repository
                .Get<InformeHSA>(q => q.Proceso.DefinicionProcesoId == definicionProcesoId && q.Proceso.Email == persona.Funcionario.Rh_Mail.Trim())
                .Select(q => new DTOInformeHSA
                {
                    InformHSAId = q.Proceso.ProcesoId,
                    Estado = q.Proceso.EstadoProceso.Descripcion,
                    FechaSolicitud = q.FechaSolicitud.Value,
                    FechaDesde = q.FechaDesde.Value,
                    FechaHasta = q.FechaHasta.Value,
                    NumeroBoleta = q.NumeroBoleta
                }).ToList();

            return Json(new { ok = true, error = "", DTOInformeHSA }, JsonRequestBehavior.AllowGet);
        }

        public FileResult Report()
        {
            using (var context = new Infrastructure.GestionProcesos.AppContext())
            {
                var result = context
                    .InformeHSA
                    .AsNoTracking()
                    .Where(q => q.Proceso.EstadoProcesoId != (int)App.Util.Enum.EstadoProceso.Anulado).Select(hsa => new
                    {
                        hsa.ProcesoId,
                        hsa.Proceso.EstadoProceso.Descripcion,
                        hsa.FechaSolicitud,
                        hsa.FechaDesde,
                        hsa.FechaHasta,
                        hsa.RUT,
                        hsa.Nombre,
                        hsa.Unidad,
                        hsa.NombreJefatura,
                        ConJornada = hsa.ConJornada ? "SI" : "NO",
                        hsa.Funciones,
                        hsa.Actividades,
                        hsa.Observaciones,
                        hsa.FechaBoleta,
                        hsa.NumeroBoleta,
                    });

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var excel = new ExcelPackage(new FileInfo(string.Concat(Request.PhysicalApplicationPath, @"App_Data\HSA.xlsx")));
                excel.Workbook.Worksheets[0].Cells[2, 1].LoadFromCollection(result);
                excel.Workbook.Worksheets[0].Cells.AutoFitColumns();

                return File(excel.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
            }
        }
    }
}