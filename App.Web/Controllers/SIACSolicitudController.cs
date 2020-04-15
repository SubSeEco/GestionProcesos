using System.Linq;
using System.Web.Mvc;
using App.Model.Core;
using App.Model.SIAC;
using App.Model.Shared;
using App.Core.Interfaces;
using Rotativa.MVC;

namespace App.Web.Controllers
{
    [Audit]
    //[Authorize]
    public class SIACSolicitudController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;

        public SIACSolicitudController(IGestionProcesos repository, ISIGPER sigper, IFile file)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<SIACSolicitud>();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<SIACSolicitud>(id);
            return View(model);
        }

        public ActionResult View(int id)
        {
            var model = _repository.GetById<SIACSolicitud>(id);
            return View(model);
        }

        public ActionResult Pdf(int id)
        {
            var model = _repository.GetById<SIACSolicitud>(id);
            model.QR = _file.CreateQR(id.ToString());

            var email = UserExtended.Email(User);
            var rubrica = _repository.GetFirst<Rubrica>(q=>q.Email == email);
            if (rubrica != null)
                model.Signature = rubrica.File;

            return new ViewAsPdf("pdf", model);
        }

        public ActionResult Create(int WorkFlowId)
        {
            ViewBag.SIACTipoSolicitudId = new SelectList(_repository.Get<SIACTipoSolicitud>().OrderBy(q => q.Nombre), "SIACTipoSolicitudId", "Nombre");
            ViewBag.SIACOcupacionId = new SelectList(_repository.Get<SIACOcupacion>().OrderBy(q => q.Nombre), "SIACOcupacionId", "Nombre");
            ViewBag.SIACTemaId = new SelectList(_repository.Get<SIACTema>().OrderBy(q => q.Nombre), "SIACTemaId", "Nombre");
            ViewBag.GeneroId = new SelectList(_repository.Get<Genero>().OrderBy(q => q.Nombre), "GeneroId", "Nombre");
            ViewBag.RegionId = new SelectList(_repository.Get<Region>().OrderBy(q => q.Nombre), "RegionId", "Nombre");

            var persona = _sigper.GetUserByEmail(User.Email());
            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new SIACSolicitud
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SIACSolicitud model)
        {
            ViewBag.SIACTipoSolicitudId = new SelectList(_repository.Get<SIACTipoSolicitud>().OrderBy(q => q.Nombre), "SIACTipoSolicitudId", "Nombre", model.SIACTipoSolicitudId);
            ViewBag.SIACOcupacionId = new SelectList(_repository.Get<SIACOcupacion>().OrderBy(q => q.Nombre), "SIACOcupacionId", "Nombre", model.SIACOcupacionId);
            ViewBag.SIACTemaId = new SelectList(_repository.Get<SIACTema>().OrderBy(q => q.Nombre), "SIACTemaId", "Nombre", model.SIACTemaId);
            ViewBag.GeneroId = new SelectList(_repository.Get<Genero>().OrderBy(q => q.Nombre), "GeneroId", "Nombre", model.GeneroId);
            ViewBag.RegionId = new SelectList(_repository.Get<Region>().OrderBy(q => q.Nombre), "RegionId", "Nombre", model.Region);

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseInteractor(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.SIACIngresoInsert(model);
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
            var model = _repository.GetById<SIACSolicitud>(id);

            ViewBag.SIACTipoSolicitudId = new SelectList(_repository.Get<SIACTipoSolicitud>().OrderBy(q => q.Nombre), "SIACTipoSolicitudId", "Nombre", model.SIACTipoSolicitudId);
            ViewBag.SIACOcupacionId = new SelectList(_repository.Get<SIACOcupacion>().OrderBy(q => q.Nombre), "SIACOcupacionId", "Nombre", model.SIACOcupacionId);
            ViewBag.SIACTemaId = new SelectList(_repository.Get<SIACTema>().OrderBy(q => q.Nombre), "SIACTemaId", "Nombre", model.SIACTemaId);
            ViewBag.GeneroId = new SelectList(_repository.Get<Genero>().OrderBy(q => q.Nombre), "GeneroId", "Nombre", model.GeneroId);
            ViewBag.RegionId = new SelectList(_repository.Get<Region>().OrderBy(q => q.Nombre), "RegionId", "Nombre", model.Region);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SIACSolicitud model)
        {
            ViewBag.SIACTipoSolicitudId = new SelectList(_repository.Get<SIACTipoSolicitud>().OrderBy(q => q.Nombre), "SIACTipoSolicitudId", "Nombre", model.SIACTipoSolicitudId);
            ViewBag.SIACOcupacionId = new SelectList(_repository.Get<SIACOcupacion>().OrderBy(q => q.Nombre), "SIACOcupacionId", "Nombre", model.SIACOcupacionId);
            ViewBag.SIACTemaId = new SelectList(_repository.Get<SIACTema>().OrderBy(q => q.Nombre), "SIACTemaId", "Nombre", model.SIACTemaId);
            ViewBag.GeneroId = new SelectList(_repository.Get<Genero>().OrderBy(q => q.Nombre), "GeneroId", "Nombre", model.GeneroId);
            ViewBag.RegionId = new SelectList(_repository.Get<Region>().OrderBy(q => q.Nombre), "RegionId", "Nombre", model.Region);

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseInteractor(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.SIACIngresoUpdate(model);
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