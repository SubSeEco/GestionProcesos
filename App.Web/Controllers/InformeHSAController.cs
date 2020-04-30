using System;
using System.Web.Mvc;
using App.Model.Core;
using App.Model.InformeHSA;
using App.Core.Interfaces;
using Rotativa.MVC;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class InformeHSAController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;

        public InformeHSAController(IGestionProcesos repository, ISIGPER sigper, IFile file)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
        }

        public ActionResult View(int id)
        {
            var model = _repository.GetFirst<InformeHSA>(q => q.ProcesoId == id);
            if (model == null)
                return RedirectToAction("Details", "Proceso", new { id });

            return View(model);
        }

        public ActionResult Pdf(int id)
        {
            var model = _repository.GetById<InformeHSA>(id);
            model.QR = _file.CreateQR(id.ToString());

            //var email = UserExtended.Email(User);
            //var rubrica = _repository.GetFirst<Rubrica>(q => q.Email == email);
            //if (rubrica != null)
            //    model.Signature = rubrica.File;

            return new ViewAsPdf("pdf", model);
        }

        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<InformeHSA>(id);
            return View(model);
        }

        public ActionResult Create(int WorkFlowId)
        {
            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new InformeHSA()
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
            };

            var persona = _sigper.GetUserByEmail(User.Email());
            if (persona.Funcionario == null)
                ModelState.AddModelError(string.Empty, "No se encontró información del funcionario en SIGPER");
            if (persona.Unidad == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la unidad del funcionario en SIGPER");
            if (persona.Jefatura == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la jefatura del funcionario en SIGPER");

            if (ModelState.IsValid)
            {
                model.RUT = persona.Funcionario.RH_NumInte;
                model.Nombre = persona.Funcionario.PeDatPerChq;
                model.Unidad = persona.Unidad.Pl_UndDes;
                model.NombreJefatura = persona.Jefatura.PeDatPerChq;
                model.FechaDesde = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                model.FechaHasta = model.FechaDesde.Value.AddMonths(1).AddDays(-1);
                model.FechaBoleta = model.FechaHasta;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InformeHSA model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new Core.UseCases.UseCaseInformeHSA(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.Insert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Execute", "Workflow", new { id = model.WorkflowId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<InformeHSA>(id);

            return View(model);
        }
        public ActionResult Details(int id)
        {
            var model = _repository.GetById<InformeHSA>(id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InformeHSA model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new Core.UseCases.UseCaseInformeHSA(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.Update(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            return View(model);
        }
    }
}