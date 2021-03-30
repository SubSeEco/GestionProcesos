using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Model.Comisiones;
using App.Model.Shared;
//using App.Model.Shared;
using App.Core.Interfaces;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]

    public class ViaticoInternacionalController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        private static List<App.Model.DTO.DTODomainUser> ActiveDirectoryUsers { get; set; }
        public ViaticoInternacionalController(IGestionProcesos repository, ISIGPER sigper)
        {
            _repository = repository;
            _sigper = sigper;

            if (ActiveDirectoryUsers == null)
                ActiveDirectoryUsers = AuthenticationService.GetDomainUser().ToList();
        }

        public JsonResult GetCiudad(string IdPais)
        {
            var ciudad = _repository.Get<Ciudad>().Where(p => p.PaisId == int.Parse(IdPais));
            return Json(ciudad.Select(q => new { value = q.CiudadId, text = q.CiudadNombre.Trim() }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<ViaticoInternacional>();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<ViaticoInternacional>(id);
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new ViaticoInternacional();

            ViewBag.PaisId = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            ViewBag.CiudadId = new SelectList(_repository.Get<Ciudad>(), "CiudadId", "CiudadNombre");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViaticoInternacional model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.ViaticoInternacionalInsert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index");
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            ViewBag.PaisId = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            ViewBag.CiudadId = new SelectList(_repository.Get<Ciudad>(), "CiudadId", "CiudadNombre");

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<ViaticoInternacional>(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ViaticoInternacional model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.ViaticoInternacionalUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Details", new { id = model.ViaticoInternacionalId });
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<ViaticoInternacional>(id);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseCometidoComision(_repository);
            var _UseCaseResponseMessage = _useCaseInteractor.ViaticoInternacionalDelete(id);

            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            return RedirectToAction("Index");
        }
    }
}
