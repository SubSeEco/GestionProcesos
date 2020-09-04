using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using App.Model.Core;
using App.Core.Interfaces;
using System.Linq;
using App.Util;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class ProcesoConsultorController : Controller
    {
        public class DTOFilter
        {
            public DTOFilter()
            {
                TextSearch = string.Empty;
                Select = new HashSet<App.Model.DTO.DTOSelect>();
                Result = new HashSet<Proceso>();
            }

            [Display(Name = "Texto de búsqueda")]
            public string TextSearch { get; set; }

            [Display(Name = "Desde")]
            [DataType(DataType.Date)]
            public System.DateTime? Desde { get; set; }

            [Display(Name = "Hasta")]
            [DataType(DataType.Date)]
            public System.DateTime? Hasta { get; set; }

            [Display(Name = "Tipos de proceso")]
            public IEnumerable<App.Model.DTO.DTOSelect> Select { get; set; }
            public IEnumerable<Proceso> Result { get; set; }
            [Display(Name = "Estado")]
            public int? EstadoProcesoId { get; set; }

        }

        protected readonly IGestionProcesos _repository;
        protected readonly IEmail _email;

        public ProcesoConsultorController(IGestionProcesos repository, IEmail email)
        {
            _repository = repository;
            _email = email;
        }


        public ActionResult Index()
        {
            ViewBag.EstadoProcesoId = new SelectList(_repository.Get<EstadoProceso>(), "EstadoProcesoId", "Descripcion");

            var model = new DTOFilter()
            {
                Select = _repository.GetAll<DefinicionProceso>().Where(q => q.Habilitado).OrderBy(q => q.Nombre).ToList().Select(q => new App.Model.DTO.DTOSelect() { Id = q.DefinicionProcesoId, Descripcion = q.Nombre, Selected = false }),
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DTOFilter model)
        {
            var predicate = PredicateBuilder.True<Proceso>();

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(model.TextSearch))
                    predicate = predicate.And(q => q.ProcesoId.ToString().Contains(model.TextSearch) || q.Observacion.Contains(model.TextSearch) || q.Email.Contains(model.TextSearch));

                if (model.Desde.HasValue)
                    predicate = predicate.And(q =>
                        q.FechaCreacion.Year >= model.Desde.Value.Year &&
                        q.FechaCreacion.Month >= model.Desde.Value.Month &&
                        q.FechaCreacion.Day >= model.Desde.Value.Day);

                if (model.Hasta.HasValue)
                    predicate = predicate.And(q =>
                        q.FechaCreacion.Year <= model.Hasta.Value.Year &&
                        q.FechaCreacion.Month <= model.Hasta.Value.Month &&
                        q.FechaCreacion.Day <= model.Hasta.Value.Day);

                var DefinicionProcesoId = model.Select.Where(q => q.Selected).Select(q => q.Id).ToList();
                if (DefinicionProcesoId.Any())
                    predicate = predicate.And(q => DefinicionProcesoId.Contains(q.DefinicionProcesoId));

                if (model.EstadoProcesoId.HasValue)
                    predicate = predicate.And(q => q.EstadoProcesoId == model.EstadoProcesoId);

                model.Result = _repository.Get(predicate);
            }
            ViewBag.EstadoProcesoId = new SelectList(_repository.Get<EstadoProceso>(), "EstadoProcesoId", "Descripcion", model.EstadoProcesoId);

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Proceso>(id);
            return View(model);
        }
    }
}