using App.Model.Core;
using App.Core.Interfaces;
using System.Web.Mvc;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class ConfiguracionController : Controller
    {
        protected readonly IGestionProcesos _repository;

        public ConfiguracionController(IGestionProcesos repository)
        {

            
            _repository = repository;
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<Configuracion>();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Configuracion>(id);
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<Configuracion>(id);
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }


        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Configuracion model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.ConfiguracionUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            return View(model);
        }
    }
}