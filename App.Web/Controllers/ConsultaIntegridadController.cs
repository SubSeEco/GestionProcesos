using App.Core.Interfaces;
using App.Core.UseCases;
using App.Model.Core;
using App.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    public class ConsultaIntegridadController : Controller
    {

        private readonly IGestionProcesos _repository;
        private readonly ISigper _sigper;
        private readonly IFile _file;
        private readonly IFolio _folio;
        private readonly IHsm _hsm;
        private readonly IEmail _email;
        private static List<DTODomainUser> ActiveDirectoryUsers { get; set; }

        public ConsultaIntegridadController(IGestionProcesos repository, ISigper sigper, IFile file, IFolio folio, IHsm hsm, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
            _email = email;


            if (ActiveDirectoryUsers == null)
                ActiveDirectoryUsers = AuthenticationService.GetDomainUser().ToList();

        }


        // GET: Consulta
        public ActionResult Index()
        {
            return View();
        }

        // GET: Consulta/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult View(int id)
        {
            var proceso = _repository.GetById<Proceso>(id);
            var model = _repository.GetFirst<Consulta>(q => q.ProcesoId == proceso.ProcesoId);

            return View(model);
        }

        // GET: Consulta/Create
        public ActionResult Create(int WorkFlowId)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new Consulta()
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
                Nombre = persona.Funcionario.PeDatPerChq,
                Rut = persona.Funcionario.RH_NumInte,
                DV = persona.Funcionario.RH_DvNuInt,
                Unidad = persona.Unidad.Pl_UndDes,
                Email = persona.Funcionario.Rh_Mail,
                CampoPrivacidad = true,
            };
            ViewBag.Fecha = DateTime.Now.ToString("yyyy-MM-dd");
            return View(model);
        }

        // POST: Consulta/Create
        [HttpPost]
        public ActionResult Create(Consulta model)
        {
            var _useCaseInteractor = new UseCaseIntegridad(_repository, _sigper, _file, _folio, _hsm, _email);
            var _UseCaseResponseMessage = _useCaseInteractor.ConsultaInsert(model);
            if (_UseCaseResponseMessage.IsValid)
            {
                return RedirectToAction("Edit", "ConsultaIntegridad", new { id = model.ConsultaId });
            }
            else
            {
                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            return View(model);
        }

        // GET: Consulta/Edit/5
        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<Consulta>(id);
            return View(model);

        }

        // POST: Consulta/Edit/5
        [HttpPost]
        public ActionResult Edit(Consulta model)
        {
            var _useCaseInteractor = new UseCaseIntegridad(_repository, _sigper, _file, _folio, _hsm, _email);
            var _UseCaseResponseMessage = _useCaseInteractor.ConsultaUpdate(model);

            if (_UseCaseResponseMessage.Warnings.Count > 0)
                TempData["Warning"] = _UseCaseResponseMessage.Warnings;

            if (_UseCaseResponseMessage.IsValid)
            {
                TempData["Success"] = "Operación terminada correctamente.";
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }
            else
            {
                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return View();
        }

        // GET: Consulta/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Consulta/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult ValidaAbogado(int id)
        {
            var model = _repository.GetById<Consulta>(id);
            return View(model);
        }

        public ActionResult ValidaCoordinador(int id)
        {
            var model = _repository.GetById<Consulta>(id);
            return View(model);
        }

        public ActionResult ValidaJefatura(int id)
        {
            var model = _repository.GetById<Consulta>(id);
            return View(model);
        }
    }
}
