using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using App.Model.Core;
using App.Core.Interfaces;
using System.Linq;
using System;
using System.Text;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
    public class ProcesoConsultorController : Controller
    {
        public class DTOResult
        {
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
                Select = new HashSet<Model.DTO.DTOSelect>();
                Result = new HashSet<DTOResult>();
            }

            [Display(Name = "Ingrese datos conocidos del proceso")]
            public string TextSearch { get; set; }

            [Display(Name = "Creado desde")]
            [DataType(DataType.Date)]
            public DateTime? Desde { get; set; }

            [Display(Name = "Creado hasta")]
            [DataType(DataType.Date)]
            public DateTime? Hasta { get; set; }

            [Display(Name = "Incluir los siguientes tipos de procesos")]
            public IEnumerable<Model.DTO.DTOSelect> Select { get; set; }
            public IEnumerable<DTOResult> Result { get; set; }
            [Display(Name = "Estado del proceso")]
            public int? EstadoProcesoId { get; set; }
        }

        private readonly IGestionProcesos _repository;

        public ProcesoConsultorController(IGestionProcesos repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            ViewBag.EstadoProcesoId = new SelectList(_repository.Get<EstadoProceso>(), "EstadoProcesoId", "Descripcion");

            var model = new DTOFilter() {
                Select = _repository.Get<DefinicionProceso>(q => q.Habilitado).OrderBy(q => q.Nombre).ToList().Select(q => new Model.DTO.DTOSelect() { Id = q.DefinicionProcesoId, Descripcion = q.Nombre, Selected = false }),
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DTOFilter model)
        {
            if (string.IsNullOrWhiteSpace(model.TextSearch)
                && !model.Desde.HasValue
                && !model.Desde.HasValue
                && !model.Select.Any(q => q.Selected)
                && !model.EstadoProcesoId.HasValue)
                ModelState.AddModelError(string.Empty, "Debe especificar al menos un filtro de búsqueda");

            if (ModelState.IsValid)
            {
                using (var context = new Infrastructure.GestionProcesos.AppContext())
                {
                    StringBuilder query = new StringBuilder("SELECT * FROM CoreProceso WHERE 1=1");

                    if (model.Desde.HasValue)
                        query.Append(string.Format(" AND (DAY(FechaCreacion) >= {0} AND MONTH(FechaCreacion) >= {1} AND YEAR(FechaCreacion) >= {2})", model.Desde.Value.Day, model.Desde.Value.Month, model.Desde.Value.Year));

                    if (model.Hasta.HasValue)
                        query.Append(string.Format(" AND (DAY(FechaCreacion) <= {0} AND MONTH(FechaCreacion) <= {1} AND YEAR(FechaCreacion) <= {2})", model.Hasta.Value.Day, model.Hasta.Value.Month, model.Hasta.Value.Year));

                    if (model.EstadoProcesoId.HasValue)
                        query.Append(string.Format(" AND EstadoProcesoId = {0}", model.EstadoProcesoId.Value));

                    var DefinicionProcesoId = model.Select.Where(q => q.Selected).Select(q => q.Id).ToList();
                    if (DefinicionProcesoId.Any())
                        query.Append(string.Format(" AND DefinicionProcesoId IN ({0})", string.Join(",", DefinicionProcesoId)));

                    if (!string.IsNullOrWhiteSpace(model.TextSearch))
                        for (int i = 0; i < model.TextSearch.Split().Count(); i++)
                            if (!string.IsNullOrWhiteSpace(model.TextSearch.Split()[i]))
                                query.Append(string.Format(" AND CONTAINS(Tags,'{0}')", model.TextSearch.Split()[i].Trim()));

                    var email = UserExtended.Email(User);

                    model.Result = context.Proceso.SqlQuery(query.ToString()).Select(q => new DTOResult
                    {
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

            ViewBag.EstadoProcesoId = new SelectList(_repository.Get<EstadoProceso>(), "EstadoProcesoId", "Descripcion", model.EstadoProcesoId);

            return View(model);
        }
    }
}