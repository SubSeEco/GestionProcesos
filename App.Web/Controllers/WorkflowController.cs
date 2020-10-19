using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using App.Model.Core;
using App.Model.Pasajes;
using App.Core.Interfaces;
using App.Util;
using App.Model.Cometido;
using App.Model.Comisiones;
using App.Model.FirmaDocumento;
using App.Core.UseCases;
using App.Model.InformeHSA;
using App.Model.Memorandum;
using App.Model.GestionDocumental;
using App.Model.ProgramacionHorasExtraordinarias;
using System;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class WorkflowController : Controller
    {
        protected readonly IGestionProcesos _repository;
        protected readonly IEmail _email;
        protected readonly ISIGPER _sigper;
        protected readonly IFolio _folio;
        protected readonly IFile _file;
        protected readonly IHSM _hsm;

        public class DTOWorkflow
        {
            public DTOWorkflow()
            {
            }

            public int WorkflowId { get; set; }
            public DateTime FechaCreacion { get; set; }
            public string Asunto { get; set; }
            public string Definicion { get; set; }
            public bool TareaPersonal { get; set; }
            public string NombreFuncionario { get; set; }
            public string Pl_UndDes { get; set; }
            public string Grupo { get; set; }
            public string Mensaje { get; set; }

            public int ProcesoId { get; set; }
            public DateTime? ProcesoFechaVencimiento { get; set; }
            public string ProcesoDefinicion { get; set; }
            public string ProcesoNombreFuncionario { get; set; }
            public string ProcesoEmail { get; set; }
            public string ProcesoEntidad { get; set; }
        }

        public class DTOUser
        {
            public string id { get; set; }
            public string value { get; set; }
        }

        public class DTOFilter
        {
            public DTOFilter()
            {
                TextSearch = string.Empty;
                Select = new List<App.Model.DTO.DTOSelect>();
                TareasGrupales = new List<DTOWorkflow>();
                TareasPersonales = new List<DTOWorkflow>();
            }

            [Display(Name = "Texto de búsqueda")]
            public string TextSearch { get; set; }

            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [DataType(DataType.Date)]
            [Display(Name = "Desde")]
            public System.DateTime? Desde { get; set; }

            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            [DataType(DataType.Date)]
            [Display(Name = "Hasta")]
            public System.DateTime? Hasta { get; set; }

            public List<App.Model.DTO.DTOSelect> Select { get; set; }
            //public List<Workflow> TareasPersonales { get; set; }
            //public List<Workflow> TareasGrupales { get; set; }            
            public List<DTOWorkflow> TareasPersonales { get; set; }
            public List<DTOWorkflow> TareasGrupales { get; set; }
        }

        public WorkflowController(IGestionProcesos repository, IEmail email, ISIGPER sigper, IFolio folio, IFile file, IHSM hsm)
        {
            _repository = repository;
            _email = email;
            _sigper = sigper;
            _folio = folio;
            _file = file;
            _hsm = hsm;
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

        public ActionResult Index()
        {
            var email = UserExtended.Email(User);
            var user = _sigper.GetUserByEmail(email);
            var gruposEspeciales = _repository.Get<Usuario>(q => q.Email == email).Select(q => q.GrupoId).ToList();

            var model = new DTOFilter();

            using (var context = new App.Infrastructure.GestionProcesos.AppContext())
            {
                //usuario administrador
                if (_repository.GetExists<Usuario>(q => q.Habilitado && q.Email == email && q.Grupo.Nombre.Contains(App.Util.Enum.Grupo.Administrador.ToString())))
                {
                    model.TareasPersonales = context.Workflow.Where(q => !q.Terminada && q.TareaPersonal).Select(q => new DTOWorkflow
                    {
                        WorkflowId = q.WorkflowId,
                        FechaCreacion = q.FechaCreacion,
                        Asunto = q.Asunto ,
                        Definicion = q.DefinicionWorkflow.Nombre,
                        TareaPersonal = q.TareaPersonal,
                        NombreFuncionario = q.NombreFuncionario,
                        Pl_UndDes = q.Pl_UndDes,
                        Grupo = q.Grupo != null ? q.Grupo.Nombre : string.Empty,
                        Mensaje = q.Mensaje,
                        ProcesoId = q.ProcesoId,
                        ProcesoFechaVencimiento = q.Proceso.FechaVencimiento,
                        ProcesoDefinicion = q.Proceso.DefinicionProceso.Nombre,
                        ProcesoNombreFuncionario = q.Proceso.NombreFuncionario,
                        ProcesoEmail = q.Proceso.Email,
                        ProcesoEntidad = q.Proceso.DefinicionProceso.Entidad.Codigo
                    }).ToList();
                    model.TareasGrupales = context.Workflow.Where(q => !q.Terminada && !q.TareaPersonal).Select(q => new DTOWorkflow
                    {
                        WorkflowId = q.WorkflowId,
                        FechaCreacion = q.FechaCreacion,
                        Asunto = q.Asunto,
                        Definicion = q.DefinicionWorkflow.Nombre,
                        TareaPersonal = q.TareaPersonal,
                        NombreFuncionario = q.NombreFuncionario,
                        Pl_UndDes = q.Pl_UndDes,
                        Grupo = q.Grupo != null ? q.Grupo.Nombre : string.Empty,
                        Mensaje = q.Mensaje,
                        ProcesoId = q.ProcesoId,
                        ProcesoFechaVencimiento = q.Proceso.FechaVencimiento,
                        ProcesoDefinicion = q.Proceso.DefinicionProceso.Nombre,
                        ProcesoNombreFuncionario = q.Proceso.NombreFuncionario,
                        ProcesoEmail = q.Proceso.Email,
                        ProcesoEntidad = q.Proceso.DefinicionProceso.Entidad.Codigo
                    }).ToList();
                }

                //usuario normal
                else
                {
                    model.TareasPersonales = context.Workflow.Where(q => !q.Terminada && q.Email == email).Select(q => new DTOWorkflow
                    {
                        WorkflowId = q.WorkflowId,
                        FechaCreacion = q.FechaCreacion,
                        Asunto = q.Asunto,
                        Definicion = q.DefinicionWorkflow.Nombre,
                        TareaPersonal = q.TareaPersonal,
                        NombreFuncionario = q.NombreFuncionario,
                        Pl_UndDes = q.Pl_UndDes,
                        Grupo = q.Grupo != null ? q.Grupo.Nombre : string.Empty,
                        Mensaje = q.Mensaje,
                        ProcesoId = q.ProcesoId,
                        ProcesoFechaVencimiento = q.Proceso.FechaVencimiento,
                        ProcesoDefinicion = q.Proceso.DefinicionProceso.Nombre,
                        ProcesoNombreFuncionario = q.Proceso.NombreFuncionario,
                        ProcesoEmail = q.Proceso.Email,
                        ProcesoEntidad = q.Proceso.DefinicionProceso.Entidad.Codigo
                    }).ToList();
                    model.TareasGrupales = context.Workflow.Where(q => !q.Terminada && !q.TareaPersonal && q.Pl_UndCod == user.Unidad.Pl_UndCod).Select(q => new DTOWorkflow
                    {
                        WorkflowId = q.WorkflowId,
                        FechaCreacion = q.FechaCreacion,
                        Asunto = q.Asunto,
                        Definicion = q.DefinicionWorkflow.Nombre,
                        TareaPersonal = q.TareaPersonal,
                        NombreFuncionario = q.NombreFuncionario,
                        Pl_UndDes = q.Pl_UndDes,
                        Grupo = q.Grupo != null ? q.Grupo.Nombre : string.Empty,
                        Mensaje = q.Mensaje,
                        ProcesoId = q.ProcesoId,
                        ProcesoFechaVencimiento = q.Proceso.FechaVencimiento,
                        ProcesoDefinicion = q.Proceso.DefinicionProceso.Nombre,
                        ProcesoNombreFuncionario = q.Proceso.NombreFuncionario,
                        ProcesoEmail = q.Proceso.Email,
                        ProcesoEntidad = q.Proceso.DefinicionProceso.Entidad.Codigo
                    }).ToList();
                    model.TareasGrupales.AddRange(context.Workflow.Where(q => !q.Terminada && !q.TareaPersonal && gruposEspeciales.Contains(q.GrupoId.Value)).Select(q => new DTOWorkflow
                    {
                        WorkflowId = q.WorkflowId,
                        FechaCreacion = q.FechaCreacion,
                        Asunto = q.Asunto,
                        Definicion = q.DefinicionWorkflow.Nombre,
                        TareaPersonal = q.TareaPersonal,
                        NombreFuncionario = q.NombreFuncionario,
                        Pl_UndDes = q.Pl_UndDes,
                        Grupo = q.Grupo != null ? q.Grupo.Nombre : string.Empty,
                        Mensaje = q.Mensaje,
                        ProcesoId = q.ProcesoId,
                        ProcesoFechaVencimiento = q.Proceso.FechaVencimiento,
                        ProcesoDefinicion = q.Proceso.DefinicionProceso.Nombre,
                        ProcesoNombreFuncionario = q.Proceso.NombreFuncionario,
                        ProcesoEmail = q.Proceso.Email,
                        ProcesoEntidad = q.Proceso.DefinicionProceso.Entidad.Codigo
                    }).ToList());
                }
            }

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

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.Cometido.ToString())
            {
                var obj = _repository.GetFirst<Cometido>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.CometidoId;
                    workflow.Entity = App.Util.Enum.Entidad.Cometido.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.Pasaje.ToString())
            {
                var obj = _repository.GetFirst<Pasaje>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.PasajeId;
                    workflow.Entity = App.Util.Enum.Entidad.Pasaje.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.Comision.ToString())
            {
                var obj = _repository.GetFirst<Comisiones>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.ComisionesId;
                    workflow.Entity = App.Util.Enum.Entidad.Comision.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.FirmaDocumento.ToString())
            {
                var obj = _repository.GetFirst<FirmaDocumento>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.FirmaDocumentoId;
                    workflow.Entity = App.Util.Enum.Entidad.FirmaDocumento.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.InformeHSA.ToString())
            {
                var obj = _repository.GetFirst<InformeHSA>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.InformeHSAId;
                    workflow.Entity = App.Util.Enum.Entidad.InformeHSA.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.Memorandum.ToString())
            {
                var obj = _repository.GetFirst<Memorandum>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.MemorandumId;
                    workflow.Entity = App.Util.Enum.Entidad.Memorandum.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }


            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.GDInterno.ToString())
            {
                var obj = _repository.GetFirst<GD>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.GDId;
                    workflow.Entity = App.Util.Enum.Entidad.GDInterno.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }
            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.GDExterno.ToString())
            {
                var obj = _repository.GetFirst<GD>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.GDId;
                    workflow.Entity = App.Util.Enum.Entidad.GDExterno.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            if (workflow != null && workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.ProgramacionHorasExtraordinarias.ToString())
            {
                var obj = _repository.GetFirst<ProgramacionHorasExtraordinarias>(q => q.ProcesoId == workflow.ProcesoId);
                if (obj != null)
                {
                    workflow.EntityId = obj.ProgramacionHorasExtraordinariasId;
                    workflow.Entity = App.Util.Enum.Entidad.ProgramacionHorasExtraordinarias.ToString();
                    obj.WorkflowId = workflow.WorkflowId;

                    _repository.Update(obj);
                    _repository.Save();
                }
            }

            if (workflow != null && workflow.DefinicionWorkflow.Accion.Codigo == "Create" && workflow.EntityId.HasValue)
                workflow.DefinicionWorkflow.Accion.Codigo = "Edit";
            if (workflow != null && workflow.DefinicionWorkflow.Accion.Codigo == "Edit" && !workflow.EntityId.HasValue)
                workflow.DefinicionWorkflow.Accion.Codigo = "Edit";
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
            var email = UserExtended.Email(User);

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
            model.Mensaje = string.Empty;

            if (!string.IsNullOrWhiteSpace(model.To))
            {
                var persona = _sigper.GetUserByEmail(model.To.Trim());
                if (persona != null)
                {
                    ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades().OrderBy(q => q.Pl_UndDes).Select(q => new SelectListItem { Value = q.Pl_UndCod.ToString(), Text = q.Pl_UndDes.Trim() }), "Value", "Text", persona.Unidad.Pl_UndCod);
                    ViewBag.To = new SelectList(
                        _sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod)
                        .Where(q => !q.Rh_Mail.Trim().Equals(email.Trim())) // excluir ejecutor de tarea
                        .OrderBy(q => q.PeDatPerChq)
                        .Select(q => new SelectListItem { Value = q.Rh_Mail.Trim(), Text = q.PeDatPerChq.Trim() }),
                        "Value", "Text", persona.Funcionario.Rh_Mail.Trim());

                    model.Pl_UndCod = persona.Unidad.Pl_UndCod;
                    model.To = persona.Funcionario.Rh_Mail.Trim();
                }
            }
            else
            {
                ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
                ViewBag.To = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");

                if (model.Pl_UndCod.HasValue)
                    ViewBag.To = new SelectList(
                        _sigper.GetUserByUnidad(model.Pl_UndCod.Value)
                        .Where(q => !q.Rh_Mail.Trim().Equals(email.Trim())) // excluir ejecutor de tarea
                        .Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() })
                        .OrderBy(q => q.Nombre).Distinct().ToList(),
                        "Email", "Nombre", model.Email);

                //fix para solucionar problemas de fvidal
                if (model.Pl_UndCod.HasValue && model.EsFirmaDocumento && model.Pl_UndCod.Value == 200310)
                {
                    var funcionarios = _sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail.Trim(), Nombre = c.PeDatPerChq.Trim() }).ToList();
                    funcionarios.Add(new { Email = "fvial@economia.cl", Nombre = "FELIPE VIAL TAGLE" });
                    ViewBag.To = new SelectList(funcionarios.OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.Email);
                }

            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send(Workflow model)
        {
            model.Email = UserExtended.Email(User);

            if (ModelState.IsValid)
            {
                var workflow = _repository.GetById<Workflow>(model.WorkflowId);

                if (workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje || workflow.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudPasaje)
                {
                    var _useCaseInteractor = new App.Core.UseCases.UseCaseCometidoComision(_repository, _email, _sigper, _file);
                    var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model, User.Email());
                    if (_UseCaseResponseMessage.IsValid)
                    {
                        TempData["Success"] = "Operación terminada correctamente.";
                        return RedirectToAction("Index", "Workflow");
                    }

                    _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
                }
                else if (workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == App.Util.Enum.Entidad.FirmaDocumento.ToString())
                {
                    var _useCaseInteractor = new App.Core.UseCases.UseCaseFirmaDocumento(_repository, _sigper, _file, _folio, _hsm, _email);
                    var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model);
                    if (_UseCaseResponseMessage.IsValid)
                    {
                        TempData["Success"] = "Operación terminada correctamente.";
                        return RedirectToAction("Index", "Workflow");
                    }

                    _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
                }
                else if (workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == App.Util.Enum.Entidad.Memorandum.ToString())
                {
                    var _useCaseInteractor = new App.Core.UseCases.UseCaseMemorandum(_repository, _sigper, _file, _folio, _hsm, _email);
                    var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model);
                    if (_UseCaseResponseMessage.IsValid)
                    {
                        TempData["Success"] = "Operación terminada correctamente.";
                        return RedirectToAction("OK");
                    }
                    _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
                }
                else if (workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == App.Util.Enum.Entidad.ProgramacionHorasExtraordinarias.ToString())
                {
                    var _useCaseInteractor = new App.Core.UseCases.UseCaseProgramacionHorasExtraordinarias(_repository, _sigper, _file, _folio, _hsm, _email);
                    var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model);
                    if (_UseCaseResponseMessage.IsValid)
                    {
                        TempData["Success"] = "Operación terminada correctamente.";
                        return RedirectToAction("OK");
                    }
                    _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
                }
                else if (workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == App.Util.Enum.Entidad.GDInterno.ToString())
                {
                    var _useCaseInteractor = new App.Core.UseCases.UseCaseGD(_repository, _file, _folio, _sigper, _email);
                    var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdateInterno(model);
                    if (_UseCaseResponseMessage.IsValid)
                    {
                        TempData["Success"] = "Operación terminada correctamente.";
                        return RedirectToAction("Index", "Workflow");
                    }
                    _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
                }
                else if (workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == App.Util.Enum.Entidad.GDExterno.ToString())
                {
                    var _useCaseInteractor = new App.Core.UseCases.UseCaseGD(_repository, _file, _folio, _sigper, _email);
                    var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdateExterno(model);
                    if (_UseCaseResponseMessage.IsValid)
                    {
                        TempData["Success"] = "Operación terminada correctamente.";
                        return RedirectToAction("Index", "Workflow");
                    }
                    _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
                }
                else
                {
                    var _useCaseInteractor = new App.Core.UseCases.UseCaseCore(_repository, _email, _sigper);
                    var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model);
                    if (_UseCaseResponseMessage.IsValid)
                    {
                        TempData["Success"] = "Operación terminada correctamente.";
                        return RedirectToAction("Index", "Workflow");
                    }

                    _UseCaseResponseMessage.Errors.ForEach(q => ModelState.AddModelError(string.Empty, q));
                }
            }

            ViewBag.TipoAprobacionId = new SelectList(_repository.Get<TipoAprobacion>(q => q.TipoAprobacionId > 1).OrderBy(q => q.Nombre), "TipoAprobacionId", "Nombre", model.TipoAprobacionId);
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>(), "GrupoId", "Nombre", model.GrupoId);

            if (!model.To.IsNullOrWhiteSpace())
            {
                var persona = _sigper.GetUserByEmail(model.To.Trim());
                if (persona != null)
                {
                    ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades().OrderBy(q => q.Pl_UndDes).Select(q => new SelectListItem { Value = q.Pl_UndCod.ToString(), Text = q.Pl_UndDes.Trim() }), "Value", "Text", persona.Unidad.Pl_UndCod);
                    ViewBag.To = new SelectList(_sigper.GetUserByUnidad(persona.Unidad.Pl_UndCod).OrderBy(q => q.PeDatPerChq).Select(q => new SelectListItem { Value = q.Rh_Mail.Trim(), Text = q.PeDatPerChq.Trim() }), "Value", "Text", persona.Funcionario.Rh_Mail.Trim());

                    model.Pl_UndCod = persona.Unidad.Pl_UndCod;
                    model.To = persona.Funcionario.Rh_Mail.Trim();
                }
            }
            else
            {
                ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
                ViewBag.To = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
                if (model.Pl_UndCod.HasValue)
                    ViewBag.To = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.Email);
            }

            return View(model);
        }

        public ActionResult Forward(int id)
        {
            var model = _repository.GetById<Workflow>(id);

            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>(), "GrupoId", "Nombre", model.GrupoId);
            ViewBag.To = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
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
            ViewBag.To = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
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
            ViewBag.To = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.Pl_UndCod.HasValue)
                ViewBag.To = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.Email);

            return View(model);
        }
    }
}