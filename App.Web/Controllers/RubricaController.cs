using App.Model.Core;
using App.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class RubricaController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        public class FileUpload
        {
            public FileUpload()
            {
            }

            [Display(Name = "Autor")]
            public string Email { get; set; }

            [Display(Name = "Funcionario")]
            public string Funcionario { get; set; }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Archivo")]
            [DataType(DataType.Upload)]
            public HttpPostedFileBase File { get; set; }

            [Display(Name = "Identificador de firma electrónica")]
            public string IdentificadorFirma { get; set; }

            [Display(Name = "Habilitado para firmar")]
            public bool HabilitadoFirma { get; set; }
        }

        public RubricaController(IGestionProcesos repository, ISIGPER sigper)
        {
            _repository = repository;
            _sigper = sigper;
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<Rubrica>();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Rubrica>(id);
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Model.Core.Rubrica model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.RubricaInsert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<Rubrica>(id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Rubrica model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.RubricaUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<Rubrica>(id);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseCore(_repository);
            var _UseCaseResponseMessage = _useCaseInteractor.RubricaDelete(id);

            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            return RedirectToAction("Index");
        }
    }
}
