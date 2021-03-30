using App.Model.Core;
using App.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]

    public class UsuarioController : Controller
    {
        protected readonly IGestionProcesos _repository;
        private static List<App.Model.DTO.DTODomainUser> ActiveDirectoryUsers { get; set; }

        public UsuarioController(IGestionProcesos repository)
        {
            _repository = repository;

            if (ActiveDirectoryUsers == null)
                ActiveDirectoryUsers = AuthenticationService.GetDomainUser().ToList();
        }

        public JsonResult GetUser(string term)
        {
            var result = ActiveDirectoryUsers
               .Where(q => (q.User != null && q.User.ToLower().Contains(term.ToLower())) || (q.Email != null && q.Email.ToLower().Contains(term.ToLower())))
               .Take(25)
               .Select(c => new { id = c.Email, value = string.Format("{0} ({1})", c.User, c.Email) })
               .OrderBy(q => q.value)
               .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = _repository.Get<Usuario>(q=>q.Habilitado);
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Usuario>(id);
            return View(model);
        }

        public ActionResult Create(int id)
        {
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>().OrderBy(q=>q.Nombre), "GrupoId", "Nombre", id);

            return View(new Usuario() { GrupoId = id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.UsuarioInsert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index", "Grupo");
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>().OrderBy(q => q.Nombre), "GrupoId", "Nombre", model.GrupoId);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<Usuario>(id);
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>().OrderBy(q => q.Nombre), "GrupoId", "Nombre");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Usuario model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.UsuarioUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index", "Grupo");
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>().OrderBy(q => q.Nombre), "GrupoId", "Nombre", model.GrupoId);

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<Usuario>(id);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var model = _repository.GetById<Usuario>(id);
            var _useCaseInteractor = new UseCaseCore(_repository);
            var _UseCaseResponseMessage = _useCaseInteractor.UsuarioDelete(id);

            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            return RedirectToAction("Index", "Grupo");
        }
    }
}
