using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Model.Cometido;
using App.Model.Core;
using App.Model.Pasajes;
using App.Model.Shared;
//using App.Model.Shared;
using App.Core.Interfaces;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class PasajeController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly ISIGPER _sigper;

        public PasajeController(IGestionProcesos repository, ISIGPER sigper)
        {
            _repository = repository;
            _sigper = sigper;
        }

        public JsonResult GetCiudad(string IdPais)
        {
            var ciudad = _repository.Get<Ciudad>().Where(p => p.PaisId == int.Parse(IdPais));
            return Json(ciudad.Select(q => new { value = q.CiudadId, text = q.CiudadNombre.Trim()}),JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetComunas(string IdRegion)
        {
            var comunas = _sigper.GetComunasbyRegion(IdRegion);
            return Json(comunas.Select(q => new { value = q.Pl_CodCom.Trim(), text = q.Pl_DesCom.Trim() }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuario(int Rut)
        {
            var correo = _sigper.GetUserByRut(Rut).Funcionario.Rh_Mail;
            var per = _sigper.GetUserByEmail(correo);
            return Json(new
            {
                Rut = per.Funcionario.RH_NumInte,
                DV = per.Funcionario.RH_DvNuInt.ToString(),
                Cargo = _sigper.GetGESCALAFONEs().Where(e => e.Pl_CodEsc == per.FunDatosLaborales.RhConEsc).FirstOrDefault().Pl_DesEsc.Trim(),
                CalidadJuridica = _sigper.GetGESCALAFONEs().Where(e => e.Pl_CodEsc == per.FunDatosLaborales.RhConEsc).FirstOrDefault().Pl_DesEsc.Trim(),
                Grado = per.FunDatosLaborales.RhConGra.Trim(),
                Estamento = _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == per.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc.Trim(),
                Programa = "S/A",
                Conglomerado = "S/A",
                Unidad = per.Unidad.Pl_UndDes.Trim()
            }, JsonRequestBehavior.AllowGet);


            //return Json("ok",JsonRequestBehavior.AllowGet);
            //return Json(per.Funcionario, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = _repository.GetAll<Pasaje>();
            return View(model);
        }

        public ActionResult Menu()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            var model = _repository.GetById<Pasaje>(id);
            ViewBag.IdComunaOrigen = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            ViewBag.IdCiudadOrigen = new SelectList(_repository.Get<Ciudad>(), "CiudadId", "CiudadNombre");

            return View(model);
        }

        public ActionResult Create(int? WorkFlowId, int? ProcesoId)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(0), "RH_NumInte", "PeDatPerChq");

            //ViewBag.IdComunaOrigen = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            //ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            //ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            //ViewBag.IdCiudadOrigen = new SelectList(_repository.Get<Ciudad>(), "CiudadId", "CiudadNombre");

            var workflow = _repository.GetById<Workflow>(WorkFlowId);
            var model = new Pasaje
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
                model.FechaSolicitud = DateTime.Now.Date;
                model.Rut = persona.Funcionario.RH_NumInte;
                model.DV = persona.Funcionario.RH_DvNuInt.Trim();
                model.NombreId = persona.Funcionario.RH_NumInte;
                model.Nombre = persona.Funcionario.PeDatPerChq.Trim();
                //model.IdCargo = persona.FunDatosLaborales.RhConCar.Value;
                //model.CargoDescripcion = _sigper.GetPECARGOs().Where(c => c.Pl_CodCar == persona.FunDatosLaborales.RhConCar.Value).FirstOrDefault().Pl_DesCar;
                model.IdCalidad = persona.FunDatosLaborales.RH_ContCod;/*id calidad juridica*/
                //model.CalidadDescripcion = _sigper.GetGESCALAFONEs().Where(e => e.Pl_CodEsc == persona.FunDatosLaborales.RH_ContCod.ToString()).FirstOrDefault().Pl_DesEsc;
                model.CalidadDescripcion = _sigper.GetDGCONTRATOS().Where(e => e.RH_ContCod == persona.FunDatosLaborales.RH_ContCod).FirstOrDefault().RH_ContDes.Trim();
                //model.IdGrado = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra) ? int.Parse(persona.FunDatosLaborales.RhConGra) : 0;
                //model.GradoDescripcion = string.IsNullOrEmpty(persona.FunDatosLaborales.RhConGra) ? persona.FunDatosLaborales.RhConGra : "0";
                //model.IdEstamento = persona.FunDatosLaborales.PeDatLabEst;
                //model.EstamentoDescripcion = _sigper.GetDGESTAMENTOs().Where(e => e.DgEstCod.ToString() == persona.FunDatosLaborales.PeDatLabEst.Value.ToString()).FirstOrDefault().DgEstDsc;
                //model.FinanciaOrganismo = false;
                //model.Vehiculo = false;
                //model.SolicitaReembolso = false;
                //model.TipoDestino = true;
            }


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pasaje model)
        {
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");            

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.PasajeInsert(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {

                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Edit", "Pasaje", new { model.WorkflowId, id = model.PasajeId });
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
            var model = _repository.GetById<Pasaje>(id);
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdComunaOrigen = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            ViewBag.IdCiudadOrigen = new SelectList(_repository.Get<Ciudad>(), "CiudadId", "CiudadNombre");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pasaje model)
        {
            //var model = _repository.GetById<Pasaje>(id);
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdComunaOrigen = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            ViewBag.IdCiudadOrigen = new SelectList(_repository.Get<Ciudad>(), "CiudadId", "CiudadNombre");


            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.PasajeUpdate(model);

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

                //TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
            }

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = _repository.GetById<Pasaje>(id);
            ViewBag.IdComunaOrigen = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            ViewBag.IdCiudadOrigen = new SelectList(_repository.Get<Ciudad>(), "CiudadId", "CiudadNombre");

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var _useCaseInteractor = new UseCaseCometidoComision(_repository);
            var _UseCaseResponseMessage = _useCaseInteractor.PasajeDelete(id);

            if (_UseCaseResponseMessage.IsValid)
                TempData["Success"] = "Operación terminada correctamente.";
            else
                TempData["Error"] = _UseCaseResponseMessage.Errors;

            return RedirectToAction("Index");
        }

        public ActionResult EditAbast(int id)
        {
            var model = _repository.GetById<Pasaje>(id);
            var persona = _sigper.GetUserByEmail(User.Email());
            ViewBag.NombreId = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod), "RH_NumInte", "PeDatPerChq");
            ViewBag.IdComunaOrigen = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            ViewBag.IdCiudadOrigen = new SelectList(_repository.Get<Ciudad>(), "CiudadId", "CiudadNombre");
            //ViewBag.Cometido = _repository.Get<Cometido>(c => c.ProcesoId.Value == model.ProcesoId.Value).FirstOrDefault();


            ViewBag.Cometido = _repository.Get<Cometido>(c => c.ProcesoId.Value == model.ProcesoId.Value).FirstOrDefault();
            var pasaje = _repository.Get<Pasaje>(p => p.ProcesoId.Value == model.ProcesoId.Value).FirstOrDefault();
            ViewBag.Pasaje = pasaje;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAbast(Pasaje model)
        {
            ViewBag.Cometido = _repository.Get<Cometido>(c => c.ProcesoId.Value == model.ProcesoId.Value).FirstOrDefault();
            
            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.PasajeUpdate(model);

                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("EditAbast", "Pasaje", new { model.WorkflowId, id = model.PasajeId });
                }

                TempData["Error"] = _UseCaseResponseMessage.Errors;
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
            }
            ViewBag.IdComunaOrigen = new SelectList(_sigper.GetDGCOMUNAs(), "Pl_CodCom", "Pl_DesCom");
            ViewBag.IdRegionOrigen = new SelectList(_sigper.GetRegion(), "Pl_CodReg", "Pl_DesReg");
            ViewBag.IdPaisOrigen = new SelectList(_repository.Get<Pais>(), "PaisId", "PaisNombre");
            ViewBag.IdCiudadOrigen = new SelectList(_repository.Get<Ciudad>(), "CiudadId", "CiudadNombre");

            return View(model);
        }

        public ActionResult EditSeleccion(int id)
        {
            var model = _repository.GetById<Pasaje>(id);
            ViewBag.Cometido = _repository.Get<Cometido>(c => c.ProcesoId.Value == model.ProcesoId.Value).FirstOrDefault();
            var pasaje = _repository.Get<Pasaje>(p => p.ProcesoId.Value == model.ProcesoId.Value).FirstOrDefault();
            ViewBag.Pasaje = pasaje;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditSeleccion(List<CotizacionDocumento> CotizacionDocumento, int PasajeId)
        //public ActionResult EditSeleccion(Pasaje model)
        {
            var model = _repository.Get<Pasaje>(c => c.PasajeId == PasajeId).FirstOrDefault();
            var Cotizacion = _repository.Get<Cotizacion>(c => c.PasajeId == PasajeId).ToList();
            ViewBag.Cometido = _repository.Get<Cometido>(c => c.ProcesoId.Value == model.ProcesoId.Value).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository);
                var _UseCaseResponseMessage = _useCaseInteractor.CotizacionDocumentoUpdate(CotizacionDocumento);
                //ResponseMessage _UseCaseResponseMessage = new ResponseMessage();
                //foreach (var cot in model.Cotizacion)
                //{
                //    _UseCaseResponseMessage = _useCaseInteractor.CotizacionUpdateSeleccion(cot);
                //}                
                
                if (_UseCaseResponseMessage.Warnings.Count > 0)
                    TempData["Warning"] = _UseCaseResponseMessage.Warnings;

                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("EditSeleccion", "Pasaje", new { id = model.PasajeId });
                }
                else
                {
                    TempData["Error"] = _UseCaseResponseMessage.Errors;
                    return RedirectToAction("EditSeleccion", "Pasaje", new { id = model.PasajeId });
                }
                //foreach (var item in _UseCaseResponseMessage.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, item);                    
                //}                
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .ToList();
            }

            return View(model);
        }
    }
}