using App.Model.Pasajes;
using App.Model.Core;
using App.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Util;

namespace App.Web.Controllers
{
    [Authorize]
    public class CotizacionDocumentoController : Controller
    {
        protected readonly IFile _file;
        protected readonly IHSM _IHSM;

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

        protected readonly IGestionProcesos _repository;

        public CotizacionDocumentoController(IGestionProcesos repository, IFile pdf, IHSM hsm)
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
            var model = _repository.GetById<CotizacionDocumento>(id);
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

                    var data = _file.BynaryToText(target.ToArray());

                    var doc = new CotizacionDocumento();
                    doc.Fecha = DateTime.Now;
                    doc.Email = email;
                    doc.FileName = file.FileName;
                    doc.File = target.ToArray();
                    //doc.ProcesoId = model.ProcesoId;
                    //doc.WorkflowId = model.WorkflowId;
                    doc.Signed = false;
                    doc.Texto = data.Text;
                    doc.Metadata = data.Metadata;
                    doc.Type = data.Type;
                    doc.TipoPrivacidadId = 1;

                    _repository.Create(doc);
                    _repository.Save();
                }


                //var file = Request.Files[0];
                //var target = new MemoryStream();
                //file.InputStream.CopyTo(target);

                //var data = _file.BynaryToText(target.ToArray());

                //var doc = new Documento();
                //doc.Fecha = DateTime.Now;
                //doc.Email = email;
                //doc.FileName = file.FileName;
                //doc.File = target.ToArray();
                //doc.ProcesoId = model.ProcesoId;
                //doc.WorkflowId = model.WorkflowId;
                //doc.Signed = false;
                //doc.Texto = data.Text;
                //doc.Metadata = data.Metadata;
                //doc.Type = data.Type;
                //doc.TipoPrivacidadId = 1;

                //_repository.Create(doc);
                //_repository.Save();

                TempData["Success"] = "Operación terminada correctamente.";
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }

            return View(model);
        }

        public ActionResult Sign(int DocumentoId)
        {
            var model = _repository.GetById<CotizacionDocumento>(DocumentoId);
            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Sign(CotizacionDocumento model,int? DocumentoId)
        //{
        //    /*Se debe volver a generar el documento si corresponde a cometido para agregar los campos que se han actualizado*/
        //    /*Se verifica que corresponde a un proceso de cometido*/
        //    //var doc = _repository.GetById<Documento>(model.DocumentoId);
        //    //var DefinicionProceso = _repository.GetAll<Proceso>().Where(p => p.ProcesoId == doc.ProcesoId).FirstOrDefault().DefinicionProcesoId;
        //    //if(DefinicionProceso == 10)
        //    //{
        //    //    var IdCom = _repository.GetAll<Cometido>().Where(c => c.ProcesoId == doc.ProcesoId).FirstOrDefault().CometidoId;
        //    //    GeneraDocumento(IdCom);
        //    //}


        //    var email = UserExtended.Email(User);

        //    if (ModelState.IsValid)
        //    {
        //        var _useCaseInteractor = new UseCaseInteractorCustom(_repository, _IHSM);
        //        var _UseCaseResponseMessage = _useCaseInteractor.DocumentoSign(model, email);

        //        if (_UseCaseResponseMessage.Warnings.Count > 0)
        //            TempData["Warning"] = _UseCaseResponseMessage.Warnings;

        //        if (_UseCaseResponseMessage.IsValid)
        //        {
        //            TempData["Success"] = "Operación terminada correctamente.";
        //            return Redirect(Request.UrlReferrer.PathAndQuery);
        //        }

        //        foreach (var item in _UseCaseResponseMessage.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, item);
        //        }
        //        //TempData["Error"] = _UseCaseResponseMessage.Errors;
        //    }

        //    return View(model);
        //}


        public FileResult Download(int id)
        {
            var model = _repository.GetById<CotizacionDocumento>(id);

            if (string.IsNullOrWhiteSpace(model.Type))
                return File(model.File, System.Net.Mime.MediaTypeNames.Application.Octet, model.FileName);
            else
                return File(model.File, model.Type, model.FileName);
        }
        public FileResult Show(int id)
        {
            var model = _repository.GetById<CotizacionDocumento>(id);

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

        //private DTOFileMetadata GetBynary(byte[] pdf)
        //{
        //    var data = new App.Model.DTO.DTOFileMetadata();
        //    var textExtractor = new TextExtractor();
        //    var extract = textExtractor.Extract(pdf);

        //    data.Text = !string.IsNullOrWhiteSpace(extract.Text) ? extract.Text.Trim() : null;
        //    data.Metadata = extract.Metadata.Any() ? string.Join(";", extract.Metadata) : null;
        //    data.Type = extract.ContentType;

        //    return data;
        //}


        //public ActionResult GetBarCode(string Pl_UndCod, string GDTipoIngresoId)
        //{
        //    byte[] imagebyte;

        //    var barcode39 = BarcodeDrawFactory.Code39WithoutChecksum;
        //    var image = barcode39.Draw(string.Concat(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,Pl_UndCod, GDTipoIngresoId), 80);

        //    using (var ms = new MemoryStream())
        //    {
        //        image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        imagebyte = ms.ToArray();
        //    }

        //    return File(imagebyte, "image/png"); 
        //}
    }
}

