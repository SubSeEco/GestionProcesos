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
using App.Model.Helper;
using App.Util;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class GDExternoController : Controller
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
            public string FirmanteUnidadCodigo { get; set; }

            [Display(Name = "Unidad del firmante")]
            public string FirmanteUnidadDescripcion { get; set; }

            [RequiredIf("RequiereFirmaElectronica", true, ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Usuario firmante")]
            public string FirmanteEmail { get; set; }

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
        protected readonly IEmail _email;
        public GDExternoController(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _email = email;
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<GD>(id);
            return View(model);
        }

        public ActionResult View(int id)
        {
            var model = _repository.GetFirst<GD>(q => q.ProcesoId == id);
            if (model == null)
                return RedirectToAction("View", "Proceso", new { id });

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
                IngresoExterno = true
            };

            ViewBag.GDOrigenId = new SelectList(_repository.GetAll<GDOrigen>(), "GDorigenId", "Descripcion");
            ViewBag.DestinoUnidadCodigo = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.DestinoFuncionarioEmail = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre").OrderBy(q => q.Text);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GD model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseGD(_repository, _file, _folio, _sigper, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.Insert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.GDOrigenId = new SelectList(_repository.GetAll<GDOrigen>(), "GDorigenId", "Descripcion");
            ViewBag.DestinoUnidadCodigo = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            if (model.DestinoUnidadCodigo != null && model.DestinoUnidadCodigo.IsInt())
                ViewBag.DestinoFuncionarioEmail = new SelectList(_sigper.GetUserByUnidad(model.DestinoUnidadCodigo.ToInt()).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre").OrderBy(q => q.Text);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<GD>(id);

            ViewBag.GDOrigenId = new SelectList(_repository.GetAll<GDOrigen>(), "GDorigenId", "Descripcion");
            ViewBag.DestinoUnidadCodigo = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            if (model.DestinoUnidadCodigo != null && model.DestinoUnidadCodigo.IsInt())
                ViewBag.DestinoFuncionarioEmail = new SelectList(_sigper.GetUserByUnidad(model.DestinoUnidadCodigo.ToInt()).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre").OrderBy(q => q.Text);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GD model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseGD(_repository, _file, _folio, _sigper, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.Update(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.GDOrigenId = new SelectList(_repository.GetAll<GDOrigen>(), "GDorigenId", "Descripcion");
            ViewBag.DestinoUnidadCodigo = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.DestinoUnidadCodigo);
            if (model.DestinoUnidadCodigo != null && model.DestinoUnidadCodigo.IsInt())
                ViewBag.DestinoFuncionarioEmail = new SelectList(_sigper.GetUserByUnidad(model.DestinoUnidadCodigo.ToInt()).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre").OrderBy(q => q.Text);

            return View(model);
        }


        public ActionResult FEAUpload(int ProcesoId, int WorkflowId)
        {
            ViewBag.TipoDocumentoCodigo = new SelectList(_folio.GetTipoDocumento().Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion");
            ViewBag.FirmanteUnidadCodigo = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.FirmanteEmail = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");

            var model = new DTOFileUploadFEA() { ProcesoId = ProcesoId, WorkflowId = WorkflowId };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FEAUpload(DTOFileUploadFEA model)
        {
            ViewBag.TipoDocumentoCodigo = new SelectList(_folio.GetTipoDocumento().Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion");
            ViewBag.FirmanteUnidadCodigo = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.FirmanteEmail = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");

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
                    documento.FirmanteUnidad = model.FirmanteUnidadCodigo;
                    documento.FirmanteEmail = !string.IsNullOrWhiteSpace(model.FirmanteEmail) ? model.FirmanteEmail.Trim() : null;
                    documento.Descripcion = model.Descripcion;

                    //contenido
                    var file = Request.Files[i];
                    if (file != null)
                    {
                        var target = new MemoryStream();
                        file.InputStream.CopyTo(target);
                        documento.FileName = file.FileName;
                        documento.File = target.ToArray();

                        //metadata
                        var metadata = _file.BynaryToText(target.ToArray());
                        if (metadata != null)
                        {
                            documento.Texto = metadata.Text;
                            documento.Metadata = metadata.Metadata;
                            documento.Type = metadata.Type;
                        }
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
            var model = _repository.Get<Documento>(q => q.ProcesoId == ProcesoId && q.Activo);
            foreach (var item in model)
                item.AutorizadoParaFirma = item.FirmanteEmail == email;

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteDocument(int id)
        {
            var document = _repository.GetById<Documento>(id);
            if (document != null)
            {
                document.Activo = false;
                _repository.Save();
                TempData["Success"] = "Operación terminada correctamente.";
            }

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }
    }
}