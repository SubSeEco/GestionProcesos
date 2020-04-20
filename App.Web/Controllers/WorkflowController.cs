using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using App.Model.Core;
using App.Model.Pasajes;
using App.Core.Interfaces;
using App.Infrastructure.Extensions;
using App.Model.Cometido;
using App.Model.Comisiones;
using App.Model.FirmaDocumento;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class WorkflowController : Controller
    {
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
                Workflows = new List<Workflow>();
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
            public List<Workflow> Workflows { get; set; }
        }
        protected readonly IGestionProcesos _repository;
        protected readonly IEmail _email;
        protected readonly ISIGPER _sigper;

        public WorkflowController(IGestionProcesos repository, IEmail email, ISIGPER sigper)
        {
            _repository = repository;
            _email = email;
            _sigper = sigper;
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
               .Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq })
               .OrderBy(q => q.Nombre)
               .ToList().Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = new DTOFilter();

            var email = UserExtended.Email(User);
            var user = _sigper.GetUserByEmail(email);
            var gruposEspeciales = _repository.Get<Usuario>(q => q.Habilitado && q.Email == email && !q.Grupo.Nombre.Contains(App.Util.Enum.Grupo.Administrador.ToString())).Select(q => q.GrupoId).ToList();

            if (user.Funcionario == null || user.Unidad == null)
                ModelState.AddModelError(string.Empty, "No se encontró información del funcionario en el sistema SIGPER");

            if (ModelState.IsValid)
            {
                //tareas no terminadas, personales y de mis grupos
                var predicatePersonal = PredicateBuilder.True<Workflow>();
                var predicateGrupal = PredicateBuilder.True<Workflow>();

                //tareas no terminadas
                predicatePersonal = predicatePersonal.And(q => !q.Terminada && q.TareaPersonal);
                predicateGrupal = predicateGrupal.And(q => !q.Terminada && !q.TareaPersonal);

                //filtrar texto
                if (!string.IsNullOrWhiteSpace(model.TextSearch))
                {
                    predicatePersonal = predicatePersonal.And(q => q.Observacion.Contains(model.TextSearch));
                    predicateGrupal = predicateGrupal.And(q => q.Observacion.Contains(model.TextSearch));
                }
                if (model.Desde.HasValue)
                {
                    predicatePersonal = predicatePersonal.And(q => q.FechaCreacion.Year >= model.Desde.Value.Year && q.FechaCreacion.Month >= model.Desde.Value.Month && q.FechaCreacion.Day >= model.Desde.Value.Day);
                    predicateGrupal = predicateGrupal.And(q => q.FechaCreacion.Year >= model.Desde.Value.Year && q.FechaCreacion.Month >= model.Desde.Value.Month && q.FechaCreacion.Day >= model.Desde.Value.Day);
                }
                if (model.Hasta.HasValue)
                {
                    predicatePersonal = predicatePersonal.And(q => q.FechaCreacion.Year <= model.Desde.Value.Year && q.FechaCreacion.Month <= model.Desde.Value.Month && q.FechaCreacion.Day <= model.Desde.Value.Day);
                    predicateGrupal = predicateGrupal.And(q => q.FechaCreacion.Year <= model.Desde.Value.Year && q.FechaCreacion.Month <= model.Desde.Value.Month && q.FechaCreacion.Day <= model.Desde.Value.Day);
                }

                //no es administrador, filtrar por grupo y tareas personales
                if (!_repository.GetExists<Usuario>(q => q.Habilitado && q.Email == email && q.Grupo.Nombre.Contains(App.Util.Enum.Grupo.Administrador.ToString())))
                {
                    predicatePersonal = predicatePersonal.And(q => q.TareaPersonal && q.Email == email);
                    predicateGrupal = predicateGrupal.And(q => !q.TareaPersonal && (q.Pl_UndCod == user.Unidad.Pl_UndCod || gruposEspeciales.Contains(q.GrupoId.Value)));
                }

                model.Workflows = _repository.Get(predicatePersonal).Union(_repository.Get(predicateGrupal)).ToList();
            }
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DTOFilter model)
        {
            var email = UserExtended.Email(User);
            var user = _sigper.GetUserByEmail(email);
            var gruposEspeciales = _repository.Get<Usuario>(q => q.Email == email).Select(q => q.GrupoId).ToList();

            var predicate = PredicateBuilder.True<Workflow>();
            //tareas no terminadas
            predicate = predicate.And(q => !q.Terminada);

            if (!_repository.GetExists<Usuario>(q => q.Email == email && q.Grupo.Nombre.Contains(App.Util.Enum.Grupo.Administrador.ToString())))
                predicate = predicate.And(q => q.Email == email || q.Pl_UndCod == user.Unidad.Pl_UndCod || gruposEspeciales.Contains(q.GrupoId.Value));

            //filtrar texto y fechas
            if (!string.IsNullOrWhiteSpace(model.TextSearch))
                predicate = predicate.And(q => q.Observacion.Contains(model.TextSearch));
            if (model.Desde.HasValue)
                predicate = predicate.And(q => q.FechaCreacion.Year >= model.Desde.Value.Year && q.FechaCreacion.Month >= model.Desde.Value.Month && q.FechaCreacion.Day >= model.Desde.Value.Day);
            if (model.Hasta.HasValue)
                predicate = predicate.And(q => q.FechaCreacion.Year <= model.Desde.Value.Year && q.FechaCreacion.Month <= model.Desde.Value.Month && q.FechaCreacion.Day <= model.Desde.Value.Day);

            model.Workflows = _repository.Get(predicate).ToList();

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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Sign(Workflow model)

        //{
        //    var email = UserExtended.Email(User);

        //    if (ModelState.IsValid)
        //    {
        //        var _useCaseInteractor = new UseCaseInteractorCustom(_repository);
        //        var _UseCaseResponseMessage = _useCaseInteractor.DocumentoSign(model, email);
        //        if (_UseCaseResponseMessage.IsValid)
        //        {
        //            TempData["Success"] = "Operación terminada correctamente.";
        //            return Redirect(Request.UrlReferrer.PathAndQuery);
        //        }

        //        TempData["Error"] = _UseCaseResponseMessage.Errors;
        //    }

        //    return View(model);
        //}

        public ActionResult Execute(int id)
        {
            var workflow = _repository.GetById<Workflow>(id);
            if (workflow == null)
                ModelState.AddModelError(string.Empty, "No se encontró información de la tarea a ejecutar");
            if (workflow != null && workflow.Terminada)
                ModelState.AddModelError(string.Empty, "La tarea no se puede ejecutar ya que se encuentra terminada");
            if (workflow != null && workflow.Anulada)
                ModelState.AddModelError(string.Empty, "La tarea no se puede ejecutar ya que se encuentra anulada");

            if (workflow != null && workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == App.Util.Enum.Entidad.Cometido.ToString())
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

            if (workflow != null && workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == App.Util.Enum.Entidad.Pasaje.ToString())
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

            if (workflow != null && workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == App.Util.Enum.Entidad.Comision.ToString())
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
              
            if (workflow != null && workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == App.Util.Enum.Entidad.FirmaDocumento.ToString())
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
                    return RedirectToAction(workflow.DefinicionWorkflow.Accion.Codigo, workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo, new { workflow.WorkflowId });
                else
                    return RedirectToAction(workflow.DefinicionWorkflow.Accion.Codigo, workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo, new { id = workflow.EntityId });

            return View(workflow);
        }

        public ActionResult Send(int id)
        {
            var model = _repository.GetById<Workflow>(id);

            ViewBag.TipoAprobacionId = new SelectList(_repository.Get<TipoAprobacion>(q => q.TipoAprobacionId > 1).OrderBy(q => q.Nombre), "TipoAprobacionId", "Nombre");
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>(), "GrupoId", "Nombre");
            ViewBag.To = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.Pl_UndCod.HasValue)
                ViewBag.To = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.Email);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send(Workflow model, string action)
        {
            model.Email = UserExtended.Email(User);

            if (ModelState.IsValid)
            {
                var workflow = _repository.GetById<Workflow>(model.WorkflowId);

                if (workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo.ToString() == App.Util.Enum.Entidad.Cometido.ToString()
                  || workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo.ToString() == App.Util.Enum.Entidad.Comision.ToString()
                  || workflow.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo.ToString() == App.Util.Enum.Entidad.CometidoPasaje.ToString())
                {
                    var _useCaseInteractor = new App.Core.UseCases.UseCaseCometidoComision(_repository, _email, _sigper);
                    var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model);
                    if (_UseCaseResponseMessage.IsValid)
                    {
                        TempData["Success"] = "Operación terminada correctamente.";
                        return RedirectToAction("Index", "Workflow");
                    }
                    else
                        TempData["Error"] = _UseCaseResponseMessage.Errors;
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
                    else
                        TempData["Error"] = _UseCaseResponseMessage.Errors;
                }
            }

            ViewBag.TipoAprobacionId = new SelectList(_repository.Get<TipoAprobacion>(q => q.TipoAprobacionId > 1).OrderBy(q => q.Nombre), "TipoAprobacionId", "Nombre", model.TipoAprobacionId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>(), "GrupoId", "Nombre", model.GrupoId);
            ViewBag.To = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.Pl_UndCod.HasValue)
                ViewBag.To = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.Email);

            return View(model);
        }

        public ActionResult Validate(int id)
        {
            var model = _repository.GetById<Workflow>(id);
            ViewBag.DefinicionWorkflowId = new SelectList(_repository.Get<DefinicionWorkflow>(q => q.DefinicionWorkflowDependeDeId == model.DefinicionWorkflowId).OrderBy(q => q.Secuencia).AsEnumerable().Select(q => new { Value = q.DefinicionWorkflowId.ToString(), Text = q.Nombre }), "Value", "Text");
            ViewBag.TipoAprobacionId = new SelectList(_repository.Get<TipoAprobacion>(q => q.TipoAprobacionId > 1).OrderBy(q => q.Nombre), "TipoAprobacionId", "Nombre");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Validate(Workflow model)
        {
            var workflow = _repository.GetById<Workflow>(model.WorkflowId);

            model.Email = UserExtended.Email(User);

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository, _email, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                    TempData["Success"] = "Operación terminada correctamente.";
                else
                    TempData["Error"] = _UseCaseResponseMessage.Errors;

                return RedirectToAction("Index", "Workflow");
            }

            ViewBag.DefinicionWorkflowId = new SelectList(_repository.Get<DefinicionWorkflow>(q => q.DefinicionWorkflowDependeDeId == model.DefinicionWorkflowId).OrderBy(q => q.Secuencia).AsEnumerable().Select(q => new { Value = q.DefinicionWorkflowId.ToString(), Text = q.Nombre }), "Value", "Text", model.DefinicionWorkflowId);
            ViewBag.TipoAprobacionId = workflow.DefinicionWorkflow.RequiereAprobacionAlEnviar ?
                new SelectList(_repository.Get<TipoAprobacion>(q => q.TipoAprobacionId > 1).OrderBy(q => q.Nombre), "TipoAprobacionId", "Nombre") :
                new SelectList(new List<TipoAprobacion>(), "TipoAprobacionId", "Nombre");

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

            //if (ModelState.IsValid)
            //{
            //    var _useCaseInteractor = new App.Core(_repository, _email, _sigper);
            //    var _UseCaseResponseMessage = _useCaseInteractor.WorkflowForward(model);
            //    if (_UseCaseResponseMessage.IsValid)
            //    {
            //        TempData["Success"] = "Operación terminada correctamente.";
            //        return RedirectToAction("Index", "Workflow");
            //    }
            //    else
            //        TempData["Error"] = _UseCaseResponseMessage.Errors;
            //}

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

        public ActionResult Next(int id)
        {
            var model = _repository.GetById<Workflow>(id);

            ViewBag.TipoAprobacionId = new SelectList(_repository.Get<TipoAprobacion>(q => q.TipoAprobacionId > 1).OrderBy(q => q.Nombre), "TipoAprobacionId", "Nombre");
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes");
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>(), "GrupoId", "Nombre");
            ViewBag.To = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.Pl_UndCod.HasValue)
                ViewBag.To = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.Email);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Next(Workflow model)
        {
            model.Email = UserExtended.Email(User);

            if (ModelState.IsValid)
            {
                var _useCaseInteractor = new UseCaseCore(_repository, _email, _sigper);
                var _UseCaseResponseMessage = _useCaseInteractor.WorkflowUpdate(model);
                if (_UseCaseResponseMessage.IsValid)
                {
                    TempData["Success"] = "Operación terminada correctamente.";
                    return RedirectToAction("Index", "Workflow");
                }
                else
                    TempData["Error"] = _UseCaseResponseMessage.Errors;
            }

            ViewBag.TipoAprobacionId = new SelectList(_repository.Get<TipoAprobacion>(q => q.TipoAprobacionId > 1).OrderBy(q => q.Nombre), "TipoAprobacionId", "Nombre", model.TipoAprobacionId);
            ViewBag.Pl_UndCod = new SelectList(_sigper.GetUnidades(), "Pl_UndCod", "Pl_UndDes", model.Pl_UndCod);
            ViewBag.GrupoId = new SelectList(_repository.GetAll<Grupo>(), "GrupoId", "Nombre", model.GrupoId);
            ViewBag.To = new SelectList(new List<App.Model.SIGPER.PEDATPER>().Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).ToList(), "Email", "Nombre");
            if (model.Pl_UndCod.HasValue)
                ViewBag.To = new SelectList(_sigper.GetUserByUnidad(model.Pl_UndCod.Value).Select(c => new { Email = c.Rh_Mail, Nombre = c.PeDatPerChq }).OrderBy(q => q.Nombre).Distinct().ToList(), "Email", "Nombre", model.Email);

            return View(model);
        }
    }
}
