using System;
using System.Linq;
using App.Model.Core;
using App.Model.Sigper;
using App.Core.Interfaces;
using App.Model.HorasExtras;

namespace App.Core.UseCases
{
    public class UseCaseGeneraResolucion
    {
        private readonly IGestionProcesos _repository;
        private readonly ISigper _sigper;
        private readonly IEmail _email;


        public UseCaseGeneraResolucion(IGestionProcesos repository, ISigper sigper, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _email = email;
        }

        public ResponseMessage WorkflowUpdate(Workflow obj)
        {
            var response = new ResponseMessage();
            var persona = new Sigper();

            try
            {
                if (obj == null)
                    throw new Exception("Debe especificar un workflow.");
                var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == obj.WorkflowId) ?? null;
                if (workflowActual == null)
                    throw new Exception("No se encontró el workflow.");
                var definicionworkflowlist = _repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == workflowActual.Proceso.DefinicionProcesoId).OrderBy(q => q.Secuencia).ThenBy(q => q.DefinicionWorkflowId) ?? null;
                if (!definicionworkflowlist.Any())
                    throw new Exception("No se encontró la definición de tarea del proceso asociado al workflow.");
                if (string.IsNullOrWhiteSpace(obj.Email))
                    throw new Exception("No se encontró el usuario que ejecutó el workflow.");
                if (workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar && (obj.TipoAprobacionId == null || obj.TipoAprobacionId == 0))
                    workflowActual.TipoAprobacionId = (int)Util.Enum.TipoAprobacion.Aprobada;
                else
                {
                    if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any())
                        throw new Exception("Debe adjuntar documentos.");
                }

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
                    workflowActual.TipoAprobacionId = (int)Util.Enum.TipoAprobacion.Aprobada;

                //determinar siguiente tarea en base a estado y definicion de proceso
                DefinicionWorkflow definicionWorkflow = null;

                //si permite multiple evaluacion generar la misma tarea
                if (workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                    definicionWorkflow = _repository.GetById<DefinicionWorkflow>(workflowActual.DefinicionWorkflowId);
                else
                {
                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                    {
                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                    }
                    else
                    {
                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);
                    }
                }

                //en el caso de no existir mas tareas, cerrar proceso
                if (definicionWorkflow == null)
                {
                    //workflowActual.Proceso.Terminada = true;
                    workflowActual.Proceso.FechaTermino = DateTime.Now;
                    _repository.Save();
                }

                //en el caso de existir mas tareas, crearla
                if (definicionWorkflow != null)
                {
                    var workflow = new Workflow();
                    workflow.FechaCreacion = DateTime.Now;
                    workflow.TipoAprobacionId = (int)Util.Enum.TipoAprobacion.SinAprobacion;
                    workflow.Terminada = false;
                    workflow.DefinicionWorkflow = definicionWorkflow;
                    workflow.ProcesoId = workflowActual.ProcesoId;
                    workflow.Mensaje = obj.Observacion;
                    workflow.TareaPersonal = false;
                    workflow.Asunto = !string.IsNullOrEmpty(workflowActual.Asunto) ? workflowActual.Asunto : workflowActual.DefinicionWorkflow.DefinicionProceso.Nombre + " Nro: " + _repository.Get<HorasExtras>(c => c.ProcesoId == workflow.ProcesoId).FirstOrDefault().HorasExtrasId;

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.CualquierPersonaGrupo)
                    {
                        if (obj.Pl_UndCod.HasValue)
                        {
                            var unidad = _sigper.GetUnidad(obj.Pl_UndCod.Value);
                            if (unidad == null)
                                throw new Exception("No se encontró la unidad en Sigper.");

                            workflow.Pl_UndCod = unidad.Pl_UndCod;
                            workflow.Pl_UndDes = unidad.Pl_UndDes;
                        }

                        if (!string.IsNullOrEmpty(obj.To))
                        {
                            persona = _sigper.GetUserByEmail(obj.To);
                            if (persona == null)
                                throw new Exception("No se encontró el usuario en Sigper.");

                            workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                            workflow.TareaPersonal = true;
                        }
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaDestinoInicial)
                    {

                        var workflowInicial = _repository.Get<Workflow>(q => q.ProcesoId == workflowActual.ProcesoId && (q.To != null || q.Pl_UndCod != null) && q.WorkflowId != workflowActual.WorkflowId).OrderByDescending(q => q.WorkflowId).FirstOrDefault();
                        if (workflowInicial == null)
                            throw new Exception("No se encontró el workflow inicial.");

                        if (workflowInicial.Pl_UndCod.HasValue)
                        {
                            var unidad = _sigper.GetUnidad(workflowInicial.Pl_UndCod.Value);
                            if (unidad == null)
                                throw new Exception("No se encontró la unidad en Sigper.");

                            workflow.Pl_UndCod = unidad.Pl_UndCod;
                            workflow.Pl_UndDes = unidad.Pl_UndDes;
                        }

                        if (!string.IsNullOrEmpty(workflowInicial.To))
                        {
                            persona = _sigper.GetUserByEmail(workflowInicial.To);
                            if (persona == null)
                                throw new Exception("No se encontró el usuario en Sigper.");

                            workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                            workflow.TareaPersonal = true;
                        }
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso)
                    {
                        persona = _sigper.GetUserByEmail(workflowActual.Proceso.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en Sigper.");
                        workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso)
                    {
                        persona = _sigper.GetUserByEmail(workflowActual.Proceso.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en Sigper.");
                        workflow.Email = persona.Jefatura.Rh_Mail.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienEjecutoTareaAnterior)
                    {

                        persona = _sigper.GetUserByEmail(obj.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en Sigper.");
                        workflow.Email = persona.Jefatura.Rh_Mail.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;
                    }


                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico)
                    {

                        workflow.GrupoId = definicionWorkflow.GrupoId;
                        workflow.Pl_UndCod = definicionWorkflow.Pl_UndCod;
                        workflow.Pl_UndDes = definicionWorkflow.Pl_UndDes;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaUsuarioEspecifico)
                    {
                        persona = _sigper.GetUserByEmail(definicionWorkflow.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en Sigper.");

                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes.Trim();
                        workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflow.TareaPersonal = true;
                    }

                    //guardar información
                    _repository.Create(workflow);
                    _repository.Save();


                    //notificar actualización del estado al dueño
                    if (workflowActual.DefinicionWorkflow.NotificarAlAutor)
                        _email.NotificarNuevoWorkflow(workflowActual,
                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaCorreoCambioEstado),
                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.AsuntoCorreoNotificacion));

                    //notificar por email al ejecutor de proxima tarea
                    if (workflow.DefinicionWorkflow.NotificarAsignacion)
                        _email.NotificarNuevoWorkflow(workflow,
                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaNuevaTarea),
                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.AsuntoCorreoNotificacion));
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
