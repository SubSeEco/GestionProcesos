using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Model.Comisiones;
using App.Model.Core;
//using App.Model.Shared;
using App.Core.Interfaces;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]
    public class ComisionController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        private static List<Model.DTO.DTODomainUser> ActiveDirectoryUsers { get; set; }
        public static List<DestinosComision> ListDestino = new List<DestinosComision>();


        public ComisionController(IGestionProcesos repository, ISIGPER sigper)
        {
            _repository = repository;
            _sigper = sigper;

            if (ActiveDirectoryUsers == null)
                ActiveDirectoryUsers = AuthenticationService.GetDomainUser().ToList();
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
            var Programa = ProgId != 0 ? _sigper.GetREPYTs().Where(c => c.RePytCod == ProgId).FirstOrDefault().RePytDes : "S/A";
            var conglomerado = _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == per.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == per.Funcionario.RH_NumInte).ReContraSed;

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
                Unidad = per.Unidad.Pl_UndDes.Trim()
            }, JsonRequestBehavior.AllowGet);
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

        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<Comisiones>();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Comisiones>(id);
            if (model.GeneracionCDPComision.Count > 0)
            {
                var cdp = _repository.GetById<GeneracionCDPComision>(model.GeneracionCDPComision.FirstOrDefault().GeneracionCDPComisionId);
                model.GeneracionCDPComision.Add(cdp);
            }

            return View(model);
        }

        public ActionResult View(int id)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo");
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");

            var model = _repository.GetById<Comisiones>(id);
            return View(model);
        }

        public ActionResult Create(int? WorkFlowId, int? ProcesoId)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo");
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");

            var user = User.Identity.Name;
            var host = System.Net.Dns.GetHostEntry(Request.UserHostAddress).HostName;
            ViewBag.Host = host.ToString();
            ViewBag.Name = user.ToString();

           var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new Comisiones
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
                model.IdCargo = persona.DatosLaborales.RhConCar.Value;
                model.CargoDescripcion = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.DatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar.Trim();
                model.IdCalidad = persona.DatosLaborales.RH_ContCod;/*id calidad juridica*/
                model.CalidadDescripcion = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.DatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                model.IdGrado = string.IsNullOrEmpty(persona.DatosLaborales.RhConGra.Trim()) ? "0" : persona.DatosLaborales.RhConGra.Trim();
                model.GradoDescripcion = string.IsNullOrEmpty(persona.DatosLaborales.RhConGra.Trim()) ? "0" : persona.DatosLaborales.RhConGra;
                model.IdEstamento = persona.DatosLaborales.PeDatLabEst;
                model.EstamentoDescripcion = persona.DatosLaborales.PeDatLabEst == 0 ? "" : _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.DatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim();
                model.FinanciaOrganismo = false;
                model.Vehiculo = false;
                model.SolicitaReembolso = true;
                model.IdConglomerado = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed;
                model.ConglomeradoDescripcion = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().ReContraSed.ToString();
                model.IdPrograma = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? 0 : Convert.ToInt32(_sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt);
                //model.ProgramaDescripcion = _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault(c => c.RH_NumInte == persona.Funcionario.RH_NumInte) == null ? "0" : _sigper.GetReContra().Where(c => c.RH_NumInte == persona.Funcionario.RH_NumInte).FirstOrDefault().Re_ConPyt.ToString();
                model.ProgramaDescripcion = model.IdPrograma != null ? _sigper.GetREPYTs().Where(c => c.RePytCod == model.IdPrograma).FirstOrDefault().RePytDes : "S/A";

                model.DestinosComision = ListDestino;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Comisiones model)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);

            model.FechaSolicitud = DateTime.Now;
            model.SolicitaReembolso = true;
            model.DestinosComision = ListDestino;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.ComisionesInsert(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Edit", "Comision", new { model.WorkflowId, id = model.ComisionesId });
                }
                foreach (var item in _UseCaseResponseMessage.Errors)
                {
                    ModelState.AddModelError(string.Empty, item);
                }
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _repository.GetById<Comisiones>(id);

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            ViewBag.FinanciaOrganismo = model.FinanciaOrganismo;
            ViewBag.Alimentacion = model.Alimentacion;
            ViewBag.Alojamiento = model.Alojamiento;
            ViewBag.Pasajes = model.Pasajes;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Comisiones model)
        {
            
            //var com = _repository.GetById<Comisiones>(model.ComisionesId);
            //model.Alojamiento = com.Alojamiento;
            //model.Alimentacion = com.Alimentacion;
            //model.Pasajes = com.Pasajes;

            if (ModelState.IsValid)
            {
                //var _useCaseInteractor = new UseCaseInteractorCustom(_repository, _sigper);
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.ComisionesUpdate(model);

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
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
            }
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);

            return View(model);
        }

        public ActionResult EditGP(int id)
        {
            var model = _repository.GetById<Comisiones>(id);

            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);
            ViewBag.FinanciaOrganismo = model.FinanciaOrganismo;
            ViewBag.Alimentacion = model.Alimentacion;
            ViewBag.Alojamiento = model.Alojamiento;
            ViewBag.Pasajes = model.Pasajes;

            return View(model);
        }

        [HttpPost]
        public ActionResult EditGP(Comisiones model)
        {

            //var com = _repository.GetById<Comisiones>(model.ComisionesId);
            //model.Alojamiento = com.Alojamiento;
            //model.Alimentacion = com.Alimentacion;
            //model.Pasajes = com.Pasajes;

            if (ModelState.IsValid)
            {
                //var _useCaseInteractor = new UseCaseInteractorCustom(_repository, _sigper);
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.ComisionesUpdate(model);

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
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
            }
            ViewBag.TipoVehiculoId = new SelectList(_repository.Get<SIGPERTipoVehiculo>().OrderBy(q => q.SIGPERTipoVehiculoId), "SIGPERTipoVehiculoId", "Vehiculo", model.TipoVehiculoId);

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<Comisiones>(id);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseCometidoComision(_repository);
            var _UseCaseResponseMessage = _useCaseInteractor.ComisionesDelete(id);

            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            return RedirectToAction("Index");
        }
    }
}