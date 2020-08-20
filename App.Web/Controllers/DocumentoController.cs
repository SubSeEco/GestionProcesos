using App.Model.Cometido;
using App.Model.Core;
using App.Core.Interfaces;
using App.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using App.Core.UseCases;
using com.sun.xml.@internal.txw2;
using org.bouncycastle.cert.dane;

namespace App.Web.Controllers
{
    public class DTODocumento
    {
        public DTODocumento()
        {

        }

        public int Id { get; set; }
        public string Archivo { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaEmision { get; set; }
        public bool OK { get; set; }
        public string Error { get; set; }
        public string Autor { get; set; }
        public string Folio { get; set; }
    }


    [Authorize]
    public class DocumentoController : Controller
    {
        protected readonly IGestionProcesos _repository;        
        protected readonly IFile _file;
        protected readonly IHSM _IHSM;
        protected readonly IFolio _folio;

        public class DTOFilter
        {
            public DTOFilter()
            {
                TextSearch = string.Empty;
                Select = new HashSet<App.Model.DTO.DTOSelect>();
                Result = new HashSet<Documento>();
            }

            [Display(Name = "Texto de búsqueda")]
            public string TextSearch { get; set; }

            [Display(Name = "Desde")]
            [DataType(DataType.Date)]
            public System.DateTime? Desde { get; set; }

            [Display(Name = "Hasta")]
            [DataType(DataType.Date)]
            public System.DateTime? Hasta { get; set; }

            public IEnumerable<App.Model.DTO.DTOSelect> Select { get; set; }
            public IEnumerable<Documento> Result { get; set; }
        }

        public class FileUpload
        {
            public FileUpload()
            {
            }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Archivo")]
            [DataType(DataType.Upload)]
            //public HttpPostedFileBase File { get; set; }
            public HttpPostedFileBase[] File { get; set; }

            public int ProcesoId { get; set; }
            public int WorkflowId { get; set; }
        }       

        public DocumentoController(IGestionProcesos repository, IFile pdf, IHSM hsm)
        {
            _repository = repository;
            _file = pdf;
            _IHSM = hsm;
        }

        public ActionResult Index()
        {
            var model = new DTOFilter()
            {
                Select = _repository.GetAll<DefinicionProceso>().OrderBy(q => q.Nombre).ToList().Select(q => new App.Model.DTO.DTOSelect() { Id = q.DefinicionProcesoId, Descripcion = q.Nombre, Selected = false }),
                Result = new List<Documento>()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DTOFilter model)
        {
            var predicate = PredicateBuilder.True<Documento>();

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(model.TextSearch))
                    predicate = predicate.And(q => q.DocumentoId.ToString().Contains(model.TextSearch) || q.Email.ToString().Contains(model.TextSearch) || q.FileName.Contains(model.TextSearch) || q.Texto.Contains(model.TextSearch) || q.Metadata.Contains(model.TextSearch));

                if (model.Desde.HasValue)
                    predicate = predicate.And(q =>
                        q.Fecha.Year >= model.Desde.Value.Year &&
                        q.Fecha.Month >= model.Desde.Value.Month &&
                        q.Fecha.Day >= model.Desde.Value.Day);

                if (model.Hasta.HasValue)
                    predicate = predicate.And(q =>
                        q.Fecha.Year <= model.Desde.Value.Year &&
                        q.Fecha.Month <= model.Desde.Value.Month &&
                        q.Fecha.Day <= model.Desde.Value.Day);

                var DefinicionProcesoId = model.Select.Where(q => q.Selected).Select(q => q.Id).ToList();
                if (DefinicionProcesoId.Any())
                    predicate = predicate.And(q => DefinicionProcesoId.Contains(q.Proceso.DefinicionProcesoId));

                model.Result = _repository.Get(predicate).OrderByDescending(q => q.DocumentoId);
            }

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Documento>(id);
            return View(model);
        }

        public ActionResult Create(int ProcesoId, int WorkflowId)
        {
            var model = new FileUpload() { ProcesoId = ProcesoId, WorkflowId = WorkflowId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FileUpload model)
        {
            var email = UserExtended.Email(User);

            if (Request.Files.Count == 0)
                ModelState.AddModelError(string.Empty, "Debe adjuntar un archivo.");

            if (ModelState.IsValid)
            {
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
                    doc.TipoDocumentoId = 6; /*Por default el tipo de documento es "Otros"*/

                    //obtener metadata del documento
                    var metadata = _file.BynaryToText(target.ToArray());
                    if (metadata != null)
                    {
                        doc.Texto = metadata.Text;
                        doc.Metadata = metadata.Metadata;
                        doc.Type = metadata.Type;
                    }

                    /*Se define el tipo de documento de acuerdo a la tarea dentro del proceso de cometido*/
                    var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId);
                    if (workflowActual != null)
                    {
                        if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                        {
                            if (workflowActual.DefinicionWorkflow.Secuencia == 16)/*analista contabilidad*/
                            {
                                doc.TipoDocumentoId = 4;
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 18)/*analista tesoreria*/
                            {
                                doc.TipoDocumentoId = 5;
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 9)/*jefatura ppto*/
                            {
                                doc.TipoDocumentoId = 7;
                            }
                        }
                    }

                    _repository.Create(doc);
                    _repository.Save();
                }

                TempData["Success"] = "Operación terminada correctamente.";
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }

            return View(model);
        }

        public ActionResult Sign(int DocumentoId)
        {
            var model = _repository.GetById<Documento>(DocumentoId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sign(Documento model, int? DocumentoId)
        {
            /*Se debe volver a generar el documento si corresponde a cometido para agregar los campos que se han actualizado*/
            /*Se verifica que corresponde a un proceso de cometido*/
            //var doc = _repository.GetById<Documento>(model.DocumentoId);
            //var DefinicionProceso = _repository.GetAll<Proceso>().Where(p => p.ProcesoId == doc.ProcesoId).FirstOrDefault().DefinicionProcesoId;
            //if(DefinicionProceso == 10)
            //{
            //    var IdCom = _repository.GetAll<Cometido>().Where(c => c.ProcesoId == doc.ProcesoId).FirstOrDefault().CometidoId;
            //    GeneraDocumento(IdCom);
            //}


            var email = UserExtended.Email(User);

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository, _IHSM);
                var _UseCaseResponseMessage = _useCaseInteractor.DocumentoSign(model, email);
                //var _useCaseInteractor = new UseCaseCometidoComision(_repository, _IHSM, _file, _folio);
                //var _UseCaseResponseMessage = _useCaseInteractor.DocumentoSign(model, email);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
                //TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return View(model);
        }


        public FileResult Download(int id)
        {
            var model = _repository.GetById<Documento>(id);

            if (string.IsNullOrWhiteSpace(model.Type))
                return File(model.File, System.Net.Mime.MediaTypeNames.Application.Octet, model.FileName);
            else
                return File(model.File, model.Type, model.FileName);
        }
        public FileResult Show(int id)
        {
            var model = _repository.GetById<Documento>(id);

            if (string.IsNullOrWhiteSpace(model.Type))
                return File(model.File, System.Net.Mime.MediaTypeNames.Application.Octet);
            else
                return File(model.File, "application/pdf");
        }

        [HttpPost]
        public ActionResult Delete(int id, int WorkflowId)
        {
            var model = _repository.GetById<DefinicionWorkflow>(id);
            return RedirectToAction("Execute", "Workflow", new { id = WorkflowId });
        }

        private void GeneraDocumento(int id)
        {
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }

            var model = _repository.GetById<Cometido>(id);
            Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Pdf", new { id = model.CometidoId }) { FileName = "Resolucion" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
            byte[] pdf = resultPdf.BuildFile(ControllerContext);
            //var data = GetBynary(pdf);
            var data = _file.BynaryToText(pdf);


            /*se busca si existe una resolucion asociada al proceso*/
            int IdDocto = 0;
            var resolucion = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId && d.TipoDocumentoId == 1);
            if (resolucion != null)
            {
                IdDocto = resolucion.FirstOrDefault().DocumentoId;
            }

            /*se guarda el pdf generado como documento adjunto -- se valida si ya existe el documento para actualizar*/
            if (IdDocto == 0)
            {
                var email = UserExtended.Email(User);
                var doc = new Documento();
                doc.Fecha = DateTime.Now;
                doc.Email = email;
                doc.FileName = "Resolucion Cometido nro" + " " + model.CometidoId.ToString() + ".pdf";
                doc.File = pdf;
                doc.ProcesoId = model.ProcesoId.Value;
                doc.WorkflowId = model.WorkflowId.Value;
                doc.Signed = false;
                doc.Texto = data.Text;
                doc.Metadata = data.Metadata;
                doc.Type = data.Type;
                doc.TipoPrivacidadId = (int)App.Util.Enum.Privacidad.Privado;
                doc.TipoDocumentoId = 1;

                _repository.Create(doc);
                _repository.Save();
            }
            else
            {
                var docOld = _repository.GetById<Documento>(IdDocto);
                docOld.File = pdf;
                _repository.Update(docOld);
                _repository.Save();
            }


            //return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        //metodo para verificación de documento
        //el documento debe ser pdf y publico
        [AllowAnonymous]
        public JsonResult GetById(string id)
        {
            if (id.IsNullOrWhiteSpace())
                return Json(new DTODocumento { OK = false, Error = "Error: código no especificado" }, JsonRequestBehavior.AllowGet);

            var documento = new App.Model.Core.Documento();
            if (id.IsInt())
                documento = _repository.GetById<Documento>(id.ToInt());
            else
                documento = _repository.GetFirst<Documento>(q=>q.Folio == id);

            if (documento == null)
                return Json(new DTODocumento { OK = false, Error = "Error: documento no encontrado" }, JsonRequestBehavior.AllowGet);

            if (documento != null && documento.File == null)
                return Json(new DTODocumento { OK = false, Error = "Error: documento encontrado, pero sin contenido" }, JsonRequestBehavior.AllowGet);

            if (documento != null && !documento.FileName.IsNullOrWhiteSpace() && !documento.FileName.Contains(".pdf"))
                return Json(new DTODocumento { OK = false, Error = "Error: documento no es tipo PDF" }, JsonRequestBehavior.AllowGet);

            if (documento != null && documento.TipoPrivacidadId == (int)App.Util.Enum.Privacidad.Privado)
                return Json(new DTODocumento { OK = false, Error = "Error: documento no es público" }, JsonRequestBehavior.AllowGet);

            var model = new DTODocumento() {
                OK = true,
                Id = documento.DocumentoId,
                FechaEmision = documento.Fecha,
                Nombre = documento.FileName,
                Archivo = System.Text.Encoding.Default.GetString(documento.File),
                Autor = documento.Email,
                Folio = documento.Folio,
            };

            return Json(model , JsonRequestBehavior.AllowGet);
        }
    }
}

