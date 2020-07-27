using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Model.Core;
using App.Model.GestionDocumental;
using App.Core.Interfaces;
using App.Core.UseCases;
using App.Model.FirmaDocumento;

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
        static List<DTOTipoDocumento> tipoDocumentoList = null;

        public GDController(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;

            if (tipoDocumentoList == null)
                tipoDocumentoList = _folio.GetTipoDocumento();

        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<GD>();
            return View(model);
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

        public ActionResult Validate(int id)
        {
            var model = _repository.GetById<GD>(id);
            return View(model);
        }


        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<GD>(id);
            return View(model);
        }

        public ActionResult Create(int? WorkFlowId, int? ProcesoId)
        {
            ViewBag.TipoDocumentoCodigo = new SelectList(tipoDocumentoList.Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion");
            //ViewBag.GDTipoIngresoId = new SelectList(_repository.Get<GDTipoIngreso>().OrderBy(q => q.Nombre), "GDTipoIngresoId", "Nombre");
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.UsuarioDestino = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            
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
        public ActionResult Create(GD model, string Pl_UndDes)
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

            //ViewBag.GDTipoIngresoId = new SelectList(_repository.Get<GDTipoIngreso>().OrderBy(q => q.Nombre), "GDTipoIngresoId", "Nombre", model.GDTipoIngresoId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.UsuarioDestino = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.Pl_UndCod.HasValue)
                ViewBag.UsuarioDestino = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.UsuarioDestino);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<GD>(id);
            //ViewBag.GDTipoIngresoId = new SelectList(_repository.Get<GDTipoIngreso>().OrderBy(q => q.Nombre), "GDTipoIngresoId", "Nombre", model.GDTipoIngresoId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.UsuarioDestino = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.Pl_UndCod.HasValue)
                ViewBag.UsuarioDestino = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.UsuarioDestino);

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

            //ViewBag.GDTipoIngresoId = new SelectList(_repository.Get<GDTipoIngreso>().OrderBy(q => q.Nombre), "GDTipoIngresoId", "Nombre", model.GDTipoIngresoId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.UsuarioDestino = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.Pl_UndCod.HasValue)
                ViewBag.UsuarioDestino = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.UsuarioDestino);

            return View(model);
        }

        //public JsonResult GetOrganizacion(string term)
        //{
        //    var result = _daes.Get<App.Model.SistemaIntegrado.Organizacion>(q=>q.RazonSocial.Contains(term) || q.NumeroRegistro.ToString().Contains(term)).Select(c => new { id = c.OrganizacionId, value = c.TipoOrganizacion.Nombre + " " + c.NumeroRegistro + " - " + c.RazonSocial }).Take(25).ToList();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
    }
}