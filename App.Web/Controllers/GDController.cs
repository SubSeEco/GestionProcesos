using System.Web.Mvc;
using App.Model.Core;
using App.Model.GestionDocumental;
using App.Core.Interfaces;
using App.Core.UseCases;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class GDController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IFolio _folio;
        public GDController(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<GD>(id);
            return View(model);
        }

        public ActionResult View(int id)
        {
            var model = _repository.GetById<GD>(id);
            return View(model);
        }

        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<GD>(id);
            return View(model);
        }

        public ActionResult Create(int? WorkFlowId)
        {
            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new GD
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GD model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseGD(_repository, _file, _folio);
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

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<GD>(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GD model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseGD(_repository, _file, _folio);
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
    }
}