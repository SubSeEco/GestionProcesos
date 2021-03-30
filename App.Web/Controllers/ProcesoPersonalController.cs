using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using App.Model.Core;
using App.Core.Interfaces;
using App.Core.UseCases;
using App.Util;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
    public class ProcesoPersonalController : Controller
    {
        public class DTODelete
        {
            public DTODelete()
            {
                    
            }

            public int ProcesoId { get; set; }


            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Justificación")]
            [DataType(DataType.MultilineText)]
            public string JustificacionAnulacion { get; set; }
        }


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
        protected readonly ISIGPER _sigper;

        public ProcesoPersonalController(IGestionProcesos repository, IEmail email, ISIGPER sigper)
        {
            _repository = repository;
            _email = email;
            _sigper = sigper;
        }

        public ActionResult Index()
        {
            ViewBag.EstadoProcesoId = new SelectList(_repository.Get<EstadoProceso>(), "EstadoProcesoId", "Descripcion");

            var email = UserExtended.Email(User);
            var model = new DTOFilter()
            {
                Select = _repository.GetAll<DefinicionProceso>().Where(q => q.Habilitado).OrderBy(q => q.Nombre).ToList().Select(q => new App.Model.DTO.DTOSelect() { Id = q.DefinicionProcesoId, Descripcion = q.Nombre, Selected = false }),
                Result = _repository.Get<Proceso>(q => q.Email == email).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DTOFilter model)
        {
            var email = UserExtended.Email(User);

            var predicate = PredicateBuilder.True<Proceso>();

            if (ModelState.IsValid)
            {
                predicate = predicate.And(q => q.Email == email);

                if (!string.IsNullOrWhiteSpace(model.TextSearch))
                    predicate = predicate.And(q => q.ProcesoId.ToString().Contains(model.TextSearch) || q.Observacion.Contains(model.TextSearch) || q.Email.Contains(model.TextSearch));

                if (model.Desde.HasValue)
                    predicate = predicate.And(q =>
                        q.FechaCreacion.Year >= model.Desde.Value.Year &&
                        q.FechaCreacion.Month >= model.Desde.Value.Month &&
                        q.FechaCreacion.Day >= model.Desde.Value.Day);

                if (model.Hasta.HasValue)
                    predicate = predicate.And(q =>
                        q.FechaCreacion.Year <= model.Desde.Value.Year &&
                        q.FechaCreacion.Month <= model.Desde.Value.Month &&
                        q.FechaCreacion.Day <= model.Desde.Value.Day);

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

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<Proceso>(id);
            return View(new DTODelete {ProcesoId = model.ProcesoId });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DTODelete model)
        {
            var _useCaseInteractor = new UseCaseCore(_repository, _email);
            var _UseCaseResponseMessage = _useCaseInteractor.ProcesoDelete(model.ProcesoId, model.JustificacionAnulacion);

            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            return RedirectToAction("Index");
        }
    }
}