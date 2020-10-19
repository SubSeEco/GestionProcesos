using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using App.Model.Core;
using App.Core.Interfaces;
using System.Linq;
using App.Util;
using System;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class ProcesoConsultorController : Controller
    {
        public class DTOResult
        {
            public DTOResult()
            {
            }

            public int ProcesoId { get; set; }
            public bool Reservado { get; set; }
            public bool EsAutor { get; set; }
            public string Email { get; set; }
            public string NombreFuncionario { get; set; }
            public DateTime FechaCreacion { get; set; }
            public string Estado { get; set; }
            public string Observacion { get; set; }
            public string Definicion { get; set; }
            public string Entidad { get; set; }
        }

        public class DTOFilter
        {
            public DTOFilter()
            {
                TextSearch = string.Empty;
                Select = new HashSet<App.Model.DTO.DTOSelect>();
                Result = new HashSet<DTOResult>();
            }

            [Display(Name = "Ingrese datos conocidos del proceso")]
            public string TextSearch { get; set; }

            [Display(Name = "Creado desde")]
            [DataType(DataType.Date)]
            public System.DateTime? Desde { get; set; }

            [Display(Name = "Creado hasta")]
            [DataType(DataType.Date)]
            public System.DateTime? Hasta { get; set; }

            [Display(Name = "Incluir los siguientes tipos de procesos")]
            public IEnumerable<App.Model.DTO.DTOSelect> Select { get; set; }
            public IEnumerable<DTOResult> Result { get; set; }
            [Display(Name = "Estado del proceso")]
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
            var email = UserExtended.Email(User);
            
            ViewBag.EstadoProcesoId = new SelectList(_repository.Get<EstadoProceso>(), "EstadoProcesoId", "Descripcion", model.EstadoProcesoId);

            if (ModelState.IsValid)
            {
                var predicate = PredicateBuilder.True<Proceso>();

                if (!string.IsNullOrWhiteSpace(model.TextSearch))
                    foreach (var item in model.TextSearch.Split())
                        predicate = predicate.And(q => q.Tags.Contains(item));

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

                using (var context = new App.Infrastructure.GestionProcesos.AppContext())
                {
                    model.Result = context.Proceso.Where(predicate).Select(q => new DTOResult {
                        ProcesoId = q.ProcesoId,
                        Reservado = q.Reservado,
                        EsAutor = q.Email.Trim() == email.Trim(),
                        Email = q.Email,
                        NombreFuncionario = q.NombreFuncionario,
                        FechaCreacion = q.FechaCreacion,
                        Estado = q.EstadoProceso.Descripcion,
                        Observacion = q.Observacion,
                        Definicion = q.DefinicionProceso.Nombre,
                        Entidad = q.DefinicionProceso.Entidad.Codigo
                    }).ToList();
                }
            }

            return View(model);
        }
    }
}