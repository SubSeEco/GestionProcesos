using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Model.Core;
using App.Model.FirmaDocumento;
using App.Core.Interfaces;
using App.Core.UseCases;
using System.ComponentModel.DataAnnotations;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class FirmaDocumentoController : Controller
    {
        public class DTOFileUpload
        {
            public DTOFileUpload()
            {
            }

            public int FirmaDocumentoId { get; set; }
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

            public byte[] File { get; set; }

            public string Firmante { get; set; }

            public bool TieneFirma { get; set; }

            public System.DateTime? FechaCreacion { get; set; }

            [Display(Name = "Folio")]
            public string Folio { get; set; }
        }



        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        //protected readonly ISistemaIntegrado _daes;
        protected readonly IFile _file;
        protected readonly IFolio _folio;
        protected readonly IHSM _hsm;

        static List<DTOTipoDocumento> tipoDocumentoList = null;

        public FirmaDocumentoController(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IHSM hsm)
        {
            _repository = repository;
            _sigper = sigper;
            //_daes = daes;
            _file = file;
            _folio = folio;
            _hsm = hsm;

            if (tipoDocumentoList == null)
                tipoDocumentoList = _folio.GetTipoDocumento();
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<FirmaDocumento>();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<FirmaDocumento>(id);
            return View(model);
        }

        public ActionResult View(int id)
        {
            var model = _repository.GetFirst<FirmaDocumento>(q => q.ProcesoId == id);
            return View(model);
        }

        public ActionResult Validate(int id)
        {
            var model = _repository.GetById<FirmaDocumento>(id);
            return View(model);
        }

        public ActionResult Create(int? WorkFlowId, int? ProcesoId)
        {
            ViewBag.TipoDocumentoCodigo = new SelectList(tipoDocumentoList.Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion");

            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new DTOFileUpload
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DTOFileUpload model)
        {
            if (ModelState.IsValid)
            {
                model.Autor = UserExtended.Email(User);

                var tipodocumento = tipoDocumentoList.FirstOrDefault(q => q.Codigo == model.TipoDocumentoCodigo);
                var target = new MemoryStream();
                model.FileUpload.InputStream.CopyTo(target);

                var _useCaseInteractor = new UseCaseFirmaDocumento(_repository, _file);
                var _UseCaseResponseMessage = _useCaseInteractor.Insert(new FirmaDocumento()
                {
                    FirmaDocumentoId = model.FirmaDocumentoId,
                    ProcesoId = model.ProcesoId,
                    WorkflowId = model.WorkflowId,
                    TipoDocumentoCodigo = model.TipoDocumentoCodigo,
                    TipoDocumentoDescripcion = tipodocumento != null ? tipodocumento.Descripcion : "No encontrado",
                    DocumentoSinFirma = target.ToArray(),
                    DocumentoSinFirmaFilename = model.FileUpload.FileName,
                    Observaciones = model.Comentario,
                    Autor = model.Autor
                });

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.TipoDocumentoCodigo = new SelectList(tipoDocumentoList.Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion");

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var firma = _repository.GetById<FirmaDocumento>(id);
            ViewBag.TipoDocumentoCodigo = new SelectList(tipoDocumentoList.Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion", firma.TipoDocumentoCodigo);

            var model = new DTOFileUpload()
            {
                FirmaDocumentoId = firma.FirmaDocumentoId,
                ProcesoId = firma.ProcesoId,
                WorkflowId = firma.WorkflowId,
                File = firma.DocumentoSinFirma,
                Comentario = firma.Observaciones
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DTOFileUpload model)
        {
            if (ModelState.IsValid)
            {
                model.Autor = UserExtended.Email(User);

                var tipodocumento = tipoDocumentoList.FirstOrDefault(q => q.Codigo == model.TipoDocumentoCodigo);
                var target = new MemoryStream();
                model.FileUpload.InputStream.CopyTo(target);

                var _useCaseInteractor = new UseCaseFirmaDocumento(_repository, _file);
                var _UseCaseResponseMessage = _useCaseInteractor.Edit(new FirmaDocumento()
                {
                    FirmaDocumentoId = model.FirmaDocumentoId,
                    ProcesoId = model.ProcesoId,
                    WorkflowId = model.WorkflowId,
                    TipoDocumentoCodigo = model.TipoDocumentoCodigo,
                    TipoDocumentoDescripcion = tipodocumento != null ? tipodocumento.Descripcion : "No encontrado",
                    DocumentoSinFirma = target.ToArray(),
                    DocumentoSinFirmaFilename = model.FileUpload.FileName,
                    Observaciones = model.Comentario,
                    Autor = model.Autor
                });

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Edit", "FirmaDocumento", new { id = model.FirmaDocumentoId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.TipoDocumentoCodigo = new SelectList(tipoDocumentoList.Select(q => new { q.Codigo, q.Descripcion }), "Codigo", "Descripcion", model.TipoDocumentoCodigo);

            return View(model);
        }

        public ActionResult Sign(int id)
        {
            var firma = _repository.GetById<FirmaDocumento>(id);
            var email = UserExtended.Email(User);

            var model = new DTOFileUpload()
            {
                FirmaDocumentoId = firma.FirmaDocumentoId,
                ProcesoId = firma.ProcesoId,
                WorkflowId = firma.WorkflowId,
                File = firma.DocumentoSinFirma,
                Comentario = firma.Observaciones,
                Firmante = email,
                TieneFirma = _repository.GetExists<Rubrica>(q => q.Email == email),
                TipoDocumentoDescripcion = firma.TipoDocumentoDescripcion,
                FechaCreacion = firma.FechaCreacion,
                Autor = firma.Autor,
                Folio = firma.Folio
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sign(FirmaDocumento model)
        {
            if (ModelState.IsValid)
            {
                model.Firmante = UserExtended.Email(User);

                var _useCaseInteractor = new UseCaseFirmaDocumento(_repository, _sigper, _file, _folio, _hsm);
                var _UseCaseResponseMessage = _useCaseInteractor.Sign(model.FirmaDocumentoId, model.Firmante);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Sign", "FirmaDocumento", new { id = model.FirmaDocumentoId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return RedirectToAction("Sign", "FirmaDocumento", new { id = model.FirmaDocumentoId });
        }



        public FileResult ShowDocumentoSinFirma(int id)
        {
            var model = _repository.GetById<FirmaDocumento>(id);
            return File(model.DocumentoSinFirma, "application/pdf");
        }


        //public FileResult DownloadDocumentoSinFirma(int id)
        //{
        //    var model = _repository.GetById<FirmaDocumento>(id);
        //    return File(model.DocumentoSinFirma, System.Net.Mime.MediaTypeNames.Application.Octet, model.DocumentoSinFirmaFilename);
        //}

        //public FileResult DownloadConFirma(int id)
        //{
        //    var model = _repository.GetById<FirmaDocumento>(id);
        //    return File(model.DocumentoConFirma, System.Net.Mime.MediaTypeNames.Application.Octet, model.DocumentoConFirmaFilename);
        //}
        //public FileResult ShowConFirma(int id)
        //{
        //    var model = _repository.GetById<FirmaDocumento>(id);
        //    return File(model.DocumentoConFirma, "application/pdf");
        //}
    }
}