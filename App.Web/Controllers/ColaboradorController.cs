using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using App.Model.ProgramacionHorasExtraordinarias;
using App.Model.Core;
using App.Model.DTO;
//using App.Model.Shared;
using App.Core.Interfaces;
using App.Model.Shared;
using App.Util;
using Newtonsoft.Json;
using App.Core.UseCases;
using App.Model.Comisiones;
using System.ComponentModel.DataAnnotations;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using Rotativa;
using App.Model.HorasExtras;
using App.Model.Cometido;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class ColaboradorController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IFolio _folio;
        protected readonly IHSM _hsm;
        protected readonly IEmail _email;
        private static List<App.Model.DTO.DTODomainUser> ActiveDirectoryUsers { get; set; }
        public ColaboradorController(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IHSM hsm, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
            _email = email;

            //if (tipoDocumentoList == null)
            //    tipoDocumentoList = _folio.GetTipoDocumento();

            if (ActiveDirectoryUsers == null)
                ActiveDirectoryUsers = AuthenticationService.GetDomainUser().ToList();
        }

        public DTOImputacion Imputacion(int Rut)
        {
            var correo = _sigper.GetUserByRut(Rut).Funcionario.Rh_Mail.Trim();
            var per = _sigper.GetUserByEmail(correo);
            DTOImputacion Imputacion = new DTOImputacion();

            var ProgId = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte) == null ? 0 : (int)_sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt;
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

            Imputacion.Item = Iditem;
            Imputacion.Asignacion = Idasignacion;
            Imputacion.Subtitulo = Idsubtitulo;

            return Imputacion;
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
            var jefatura = per.Jefatura != null ? per.Jefatura.PeDatPerChq : "Sin jefatura definida";
            var ecorreorem = per.Funcionario != null ? per.Funcionario.Rh_Mail.Trim() : "Sin correo definido";
            var nombrerem = per.Funcionario != null ? per.Funcionario.PeDatPerChq.Trim() : "Sin nombre definido";

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

            return Json(new
            {
                Rut = rut,
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
            var persona = _sigper.GetUserByEmail(User.Email());
            var model = _repository.GetById<Colaborador>(id);

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
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

            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");

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

                    /*se redireccina a la vista que llamo al metodo de borrar*/
                    var com = _repository.Get<HorasExtras>(c => c.HorasExtrasId == model.HorasExtrasId).FirstOrDefault();
                    var secuencia = _repository.Get<Workflow>(p => p.ProcesoId == com.ProcesoId).OrderByDescending(c =>c.WorkflowId).FirstOrDefault().DefinicionWorkflow.Secuencia;
                    if (secuencia == 1)
                        return RedirectToAction("Edit", "HorasExtras", new {  id = com.HorasExtrasId});
                    else
                        return RedirectToAction("EditGP", "HorasExtras", new { id = com.HorasExtrasId });
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
            //model.HorasExtras = _repository.Get<HorasExtras>(c => c.HorasExtrasId == model.HorasExtrasId).FirstOrDefault();

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
                    return RedirectToAction("Edit", "HorasExtras", new { id = model.HorasExtrasId});
                }

                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq",model.NombreId);

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
                var com = _repository.Get<HorasExtras>(c => c.HorasExtrasId == HrsId).FirstOrDefault();
                var pro = _repository.Get<Workflow>(p => p.ProcesoId == com.ProcesoId).Where(c => c.DefinicionWorkflow.Secuencia == 6);
                if (pro.Count() > 0)
                    return RedirectToAction("Edit", "HorasExtras", new { id = HrsId });
                else
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
            //model.HorasExtras = _repository.Get<HorasExtras>(c => c.HorasExtrasId == model.HorasExtrasId).FirstOrDefault();

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
                    var he = _repository.Get<HorasExtras>(c => c.HorasExtrasId == model.HorasExtrasId).FirstOrDefault();
                    var pro = _repository.Get<Workflow>(p => p.ProcesoId == he.ProcesoId).Where(c => c.DefinicionWorkflow.Secuencia == 3 || c.DefinicionWorkflow.Secuencia == 4);
                    if (pro.Count() > 0)
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
            var persona = _sigper.GetUserByEmail(User.Email());

            ViewBag.TipoItemId = new SelectList(_repository.GetAll<TipoItem>(), "TipoItemId", "TitNombre",model.TipoItemId);
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
                    var he = _repository.Get<HorasExtras>(c => c.HorasExtrasId == model.HorasExtrasId).FirstOrDefault();
                    var pro = _repository.Get<Workflow>(p => p.ProcesoId == he.ProcesoId).Where(c => c.DefinicionWorkflow.Secuencia == 3 || c.DefinicionWorkflow.Secuencia == 4);
                    if (pro.Count() > 0)
                        return RedirectToAction("Details", "HorasExtras", new { id = he.HorasExtrasId });
                    else
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