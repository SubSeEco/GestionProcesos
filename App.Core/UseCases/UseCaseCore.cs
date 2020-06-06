
using System;
using System.Linq;
using App.Model.Core;
using App.Core.Interfaces;
using FluentDateTime;
using App.Model.SIGPER;
using App.Util;

namespace App.Core.UseCases
{
    public class UseCaseCore
    {
        protected readonly IGestionProcesos _repository;
        protected readonly IEmail _email;
        protected readonly IHSM _hsm;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IFolio _folio;

        public UseCaseCore(IGestionProcesos repositoryGestionProcesos, IHSM hsm)
        {
            _repository = repositoryGestionProcesos;
            _hsm = hsm;
        }
        public UseCaseCore(IGestionProcesos repositoryGestionProcesos)
        {
            _repository = repositoryGestionProcesos;
        }
        public UseCaseCore(IGestionProcesos repository, ISIGPER sigper)
        {
            _repository = repository;
            _sigper = sigper;
        }
        public UseCaseCore(IGestionProcesos repository, IEmail email)
        {
            _repository = repository;
            _email = email;
        }
        public UseCaseCore(IGestionProcesos repository, IEmail email, ISIGPER sigper)
        {
            _repository = repository;
            _email = email;
            _sigper = sigper;
        }

        public ResponseMessage DefinicionProcesoInsert(DefinicionProceso obj)
        {
            var response = new ResponseMessage();

            if (string.IsNullOrEmpty(obj.Nombre))
                response.Errors.Add("Debe especificar el nombre");
            if (string.IsNullOrEmpty(obj.Descripcion))
                response.Errors.Add("Debe especificar la descripción");
            if (obj.DuracionHoras <= 0)
                response.Errors.Add("La duración debe ser mayor a 0");

            if (!response.IsValid)
                return response;

            _repository.Create(obj);
            _repository.Save();

            return response;
        }
        public ResponseMessage DefinicionProcesoUpdate(DefinicionProceso obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(obj.Nombre))
                    response.Errors.Add("Debe especificar el nombre");
                if (string.IsNullOrEmpty(obj.Descripcion))
                    response.Errors.Add("Debe especificar la descripción");
                if (obj.DuracionHoras <= 0)
                    response.Errors.Add("La duración debe ser mayor a 0");

                if (response.IsValid)
                {
                    _repository.Update(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage DefinicionProcesoDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<DefinicionProceso>(id);
                if (obj == null)
                    response.Errors.Add("Dato no encontrado");
                if (obj != null && obj.Procesos.Any())
                    response.Errors.Add("La definición de proceso no puede ser eliminada ya que tiene procesos asociados");

                foreach (var item in obj.DefinicionWorkflows.ToList())
                    _repository.Delete(item);

                if (response.IsValid)
                {
                    _repository.Delete(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public ResponseMessage DefinicionWorkflowInsert(DefinicionWorkflow obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(obj.Nombre))
                    response.Errors.Add("Debe especificar el nombre");

                if (response.IsValid)
                {
                    if (obj.Pl_UndCod.HasValue)
                    {
                        var unidad = _sigper.GetUnidad(obj.Pl_UndCod.Value);
                        if (unidad != null)
                        {
                            obj.Pl_UndCod = unidad.Pl_UndCod;
                            obj.Pl_UndDes = unidad.Pl_UndDes;
                        }
                    }
                    if (obj.GrupoId.HasValue)
                        obj.Grupo = _repository.GetById<Grupo>(obj.GrupoId);


                    obj.Habilitado = true;
                    _repository.Create(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage DefinicionWorkflowUpdate(DefinicionWorkflow obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(obj.Nombre))
                    response.Errors.Add("Debe especificar el nombre");

                if (response.IsValid)
                {

                    if (obj.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico)
                    {
                        if (obj.Pl_UndCod.HasValue)
                        {
                            var unidad = _sigper.GetUnidad(obj.Pl_UndCod.Value);
                            if (unidad != null)
                            {
                                obj.Pl_UndCod = unidad.Pl_UndCod;
                                obj.Pl_UndDes = unidad.Pl_UndDes;
                            }
                        }
                        if (obj.GrupoId.HasValue)
                            obj.Grupo = _repository.GetById<Grupo>(obj.GrupoId);

                        obj.Email = null;

                    }
                    if (obj.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaUsuarioEspecifico)
                    {
                        obj.GrupoId = null;

                        var persona = _sigper.GetUserByEmail(obj.Email);
                        if (persona != null)
                        {
                            obj.Pl_UndCod = persona.Unidad.Pl_UndCod;
                            obj.Pl_UndDes = persona.Unidad.Pl_UndDes;
                            obj.Email = persona.Funcionario.Rh_Mail.Trim();
                        }
                    }

                    _repository.Update(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage DefinicionWorkflowDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<DefinicionWorkflow>(id);
                if (obj == null)
                    response.Errors.Add("Dato no encontrado");

                if (response.IsValid)
                {
                    obj.Habilitado = false;
                    _repository.Update(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public ResponseMessage GrupoInsert(Grupo obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(obj.Nombre))
                    response.Errors.Add("Debe especificar el nombre");

                if (response.IsValid)
                {
                    _repository.Create(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage GrupoUpdate(Grupo obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(obj.Nombre))
                    response.Errors.Add("Debe especificar el nombre");

                if (response.IsValid)
                {
                    _repository.Update(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage GrupoDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Grupo>(id);
                if (obj == null)
                    response.Errors.Add("Dato no encontrado");

                if (response.IsValid)
                {
                    foreach (var item in obj.Usuarios.ToList())
                        item.Habilitado = false;

                    _repository.Delete(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public ResponseMessage ProcesoInsert(Proceso obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (!_repository.GetExists<DefinicionProceso>(q => q.DefinicionProcesoId == obj.DefinicionProcesoId))
                    throw new ArgumentNullException("No se encontró la definición del proceso");

                if (string.IsNullOrWhiteSpace(obj.Email))
                    throw new ArgumentNullException("No se encontró el usuario que ejecutó el workflow.");

                var definicionProceso = _repository.GetById<DefinicionProceso>(obj.DefinicionProcesoId);
                if (definicionProceso == null)
                    throw new ArgumentNullException("No se encontró la definición proceso.");

                var definicionWorkflow = _repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == obj.DefinicionProcesoId).OrderBy(q => q.Secuencia).ThenBy(q => q.DefinicionWorkflowId).FirstOrDefault();
                if (definicionWorkflow == null)
                    throw new ArgumentNullException("No se encontró la definición de tarea del proceso asociado al workflow.");

                var persona = new SIGPER();
                persona = _sigper.GetUserByEmail(obj.Email);

                var proceso = new Proceso();
                proceso.DefinicionProcesoId = obj.DefinicionProcesoId;
                proceso.Observacion = obj.Observacion;
                proceso.FechaCreacion = DateTime.Now;
                proceso.FechaVencimiento = DateTime.Now.AddBusinessDays(definicionProceso.DuracionHoras);
                proceso.FechaTermino = null;
                proceso.Email = obj.Email;
                proceso.EstadoProcesoId = (int)App.Util.Enum.EstadoProceso.EnProceso;
                proceso.NombreFuncionario = persona != null && persona.Funcionario != null? persona.Funcionario.PeDatPerChq.Trim() : null;

                var workflow = new Workflow();
                workflow.FechaCreacion = DateTime.Now;
                workflow.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.SinAprobacion;
                workflow.Terminada = false;
                workflow.Proceso = proceso;
                workflow.DefinicionWorkflow = definicionWorkflow;
                workflow.FechaVencimiento = DateTime.Now.AddBusinessDays(definicionWorkflow.DefinicionProceso.DuracionHoras);

                switch (definicionWorkflow.TipoEjecucionId)
                {
                    case (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso:

                        persona = _sigper.GetUserByEmail(proceso.Email);
                        if (persona.Funcionario == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");
                        workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;

                        break;

                    case (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso:

                        persona = _sigper.GetUserByEmail(proceso.Email);
                        if (persona.Funcionario == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");
                        workflow.Email = persona.Jefatura.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;

                        break;

                    case (int)App.Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico:

                        workflow.GrupoId = definicionWorkflow.GrupoId;
                        workflow.Pl_UndCod = definicionWorkflow.Pl_UndCod;
                        workflow.Pl_UndDes = definicionWorkflow.Pl_UndDes;
                        workflow.TareaPersonal = false;

                        break;


                    case (int)App.Util.Enum.TipoEjecucion.EjecutaUsuarioEspecifico:

                        persona = _sigper.GetUserByEmail(definicionWorkflow.Email);
                        if (persona.Funcionario == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");

                        workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes.Trim();
                        workflow.TareaPersonal = true;

                        break;
                }

                proceso.Workflows.Add(workflow);

                _repository.Create(proceso);
                _repository.Save();

                ////notificar al dueño del proceso
                if (workflow.DefinicionWorkflow.NotificarAlAutor)
                    _email.NotificarInicioProceso(proceso,
                    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaCorreoNuevoProceso),
                    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacionTarea));

                //notificar por email al destinatario de la tarea
                if (workflow.DefinicionWorkflow.NotificarAsignacion)
                    _email.NotificarCambioWorkflow(workflow,
                    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaCorreoNotificacionTarea),
                    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacionTarea));

                response.EntityId = proceso.ProcesoId;
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage ProcesoDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Proceso>(id);
                if (response.IsValid)
                {
                    //terminar todas las tareas pendientes
                    foreach (var workflow in obj.Workflows.Where(q => !q.Terminada))
                    {
                        workflow.FechaTermino = DateTime.Now;
                        workflow.Terminada = true;
                        workflow.Anulada = true;
                    }

                    //terminar proceso
                    obj.FechaTermino = DateTime.Now;
                    obj.Terminada = true;
                    obj.Anulada = true;
                    obj.EstadoProcesoId = (int)App.Util.Enum.EstadoProceso.Anulado;
                    _repository.Save();

                    //notificar al dueño del proceso
                    _email.NotificarInicioProceso(obj,
                    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaCorreoProcesoAnulado),
                    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacionTarea));

                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public ResponseMessage WorkflowForward(Workflow obj)
        {
            var response = new ResponseMessage();
            try
            {
                if (obj == null)
                    throw new Exception("Debe especificar un workflow.");

                var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == obj.WorkflowId) ?? null;
                if (workflowActual == null)
                    throw new Exception("No se encontró el workflow.");

                if (string.IsNullOrWhiteSpace(obj.Email))
                    throw new Exception("No se encontró el usuario que ejecutó el workflow.");

                workflowActual.FechaTermino = DateTime.Now;
                workflowActual.Observacion = obj.Observacion;
                workflowActual.Email = obj.Email;
                workflowActual.Terminada = true;
                workflowActual.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.Aprobada;

                Workflow workflow = null;
                workflow = new Workflow();
                workflow.FechaCreacion = DateTime.Now;
                workflow.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.SinAprobacion;
                workflow.Terminada = false;
                workflow.DefinicionWorkflow = workflowActual.DefinicionWorkflow;
                workflow.ProcesoId = workflowActual.ProcesoId;
                workflow.Mensaje = obj.Observacion;

                //unidad o funcionario
                if (obj.Pl_UndCod.HasValue)
                {
                    var unidad = _sigper.GetUnidad(obj.Pl_UndCod.Value);
                    if (unidad == null)
                        throw new Exception("No se encontró la unidad en SIGPER.");

                    workflow.Pl_UndCod = unidad.Pl_UndCod;
                    workflow.Pl_UndDes = unidad.Pl_UndDes;
                }
                workflow.Email = obj.To;

                //grupo especial
                workflow.GrupoId = obj.GrupoId;


                _repository.Create(workflow);
                _repository.Save();

                //notificar por email
                if (workflow.DefinicionWorkflow.NotificarAsignacion)
                    _email.NotificarCambioWorkflow(workflow,
                    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaCorreoNotificacionTarea),
                    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacionTarea));
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage WorkflowUpdate(Workflow obj)
        {
            var response = new ResponseMessage();
            var persona = new SIGPER();

            try
            {
                if (obj == null)
                    throw new Exception("Debe especificar un workflow.");

                if (obj != null && obj.Email.IsNullOrWhiteSpace())
                    throw new Exception("No se encontró el usuario que ejecutó el workflow.");

                var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == obj.WorkflowId) ?? null;
                if (workflowActual == null)
                    throw new Exception("No se encontró el workflow.");

                if (workflowActual != null 
                    && workflowActual.DefinicionWorkflow != null
                    && workflowActual.DefinicionWorkflow.RequireDocumentacion
                    && !workflowActual.Proceso.Documentos.Any())
                    throw new Exception("Es necesario adjuntar documentos.");

                var definicionworkflowlist = _repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == workflowActual.Proceso.DefinicionProcesoId).OrderBy(q => q.Secuencia).ThenBy(q => q.DefinicionWorkflowId) ?? null;
                if (!definicionworkflowlist.Any())
                    throw new Exception("No se encontró la definición de tarea del proceso asociado al workflow.");

                if (workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar && (obj.TipoAprobacionId == null || obj.TipoAprobacionId == 0 || obj.TipoAprobacionId == 1))
                    throw new Exception("Es necesario aceptar o rechazar la tarea.");
                //workflowActual.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.Aprobada;

                //terminar workflow actual
                workflowActual.FechaTermino = DateTime.Now;
                workflowActual.Observacion = obj.Observacion;
                workflowActual.Terminada = true;
                workflowActual.Pl_UndCod = obj.Pl_UndCod;
                workflowActual.GrupoId = obj.GrupoId;
                workflowActual.Email = obj.Email;
                workflowActual.To = obj.To;

                if (workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar)
                    workflowActual.TipoAprobacionId = obj.TipoAprobacionId;

                if (!workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar)
                    workflowActual.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.Aprobada;

                //determinar siguiente tarea en base a estado y definicion de proceso
                DefinicionWorkflow definicionWorkflow = null;

                //si permite multiple evaluacion generar la misma tarea
                if (workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                    definicionWorkflow = _repository.GetById<DefinicionWorkflow>(workflowActual.DefinicionWorkflowId);

                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                else
                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);

                //en el caso de no existir mas tareas, cerrar proceso
                if (definicionWorkflow == null)
                {
                    workflowActual.Proceso.EstadoProcesoId = (int)App.Util.Enum.EstadoProceso.Terminado;
                    workflowActual.Proceso.Terminada = true;
                    workflowActual.Proceso.FechaTermino = DateTime.Now;
                    _repository.Save();
                }

                //en el caso de existir mas tareas, crearla
                if (definicionWorkflow != null)
                {
                    var workflow = new Workflow();
                    workflow.FechaCreacion = DateTime.Now;
                    workflow.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.SinAprobacion;
                    workflow.Terminada = false;
                    workflow.DefinicionWorkflow = definicionWorkflow;
                    workflow.ProcesoId = workflowActual.ProcesoId;
                    workflow.Mensaje = obj.Observacion;
                    //workflow.TareaPersonal = false;

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo)
                    {
                        if (obj.Pl_UndCod.HasValue)
                        {
                            var unidad = _sigper.GetUnidad(obj.Pl_UndCod.Value);
                            if (unidad == null)
                                throw new Exception("No se encontró la unidad en SIGPER.");

                            workflow.Pl_UndCod = unidad.Pl_UndCod;
                            workflow.Pl_UndDes = unidad.Pl_UndDes;
                            workflow.TareaPersonal = false;
                        }

                        if (!string.IsNullOrEmpty(obj.To))
                        {
                            persona = _sigper.GetUserByEmail(obj.To);
                            if (persona == null)
                                throw new Exception("No se encontró el usuario en SIGPER.");

                            workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                            workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                            workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                            workflow.TareaPersonal = true;
                        }
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaDestinoInicial)
                    {

                        var workflowInicial = _repository.Get<Workflow>(q => q.ProcesoId == workflowActual.ProcesoId && (q.To != null || q.Pl_UndCod != null) && q.WorkflowId != workflowActual.WorkflowId).OrderByDescending(q => q.WorkflowId).FirstOrDefault();
                        if (workflowInicial == null)
                            throw new Exception("No se encontró el workflow inicial.");

                        if (workflowInicial.Pl_UndCod.HasValue)
                        {
                            var unidad = _sigper.GetUnidad(workflowInicial.Pl_UndCod.Value);
                            if (unidad == null)
                                throw new Exception("No se encontró la unidad en SIGPER.");

                            workflow.Pl_UndCod = unidad.Pl_UndCod;
                            workflow.Pl_UndDes = unidad.Pl_UndDes;
                            workflow.TareaPersonal = false;
                        }

                        if (!string.IsNullOrEmpty(workflowInicial.To))
                        {
                            persona = _sigper.GetUserByEmail(workflowInicial.To);
                            if (persona == null)
                                throw new Exception("No se encontró el usuario en SIGPER.");

                            workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                            workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                            workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                            workflow.TareaPersonal = true;
                        }
                    }

                    //if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaDestinoGD)
                    //{
                    //    //traer el ultimo ingreso GD
                    //    var workflowInicial = _repository.Get<Workflow>(q => q.ProcesoId == workflowActual.ProcesoId && q.EntityId != null).OrderByDescending(q => q.WorkflowId).FirstOrDefault();
                    //    if (workflowInicial == null)
                    //        throw new Exception("No se encontró el workflow inicial.");

                    //    var ingresogd = _repository.GetFirst<GDIngreso>(q => q.GDIngresoId == workflowInicial.EntityId);
                    //    if (ingresogd == null)
                    //        throw new Exception("No se encontró el ingreso de gestión documental.");

                    //    if (ingresogd != null)
                    //    {
                    //        workflow.Pl_UndCod = ingresogd.Pl_UndCod;
                    //        workflow.Pl_UndDes = ingresogd.Pl_UndDes;
                    //        workflow.Email = ingresogd.UsuarioDestino;
                    //        workflow.TareaPersonal = !string.IsNullOrWhiteSpace(workflow.Email);
                    //    }
                    //}

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso)
                    {
                        persona = _sigper.GetUserByEmail(workflowActual.Proceso.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");
                        workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso)
                    {
                        persona = _sigper.GetUserByEmail(workflowActual.Proceso.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");

                        var jefatura = _sigper.GetUserByEmail(persona.Jefatura.Rh_Mail.Trim());
                        if (jefatura == null)
                            throw new Exception("No se encontró la jefatura en SIGPER.");
                        workflow.Email = jefatura.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = jefatura.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = jefatura.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = jefatura.Unidad.Pl_UndDes;

                        workflow.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienEjecutoTareaAnterior)
                    {

                        persona = _sigper.GetUserByEmail(obj.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");
                        workflow.Email = persona.Jefatura.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico)
                    {

                        workflow.GrupoId = definicionWorkflow.GrupoId;
                        workflow.Pl_UndCod = definicionWorkflow.Pl_UndCod;
                        workflow.Pl_UndDes = definicionWorkflow.Pl_UndDes;
                        workflow.TareaPersonal = false;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaUsuarioEspecifico)
                    {
                        persona = _sigper.GetUserByEmail(definicionWorkflow.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");

                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes.Trim();
                        workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.TareaPersonal = true;
                    }

                    //guardar información
                    _repository.Create(workflow);
                    _repository.Save();

                    //notificar actualización del estado al dueño
                    if (workflowActual.DefinicionWorkflow.NotificarAlAutor)
                        _email.NotificarCambioWorkflow(workflowActual,
                        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaCorreoCambioEstado),
                        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacionTarea));

                    //notificar por email al ejecutor de proxima tarea
                    if (workflow.DefinicionWorkflow.NotificarAsignacion)
                        _email.NotificarCambioWorkflow(workflow,
                        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaCorreoNotificacionTarea),
                        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacionTarea));
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage WorkflowArchive(Workflow obj)
        {
            var response = new ResponseMessage();
            try
            {
                if (obj == null)
                    throw new Exception("Debe especificar un workflow.");

                var workflow = _repository.GetFirst<Workflow>(q => q.WorkflowId == obj.WorkflowId) ?? null;
                if (workflow == null)
                    throw new Exception("No se encontró el workflow.");

                if (string.IsNullOrWhiteSpace(obj.Email))
                    throw new Exception("No se encontró el usuario que ejecutó el workflow.");

                workflow.FechaTermino = DateTime.Now;
                workflow.Observacion = obj.Observacion;
                workflow.Email = obj.Email;
                workflow.Terminada = true;
                workflow.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.Aprobada;
                workflow.Proceso.Terminada = true;
                workflow.Proceso.FechaTermino = DateTime.Now;

                _repository.Save();

                //notificar por email
                if (workflow.DefinicionWorkflow.NotificarAsignacion)
                    _email.NotificarCambioWorkflow(workflow,
                    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaCorreoArchivoTarea),
                    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacionTarea));
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public ResponseMessage ConfiguracionInsert(Configuracion obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(obj.Nombre))
                    response.Errors.Add("Debe especificar el nombre");
                if (string.IsNullOrEmpty(obj.Valor))
                    response.Errors.Add("Debe especificar el valor");

                if (response.IsValid)
                {
                    _repository.Create(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage ConfiguracionUpdate(Configuracion obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(obj.Nombre))
                    response.Errors.Add("Debe especificar el nombre");
                if (string.IsNullOrEmpty(obj.Valor))
                    response.Errors.Add("Debe especificar el valor");

                if (response.IsValid)
                {
                    _repository.Update(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public ResponseMessage UsuarioInsert(Usuario obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(obj.Email))
                    response.Errors.Add("Debe especificar el email");

                if (response.IsValid)
                {
                    _repository.Create(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage UsuarioUpdate(Usuario obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(obj.Email))
                    response.Errors.Add("Debe especificar el email");

                if (response.IsValid)
                {
                    _repository.Update(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage UsuarioDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Usuario>(id);
                if (obj == null)
                    response.Errors.Add("Dato no encontrado");

                if (response.IsValid)
                {
                    obj.Habilitado = false;
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public ResponseMessage RubricaInsert(Rubrica obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (response.IsValid)
                {
                    _repository.Create(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage RubricaUpdate(Rubrica obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (response.IsValid)
                {
                    _repository.Update(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage RubricaDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Rubrica>(id);
                if (obj == null)
                    response.Errors.Add("Dato no encontrado");

                if (response.IsValid)
                {
                    _repository.Delete(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public ResponseMessage DocumentoSign(Documento obj, string email)
        {
            var response = new ResponseMessage();

            try
            {
                var documento = _repository.GetById<Documento>(obj.DocumentoId);
                if (documento == null)
                    response.Errors.Add("Documento no encontrado");

                if (obj.Signed == true)
                    response.Errors.Add("Documento ya se encuentra firmado");

                 var rubrica = _repository.GetFirst<Rubrica>(q => q.Email == email && q.HabilitadoFirma);
               if (rubrica == null)
                    response.Errors.Add("No se encontraron firmas habilitadas para el usuario");

                var HSMUser = _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.HSMUser);
                if (HSMUser == null)
                    response.Errors.Add("No se encontró la configuración de usuario de HSM.");
                if (HSMUser != null && string.IsNullOrWhiteSpace(HSMUser.Valor))
                    response.Errors.Add("La configuración de usuario de HSM es inválida.");

                var HSMPassword = _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.HSMPassword);
                if (HSMPassword == null)
                    response.Errors.Add("No se encontró la configuración de usuario de HSM.");
                if (HSMPassword != null && string.IsNullOrWhiteSpace(HSMPassword.Valor))
                    response.Errors.Add("La configuración de password de HSM es inválida.");

                if (response.IsValid)
                {
                    documento.File = _hsm.Sign(documento.File, rubrica.IdentificadorFirma, rubrica.UnidadOrganizacional, null,null); 
                    documento.Signed = true;

                    _repository.Update(documento);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}