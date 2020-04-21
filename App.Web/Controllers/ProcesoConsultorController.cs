using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using App.Model.Core;
using App.Core.Interfaces;

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

            public IEnumerable<App.Model.DTO.DTOSelect> Select { get; set; }
            public IEnumerable<Proceso> Result { get; set; }
        }

        protected readonly IGestionProcesos _repository;
        protected readonly IEmail _email;

        public ProcesoConsultorController(IGestionProcesos repository, IEmail email)
        {
            _repository = repository;
            _email = email;
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Proceso>(id);
            return View(model);
        }
    }
}