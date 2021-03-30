using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Model.Pasajes;
using App.Model.Shared;
//using App.Model.Shared;
using App.Core.Interfaces;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
    public class DestinosPasajesController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        private static List<App.Model.DTO.DTODomainUser> ActiveDirectoryUsers { get; set; }
        public static List<DestinosPasajes> ListDestino { get; set; }
        //public class DtoMontos
        //{
        //    public double? Dias100 { get; set; } = 0;
        //    public double? Dias60 { get; set; } = 0;
        //    public double? Dias40 { get; set; } = 0;
        //    public double? Dias50 { get; set; } = 0;
        //    public double? Dias00 { get; set; } = 0;
        //    public double? DiasTotal { get; set; } = 0;
        //}

        public DestinosPasajesController(IGestionProcesos repository, ISIGPER sigper)
        {
            _repository = repository;
            _sigper = sigper;

            if (ActiveDirectoryUsers == null)
                ActiveDirectoryUsers = AuthenticationService.GetDomainUser().ToList();
        }

        //public JsonResult GetCiudad(string IdPais)
        //{
        //    var ciudad = _repository.Get<Ciudad>().Where(p => p.PaisId == int.Parse(IdPais));
        //    return Json(ciudad.Select(q => new { value = q.CiudadId, text = q.CiudadNombre.Trim() }), JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult Viatico(int ComisionId, int CantDias100, int CantDias60, int CantDias40, int CantDias50, int CiudadId)
        //                                    {
        //    DtoMontos valor = CalculoViatico(ComisionId, CantDias100, CantDias60, CantDias40, CantDias50, CiudadId);
        //    return Json(new
        //    {
        //        Dias100Monto = Math.Round(valor.Dias100.Value, 0),
        //        Dias60Monto = Math.Round(valor.Dias60.Value, 0),
        //        Dias40Monto = Math.Round(valor.Dias40.Value, 0),
        //        Dias50Monto = Math.Round(valor.Dias50.Value, 0),
        //        DiasTotal = Math.Round(valor.DiasTotal.Value, 2)}, JsonRequestBehavior.AllowGet);
        //}

        //public DtoMontos CalculoViatico(int ComisionId, int CantDias100, int CantDias60, int CantDias40, int CantDias50, int CiudadId)
        //{
        //    DtoMontos total = new DtoMontos();
        //    double porcentaje60 = 0.6;
        //    double porcentaje40 = 0.4;
        //    double porcentaje50 = 0.5;
        //    var comision = _repository.GetById<Comisiones>(ComisionId);
        //    var viatico = _repository.GetAll<ViaticoInternacional>().Where(v => v.Año == DateTime.Now.Year && v.CiudadId.Value == CiudadId).FirstOrDefault();
        //    var viaticoInternacional = _repository.GetAll<ViaticoInternacional>().Where(v => v.Año == DateTime.Now.Year && v.CiudadId.Value == CiudadId).FirstOrDefault();
        //    var grado = comision.GradoDescripcion;
        //    var CalidadJuridica = comision.CalidadDescripcion;
        //    float monto;
        //    //if (CalidadJuridica == "HONORARIOS") // "CONTRATA")
        //    if (comision.IdGrado == "0") // "CONTRATA")
        //    {
        //        /*Se busca el valor del sueldo bruto para definir el tramo en el que se encuentra*/
        //        var Sueldo = _sigper.GetReContra().Where(s => s.RH_NumInte == comision.NombreId.Value).FirstOrDefault().Re_SuelBas;
        //        if (Sueldo > 0 && Sueldo <= 2004707)
        //        {
        //            /*Tramo C*/
        //            monto = viatico.PorcentajeRango3.Value * viaticoInternacional.Factor * viaticoInternacional.CostoVida;
        //            total.Dias100 += Convert.ToDouble(CantDias100) * monto;
        //            total.Dias60 += Convert.ToDouble(CantDias60) * monto * porcentaje60;
        //            total.Dias40 += Convert.ToDouble(CantDias40) * monto * porcentaje40;
        //            total.Dias50 += Convert.ToDouble(CantDias50) * monto * porcentaje50;
        //            total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
        //        }

        //        if (Sueldo >= 2004708 && Sueldo <= 3092366)
        //        {
        //            /*Tramo B*/
        //            monto = viatico.PorcentajeRango2.Value * viaticoInternacional.Factor * viaticoInternacional.CostoVida;
        //            total.Dias100 += Convert.ToDouble(CantDias100) * monto;
        //            total.Dias60 += Convert.ToDouble(CantDias60) * monto * porcentaje60;
        //            total.Dias40 += Convert.ToDouble(CantDias40) * monto * porcentaje40;
        //            total.Dias50 += Convert.ToDouble(CantDias50) * monto * porcentaje50;
        //            total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
        //        }
        //        if (Sueldo > 3092367)
        //        {
        //            /*Tramo A*/
        //            monto = viatico.PorcentajeRango1.Value * viaticoInternacional.Factor * viaticoInternacional.CostoVida;
        //            total.Dias100 += Convert.ToDouble(CantDias100) * monto;
        //            total.Dias60 += Convert.ToDouble(CantDias60) * monto * porcentaje60;
        //            total.Dias40 += Convert.ToDouble(CantDias40) * monto * porcentaje40;
        //            total.Dias50 += Convert.ToDouble(CantDias50) * monto * porcentaje50;
        //            total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
        //        }
        //        if (Sueldo == 0)
        //        {
        //            /*AD HONOREM*/
        //            monto = viatico.PorcentajeRango4.Value * viaticoInternacional.Factor * viaticoInternacional.CostoVida;
        //            total.Dias100 += Convert.ToDouble(CantDias100) * monto;
        //            total.Dias60 += Convert.ToDouble(CantDias60) * monto * porcentaje60;
        //            total.Dias40 += Convert.ToDouble(CantDias40) * monto * porcentaje40;
        //            total.Dias50 += Convert.ToDouble(CantDias50) * monto * porcentaje50;
        //            total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
        //        }
        //    }
        //    else
        //    {
        //        switch (grado)

        //        {
        //            case "A":
        //                monto = viaticoInternacional.PorcentajeRango1.Value * viaticoInternacional.Factor * viaticoInternacional.CostoVida;
        //                total.Dias100 += Convert.ToDouble(CantDias100) * monto;
        //                total.Dias60 += Convert.ToDouble(CantDias60) * monto * porcentaje60;
        //                total.Dias40 += Convert.ToDouble(CantDias40) * monto * porcentaje40;
        //                total.Dias50 += Convert.ToDouble(CantDias50) * monto * porcentaje50;
        //                total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50; 
        //                break;
        //            case "B":
        //            case "C":
        //                monto = viaticoInternacional.PorcentajeRango1.Value * viaticoInternacional.Factor * viaticoInternacional.CostoVida;
        //                total.Dias100 += Convert.ToDouble(CantDias100) * monto;
        //                total.Dias60 += Convert.ToDouble(CantDias60) * monto * porcentaje60;
        //                total.Dias40 += Convert.ToDouble(CantDias40) * monto * porcentaje40;
        //                total.Dias50 += Convert.ToDouble(CantDias50) * monto * porcentaje50;
        //                total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
        //                break;
        //            default:
        //                //profesionales
        //                var numeroRangoEus = ISConvert.ToInteger(grado);
        //                if (numeroRangoEus >= 2 && numeroRangoEus <= 5) //(grado == "10") Vna_EstamentoRango3
        //                {
        //                    monto = viaticoInternacional.PorcentajeRango2.Value * viaticoInternacional.Factor * viaticoInternacional.CostoVida;
        //                    total.Dias100 += Convert.ToDouble(CantDias100) * monto;
        //                    total.Dias60 += Convert.ToDouble(CantDias60) * monto * porcentaje60;
        //                    total.Dias40 += Convert.ToDouble(CantDias40) * monto * porcentaje40;
        //                    total.Dias50 += Convert.ToDouble(CantDias50) * monto * porcentaje50;
        //                    total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
        //                    break;
        //                }
        //                //tecnicos
        //                if (numeroRangoEus >= 6 && numeroRangoEus <= 15) // --> Vna_EstamentoRango4
        //                {
        //                    monto = viaticoInternacional.PorcentajeRango3.Value * viaticoInternacional.Factor * viaticoInternacional.CostoVida;
        //                    total.Dias100 += Convert.ToDouble(CantDias100) * monto;
        //                    total.Dias60 += Convert.ToDouble(CantDias60) * monto * porcentaje60;
        //                    total.Dias40 += Convert.ToDouble(CantDias40) * monto * porcentaje40;
        //                    total.Dias50 += Convert.ToDouble(CantDias50) * monto * porcentaje50;
        //                    total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
        //                    break;
        //                }
        //                //administrativos
        //                if (numeroRangoEus >= 16 && numeroRangoEus <= 31) // Vna_EstamentoRango5,Vna_EstamentoRango6
        //                {
        //                    monto = viaticoInternacional.PorcentajeRango3.Value * viaticoInternacional.Factor * viaticoInternacional.CostoVida;
        //                    total.Dias100 += Convert.ToDouble(CantDias100) * monto;
        //                    total.Dias60 += Convert.ToDouble(CantDias60) * monto * porcentaje60;
        //                    total.Dias40 += Convert.ToDouble(CantDias40) * monto * porcentaje40;
        //                    total.Dias50 += Convert.ToDouble(CantDias50) * monto * porcentaje50;
        //                    total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
        //                    break;
        //                }
        //                break;
        //        }
        //    }
        //    return total;
        //}

        public JsonResult GetCiudad(string IdPais)
        {
            var ciudad = _repository.Get<Ciudad>().Where(p => p.PaisId == int.Parse(IdPais));
            return Json(ciudad.Select(q => new { value = q.CiudadId, text = q.CiudadNombre.Trim() }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetComunas(string IdRegion)
        {
            var comunas = _sigper.GetComunasbyRegion(IdRegion);
            return Json(comunas.Select(q => new { value = q.Pl_CodCom.Trim(), text = q.Pl_DesCom.Trim() }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<DestinosPasajes>();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<DestinosPasajes>(id);
            return View(model);
        }

        //public ActionResult CreatePartial(int id)
        //{
        //    var Pasaje = _repository.GetById<Pasaje>(id);
        //    var model = new DestinosPasajes();

        //    //ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
        //    ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
        //    ViewBag.IdPais = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
        //    //ViewBag.IdCiudad = new SelectList(_repository.Get<Ciudad>(), "CiudadId", "CiudadNombre");

        //    //ViewBag.IdComunaOrigen = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
        //    ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
        //    ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
        //    //ViewBag.IdCiudadOrigen = new SelectList(_repository.Get<Ciudad>(), "CiudadId", "CiudadNombre");

        //    model.WorkflowId = Pasaje.WorkflowId;
        //    model.PasajeId = id;
        //    model.Pasaje = Pasaje;
        //    model.Pasaje.TipoDestino = Pasaje.TipoDestino;
        //    return View();
        //}

        public ActionResult Create(int id)
        {
            var Pasaje = _repository.GetById<Pasaje>(id);
            var model = new DestinosPasajes();

            //ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPais = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            //ViewBag.IdCiudad = new SelectList(_repository.Get<Ciudad>(), "CiudadId", "CiudadNombre");

            //ViewBag.IdComunaOrigen = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            //ViewBag.IdCiudadOrigen = new SelectList(_repository.Get<Ciudad>(), "CiudadId", "CiudadNombre");

            model.WorkflowId = Pasaje.WorkflowId;
            model.PasajeId = id;
            model.Pasaje = Pasaje;
            model.Pasaje.TipoDestino = Pasaje.TipoDestino;            
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DestinosPasajes model)
        {
            model.FechaIda = DateTime.Now;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.DestinosPasajesInsert(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Edit", "Pasaje", new { id = model.PasajeId });
                }
                else
                {
                    TempData["Error"] = _UseCaseResponseMessage.Errors;
                }
            }

            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPais = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<DestinosPasajes>(id);
            var pasaje = _repository.GetById<Pasaje>(model.PasajeId);
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());
            ViewBag.IdPais = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre".Trim());
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());
            ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre".Trim());
            model.Pasaje.TipoDestino = pasaje.TipoDestino;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(DestinosPasajes model)
        {
            model.FechaIda = DateTime.Now;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.DestinosPasajesUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Edit", "Pasaje", new { model.WorkflowId, id = model.PasajeId });
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPais = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");

            return View(model);
        }

        public ActionResult EditGP(int id)
        {
            var model = _repository.GetById<DestinosPasajes>(id);
            var pasaje = _repository.GetById<Pasaje>(model.PasajeId);

            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPais = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            model.Pasaje.TipoDestino = pasaje.TipoDestino;

            return View(model);
        }

        [HttpPost]
        public ActionResult EditGP(DestinosPasajes model)        
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.DestinosPasajesUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
                    ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");

                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("EditGP", "Pasaje", new { model.WorkflowId, id = model.PasajeId });
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPais = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<DestinosPasajes>(id);
            var pasaje = _repository.GetById<Pasaje>(model.PasajeId);

            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());
            ViewBag.IdPais = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre".Trim());
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());
            ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre".Trim());
            model.Pasaje.TipoDestino = pasaje.TipoDestino;

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseCometidoComision(_repository);
            var model = _repository.GetById<DestinosPasajes>(id);

            var _UseCaseResponseMessage = _useCaseInteractor.DestinosPasajesDelete(id);


            if (_UseCaseResponseMessage.IsValid)
            {
                TempData["Success"] = "Operación terminada correctamente.";
                return RedirectToAction("Edit", "Pasaje", new { id = model.PasajeId });
            }


            foreach (var item in _UseCaseResponseMessage.Errors)
            {
                ModelState.AddModelError(string.Empty, item);
                ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());
                ViewBag.IdPais = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre".Trim());
                ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());
                ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre".Trim());
            }
            return View(model);
        }
    }
}