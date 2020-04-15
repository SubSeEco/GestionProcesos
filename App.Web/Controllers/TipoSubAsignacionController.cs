using System.Web.Mvc;
using App.Model.Cometido;
using App.Core.Interfaces;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class TipoSubAsignacionController : Controller
    {
        protected readonly IGestionProcesos _repository;

        public TipoSubAsignacionController(IGestionProcesos repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<TipoSubAsignacion>();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<TipoSubAsignacion>(id);
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TipoSubAsignacion model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.TipoSubAsignacionInsert(model);
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
            var model = _repository.GetById<TipoSubAsignacion>(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TipoSubAsignacion model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.TipoSubAsignacionUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Details", new { id = model.TipoSubAsignacionId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<TipoSubAsignacion>(id);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseCometidoComision(_repository);
            var _UseCaseResponseMessage = _useCaseInteractor.TipoSubAsignacionDelete(id);

            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            return RedirectToAction("Index");
        }
    }
}