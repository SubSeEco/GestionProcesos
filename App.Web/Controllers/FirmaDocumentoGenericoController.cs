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
using System.Web.Security;
using App.Model.DTO;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
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
            public DateTime? FechaCreacion { get; set; }

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
            public DateTime? FechaCreacion { get; set; }

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
            string subsecretaria = persona.SubSecretaria.Trim();

            if (ModelState.IsValid)
            {
                model.FechaCreacion = DateTime.Now;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FirmaDocumentoGenerico model/*, HttpPostedFileBase file*/)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            string rut = persona.Funcionario.RH_NumInte.ToString().Trim();

            string nombre = persona.Funcionario.PeDatPerChq.Trim();

            string subsecretaria = persona.SubSecretaria.Trim();

            string email = persona.Funcionario.Rh_Mail.Trim();

            if (Request.Files.Count == 0)
                ModelState.AddModelError(string.Empty, "Debe adjuntar un archivo.");

            //var doc = ConvertToByte(file);

 

            if (ModelState.IsValid)
            {
                //model.Archivo = doc;
                model.Run = rut;
                model.Nombre = nombre;
                model.Subsecretaria = subsecretaria;
                model.Email = email;

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    var target = new MemoryStream();
                    file.InputStream.CopyTo(target);

                    var doc = new Documento();
                    doc.Fecha = DateTime.Now;
                    doc.Email = email;
                    doc.FileName = file.FileName;
                    doc.File = target.ToArray();
                    doc.ProcesoId = model.ProcesoId;
                    doc.WorkflowId = model.WorkflowId;
                    doc.Signed = false;
                    doc.TipoPrivacidadId = (int)App.Util.Enum.Privacidad.Privado;
                    doc.TipoDocumentoId = 15; /*Por default el tipo de documento es "Otros"*/

                    

                    //obtener metadata del documento
                    var metadata = _file.BynaryToText(target.ToArray());
                    if (metadata != null)
                    {
                        doc.Texto = metadata.Text;
                        doc.Metadata = metadata.Metadata;
                        doc.Type = metadata.Type;
                    }

                    ///*Se define el tipo de documento de acuerdo a la tarea dentro del proceso de cometido*/
                    //var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
                    //if (workflowActual != null)
                    //{
                    //    if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                    //    {
                    //        if (workflowActual.DefinicionWorkflow.Secuencia == 16)/*analista contabilidad*/
                    //        {
                    //            doc.TipoDocumentoId = 4;
                    //            doc.TipoDocumentoFirma = "OTRO";
                    //        }
                    //        else if (workflowActual.DefinicionWorkflow.Secuencia == 18)/*analista tesoreria*/
                    //        {
                    //            doc.TipoDocumentoId = 5;
                    //            doc.TipoDocumentoFirma = "OTRO";
                    //        }
                    //        else if (workflowActual.DefinicionWorkflow.Secuencia == 9)/*jefatura ppto*/
                    //        {
                    //            doc.TipoDocumentoId = 7;
                    //            doc.TipoDocumentoFirma = "OTRO";
                    //        }
                    //    }
                    //}

                    _repository.Create(doc);
                    _repository.Save();
                }

                var _useCaseInteractor = new UseCaseFirmaDocumentoGenerico(_repository, _sigper, _file, _folio, _email, _minsegpres);
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

        public ActionResult FEADocumentos(int ProcesoId)
        {
            var email = UserExtended.Email(User);

            var model = _repository.Get<Documento>(q => q.ProcesoId == ProcesoId && q.Activo);
            foreach (var item in model)
            {
                item.AutorizadoParaFirma = item.FirmanteEmail == email;
                //item.AutorizadoParaEliminar = item.Email == email;
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

            if (ModelState.IsValid)
            {
                model.FechaCreacion = DateTime.Now;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FirmaDocumentoGenerico model/*, HttpPostedFileBase file*/)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            //var doc = ConvertToByte(file);

            string rut = persona.Funcionario.RH_NumInte.ToString().Trim();

            string nombre = persona.Funcionario.PeDatPerChq.Trim();

            string subsecretaria = persona.SubSecretaria.Trim();

            string email = persona.Funcionario.Rh_Mail.Trim();

            if (ModelState.IsValid)
            {
                //var doc = ConvertToByte(file);

                //model.Archivo = doc;
                model.Run = rut;
                model.Nombre = nombre;
                model.Subsecretaria = subsecretaria;
                model.Email = email;

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    var target = new MemoryStream();
                    file.InputStream.CopyTo(target);

                    var doc = new Documento();
                    doc.Fecha = DateTime.Now;
                    doc.Email = email;
                    doc.FileName = file.FileName;
                    doc.File = target.ToArray();
                    doc.ProcesoId = model.ProcesoId;
                    doc.WorkflowId = model.WorkflowId;
                    doc.Signed = false;
                    doc.TipoPrivacidadId = (int)App.Util.Enum.Privacidad.Privado;
                    doc.TipoDocumentoId = 15; /*Por default el tipo de documento es "Otros"*/



                    //obtener metadata del documento
                    var metadata = _file.BynaryToText(target.ToArray());
                    if (metadata != null)
                    {
                        doc.Texto = metadata.Text;
                        doc.Metadata = metadata.Metadata;
                        doc.Type = metadata.Type;
                    }

                    ///*Se define el tipo de documento de acuerdo a la tarea dentro del proceso de cometido*/
                    //var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
                    //if (workflowActual != null)
                    //{
                    //    if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                    //    {
                    //        if (workflowActual.DefinicionWorkflow.Secuencia == 16)/*analista contabilidad*/
                    //        {
                    //            doc.TipoDocumentoId = 4;
                    //            doc.TipoDocumentoFirma = "OTRO";
                    //        }
                    //        else if (workflowActual.DefinicionWorkflow.Secuencia == 18)/*analista tesoreria*/
                    //        {
                    //            doc.TipoDocumentoId = 5;
                    //            doc.TipoDocumentoFirma = "OTRO";
                    //        }
                    //        else if (workflowActual.DefinicionWorkflow.Secuencia == 9)/*jefatura ppto*/
                    //        {
                    //            doc.TipoDocumentoId = 7;
                    //            doc.TipoDocumentoFirma = "OTRO";
                    //        }
                    //    }
                    //}

                    _repository.Create(doc);                   
                    _repository.Save();
                }

                var _useCaseInteractor = new UseCaseFirmaDocumentoGenerico(_repository, _sigper, _file, _folio, _email, _minsegpres);
                var _UseCaseResponseMessage = _useCaseInteractor.Update(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    //return Redirect(Request.UrlReferrer.PathAndQuery);
                    return RedirectToAction("GeneraDocumento2", "FirmaDocumentoGenerico", new { model.WorkflowId, id = model.FirmaDocumentoGenericoId });
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
            var model = _repository.GetById<FirmaDocumentoGenerico>(id);

            //permiso = persona.a

            var permiso = _repository.GetExists<Usuario>(q => q.Habilitado && q.Email == model.Email && q.Grupo.Nombre.Contains(App.Util.Enum.Grupo.Cometido.ToString()));

            var permisoEspecial = _repository.GetExists<Usuario>(q => q.Habilitado && q.Email == model.Email && q.Grupo.Nombre.Contains(App.Util.Enum.Grupo.Administrador.ToString()));

            if (permisoEspecial == true)
            {
                permisoEspecial = model.permisoEspecial;
            }

            //if (model.TiposFirmas == "DESATENDIDA")
            //{
            //    return RedirectToAction("FirmaDesatendida", "FirmaDocumentoGenerico", new { model.WorkflowId, id = model.FirmaDocumentoGenericoId });
            //}
            //else
            //{
            //var documasivo = new SelectList(_repository.Get<Documento>().Where(c => c.ProcesoId == model.ProcesoId));
            var archivos = _repository.Get<Documento>().Where(c => c.ProcesoId == model.ProcesoId).Select(q => q.File).ToList();

                return View(model);
            //}
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FirmaAtendida(FirmaDocumentoGenerico model, HttpPostedFileBase file)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            //var doc = ConvertToByte(file);

            string rut = persona.Funcionario.RH_NumInte.ToString().Trim();

            string nombre = persona.Funcionario.PeDatPerChq.Trim();
           
            //var docugenerico = _repository.GetFirst<Documento>(d => d.ProcesoId == model.ProcesoId);
         
            var docugenerico = _repository.GetFirst<Documento>(d => d.ProcesoId == model.ProcesoId);

            var archivos = _repository.Get<Documento>().Where(c => c.ProcesoId == model.ProcesoId).Select(q => q.File).ToArray();

            if (docugenerico != null)
            {
                if (docugenerico.DocumentoId == docugenerico.DocumentoId)
                    model.DocumentoId = docugenerico.DocumentoId;
            }

            if (ModelState.IsValid)
            {
                //var doc = ConvertToByte(file);

                //model.Archivo = doc;
                model.DocumentoId = docugenerico.DocumentoId;
                model.Run = rut;
                model.Nombre = nombre;
                model.Archivo = docugenerico.File;

                var _useCaseInteractor = new UseCaseFirmaDocumentoGenerico(_repository, _sigper, _file, _folio, _email, _minsegpres);
                var _UseCaseResponseMessage = _useCaseInteractor.Firma(archivos, model.OTP, null, model.FirmaDocumentoGenericoId, rut, nombre, model.TipoDocumento, model.DocumentoId);
                //var _UseCaseResponseMessage = _useCaseInteractor.Firma(model.Archivo, model.OTP, null, model.FirmaDocumentoGenericoId, rut, nombre, model.TipoDocumento, model.DocumentoId);

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

        public ActionResult FirmaDesatendida(int id)
        {
            var model = _repository.GetById<FirmaDocumentoGenerico>(id);

            //var documasivo = new SelectList(_repository.Get<Documento>().Where(c => c.ProcesoId == model.ProcesoId));
            var archivos = _repository.Get<Documento>().Where(c => c.ProcesoId == model.ProcesoId).Select(q => q.File).ToList();


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FirmaDesatendida(FirmaDocumentoGenerico model, HttpPostedFileBase file)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            //var doc = ConvertToByte(file);

            string rut = persona.Funcionario.RH_NumInte.ToString().Trim();

            string nombre = persona.Funcionario.PeDatPerChq.Trim();

            //var docugenerico = _repository.GetFirst<Documento>(d => d.ProcesoId == model.ProcesoId);

            var docugenerico = _repository.GetFirst<Documento>(d => d.ProcesoId == model.ProcesoId);

            var archivos = _repository.Get<Documento>().Where(c => c.ProcesoId == model.ProcesoId).Select(q => q.File).ToArray();

            if (docugenerico != null)
            {
                if (docugenerico.DocumentoId == docugenerico.DocumentoId)
                    model.DocumentoId = docugenerico.DocumentoId;
            }

            if (ModelState.IsValid)
            {
                //var doc = ConvertToByte(file);

                //model.Archivo = doc;
                //model.DocumentoId = docugenerico.DocumentoId;

                model.Run = rut;
                model.Nombre = nombre;
                model.Archivo = docugenerico.File;

                var _useCaseInteractor = new UseCaseFirmaDocumentoGenerico(_repository, _sigper, _file, _folio, _email, _minsegpres);
                var _UseCaseResponseMessage = _useCaseInteractor.FirmaMasiva(archivos, model.OTP, null, model.FirmaDocumentoGenericoId, rut, nombre, model.TipoDocumentos, model.DocumentoId);
                //var _UseCaseResponseMessage = _useCaseInteractor.Firma(model.Archivo, model.OTP, null, model.FirmaDocumentoGenericoId, rut, nombre, model.TipoDocumento, model.DocumentoId);

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

        public ActionResult DocumentoFirmado(int id)
        {
            var model = _repository.GetById<FirmaDocumentoGenerico>(id);

            //var documasivo = new SelectList(_repository.Get<Documento>().Where(c => c.ProcesoId == model.ProcesoId));
            //var archivos = _repository.Get<Documento>().Where(c => c.ProcesoId == model.ProcesoId).Select(q => q.File).ToList();


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

        public FileResult ShowDocumentoCon2Firma(int id)
        {
            var model = _repository.GetById<FirmaDocumentoGenerico>(id);
            return File(model.ArchivoFirmado2, "application/pdf");
        }
        

        public ActionResult GeneraDocumento(int id)
        {
            byte[] pdf = null;
            DTOFileMetadata data = new DTOFileMetadata();
            //int tipoDoc = 0;
            //int IdDocto = 0;
            string Name = string.Empty;
            var model = _repository.GetById<FirmaDocumentoGenerico>(id);
            var Workflow = _repository.Get<Workflow>(q => q.WorkflowId == model.WorkflowId).FirstOrDefault();

            /*Se genera certificado de viatico*/
            Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Documento Genérico nro", new { id = model.FirmaDocumentoGenericoId }) { FileName = "Documento Genérico" + ".pdf", /*Cookies = cookieCollection,*/ FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
            //pdf = resultPdf.BuildFile(ControllerContext);
            pdf = model.Archivo;
            //data = GetBynary(pdf);
            data = _file.BynaryToText(pdf);
            //tipoDoc = 15;
            Name = "Documento Genérico nro" + " " + model.FirmaDocumentoGenericoId.ToString() + ".pdf";
            int idDoctoViatico = 0;

            ///*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
            //var cdpViatico = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId);
            //if (cdpViatico != null)
            //{
            //    foreach (var res in cdpViatico)
            //    {
            //        if (res.TipoDocumentoId == 2)
            //            idDoctoViatico = res.DocumentoId;
            //    }
            //}

            if (idDoctoViatico == 0)
            {
                /*se guarda certificado de viatico*/
                var email = UserExtended.Email(User);
                var doc = new Documento();
                doc.Fecha = DateTime.Now;
                doc.Email = email;
                doc.FileName = Name;
                doc.File = pdf;
                doc.ProcesoId = model.ProcesoId;
                doc.WorkflowId = model.WorkflowId;
                doc.Signed = false;
                doc.Texto = data.Text;
                doc.Metadata = data.Metadata;
                doc.Type = data.Type;
                doc.TipoPrivacidadId = 1;
                //doc.TipoDocumentoId = tipoDoc;

                //doc.File = model.Archivo;
                //doc.DocumentoId = model.DocumentoId;

                _repository.Create(doc);
                //_repository.Update(doc);
                _repository.Save();
            }
            else
            {
                /*se guarda certificado de viatico*/
                var email = UserExtended.Email(User);
                var doc = new Documento();
                doc.Fecha = DateTime.Now;
                doc.Email = email;
                doc.FileName = Name;
                doc.File = pdf;
                doc.ProcesoId = model.ProcesoId;
                doc.WorkflowId = model.WorkflowId;
                doc.Signed = false;
                doc.Texto = data.Text;
                doc.Metadata = data.Metadata;
                doc.Type = data.Type;
                doc.TipoPrivacidadId = 1;
                //doc.TipoDocumentoId = tipoDoc;

                //doc.File = model.Archivo;
                //doc.DocumentoId = model.DocumentoId;

                _repository.Create(doc);
                //_repository.Update(doc);
                _repository.Save();
            }

            return RedirectToAction("Edit", "FirmaDocumentoGenerico", new { model.WorkflowId, id = model.FirmaDocumentoGenericoId });

            //return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });

            //return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult GeneraDocumento2(int id)
        {
            byte[] pdf = null;
            DTOFileMetadata data = new DTOFileMetadata();
            //int tipoDoc = 0;
            //int IdDocto = 0;
            string Name = string.Empty;
            var model = _repository.GetById<FirmaDocumentoGenerico>(id);
            var Workflow = _repository.Get<Workflow>(q => q.WorkflowId == model.WorkflowId).FirstOrDefault();

            /*Se genera certificado de viatico*/
            Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Documento Genérico nro", new { id = model.FirmaDocumentoGenericoId }) { FileName = "Documento Genérico" + ".pdf", /*Cookies = cookieCollection,*/ FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
            //pdf = resultPdf.BuildFile(ControllerContext);
            pdf = model.Archivo;
            //data = GetBynary(pdf);
            data = _file.BynaryToText(pdf);
            //tipoDoc = 15;
            Name = "Documento Genérico nro" + " " + model.FirmaDocumentoGenericoId.ToString() + ".pdf";
            int idDoctoViatico = 0;

            ///*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
            //var cdpViatico = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId);
            //if (cdpViatico != null)
            //{
            //    foreach (var res in cdpViatico)
            //    {
            //        if (res.TipoDocumentoId == 2)
            //            idDoctoViatico = res.DocumentoId;
            //    }
            //}

            if (idDoctoViatico == 0)
            {
                ///*se guarda certificado de viatico*/
                //var email = UserExtended.Email(User);
                //var doc = new Documento();
                //doc.Fecha = DateTime.Now;
                //doc.Email = email;
                //doc.FileName = Name;
                //doc.File = pdf;
                //doc.ProcesoId = model.ProcesoId.Value;
                //doc.WorkflowId = model.WorkflowId.Value;
                //doc.Signed = false;
                //doc.Texto = data.Text;
                //doc.Metadata = data.Metadata;
                //doc.Type = data.Type;
                //doc.TipoPrivacidadId = 1;
                //doc.TipoDocumentoId = tipoDoc;

                ////doc.File = model.Archivo;
                ////doc.DocumentoId = model.DocumentoId;

                //_repository.Create(doc);
                ////_repository.Update(doc);
                //_repository.Save();

                var docOld = _repository.GetById<Documento>(idDoctoViatico);
                docOld.Fecha = DateTime.Now;
                docOld.File = pdf;
                docOld.Signed = false;
                docOld.Texto = data.Text;
                docOld.Metadata = data.Metadata;
                docOld.Type = data.Type;
                _repository.Update(docOld);
                _repository.Save();
            }
            else
            {
                var docOld = _repository.GetById<Documento>(idDoctoViatico);
                docOld.Fecha = DateTime.Now;
                docOld.File = pdf;
                docOld.Signed = false;
                docOld.Texto = data.Text;
                docOld.Metadata = data.Metadata;
                docOld.Type = data.Type;
                _repository.Update(docOld);
                _repository.Save();
            }

            return RedirectToAction("Edit", "FirmaDocumentoGenerico", new { model.WorkflowId, id = model.FirmaDocumentoGenericoId });

            //return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });

            //return Redirect(Request.UrlReferrer.PathAndQuery);
        }
    }
}