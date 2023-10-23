using App.Core.Interfaces;
using App.Model.Cometido;
using App.Model.Core;
using App.Model.Sigper;
using App.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Core.UseCases
{
    public class UseCaseCore
    {
        private readonly IGestionProcesos _repository;
        private readonly IEmail _email;
        private readonly IHsm _hsm;
        private readonly ISigper _sigper;
        private readonly IFile _file;
        private readonly IFolio _folio;

        public UseCaseCore(IGestionProcesos repositoryGestionProcesos)
        {
            _repository = repositoryGestionProcesos;
        }
        public UseCaseCore(IGestionProcesos repository, ISigper sigper)
        {
            _repository = repository;
            _sigper = sigper;
        }
        public UseCaseCore(IGestionProcesos repository, IEmail email)
        {
            _repository = repository;
            _email = email;
        }
        public UseCaseCore(IGestionProcesos repository, IEmail email, ISigper sigper)
        {
            _repository = repository;
            _email = email;
            _sigper = sigper;
        }
        public UseCaseCore(IGestionProcesos repository, ISigper sigper, IFile file, IFolio folio, IHsm hsm, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
            _email = email;
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

                if (response.IsValid)
                {
                    foreach (var item in obj.DefinicionWorkflows.ToList())
                        _repository.Delete(item);

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
                //validaciones
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

                //traer informacion del ejecutor
                var persona = _sigper.GetUserByEmail(obj.Email.Trim());
                if (persona == null)
                    throw new ArgumentNullException("No se encontró información del usuario que inició el proceso.");

                //nuevo proceso
                var proceso = new Proceso();
                proceso.DefinicionProcesoId = obj.DefinicionProcesoId;
                proceso.Observacion = obj.Observacion;
                proceso.FechaCreacion = DateTime.Now;
                proceso.CalcularFechaVencimiento(_repository.Get<Festivo>().Select(q => q.Fecha).ToList());
                proceso.FechaTermino = null;
                proceso.Email = obj.Email.Trim();
                proceso.EstadoProcesoId = (int)Util.Enum.EstadoProceso.EnProceso;
                proceso.NombreFuncionario = persona != null && persona.Funcionario != null ? persona.Funcionario.PeDatPerChq.Trim() : null;
                if (obj.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.ConsultaIntegridad ||
                    obj.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.DenunciaIntegridad)
                {
                    proceso.Reservado = true;
                }
                else
                {
                    proceso.Reservado = obj.Reservado;
                }
                if (persona.Unidad != null)
                {
                    proceso.Pl_UndCod = persona.Unidad.Pl_UndCod;
                    proceso.Pl_UndDes = persona.Unidad.Pl_UndDes;
                }

                //nuevo workflow
                var workflow = new Workflow();
                workflow.FechaCreacion = DateTime.Now;
                workflow.TipoAprobacionId = (int)Util.Enum.TipoAprobacion.SinAprobacion;
                workflow.Terminada = false;
                workflow.Proceso = proceso;
                workflow.DefinicionWorkflow = definicionWorkflow;
                workflow.FechaVencimiento = proceso.FechaVencimiento;

                //determinar destino del workflow
                switch (definicionWorkflow.TipoEjecucionId)
                {
                    case (int)Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso:

                        persona = _sigper.GetUserByEmail(proceso.Email);
                        if (persona.Funcionario == null)
                            throw new Exception("No se encontró el usuario en Sigper.");
                        workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;

                        break;

                    case (int)Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso:

                        persona = _sigper.GetUserByEmail(proceso.Email);
                        if (persona.Funcionario == null)
                            throw new Exception("No se encontró el usuario en Sigper.");
                        workflow.Email = persona.Jefatura.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;

                        break;

                    case (int)Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico:

                        if (!workflow.Pl_UndCod.HasValue)
                            throw new Exception("No se encontró el grupo de destino." + definicionWorkflow.Pl_UndCod);

                        workflow.GrupoId = definicionWorkflow.GrupoId;
                        workflow.Pl_UndCod = definicionWorkflow.Pl_UndCod;
                        workflow.Pl_UndDes = definicionWorkflow.Pl_UndDes;
                        workflow.TareaPersonal = false;

                        var emails = _sigper.GetUserByUnidad(workflow.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                        if (emails.Any())
                            workflow.Email = string.Join(";", emails);

                        break;

                    case (int)Util.Enum.TipoEjecucion.EjecutaUsuarioEspecifico:

                        persona = _sigper.GetUserByEmail(definicionWorkflow.Email);
                        if (persona.Funcionario == null)
                            throw new Exception("No se encontró el usuario en Sigper.");

                        workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes.Trim();
                        workflow.TareaPersonal = true;

                        break;
                }

                //guardar información
                proceso.Workflows.Add(workflow);
                _repository.Create(proceso);
                _repository.Save();

                proceso.Tags = proceso.GetTags();
                _repository.Save();

                //notificar al dueño del proceso
                if (workflow.DefinicionWorkflow.NotificarAlAutor)
                    _email.NotificarInicioProceso(proceso,
                    _repository.GetFirst<Configuracion>(q => q.Nombre == nameof(Util.Enum.Configuracion.plantilla_nuevo_proceso)),
                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.AsuntoCorreoNotificacion));

                //notificar por email al destinatario de la tarea
                if (workflow.DefinicionWorkflow.NotificarAsignacion)
                    _email.NotificarNuevoWorkflow(workflow,
                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaNuevaTarea),
                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.AsuntoCorreoNotificacion));

                //reternar id del proceso para efectos de seguimiento
                response.EntityId = proceso.ProcesoId;
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage ProcesoDelete(int id, string JustificacionAnulacion)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Proceso>(id);
                var cometido = _repository.GetFirst<Cometido>(q => q.ProcesoId == obj.ProcesoId);
                var listaDestinos = new List<Destinos>();
                if (cometido != null)
                {
                    listaDestinos = _repository.GetAll<Destinos>().Where(q => q.CometidoId == cometido.CometidoId).ToList();
                }

                if (response.IsValid)
                {
                    if (obj.DefinicionProcesoId != (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                    {
                        foreach (var workflow in obj.Workflows.Where(q => !q.Terminada))
                        {
                            workflow.FechaTermino = DateTime.Now;
                            workflow.Terminada = true;
                            workflow.Anulada = true;
                        }
                    }
                    else
                    {
                        foreach (var workflow in obj.Workflows.Where(q => !q.Terminada))
                        {
                            if (workflow.DefinicionWorkflow.Secuencia != (int)Util.Enum.CometidoSecuencia.SolicitudCometido)
                            {
                                throw new Exception("No se puede anular el proceso debido a que ya cuenta con aprobación de Jefatura.");
                            }
                            else
                            {
                                workflow.FechaTermino = DateTime.Now;
                                workflow.Terminada = true;
                                workflow.Anulada = true;

                                foreach (var dest in listaDestinos)
                                {
                                    dest.DestinoActivo = false;
                                }
                            }
                        }
                    }
                    //terminar todas las tareas pendientes

                    //terminar proceso
                    obj.FechaTermino = DateTime.Now;
                    obj.EstadoProcesoId = (int)Util.Enum.EstadoProceso.Anulado;
                    obj.JustificacionAnulacion = JustificacionAnulacion;
                    _repository.Save();

                    //notificar al dueño del proceso
                    _email.NotificarAnulacionProceso(obj,
                    _repository.GetFirst<Configuracion>(q => q.Nombre == nameof(Util.Enum.Configuracion.plantilla_anulacion_proceso)),
                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.AsuntoCorreoNotificacion));
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
                workflowActual.TipoAprobacionId = (int)Util.Enum.TipoAprobacion.Aprobada;

                Workflow workflow = null;
                workflow = new Workflow();
                workflow.FechaCreacion = DateTime.Now;
                workflow.TipoAprobacionId = (int)Util.Enum.TipoAprobacion.SinAprobacion;
                workflow.Terminada = false;
                workflow.DefinicionWorkflow = workflowActual.DefinicionWorkflow;
                workflow.ProcesoId = workflowActual.ProcesoId;
                workflow.Mensaje = obj.Observacion;

                //unidad o funcionario
                if (obj.Pl_UndCod.HasValue)
                {
                    var unidad = _sigper.GetUnidad(obj.Pl_UndCod.Value);
                    if (unidad == null)
                        throw new Exception("No se encontró la unidad en Sigper.");

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
                    _email.NotificarNuevoWorkflow(workflow,
                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaNuevaTarea),
                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.AsuntoCorreoNotificacion));
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
            var persona = new Sigper();

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
                    workflowActual.TipoAprobacionId = (int)Util.Enum.TipoAprobacion.Aprobada;

                //determinar siguiente tarea en base a estado y definicion de proceso
                DefinicionWorkflow definicionWorkflow = null;

                //si permite multiple evaluacion generar la misma tarea
                if (workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                    definicionWorkflow = _repository.GetById<DefinicionWorkflow>(workflowActual.DefinicionWorkflowId);
                else

                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                else
                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);

                //en el caso de no existir mas tareas, cerrar proceso
                if (definicionWorkflow == null)
                {
                    workflowActual.Proceso.EstadoProcesoId = (int)Util.Enum.EstadoProceso.Terminado;
                    //workflowActual.Proceso.Terminada = true;
                    workflowActual.Proceso.FechaTermino = DateTime.Now;
                    _repository.Save();

                    //notificar al dueño del proceso
                    _email.NotificarFinProceso(workflowActual.Proceso,
                    _repository.GetFirst<Configuracion>(q => q.Nombre == nameof(Util.Enum.Configuracion.plantilla_fin_proceso)),
                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.AsuntoCorreoNotificacion));
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
                    //workflow.TareaPersonal = false;

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.CualquierPersonaGrupo)
                    {
                        // si seleccionó unidad y usuario...
                        if (obj.Pl_UndCod.HasValue && !string.IsNullOrEmpty(obj.To))
                        {
                            persona = _sigper.GetUserByEmail(obj.To);
                            if (persona == null)
                                throw new Exception("No se encontró el usuario en Sigper.");

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
                                throw new Exception("No se encontró la unidad en Sigper.");

                            workflow.Pl_UndCod = unidad.Pl_UndCod;
                            workflow.Pl_UndDes = unidad.Pl_UndDes;
                            workflow.TareaPersonal = false;

                            var emails = _sigper.GetUserByUnidad(workflow.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                            if (emails.Any())
                                workflow.Email = string.Join(";", emails);

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
                            workflow.TareaPersonal = false;

                            var emails = _sigper.GetUserByUnidad(workflow.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                            if (emails.Any())
                                workflow.Email = string.Join(";", emails);
                        }

                        if (!string.IsNullOrEmpty(workflowInicial.To))
                        {
                            persona = _sigper.GetUserByEmail(workflowInicial.To);
                            if (persona == null)
                                throw new Exception("No se encontró el usuario en Sigper.");

                            workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                            workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                            workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                            workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                            workflow.TareaPersonal = true;
                        }
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso)
                    {
                        persona = _sigper.GetUserByEmail(workflowActual.Proceso.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en Sigper.");
                        workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso)
                    {
                        persona = _sigper.GetUserByEmail(workflowActual.Proceso.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en Sigper.");

                        var jefatura = _sigper.GetUserByEmail(persona.Jefatura.Rh_Mail.Trim());
                        if (jefatura == null)
                            throw new Exception("No se encontró la jefatura en Sigper.");
                        workflow.Email = jefatura.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = jefatura.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = jefatura.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = jefatura.Unidad.Pl_UndDes;

                        workflow.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienEjecutoTareaAnterior)
                    {
                        persona = _sigper.GetUserByEmail(obj.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en Sigper.");
                        workflow.Email = persona.Jefatura.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico)
                    {
                        if (!definicionWorkflow.Pl_UndCod.HasValue && !definicionWorkflow.GrupoId.HasValue)
                            throw new Exception("No se especificó la unidad o grupo de destino.");

                        if (definicionWorkflow.Pl_UndCod.HasValue)
                        {
                            var unidad = _sigper.GetUnidad(definicionWorkflow.Pl_UndCod.Value);
                            if (unidad == null)
                                throw new Exception("No se encontró la unidad destino en Sigper.");
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

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaUsuarioEspecifico)
                    {
                        persona = _sigper.GetUserByEmail(definicionWorkflow.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en Sigper.");

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

                //si es la ultima tarea, terminar proceso
                if (workflow.Proceso.Workflows.Count(q => !q.Terminada) <= 1)
                {
                    //workflow.Proceso.Terminada = true;
                    workflow.Proceso.FechaTermino = DateTime.Now;
                    workflow.Proceso.EstadoProcesoId = (int)Util.Enum.EstadoProceso.Terminado;
                }

                //terminar tarea
                workflow.FechaTermino = DateTime.Now;
                workflow.Observacion = obj.Observacion;
                workflow.Email = obj.Email;
                workflow.Terminada = true;
                workflow.TipoAprobacionId = (int)Util.Enum.TipoAprobacion.Aprobada;
                workflow.EsTareaCierre = true;

                _repository.Save();

                if (workflow.Proceso.EstadoProcesoId == (int)Util.Enum.EstadoProceso.Terminado)
                    //notificar al dueño del proceso
                    _email.NotificarFinProceso(workflow.Proceso,
                    _repository.GetFirst<Configuracion>(q => q.Nombre == nameof(Util.Enum.Configuracion.plantilla_fin_proceso)),
                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.AsuntoCorreoNotificacion));
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

            if (_repository.GetExists<Rubrica>(q => q.Email == obj.Email && q.IdentificadorFirma == obj.IdentificadorFirma))
                response.Errors.Add("El email e identificador de firma ya está registrado");

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

            var rubrica = _repository.GetById<Usuario>(obj.RubricaId);
            if (rubrica == null)
                response.Errors.Add("Dato no encontrado");

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

        //Sobrecarga de firma multiple con tabla de verificacion
        public ResponseMessage Sign(int id, List<string> emailsFirmantes, string firmante)
        {
            var response = new ResponseMessage();

            if (id == 0)
                response.Errors.Add("Documento a firmar no encontrado");
            var documento = _repository.GetById<Documento>(id);
            if (documento == null)
                response.Errors.Add("Documento a firmar no encontrado");

            var url_tramites_en_linea = _repository.GetFirst<Configuracion>(q => q.Nombre == nameof(Util.Enum.Configuracion.url_tramites_en_linea));
            if (url_tramites_en_linea == null)
                response.Errors.Add("No se encontró la configuración de la url de verificación de documentos");
            if (url_tramites_en_linea != null && url_tramites_en_linea.Valor.IsNullOrWhiteSpace())
                response.Errors.Add("No se encontró la configuración de la url de verificación de documentos");

            if (!emailsFirmantes.Any())
                response.Errors.Add("Debe especificar al menos un firmante");
            if (emailsFirmantes.Any())
                foreach (var email in emailsFirmantes)
                    if (!string.IsNullOrWhiteSpace(email) && !_repository.GetExists<Rubrica>(q => q.Email == email && q.HabilitadoFirma))
                        response.Errors.Add("No se encontró rúbrica habilitada para el firmante " + email);

            var _responsePersona = _sigper.GetUserByEmail(firmante);
            if (_responsePersona == null)
                response.Errors.Add("No se encontró usuario firmante en sistema Sigper");

            if (_responsePersona != null && string.IsNullOrWhiteSpace(_responsePersona.SubSecretaria))
                response.Errors.Add("No se encontró la subsecretaría del firmante");


            if (!response.IsValid)
                return response;

            //listado de id de firmantes
            var idsFirma = new List<string>();
            foreach (var email in emailsFirmantes)
            {
                var rubrica = _repository.GetFirst<Rubrica>(q => q.Email == email && q.HabilitadoFirma);
                if (rubrica != null)
                    idsFirma.Add(rubrica.IdentificadorFirma);
            }

            //si el documento ya tiene folio no solicitarlo nuevamente
            if (string.IsNullOrWhiteSpace(documento.Folio))
            {
                var _responseFolio = _folio.GetFolio(string.Join(", ", emailsFirmantes), documento.TipoDocumentoFirma, _responsePersona.SubSecretaria);
                if (_responseFolio == null)
                    response.Errors.Add("Servicio de folio no entregó respuesta");

                if (_responseFolio != null && _responseFolio.status == "ERROR")
                    response.Errors.Add(_responseFolio.error);

                documento.Folio = _responseFolio.folio;

                _repository.Update(documento);
                _repository.Save();
            }

            if (!response.IsValid)
                return response;

            //generar código QR
            var _responseQR = _file.CreateQr(string.Concat(url_tramites_en_linea.Valor, "/GPDocumentoVerificacion/Details/", documento.DocumentoId));

            //firmar documento
            var _responseHSM = _hsm.SignWSDL(documento.File, idsFirma, documento.DocumentoId, documento.Folio, url_tramites_en_linea.Valor, _responseQR);

            //actualizar documento con contenido firmado
            documento.File = _responseHSM;
            documento.Signed = true;
            documento.FechaFirma = DateTime.Now;
            documento.FirmanteUnidad = _responsePersona.Unidad.Pl_UndDes.Trim();

            _repository.Update(documento);

            //guardar cambios
            _repository.Save();

            return response;
        }
    }
}