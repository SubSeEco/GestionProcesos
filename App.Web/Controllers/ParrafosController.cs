using System.Web.Mvc;
using App.Model.Cometido;
using App.Core.Interfaces;
using App.Core.UseCases;
using App.Model.Core;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class ParrafosController : Controller
    {
        protected readonly IGestionProcesos _repository;

        public ParrafosController(IGestionProcesos repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<Parrafos>();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Parrafos>(id);
            return View(model);
        }

        public ActionResult Create()
        {
            ViewBag.DefinicionProcesoId = new SelectList(_repository.Get<DefinicionProceso>(), "DefinicionProcesoId", "Nombre");
            ViewBag.TipoDocumentoId = new SelectList(_repository.Get<TipoDocumento>(), "TipoDocumentoId", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Parrafos model)
        {
            ViewBag.DefinicionProcesoId = new SelectList(_repository.Get<DefinicionProceso>(), "DefinicionProcesoId", "Nombre");
            ViewBag.TipoDocumentoId = new SelectList(_repository.Get<TipoDocumento>(), "TipoDocumentoId", "Nombre");

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.ParrafosInsert(model);
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
            var model = _repository.GetById<Parrafos>(id);
            ViewBag.DefinicionProcesoId = new SelectList(_repository.Get<DefinicionProceso>(), "DefinicionProcesoId", "Nombre",model.DefinicionProcesoId);
            ViewBag.TipoDocumentoId = new SelectList(_repository.Get<TipoDocumento>(), "TipoDocumentoId", "Nombre",model.TipoDocumentoId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Parrafos model)
        {
            ViewBag.DefinicionProcesoId = new SelectList(_repository.Get<DefinicionProceso>(), "DefinicionProcesoId", "Nombre", model.DefinicionProcesoId);
            ViewBag.TipoDocumentoId = new SelectList(_repository.Get<TipoDocumento>(), "TipoDocumentoId", "Nombre", model.TipoDocumentoId);

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.ParrafosUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index");
                    //return RedirectToAction("Details", new { id = model.ParrafosId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<Parrafos>(id);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseCometidoComision(_repository);
            var _UseCaseResponseMessage = _useCaseInteractor.ParrafosDelete(id);

            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            return RedirectToAction("Index");
        }
    }
}