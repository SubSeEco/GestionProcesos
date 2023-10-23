using App.Model.Core;
using App.Core.Interfaces;
using System.Linq;
using System.Web.Mvc;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]

    public class DefinicionWorkflowController : Controller
    {
        private readonly IGestionProcesos _repository;
        private readonly ISigper _sigper;

        public DefinicionWorkflowController(IGestionProcesos repository, ISigper sigper)
        {
            _repository = repository;
            _sigper = sigper;
        }

        public ActionResult Create(int DefinicionProcesoId)
        {
            ViewBag.DefinicionProcesoId = new SelectList(_repository.GetAll<DefinicionProceso>(), "DefinicionProcesoId", "Nombre", DefinicionProcesoId);
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>().OrderBy(q=>q.Nombre), "GrupoId", "Nombre");
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.DefinicionWorkflowRechazoId = new SelectList(_repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == DefinicionProcesoId).Select(q => new { Value = q.DefinicionWorkflowId.ToString(), Text = string.Format("{0} - {1}", q.DefinicionWorkflowId, q.Nombre)}), "Value", "Text");
            ViewBag.DefinicionWorkflowDependeDeId = new SelectList(_repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == DefinicionProcesoId).Select(q => new { Value = q.DefinicionWorkflowId.ToString(), Text = string.Format("{0} - {1}", q.DefinicionWorkflowId, q.Nombre) }), "Value", "Text");
            ViewBag.AccionId = new SelectList(_repository.GetAll<Accion>().OrderBy(q => q.Nombre), "AccionId", "Nombre");
            ViewBag.EntidadId = new SelectList(_repository.GetAll<Entidad>().OrderBy(q => q.Nombre), "EntidadId", "Nombre");
            ViewBag.TipoEjecucionId = new SelectList(_repository.Get<TipoEjecucion>(q=>q.Activo).OrderBy(q=>q.Order), "TipoEjecucionId", "Nombre");

            var definicionWorkflows = _repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == DefinicionProcesoId);

            return View(new DefinicionWorkflow() {Secuencia = definicionWorkflows.Any() ? definicionWorkflows.Select(q=>q.Secuencia).Max() + 1 : 1});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DefinicionWorkflow model)
        {
            if (model.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso || model.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso || model.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaJefaturaDeFuncionarioQueViaja)
                model.GrupoId = null;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.DefinicionWorkflowInsert(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Edit", "DefinicionProceso", new { id = model.DefinicionProcesoId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.DefinicionProcesoId = new SelectList(_repository.GetAll<DefinicionProceso>(), "DefinicionProcesoId", "Nombre", model.DefinicionProcesoId);
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>().OrderBy(q => q.Nombre), "GrupoId", "Nombre", model.GrupoId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.DefinicionWorkflowRechazoId = new SelectList(_repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == model.DefinicionProcesoId).Select(q => new { Value = q.DefinicionWorkflowId.ToString(), Text = q.Nombre }), "Value", "Text", model.DefinicionWorkflowRechazoId);
            ViewBag.DefinicionWorkflowDependeDeId = new SelectList(_repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == model.DefinicionProcesoId).Select(q => new { Value = q.DefinicionWorkflowId.ToString(), Text = q.Nombre }), "Value", "Text", model.DefinicionWorkflowDependeDeId);
            ViewBag.AccionId = new SelectList(_repository.GetAll<Accion>().OrderBy(q => q.Nombre), "AccionId", "Nombre", model.AccionId);
            ViewBag.EntidadId = new SelectList(_repository.GetAll<Entidad>().OrderBy(q => q.Nombre), "EntidadId", "Nombre", model.EntidadId);
            ViewBag.TipoEjecucionId = new SelectList(_repository.Get<TipoEjecucion>(q => q.Activo).OrderBy(q => q.Order), "TipoEjecucionId", "Nombre", model.TipoEjecucionId);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<DefinicionWorkflow>(id);

            ViewBag.DefinicionProcesoId = new SelectList(_repository.GetAll<DefinicionProceso>(), "DefinicionProcesoId", "Nombre", model.DefinicionProcesoId);
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>().OrderBy(q => q.Nombre), "GrupoId", "Nombre", model.GrupoId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.DefinicionWorkflowRechazoId = new SelectList(_repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == model.DefinicionProcesoId).Select(q => new { Value = q.DefinicionWorkflowId.ToString(), Text = q.Nombre }), "Value", "Text", model.DefinicionWorkflowRechazoId);
            ViewBag.DefinicionWorkflowDependeDeId = new SelectList(_repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == model.DefinicionProcesoId).Select(q => new { Value = q.DefinicionWorkflowId.ToString(), Text = q.Nombre }), "Value", "Text", model.DefinicionWorkflowDependeDeId);
            ViewBag.AccionId = new SelectList(_repository.GetAll<Accion>().OrderBy(q => q.Nombre), "AccionId", "Nombre", model.AccionId);
            ViewBag.EntidadId = new SelectList(_repository.GetAll<Entidad>().OrderBy(q => q.Nombre), "EntidadId", "Nombre", model.EntidadId);
            ViewBag.TipoEjecucionId = new SelectList(_repository.Get<TipoEjecucion>(q => q.Activo).OrderBy(q => q.Order), "TipoEjecucionId", "Nombre", model.TipoEjecucionId);

            return View(model);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DefinicionWorkflow model)
        {
            if (model.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso || model.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso)
                model.GrupoId = null;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.DefinicionWorkflowUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Edit", "DefinicionProceso", new { id = model.DefinicionProcesoId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.DefinicionProcesoId = new SelectList(_repository.GetAll<DefinicionProceso>(), "DefinicionProcesoId", "Nombre", model.DefinicionProcesoId);
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>().OrderBy(q => q.Nombre), "GrupoId", "Nombre", model.GrupoId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.DefinicionWorkflowRechazoId = new SelectList(_repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == model.DefinicionProcesoId).Select(q => new { Value = q.DefinicionWorkflowId.ToString(), Text = q.Nombre }), "Value", "Text", model.DefinicionWorkflowRechazoId);
            ViewBag.DefinicionWorkflowDependeDeId = new SelectList(_repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == model.DefinicionProcesoId).Select(q => new { Value = q.DefinicionWorkflowId.ToString(), Text = q.Nombre }), "Value", "Text", model.DefinicionWorkflowDependeDeId);
            ViewBag.AccionId = new SelectList(_repository.GetAll<Accion>().OrderBy(q => q.Nombre), "AccionId", "Nombre", model.AccionId);
            ViewBag.EntidadId = new SelectList(_repository.GetAll<Entidad>().OrderBy(q => q.Nombre), "EntidadId", "Nombre", model.EntidadId);
            ViewBag.TipoEjecucionId = new SelectList(_repository.Get<TipoEjecucion>(q => q.Activo).OrderBy(q => q.Order), "TipoEjecucionId", "Nombre", model.TipoEjecucionId);

            return View(model);
        }

        public ActionResult EditRunningTaskDefinition(int id)
        {
            var model = _repository.GetById<DefinicionWorkflow>(id);

            ViewBag.DefinicionProcesoId = new SelectList(_repository.GetAll<DefinicionProceso>(), "DefinicionProcesoId", "Nombre", model.DefinicionProcesoId);
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>().OrderBy(q => q.Nombre), "GrupoId", "Nombre", model.GrupoId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.DefinicionWorkflowRechazoId = new SelectList(_repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == model.DefinicionProcesoId).Select(q => new { Value = q.DefinicionWorkflowId.ToString(), Text = q.Nombre }), "Value", "Text", model.DefinicionWorkflowRechazoId);
            ViewBag.DefinicionWorkflowDependeDeId = new SelectList(_repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == model.DefinicionProcesoId).Select(q => new { Value = q.DefinicionWorkflowId.ToString(), Text = q.Nombre }), "Value", "Text", model.DefinicionWorkflowDependeDeId);

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRunningTaskDefinition(DefinicionWorkflow model, int ProcesoId)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.DefinicionWorkflowUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Details", "Proceso", new { id = ProcesoId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.DefinicionProcesoId = new SelectList(_repository.GetAll<DefinicionProceso>(), "DefinicionProcesoId", "Nombre", model.DefinicionProcesoId);
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>().OrderBy(q => q.Nombre), "GrupoId", "Nombre", model.GrupoId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.DefinicionWorkflowRechazoId = new SelectList(_repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == model.DefinicionProcesoId).Select(q => new { Value = q.DefinicionWorkflowId.ToString(), Text = q.Nombre }), "Value", "Text", model.DefinicionWorkflowRechazoId);
            ViewBag.DefinicionWorkflowDependeDeId = new SelectList(_repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == model.DefinicionProcesoId).Select(q => new { Value = q.DefinicionWorkflowId.ToString(), Text = q.Nombre }), "Value", "Text", model.DefinicionWorkflowDependeDeId);

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<DefinicionWorkflow>(id);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var model = _repository.GetById<DefinicionWorkflow>(id);
            var _useCaseInteractor = new UseCaseCore(_repository, _sigper);
            var _UseCaseResponseMessage = _useCaseInteractor.DefinicionWorkflowDelete(id);

            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            return RedirectToAction("Edit", "DefinicionProceso", new { id = model.DefinicionProcesoId });
        }
    }
}
