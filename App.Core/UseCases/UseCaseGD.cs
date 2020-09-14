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
            var TipoEjecucionId = 0;

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

                //tarea requiere asistencia secretaria
                if (personaOrigen.Unidad.Pl_UndCod != personaDestino.Unidad.Pl_UndCod)
                {
                    definicionWorkflow = _repository.GetById<DefinicionWorkflow>(workflowActual.DefinicionWorkflowId);
                }

                //si permite multiple evaluacion generar la misma tarea
                else if (workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                {
                    definicionWorkflow = _repository.GetById<DefinicionWorkflow>(workflowActual.DefinicionWorkflowId);
                    TipoEjecucionId = definicionWorkflow.TipoEjecucionId;
                }


                //traer la siguiente tarea
                else
                {
                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                    TipoEjecucionId = definicionWorkflow.TipoEjecucionId;
                }



                //terminar workflow actual
                workflowActual.FechaTermino = DateTime.Now;
                workflowActual.Observacion = obj.Observacion;
                workflowActual.Terminada = true;
                workflowActual.Pl_UndCod = obj.Pl_UndCod;
                workflowActual.GrupoId = obj.GrupoId;
                workflowActual.Email = obj.Email;
                workflowActual.To = obj.To;

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

                    //establecer origen
                    workflow.OrigenUnidad = personaOrigen.Unidad.Pl_UndCod;
                    workflow.OrigenUsuario = personaOrigen.Funcionario.Rh_Mail.Trim();

                    if (personaOrigen.Unidad.Pl_UndCod != personaDestino.Unidad.Pl_UndCod)
                    {
                        // buscar ruta
                        List<string> ruta = new List<string>();
                        //mi secretaria
                        ruta.Add(personaOrigen.Secretaria.Rh_Mail);
                        //su secretaria
                        ruta.Add(personaDestino.Secretaria.Rh_Mail);
                        //su secretaria
                        ruta.Add(personaDestino.Funcionario.Rh_Mail);
                    }

                    if (TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo)
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

                            workflow.DestinoUnidad = persona.Unidad.Pl_UndCod;
                            workflow.DestinoUsuario = persona.Funcionario.Rh_Mail.Trim();
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

                    if (TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaSecretariaUnidadOrigen)
                    {
                        persona = _sigper.GetUserByEmail(workflowActual.OrigenUsuario);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");

                        if (persona.Secretaria == null)
                            throw new Exception("No se encontró la secretaria de la unidad en SIGPER.");

                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes.Trim();
                        workflow.Email = persona.Secretaria.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Secretaria.PeDatPerChq.Trim();
                        workflow.TareaPersonal = true;

                        workflow.DestinoUnidad = persona.Unidad.Pl_UndCod;
                        workflow.DestinoUsuario = persona.Secretaria.Rh_Mail.Trim();

                        workflow.DestinoUnidadFinal = personaDestino.Unidad.Pl_UndCod;
                        workflow.DestinoUsuarioFinal = personaDestino.Funcionario.Rh_Mail.Trim();
                    }

                    if (TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaSecretariaUnidadDestino)
                    {
                        persona = _sigper.GetUserByEmail(workflowActual.OrigenUsuario);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");

                        if (persona.Secretaria == null)
                            throw new Exception("No se encontró la secretaria de la unidad en SIGPER.");

                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes.Trim();
                        workflow.Email = persona.Secretaria.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Secretaria.PeDatPerChq.Trim();
                        workflow.TareaPersonal = true;

                        workflow.DestinoUnidad = persona.Unidad.Pl_UndCod;
                        workflow.DestinoUsuario = persona.Secretaria.Rh_Mail.Trim();

                        workflow.DestinoUnidadFinal = personaDestino.Unidad.Pl_UndCod;
                        workflow.DestinoUsuarioFinal = personaDestino.Funcionario.Rh_Mail.Trim();
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
    }
}