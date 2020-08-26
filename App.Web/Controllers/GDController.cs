using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Model.Core;
using App.Model.GestionDocumental;
using App.Core.Interfaces;
using App.Core.UseCases;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.IO;
using System;

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
        public class FileUpload
        {
            public FileUpload()
            {
            }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Archivo")]
            [DataType(DataType.Upload)]
            public HttpPostedFileBase[] File { get; set; }

            public int ProcesoId { get; set; }
            public int WorkflowId { get; set; }
        }


        public GDController(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
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
            //ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            //ViewBag.UsuarioFirmante = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");

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

            //ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            //ViewBag.UsuarioFirmante = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            
            //if (model.Pl_UndCod.HasValue)
            //    ViewBag.UsuarioDestino = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.UsuarioFirmante);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<GD>(id);
            //ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            //ViewBag.UsuarioFirmante = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            //if (model.Pl_UndCod.HasValue)
            //    ViewBag.UsuarioFirmante = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.UsuarioFirmante);

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

            //ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            //ViewBag.UsuarioFirmante = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            //if (model.Pl_UndCod.HasValue)
            //    ViewBag.UsuarioFirmante = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.UsuarioFirmante);

            return View(model);
        }
    }
}