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
using RestSharp;
using System.Net;
using javax.swing.text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class FirmaDocumentoGenericoController : Controller
    {
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
        protected readonly IHSM _hsm;
        protected readonly IEmail _email;

        public FirmaDocumentoGenericoController(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IHSM hsm, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
            _email = email;
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

        public ActionResult Create(int? WorkFlowId)
        {
            //ViewBag.TipoDocumentoCodigo = new SelectList(_folio.GetTipoDocumento().Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion");

            ViewBag.TipoDocumentoCodigo = new SelectList(_folio.GetTipoDocumento().Where(c => c.Descripcion.Contains("OTRO")), "Codigo", "Descripcion");

            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new DTOFileUploadCreate
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DTOFileUploadCreate model)
        {
            if (ModelState.IsValid)
            {
                model.Autor = UserExtended.Email(User);

                var tipodocumento = _folio.GetTipoDocumento().FirstOrDefault(q => q.Codigo == model.TipoDocumentoCodigo);
                var target = new MemoryStream();
                model.FileUpload.InputStream.CopyTo(target);

                var _useCaseInteractor = new UseCaseFirmaDocumentoGenerico(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.Insert(new FirmaDocumentoGenerico()
                {
                    FirmaDocumentoGenericoId = model.FirmaDocumentoGenericoId,
                    ProcesoId = model.ProcesoId,
                    WorkflowId = model.WorkflowId,
                    TipoDocumentoCodigo = model.TipoDocumentoCodigo,
                    TipoDocumentoDescripcion = tipodocumento != null ? tipodocumento.Descripcion : "No encontrado",
                    DocumentoSinFirma = target.ToArray(),
                    DocumentoSinFirmaFilename = model.FileUpload.FileName,
                    Observaciones = model.Comentario,
                    Autor = model.Autor,
                    URL = model.URL
                });

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.TipoDocumentoCodigo = new SelectList(_folio.GetTipoDocumento().Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion");

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var firma = _repository.GetById<FirmaDocumentoGenerico>(id);
            ViewBag.TipoDocumentoCodigo = new SelectList(_folio.GetTipoDocumento().Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion", firma.TipoDocumentoCodigo);

            var model = new DTOFileUploadEdit()
            {
                FirmaDocumentoGenericoId = firma.FirmaDocumentoGenericoId,
                ProcesoId = firma.ProcesoId,
                WorkflowId = firma.WorkflowId,
                File = firma.DocumentoSinFirma,
                Comentario = firma.Observaciones,
                URL = firma.URL
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DTOFileUploadEdit model)
        {
            if (ModelState.IsValid)
            {
                model.Autor = UserExtended.Email(User);

                var tipodocumento = _folio.GetTipoDocumento().FirstOrDefault(q => q.Codigo == model.TipoDocumentoCodigo);

                var target = new MemoryStream();
                if (model.FileUpload != null)
                    model.FileUpload.InputStream.CopyTo(target);

                var _useCaseInteractor = new UseCaseFirmaDocumentoGenerico(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.Edit(new FirmaDocumentoGenerico()
                {
                    FirmaDocumentoGenericoId = model.FirmaDocumentoGenericoId,
                    ProcesoId = model.ProcesoId,
                    WorkflowId = model.WorkflowId,
                    TipoDocumentoCodigo = model.TipoDocumentoCodigo,
                    TipoDocumentoDescripcion = tipodocumento != null ? tipodocumento.Descripcion : "No encontrado",
                    DocumentoSinFirma = model.FileUpload != null ? target.ToArray() : null,
                    DocumentoSinFirmaFilename = model.FileUpload != null ? model.FileUpload.FileName : null,
                    URL = model.URL,
                    Observaciones = model.Comentario,
                    Autor = model.Autor,
                }); ;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Edit", "FirmaDocumentoGenerico", new { id = model.FirmaDocumentoGenericoId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.TipoDocumentoCodigo = new SelectList(_folio.GetTipoDocumento().Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion", model.TipoDocumentoCodigo);

            return View(model);
        }

        public ActionResult Sign(int id)
        {
            var firma = _repository.GetById<FirmaDocumentoGenerico>(id);
            var email = UserExtended.Email(User);

            var model = new DTOFileUploadEdit()
            {
                FirmaDocumentoGenericoId = firma.FirmaDocumentoGenericoId,
                ProcesoId = firma.ProcesoId,
                WorkflowId = firma.WorkflowId,
                File = firma.DocumentoSinFirma,
                Comentario = firma.Observaciones,
                Firmante = email,
                TieneFirma = _repository.GetExists<Rubrica>(q => q.Email == email),
                TipoDocumentoDescripcion = firma.TipoDocumentoDescripcion,
                FechaCreacion = firma.FechaCreacion,
                Autor = firma.Autor,
                Folio = firma.Folio,
                URL = firma.URL
            };


            //var url = "https://api.firma.test.digital.gob.cl/firma/v2/files/tickets";
            //var client = new RestClient(url);
            //var request = new RestRequest(Method.POST)
            //{
            //    RequestFormat = DataFormat.Json
            //};

            //request.AddJsonBody(firma);

            //IRestResponse response = client.Execute(request);

            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //{
            //    ModelState.AddModelError(string.Empty, "Error de Conexión a Gestión de Procesos");
            //}

            //if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    try
            //    {
            //        var returnValue = JsonConvert.DeserializeObject<byte>(response.Content);
            //        return RedirectToAction("Sign", "FirmaDocumentoGenerico", new { id = model.FirmaDocumentoGenericoId });
            //    }
            //    catch (Exception ex)
            //    {
            //        ModelState.AddModelError(String.Empty, "Error al Deserializar los Datos");
            //    }
            //}


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sign(FirmaDocumentoGenerico model)
        {
            if (ModelState.IsValid)
            {
                model.Firmante = UserExtended.Email(User);

                var _useCaseInteractor = new UseCaseFirmaDocumentoGenerico(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.Sign(model.FirmaDocumentoGenericoId, new List<string> { model.Firmante }, model.Firmante);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Sign", "FirmaDocumentoGenerico", new { id = model.FirmaDocumentoGenericoId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return RedirectToAction("Sign", "FirmaDocumentoGenerico", new { id = model.FirmaDocumentoGenericoId });
        }

        public FileResult ShowDocumentoSinFirma(int id)
        {
            var model = _repository.GetById<FirmaDocumentoGenerico>(id);
            return File(model.DocumentoSinFirma, "application/pdf");
        }

        //[HttpPost]
        //public ActionResult Token(int id)
        //{
        //    var model = _repository.GetById<FirmaDocumentoGenerico>(id);

        //    //var doc = _repository.GetAll<Documento>().Where(c => c.ProcesoId == model.ProcesoId/* && c.TipoDocumentoId == 8*/).FirstOrDefault();
        //    var doc = model.DocumentoSinFirma;

        //    DateTime issuedAt = DateTime.Now;

        //    DateTime expires = DateTime.Now.AddMinutes(30);

        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    JwtHeader head = new JwtHeader
        //    {
        //        {"typ", "JWT" },
        //        {"alg", "HS256" }
        //    };
        //    JwtPayload body = new JwtPayload
        //    {
        //            { "purpose", "Propósito General" },
        //            { "entity", "Subsecretaría General de La Presidencia" },
        //            { "run", "11111111"},
        //            { "expiration", "2016-06-15T17:31:00" },
        //            //{ "purpose", "Propósito General" },
        //            //{ "entity", "Subsecretaría de Economía y Empresas de Menor Tamaño" },
        //            //{ "expiration", expires },
        //            ////{ "run", _sigper.GetUserByEmail(model.Autor).Funcionario.RH_NumInte }
        //            //{ "run", "13540749" }

        //    };

        //    var tokenString = new JwtSecurityToken(head, body);

        //    var token = tokenHandler.WriteToken(tokenString);
        //    var api_token_key = "sandbox";
        //    //var api_token_key = "5620e6ec-a7dc-4237-adf5-9080ae754536";

        //    var url = "https://api.firma.test.digital.gob.cl/firma/v2/files/tickets";
        //    var client = new RestClient(url);
        //    var request = new RestRequest(Method.POST)
        //    {
        //        RequestFormat = DataFormat.Json
        //    };

        //    request.AddJsonBody(api_token_key);
        //    request.AddJsonBody(token);
        //    request.AddJsonBody(doc);

        //    IRestResponse response = client.Execute(request);

        //    if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //    {
        //        ModelState.AddModelError(String.Empty, "Error de Conexión a Gestión de Procesos");
        //    }
        //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        try
        //        {
        //            var returnValue = JsonConvert.DeserializeObject<string>(response.Content);
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError(String.Empty, "Error al Deserializar los Datos");
        //        }
        //    }

        //    //return View(model);
        //    return View(response);

        //    //var returnValue = JsonConvert.DeserializeObject<int>(response.Content);

        //    //return Request.CreateResponse(HttpStatusCode.OK, fileName);
        //}

        public ActionResult Token(int id)
        {
            var model = _repository.GetById<FirmaDocumentoGenerico>(id);

            //var doc = _repository.GetAll<Documento>().Where(c => c.ProcesoId == model.ProcesoId/* && c.TipoDocumentoId == 8*/).FirstOrDefault();
            var doc = model.DocumentoSinFirma;

            DateTime issuedAt = DateTime.Now;

            DateTime expires = DateTime.Now.AddMinutes(30);

            var tokenHandler = new JwtSecurityTokenHandler();

           JwtHeader head = new JwtHeader
            {
                {"typ", "JWT" },
                {"alg", "HS256" }
            };
            JwtPayload body = new JwtPayload
            {
                    { "purpose", "Desatendido" },
                    { "entity", "Subsecretaría General de La Presidencia" },
                    { "run", "22222222"},
                    //{ "expiration", "2016-06-15T17:31:00" },
                    { "expiration", expires }
                    //{ "purpose", "Propósito General" },
                    //{ "entity", "Subsecretaría de Economía y Empresas de Menor Tamaño" },
                    //{ "expiration", expires },
                    ////{ "run", _sigper.GetUserByEmail(model.Autor).Funcionario.RH_NumInte }
                    //{ "run", "13540749" }

            };

            var tokenString = new JwtSecurityToken(head, body);

            //var token = tokenHandler.WriteToken(tokenString);

            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbnRpdHkiOiJTdWJzZWNyZXRhcsOtYSBHZW5lcmFsIGRlIExhIFByZXNpZGVuY2lhIiwicnVuIjoiMjIyMjIyMjIiLCJleHBpcmF0aW9uIjoiMjAyMC0xMi0xNFQxMzowMDowMCIsInB1cnBvc2UiOiJEZXNhdGVuZGlkbyJ9.49Yy6lGYSCMqi7UCV9O3esXCSeVJa6zc_CZn8dvfnsw";

            var api_token_key = "sandbox";
            //var api_token_key = "5620e6ec-a7dc-4237-adf5-9080ae754536";
            var secreto = "abcd";

            var persona = _sigper.GetUserByEmail(User.Email());

            //var rut = persona.Funcionario.RH_NumInte.ToString().Trim();

            var rut = "22222222";

            var file = model.DocumentoSinFirma;

            //var url = "https://api.firma.test.digital.gob.cl/firma/v2/files/tickets";
            var url = "https://sintegridadwebtest.economia.cl/api/FileAPI/UploadFiles";
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            //request.AddJsonBody(api_token_key);
            //request.AddJsonBody(token);
/*            request.AddJsonBody(doc)*/;
            //request.AddJsonBody(secreto);
            request.AddJsonBody(rut);
            request.AddJsonBody(file);

            IRestResponse response = client.Execute(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ModelState.AddModelError(String.Empty, "Error de Conexión a Gestión de Procesos");
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var returnValue = JsonConvert.DeserializeObject<string>(response.Content);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty, "Error al Deserializar los Datos");
                }
            }

            

            //return View(model);

            return View(response.StatusDescription);

            //var returnValue = JsonConvert.DeserializeObject<int>(response.Content);

            //return Request.CreateResponse(HttpStatusCode.OK, fileName);

            //return RedirectToAction("Sign", "FirmaDocumentoGenerico", new { model.WorkflowId, id = model.FirmaDocumentoGenericoId });

        }
    }
}