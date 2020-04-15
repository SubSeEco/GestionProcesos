using System.Linq;
using System.Web.Mvc;
using App.Model.CDP;
using App.Model.Core;
using App.Model.Shared;
using App.Core.Interfaces;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class CDPController : Controller
    {
        protected readonly IGestionProcesos _repository;

        public CDPController(IGestionProcesos repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<CDP>();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<CDP>(id);
            return View(model);
        }

        public ActionResult Validate(int id)
        {
            var model = _repository.GetById<CDP>(id);
            return View(model);
        }

        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<CDP>(id);
            return View(model);
        }

        public ActionResult Create(int WorkFlowId)
        {
            ViewBag.InstitucionId = new SelectList(_repository.Get<Institucion>().OrderBy(q => q.Nombre), "InstitucionId", "Nombre");
            ViewBag.CDPTipoSolicitudId = new SelectList(_repository.Get<CDPTipoSolicitud>().OrderBy(q => q.Nombre), "CDPTipoSolicitudId", "Nombre");
            ViewBag.CDPBienId = new SelectList(_repository.Get<CDPBien>().OrderBy(q => q.Nombre), "CDPBienId", "Nombre");
            ViewBag.RegionId = new SelectList(_repository.Get<Region>().OrderBy(q => q.Nombre), "RegionId", "Nombre");

            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new CDP {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
                Tarea = workflow.DefinicionWorkflow.Nombre
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CDP model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCDP(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.Insert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.InstitucionId = new SelectList(_repository.Get<Institucion>().OrderBy(q => q.Nombre), "InstitucionId", "Nombre", model.InstitucionId);
            ViewBag.CDPTipoSolicitudId = new SelectList(_repository.Get<CDPTipoSolicitud>().OrderBy(q => q.Nombre), "CDPTipoSolicitudId", "Nombre", model.CDPTipoSolicitudId);
            ViewBag.CDPBienId = new SelectList(_repository.Get<CDPBien>().OrderBy(q => q.Nombre), "CDPBienId", "Nombre", model.CDPBienId);
            ViewBag.RegionId = new SelectList(_repository.Get<Region>().OrderBy(q => q.Nombre), "RegionId", "Nombre", model.RegionId);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<CDP>(id);

            ViewBag.InstitucionId = new SelectList(_repository.Get<Institucion>().OrderBy(q => q.Nombre), "InstitucionId", "Nombre", model.InstitucionId);
            ViewBag.CDPTipoSolicitudId = new SelectList(_repository.Get<CDPTipoSolicitud>().OrderBy(q => q.Nombre), "CDPTipoSolicitudId", "Nombre", model.CDPTipoSolicitudId);
            ViewBag.CDPBienId = new SelectList(_repository.Get<CDPBien>().OrderBy(q => q.Nombre), "CDPBienId", "Nombre", model.CDPBienId);
            ViewBag.RegionId = new SelectList(_repository.Get<Region>().OrderBy(q => q.Nombre), "RegionId", "Nombre", model.RegionId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CDP model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCDP(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.Update(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            ViewBag.InstitucionId = new SelectList(_repository.Get<Institucion>().OrderBy(q => q.Nombre), "InstitucionId", "Nombre", model.InstitucionId);
            ViewBag.CDPTipoSolicitudId = new SelectList(_repository.Get<CDPTipoSolicitud>().OrderBy(q => q.Nombre), "CDPTipoSolicitudId", "Nombre", model.CDPTipoSolicitudId);
            ViewBag.CDPBienId = new SelectList(_repository.Get<CDPBien>().OrderBy(q => q.Nombre), "CDPBienId", "Nombre", model.CDPBienId);
            ViewBag.RegionId = new SelectList(_repository.Get<Region>().OrderBy(q => q.Nombre), "RegionId", "Nombre", model.RegionId);

            return View(model);
        }
    }
}