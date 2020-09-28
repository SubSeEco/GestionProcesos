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
using App.Infrastructure.Folio;

namespace App.Web.Controllers
{
    [Authorize]
    public class DocumentoBusquedaController : Controller
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

        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IFolio _folio;
        protected readonly IHSM _hsm;
        protected readonly IEmail _email;

        public DocumentoBusquedaController(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IHSM hsm, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
            _email = email;
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
    }
}

