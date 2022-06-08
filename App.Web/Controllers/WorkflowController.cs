using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using App.Model.Core;
using App.Model.Pasajes;
using App.Core.Interfaces;
using App.Model.Cometido;
using App.Model.Comisiones;
using App.Model.FirmaDocumento;
using App.Core.UseCases;
using App.Model.InformeHSA;
using App.Model.GestionDocumental;
using App.Model.ProgramacionHorasExtraordinarias;
using System;
using App.Model.HorasExtras;
using App.Model.DTO;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    [NoDirectAccess]

    public class WorkflowController : Controller
    {
        private readonly IGestionProcesos _repository;
        private readonly IEmail _email;
        private readonly ISigper _sigper;
        private readonly IFolio _folio;
        private readonly IFile _file;
        private readonly IHsm _hsm;
        private readonly IWorkflowService _workflowService;

        private class DTOUser
        {
            public string id { get; set; }
            public string value { get; set; }
        }

        public class DTOFilter
        {
            public DTOFilter()
            {
                TextSearch = string.Empty;
                Select = new List<DTOSelect>();
                TareasGrupales = new List<WorkflowDTO>();
                TareasPersonales = new List<WorkflowDTO>();
            }

            [Display(Name = "Texto de búsqueda")]
            public string TextSearch { get; set; }

            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [DataType(DataType.Date)]
            [Display(Name = "Desde")]
            public DateTime? Desde { get; set; }

            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [DataType(DataType.Date)]
            [Display(Name = "Hasta")]
            public DateTime? Hasta { get; set; }

            public List<DTOSelect> Select { get; set; }
            public List<WorkflowDTO> TareasPersonales { get; set; }
            public List<WorkflowDTO> TareasGrupales { get; set; }
        }

        public WorkflowController(IGestionProcesos repository, IEmail email, ISigper sigper, IFolio folio, IFile file, IHsm hsm, IWorkflowService workflowService)
        {
            _repository = repository;
            _email = email;
            _sigper = sigper;
            _folio = folio;
            _file = file;
            _hsm = hsm;
            _workflowService = workflowService;
        }

        public JsonResult GetUser(string term)
        {
            var result = _sigper.GetUserByTerm(term)
               .Take(25)
               .Select(c => new DTOUser { id = c.Rh_Mail, value = string.Format("{0} ({1})", c.PeDatPerChq, c.Rh_Mail) })
               .OrderBy(q => q.value)
               .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserByUnidad(int Pl_UndCod)
        {
            var result = _sigper.GetUserByUnidad(Pl_UndCod)
               .Select(c => new
               {
                   Email = !string.IsNullOrWhiteSpace(c.Rh_Mail) ? c.Rh_Mail.Trim() : string.Empty,
                   Nombre = !string.IsNullOrWhiteSpace(c.PeDatPerChq) ? c.PeDatPerChq.Trim() : string.Empty
               })
               .OrderBy(q => q.Nombre)
               .ToList().Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserFirmanteByUnidad(int Pl_UndCod)
        {
            var result = _sigper.GetUserFirmanteByUnidad(Pl_UndCod, _repository.Get<Rubrica>(q => q.HabilitadoFirma).Select(q => q.Email.Trim()).ToList())
               .Select(c => new
               {
                   Email = !string.IsNullOrWhiteSpace(c.Rh_Mail) ? c.Rh_Mail.Trim() : string.Empty,
                   Nombre = !string.IsNullOrWhiteSpace(c.PeDatPerChq) ? c.PeDatPerChq.Trim() : string.Empty
               })
               .OrderBy(q => q.Nombre)
               .ToList().Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteDocumento(int DocumentoId)
        {
            var result = _repository.GetById<Documento>(DocumentoId);

            result.Activo = false;
            _repository.Update(result);
            _repository.Save();

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult Index()
        {
            var email = UserExtended.Email(User);
            var user = _sigper.GetUserByEmail(email);
            var result = _workflowService.GetPendingTask(user);

            var model = new DTOFilter();
            model.TareasPersonales = result.Where(q => q.TareaPersonal).ToList();
            model.TareasGrupales = result.Where(q => !q.TareaPersonal).ToList();

            return View(model);
        }

        public ActionResult History(int workflowId)
        {
            var model = _repository.GetById<Workflow>(workflowId);
            return View(model);
        }

        public ActionResult Header(int workflowId)
        {
            var model = _repository.GetById<Workflow>(workflowId);
            return View(model);
        }

        public ActionResult Documents(int workflowId)
        {
            var model = _repository.GetById<Workflow>(workflowId);
            return View(model);
        }

        public ActionResult Sign(int workflowId)
        {
            var model = _repository.GetById<Workflow>(workflowId);
            return View(model);
        }

        public ActionResult Execute(int id)
        {
            var workflow = _repository.GetById<Workflow>(id);
            if (workflow == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la tarea a ejecutar");
            if (workflow != null && workflow.Terminada)
                ModelState.AddModelError(string.Empty, "La tarea no se puede ejecutar ya que se encuentra terminada");
            if (workflow != null && workflow.Anulada)
                ModelState.AddModelError(string.Empty, "La tarea no se puede ejecutar ya que se encuentra anulada");

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == Util.Enum.Entidad.Cometido.ToString())
            {
                var obj = _repository.GetFirst<Cometido>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.CometidoId;
                    workflow.Entity = Util.Enum.Entidad.Cometido.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == Util.Enum.Entidad.Pasaje.ToString())
            {
                var obj = _repository.GetFirst<Pasaje>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.PasajeId;
                    workflow.Entity = Util.Enum.Entidad.Pasaje.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == Util.Enum.Entidad.Comision.ToString())
            {
                var obj = _repository.GetFirst<Comisiones>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.ComisionesId;
                    workflow.Entity = Util.Enum.Entidad.Comision.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == Util.Enum.Entidad.FirmaDocumento.ToString())
            {
                var obj = _repository.GetFirst<FirmaDocumento>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.FirmaDocumentoId;
                    workflow.Entity = Util.Enum.Entidad.FirmaDocumento.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == Util.Enum.Entidad.InformeHSA.ToString())
            {
                var obj = _repository.GetFirst<InformeHSA>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.InformeHSAId;
                    workflow.Entity = Util.Enum.Entidad.InformeHSA.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            //if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == Util.Enum.Entidad.Memorandum.ToString())
            //{
            //    var obj = _repository.GetFirst<Memorandum>(q => q.ProcesoId == workflow.ProcesoId);
            //    if (obj != null)
            //    {
            //        workflow.EntityId = obj.MemorandumId;
            //        workflow.Entity = Util.Enum.Entidad.Memorandum.ToString();
            //        obj.WorkflowId = workflow.WorkflowId;

            //        _repository.Update(obj);
            //        _repository.Save();
            //    }
            //}


            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == Util.Enum.Entidad.GDInterno.ToString())
            {
                var obj = _repository.GetFirst<GD>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.GDId;
                    workflow.Entity = Util.Enum.Entidad.GDInterno.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }
            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == Util.Enum.Entidad.GDExterno.ToString())
            {
                var obj = _repository.GetFirst<GD>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.GDId;
                    workflow.Entity = Util.Enum.Entidad.GDExterno.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == Util.Enum.Entidad.ProgramacionHorasExtraordinarias.ToString())
            {
                var obj = _repository.GetFirst<ProgramacionHorasExtraordinarias>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.ProgramacionHorasExtraordinariasId;
                    workflow.Entity = Util.Enum.Entidad.ProgramacionHorasExtraordinarias.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == Util.Enum.Entidad.HorasExtras.ToString())
            {
                var obj = _repository.GetFirst<HorasExtras>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.HorasExtrasId;
                    workflow.Entity = Util.Enum.Entidad.HorasExtras.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == Util.Enum.Entidad.GeneraResolucion.ToString())
            {
                var obj = _repository.GetFirst<GeneracionResolucion>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.GeneracionResolucionId;
                    workflow.Entity = Util.Enum.Entidad.GeneraResolucion.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            //if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == Util.Enum.Entidad.FirmaDocumentoGenerico.ToString())
            //{
            //    var obj = _repository.GetFirst<FirmaDocumentoGenerico>(q => q.ProcesoId == workflow.ProcesoId);
            //    if (obj != null)
            //    {
            //        workflow.EntityId = obj.FirmaDocumentoGenericoId;
            //        workflow.Entity = Util.Enum.Entidad.FirmaDocumentoGenerico.ToString();
            //        obj.WorkflowId = workflow.WorkflowId;

            //        _repository.Update(obj);
            //        _repository.Save();
            //    }
            //}


            if (workflow != null && workflow.DefinicionWorkflow.Accion.Codigo == "Create" && workflow.EntityId.HasValue)
                workflow.DefinicionWorkflow.Accion.Codigo = "Edit";
            if (workflow != null && workflow.DefinicionWorkflow.Accion.Codigo == "Edit" && !workflow.EntityId.HasValue)
                workflow.DefinicionWorkflow.Accion.Codigo = "Edit";
            if (workflow != null && workflow.DefinicionWorkflow.Accion.Codigo == "GeneraResolucion" && !workflow.EntityId.HasValue)
                workflow.DefinicionWorkflow.Accion.Codigo = "GeneraResolucion";
            if (workflow != null && workflow.DefinicionWorkflow.Accion.Codigo == "Delete" && !workflow.EntityId.HasValue)
                ModelState.AddModelError(string.Empty, "No se encontró información asociada al proceso para eliminar");
            if (workflow != null && workflow.DefinicionWorkflow.Accion.Codigo == "Details" && !workflow.EntityId.HasValue)
                ModelState.AddModelError(string.Empty, "No se encontró información asociada al proceso para ver");

            if (ModelState.IsValid)
                if (workflow.DefinicionWorkflow.Accion.Codigo == "Create")
                    return RedirectToAction(workflow.DefinicionWorkflow.Accion.Codigo, workflow.DefinicionWorkflow.Entidad.Codigo, new { workflow.WorkflowId });
                else
                    return RedirectToAction(workflow.DefinicionWorkflow.Accion.Codigo, workflow.DefinicionWorkflow.Entidad.Codigo, new { id = workflow.EntityId });

            return View(workflow);
        }

        public ActionResult Send(int id)
        {
            //var email = UserExtended.Email(User);

            ViewBag.TipoAprobacionId = new SelectList(_repository.Get<TipoAprobacion>(q => q.TipoAprobacionId > 1).OrderBy(q => q.Nombre), "TipoAprobacionId", "Nombre");
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>(), "GrupoId", "Nombre");

            var model = _repository.GetById<Workflow>(id);
            model.RequiereAprobacionAlEnviar = model.DefinicionWorkflow.RequiereAprobacionAlEnviar;
            model.PermitirMultipleEvaluacion = model.DefinicionWorkflow.PermitirMultipleEvaluacion;
            model.PermitirSeleccionarUnidadDestino = model.DefinicionWorkflow.PermitirSeleccionarUnidadDestino;
            model.PermitirSeleccionarPersonasMismaUnidad = model.DefinicionWorkflow.PermitirSeleccionarPersonasMismaUnidad;
            model.PermitirSeleccionarGrupoEspecialDestino = model.DefinicionWorkflow.PermitirSeleccionarGrupoEspecialDestino;
            model.PermitirFinalizarProceso = model.DefinicionWorkflow.PermitirFinalizarProceso;
            model.PermitirTerminar = model.DefinicionWorkflow.PermitirTerminar;
            model.DesactivarDestinoEnRechazo = model.DefinicionWorkflow.DesactivarDestinoEnRechazo;
            model.Reservado = model.Proceso.Reservado;
            model.Mensaje = string.Empty;

            //si va a otra unidad => preseleccionar unidad destino
            if (model.ToPl_UndCod.HasValue)
            {
                ViewBag.Unidad = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.ToPl_UndCod);

                if (!string.IsNullOrWhiteSpace(model.To))
                    ViewBag.Funcionario = new SelectList(
                        _sigper.GetUserByUnidad(model.ToPl_UndCod.Value)
                        //.Where(q => !q.Rh_Mail.Trim().Equals(email.Trim())) // excluir ejecutor de tarea
                        .Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() })
                        .OrderBy(q => q.Nombre).Distinct().ToList(),
                        "Email", "Nombre", model.To);
                else
                    ViewBag.Funcionario = new SelectList(
                        _sigper.GetUserByUnidad(model.ToPl_UndCod.Value)
                        //.Where(q => !q.Rh_Mail.Trim().Equals(email.Trim())) // excluir ejecutor de tarea
                        .Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() })
                        .OrderBy(q => q.Nombre).Distinct().ToList(),
                        "Email", "Nombre");
            }

            //si es unidad local => preseleccionar unidad local
            if (!model.ToPl_UndCod.HasValue && model.Pl_UndCod.HasValue)
            {
                ViewBag.Unidad = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);

                if (!string.IsNullOrWhiteSpace(model.To))
                    ViewBag.Funcionario = new SelectList(
                        _sigper.GetUserByUnidad(model.Pl_UndCod.Value)
                        //.Where(q => !q.Rh_Mail.Trim().Equals(email.Trim())) // excluir ejecutor de tarea
                        .Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() })
                        .OrderBy(q => q.Nombre).Distinct().ToList(),
                        "Email", "Nombre", model.To);
                else
                    ViewBag.Funcionario = new SelectList(
                        _sigper.GetUserByUnidad(model.Pl_UndCod.Value)
                        //.Where(q => !q.Rh_Mail.Trim().Equals(email.Trim())) // excluir ejecutor de tarea
                        .Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() })
                        .OrderBy(q => q.Nombre).Distinct().ToList(),
                        "Email", "Nombre");
            }
            //fix para solucionar problemas de fvidal
            if (model.Pl_UndCod.HasValue && model.EsFirmaDocumento && model.Pl_UndCod.Value == 200310)
            {
                var funcionarios = _sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList();
                funcionarios.Add(new { Email = "fvial@economia.cl", Nombre = "FELIPE VIAL TAGLE" });
                ViewBag.Funcionario = new SelectList(funcionarios.OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.Email);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send(Workflow model, int? Unidad, string Funcionario)
        {
            model.Email = UserExtended.Email(User);

            if (Unidad.HasValue)
                model.Pl_UndCod = Unidad;
            if (!string.IsNullOrEmpty(Funcionario))
                model.To = Funcionario;

            ModelState.Clear();
            TryValidateModel(model);

            var workflow = _repository.GetById<Workflow>(model.WorkflowId);
            var proceso = _repository.GetById<Proceso>(workflow.ProcesoId);
            /*var lista = proceso.Documentos.ToList();

            var cometido = _repository.GetFirst<Cometido>(c => c.ProcesoId == proceso.ProcesoId);
            
            if(workflow.DefinicionWorkflowId == (int)Util.Enum.DefinicionWorkflow.IngresoAnalistaPresupuesto)
            {
                if(!cometido.GeneracionCDP.Any())
                {
                    var _useCaseInteractor = new UseCaseCometidoComision(_repository, _email, _sigper, _file);
                    var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model, User.Email());
                    if(_UseCaseResponseMessage.IsValid)
                    {
                        TempData["Success"] = "Operación terminada correctamente.";
                        return RedirectToAction("Index", "Workflow");
                    }
                    _UseCaseResponseMessage.Errors.ForEach(e => ModelState.AddModelError(string.Empty, e));
                    *//*throw new Exception("Falta Crear el Certificado de Refrendación.");*//*
                    
                }
            }


            if (workflow.DefinicionWorkflowId == (int)Util.Enum.DefinicionWorkflow.FirmaActoAdministrativo)
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    if(workflow.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                    {
                        if (lista[i].FileName.Contains("Orden de Pago") || lista[i].FileName.Contains("Resolucion Cometido") || lista[i].FileName.Contains("Resolucion Ministerial Exenta"))
                    {
                        if (!lista[i].Signed)
                        {
                            throw new Exception("El documento no esta firmado.");
                        }
                    }
                    }                    
                }
            }*/            

            if (workflow.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje || workflow.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudPasaje)
            {
                var _useCaseInteractor = new UseCaseCometidoComision(_repository, _email, _sigper, _file);
                var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model, User.Email());
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index", "Workflow");
                }

                _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
            }
            
            else if (workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == Util.Enum.Entidad.FirmaDocumento.ToString())
            {
                var _useCaseInteractor = new UseCaseFirmaDocumento(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index", "Workflow");
                }

                _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
            }
            
            //else if (workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == Util.Enum.Entidad.Memorandum.ToString())
            //{
            //    var _useCaseInteractor = new UseCaseMemorandum(_repository, _sigper, _file, _folio, _hsm, _email);
            //    var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model);
            //    if (_UseCaseResponseMessage.IsValid)
            //    {
            //        TempData["Success"] = "Operación terminada correctamente.";
            //        return RedirectToAction("OK");
            //    }
            //    _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
            //}
            //else if (workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == Util.Enum.Entidad.ProgramacionHorasExtraordinarias.ToString())
            //{
            //    var _useCaseInteractor = new UseCaseProgramacionHorasExtraordinarias(_repository, _sigper, _file, _folio, _hsm);
            //    var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model);
            //    if (_UseCaseResponseMessage.IsValid)
            //    {
            //        TempData["Success"] = "Operación terminada correctamente.";
            //        return RedirectToAction("OK");
            //    }
            //    _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
            //}
            
            else if (workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == Util.Enum.Entidad.GDInterno.ToString())
            {
                var _useCaseInteractor = new UseCaseGD(_repository, _file, _sigper, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdateInterno(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index", "Workflow");
                }
                _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
            }
            
            else if (workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == Util.Enum.Entidad.GDExterno.ToString())
            {
                var _useCaseInteractor = new UseCaseGD(_repository, _file, _sigper, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdateExterno(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index", "Workflow");
                }
                _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
            }

            //else if (workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == Util.Enum.Entidad.HorasExtras.ToString())
            //{
            //    var _useCaseInteractor = new UseCaseHorasExtras(_repository, _sigper, _file, _folio, _hsm, _email);
            //    var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model);
            //    if (_UseCaseResponseMessage.IsValid)
            //    {
            //        TempData["Success"] = "Operación terminada correctamente.";
            //        return RedirectToAction("OK");
            //    }
            //    _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
            //}
            
            else if (workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == Util.Enum.Entidad.GeneraResolucion.ToString())
            {
                var _useCaseInteractor = new UseCaseGeneraResolucion(_repository, _sigper, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("OK");
                }
                _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
            }
            else if (workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == Util.Enum.Entidad.FirmaDocumentoGenerico.ToString())
            {
                var _useCaseInteractor = new UseCaseFirmaDocumento(_repository, _sigper, _file, _folio, _hsm, _email);
                var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index", "Workflow");
                }

                _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
            }
            else
            {
                var _useCaseInteractor = new UseCaseCore(_repository, _email, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index", "Workflow");
                }

                _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
            }

            ViewBag.TipoAprobacionId = new SelectList(_repository.Get<TipoAprobacion>(q => q.TipoAprobacionId > 1).OrderBy(q => q.Nombre), "TipoAprobacionId", "Nombre");
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>(), "GrupoId", "Nombre");

            //si va a otra unidad => preseleccionar unidad destino
            if (model.ToPl_UndCod.HasValue)
            {
                ViewBag.Unidad = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.ToPl_UndCod);

                if (!string.IsNullOrWhiteSpace(model.To))
                    ViewBag.Funcionario = new SelectList(
                        _sigper.GetUserByUnidad(model.ToPl_UndCod.Value)
                        //.Where(q => !q.Rh_Mail.Trim().Equals(email.Trim())) // excluir ejecutor de tarea
                        .Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() })
                        .OrderBy(q => q.Nombre).Distinct().ToList(),
                        "Email", "Nombre", model.To);
                else
                    ViewBag.Funcionario = new SelectList(
                        _sigper.GetUserByUnidad(model.ToPl_UndCod.Value)
                        //.Where(q => !q.Rh_Mail.Trim().Equals(email.Trim())) // excluir ejecutor de tarea
                        .Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() })
                        .OrderBy(q => q.Nombre).Distinct().ToList(),
                        "Email", "Nombre");
            }

            //si es unidad local => preseleccionar unidad local
            if (!model.ToPl_UndCod.HasValue && model.Pl_UndCod.HasValue)
            {
                ViewBag.Unidad = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);

                if (!string.IsNullOrWhiteSpace(model.To))
                    ViewBag.Funcionario = new SelectList(
                        _sigper.GetUserByUnidad(model.Pl_UndCod.Value)
                        //.Where(q => !q.Rh_Mail.Trim().Equals(email.Trim())) // excluir ejecutor de tarea
                        .Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() })
                        .OrderBy(q => q.Nombre).Distinct().ToList(),
                        "Email", "Nombre", model.To);
                else
                    ViewBag.Funcionario = new SelectList(
                        _sigper.GetUserByUnidad(model.Pl_UndCod.Value)
                        //.Where(q => !q.Rh_Mail.Trim().Equals(email.Trim())) // excluir ejecutor de tarea
                        .Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() })
                        .OrderBy(q => q.Nombre).Distinct().ToList(),
                        "Email", "Nombre");
            }

            //fix para solucionar problemas de fvidal
            if (model.Pl_UndCod.HasValue && model.EsFirmaDocumento && model.Pl_UndCod.Value == 200310)
            {
                var funcionarios = _sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList();
                funcionarios.Add(new { Email = "fvial@economia.cl", Nombre = "FELIPE VIAL TAGLE" });
                ViewBag.Funcionario = new SelectList(funcionarios.OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.Email);
            }
            return View(model);
        }

        public ActionResult Forward(int id)
        {
            var model = _repository.GetById<Workflow>(id);

            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>(), "GrupoId", "Nombre", model.GrupoId);
            ViewBag.To = new SelectList(new List<Model.Sigper.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.Pl_UndCod.HasValue)
                ViewBag.To = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.Email);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Forward(Workflow model)
        {
            model.Email = UserExtended.Email(User);

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository, _email, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.WorkflowForward(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index", "Workflow");
                }
                else
                    TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>(), "GrupoId", "Nombre", model.GrupoId);
            ViewBag.To = new SelectList(new List<Model.Sigper.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.Pl_UndCod.HasValue)
                ViewBag.To = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.Email);

            return View(model);
        }

        public PartialViewResult Footer(int WorkflowId)
        {
            var model = _repository.GetById<Workflow>(WorkflowId);
            return PartialView(model);
        }

        public ActionResult OK()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Stop(Workflow model)
        {
            model.Email = UserExtended.Email(User);
            model.Observacion = model.Mensaje;
            model.Mensaje = string.Empty;

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository, _email, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.WorkflowArchive(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index", "Workflow");
                }
                else
                    TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>(), "GrupoId", "Nombre", model.GrupoId);
            ViewBag.To = new SelectList(new List<Model.Sigper.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.Pl_UndCod.HasValue)
                ViewBag.To = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.Email);

            return View(model);
        }
    }
}