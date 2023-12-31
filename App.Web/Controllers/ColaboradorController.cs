﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Model.Core;
using App.Model.DTO;
//using App.Model.Shared;
using App.Core.Interfaces;
using App.Core.UseCases;
using App.Model.HorasExtras;
using App.Model.Cometido;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
    public class ColaboradorController : Controller
    {
        private readonly IGestionProcesos _repository;
        private readonly ISigper _sigper;
        private static List<DTODomainUser> ActiveDirectoryUsers { get; set; }
        public ColaboradorController(IGestionProcesos repository, ISigper sigper)
        {
            _repository = repository;
            _sigper = sigper;

            if (ActiveDirectoryUsers == null)
                ActiveDirectoryUsers = AuthenticationService.GetDomainUser().ToList();
        }

        public DTOImputacion Imputacion(int Rut)
        {
            var per = _sigper.GetUserByRut(Rut);
            var imputacion = new DTOImputacion();

            var ProgId = per.Contrato.Re_ConPyt != 0 ? per.Contrato.Re_ConPyt : 1;
            int Iditem;
            int Idasignacion = 0;
            int Idsubtitulo = ProgId == 5 || ProgId == 7 || ProgId == 11 || ProgId == 12 || ProgId == 16 || ProgId == 17 || ProgId == 18 || ProgId == 19 || ProgId == 20 ? 3 : 1;
            if (ProgId == 21)
            {
                Iditem = 2;
                Idasignacion = 2; //004
            }
            else if (ProgId == 22)
            {
                Iditem = 1;
                Idasignacion = 2; //004
            }
            else
                Iditem = 3;

            switch (ProgId)
            {
                case 1:
                    Idasignacion = 1;
                    break;
                case 2:
                    Idasignacion = 1;
                    break;
                case 3:
                    Idasignacion = 1;
                    break;
                case 4:
                    Idasignacion = 1;
                    break;
                case 5:
                    Idasignacion = 4;// 472;
                    break;
                case 7:
                    Idasignacion = 12;// 477;
                    break;
                case 11:
                    Idasignacion = 15; // 214.05.008;
                    break;
                case 12:
                    Idasignacion = 1;// 001;
                    break;
                case 16:
                    Idasignacion = 16;// 413;
                    break;
                case 17:
                    Idasignacion = 17;// 611;
                    break;
                case 18:
                    Idasignacion = 18;//612;
                    break;
                case 19:
                    Idasignacion = 19;// 613;
                    break;
                case 20:
                    Idasignacion = 20;// 614;
                    break;
                case 21:
                    Idasignacion = 2;
                    break;
                case 22:
                    Idasignacion = 2;
                    break;
            }

            imputacion.Item = Iditem;
            imputacion.Asignacion = Idasignacion;
            imputacion.Subtitulo = Idsubtitulo;

            return imputacion;
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
            var IdCargo = per.DatosLaborales.RhConCar.Value;
            var cargo = string.IsNullOrEmpty(per.DatosLaborales.RhConEsc.Trim()) ? "S/A" : _sigper.GetPECARGOs().Where(e => e.Pl_CodCar == per.DatosLaborales.RhConCar).FirstOrDefault().Pl_DesCar.Trim();
            var IdCalidad = per.DatosLaborales.RH_ContCod;
            var calidad = string.IsNullOrEmpty(per.DatosLaborales.RhConCar.ToString()) ? "S/A" : _sigper.GetDGCONTRATOS().Where(c => c.RH_ContCod == per.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
            var IdGrado = string.IsNullOrEmpty(per.DatosLaborales.RhConGra.Trim()) ? "0" : per.DatosLaborales.RhConGra.Trim();
            var grado = string.IsNullOrEmpty(per.DatosLaborales.RhConGra.Trim()) ? "Sin Grado" : per.DatosLaborales.RhConGra.Trim();
            var estamento = per.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == per.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
            var ProgId = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
            var Programa = ProgId != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgId).FirstOrDefault()?.RePytDes : "S/A";
            var conglomerado = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte).ReContraSed;
            var jefatura = per.Jefatura != null ? per.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreorem = per.Funcionario != null ? per.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            //var nombrerem = per.Funcionario != null ? per.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

            string rut;
            if (per.Funcionario.RH_NumInte.ToString().Length < 8)
            {
                string t = per.Funcionario.RH_NumInte.ToString();
                rut = string.Concat("0", t);
            }
            else
            {
                rut = per.Funcionario.RH_NumInte.ToString();
            }

            return Json(new
            {
                Rut = rut,
                DV = per.Funcionario.RH_DvNuInt,
                IdCargo,
                Cargo = cargo,
                IdCalidad,
                CalidadJuridica = calidad,
                IdGrado,
                Grado = grado,
                Estamento = estamento,
                Programa = Programa.Trim(),
                Conglomerado = conglomerado,
                IdUnidad = per.Unidad.Pl_UndCod,
                Unidad = per.Unidad.Pl_UndDes.Trim(),
                Jefatura = jefatura,
                EmailRem = ecorreorem,
                NombreChqRem = per.Funcionario.PeDatPerChq.Trim(),

            }, JsonRequestBehavior.AllowGet);

        }

        //public JsonResult GetImputacion(int Rut)
        //{
        //    var correo = _sigper.GetUserByRut(Rut).Funcionario.Rh_Mail.Trim();
        //    var per = _sigper.GetUserByEmail(correo);

        //    var ProgId = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
        //    //var Programa = ProgId != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgId).FirstOrDefault().RePytDes : "S/A";
        //    int Iditem;
        //    int Idasignacion = 0;
        //    int Idsubtitulo = ProgId == 5 || ProgId == 7 || ProgId == 11 || ProgId == 12 || ProgId == 16 || ProgId == 17 || ProgId == 18 || ProgId == 19 || ProgId == 20 ? 3 : 1;
        //    if (ProgId == 21)
        //    {
        //        Iditem = 2;
        //        Idasignacion = 2; //004
        //    }
        //    else if (ProgId == 22)
        //    {
        //        Iditem = 1;
        //        Idasignacion = 2; //004
        //    }
        //    else
        //        Iditem = 3;

        //    switch (ProgId)
        //    {
        //        case 1:
        //            Idasignacion = 1;
        //            break;
        //        case 2:
        //            Idasignacion = 1;
        //            break;
        //        case 3:
        //            Idasignacion = 1;
        //            break;
        //        case 4:
        //            Idasignacion = 1;
        //            break;
        //        case 5:
        //            Idasignacion = 4;// 472;
        //            break;
        //        case 7:
        //            Idasignacion = 12;// 477;
        //            break;
        //        case 11:
        //            Idasignacion = 15; // 214.05.008;
        //            break;
        //        case 12:
        //            Idasignacion = 1;// 001;
        //            break;
        //        case 16:
        //            Idasignacion = 16;// 413;
        //            break;
        //        case 17:
        //            Idasignacion = 17;// 611;
        //            break;
        //        case 18:
        //            Idasignacion = 18;//612;
        //            break;
        //        case 19:
        //            Idasignacion = 19;// 613;
        //            break;
        //        case 20:
        //            Idasignacion = 20;// 614;
        //            break;
        //        case 21:
        //            Idasignacion = 2;
        //            break;
        //        case 22:
        //            Idasignacion = 2;
        //            break;
        //    }

        //    return Json(new
        //    {
        //        Iditem = Iditem,
        //        Idasignacion = Idasignacion,
        //        Idsubtitulo = Idsubtitulo,
        //    }, JsonRequestBehavior.AllowGet);

        //}
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult View(int id)
        {
            //var persona = _sigper.GetUserByEmail(User.Email());
            var model = _repository.GetById<Colaborador>(id);

            return View(model);
        }

        public ActionResult Details(int id)
        {
            //var persona = _sigper.GetUserByEmail(User.Email());
            //var usuarios = new SelectList(_sigper.GetAllUsers().Where(c => c.Rh_Mail.Contains("economia")), "RH_NumInte", "PeDatPerChq");
            var model = _repository.GetById<Colaborador>(id);

            return View(model);
        }

        public ActionResult Validate(int id)
        {
            var model = _repository.GetById<Colaborador>(id);
            return View(model);
        }

        public ActionResult Create(int? Id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
            var HorasExtras = _repository.GetById<HorasExtras>(Id);

            //ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidadWithoutHonorarios(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");

            var model = new Colaborador();
            //model.HorasExtras = HorasExtras;
            model.HorasExtrasId = HorasExtras.HorasExtrasId;


            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Colaborador model)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
            var imp = Imputacion(model.NombreId.Value);
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");

            model.HDPagoAprobados = model.HDPago;
            model.HDCompensarAprobados = model.HDCompensar;
            model.HNPagoAprobados = model.HNPago;
            model.HNCompensarAprobados = model.HNCompensar;
            model.TipoSubTituloId = imp.Subtitulo;
            model.TipoItemId = imp.Item;
            model.TipoAsignacionId = imp.Asignacion;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseHorasExtras(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.ColaboradorInsert(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";

                    /*se redireccina a la vista que llamo al metodo de crear*/
                    var hrs = _repository.GetFirst<HorasExtras>(c => c.HorasExtrasId == model.HorasExtrasId);
                    var secuencia = _repository.Get<Workflow>(p => p.ProcesoId == hrs.ProcesoId).OrderByDescending(c => c.WorkflowId).FirstOrDefault().DefinicionWorkflow.Secuencia;
                    if (secuencia == 1 || secuencia == 2)
                        return RedirectToAction("Edit", "HorasExtras", new { id = hrs.HorasExtrasId });
                    else
                        return RedirectToAction("EditGP", "HorasExtras", new { id = hrs.HorasExtrasId });
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
            var model = _repository.GetById<Colaborador>(id);
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq", model.NombreId);

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(Colaborador model)
        {
            model.HDPagoAprobados = model.HDPago;
            model.HDCompensarAprobados = model.HDCompensar;
            model.HNPagoAprobados = model.HNPago;
            model.HNCompensarAprobados = model.HNCompensar;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseHorasExtras(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.ColaboradorUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Edit", "HorasExtras", new { id = model.HorasExtrasId });
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq", model.NombreId);

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<Colaborador>(id);
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq", model.NombreId);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseHorasExtras(_repository);
            var model = _repository.GetById<Colaborador>(id);
            var HrsId = model.HorasExtrasId;
            var _UseCaseResponseMessage = _useCaseInteractor.ColaboradorDelete(id);


            if (_UseCaseResponseMessage.IsValid)
            {
                TempData["Success"] = "Operación terminada correctamente.";

                /*se redireccina a la vista que llamo al metodo de borrar*/
                var com = _repository.GetFirst<HorasExtras>(c => c.HorasExtrasId == HrsId);
                var pro = _repository.GetExists<Workflow>(p => p.ProcesoId == com.ProcesoId && p.DefinicionWorkflow.Secuencia == 6);
                if (pro)
                    return RedirectToAction("Edit", "HorasExtras", new { id = HrsId });

                return RedirectToAction("Edit", "HorasExtras", new { id = HrsId });
            }


            foreach (var item in _UseCaseResponseMessage.Errors)
            {
                ModelState.AddModelError(string.Empty, item);

                var persona = _sigper.GetUserByEmail(User.Email());
                ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq", model.NombreId);
            }
            return View(model);
        }

        public ActionResult EditGP(int id)
        {
            var model = _repository.GetById<Colaborador>(id);
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq", model.NombreId);

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditGP(Colaborador model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseHorasExtras(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.ColaboradorUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    /*se redireccina a la vista que llamo al metodo de borrar*/
                    var he = _repository.GetFirst<HorasExtras>(c => c.HorasExtrasId == model.HorasExtrasId);
                    var pro = _repository.Get<Workflow>(p => p.ProcesoId == he.ProcesoId).Where(c => c.DefinicionWorkflow.Secuencia == 3 || c.DefinicionWorkflow.Secuencia == 4);
                    if (pro.Any())
                        return RedirectToAction("EditGP", "HorasExtras", new { id = he.HorasExtrasId });
                    else
                        return RedirectToAction("Edit", "HorasExtras", new { id = he.HorasExtrasId });

                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq", model.NombreId);

            return View(model);
        }

        public ActionResult EditPpto(int id)
        {
            var model = _repository.GetById<Colaborador>(id);
            //var persona = _sigper.GetUserByEmail(User.Email());

            ViewBag.TipoItemId = new SelectList(_repository.GetAll<TipoItem>(), "TipoItemId", "TitNombre", model.TipoItemId);
            ViewBag.TipoAsignacionId = new SelectList(_repository.GetAll<TipoAsignacion>(), "TipoAsignacionId", "TasNombre", model.TipoAsignacionId);
            ViewBag.TipoSubTituloId = new SelectList(_repository.GetAll<TipoSubTitulo>(), "TipoSubTituloId", "TstNombre", model.TipoSubTituloId);

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditPpto(Colaborador model)
        {
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseHorasExtras(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.ColaboradorUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    /*se redireccina a la vista que llamo al metodo de borrar*/
                    var he = _repository.GetFirst<HorasExtras>(c => c.HorasExtrasId == model.HorasExtrasId);
                    var pro = _repository.GetExists<Workflow>(p => p.ProcesoId == he.ProcesoId && (p.DefinicionWorkflow.Secuencia == 3 || p.DefinicionWorkflow.Secuencia == 4));
                    if (pro)
                        return RedirectToAction("Details", "HorasExtras", new { id = he.HorasExtrasId });

                    return RedirectToAction("Details", "HorasExtras", new { id = he.HorasExtrasId });

                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq", model.NombreId);

            return View(model);
        }
    }
}