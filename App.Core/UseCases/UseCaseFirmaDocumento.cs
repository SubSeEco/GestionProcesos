using App.Core.Interfaces;
using App.Model.Core;
using App.Model.FirmaDocumento;
using App.Model.Sigper;
using App.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Core.UseCases
{
    public class UseCaseFirmaDocumento
    {
        private readonly IGestionProcesos _repository;
        private readonly IHsm _hsm;
        private readonly ISigper _sigper;
        private readonly IEmail _email;
        private readonly IFolio _folio;
        private readonly IFile _file;
        public UseCaseFirmaDocumento(IGestionProcesos repository)
        {
            _repository = repository;
        }
        public UseCaseFirmaDocumento(IGestionProcesos repository, ISigper sigper, IFile file, IFolio folio, IHsm hsm, IEmail email)
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

            if (obj == null)
                response.Errors.Add("No se encontró información del archivo a firmar");
            if (obj != null && string.IsNullOrWhiteSpace(obj.TipoDocumentoCodigo))
                response.Errors.Add("No se encontró el dato tipo documento");
            if (obj != null && obj.DocumentoSinFirma == null)
                response.Errors.Add("No se encontró el archivo a firmar");

            if (response.IsValid)
            {
                _repository.Create(obj);
                _repository.Save();
            }

            return response;
        }
        public ResponseMessage Edit(FirmaDocumento obj)
        {
            var response = new ResponseMessage();

            //validaciones
            if (obj == null)
                response.Errors.Add("No se encontró información del archivo a firmar");
            if (obj != null && string.IsNullOrWhiteSpace(obj.TipoDocumentoCodigo))
                response.Errors.Add("No se encontró el dato tipo documento");

            if (response.IsValid)
            {
                var firmaDocumento = _repository.GetFirst<FirmaDocumento>(q => q.FirmaDocumentoId == obj.FirmaDocumentoId);
                if (firmaDocumento != null)
                {
                    if (obj.DocumentoSinFirma != null)
                        firmaDocumento.DocumentoSinFirma = obj.DocumentoSinFirma;

                    if (obj.DocumentoSinFirmaFilename != null)
                        firmaDocumento.DocumentoSinFirmaFilename = obj.DocumentoSinFirmaFilename;

                    firmaDocumento.TipoDocumentoCodigo = obj.TipoDocumentoCodigo;
                    firmaDocumento.TipoDocumentoDescripcion = obj.TipoDocumentoDescripcion;
                    firmaDocumento.URL = obj.URL;
                    firmaDocumento.Observaciones = obj.Observaciones;
                    firmaDocumento.Firmado = false;

                    _repository.Update(firmaDocumento);
                    _repository.Save();
                }
            }

            return response;
        }
        public ResponseMessage Sign(int id, List<string> emailsFirmantes, string firmante)
        {
            var responseCaseUse = new ResponseMessage();
            var persona = new Sigper();

            if (id == 0)
                responseCaseUse.Errors.Add("Id de documento a firmar no encontrado");
            var documentoOriginal = _repository.GetById<FirmaDocumento>(id);
            if (documentoOriginal == null)
                responseCaseUse.Errors.Add("Documento a firmar no encontrado");
            if (documentoOriginal != null && documentoOriginal.DocumentoSinFirma == null)
                responseCaseUse.Errors.Add("Documento a firmar sin contenido");

            var url_tramites_en_linea = _repository.GetFirst<Configuracion>(q => q.Nombre == nameof(Util.Enum.Configuracion.url_tramites_en_linea));
            if (url_tramites_en_linea == null)
                responseCaseUse.Errors.Add("No se encontró la configuración de la url de verificación de documentos");
            if (url_tramites_en_linea != null && url_tramites_en_linea.Valor.IsNullOrWhiteSpace())
                responseCaseUse.Errors.Add("No se encontró la configuración de la url de verificación de documentos");

            if (!emailsFirmantes.Any())
                responseCaseUse.Errors.Add("Debe especificar al menos un firmante");
            if (emailsFirmantes.Any())
                foreach (var email in emailsFirmantes)
                    if (!string.IsNullOrWhiteSpace(email) && !_repository.GetExists<Rubrica>(q => q.Email == email && q.HabilitadoFirma))
                        responseCaseUse.Errors.Add("No se encontró rúbrica habilitada para el firmante " + email);

            if (string.IsNullOrEmpty(firmante))
                responseCaseUse.Errors.Add("Debe especificar el email del usuario firmante");

            if (!string.IsNullOrEmpty(firmante))
            {
                persona = _sigper.GetUserByEmail(firmante);
                if (persona == null)
                    responseCaseUse.Errors.Add("No se encontró usuario firmante en sistema Sigper");

                if (persona != null && string.IsNullOrWhiteSpace(persona.SubSecretaria))
                    responseCaseUse.Errors.Add("No se encontró la subsecretaría del firmante");
            }

            if (!responseCaseUse.IsValid)
                return responseCaseUse;

            //listado de id de firmantes
            var idsFirma = new List<string>();
            foreach (var email in emailsFirmantes)
            {
                var rubrica = _repository.GetFirst<Rubrica>(q => q.Email == email && q.HabilitadoFirma);
                if (rubrica != null)
                    idsFirma.Add(rubrica.IdentificadorFirma);
            }

            //si el documento ya tiene folio, no solicitarlo nuevamente
            if (string.IsNullOrWhiteSpace(documentoOriginal.Folio))
            {
                try
                {
                    var _responseFolio = _folio.GetFolio(string.Join(", ", emailsFirmantes), documentoOriginal.TipoDocumentoCodigo, persona.SubSecretaria);
                    if (_responseFolio == null)
                        responseCaseUse.Errors.Add("Error al llamar el servicio externo de folio");

                    if (_responseFolio != null && _responseFolio.status == "ERROR")
                        responseCaseUse.Errors.Add(_responseFolio.error);

                    documentoOriginal.Folio = _responseFolio.folio;

                    _repository.Update(documentoOriginal);
                    _repository.Save();
                }
                catch (Exception ex)
                {
                    responseCaseUse.Errors.Add(ex.Message);
                }
            }

            if (!responseCaseUse.IsValid)
                return responseCaseUse;

            //crear nuevo documento
            var documentoFirmado = new Documento()
            {
                Proceso = documentoOriginal.Proceso,
                Workflow = documentoOriginal.Workflow,
                Fecha = DateTime.Now,
                Email = documentoOriginal.Autor,
                Signed = false,
                TipoPrivacidadId = (int)Util.Enum.Privacidad.Publico,
                TipoDocumentoId = 6,
                Folio = documentoOriginal.Folio,
                FirmanteEmail = firmante,
                FirmanteUnidad = persona.Unidad.Pl_UndDes.Trim(),
                FechaFirma = DateTime.Now,
                TipoDocumentoFirma = documentoOriginal.TipoDocumentoCodigo
            };
            _repository.Create(documentoFirmado);
            _repository.Save();

            try
            {
                //generar código QR
                var _responseQR = _file.CreateQr(string.Concat(url_tramites_en_linea.Valor, "/GPDocumentoVerificacion/Details/", documentoFirmado.DocumentoId));

                //firmar documento
                var _responseHSM = _hsm.SignWSDL(documentoOriginal.DocumentoSinFirma, idsFirma, documentoFirmado.DocumentoId, documentoFirmado.Folio, url_tramites_en_linea.Valor, _responseQR);

                //actualizar firma documento
                documentoOriginal.DocumentoConFirma = _responseHSM;
                documentoOriginal.DocumentoConFirmaFilename = "Firmado " + documentoOriginal.DocumentoSinFirmaFilename;
                documentoOriginal.Firmante = string.Join(", ", idsFirma);
                documentoOriginal.Firmado = true;
                documentoOriginal.FechaFirma = DateTime.Now;
                documentoOriginal.DocumentoId = documentoFirmado.DocumentoId;

                //actualizar documento con contenido firmado
                documentoFirmado.File = _responseHSM;
                documentoFirmado.FileName = documentoOriginal.DocumentoConFirmaFilename;
                documentoFirmado.Signed = true;

                //obtener metadata del documento
                var _responseMetadata = _file.BynaryToText(documentoFirmado.File);
                if (_responseMetadata != null)
                {
                    documentoFirmado.Texto = _responseMetadata.Text;
                    documentoFirmado.Metadata = _responseMetadata.Metadata;
                    documentoFirmado.Type = _responseMetadata.Type;
                }

                //actualizar datos
                _repository.Update(documentoOriginal);
                _repository.Update(documentoFirmado);
            }
            catch (Exception ex)
            {
                //documento.Activo = false;
                //_repository.Update(documento);
                responseCaseUse.Errors.Add(ex.Message);
            }

            //guardar cambios
            _repository.Save();

            return responseCaseUse;
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
                if (workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar && (obj.TipoAprobacionId == null || obj.TipoAprobacionId == 0 || obj.TipoAprobacionId == 1))
                    throw new Exception("Es necesario aceptar o rechazar la tarea.");

                //si ingreso esta ok y firmador aprueba sin firma...
                if (workflowActual.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == Util.Enum.Entidad.FirmaDocumento.ToString() && workflowActual.DefinicionWorkflow.Secuencia == 2)
                {
                    var firma = _repository.GetFirst<FirmaDocumento>(q => q.WorkflowId == obj.WorkflowId);
                    if (firma == null)
                        throw new Exception("No se encontró el ingreso de documento.");

                    if (firma != null && firma.DocumentoConFirma == null && obj.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
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
                    workflowActual.TipoAprobacionId = (int)Util.Enum.TipoAprobacion.Aprobada;

                //determinar siguiente tarea en base a estado y definicion de proceso
                DefinicionWorkflow definicionWorkflow = null;

                //si permite multiple evaluacion generar la misma tarea
                if (workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                    definicionWorkflow = _repository.GetById<DefinicionWorkflow>(workflowActual.DefinicionWorkflowId);

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
                        if (obj.Pl_UndCod.HasValue)
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

                        if (!string.IsNullOrEmpty(obj.To))
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
    }
}