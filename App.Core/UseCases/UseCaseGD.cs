using System;
using App.Model.Core;
using App.Model.GestionDocumental;
using App.Core.Interfaces;
using App.Model.SIGPER;
using System.Linq;
using App.Util;
using System.Collections.Generic;

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

        public UseCaseGD(IGestionProcesos repository, IFile file, IFolio folio, ISIGPER sigper, IEmail email)
        {
            _repository = repository;
            _file = file;
            _folio = folio;
            _sigper = sigper;
            _email = email;
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

        public ResponseMessage WorkflowUpdateInterno(Workflow obj)
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

                //traer informacion del ejecutor
                var personaOrigen = _sigper.GetUserByEmail(obj.Email);
                if (personaOrigen == null || personaOrigen.Funcionario == null)
                    throw new Exception(string.Format("No se encontró el usuario {0} en SIGPER.", obj.Email));

                //traer informacion del ejecutor
                var personaDestino = _sigper.GetUserByEmail(obj.To);
                if (personaDestino == null || personaDestino.Funcionario == null)
                    throw new Exception(string.Format("No se encontró el usuario {0} en SIGPER.", obj.To));

                if (workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar)
                    workflowActual.TipoAprobacionId = obj.TipoAprobacionId;

                if (!workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar)
                    workflowActual.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.Aprobada;

                //determinar siguiente tarea en base a estado y definicion de proceso
                DefinicionWorkflow definicionWorkflow = null;

                //si permite multiple evaluacion generar la misma tarea
                if (workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                    definicionWorkflow = _repository.GetById<DefinicionWorkflow>(workflowActual.DefinicionWorkflowId);

                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);

                else
                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);

                //terminar workflow actual
                workflowActual.FechaTermino = DateTime.Now;
                workflowActual.Observacion = obj.Observacion;
                workflowActual.Terminada = true;
                workflowActual.Pl_UndCod = obj.Pl_UndCod;
                workflowActual.GrupoId = obj.GrupoId;
                workflowActual.Email = obj.Email;
                workflowActual.To = obj.To.Trim();

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

                    return response;
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
                    //workflow.Firmante = obj.Firmante;
                    workflow.TareaPersonal = true;
                    workflow.To = workflowActual.To;

                    //es envío a otra unidad?
                    if (personaOrigen.Unidad.Pl_UndCod != personaDestino.Unidad.Pl_UndCod)
                    {
                        List<string> path = new List<string>();

                        //yo
                        if (personaOrigen.Funcionario != null)
                            path.Add(personaOrigen.Funcionario.Rh_Mail.Trim());

                        //secretaria mi unidad
                        if (personaOrigen.Secretaria != null)
                            path.Add(personaOrigen.Secretaria.Rh_Mail.Trim());

                        //secretaria unidad destino
                        if (personaDestino.Secretaria != null)
                            path.Add(personaDestino.Secretaria.Rh_Mail.Trim());

                        //destino final
                        path.Add(personaDestino.Funcionario.Rh_Mail.Trim());

                        foreach (var item in path)
                        {
                            if (workflowActual.Email.Trim() == item.Trim())
                                continue;

                            persona = _sigper.GetUserByEmail(item);
                            if (personaOrigen == null || personaOrigen.Funcionario == null)
                                throw new Exception(string.Format("No se encontró el usuario {0} en SIGPER.", item));

                            workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                            workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                            workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                            workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();

                            break;
                        }
                    }

                    // no, es envío dentro de mi unidad
                    else
                    {
                        workflow.Pl_UndCod = personaDestino.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = personaDestino.Unidad.Pl_UndDes;
                        workflow.Email = personaDestino.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = personaDestino.Funcionario.PeDatPerChq.Trim();
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

        public ResponseMessage WorkflowUpdateExterno(Workflow obj)
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

                var ejecutor = _sigper.GetUserByEmail(obj.Email);
                if (ejecutor == null || ejecutor.Funcionario == null)
                    throw new Exception(string.Format("No se encontró el usuario {0} en SIGPER.", obj.Email));

                //terminar workflow actual
                workflowActual.FechaTermino = DateTime.Now;
                workflowActual.Observacion = obj.Observacion;
                workflowActual.Terminada = true;
                workflowActual.Pl_UndCod = ejecutor.Unidad.Pl_UndCod;
                workflowActual.GrupoId = obj.GrupoId;
                workflowActual.Email = ejecutor.Funcionario.Rh_Mail.Trim();
                workflowActual.NombreFuncionario = ejecutor.Funcionario.PeDatPerChq.Trim();

                // si la tarea no tiene destino, asignarlo desde ingreso gd
                var gd = _repository.GetFirst<GD>(q => q.WorkflowId == workflowActual.WorkflowId);
                if (gd != null)
                    workflowActual.To = gd.DestinoFuncionarioEmail.Trim();

                if (workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar)
                    workflowActual.TipoAprobacionId = obj.TipoAprobacionId;

                if (!workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar)
                    workflowActual.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.Aprobada;

                //determinar siguiente tarea en base a estado y definicion de proceso
                DefinicionWorkflow definicionWorkflow = null;

                //si permite multiple evaluacion generar la misma tarea
                if (workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                    definicionWorkflow = _repository.GetById<DefinicionWorkflow>(workflowActual.DefinicionWorkflowId);

                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
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
                    var workflowSiguiente = new Workflow();
                    workflowSiguiente.FechaCreacion = DateTime.Now;
                    workflowSiguiente.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.SinAprobacion;
                    workflowSiguiente.Terminada = false;
                    workflowSiguiente.DefinicionWorkflow = definicionWorkflow;
                    workflowSiguiente.ProcesoId = workflowActual.ProcesoId;
                    workflowSiguiente.Mensaje = obj.Observacion;

                    //si hay destino manual, guardarlo
                    if (!workflowActual.To.IsNullOrWhiteSpace())
                        workflowSiguiente.To = workflowActual.To.Trim();

                    //si hay destino manual, guardarlo
                    if (!obj.To.IsNullOrWhiteSpace())
                        workflowSiguiente.To = obj.To.Trim();

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo)
                    {
                        //si es validacion de jefa de op => enviar secretaria de usuario destino
                        if (workflowActual.DefinicionWorkflow.Secuencia == 3)
                        {
                            var destino = _sigper.GetUserByEmail(workflowActual.To);
                            if (destino == null || destino.Funcionario == null)
                                throw new Exception(string.Format("No se encontró el usuario {0} en SIGPER.", workflowActual.To));

                            //tiene secretaria => asignar a secretaria
                            if (destino != null && destino.Secretaria != null)
                            {
                                workflowSiguiente.Pl_UndCod = destino.Unidad.Pl_UndCod;
                                workflowSiguiente.Pl_UndDes = destino.Unidad.Pl_UndDes;
                                workflowSiguiente.Email = destino.Secretaria.Rh_Mail.Trim();
                                workflowSiguiente.NombreFuncionario = destino.Secretaria.PeDatPerChq.Trim();
                                workflowSiguiente.TareaPersonal = true;
                            }

                            //no tiene secretaria => asignar a destino
                            if (destino != null && destino.Secretaria == null)
                            {
                                workflowSiguiente.Pl_UndCod = destino.Unidad.Pl_UndCod;
                                workflowSiguiente.Pl_UndDes = destino.Unidad.Pl_UndDes;
                                workflowSiguiente.Email = destino.Funcionario.Rh_Mail.Trim();
                                workflowSiguiente.NombreFuncionario = destino.Funcionario.PeDatPerChq.Trim();
                                workflowSiguiente.TareaPersonal = true;
                            }
                        }

                        //si es reenvio fuera de OP
                        if (workflowActual.DefinicionWorkflow.Secuencia > 3)
                        {
                            //traer informacion del ejecutor
                            var personaOrigen = _sigper.GetUserByEmail(workflowActual.Email);
                            if (personaOrigen == null || personaOrigen.Funcionario == null)
                                throw new Exception(string.Format("No se encontró el usuario {0} en SIGPER.", workflowSiguiente.Email));

                            //traer informacion del ejecutor
                            var personaDestino = _sigper.GetUserByEmail(workflowSiguiente.To);
                            if (personaDestino == null || personaDestino.Funcionario == null)
                                throw new Exception(string.Format("No se encontró el usuario {0} en SIGPER.", workflowSiguiente.To));

                            //es envío a otra unidad?
                            if (personaOrigen.Unidad.Pl_UndCod != personaDestino.Unidad.Pl_UndCod)
                            {
                                List<string> path = new List<string>();

                                //yo
                                if (personaOrigen.Funcionario != null)
                                    path.Add(personaOrigen.Funcionario.Rh_Mail.Trim());

                                //secretaria mi unidad
                                if (personaOrigen.Secretaria != null && personaOrigen.Secretaria.Rh_Mail.Trim() != personaOrigen.Funcionario.Rh_Mail.Trim())
                                    path.Add(personaOrigen.Secretaria.Rh_Mail.Trim());

                                //secretaria unidad destino
                                if (personaDestino.Secretaria != null)
                                    path.Add(personaDestino.Secretaria.Rh_Mail.Trim());

                                //destino final
                                path.Add(personaDestino.Funcionario.Rh_Mail.Trim());

                                foreach (var item in path)
                                {
                                    if (workflowActual.Email.Trim() == item.Trim())
                                        continue;

                                    persona = _sigper.GetUserByEmail(item);
                                    if (personaOrigen == null || personaOrigen.Funcionario == null)
                                        throw new Exception(string.Format("No se encontró el usuario {0} en SIGPER.", item));

                                    workflowSiguiente.Pl_UndCod = persona.Unidad.Pl_UndCod;
                                    workflowSiguiente.Pl_UndDes = persona.Unidad.Pl_UndDes;
                                    workflowSiguiente.Email = persona.Funcionario.Rh_Mail.Trim();
                                    workflowSiguiente.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                                    workflowSiguiente.TareaPersonal = true;

                                    break;
                                }
                            }

                            // no, es envío dentro de mi unidad
                            else
                            {
                                workflowSiguiente.Pl_UndCod = personaDestino.Unidad.Pl_UndCod;
                                workflowSiguiente.Pl_UndDes = personaDestino.Unidad.Pl_UndDes;
                                workflowSiguiente.Email = personaDestino.Funcionario.Rh_Mail.Trim();
                                workflowSiguiente.NombreFuncionario = personaDestino.Funcionario.PeDatPerChq.Trim();
                                workflowSiguiente.TareaPersonal = true;
                            }
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

                            workflowSiguiente.Pl_UndCod = unidad.Pl_UndCod;
                            workflowSiguiente.Pl_UndDes = unidad.Pl_UndDes;
                            workflowSiguiente.TareaPersonal = false;

                            var emails = _sigper.GetUserByUnidad(workflowSiguiente.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                            if (emails.Any())
                                workflowSiguiente.Email = string.Join(";", emails);
                        }

                        if (!string.IsNullOrEmpty(workflowInicial.To))
                        {
                            persona = _sigper.GetUserByEmail(workflowInicial.To);
                            if (persona == null)
                                throw new Exception("No se encontró el usuario en SIGPER.");

                            workflowSiguiente.Email = persona.Funcionario.Rh_Mail.Trim();
                            workflowSiguiente.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                            workflowSiguiente.Pl_UndCod = persona.Unidad.Pl_UndCod;
                            workflowSiguiente.Pl_UndDes = persona.Unidad.Pl_UndDes;
                            workflowSiguiente.TareaPersonal = true;
                        }
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso)
                    {
                        persona = _sigper.GetUserByEmail(workflowActual.Proceso.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");
                        workflowSiguiente.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflowSiguiente.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflowSiguiente.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflowSiguiente.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflowSiguiente.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso)
                    {
                        persona = _sigper.GetUserByEmail(workflowActual.Proceso.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");

                        var jefatura = _sigper.GetUserByEmail(persona.Jefatura.Rh_Mail.Trim());
                        if (jefatura == null)
                            throw new Exception("No se encontró la jefatura en SIGPER.");
                        workflowSiguiente.Email = jefatura.Funcionario.Rh_Mail.Trim();
                        workflowSiguiente.NombreFuncionario = jefatura.Funcionario.PeDatPerChq.Trim();
                        workflowSiguiente.Pl_UndCod = jefatura.Unidad.Pl_UndCod;
                        workflowSiguiente.Pl_UndDes = jefatura.Unidad.Pl_UndDes;

                        workflowSiguiente.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienEjecutoTareaAnterior)
                    {
                        persona = _sigper.GetUserByEmail(obj.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");
                        workflowSiguiente.Email = persona.Jefatura.Rh_Mail.Trim();
                        workflowSiguiente.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflowSiguiente.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflowSiguiente.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflowSiguiente.TareaPersonal = true;
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
                            workflowSiguiente.Pl_UndCod = definicionWorkflow.Pl_UndCod;
                            workflowSiguiente.Pl_UndDes = definicionWorkflow.Pl_UndDes;
                            workflowSiguiente.TareaPersonal = false;
                            var emails = _sigper.GetUserByUnidad(workflowSiguiente.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                            if (emails.Any())
                                workflowSiguiente.Email = string.Join(";", emails);
                        }
                        if (definicionWorkflow.GrupoId.HasValue)
                        {
                            var grupo = _repository.GetById<Grupo>(definicionWorkflow.GrupoId.Value);
                            if (grupo == null)
                                throw new Exception("No se encontró el grupo de destino.");
                            workflowSiguiente.GrupoId = definicionWorkflow.GrupoId;
                            workflowSiguiente.Pl_UndCod = null;
                            workflowSiguiente.Pl_UndDes = null;
                            workflowSiguiente.TareaPersonal = false;
                            var emails = grupo.Usuarios.Where(q => q.Habilitado).Select(q => q.Email);
                            if (emails.Any())
                                workflowSiguiente.Email = string.Join(";", emails);
                        }

                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaUsuarioEspecifico)
                    {
                        persona = _sigper.GetUserByEmail(definicionWorkflow.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");

                        workflowSiguiente.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflowSiguiente.Pl_UndDes = persona.Unidad.Pl_UndDes.Trim();
                        workflowSiguiente.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflowSiguiente.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflowSiguiente.TareaPersonal = true;
                    }

                    //guardar información
                    _repository.Create(workflowSiguiente);
                    _repository.Save();

                    //notificar por email al ejecutor de proxima tarea
                    //se adjunta copia al autor
                    if (workflowSiguiente.DefinicionWorkflow.NotificarAsignacion)
                        _email.NotificarNuevoWorkflow(workflowSiguiente,
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

    }
}