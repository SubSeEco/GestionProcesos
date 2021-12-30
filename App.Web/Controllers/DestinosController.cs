using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Model.Cometido;
//using App.Model.Shared;
using App.Core.Interfaces;
using App.Core.UseCases;
using App.Model.Core;
using App.Model.Shared;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
    public class DestinosController : Controller
    {
        private readonly IGestionProcesos _repository;
        private readonly ISigper _sigper;
        private static List<Model.DTO.DTODomainUser> ActiveDirectoryUsers { get; set; }
        public static List<Destinos> ListDestino { get; set; }
        public class DtoMontos
        {
            public double? Dias100 { get; set; } = 0;
            public double? Dias60 { get; set; } = 0;
            public double? Dias40 { get; set; } = 0;
            public double? Dias50 { get; set; } = 0;
            public double? Dias00 { get; set; } = 0;
            public double? DiasTotal { get; set; } = 0;
        }

        //public class DtoPasaje
        //{
        //    public string OrigenRegion { get; set; }
        //    public DateTime? FechaOrigen { get; set; }
        //    public string ObsOrigen { get; set; }
        //    public string ObsDestino { get; set; }

        //}

        public DestinosController(IGestionProcesos repository, ISigper sigper)
        {
            _repository = repository;
            _sigper = sigper;

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

        public JsonResult GetRegion(string IdComuna)
        {
            var region = _sigper.GetRegionbyComuna(IdComuna).Trim();
            return Json(new { regionDescripcion = region }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetComunas(string IdRegion)
        {
            var comunas = _sigper.GetComunasbyRegion(IdRegion).OrderBy(c =>c.Pl_DesCom);
            return Json(comunas.Select(q => new { value = q.Pl_CodCom.Trim(), text = q.Pl_DesCom.Trim() }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocalidad(string IdComuna)
        {
            var comunas = _repository .Get<Localidad>(c => c.IdComuna == IdComuna).OrderBy(c => c.NombreLocalidad);
            return Json(comunas.Select(q => new { value = q.LocalidadId, text = q.NombreLocalidad.Trim() }), JsonRequestBehavior.AllowGet);
        }

        public DtoMontos CalculoViatico(int CometidoId, int CantDias100, int CantDias60, int CantDias40, int CantDias50)
        {
            DtoMontos total = new DtoMontos();
            double porcentaje60 = 0.6;
            double porcentaje40 = 0.4;
            double porcentaje50 = 0.5;
            var cometido = _repository.GetById<Cometido>(CometidoId);
            var viatico = _repository.GetFirst<Viatico>(v => v.Año == DateTime.Now.Year);
            var viaticoHonorario = _repository.Get<ViaticoHonorario>(v => v.Año == DateTime.Now.Year);
            var grado = cometido.GradoDescripcion;
            //var CalidadJuridica = cometido.CalidadDescripcion;
            //if (CalidadJuridica == "HONORARIOS") // "CONTRATA")
            if (cometido.IdGrado == "0") // "CONTRATA")
            {
                /*Se busca el valor del sueldo bruto para definir el tramo en el que se encuentra*/
                var Sueldo = _sigper.GetReContra().Where(s => s.RH_NumInte == cometido.NombreId.Value).FirstOrDefault().Re_SuelBas;
                if (Sueldo > viaticoHonorario.Where(v => v.Tramo == "Tramo C").FirstOrDefault().Desde.Value && Sueldo <= viaticoHonorario.Where(v => v.Tramo == "Tramo C").FirstOrDefault().Hasta.Value)
                {
                    /*Tramo C*/
                    var tramo = viaticoHonorario.Where(v => v.Tramo == "Tramo C");
                    total.Dias100 += Convert.ToDouble(CantDias100) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje100.Value);
                    total.Dias60 += Convert.ToDouble(CantDias60) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje60.Value);
                    total.Dias40 += Convert.ToDouble(CantDias40) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje40.Value);
                    total.Dias50 += Convert.ToDouble(CantDias50) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje50.Value);
                    total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
                }

                if (Sueldo >= viaticoHonorario.Where(v => v.Tramo == "Tramo B").FirstOrDefault().Desde.Value && Sueldo <= viaticoHonorario.Where(v => v.Tramo == "Tramo B").FirstOrDefault().Hasta.Value)
                {
                    /*Tramo B*/
                    var tramo = viaticoHonorario.Where(v => v.Tramo == "Tramo B");
                    total.Dias100 += Convert.ToDouble(CantDias100) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje100.Value);
                    total.Dias60 += Convert.ToDouble(CantDias60) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje60.Value);
                    total.Dias40 += Convert.ToDouble(CantDias40) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje40.Value);
                    total.Dias50 += Convert.ToDouble(CantDias50) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje50.Value);
                    total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
                }
                if (Sueldo > viaticoHonorario.Where(v => v.Tramo == "Tramo A").FirstOrDefault().Desde.Value)
                {
                    /*Tramo A*/
                    var tramo = viaticoHonorario.Where(v => v.Tramo == "Tramo A");
                    total.Dias100 += Convert.ToDouble(CantDias100) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje100.Value);
                    total.Dias60 += Convert.ToDouble(CantDias60) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje60.Value);
                    total.Dias40 += Convert.ToDouble(CantDias40) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje40.Value);
                    total.Dias50 += Convert.ToDouble(CantDias50) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje50.Value);
                    total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
                }
                if (Sueldo == 0)
                {
                    /*AD HONOREM*/
                    var tramo = viaticoHonorario.Where(v => v.Tramo == "AD HONOREM");
                    total.Dias100 += Convert.ToDouble(CantDias100) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje100.Value);
                    total.Dias60 += Convert.ToDouble(CantDias60) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje60.Value);
                    total.Dias40 += Convert.ToDouble(CantDias40) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje40.Value);
                    total.Dias50 += Convert.ToDouble(CantDias50) * Convert.ToDouble(tramo.FirstOrDefault().Porcentaje50.Value);
                    total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
                }
            }
            else
            {
                switch (grado)

                {
                    case "A":
                        total.Dias100 += Convert.ToDouble(CantDias100) * Math.Round(Convert.ToDouble(viatico.Rango1.Value), 0);
                        total.Dias60 += Convert.ToDouble(CantDias60) * Math.Round(Convert.ToDouble(viatico.Rango1.Value) * porcentaje60, 0);
                        total.Dias40 += Convert.ToDouble(CantDias40) * Math.Round(Convert.ToDouble(viatico.Rango1.Value) * porcentaje40, 0);
                        total.Dias50 += Convert.ToDouble(CantDias50) * Math.Round(Convert.ToDouble(viatico.Rango1.Value) * porcentaje50, 0);
                        total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
                        break;
                    case "B":
                    case "C":
                        total.Dias100 += Convert.ToDouble(CantDias100) * Math.Round(Convert.ToDouble(viatico.Rango2.Value), 0);
                        total.Dias60 += Convert.ToDouble(CantDias60) * Math.Round(Convert.ToDouble(viatico.Rango2.Value) * porcentaje60, 0);
                        total.Dias40 += Convert.ToDouble(CantDias40) * Math.Round(Convert.ToDouble(viatico.Rango2.Value) * porcentaje40, 0);
                        total.Dias50 += Convert.ToDouble(CantDias50) * Math.Round(Convert.ToDouble(viatico.Rango2.Value) * porcentaje50, 0);
                        total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
                        break;
                    default:
                        //profesionales
                        var numeroRangoEus = Convert.ToInt32(grado);
                        if (numeroRangoEus >= 2 && numeroRangoEus <= 4) //(grado == "10") Vna_EstamentoRango3
                        {
                            total.Dias100 += Convert.ToDouble(CantDias100) * Math.Round(Convert.ToDouble(viatico.Rango3.Value), 0);
                            total.Dias60 += Convert.ToDouble(CantDias60) * Math.Round(Convert.ToDouble(viatico.Rango3.Value) * porcentaje60, 0);
                            total.Dias40 += Convert.ToDouble(CantDias40) * Math.Round(Convert.ToDouble(viatico.Rango3.Value) * porcentaje40, 0);
                            total.Dias50 += Convert.ToDouble(CantDias50) * Math.Round(Convert.ToDouble(viatico.Rango3.Value) * porcentaje50, 0);
                            total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
                            break;
                        }
                        //tecnicos
                        if (numeroRangoEus >= 5 && numeroRangoEus <= 10) // --> Vna_EstamentoRango4
                        {
                            total.Dias100 += Convert.ToDouble(CantDias100) * Math.Round(Convert.ToDouble(viatico.Rango4.Value), 0);
                            total.Dias60 += Convert.ToDouble(CantDias60) * Math.Round(Convert.ToDouble(viatico.Rango4.Value) * porcentaje60, 0);
                            total.Dias40 += Convert.ToDouble(CantDias40) * Math.Round(Convert.ToDouble(viatico.Rango4.Value) * porcentaje40, 0);
                            total.Dias50 += Convert.ToDouble(CantDias50) * Math.Round(Convert.ToDouble(viatico.Rango4.Value) * porcentaje50, 0);
                            total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
                            break;
                        }
                        //administrativos
                        if (numeroRangoEus >= 11 && numeroRangoEus <= 31) // Vna_EstamentoRango5,Vna_EstamentoRango6
                        {
                            total.Dias100 += Convert.ToDouble(CantDias100) * Math.Round(Convert.ToDouble(viatico.Rango5.Value), 0);
                            total.Dias60 += Convert.ToDouble(CantDias60) * Math.Round(Convert.ToDouble(viatico.Rango5.Value) * porcentaje60, 0);
                            total.Dias40 += Convert.ToDouble(CantDias40) * Math.Round(Convert.ToDouble(viatico.Rango5.Value) * porcentaje40, 0);
                            total.Dias50 += Convert.ToDouble(CantDias50) * Math.Round(Convert.ToDouble(viatico.Rango5.Value) * porcentaje50, 0);
                            total.DiasTotal = total.Dias100 + total.Dias60 + total.Dias40 + total.Dias50;
                            break;
                        }
                        break;
                }
            }
            return total;
        }

        public JsonResult Viatico(int CometidoId, int CantDias100, int CantDias60, int CantDias40, int CantDias50)
        {
            DtoMontos valor = CalculoViatico(CometidoId, CantDias100, CantDias60, CantDias40, CantDias50);
            return Json(new
            {
                Dias100Monto = Math.Round(valor.Dias100.Value, 0)
                            ,
                Dias60Monto = Math.Round(valor.Dias60.Value, 0)
                            ,
                Dias40Monto = Math.Round(valor.Dias40.Value, 0)
                            ,
                Dias50Monto = Math.Round(valor.Dias50.Value, 0)
                            ,
                DiasTotal = Math.Round(valor.DiasTotal.Value, 0)
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Viatico60(int CometidoId, int CantDias)
        {
            double monto60 = CalculoViatico(CometidoId, 0, CantDias, 0, 0).Dias60.Value;
            return Json(new { Dias60Monto = Math.Round(monto60, 0) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Viatico40(int CometidoId, int CantDias)
        {
            double monto40 = CalculoViatico(CometidoId, 0, 0, CantDias, 0).Dias40.Value;
            return Json(new { Dias40Monto = Math.Round(monto40, 0) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Viatico50(int CometidoId, int CantDias)
        {
            double monto50 = CalculoViatico(CometidoId, 0, 0, 0, CantDias).Dias50.Value;
            return Json(new { Dias50Monto = Math.Round(monto50, 0) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Viatico100(int CometidoId, int cantDias)
        {
            double monto = CalculoViatico(CometidoId, cantDias, 0, 0, 0).Dias100.Value;
            return Json(new { Dias100Monto = Math.Round(monto, 0) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<Destinos>();
            return View(model);
        }

        public ActionResult View(int id)
        {
            var model = _repository.GetById<Destinos>(id);
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Destinos>(id);
            return View(model);
        }

        public ActionResult Validate(int id)
        {
            var model = _repository.GetById<Destinos>(id);
            return View(model);
        }

        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<Destinos>(id);
            return View(model);
        }

        public ActionResult Create(int id)
        {
            /*inicializacion de atributos*/
            var cometido = _repository.GetById<Cometido>(id);
            var model = new Destinos();
            ViewBag.IdComuna = new List<SelectListItem>();// SelectList(Enumerable.Empty<SelectListItem>());// (_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim()).OrderBy(c => c.Value);
            ViewBag.IdOrigenRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim()).OrderBy(c => c.Value);
            ViewBag.LocalidadId = new List<SelectListItem>();
            model.FechaInicio = DateTime.Now;
            model.FechaHasta = DateTime.Now;
            model.Dias100 = 0;
            model.Dias60 = 0;
            model.Dias40 = 0;
            model.Dias50 = 0;
            model.Dias00 = 0;
            model.Total = 0;
            model.Dias100Aprobados = 0;
            model.Dias60Aprobados = 0;
            model.Dias40Aprobados = 0;
            model.Dias50Aprobados = 0;
            model.Dias00Aprobados = 0;
            model.Dias100Monto = 0;
            model.Dias60Monto = 0;
            model.Dias40Monto = 0;
            model.Dias50Monto = 0;
            model.Dias00Monto = 0;
            model.CometidoId = id;
            model.Cometido = cometido;
            //model.Cometido.ReqPasajeAereo = cometido.ReqPasajeAereo;
            model.WorkflowId = cometido.WorkflowId;



            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Destinos model)
        {
            ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom".Trim());
            ViewBag.LocalidadId = new SelectList(_repository.GetAll<Localidad>(), "LocalidadId", "NombreLocalidad");
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());
            ViewBag.IdOrigenRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.DestinosInsert(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";

                    /*se redireccina a la vista que llamo al metodo de borrar*/
                    var com = _repository.GetFirst<Cometido>(c => c.CometidoId == model.CometidoId);
                    var pro = _repository.Get<Workflow>(p => p.ProcesoId == com.ProcesoId && p.DefinicionWorkflow.Secuencia == 6);
                    if (pro.Any())
                        return RedirectToAction("EditGP", "Cometido", new { model.WorkflowId, id = model.CometidoId });
                    else
                        return RedirectToAction("Edit", "Cometido", new { model.WorkflowId, id = model.CometidoId });
                }
                else
                {
                    TempData["Error"] = _UseCaseResponseMessage.Errors;
                }
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<Destinos>(id);
            ViewBag.IdComuna = new List<SelectListItem>();
            ViewBag.LocalidadId = new List<SelectListItem>();
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim(), model.IdRegion).OrderBy(c => c.Value);
            ViewBag.IdOrigenRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim(), model.IdOrigenRegion).OrderBy(c => c.Value);
            ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom".Trim(), model.IdComuna).OrderBy(c => c.Selected);
            model.Cometido = _repository.GetFirst<Cometido>(c => c.CometidoId == model.CometidoId);

            model.Dias100Aprobados = 0;
            model.Dias60Aprobados = 0;
            model.Dias40Aprobados = 0;
            model.Dias50Aprobados = 0;
            model.Dias00Aprobados = 0;
            model.Dias100Monto = 0;
            model.Dias60Monto = 0;
            model.Dias40Monto = 0;
            model.Dias50Monto = 0;
            model.Dias00Monto = 0;
            //model.IdOrigenRegion = model.IdOrigenRegion;

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(Destinos model)
        {
            model.EditGP = false;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.DestinosUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    //ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
                    //ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");

                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Edit", "Cometido", new { model.WorkflowId, id = model.CometidoId });
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom".Trim());
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());
            ViewBag.IdOrigenRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());
            ViewBag.LocalidadId = new SelectList(_repository.GetAll<Localidad>(), "LocalidadId", "NombreLocalidad");

            return View(model);
        }

        public ActionResult EditGP(int id)
        {
            var model = _repository.GetById<Destinos>(id);
            ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom".Trim());
            ViewBag.LocalidadId = new SelectList(_repository.GetAll<Localidad>(), "LocalidadId", "NombreLocalidad");
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());

            model.IdRegion = model.IdRegion;
            model.IdComuna = model.IdComuna;
            model.FechaInicio = model.FechaInicio;
            model.FechaHasta = model.FechaHasta;
            model.EditGP = true;
            model.Dias100Aprobados = model.Dias100; //model.Dias100Aprobados;
            model.Dias60Aprobados = model.Dias60; //model.Dias60Aprobados;
            model.Dias40Aprobados = model.Dias40; //model.Dias40Aprobados;
            model.Dias50Aprobados = model.Dias50; //model.Dias50Aprobados;
            model.Dias00Aprobados = model.Dias00;
            model.Dias100Monto = model.Dias100Monto;
            model.Dias60Monto = model.Dias60Monto;
            model.Dias40Monto = model.Dias40Monto;
            model.Dias50Monto = model.Dias50Monto;
            model.Dias00Monto = model.Dias00Monto;

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditGP(Destinos model)
        {
            ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom".Trim());
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim(), model.IdRegion);
            ViewBag.LocalidadId = new SelectList(_repository.GetAll<Localidad>(), "LocalidadId", "NombreLocalidad");


            var modelOld = _repository.GetById<Destinos>(model.DestinoId);
            model.IdRegion = modelOld.IdRegion;
            model.IdComuna = modelOld.IdComuna;
            model.LocalidadId = modelOld.LocalidadId;
            model.FechaInicio = model.FechaInicio;
            model.FechaHasta = model.FechaHasta;
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.DestinosUpdate(model);
                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom".Trim());
                    ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());

                    TempData["Success"] = "Operación terminada correctamente.";
                    //return Redirect(Request.UrlReferrer.PathAndQuery);
                    return RedirectToAction("EditGP", "Cometido", new { model.WorkflowId, id = model.CometidoId });
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<Destinos>(id);
            ViewBag.IdComuna = new List<SelectListItem>();
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());
            ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom".Trim());
            ViewBag.LocalidadId = new SelectList(_repository.GetAll<Localidad>(), "LocalidadId", "NombreLocalidad");

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseCometidoComision(_repository);
            var model = _repository.GetById<Destinos>(id);
            var cometidoId = model.CometidoId;
            var _UseCaseResponseMessage = _useCaseInteractor.DestinosDelete(id);


            if (_UseCaseResponseMessage.IsValid)
            {
                TempData["Success"] = "Operación terminada correctamente.";

                /*se redireccina a la vista que llamo al metodo de borrar*/
                var com = _repository.GetFirst<Cometido>(c => c.CometidoId == cometidoId);
                var pro = _repository.GetExists<Workflow>(p => p.ProcesoId == com.ProcesoId && p.DefinicionWorkflow.Secuencia == 6);
                if (pro)
                    return RedirectToAction("EditGP", "Cometido", new { id = cometidoId });

                return RedirectToAction("Edit", "Cometido", new { id = cometidoId });
            }


            foreach (var item in _UseCaseResponseMessage.Errors)
            {
                ModelState.AddModelError(string.Empty, item);

                ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom".Trim());
                ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg".Trim());
                ViewBag.LocalidadId = new SelectList(_repository.GetAll<Localidad>(), "LocalidadId", "NombreLocalidad");
            }
            return View(model);
        }
    }
}