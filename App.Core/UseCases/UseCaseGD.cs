using System;
using App.Model.Core;
using App.Model.GestionDocumental;
using App.Core.Interfaces;
using App.Model.SIGPER;
using System.Linq;
using App.Util;

namespace App.Core.UseCases
{
    public class UseCaseGD
    {
        protected readonly IGestionProcesos _repository;
        protected readonly IEmail _email;
        protected readonly IHSM _hsm;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IFolio _folio;

        public UseCaseGD(IGestionProcesos repository, IFile file, IFolio folio)
        {
            _repository = repository;
            _file = file;
            _folio = folio;
        }
        public ResponseMessage Insert(GD obj)
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
        public ResponseMessage Update(GD obj)
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
        public ResponseMessage Delete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<GD>(id);
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

                    //notificar al dueño del proceso
                    _email.NotificarFinProceso(workflowActual.Proceso,
                    _repository.GetFirst<Configuracion>(q => q.Nombre == nameof(App.Util.Enum.Configuracion.plantilla_fin_proceso)),
                    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion));
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
                    workflow.Firmante = obj.Firmante;

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo)
                    {
                        // si seleccionó unidad y usuario...
                        if (obj.Pl_UndCod.HasValue && !string.IsNullOrEmpty(obj.To))
                        {
                            persona = _sigper.GetUserByEmail(obj.To);
                            if (persona == null)
                                throw new Exception("No se encontró el usuario en SIGPER.");

                            workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                            workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                            workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                            workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                            workflow.TareaPersonal = true;
                        }

                        // si seleccionó solo unidad ...
                        if (obj.Pl_UndCod.HasValue && string.IsNullOrEmpty(obj.To))
                        {
                            var unidad = _sigper.GetUnidad(obj.Pl_UndCod.Value);
                            if (unidad == null)
                                throw new Exception("No se encontró la unidad en SIGPER.");

                            workflow.Pl_UndCod = unidad.Pl_UndCod;
                            workflow.Pl_UndDes = unidad.Pl_UndDes;
                            workflow.TareaPersonal = false;

                            var emails = _sigper.GetUserByUnidad(workflow.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                            if (emails.Any())
                                workflow.Email = string.Join(";", emails);

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

                            var emails = _sigper.GetUserByUnidad(workflow.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                            if (emails.Any())
                                workflow.Email = string.Join(";", emails);
                        }

                        if (!string.IsNullOrEmpty(workflowInicial.To))
                        {
                            persona = _sigper.GetUserByEmail(workflowInicial.To);
                            if (persona == null)
                                throw new Exception("No se encontró el usuario en SIGPER.");

                            workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                            workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                            workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                            workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                            workflow.TareaPersonal = true;
                        }
                    }

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
                        if (!definicionWorkflow.Pl_UndCod.HasValue && !definicionWorkflow.GrupoId.HasValue)
                            throw new Exception("No se especificó la unidad o grupo de destino.");

                        if (definicionWorkflow.Pl_UndCod.HasValue)
                        {
                            var unidad = _sigper.GetUnidad(definicionWorkflow.Pl_UndCod.Value);
                            if (unidad == null)
                                throw new Exception("No se encontró la unidad destino en SIGPER.");
                            workflow.Pl_UndCod = definicionWorkflow.Pl_UndCod;
                            workflow.Pl_UndDes = definicionWorkflow.Pl_UndDes;
                            workflow.TareaPersonal = false;
                            var emails = _sigper.GetUserByUnidad(workflow.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                            if (emails.Any())
                                workflow.Email = string.Join(";", emails);
                        }
                        if (definicionWorkflow.GrupoId.HasValue)
                        {
                            var grupo = _repository.GetById<Grupo>(definicionWorkflow.GrupoId.Value);
                            if (grupo == null)
                                throw new Exception("No se encontró el grupo de destino.");
                            workflow.GrupoId = definicionWorkflow.GrupoId;
                            workflow.Pl_UndCod = null;
                            workflow.Pl_UndDes = null;
                            workflow.TareaPersonal = false;
                            var emails = grupo.Usuarios.Where(q => q.Habilitado).Select(q => q.Email);
                            if (emails.Any())
                                workflow.Email = string.Join(";", emails);
                        }

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

                    //notificar por email al ejecutor de proxima tarea
                    //se adjunta copia al autor
                    if (workflow.DefinicionWorkflow.NotificarAsignacion)
                        _email.NotificarNuevoWorkflow(workflow,
                        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaNuevaTarea),
                        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion));
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public ResponseMessage WorkflowUpdate2(Workflow obj)
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

                //terminar workflow actual
                workflowActual.FechaTermino = DateTime.Now;
                workflowActual.Observacion = obj.Observacion;
                workflowActual.Terminada = true;
                workflowActual.Pl_UndCod = obj.Pl_UndCod;
                workflowActual.GrupoId = obj.GrupoId;
                workflowActual.Email = obj.Email;
                workflowActual.To = obj.To;

                workflowActual.OrigenUnidad = obj.OrigenUnidad;
                workflowActual.OrigenUsuario = obj.OrigenUsuario;
                workflowActual.DestinoUnidad = obj.DestinoUnidad;
                workflowActual.DestinoUsuario = obj.DestinoUsuario;

                //si tarea requiere aprobación => tomar validacion del usuario
                if (workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar)
                    workflowActual.TipoAprobacionId = obj.TipoAprobacionId;

                //si tarea no requiere aprobación => asumir aprobada
                if (!workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar)
                    workflowActual.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.Aprobada;

                //determinar siguiente tarea en base a estado y definicion de proceso
                var definicionWorkflow = new DefinicionWorkflow();
                var terminarProceso = false;

                //si es tarea recursiva => generar la misma tarea
                if (workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                {
                    definicionWorkflow = _repository.GetById<DefinicionWorkflow>(workflowActual.DefinicionWorkflowId);
                }

                //si tarea sale de la unidad, asignarla a secretaria
                if (workflowActual.OrigenUnidad != workflowActual.DestinoUnidad)
                {
                    definicionWorkflow = workflowActual.DefinicionWorkflow;
                    definicionWorkflow.TipoEjecucionId = (int)App.Util.Enum.TipoEjecucion.EjecutaSecretariaMiUnidad;
                }

                //en el caso de que requiera aprobación => evaluar casos
                if (workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar)
                {
                    // en el caso de tarea aprobada => traer siguiente
                    if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);

                    // en el caso de tarea rechazada => traer tarea de rechazo configurada
                    else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);

                    //en el caso de no existir mas tareas => terminar proceso
                    if (definicionWorkflow == null)
                        terminarProceso = true;
                }

                //en el caso de no existir mas tareas, cerrar proceso
                if (terminarProceso)
                {
                    workflowActual.Proceso.EstadoProcesoId = (int)App.Util.Enum.EstadoProceso.Terminado;
                    workflowActual.Proceso.Terminada = true;
                    workflowActual.Proceso.FechaTermino = DateTime.Now;
                    _repository.Save();

                    //notificar al dueño del proceso
                    _email.NotificarFinProceso(workflowActual.Proceso,
                    _repository.GetFirst<Configuracion>(q => q.Nombre == nameof(App.Util.Enum.Configuracion.plantilla_fin_proceso)),
                    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion));
                }

                // en el caso de que el proceso continúe => crear siguiente tarea
                if (!terminarProceso)
                {
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
                        workflow.Firmante = obj.Firmante;

                        if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo)
                        {
                            // si seleccionó unidad y usuario...
                            if (obj.Pl_UndCod.HasValue && !string.IsNullOrEmpty(obj.To))
                            {
                                persona = _sigper.GetUserByEmail(obj.To);
                                if (persona == null)
                                    throw new Exception("No se encontró el usuario en SIGPER.");

                                workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                                workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                                workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                                workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                                workflow.TareaPersonal = true;
                            }

                            // si seleccionó solo unidad ...
                            if (obj.Pl_UndCod.HasValue && string.IsNullOrEmpty(obj.To))
                            {
                                var unidad = _sigper.GetUnidad(obj.Pl_UndCod.Value);
                                if (unidad == null)
                                    throw new Exception("No se encontró la unidad en SIGPER.");

                                workflow.Pl_UndCod = unidad.Pl_UndCod;
                                workflow.Pl_UndDes = unidad.Pl_UndDes;
                                workflow.TareaPersonal = false;

                                var emails = _sigper.GetUserByUnidad(workflow.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                                if (emails.Any())
                                    workflow.Email = string.Join(";", emails);

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

                                var emails = _sigper.GetUserByUnidad(workflow.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                                if (emails.Any())
                                    workflow.Email = string.Join(";", emails);
                            }

                            if (!string.IsNullOrEmpty(workflowInicial.To))
                            {
                                persona = _sigper.GetUserByEmail(workflowInicial.To);
                                if (persona == null)
                                    throw new Exception("No se encontró el usuario en SIGPER.");

                                workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                                workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                                workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                                workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                                workflow.TareaPersonal = true;
                            }
                        }

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
                            if (!definicionWorkflow.Pl_UndCod.HasValue && !definicionWorkflow.GrupoId.HasValue)
                                throw new Exception("No se especificó la unidad o grupo de destino.");

                            if (definicionWorkflow.Pl_UndCod.HasValue)
                            {
                                var unidad = _sigper.GetUnidad(definicionWorkflow.Pl_UndCod.Value);
                                if (unidad == null)
                                    throw new Exception("No se encontró la unidad destino en SIGPER.");
                                workflow.Pl_UndCod = definicionWorkflow.Pl_UndCod;
                                workflow.Pl_UndDes = definicionWorkflow.Pl_UndDes;
                                workflow.TareaPersonal = false;
                                var emails = _sigper.GetUserByUnidad(workflow.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                                if (emails.Any())
                                    workflow.Email = string.Join(";", emails);
                            }
                            if (definicionWorkflow.GrupoId.HasValue)
                            {
                                var grupo = _repository.GetById<Grupo>(definicionWorkflow.GrupoId.Value);
                                if (grupo == null)
                                    throw new Exception("No se encontró el grupo de destino.");
                                workflow.GrupoId = definicionWorkflow.GrupoId;
                                workflow.Pl_UndCod = null;
                                workflow.Pl_UndDes = null;
                                workflow.TareaPersonal = false;
                                var emails = grupo.Usuarios.Where(q => q.Habilitado).Select(q => q.Email);
                                if (emails.Any())
                                    workflow.Email = string.Join(";", emails);
                            }

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

                        if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaSecretariaMiUnidad)
                        {
                            persona = _sigper.GetUserByEmail(workflowActual.To);
                            if (persona == null)
                                throw new Exception("No se encontró el usuario en SIGPER.");

                            if (persona.Secretaria == null)
                                throw new Exception("No se encontró la secretaria de la unidad en SIGPER.");

                            workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                            workflow.Pl_UndDes = persona.Unidad.Pl_UndDes.Trim();
                            workflow.Email = persona.Secretaria.Rh_Mail.Trim();
                            workflow.NombreFuncionario = persona.Secretaria.PeDatPerChq.Trim();
                            workflow.TareaPersonal = true;
                        }

                        //guardar información
                        _repository.Create(workflow);
                        _repository.Save();

                        //notificar por email al ejecutor de proxima tarea
                        //se adjunta copia al autor
                        if (workflow.DefinicionWorkflow.NotificarAsignacion)
                            _email.NotificarNuevoWorkflow(workflow,
                            _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaNuevaTarea),
                            _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion));
                    }
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