using System;
using App.Model.Core;
using App.Model.FirmaDocumento;
using App.Core.Interfaces;
using App.Model.SIGPER;
using System.Linq;

namespace App.Core.UseCases
{
    public class UseCaseFirmaDocumento
    {
        protected readonly IGestionProcesos _repository;
        protected readonly IHSM _hsm;
        protected readonly ISIGPER _sigper;
        protected readonly IEmail _email;
        protected readonly IFolio _folio;
        protected readonly IFile _file;

        public UseCaseFirmaDocumento(IGestionProcesos repository)
        {
            _repository = repository;
        }
        public UseCaseFirmaDocumento(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IHSM hsm, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
            _email = email;
        }
        public ResponseMessage Insert(FirmaDocumento obj)
        {
            var response = new ResponseMessage();

            //validaciones
            if (string.IsNullOrWhiteSpace(obj.TipoDocumentoCodigo))
                response.Errors.Add("Debe especificar el tipo de documento");
            if (obj.DocumentoSinFirma == null)
                response.Errors.Add("Debe especificar el archivo a firmar");

            if (response.IsValid)
            {
                _repository.Create(obj);
                _repository.Create(new Documento()
                {
                    ProcesoId = obj.ProcesoId,
                    WorkflowId = obj.WorkflowId,
                    Fecha = DateTime.Now,
                    Email = obj.Autor,
                    File = obj.DocumentoSinFirma,
                    FileName = obj.DocumentoSinFirmaFilename,
                    Signed = false,
                    Type = "application/pdf",
                    TipoPrivacidadId = 1,
                    TipoDocumentoId = 6
                });
                _repository.Save();
            }

            return response;
        }
        public ResponseMessage Edit(FirmaDocumento obj)
        {
            var response = new ResponseMessage();

            //validaciones
            if (string.IsNullOrWhiteSpace(obj.TipoDocumentoCodigo))
                response.Errors.Add("Debe especificar el tipo de documento");
            if (obj.DocumentoSinFirma == null)
                response.Errors.Add("Debe especificar el archivo a firmar");

            if (response.IsValid)
            {
                var ingreso = _repository.GetFirst<FirmaDocumento>(q => q.FirmaDocumentoId == obj.FirmaDocumentoId);
                if (ingreso != null)
                {
                    ingreso.TipoDocumentoCodigo = obj.TipoDocumentoCodigo;
                    ingreso.TipoDocumentoDescripcion = obj.TipoDocumentoDescripcion;
                    ingreso.DocumentoSinFirma = obj.DocumentoSinFirma;
                    ingreso.DocumentoSinFirmaFilename = obj.DocumentoSinFirmaFilename;
                    ingreso.Observaciones = obj.Observaciones;
                    ingreso.Autor = obj.Autor;
                    ingreso.Firmado = false;
                    ingreso.FechaCreacion = DateTime.Now;

                    _repository.Update(ingreso);
                    _repository.Create(new Documento()
                    {
                        ProcesoId = ingreso.ProcesoId,
                        WorkflowId = ingreso.WorkflowId,
                        Fecha = DateTime.Now,
                        Email = obj.Autor,
                        FileName = obj.DocumentoSinFirmaFilename,
                        File = obj.DocumentoSinFirma,
                        Signed = false,
                        Type = "application/pdf",
                        TipoPrivacidadId = 1,
                        TipoDocumentoId = 6
                    });
                    _repository.Save();
                }
            }

            return response;
        }
        public ResponseMessage Sign(int id, string firmante)
        {
            var response = new ResponseMessage();

            //validaciones...
            if (id == 0)
                response.Errors.Add("Documento a firmar no encontrado");
            if (string.IsNullOrWhiteSpace(firmante))
                response.Errors.Add("No se especificó el firmante");
            if (!string.IsNullOrWhiteSpace(firmante) && !_repository.GetExists<Rubrica>(q => q.Email == firmante && q.HabilitadoFirma))
                response.Errors.Add("No se encontró rúbrica habilitada para el firmante");

            var documento = _repository.GetById<FirmaDocumento>(id);
            if (documento == null)
                response.Errors.Add("Documento a firmar no encontrado");

            if (response.IsValid)
            {
                //si el documento ya tiene folio no solicitarlo nuevamente
                if (string.IsNullOrWhiteSpace(documento.Folio))
                {
                    try
                    {
                        var _folioResponse = _folio.GetFolio(firmante, documento.TipoDocumentoCodigo);
                        if (_folioResponse == null)
                            throw new Exception("Servicio no entregó respuesta");

                        if (_folioResponse != null && _folioResponse.status == "ERROR")
                            throw new Exception(_folioResponse.status);

                        documento.Folio = _folioResponse.folio;

                        _repository.Update(documento);
                        _repository.Save();

                    }
                    catch (Exception ex)
                    {
                        response.Errors.Add("Error al obtener folio del documento:" + ex.Message);
                    }
                }
            }

            //firmar documento
            if (response.IsValid)
            {
                try
                {
                    var rubrica = _repository.GetFirst<Rubrica>(q => q.Email == firmante && q.HabilitadoFirma);
                    var _hsmResponse = _hsm.Sign(documento.DocumentoSinFirma, rubrica.IdentificadorFirma, rubrica.UnidadOrganizacional, documento.Folio, null);

                    documento.DocumentoConFirma = _hsmResponse;
                    documento.DocumentoConFirmaFilename = "FIRMADO - " + documento.DocumentoSinFirmaFilename;
                    documento.Firmante = firmante;
                    documento.Firmado = true;
                    documento.FechaFirma = DateTime.Now;

                    _repository.Update(documento);
                    _repository.Create(new Documento()
                    {
                        ProcesoId = documento.ProcesoId,
                        WorkflowId = documento.WorkflowId,
                        Fecha = DateTime.Now,
                        Email = documento.Autor,
                        FileName = documento.DocumentoConFirmaFilename,
                        File = documento.DocumentoConFirma,
                        Signed = true,
                        Type = "application/pdf",
                        TipoPrivacidadId = 1,
                        TipoDocumentoId = 6
                    });

                    _repository.Save();
                }
                catch (Exception ex)
                {
                    response.Errors.Add("Error al firmar el documento:" + ex.Message);
                }
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
                var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == obj.WorkflowId) ?? null;
                if (workflowActual == null)
                    throw new Exception("No se encontró el workflow.");
                var definicionworkflowlist = _repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == workflowActual.Proceso.DefinicionProcesoId).OrderBy(q => q.Secuencia).ThenBy(q => q.DefinicionWorkflowId) ?? null;
                if (!definicionworkflowlist.Any())
                    throw new Exception("No se encontró la definición de tarea del proceso asociado al workflow.");
                if (string.IsNullOrWhiteSpace(obj.Email))
                    throw new Exception("No se encontró el usuario que ejecutó el workflow.");
                if (workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar && (obj.TipoAprobacionId == null || obj.TipoAprobacionId == 0 || obj.TipoAprobacionId == 1))
                    throw new Exception("Es necesario aceptar o rechazar la tarea.");

                //si ingreso esta ok y firmador aprueba sin firma...
                if (workflowActual.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == App.Util.Enum.Entidad.FirmaDocumento.ToString() && workflowActual.DefinicionWorkflow.Secuencia == 2)
                {

                    var firma = _repository.GetFirst<FirmaDocumento>(q => q.WorkflowId == obj.WorkflowId);
                    if (firma == null)
                        throw new Exception("No se encontró el ingreso de documento.");

                    if (firma != null && firma.DocumentoConFirma == null && obj.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                        throw new Exception("No se puede aprobar un documento no firmado.");
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
                            workflow.TareaPersonal = true;
                        }

                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso)
                    {
                        persona = _sigper.GetUserByEmail(workflowActual.Proceso.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");
                        workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso)
                    {
                        persona = _sigper.GetUserByEmail(workflowActual.Proceso.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");
                        workflow.Email = persona.Jefatura.Rh_Mail.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienEjecutoTareaAnterior)
                    {

                        persona = _sigper.GetUserByEmail(obj.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");
                        workflow.Email = persona.Jefatura.Rh_Mail.Trim();
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
    }
}