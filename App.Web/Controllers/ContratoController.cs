using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Model.Contrato;
using App.Model.Core;
using App.Model.Shared;
using App.Core.Interfaces;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class ContratoController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        private static List<Models.DTODomainUser> ActiveDirectoryUsers { get; set; }

        public ContratoController(IGestionProcesos repository, ISIGPER sigper)
        {
            _repository = repository;
            _sigper = sigper;

            if (ActiveDirectoryUsers == null)
                ActiveDirectoryUsers = AuthenticationService.GetDomainUser().ToList();
        }

        public JsonResult GetUser(string term)
        {
            var result = ActiveDirectoryUsers
               .Where(q => (q.User != null && q.User.ToLower().Contains(term.ToLower())) || (q.Email != null && q.Email.ToLower().Contains(term.ToLower())))
               .Take(25)
               .Select(c => new { id = c.Email, value = string.Format("{0} ({1})", c.User, c.Email) })
               .OrderBy(q => q.value)
               .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<Contrato>();
            return View(model);
        }

        public ActionResult View(int id)
        {
            var model = _repository.GetById<Contrato>(id);
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Contrato>(id);
            return View(model);
        }

        public ActionResult Validate(int id)
        {
            var model = _repository.GetById<Contrato>(id);
            return View(model);
        }

        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<Contrato>(id);
            return View(model);
        }

        public ActionResult Create(int? WorkFlowId, int? ProcesoId)
        {
            ViewBag.ProgramaId = new SelectList(_repository.Get<Programa>().OrderBy(q => q.Nombre), "ProgramaId", "Nombre");
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades().OrderBy(q => q.Pl_UndDes), "Pl_UndCod", "Pl_UndDes");
            ViewBag.Pl_CodCar = new SelectList(_sigper.GetCargos().OrderBy(q => q.Pl_CodCar), "Pl_CodCar", "Pl_DesCar");

            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new Contrato
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
                Tarea = workflow.DefinicionWorkflow.Nombre
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.ContratoInsert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Edit", new { model.WorkflowId, id = model.ContratoId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.ProgramaId = new SelectList(_repository.Get<Programa>().OrderBy(q => q.Nombre), "ProgramaId", "Nombre", model.ProgramaId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades().OrderBy(q => q.Pl_UndDes), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.Pl_CodCar = new SelectList(_sigper.GetCargos().OrderBy(q => q.Pl_CodCar), "Pl_CodCar", "Pl_DesCar", model.Pl_CodCar);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<Contrato>(id);
            ViewBag.ProgramaId = new SelectList(_repository.Get<Programa>().OrderBy(q => q.Nombre), "ProgramaId", "Nombre", model.ProgramaId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades().OrderBy(q => q.Pl_UndDes), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.Pl_CodCar = new SelectList(_sigper.GetCargos().OrderBy(q => q.Pl_CodCar), "Pl_CodCar", "Pl_DesCar", model.Pl_CodCar);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Contrato model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.ContratoUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            ViewBag.ProgramaId = new SelectList(_repository.Get<Programa>().OrderBy(q => q.Nombre), "ProgramaId", "Nombre", model.ProgramaId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades().OrderBy(q => q.Pl_UndDes), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.Pl_CodCar = new SelectList(_sigper.GetCargos().OrderBy(q => q.Pl_CodCar), "Pl_CodCar", "Pl_DesCar", model.Pl_CodCar);

            return View(model);
        }
    }
}