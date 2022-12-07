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
using ExpressiveAnnotations.Attributes;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
    public class GDInternoController : Controller
    {
        public class DTOFileUploadFEA
        {
            public int ProcesoId { get; set; }
            public int WorkflowId { get; set; }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Archivo")]
            [DataType(DataType.Upload)]
            public HttpPostedFileBase[] File { get; set; }

            [Display(Name = "Es documento oficial?")]
            public bool EsOficial { get; set; } 

            [Display(Name = "Tiene firma electrónica?")]
            public bool TieneFirmaElectronica { get; set; }

            [Display(Name = "Requiere firma electrónica?")]
            public bool RequiereFirmaElectronica { get; set; }

            [RequiredIf("RequiereFirmaElectronica", ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Unidad del firmante")]
            public string FirmanteUnidadCodigo { get; set; }

            [Display(Name = "Unidad del firmante")]
            public string FirmanteUnidadDescripcion { get; set; }

            [RequiredIf("RequiereFirmaElectronica", ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Usuario firmante")]
            public string FirmanteEmail { get; set; }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Tipo documento")]
            public string TipoDocumentoCodigo { get; set; }

            [Display(Name = "Descripcion")]
            public string Descripcion { get; set; }
        }

        private readonly IGestionProcesos _repository;
        private readonly ISigper _sigper;
        private readonly IFile _file;
        private readonly IFolio _folio;
        private readonly IEmail _email;

        public GDInternoController(IGestionProcesos repository, ISigper sigper, IFile file, IFolio folio, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _email = email;
        }

        public ActionResult Details(int id)
        {
            //es autoridad
            var email = UserExtended.Email(User);
            var autoridades = _repository.GetFirst<Configuracion>(q => q.Nombre == Util.Enum.Configuracion.autoridades.ToString());
            if (autoridades != null && !string.IsNullOrWhiteSpace(autoridades.Valor) && autoridades.Valor.Contains(email))
                return RedirectToAction("DetailsAutoridad", new { id });

            var model = _repository.GetById<GD>(id);
            return View(model);
        }

        public ActionResult DetailsAutoridad(int id)
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
                IngresoExterno = false
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GD model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseGD(_repository, _file, _sigper, _email);
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
                var _useCaseInteractor = new UseCaseGD(_repository, _file, _sigper, _email);
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
            ViewBag.FirmanteUnidadCodigo = new SelectList(_sigper.GetUnidadesFirmantes(_repository.Get<Rubrica>(q => q.HabilitadoFirma).Select(q => q.Email.Trim()).ToList()), "Pl_UndCod", "Pl_UndDes");
            ViewBag.FirmanteEmail = new SelectList(new List<Model.Sigper.PEDATPER>().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList(), "Email", "Nombre");

            var model = new DTOFileUploadFEA() { ProcesoId = ProcesoId, WorkflowId = WorkflowId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FEAUpload(DTOFileUploadFEA model)
        {
            ViewBag.TipoDocumentoCodigo = new SelectList(_folio.GetTipoDocumento().Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion");
            ViewBag.FirmanteUnidadCodigo = new SelectList(_sigper.GetUnidadesFirmantes(_repository.Get<Rubrica>(q => q.HabilitadoFirma).Select(q => q.Email.Trim()).ToList()), "Pl_UndCod", "Pl_UndDes");
            ViewBag.FirmanteEmail = new SelectList(new List<Model.Sigper.PEDATPER>().Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList(), "Email", "Nombre");

            var email = UserExtended.Email(User);

            if (Request.Files.Count == 0)
                ModelState.AddModelError(string.Empty, "Debe adjuntar un archivo.");

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var documento = new Documento();
                    documento.Fecha = DateTime.Now;
                    documento.Email = email;
                    documento.ProcesoId = model.ProcesoId;
                    documento.WorkflowId = model.WorkflowId;
                    documento.TipoPrivacidadId = (int)Util.Enum.Privacidad.Privado;
                    documento.TipoDocumentoFirma = model.TipoDocumentoCodigo;
                    documento.EsOficial = model.EsOficial;
                    documento.Signed = model.TieneFirmaElectronica;
                    documento.RequiereFirmaElectronica = model.RequiereFirmaElectronica;
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
                        var size = target.Length;
                        if(size> 62914560)
                        {
                            ModelState.AddModelError(string.Empty, "El archivo " + file.FileName + " excede el maximo de 50 MB.");
                        }
                    }

                    _repository.Create(documento);
                }

            if (ModelState.IsValid)
            {
                _repository.Save();
                TempData["Success"] = "Operación terminada correctamente.";
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }
            else
            {
                foreach (var error in ModelState.Values)
                {
                    for (int i = 0; i < error.Errors.Count; i++)
                    {
                        var errorModel = error.Errors[i];
                        if (errorModel != null)
                        {
                            var help = new List<string>();
                            help.Add(errorModel.ErrorMessage);
                            TempData["Error"] = help;
                        }
                    }
                }
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }

            return View(model);
        }

        public ActionResult FEADocumentos(int ProcesoId)
        {
            var email = UserExtended.Email(User);

            var model = _repository.Get<Documento>(q => q.ProcesoId == ProcesoId && q.Activo);
            foreach (var item in model)
            {
                item.AutorizadoParaFirma = item.FirmanteEmail == email;
                item.AutorizadoParaEliminar = item.Email == email;
            }

            return View(model);
        }

        public PartialViewResult Workflow(int ProcesoId)
        {
            var model = _repository.Get<Workflow>(q => q.ProcesoId == ProcesoId);
            return PartialView(model);
        }

        public ActionResult Documents(int ProcesoId)
        {
            var model = _repository.Get<Documento>(q => q.ProcesoId == ProcesoId);
            return View(model);
        }

        public PartialViewResult WorkflowAutoridad(int ProcesoId)
        {
            var model = _repository.Get<Workflow>(q => q.ProcesoId == ProcesoId && q.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada);
            return PartialView(model);
        }
    }
}