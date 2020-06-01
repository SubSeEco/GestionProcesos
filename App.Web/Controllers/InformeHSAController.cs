using System;
using System.Web.Mvc;
using App.Model.Core;
using App.Model.InformeHSA;
using App.Core.Interfaces;
using Rotativa.MVC;
using System.Web;
using RestSharp;
using App.Infrastructure.Email;
using App.Core.UseCases;
using System.Net;

namespace App.Web.Controllers
{
    public class DTOInformeHSA
    {
        public DTOInformeHSA()
        {
        }

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

        public byte[] Boleta { get; set; }

        public string Email { get; set; }
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
                var _useCaseInteractor = new Core.UseCases.UseCaseInformeHSA(_repository);
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

            //var email = UserExtended.Email(User);
            //var rubrica = _repository.GetFirst<Rubrica>(q => q.Email == email);
            //if (rubrica != null)
            //    model.Signature = rubrica.File;

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
                var _useCaseInteractor = new Core.UseCases.UseCaseInformeHSA(_repository);
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
                return null;

            if (persona != null && persona.FunDatosLaborales != null && persona.FunDatosLaborales.RH_ContCod != 10)
                return null;

            return Json(new
            {
                Nombre = persona.Funcionario.PeDatPerChq,
                Unidad = persona.Unidad.Pl_UndDes,
                NombreJefatura = persona.Jefatura.PeDatPerChq,
                Email = persona.Funcionario.Rh_Mail
            }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult StartProcess(DTOInformeHSA model)
        {
            //crear proceso
            var _useCaseCore = new UseCaseCore(_repository, _email, _sigper);
            var _UseCaseCoreResponseMessage = _useCaseCore.ProcesoInsert(new Proceso {
                DefinicionProcesoId = (int)App.Util.Enum.DefinicionProceso.InformeHSA,
                Email = model.Email
            });

            if (!_UseCaseCoreResponseMessage.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json("Problemas al crear proceso", JsonRequestBehavior.AllowGet);
            }

            //crear workflow
            var workflow = _repository.GetFirst<Workflow>(q => q.ProcesoId == _UseCaseCoreResponseMessage.EntityId);
            if (workflow == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json("Problemas al obtener el workflow inicial", JsonRequestBehavior.AllowGet);
            }

            //crear informe hsa
            var _useCaseInformeHSAInteractor = new UseCaseInformeHSA(_repository);
            var _UseCaseInformeHSAResponseMessage = _useCaseInformeHSAInteractor.Insert(new InformeHSA {
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

            if (!_UseCaseInformeHSAResponseMessage.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json("Problemas al crear informe HSA", JsonRequestBehavior.AllowGet);
            }

            //enviar tarea a paso 2
            var _UseCaseWorkflowResponseMessage = _useCaseCore.WorkflowUpdate(workflow);
            if (!_UseCaseWorkflowResponseMessage.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json("Problemas al activar la segunda tarea", JsonRequestBehavior.AllowGet);
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(_UseCaseCoreResponseMessage.EntityId, JsonRequestBehavior.AllowGet);
        }
    }
}
