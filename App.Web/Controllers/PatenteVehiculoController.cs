using App.Core.Interfaces;
using App.Core.UseCases;
using App.Model.Cometido;
using App.Model.Comisiones;
using App.Model.Shared;
using System.Linq;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
    public class PatenteVehiculoController : Controller
    {
        private readonly IGestionProcesos _repository;
        private readonly ISigper _sigper;

        public PatenteVehiculoController(IGestionProcesos repository, ISigper sigper)
        {
            _repository = repository;
            _sigper = sigper;
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<PatenteVehiculo>();

            return View(model);
        }

        public ActionResult Create()
        {
            var model = new PatenteVehiculo();

            ViewBag.TipoVehiculo = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.SIGPERTipoVehiculoId);
            /*ViewBag.ListaRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim()).OrderBy(c => c.Value);*/
            ViewBag.ListaRegion = new SelectList(_repository.Get<Region>(), "Codigo", "Nombre".Trim()).OrderBy(c => c.Value);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(PatenteVehiculo model)
        {
            var _useCaseInteractor = new UseCaseCometidoComision(_repository);
            model.RegionId = int.Parse(model.Codigo);
            var _UseCaseResponseMessage = _useCaseInteractor.PatenteVehiculoInsert(model);

            if (_UseCaseResponseMessage.IsValid)
            {
                TempData["Success"] = "Operación terminada exitosamente.";
                return RedirectToAction("Index");
            }

            ViewBag.TipoVehiculo = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.SIGPERTipoVehiculoId);
            //ViewBag.ListaRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim()).OrderBy(c => c.Value);
            ViewBag.ListaRegion = new SelectList(_repository.Get<Region>(), "Codigo", "Nombre".Trim()).OrderBy(c => c.Value);
            TempData["Error"] = _UseCaseResponseMessage.Errors;
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<PatenteVehiculo>(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(PatenteVehiculo model)
        {
            var _useCaseInteractor = new UseCaseCometidoComision(_repository);
            var _useCaseResponseMessage = _useCaseInteractor.PatenteVehiculoDelete(model.PatenteVehiculoId);
            if (_useCaseResponseMessage.IsValid)
            {
                TempData["Success"] = "Operacion terminada exitosamente.";
                return RedirectToAction("Index");
            }

            ViewBag.ListaRegion = new SelectList(_repository.Get<Region>(), "Codigo", "Nombre".Trim()).OrderBy(c => c.Value);
            ViewBag.TipoVehiculo = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.SIGPERTipoVehiculoId);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<PatenteVehiculo>(id);
            ViewBag.ListaRegion = new SelectList(_repository.Get<Region>(), "Codigo", "Nombre".Trim()).OrderBy(c => c.Value);
            ViewBag.TipoVehiculo = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.SIGPERTipoVehiculoId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PatenteVehiculo model)
        {
            var _useCaseInteractor = new UseCaseCometidoComision(_repository);
            model.RegionId = int.Parse(model.Codigo);
            var _UseCaseResponseMessage = _useCaseInteractor.PatenteVehiculoUpdate(model);
            if (_UseCaseResponseMessage.IsValid)
            {
                TempData["Success"] = "Operación terminada exitosamente.";
                return RedirectToAction("Index");
            }
            ViewBag.ListaRegion = new SelectList(_repository.Get<Region>(), "Codigo", "Nombre".Trim()).OrderBy(c => c.Value);
            ViewBag.TipoVehiculo = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.SIGPERTipoVehiculoId);
            TempData["Error"] = _UseCaseResponseMessage.Errors;
            return RedirectToAction("Index");
        }
    }
}