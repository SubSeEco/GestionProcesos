using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Model.Core;
using App.Model.FirmaDocumentoGenerico;
using App.Core.Interfaces;
using App.Core.UseCases;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using com.sun.tools.@internal.ws.processor.model;
using System.Web.Security;
using App.Model.DTO;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class FirmaDocumentoGenericoController : Controller
    {
        public class EmpModel
        {
            [Required]
            [DataType(DataType.Upload)]
            [Display(Name = "Select File")]
            public HttpPostedFileBase files { get; set; }
        }

        public class DTOFileUploadCreate
        {
            public DTOFileUploadCreate()
            {
            }

            public int FirmaDocumentoGenericoId { get; set; }
            public int ProcesoId { get; set; }
            public int WorkflowId { get; set; }
            [Display(Name = "Comentario")]
            public string Comentario { get; set; }

            public string Autor { get; set; }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Tipo documento")]
            public string TipoDocumentoCodigo { get; set; }

            [Display(Name = "Tipo documento")]
            public string TipoDocumentoDescripcion { get; set; }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Archivo")]
            [DataType(DataType.Upload)]
            public HttpPostedFileBase FileUpload { get; set; }

            [Display(Name = "Documento a firmar")]
            public byte[] File { get; set; }

            public string Firmante { get; set; }

            public bool TieneFirma { get; set; }

            [Display(Name = "Fecha creación")]
            public System.DateTime? FechaCreacion { get; set; }

            [Display(Name = "Folio")]
            public string Folio { get; set; }

            [Display(Name = "URL gestión documental")]
            [DataType(DataType.Url)]
            public string URL { get; set; }
        }

        public class DTOFileUploadEdit
        {
            public DTOFileUploadEdit()
            {
            }

            public int FirmaDocumentoGenericoId { get; set; }
            public int ProcesoId { get; set; }
            public int WorkflowId { get; set; }
            [Display(Name = "Comentario")]
            public string Comentario { get; set; }

            public string OTP { get; set; }
            public string Autor { get; set; }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Tipo documento")]
            public string TipoDocumentoCodigo { get; set; }

            [Display(Name = "Tipo documento")]
            public string TipoDocumentoDescripcion { get; set; }

            //[Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Archivo")]
            [DataType(DataType.Upload)]
            public HttpPostedFileBase FileUpload { get; set; }

            [Display(Name = "Documento a firmar")]
            public byte[] File { get; set; }

            public string Firmante { get; set; }

            public bool TieneFirma { get; set; }

            [Display(Name = "Fecha creación")]
            public System.DateTime? FechaCreacion { get; set; }

            [Display(Name = "Folio")]
            public string Folio { get; set; }

            [Display(Name = "URL gestión documental")]
            [DataType(DataType.Url)]
            public string URL { get; set; }
        }

        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IFolio _folio;
        //protected readonly IHSM _hsm;
        protected readonly IEmail _email;
        protected readonly IMinsegpres _minsegpres;

        public FirmaDocumentoGenericoController(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, /* IHSM hsm,*/ IEmail email, IMinsegpres minsegpres)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            //_hsm = hsm;
            _email = email;
            _minsegpres = minsegpres;
        }

        public ActionResult View(int id)
        {
            var model = _repository.GetFirst<FirmaDocumentoGenerico>(q => q.ProcesoId == id);
            if (model == null)
                return RedirectToAction("View", "Proceso", new { id });

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetFirst<FirmaDocumentoGenerico>(q => q.ProcesoId == id);
            if (model == null)
                return RedirectToAction("Details", "Proceso", new { id });

            return View(model);
        }

        public ActionResult Create(int WorkFlowId)
        {
            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new FirmaDocumentoGenerico()
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
            };

            var persona = _sigper.GetUserByEmail(User.Email());
            if (persona.Funcionario == null)
                ModelState.AddModelError(string.Empty, "No se encontró información del funcionario en SIGPER");
            if (persona.Unidad == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la unidad del funcionario en SIGPER");
            if (persona.Jefatura == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la jefatura del funcionario en SIGPER");

            string rut = persona.Funcionario.RH_NumInte.ToString().Trim();
            string nombre = persona.Funcionario.PeDatPerChq.Trim();

            if (ModelState.IsValid)
            {
                model.FechaCreacion = DateTime.Now;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FirmaDocumentoGenerico model, HttpPostedFileBase file)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            string rut = persona.Funcionario.RH_NumInte.ToString().Trim();

            string nombre = persona.Funcionario.PeDatPerChq.Trim();

            var doc = ConvertToByte(file);

            if (ModelState.IsValid)
            {

                model.Archivo = doc;
                model.Run = rut;
                model.Nombre = nombre;

                var _useCaseInteractor = new Core.UseCases.UseCaseFirmaDocumentoGenerico(_repository, _sigper, _file, _folio, _email, _minsegpres);
                var _UseCaseResponseMessage = _useCaseInteractor.Insert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });
                    //return RedirectToAction("GeneraDocumento", "FirmaDocumentoGenerico", new { model.WorkflowId, id = model.FirmaDocumentoGenericoId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return View(model);
        }

        public byte[] ConvertToByte(HttpPostedFileBase file)
        {
            byte[] imageByte = null;
            BinaryReader rdr = new BinaryReader(file.InputStream);
            imageByte = rdr.ReadBytes((int)file.ContentLength);
            return imageByte;
        }

        public ActionResult Edit(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            var model = _repository.GetById<FirmaDocumentoGenerico>(id);

            //var doc = _repository.GetAll<Documento>().Where(c => c.ProcesoId == model.ProcesoId && c.TipoDocumentoId == 15).FirstOrDefault();

            string rut = persona.Funcionario.RH_NumInte.ToString().Trim();

            string nombre = persona.Funcionario.PeDatPerChq.Trim();

            if (ModelState.IsValid)
            {
                model.FechaCreacion = DateTime.Now;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FirmaDocumentoGenerico model, HttpPostedFileBase file)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            //var doc = ConvertToByte(file);

            string rut = persona.Funcionario.RH_NumInte.ToString().Trim();

            string nombre = persona.Funcionario.PeDatPerChq.Trim();

            if (ModelState.IsValid && file != null)
            {
                var doc = ConvertToByte(file);

                model.Archivo = doc;
                model.Run = rut;
                model.Nombre = nombre;

                var _useCaseInteractor = new UseCaseFirmaDocumentoGenerico(_repository, _sigper, _file, _folio, _email, _minsegpres);
                var _UseCaseResponseMessage = _useCaseInteractor.Update(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                    //return RedirectToAction("Token", "FirmaDocumentoGenerico", new { model.WorkflowId, id = model.FirmaDocumentoGenericoId });
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }
            else
            {
                //var doc = ConvertToByte(file);

                //model.Archivo = doc;

                var _useCaseInteractor = new UseCaseFirmaDocumentoGenerico(_repository, _sigper, _file, _folio, _email, _minsegpres);
                var _UseCaseResponseMessage = _useCaseInteractor.Update(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                    //return RedirectToAction("Token", "FirmaDocumentoGenerico", new { model.WorkflowId, id = model.FirmaDocumentoGenericoId });
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            return View(model);
        }

        public ActionResult FirmaAtendida(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            var model = _repository.GetById<FirmaDocumentoGenerico>(id);

            //var doc = _repository.GetAll<Documento>().Where(c => c.ProcesoId == model.ProcesoId && c.TipoDocumentoId == 15).FirstOrDefault();

            string rut = persona.Funcionario.RH_NumInte.ToString().Trim();

            string nombre = persona.Funcionario.PeDatPerChq.Trim();

            var folio = model.Folio;

            if (ModelState.IsValid)
            {
                model.FechaCreacion = DateTime.Now;

                var response = new ResponseMessage();

                //si el documento ya tiene folio, no solicitarlo nuevamente
                if (string.IsNullOrWhiteSpace(model.Folio))
                {
                    try
                    {
                        //var _folioResponse = _folio.GetFolio(string.Join(", ", emailsFirmantes), firmaDocumento.TipoDocumentoCodigo, persona.SubSecretaria);
                        var _folioResponse = _folio.GetFolio(string.Join(", ", "ereyes@economia.cl"), "MEMO", "ECONOMIA");
                        if (_folioResponse == null)
                            response.Errors.Add("Error al llamar el servicio externo de folio");

                        if (_folioResponse != null && _folioResponse.status == "ERROR")
                            response.Errors.Add(_folioResponse.error);

                        model.Folio = _folioResponse.folio;

                        _repository.Update(model);
                        _repository.Save();
                    }
                    catch (Exception ex)
                    {
                        response.Errors.Add(ex.Message);
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FirmaAtendida(FirmaDocumentoGenerico model, HttpPostedFileBase file)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            //var doc = ConvertToByte(file);

            string rut = persona.Funcionario.RH_NumInte.ToString().Trim();

            string nombre = persona.Funcionario.PeDatPerChq.Trim();

            var folio = model.Folio;

            if (ModelState.IsValid/* && file == null*/)
            {
                //var doc = ConvertToByte(file);

                //model.Archivo = doc;
                model.Run = rut;
                model.Nombre = nombre;
              

                var _useCaseInteractor = new UseCaseFirmaDocumentoGenerico(_repository, _sigper, _file, _folio, _email, _minsegpres);
                //var _UseCaseResponseMessage = _useCaseInteractor.Update(model);
                var _UseCaseResponseMessage = _useCaseInteractor.Firma(model.Archivo, model.OTP, null, model.FirmaDocumentoGenericoId, rut, nombre, model.Folio, model.TipoDocumento);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                    //return RedirectToAction("DocumentoFirmado", "FirmaDocumentoGenerico", new { model.WorkflowId, id = model.FirmaDocumentoGenericoId });
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            return View(model);
        }

        public FileResult ShowDocumentoSinFirma(int id)
        {
            var model = _repository.GetById<FirmaDocumentoGenerico>(id);
            return File(model.Archivo, "application/pdf");
        }

        public FileResult ShowDocumentoConFirma(int id)
        {
            var model = _repository.GetById<FirmaDocumentoGenerico>(id);
            return File(model.ArchivoFirmado, "application/pdf");
        }
    }
}