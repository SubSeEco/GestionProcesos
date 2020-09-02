using System.Web.Mvc;
using App.Model.Core;
using App.Model.GestionDocumental;
using App.Core.Interfaces;
using App.Core.UseCases;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class GDController : Controller
    {
        public class DTOFileUploadFEA
        {
            public DTOFileUploadFEA()
            {
            }

            public int ProcesoId { get; set; }
            public int WorkflowId { get; set; }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Archivo")]
            [DataType(DataType.Upload)]
            public HttpPostedFileBase[] File { get; set; }

            [Display(Name = "Requiere firma electrónica?")]
            public bool RequiereFirmaElectronica { get; set; } = false;

            [Display(Name = "Es documento oficial?")]
            public bool EsOficial { get; set; } = false;

            [RequiredIf("RequiereFirmaElectronica", true, ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Unidad del firmante")]
            public string Pl_UndCod { get; set; }

            [Display(Name = "Unidad del firmante")]
            public string Pl_UndDes { get; set; }

            [RequiredIf("RequiereFirmaElectronica", true, ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Usuario firmante")]
            public string UsuarioFirmante { get; set; }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Tipo documento")]
            public string TipoDocumentoCodigo { get; set; }

            [Display(Name = "Descripcion")]
            public string Descripcion { get; set; }
        }


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


        public ActionResult FEAUpload(int ProcesoId, int WorkflowId)
        {
            ViewBag.TipoDocumentoCodigo = new SelectList(_folio.GetTipoDocumento().Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion");
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.UsuarioFirmante = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");

            var model = new DTOFileUploadFEA() { ProcesoId = ProcesoId, WorkflowId = WorkflowId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FEAUpload(DTOFileUploadFEA model)
        {
            ViewBag.TipoDocumentoCodigo = new SelectList(_folio.GetTipoDocumento().Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion");
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.UsuarioFirmante = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");

            var email = UserExtended.Email(User);

            if (Request.Files.Count == 0)
                ModelState.AddModelError(string.Empty, "Debe adjuntar un archivo.");

            if (ModelState.IsValid)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var documento = new Documento();
                    documento.Fecha = DateTime.Now;
                    documento.Email = email;
                    documento.ProcesoId = model.ProcesoId;
                    documento.WorkflowId = model.WorkflowId;
                    documento.Signed = false;
                    documento.TipoPrivacidadId = (int)App.Util.Enum.Privacidad.Privado;
                    documento.TipoDocumentoFirma = model.TipoDocumentoCodigo;
                    documento.RequiereFirmaElectronica = model.RequiereFirmaElectronica;
                    documento.EsOficial = model.EsOficial;
                    documento.FirmanteUnidad = model.Pl_UndCod;
                    documento.FirmanteEmail = !string.IsNullOrWhiteSpace(model.UsuarioFirmante) ? model.UsuarioFirmante.Trim() : null;
                    documento.Descripcion = model.Descripcion;

                    //contenido
                    var file = Request.Files[i];
                    var target = new MemoryStream();
                    if (target != null)
                    {
                        file.InputStream.CopyTo(target);
                        documento.FileName = file.FileName;
                        documento.File = target.ToArray();
                    }

                    //metadata
                    var metadata = _file.BynaryToText(target.ToArray());
                    if (metadata != null)
                    {
                        documento.Texto = metadata.Text;
                        documento.Metadata = metadata.Metadata;
                        documento.Type = metadata.Type;
                    }

                    _repository.Create(documento);
                    _repository.Save();
                }

                TempData["Success"] = "Operación terminada correctamente.";
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }

            return View(model);
        }

        public ActionResult FEADocumentos(int ProcesoId)
        {
            var email = UserExtended.Email(User);

            var model = _repository.Get<Documento>(q => q.ProcesoId == ProcesoId);
            foreach (var item in model)
                item.AutorizadoParaFirma = item.FirmanteEmail == email;

            return View(model);
        }
    }
}