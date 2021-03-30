using System.Web.Mvc;
using App.Model.Shared;
using App.Core.Interfaces;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
    public class RegionController : Controller
    {
        protected readonly IGestionProcesos _repository;

        public RegionController(IGestionProcesos repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<Region>();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Region>(id);
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Region model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.RegionInsert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<Region>(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Region model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.RegionUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Details", new { id = model.RegionId});
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<Region>(id);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseCometidoComision(_repository);
            var _UseCaseResponseMessage = _useCaseInteractor.RegionDelete(id);

            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            return RedirectToAction("Index");
        }
    }
}