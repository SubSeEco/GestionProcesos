﻿using System;
using App.Model.Core;
using App.Model.FirmaDocumentoGenerico;
using App.Core.Interfaces;
using App.Model.SIGPER;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using App.Util;

namespace App.Core.UseCases
{
    public class UseCaseFirmaDocumentoGenerico
    {
        protected readonly IGestionProcesos _repository;
        protected readonly IHSM _hsm;
        protected readonly ISIGPER _sigper;
        protected readonly IEmail _email;
        protected readonly IFolio _folio;
        protected readonly IFile _file;

        public UseCaseFirmaDocumentoGenerico(IGestionProcesos repository)
        {
            _repository = repository;
        }
        public UseCaseFirmaDocumentoGenerico(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IHSM hsm, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
            _email = email;
        }
        public ResponseMessage Insert(FirmaDocumentoGenerico obj)
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
        public ResponseMessage Edit(FirmaDocumentoGenerico obj)
        {
            var response = new ResponseMessage();

            //validaciones
            if (obj == null)
                response.Errors.Add("No se encontró información del archivo a firmar");
            if (obj != null && string.IsNullOrWhiteSpace(obj.TipoDocumentoCodigo))
                response.Errors.Add("No se encontró el dato tipo documento");

            if (response.IsValid)
            {
                var firmaDocumento = _repository.GetFirst<FirmaDocumentoGenerico>(q => q.FirmaDocumentoGenericoId == obj.FirmaDocumentoGenericoId);
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
            var response = new ResponseMessage();
            var persona = new SIGPER();

            if (id == 0)
                response.Errors.Add("Id de documento a firmar no encontrado");
            var firmaDocumento = _repository.GetById<FirmaDocumentoGenerico>(id);
            if (firmaDocumento == null)
                response.Errors.Add("Documento a firmar no encontrado");
            if (firmaDocumento != null && firmaDocumento.DocumentoSinFirma == null)
                response.Errors.Add("Documento a firmar sin contenido");

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

            if (string.IsNullOrEmpty(firmante))
                response.Errors.Add("Debe especificar el email del usuario firmante");

            if (!string.IsNullOrEmpty(firmante))
            {
                persona = _sigper.GetUserByEmail(firmante);
                if (persona == null)
                    response.Errors.Add("No se encontró usuario firmante en sistema SIGPER");

                if (persona != null && string.IsNullOrWhiteSpace(persona.SubSecretaria))
                    response.Errors.Add("No se encontró la subsecretaría del firmante");
            }

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

            //si el documento ya tiene folio, no solicitarlo nuevamente
            if (string.IsNullOrWhiteSpace(firmaDocumento.Folio))
            {
                try
                {
                    var _folioResponse = _folio.GetFolio(string.Join(", ", emailsFirmantes), firmaDocumento.TipoDocumentoCodigo, persona.SubSecretaria);
                    if (_folioResponse == null)
                        response.Errors.Add("Error al llamar el servicio externo de folio");

                    if (_folioResponse != null && _folioResponse.status == "ERROR")
                        response.Errors.Add(_folioResponse.error);

                    firmaDocumento.Folio = _folioResponse.folio;

                    _repository.Update(firmaDocumento);
                    _repository.Save();
                }
                catch (Exception ex)
                {
                    response.Errors.Add(ex.Message);
                }
            }

            if (!response.IsValid)
                return response;

            //crear nuevo documento
            var documento = new Documento()
            {
                Proceso = firmaDocumento.Proceso,
                Workflow = firmaDocumento.Workflow,
                Fecha = DateTime.Now,
                Email = firmaDocumento.Autor,
                Signed = false,
                TipoPrivacidadId = (int)App.Util.Enum.Privacidad.Publico,
                TipoDocumentoId = 6,
                //Folio = firmaDocumento.Folio,
            };
            _repository.Create(documento);
            _repository.Save();

            try
            {
                //generar código QR
                var _qrResponse = _file.CreateQR(string.Concat(url_tramites_en_linea.Valor, "/GPDocumentoVerificacion/Details/", documento.DocumentoId));

                //firmar documento
                var _hsmResponse = _hsm.Sign(firmaDocumento.DocumentoSinFirma, idsFirma, documento.DocumentoId, documento.Folio, url_tramites_en_linea.Valor, _qrResponse);


                //actualizar firma documento
                firmaDocumento.DocumentoConFirma = _hsmResponse;

                firmaDocumento.DocumentoConFirmaFilename = "Firmado " + firmaDocumento.DocumentoSinFirmaFilename;
                firmaDocumento.Firmante = string.Join(", ", idsFirma);
                firmaDocumento.Firmado = true;
                firmaDocumento.FechaFirma = DateTime.Now;
                firmaDocumento.DocumentoId = documento.DocumentoId;

                //actualizar documento con contenido firmado
                documento.File = _hsmResponse;

                documento.FileName = firmaDocumento.DocumentoConFirmaFilename;
                documento.Signed = true;

                //obtener metadata del documento
                var _metadataResponse = _file.BynaryToText(documento.File);
                if (_metadataResponse != null)
                {
                    documento.Texto = _metadataResponse.Text;
                    documento.Metadata = _metadataResponse.Metadata;
                    documento.Type = _metadataResponse.Type;
                }

                //actualizar datos
                _repository.Update(firmaDocumento);
                _repository.Update(documento);
            }
            catch (Exception ex)
            {
                documento.Activo = false;
                _repository.Update(documento);
                response.Errors.Add(ex.Message);
            }

            //guardar cambios
            _repository.Save();

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
                    var firma = _repository.GetFirst<FirmaDocumentoGenerico>(q => q.WorkflowId == obj.WorkflowId);
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

                            var emails = _sigper.GetUserByUnidad(workflow.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                            if (emails.Any())
                                workflow.Email = string.Join(";", emails);

                        }

                        if (!string.IsNullOrEmpty(obj.To))
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
        public ResponseMessage FixFolio()
        {
            var response = new ResponseMessage();

            var firmaDocumentos = _repository.GetAll<FirmaDocumentoGenerico>();
            foreach (var firmaDocumento in firmaDocumentos.ToList())
            {
                var documento = _repository.GetFirst<Documento>(q => q.File == firmaDocumento.DocumentoConFirma);
                if (documento != null)
                {
                    documento.FileName = firmaDocumento.DocumentoConFirmaFilename;
                    documento.Folio = firmaDocumento.Folio;
                    _repository.Update<Documento>(documento);

                    firmaDocumento.DocumentoId = documento.DocumentoId;
                    _repository.Update<FirmaDocumentoGenerico>(firmaDocumento);
                }
            }

            if (response.IsValid)
            {
                _repository.Save();
            }

            return response;
        }
        public ResponseMessage FixFileMetadata()
        {
            var response = new ResponseMessage();

            var ids = _repository.Get<Documento>(q => string.IsNullOrEmpty(q.Metadata)).Select(q => q.DocumentoId);
            foreach (var id in ids)
            {
                var doc = _repository.GetById<Documento>(id);

                //obtener metadata del documento
                var metadata = _file.BynaryToText(doc.File);
                if (metadata != null)
                {
                    doc.Texto = metadata.Text;
                    doc.Metadata = metadata.Metadata;
                    doc.Type = metadata.Type;
                }

                //actualizar datos
                _repository.Update(doc);
                _repository.Save();
            }

            return response;
        }
    }
}