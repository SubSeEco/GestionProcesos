using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Model.CuentaRed;
using App.Model.Core;
using App.Model.Shared;
using App.Core.Interfaces;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class CuentaRedController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        private static List<Models.DTODomainUser> ActiveDirectoryUsers { get; set; }

        public CuentaRedController(IGestionProcesos repository, ISIGPER sigper)
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
            var model = _repository.GetAll<CuentaRed>();
            return View(model);
        }

        public ActionResult View(int id)
        {
            var model = _repository.GetById<CuentaRed>(id);
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<CuentaRed>(id);
            return View(model);
        }

        public ActionResult Validate(int id)
        {
            var model = _repository.GetById<CuentaRed>(id);
            return View(model);
        }


        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<CuentaRed>(id);
            return View(model);
        }
        public ActionResult Create(int? WorkFlowId, int? ProcesoId)
        {
            ViewBag.GeneroId = new SelectList(_repository.Get<Genero>().OrderBy(q => q.Nombre), "GeneroId", "Nombre");
            ViewBag.RegionId = new SelectList(_repository.Get<Region>().OrderBy(q => q.Nombre), "RegionId", "Nombre");
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades().OrderBy(q => q.Pl_UndDes), "Pl_UndCod", "Pl_UndDes");
            ViewBag.Pl_CodCar = new SelectList(_sigper.GetCargos().OrderBy(q => q.Pl_CodCar), "Pl_CodCar", "Pl_DesCar");

            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new CuentaRed
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CuentaRed model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && ActiveDirectoryUsers.Any(q => q.Email == model.Email))
                ModelState.AddModelError(string.Empty, "El email " + model.Email + " ya existe.");

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseInteractorCustom(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.CuentaRedInsert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.GeneroId = new SelectList(_repository.Get<Genero>().OrderBy(q => q.Nombre), "GeneroId", "Nombre", model.GeneroId);
            ViewBag.RegionId = new SelectList(_repository.Get<Region>().OrderBy(q => q.Nombre), "RegionId", "Nombre", model.RegionId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades().OrderBy(q => q.Pl_UndDes), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.Pl_CodCar = new SelectList(_sigper.GetCargos().OrderBy(q => q.Pl_CodCar), "Pl_CodCar", "Pl_DesCar", model.Pl_CodCar);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<CuentaRed>(id);

            ViewBag.GeneroId = new SelectList(_repository.Get<Genero>().OrderBy(q => q.Nombre), "GeneroId", "Nombre", model.GeneroId);
            ViewBag.RegionId = new SelectList(_repository.Get<Region>().OrderBy(q => q.Nombre), "RegionId", "Nombre", model.RegionId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades().OrderBy(q => q.Pl_UndDes), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.Pl_CodCar = new SelectList(_sigper.GetCargos().OrderBy(q => q.Pl_CodCar), "Pl_CodCar", "Pl_DesCar", model.Pl_CodCar);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CuentaRed model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && ActiveDirectoryUsers.Any(q => q.Email == model.Email))
                ModelState.AddModelError(string.Empty, "El email " + model.Email + " ya existe.");

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseInteractorCustom(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.CuentaRedUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.GeneroId = new SelectList(_repository.Get<Genero>().OrderBy(q => q.Nombre), "GeneroId", "Nombre", model.GeneroId);
            ViewBag.RegionId = new SelectList(_repository.Get<Region>().OrderBy(q => q.Nombre), "RegionId", "Nombre", model.RegionId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades().OrderBy(q => q.Pl_UndDes), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.Pl_CodCar = new SelectList(_sigper.GetCargos().OrderBy(q => q.Pl_CodCar), "Pl_CodCar", "Pl_DesCar", model.Pl_CodCar);

            return View(model);
        }
    }
}