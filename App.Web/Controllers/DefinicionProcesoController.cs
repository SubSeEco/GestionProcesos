using System.Linq;
using System.Web.Mvc;
using App.Model.Core;
using App.Core.Interfaces;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class DefinicionProcesoController : Controller
    {
        protected readonly IGestionProcesos _repository;

        public DefinicionProcesoController(IGestionProcesos repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<DefinicionProceso>();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<DefinicionProceso>(id);
            model.Grupos = string.Join(", ", _repository.Get<DefinicionWorkflow>(q=>q.DefinicionProcesoId == id && q.Grupo != null).Select(q=>q.Grupo.Nombre).Distinct());

            return View(model);
        }

        public ActionResult Create()
        {
            ViewBag.EntidadId = new SelectList(_repository.GetAll<Entidad>().OrderBy(q => q.Nombre), "EntidadId", "Nombre");

            return View(new DefinicionProceso() { Habilitado = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DefinicionProceso model)
        {
            model.Habilitado = true;
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.DefinicionProcesoInsert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            ViewBag.EntidadId = new SelectList(_repository.GetAll<Entidad>().OrderBy(q => q.Nombre), "EntidadId", "Nombre", model.EntidadId);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.EntidadId = new SelectList(_repository.GetAll<Entidad>().OrderBy(q => q.Nombre), "EntidadId", "Nombre");

            var model = _repository.GetById<DefinicionProceso>(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DefinicionProceso model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.DefinicionProcesoUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            ViewBag.EntidadId = new SelectList(_repository.GetAll<Entidad>().OrderBy(q => q.Nombre), "EntidadId", "Nombre", model.EntidadId);

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<DefinicionProceso>(id);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseCore(_repository);
            var _UseCaseResponseMessage = _useCaseInteractor.DefinicionProcesoDelete(id);

            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            return RedirectToAction("Index");
        }
    }
}