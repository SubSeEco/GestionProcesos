﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using App.Model.Cometido;
using App.Model.Pasajes;
using App.Model.Core;
using App.Model.DTO;
//using App.Model.Shared;
using App.Core.Interfaces;
using App.Model.Shared;
using App.Infrastructure.Extensions;
using Newtonsoft.Json;
using App.Core.UseCases;
using App.Model.Comisiones;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class CometidoController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IHSM _hsm;
        private static List<App.Model.DTO.DTODomainUser> ActiveDirectoryUsers { get; set; }
        public static List<Destinos> ListDestino = new List<Destinos>();

        public CometidoController(IGestionProcesos repository, ISIGPER sigper, IHSM hsm, IFile file)
        {
            _repository = repository;
            _sigper = sigper;
            _hsm = hsm;
            _file = file;

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

        public JsonResult GetUsuario(int Rut)
        {
            var correo = _sigper.GetUserByRut(Rut).Funcionario.Rh_Mail.Trim();
            var per = _sigper.GetUserByEmail(correo);
            var IdCargo = per.FunDatosLaborales.RhConCar.Value;
            var cargo = string.IsNullOrEmpty(per.FunDatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == per.FunDatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidad = per.FunDatosLaborales.RH_ContCod;
            var calidad = string.IsNullOrEmpty(per.FunDatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == per.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGrado = string.IsNullOrEmpty(per.FunDatosLaborales.RhConGra.Trim()) ? "0" : per.FunDatosLaborales.RhConGra.Trim();
            var grado = string.IsNullOrEmpty(per.FunDatosLaborales.RhConGra.Trim()) ? "Sin Grado" : per.FunDatosLaborales.RhConGra.Trim();
            var estamento = per.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == per.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgId = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var Programa = ProgId != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgId).FirstOrDefault().RePytDes : "S/A";
            var conglomerado = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte).ReContraSed;
            var jefatura = per.Jefatura != null ? per.Jefatura.PeDatPerChq : "Sin jefatura definida" ;

            string rut;
            if (per.Funcionario.RH_NumInte.ToString().Length < 8 == true)
            {
                string t = per.Funcionario.RH_NumInte.ToString();
                rut = string.Concat("0", t);
            }
            else
            {
                rut = per.Funcionario.RH_NumInte.ToString();
            }

            return Json(new { Rut = rut,
                DV = per.Funcionario.RH_DvNuInt.ToString(),
                IdCargo = IdCargo,
                Cargo = cargo,
                IdCalidad = IdCalidad,
                CalidadJuridica = calidad,
                IdGrado = IdGrado,
                Grado = grado,
                Estamento = estamento,
                Programa = Programa.Trim(),
                Conglomerado = conglomerado,
                Unidad = per.Unidad.Pl_UndDes.Trim(),
                Jefatura = jefatura
            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<Cometido>();
            return View(model);
        }

        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult View(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo");
            //ViewBag.TipoReembolsoId = new SelectList(_repository.Get<SIGPERTipoReembolso>().OrderBy(q => q.SIGPERTipoReembolsoId), "SIGPERTipoReembolsoId", "Reembolso");
            //ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");


            //var model = _repository.GetById<Cometido>(id);
            var model = _repository.GetFirst<Cometido>(q => q.ProcesoId == id);

            return View(model);
        }

        public ActionResult CreaDocto(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            if (model.GeneracionCDP.Count > 0)
            {
                var cdp = _repository.GetById<GeneracionCDP>(model.GeneracionCDP.FirstOrDefault().GeneracionCDPId);
                model.GeneracionCDP.Add(cdp);
            }

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            if (model.GeneracionCDP.Count > 0)
            {
                var cdp = _repository.GetById<GeneracionCDP>(model.GeneracionCDP.FirstOrDefault().GeneracionCDPId);
                model.GeneracionCDP.Add(cdp);
            }

            return View(model);
        }

        public ActionResult DetailsDocto(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            //if (model.GeneracionCDP.Count > 0)
            //{
            //    var cdp = _repository.GetById<GeneracionCDP>(model.GeneracionCDP.FirstOrDefault().GeneracionCDPId);
            //    model.GeneracionCDP.Add(cdp);
            //}

            return View(model);
        }

        public ActionResult DetailsFinanzas(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DetailsFinanzas(Cometido model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _hsm);
                //var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);
                var doc = _repository.Get<Documento>(c =>c.ProcesoId == model.ProcesoId && c.TipoDocumentoId == 5).FirstOrDefault();
                var user = User.Email();
                var _UseCaseResponseMessage = _useCaseInteractor.DocumentoSign(doc, user);



                //if (_UseCaseResponseMessage.Warnings.Count > 0)
                //    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
            }

            var modelo = _repository.Get<Cometido>(c => c.CometidoId == model.CometidoId).FirstOrDefault();
            
            return View(modelo);
            //return View(model);
        }

        public ActionResult Validate(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            return View(model);
        }

        public ActionResult Sign(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            return View(model);
        }

        public ActionResult Create(int? WorkFlowId, int? ProcesoId)
        {
            var persona = _sigper.GetUserByEmail(User.Email());

            ViewBag.IdComuna = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().Where(q => q.Activo == true).OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo");
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");


            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new Cometido
            {
                WorkflowId = workflow.WorkflowId,
                ProcesoId = workflow.ProcesoId,
                Tarea = workflow.DefinicionWorkflow.Nombre
            };

            if (persona.Funcionario == null)
                ModelState.AddModelError(string.Empty, "No se encontró información del funcionario en SIGPER");
            if (persona.Unidad == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la unidad del funcionario en SIGPER");
            if (persona.Jefatura == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la jefatura del funcionario en SIGPER");

            if (ModelState.IsValid)
            {
                model.FechaSolicitud = DateTime.Now;
                model.IdUnidad = persona.Unidad.Pl_UndCod;
                model.UnidadDescripcion = persona.Unidad.Pl_UndDes.Trim();
                model.Rut = persona.Funcionario.RH_NumInte.ToString().Length < 8 ? Convert.ToInt32("0" + persona.Funcionario.RH_NumInte.ToString()) : persona.Funcionario.RH_NumInte;
                model.DV = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreId = persona.Funcionario.RH_NumInte;
                model.Nombre = persona.Funcionario.PeDatPerChq.Trim();
                model.IdCargo = persona.FunDatosLaborales.RhConCar.Value;
                model.CargoDescripcion = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.IdCalidad = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                model.CalidadDescripcion = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                model.IdGrado = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra.Trim();
                model.GradoDescripcion = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra.Trim()) ? "0" : persona.FunDatosLaborales.RhConGra;
                model.IdEstamento = persona.FunDatosLaborales.PeDatLabEst;
                model.EstamentoDescripcion = persona.FunDatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                model.FinanciaOrganismo = false;
                model.Vehiculo = false;
                model.SolicitaReembolso = true;
                model.IdConglomerado = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                model.ConglomeradoDescripcion = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                model.IdPrograma = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcion = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt.ToString();
                model.ProgramaDescripcion = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";
                model.Jefatura = persona.Jefatura.PeDatPerChq;

                model.Destinos = ListDestino;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cometido model)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);

            model.FechaSolicitud = DateTime.Now;
            model.SolicitaReembolso = true;
            model.Destinos = ListDestino;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.CometidoInsert(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Edit", "Cometido", new { model.WorkflowId, id = model.CometidoId });
                }
                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            return View(model);
        }

        public ActionResult EditSigfeTesoreria(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            List<SelectListItem> tipoPagoTesoreria = new List<SelectListItem>
            {
            new SelectListItem {Text = "Pago", Value = "1"},
            new SelectListItem {Text = "Pago con Observaciones", Value = "2"},
            new SelectListItem {Text = "No Pago", Value = "3"},
            };

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.IdFuncionarioPagadorTesoreria = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdTipoPagoTesoreria = new SelectList(_repository.GetAll<TipoPagoSIGFE>().Where(q => q.TipoActivo == true), "TipoPagoSIGFEId", "DescripcionTipoPago");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSigfeTesoreria(Cometido model)
        {
            var resp = new ResponseMessage();

            if (model.IdFuncionarioPagadorTesoreria != 0)
            {
                var nombre = _sigper.GetUserByRut(model.IdFuncionarioPagadorTesoreria).Funcionario.PeDatPerChq;
                model.NombreFuncionarioPagadorTesoreria = nombre.Trim();
            }

            if (!model.IdTipoPagoTesoreria.HasValue)
                resp.Errors.Add("Debe ingresar tipo de pago Tesoreria.");

            if (string.IsNullOrEmpty(model.IdSigfeTesoreria))
                resp.Errors.Add("Debe ingresar ID SIGFE Tesoreria.");

            if (!model.FechaPagoSigfeTesoreria.HasValue)
                resp.Errors.Add("Debe ingresar fecha pago sigfe Tesoreria");

            if (resp.Errors.Count == 0)
            {
                model.TesoreriaOk = true;

                if (ModelState.IsValid)
                {
                    var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                    var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);

                    //if (_UseCaseResponseMessage.Warnings.Count > 0)
                    //    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                    if (_UseCaseResponseMessage.IsValid)
                    {
                        TempData["Success"] = "Operación terminada correctamente.";
                        return Redirect(Request.UrlReferrer.PathAndQuery);
                    }

                    foreach (var item in _UseCaseResponseMessage.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item);
                    }
                }
                else
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                        .Where(y => y.Count > 0)
                        .ToList();
                }
            }
            else
            {
                foreach (var item in resp.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            List<SelectListItem> tipoPagoTesoreria = new List<SelectListItem>
            {
            new SelectListItem {Text = "Pago", Value = "1"},
            new SelectListItem {Text = "Pago con Observaciones", Value = "2"},
            new SelectListItem {Text = "No Pago", Value = "3"},
            };

            model = _repository.GetById<Cometido>(model.CometidoId);
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.IdFuncionarioPagadorTesoreria = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdTipoPagoTesoreria = new SelectList(_repository.GetAll<TipoPagoSIGFE>().Where(q => q.TipoActivo == true), "TipoPagoSIGFEId", "DescripcionTipoPago");
            return View(model);
        }

        public ActionResult EditSigfe(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            List<SelectListItem> tipoPago = new List<SelectListItem>
            {
            new SelectListItem {Text = "Pago", Value = "1"},
            new SelectListItem {Text = "Pago con Observaciones", Value = "2"},
            new SelectListItem {Text = "No Pago", Value = "3"},
            };

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.IdFuncionarioPagador = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdTipoPago = new SelectList(_repository.GetAll<TipoPagoSIGFE>().Where(q => q.TipoActivo == true), "TipoPagoSIGFEId", "DescripcionTipoPago");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSigfe(Cometido model)
        {
            var resp = new ResponseMessage();

            if (model.IdFuncionarioPagador.HasValue)
            {
                var nombre = _sigper.GetUserByRut(model.IdFuncionarioPagador.Value).Funcionario.PeDatPerChq;
                model.NombreFuncionarioPagador = nombre.Trim();
            }
            
            if (!model.IdTipoPago.HasValue)
                resp.Errors.Add("Debe ingresar tipo de pago.");

            if(string.IsNullOrEmpty(model.IdSigfe))
                resp.Errors.Add("Debe ingresar ID SIGFE.");

            if(!model.FechaPagoSigfe.HasValue)
                resp.Errors.Add("Debe ingresar fecha pago sigfe");

            if (resp.Errors.Count == 0)
            {
                model.ContabilidadOk = true;

                if (ModelState.IsValid)
                {
                    var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                    var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);

                    //if (_UseCaseResponseMessage.Warnings.Count > 0)
                    //    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                    if (_UseCaseResponseMessage.IsValid)
                    {
                        TempData["Success"] = "Operación terminada correctamente.";
                        return Redirect(Request.UrlReferrer.PathAndQuery);
                    }

                    foreach (var item in _UseCaseResponseMessage.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item);
                    }
                }
                else
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                        .Where(y => y.Count > 0)
                        .ToList();
                }
            }
            else
            {
                foreach (var item in resp.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            List<SelectListItem> tipoPago = new List<SelectListItem>
            {
            new SelectListItem {Text = "Pago", Value = "1"},
            new SelectListItem {Text = "Pago con Observaciones", Value = "2"},
            new SelectListItem {Text = "No Pago", Value = "3"},
            };

            model = _repository.GetById<Cometido>(model.CometidoId);
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.IdFuncionarioPagador = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdTipoPago = new SelectList(_repository.GetAll<TipoPagoSIGFE>().Where(q => q.TipoActivo == true), "TipoPagoSIGFEId", "DescripcionTipoPago");
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            //model.Destinos = ListDestino;
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            //ViewBag.TipoReembolsoId = new SelectList(_repository.Get<SIGPERTipoReembolso>().OrderBy(q => q.SIGPERTipoReembolsoId), "SIGPERTipoReembolsoId", "Reembolso", model.TipoReembolsoId);
            ViewBag.FinanciaOrganismo = model.FinanciaOrganismo;
            ViewBag.Alimentacion = model.Alimentacion;
            ViewBag.Alojamiento = model.Alojamiento;
            ViewBag.Pasajes = model.Pasajes;

            ViewBag.DestinosPasajes = _repository.Get<DestinosPasajes>(q => q.Pasaje.ProcesoId == model.ProcesoId);


            ViewBag.Pasaje = new DestinosPasajes(); //_des; //_repository.Get<DestinosPasajes>(c => c.PasajeId == 2038).FirstOrDefault(); //_repository.Get<Dest>(c => c.ProcesoId.Value == model.ProcesoId.Value).FirstOrDefault().CometidoId;
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            //ViewBag.FechaOrigen = DateTime.Now;
            //ViewBag.FechaVuelta = DateTime.Now;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cometido model, DestinosPasajes DesPasajes)
        {
            //var com = _repository.GetById<Cometido>(model.CometidoId);
            //model.Alojamiento = com.Alojamiento;
            //model.Alimentacion = com.Alimentacion;
            //model.Pasajes = com.Pasajes;

            if (ModelState.IsValid)
            {
                //var _useCaseInteractor = new UseCaseInteractorCustom(_repository, _sigper);
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);
                var resp = new ResponseMessage();
                var res = new ResponseMessage();
                

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    /*Se ingresa informacion de pasajes*/
                    /*Se crean los pasajes, si se solicitan*/
                    if (model.ReqPasajeAereo == true)
                    {
                        if (DesPasajes != null && DesPasajes.IdRegion != null)
                        {
                            /*se crea encabezado de pasaje y destinos de pasajes*/
                            Pasaje _pasaje = new Pasaje();
                            _pasaje.FechaSolicitud = DateTime.Now;
                            _pasaje.Nombre = model.Nombre;
                            _pasaje.NombreId = model.NombreId;
                            _pasaje.Rut = model.Rut;
                            _pasaje.DV = model.DV;
                            _pasaje.IdCalidad = model.IdCalidad;
                            _pasaje.CalidadDescripcion = model.CalidadDescripcion;
                            _pasaje.PasajeDescripcion = model.CometidoDescripcion;
                            _pasaje.TipoDestino = true;
                            _pasaje.ProcesoId = model.ProcesoId;
                            _pasaje.WorkflowId = model.WorkflowId;
                            resp = _useCaseInteractor.PasajeInsert(_pasaje);
                            if (resp.Errors.Count > 0)
                                _UseCaseResponseMessage.Errors.Add(resp.Errors.FirstOrDefault());

                            /*genera resgistro en tabla destino pasaje, segun los destinos señalados en el cometido*/
                            //foreach (var com in DesPasajes)
                            //{
                            DestinosPasajes _destino = new DestinosPasajes();
                            _destino.PasajeId = resp.EntityId;
                            _destino.IdRegion = DesPasajes.IdRegion;
                            _destino.RegionDescripcion = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == DesPasajes.IdRegion).Pl_DesReg.Trim(); //DesPasajes.RegionDescripcion;
                            _destino.IdRegionOrigen = DesPasajes.IdRegionOrigen;
                            _destino.OrigenRegionDescripcion = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == DesPasajes.IdRegionOrigen).Pl_DesReg.Trim();
                            _destino.FechaIda = DesPasajes.FechaOrigen;
                            _destino.FechaVuelta = DesPasajes.FechaVuelta;
                            _destino.FechaOrigen = DateTime.Now;
                            _destino.ObservacionesOrigen = DesPasajes.ObservacionesOrigen;
                            _destino.ObservacionesDestinos = DesPasajes.ObservacionesDestinos;
                            res = _useCaseInteractor.DestinosPasajesInsert(_destino);
                            if (res.Errors.Count > 0)
                                _UseCaseResponseMessage.Errors.Add(res.Errors.FirstOrDefault());
                            //}
                        }
                        //else
                        //    TempData["Errors"] = "Debe agregar datos del pasaje";
                    }


                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
            }

            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            //ViewBag.TipoReembolsoId = new SelectList(_repository.Get<SIGPERTipoReembolso>().OrderBy(q => q.SIGPERTipoReembolsoId), "SIGPERTipoReembolsoId", "Reembolso", model.TipoReembolsoId);
            ViewBag.Pasaje = new DestinosPasajes(); //_repository.Get<DestinosPasajes>(c => c.PasajeId == 2038).FirstOrDefault(); //_repository.Get<Dest>(c => c.ProcesoId.Value == model.ProcesoId.Value).FirstOrDefault().CometidoId;
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdRegion = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");

            return View(model);
        }

        public ActionResult EditGP(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            ViewBag.FinanciaOrganismo = model.FinanciaOrganismo;


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGP(Cometido model)
        {

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            //ViewBag.TipoReembolsoId = new SelectList(_repository.Get<SIGPERTipoReembolso>().OrderBy(q => q.SIGPERTipoReembolsoId), "SIGPERTipoReembolsoId", "Reembolso", model.TipoReembolsoId);

            return View(model);
        }

        public ActionResult EditPpto(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            //model.Destinos = ListDestino;
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            //ViewBag.TipoReembolsoId = new SelectList(_repository.Get<SIGPERTipoReembolso>().OrderBy(q => q.SIGPERTipoReembolsoId), "SIGPERTipoReembolsoId", "Reembolso", model.TipoReembolsoId);
            ViewBag.FinanciaOrganismo = model.FinanciaOrganismo;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPpto(Cometido model)
        {

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.CometidoUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            //ViewBag.TipoReembolsoId = new SelectList(_repository.Get<SIGPERTipoReembolso>().OrderBy(q => q.SIGPERTipoReembolsoId), "SIGPERTipoReembolsoId", "Reembolso", model.TipoReembolsoId);

            return View(model);
        }

        public ActionResult GeneraDocumento(int id)
        {
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }

            byte[] pdf = null;
            DTOFileMetadata data = new DTOFileMetadata();
            int tipoDoc = 0;
            int IdDocto = 0;
            string Name = string.Empty;
            var model = _repository.GetById<Cometido>(id);
            var Workflow = _repository.Get<Workflow>(q => q.WorkflowId == model.WorkflowId).FirstOrDefault();
            if ((Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje) || (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)) /*genera CDP, por la etapa en la que se encuentra*/
            {
                /*Se genera certificado de viatico*/
                Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("CDPViatico", new { id = model.CometidoId }) { FileName = "CDP_Viatico" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                pdf = resultPdf.BuildFile(ControllerContext);
                //data = GetBynary(pdf);
                data = _file.BynaryToText(pdf);
                tipoDoc = 2;
                Name = "CDP Viatico Cometido nro" + " " + model.CometidoId.ToString() + ".pdf";
                int idDoctoViatico = 0;

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                var cdpViatico = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId);
                if (cdpViatico != null)
                {
                    foreach (var res in cdpViatico)
                    {
                        if (res.TipoDocumentoId == 2)
                            idDoctoViatico = res.DocumentoId;
                    }
                }

                if(idDoctoViatico == 0)
                {
                    /*se guarda certificado de viatico*/
                    var email = UserExtended.Email(User);
                    var doc = new Documento();
                    doc.Fecha = DateTime.Now;
                    doc.Email = email;
                    doc.FileName = Name;
                    doc.File = pdf;
                    doc.ProcesoId = model.ProcesoId.Value;
                    doc.WorkflowId = model.WorkflowId.Value;
                    doc.Signed = false;
                    doc.Texto = data.Text;
                    doc.Metadata = data.Metadata;
                    doc.Type = data.Type;
                    doc.TipoPrivacidadId = 1;
                    doc.TipoDocumentoId = tipoDoc;

                    _repository.Create(doc);
                    _repository.Save();
                }
                else
                {
                    var docOld = _repository.GetById<Documento>(idDoctoViatico);
                    docOld.File = pdf;
                    docOld.Signed = false;
                    docOld.Texto = data.Text;
                    docOld.Metadata = data.Metadata;
                    docOld.Type = data.Type;
                    _repository.Update(docOld);
                    _repository.Save();
                }

                

                /*Se genera certificado de pasaje si es que existe*/
                //if (model.ReqPasajeAereo == true)
                //{
                //    Rotativa.ActionAsPdf resultPdfPasaje = new Rotativa.ActionAsPdf("CDPPasajes", new { id = model.CometidoId }) { FileName = "CDP_Pasajes" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                //    pdf = resultPdfPasaje.BuildFile(ControllerContext);
                //    data = GetBynary(pdf);
                //    tipoDoc = 3;
                //    Name = "CDP Pasaje Cometido nro" + " " + model.CometidoId.ToString() + ".pdf";
                //    int idDoctoPasaje = 0;

                //    /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                //    var CdpPasaje = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId);
                //    if (CdpPasaje != null)
                //    {
                //        foreach (var res in CdpPasaje)
                //        {
                //            if (res.TipoDocumentoId == 3)
                //                idDoctoPasaje = res.DocumentoId;
                //        }
                //    }

                //    if(idDoctoPasaje == 0)
                //    {
                //        /*se guarda certificado de pasaje*/
                //        var email = UserExtended.Email(User);
                //        var docPasaje = new Documento();
                //        docPasaje.Fecha = DateTime.Now;
                //        docPasaje.Email = email;
                //        docPasaje.FileName = Name;
                //        docPasaje.File = pdf;
                //        docPasaje.ProcesoId = model.ProcesoId.Value;
                //        docPasaje.WorkflowId = model.WorkflowId.Value;
                //        docPasaje.Signed = false;
                //        docPasaje.Texto = data.Text;
                //        docPasaje.Metadata = data.Metadata;
                //        docPasaje.Type = data.Type;
                //        docPasaje.TipoPrivacidadId = 1;
                //        docPasaje.TipoDocumentoId = tipoDoc;

                //        _repository.Create(docPasaje);
                //        _repository.Save();
                //    }
                //    else
                //    {
                //        var docOld = _repository.GetById<Documento>(idDoctoPasaje);
                //        docOld.File = pdf;
                //        docOld.Signed = false;
                //        docOld.Texto = data.Text;
                //        docOld.Metadata = data.Metadata;
                //        docOld.Type = data.Type;
                //        _repository.Update(docOld);
                //        _repository.Save();
                //    }
                //}
            }
            else
            {
                if (model.CalidadDescripcion.Contains("HONORARIOS"))/*valida si es contrata u honorario*/
                {
                    //if (model.IdGrado != "0" && model.GradoDescripcion != "0")
                    //{
                        Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Orden", new { id = model.CometidoId }) { FileName = "Orden_Pago" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                        pdf = resultPdf.BuildFile(ControllerContext);
                        //data = GetBynary(pdf);
                        data = _file.BynaryToText(pdf);

                    tipoDoc = 1;
                        Name = "Orden de Pago Cometido nro" + " " + model.CometidoId.ToString() + ".pdf";
                    //}
                    //else
                    //{
                    //    //TempData["Error"] = "No existen antecedentes del grado del funcionario";
                    //    TempData["Success"] = "No existen antecedentes del grado del funcionario.";
                    //    return Redirect(Request.UrlReferrer.PathAndQuery);
                    //}
                }
                else if (model.CalidadDescripcion.Contains("TITULAR"))/*valida si es autoridad*/
                {
                    Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Resolucion", new { id = model.CometidoId }) { FileName = "Resolucion Ministerial Exenta" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                    pdf = resultPdf.BuildFile(ControllerContext);
                    //data = GetBynary(pdf);
                    data = _file.BynaryToText(pdf);
                    tipoDoc = 1;
                    Name = "Resolucion Ministerial Exenta nro" + " " + model.CometidoId.ToString() + ".pdf";
                }
                else
                {
                    Rotativa.ActionAsPdf resultPdf = new Rotativa.ActionAsPdf("Pdf", new { id = model.CometidoId }) { FileName = "Resolucion" + ".pdf", Cookies = cookieCollection, FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName };
                    pdf = resultPdf.BuildFile(ControllerContext);
                    //data = GetBynary(pdf);
                    data = _file.BynaryToText(pdf);

                    tipoDoc = 1;
                    Name = "Resolucion Cometido nro" + " " + model.CometidoId.ToString() + ".pdf";
                }

                /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                var resolucion = _repository.GetAll<Documento>().Where(d => d.ProcesoId == model.ProcesoId);
                if (resolucion != null)
                {
                    foreach (var res in resolucion)
                    {
                        if (res.TipoDocumentoId == 1)
                            IdDocto = res.DocumentoId;
                    }
                }

                /*se guarda el pdf generado como documento adjunto -- se valida si ya existe el documento para actualizar*/
                if (IdDocto == 0)
                {
                    var email = UserExtended.Email(User);
                    var doc = new Documento();
                    doc.Fecha = DateTime.Now;
                    doc.Email = email;
                    doc.FileName = Name;
                    doc.File = pdf;
                    doc.ProcesoId = model.ProcesoId.Value;
                    doc.WorkflowId = model.WorkflowId.Value;
                    doc.Signed = false;
                    doc.Texto = data.Text;
                    doc.Metadata = data.Metadata;
                    doc.Type = data.Type;
                    doc.TipoPrivacidadId = 1;
                    doc.TipoDocumentoId = tipoDoc;

                    _repository.Create(doc);
                    _repository.Save();
                }
                else
                {
                    var docOld = _repository.GetById<Documento>(IdDocto);
                    if (docOld.Signed != true)
                    {
                        docOld.File = pdf;
                        docOld.Signed = false;
                        docOld.Texto = data.Text;
                        docOld.Metadata = data.Metadata;
                        docOld.Type = data.Type;
                        _repository.Update(docOld);
                        _repository.Save();
                    }
                    else
                        TempData["Error"] = "Documento ya se encuentra firmado electronicamente";
                }

            }

            return Redirect(Request.UrlReferrer.PathAndQuery);            
        }
        
        //private DTOFileMetadata GetBynary(byte[] pdf)
        //{
        //    var data = new App.Model.DTO.DTOFileMetadata();
        //    var textExtractor = new TextExtractor();
        //    var extract = textExtractor.Extract(pdf);

        //    data.Text = !string.IsNullOrWhiteSpace(extract.Text) ? extract.Text.Trim() : null;
        //    data.Metadata = extract.Metadata.Any() ? string.Join(";", extract.Metadata) : null;
        //    data.Type = extract.ContentType;

        //    return data;
        //}

        [AllowAnonymous]
        public ActionResult Pdf(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            var Workflow = _repository.Get<Workflow>(q => q.WorkflowId == model.WorkflowId).FirstOrDefault();
            if (Workflow.DefinicionWorkflow.Secuencia == 6 && Workflow.DefinicionWorkflow.DefinicionProcesoId != (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
            {
                model.GeneracionCDP.FirstOrDefault().FechaFirma = DateTime.Now;
                model.GeneracionCDP.FirstOrDefault().PsjFechaFirma = DateTime.Now;

                return new Rotativa.MVC.ViewAsPdf("CDPViatico", model);
                //return View(model);
            }
            else
            {
                model.Dias = (model.Destinos.LastOrDefault().FechaHasta.Date - model.Destinos.FirstOrDefault().FechaInicio.Date).Days + 1;
                model.DiasPlural = "s";
                model.Tiempo = model.Destinos.FirstOrDefault().FechaInicio < DateTime.Now ? "Pasado" : "Futuro";
                model.Anno = DateTime.Now.Year.ToString();
                model.Subscretaria = model.UnidadDescripcion.Contains("Turismo") ? "SUBSECRETARIO DE TURISMO" : "SUBSECRETARIA DE ECONOMÍA Y EMPRESAS DE MENOR TAMAÑO";
                model.FechaResolucion = DateTime.Now;
                model.Firma = false;
                model.NumeroResolucion = model.CometidoId;
                model.Destinos.FirstOrDefault().TotalViaticoPalabras = ExtensionesString.enletras(model.Destinos.FirstOrDefault().TotalViatico.ToString());

                /*se traen los datos de la tabla parrafos*/
                var parrafos = _repository.GetAll<Parrafos>();
                switch (model.IdGrado)
                {
                    case "B":/*Firma Subsecretario*/
                        model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenSubse).FirstOrDefault().ParrafoTexto;
                        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteSubse).FirstOrDefault().ParrafoTexto;
                        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoSubse).FirstOrDefault().ParrafoTexto;
                        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
                        break;
                    case "C": /*firma ministro*/
                        model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenMinistro).FirstOrDefault().ParrafoTexto;
                        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteMinistro).FirstOrDefault().ParrafoTexto;
                        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoMinistro).FirstOrDefault().ParrafoTexto;
                        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
                        break;
                    default:
                        model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Orden).FirstOrDefault().ParrafoTexto;
                        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Firmante).FirstOrDefault().ParrafoTexto;
                        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoFirmante).FirstOrDefault().ParrafoTexto;
                        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistoRAE).FirstOrDefault().ParrafoTexto;
                        var vit = _repository.Get<Parrafos>(c => c.ParrafosId == (int)App.Util.Enum.Firmas.ViaticodeVuelta && c.ParrafoActivo == true).ToList();
                        if (vit.Count > 0)
                            model.ViaticodeVuelta = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.ViaticodeVuelta).FirstOrDefault().ParrafoTexto;
                        else
                            model.ViaticodeVuelta = string.Empty;

                        break;
                }
                                
                /*Se valida que se encuentre en la tarea de Firma electronica para agregar folio y fecha de resolucion*/
                var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId) ?? null;
                if (workflowActual.DefinicionWorkflow.Secuencia == 8 || (workflowActual.DefinicionWorkflow.Secuencia == 13 && workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje))
                {
                    if (model.Folio == null)
                    {
                        #region Folio
                        /*se va a buscar el folio de testing*/
                        DTOFolio folio = new DTOFolio();
                        folio.periodo = DateTime.Now.Year.ToString();
                        folio.solicitante = "Gestion Procesos - Cometidos";/*Sistema que solicita el numero de Folio*/
                        if (model.IdCalidad == 10)
                        {
                            folio.tipodocumento = "RAEX";/*"ORPA";*/
                        }
                        else
                        {
                            switch (model.IdGrado)
                            {
                                case "B":/*Resolución Ministerial Exenta*/
                                    folio.tipodocumento = "RMEX";
                                    break;
                                case "C": /*Resolución Ministerial Exenta*/
                                    folio.tipodocumento = "RMEX";
                                    break;
                                default:
                                    folio.tipodocumento = "RAEX";/*Resolución Administrativa Exenta*/
                                    break;
                            }
                        }


                        //definir url
                        var url = "http://wsfolio.test.economia.cl/api/folio/";

                        //definir cliente http
                        var clientehttp = new WebClient();
                        clientehttp.Headers[HttpRequestHeader.ContentType] = "application/json";

                        //invocar metodo remoto
                        string result = clientehttp.UploadString(url, "POST", JsonConvert.SerializeObject(folio));

                        //convertir resultado en objeto 
                        var obj = JsonConvert.DeserializeObject<App.Model.DTO.DTOFolio>(result);

                        //verificar resultado
                        if (obj.status == "OK")
                        {
                            model.Folio = obj.folio;
                            model.FechaResolucion = DateTime.Now;
                            model.Firma = true;

                            _repository.Update(model);
                            _repository.Save();
                        }
                        if (obj.status == "ERROR")
                        {
                            TempData["Error"] = obj.error;
                            //return View(DTOFolio);
                        }
                        #endregion
                    }
                }

                //if (model.CalidadDescripcion.Contains("honorario"))
                //if (model.IdGrado == "0")
                if (model.GradoDescripcion == "0")
                {
                    //return new Rotativa.MVC.ViewAsPdf("Orden", model);
                    return View(model);
                }
                else
                {
                    //return new Rotativa.MVC.ViewAsPdf("Pdf", model);
                    //return new ViewAsPdf("Resolucion", model);
                    return View(model);
                }
            }
        }

        public ActionResult CDPViatico(int Id)
        {
            var model = _repository.GetById<Cometido>(Id);
            var Workflow = _repository.Get<Workflow>(q => q.WorkflowId == model.WorkflowId).FirstOrDefault();
            if (Workflow.DefinicionWorkflow.Secuencia == 6 || (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje))
            {
                model.GeneracionCDP.FirstOrDefault().FechaFirma = DateTime.Now;
                model.GeneracionCDP.FirstOrDefault().PsjFechaFirma = DateTime.Now;

                //return new Rotativa.MVC.ViewAsPdf("CDPViatico", model);
                return View(model);
            }
            //return new Rotativa.MVC.ViewAsPdf("CDPViatico", model);
            return null;
        }

        public ActionResult CDPPasajes(int Id)
        {
            var model = _repository.GetById<Cometido>(Id);
            var Workflow = _repository.Get<Workflow>(q => q.WorkflowId == model.WorkflowId).FirstOrDefault();
            if (Workflow.DefinicionWorkflow.Secuencia == 6 || (Workflow.DefinicionWorkflow.Secuencia == 8 && Workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje))
            {
                model.GeneracionCDP.FirstOrDefault().FechaFirma = DateTime.Now;
                model.GeneracionCDP.FirstOrDefault().PsjFechaFirma = DateTime.Now;

                //return new Rotativa.MVC.ViewAsPdf("CDPViatico", model);
                return View(model);
            }
            //return new Rotativa.MVC.ViewAsPdf("CDPViatico", model);
            return null;
        }

        public ActionResult Orden(int id)
        {
            var model = _repository.GetById<Cometido>(id);
            model.Dias = (model.Destinos.LastOrDefault().FechaHasta.Date - model.Destinos.FirstOrDefault().FechaInicio.Date).Days + 1;
            model.DiasPlural = "s";
            model.Tiempo = model.Destinos.FirstOrDefault().FechaInicio < DateTime.Now ? "Pasado" : "Futuro";
            model.Anno = DateTime.Now.Year.ToString();
            model.Subscretaria = model.UnidadDescripcion.Contains("Turismo") ? "SUBSECRETARIO DE TURISMO" : "SUBSECRETARIA DE ECONOMÍA Y EMPRESAS DE MENOR TAMAÑO";
            model.FechaResolucion = DateTime.Now;
            model.Firma = false;
            model.NumeroResolucion = model.CometidoId;
            model.Destinos.FirstOrDefault().TotalViaticoPalabras = ExtensionesString.enletras(model.Destinos.FirstOrDefault().TotalViatico.ToString());

            /*se traen los datos de la tabla parrafos*/
            var parrafos = _repository.GetAll<Parrafos>();
            model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Orden).FirstOrDefault().ParrafoTexto;
            model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Firmante).FirstOrDefault().ParrafoTexto;
            model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoFirmante).FirstOrDefault().ParrafoTexto;
            model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistoOP).FirstOrDefault().ParrafoTexto;
            model.DejaseConstancia = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.DejaseConstancia).FirstOrDefault().ParrafoTexto;

            //switch (model.IdGrado)
            //{
            //    case "B":/*Firma Subsecretario*/
            //        model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenSubse).FirstOrDefault().ParrafoTexto;
            //        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteSubse).FirstOrDefault().ParrafoTexto;
            //        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoSubse).FirstOrDefault().ParrafoTexto;
            //        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
            //        break;
            //    case "C": /*firma ministro*/
            //        model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenMinistro).FirstOrDefault().ParrafoTexto;
            //        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteMinistro).FirstOrDefault().ParrafoTexto;
            //        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoMinistro).FirstOrDefault().ParrafoTexto;
            //        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
            //        break;
            //    default:
            //        model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Orden).FirstOrDefault().ParrafoTexto;
            //        model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Firmante).FirstOrDefault().ParrafoTexto;
            //        model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoFirmante).FirstOrDefault().ParrafoTexto;                    
            //        model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistoOP).FirstOrDefault().ParrafoTexto;
            //        model.DejaseConstancia = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.DejaseConstancia).FirstOrDefault().ParrafoTexto;
            //        break;
            //}

            /*Se valida que se encuentre en la tarea de Firma electronica para agregar folio y fecha de resolucion*/
            var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId) ?? null;
            if (workflowActual.DefinicionWorkflow.Secuencia == 8)
            {
                if (model.Folio == null)
                {
                    #region Folio
                    /*se va a buscar el folio de testing*/
                    DTOFolio folio = new DTOFolio();
                    folio.periodo = DateTime.Now.Year.ToString();
                    folio.solicitante = "Gestion Procesos - Cometidos";/*Sistema que solicita el numero de Folio*/
                    folio.tipodocumento = "OP";
                    //if (model.IdCalidad == 10)
                    //{
                    //    folio.tipodocumento = "RAEX";/*"ORPA";*/
                    //}
                    //else
                    //{
                    //    switch (model.IdGrado)
                    //    {
                    //        case "B":/*Resolución Ministerial Exenta*/
                    //            folio.tipodocumento = "RMEX";
                    //            break;
                    //        case "C": /*Resolución Ministerial Exenta*/
                    //            folio.tipodocumento = "RMEX";
                    //            break;
                    //        default:
                    //            /*Resolución Administrativa Exenta*/
                    //            break;
                    //    }
                    //}

                    //definir url
                    var url = "http://wsfolio.test.economia.cl/api/folio/";

                    //definir cliente http
                    var clientehttp = new WebClient();
                    clientehttp.Headers[HttpRequestHeader.ContentType] = "application/json";

                    //invocar metodo remoto
                    string result = clientehttp.UploadString(url, "POST", JsonConvert.SerializeObject(folio));

                    //convertir resultado en objeto 
                    var obj = JsonConvert.DeserializeObject<App.Model.DTO.DTOFolio>(result);

                    //verificar resultado
                    if (obj.status == "OK")
                    {
                        model.Folio = obj.folio;
                        model.FechaResolucion = DateTime.Now;
                        model.Firma = true;

                        _repository.Update(model);
                        _repository.Save();
                    }
                    if (obj.status == "ERROR")
                    {
                        TempData["Error"] = obj.error;
                        //return View(DTOFolio);
                    }
                    #endregion
                }
            }


            return View(model);
        }

        public ActionResult Resolucion(int id)
        {
            var model = _repository.GetById<Cometido>(id);

            model.Dias = (model.Destinos.LastOrDefault().FechaHasta.Date - model.Destinos.FirstOrDefault().FechaInicio.Date).Days + 1;
            model.DiasPlural = "s";
            model.Tiempo = model.Destinos.FirstOrDefault().FechaInicio < DateTime.Now ? "Pasado" : "Futuro";
            model.Anno = DateTime.Now.Year.ToString();
            model.Subscretaria = model.UnidadDescripcion.Contains("Turismo") ? "SUBSECRETARIO DE TURISMO" : "SUBSECRETARIA DE ECONOMÍA Y EMPRESAS DE MENOR TAMAÑO";
            model.FechaResolucion = DateTime.Now;
            model.Firma = false;
            model.NumeroResolucion = model.CometidoId;
            model.Destinos.FirstOrDefault().TotalViaticoPalabras = ExtensionesString.enletras(model.Destinos.FirstOrDefault().TotalViatico.ToString());

            /*se traen los datos de la tabla parrafos*/
            var parrafos = _repository.GetAll<Parrafos>();
            switch (model.IdGrado)
            {
                case "B":/*Firma Subsecretario*/
                    model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenSubse).FirstOrDefault().ParrafoTexto;
                    model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteSubse).FirstOrDefault().ParrafoTexto;
                    model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoSubse).FirstOrDefault().ParrafoTexto;
                    model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
                    break;
                case "C": /*firma ministro*/
                    model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.OrdenMinistro).FirstOrDefault().ParrafoTexto;
                    model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.FirmanteMinistro).FirstOrDefault().ParrafoTexto;
                    model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoMinistro).FirstOrDefault().ParrafoTexto;
                    model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistosRME).FirstOrDefault().ParrafoTexto;
                    break;
                default:
                    model.Orden = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Orden).FirstOrDefault().ParrafoTexto;
                    model.Firmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.Firmante).FirstOrDefault().ParrafoTexto;
                    model.CargoFirmante = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.CargoFirmante).FirstOrDefault().ParrafoTexto;
                    model.Vistos = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.VistoRAE).FirstOrDefault().ParrafoTexto;
                    var vit = _repository.Get<Parrafos>(c => c.ParrafosId == (int)App.Util.Enum.Firmas.ViaticodeVuelta && c.ParrafoActivo == true);
                    if (vit != null)
                        model.ViaticodeVuelta = parrafos.Where(p => p.ParrafosId == (int)App.Util.Enum.Firmas.ViaticodeVuelta).FirstOrDefault().ParrafoTexto;
                    else
                        model.ViaticodeVuelta = string.Empty;
                    break;
            }

            /*Se valida que se encuentre en la tarea de Firma electronica para agregar folio y fecha de resolucion*/
            var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == model.WorkflowId) ?? null;
            //if (workflowActual.DefinicionWorkflow.Secuencia == 8)
            //{
            //    if (model.Folio == null)
            //    {
            //        #region Folio
            //        /*se va a buscar el folio de testing*/
            //        DTOFolio folio = new DTOFolio();
            //        folio.periodo = DateTime.Now.Year.ToString();
            //        folio.solicitante = "Gestion Procesos - Cometidos";/*Sistema que solicita el numero de Folio*/
            //        if (model.IdCalidad == 10)
            //        {
            //            folio.tipodocumento = "RAEX";/*"ORPA";*/
            //        }
            //        else
            //        {
            //            switch (model.IdGrado)
            //            {
            //                case "B":/*Resolución Ministerial Exenta*/
            //                    folio.tipodocumento = "RMEX";
            //                    break;
            //                case "C": /*Resolución Ministerial Exenta*/
            //                    folio.tipodocumento = "RMEX";
            //                    break;
            //                default:
            //                    folio.tipodocumento = "RAEX";/*Resolución Administrativa Exenta*/
            //                    break;
            //            }
            //        }


            //        //definir url
            //        var url = "http://wsfolio.test.economia.cl/api/folio/";

            //        //definir cliente http
            //        var clientehttp = new WebClient();
            //        clientehttp.Headers[HttpRequestHeader.ContentType] = "application/json";

            //        //invocar metodo remoto
            //        string result = clientehttp.UploadString(url, "POST", JsonConvert.SerializeObject(folio));

            //        //convertir resultado en objeto 
            //        var obj = JsonConvert.DeserializeObject<App.App.Model.DTO.DTOFolio>(result);

            //        //verificar resultado
            //        if (obj.status == "OK")
            //        {
            //            model.Folio = obj.folio;
            //            model.FechaResolucion = DateTime.Now;
            //            model.Firma = true;

            //            _repository.Update(model);
            //            _repository.Save();
            //        }
            //        if (obj.status == "ERROR")
            //        {
            //            TempData["Error"] = obj.error;
            //        }
            //        #endregion
            //    }
            //}

            if (workflowActual.DefinicionWorkflow.Secuencia == 13 || workflowActual.DefinicionWorkflow.Secuencia == 14 || workflowActual.DefinicionWorkflow.Secuencia == 15 && workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
            {
                if (model.Folio == null)
                {
                    #region Folio
                    /*se va a buscar el folio de testing*/
                    DTOFolio folio = new DTOFolio();
                    folio.periodo = DateTime.Now.Year.ToString();
                    folio.solicitante = "Gestion Procesos - Cometidos";/*Sistema que solicita el numero de Folio*/
                    if (model.IdCalidad == 10)
                    {
                        folio.tipodocumento = "RAEX";/*"ORPA";*/
                    }
                    else
                    {
                        switch (model.IdGrado)
                        {
                            case "B":/*Resolución Ministerial Exenta*/
                                folio.tipodocumento = "RMEX";
                                break;
                            case "C": /*Resolución Ministerial Exenta*/
                                folio.tipodocumento = "RMEX";
                                break;
                            default:
                                folio.tipodocumento = "RAEX";/*Resolución Administrativa Exenta*/
                                break;
                        }
                    }


                    //definir url
                    var url = "http://wsfolio.test.economia.cl/api/folio/";

                    //definir cliente http
                    var clientehttp = new WebClient();
                    clientehttp.Headers[HttpRequestHeader.ContentType] = "application/json";

                    //invocar metodo remoto
                    string result = clientehttp.UploadString(url, "POST", JsonConvert.SerializeObject(folio));

                    //convertir resultado en objeto 
                    var obj = JsonConvert.DeserializeObject<App.Model.DTO.DTOFolio>(result);

                    //verificar resultado
                    if (obj.status == "OK")
                    {
                        model.Folio = obj.folio;
                        model.FechaResolucion = DateTime.Now;
                        model.Firma = true;

                        _repository.Update(model);
                        _repository.Save();
                    }
                    if (obj.status == "ERROR")
                    {
                        TempData["Error"] = obj.error;
                    }
                    #endregion
                }
            }

            if (model.GradoDescripcion == "0")
            {
                return View(model);
            }
            else
            {
                return View(model);
            }            
    }


        public ActionResult Anular(int id)
        {
            var model = _repository.GetById<Workflow>(id);
            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Anular(Workflow model)
        //{
        //    model.Email = UserExtended.Email(User);

        //    if (ModelState.IsValid)
        //    {
        //        var _useCaseInteractor = new UseCaseInteractorCustom(_repository, _email, _sigper);
        //        ResponseMessage _UseCaseResponseMessageDelete = new ResponseMessage();

        //        var workflow = _repository.Get<Workflow>(c => c.WorkflowId == model.WorkflowId).FirstOrDefault();
        //        var Entity = _repository.Get<DefinicionWorkflow>(d => d.DefinicionWorkflowId == workflow.DefinicionWorkflowId).FirstOrDefault().EntidadId;
        //        if (Entity == 8)/*Se eliminan datos del destino, luego el cometido*/
        //        {
        //            var cometido = _repository.Get<Cometido>(c => c.CometidoId == workflow.EntityId.Value).FirstOrDefault();
        //            var destinos = _repository.Get<Destinos>(c => c.CometidoId == cometido.CometidoId);
        //            var cdp = _repository.Get<GeneracionCDP>(c => c.CometidoId == cometido.CometidoId);
        //            if (destinos != null)
        //            {
        //                foreach (var des in destinos)
        //                {
        //                    _UseCaseResponseMessageDelete = _useCaseInteractor.DestinosAnular(des.DestinoId);
        //                }
        //            }

        //            if (cdp != null)
        //            {
        //                foreach (var c in cdp)
        //                {
        //                    _UseCaseResponseMessageDelete = _useCaseInteractor.GeneracionCDPAnular(c.GeneracionCDPId);
        //                }
        //            }



        //            _UseCaseResponseMessageDelete = _useCaseInteractor.CometidoAnular(cometido.CometidoId);
        //        }

        //        /*se elimina datos del proceso*/
        //        //var _UseCaseResponseMessage = _useCaseInteractorCore.WorkflowArchive(model);
        //        //if (_UseCaseResponseMessage.IsValid && _UseCaseResponseMessageDelete.IsValid)
        //        //{
        //        //    TempData["Success"] = "Operación terminada correctamente.";
        //        //    return RedirectToAction("Index", "Workflow");
        //        //}
        //        //else
        //        //    TempData["Error"] = _UseCaseResponseMessage.Errors;
        //    }

        //    return View(model);
        //}
    }
}