using System;
using System.Linq;
using App.Model.Core;
using App.Model.ProgramacionHorasExtraordinarias;
using App.Model.Sigper;
using System.Collections.Generic;
using App.Core.Interfaces;
using App.Util;
//using App.Infrastructure.Extensions;

namespace App.Core.UseCases
{
    public class UseCaseProgramacionHorasExtraordinarias
    {
        private readonly IGestionProcesos _repository;
        private readonly IHsm _hsm;
        private readonly ISigper _sigper;
        private readonly IFolio _folio;
        private readonly IFile _file;

        public UseCaseProgramacionHorasExtraordinarias(IGestionProcesos repository, ISigper sigper, IFile file, IFolio folio, IHsm hsm)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
        }

        public ResponseMessage ProgramacionHorasExtraordinariasInsert(ProgramacionHorasExtraordinarias obj)
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
        public ResponseMessage ProgramacionHorasExtraordinariasUpdate(ProgramacionHorasExtraordinarias obj)
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
        //public ResponseMessage ProgramacionHorasExtraordinariasDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<ProgramacionHorasExtraordinarias>(id);
        //        if (obj == null)
        //            response.Errors.Add("Dato no encontrado");

        //        if (response.IsValid)
        //        {
        //            _repository.Delete(obj);
        //            _repository.Save();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(ex.Message);
        //    }

        //    return response;
        //}

        public ResponseMessage Sign(int id, List<string> emailsFirmantes)
        {
            var response = new ResponseMessage();

            if (id == 0)
                response.Errors.Add("Documento a firmar no encontrado");
            var memo = _repository.GetById<ProgramacionHorasExtraordinarias>(id);
            if (memo == null)
                response.Errors.Add("Documento a firmar no encontrado");
            //if (memo != null && memo.DocumentoSinFirma == null)
            //    response.Errors.Add("Documento a firmar no encontrado");

            var url_tramites_en_linea = _repository.GetFirst<Configuracion>(q => q.Nombre == Util.Enum.Configuracion.url_tramites_en_linea.ToString());
            if (url_tramites_en_linea == null)
                response.Errors.Add("No se encontró la configuración de la url de verificación de documentos");
            if (url_tramites_en_linea != null && url_tramites_en_linea.Valor.IsNullOrWhiteSpace())
                response.Errors.Add("No se encontró la configuración de la url de verificación de documentos");

            if (!emailsFirmantes.Any())
                response.Errors.Add("Debe especificar al menos un firmante");
            if (emailsFirmantes.Any())
                foreach (var firmante in emailsFirmantes)
                    if (!string.IsNullOrWhiteSpace(firmante) && !_repository.GetExists<Rubrica>(q => q.Email == firmante && q.HabilitadoFirma))
                        response.Errors.Add("No se encontró rúbrica habilitada para el firmante " + firmante);

            if (!response.IsValid)
                return response;

            //listado de id de firmantes
            var idsFirma = new List<string>();
            foreach (var firmante in emailsFirmantes)
            {
                var rubrica = _repository.GetFirst<Rubrica>(q => q.Email == firmante && q.HabilitadoFirma);
                if (rubrica != null)
                    idsFirma.Add(rubrica.IdentificadorFirma);
            }

            //si el documento ya tiene folio no solicitarlo nuevamente
            //if (string.IsNullOrWhiteSpace(memo.Folio))
            //{
            //    try
            //    {
            //        //var _folioResponse = _folio.GetFolio(string.Join(", ", emailsFirmantes), "8" /*memo.TipoDocumentoCodigo*/);
            //        var _folioResponse = _folio.GetFolio(string.Join(", ", emailsFirmantes), memo.TipoDocumentoCodigo, "");

            //        if (_folioResponse == null)
            //            response.Errors.Add("Servicio de folio no entregó respuesta");

            //        if (_folioResponse != null && _folioResponse.status == "ERROR")
            //            response.Errors.Add(_folioResponse.error);

            //        memo.Folio = _folioResponse.folio;

            //        _repository.Update(memo);
            //        _repository.Save();
            //    }
            //    catch (Exception ex)
            //    {
            //        response.Errors.Add(ex.Message);
            //    }
            //}

            if (!response.IsValid)
                return response;

            ////crear nuevo documento
            //var documento = new Documento() {
            //    Proceso = memo.Proceso,
            //    Workflow = memo.Workflow,
            //    Fecha = DateTime.Now,
            //    //Email = memo.Autor,
            //    Email = memo.EmailRem.Trim(),
            //    Signed = false,
            //    Type = "application/pdf",
            //    TipoPrivacidadId = 1,
            //    TipoDocumentoId = 6,
            //    Folio = memo.Folio
            //};
            //_repository.Create(documento);
            //_repository.Save();

            var doc = _repository.GetFirst<Documento>(c => c.ProcesoId == memo.ProcesoId);

            //generar código QR
            var qr = _file.CreateQr(string.Concat(url_tramites_en_linea.Valor, "/GPDocumentoVerificacion/Details/", doc.DocumentoId));


            //firmar documento
            var _hsmResponse = _hsm.Sign(doc.File, idsFirma, doc.DocumentoId, doc.Folio, url_tramites_en_linea.Valor, qr);

            //byte[] _hsmResponse = null;
            //foreach (var item in idsFirma)
            //{
            //     _hsmResponse = _hsm.Sign(doc.File, item, null, doc.Folio, "");

            //}

            //actualizar firma documento
            doc.File = _hsmResponse;
            //memo.DocumentoConFirmaFilename = memo.DocumentoSinFirmaFilename;
            //memo.Firmante = string.Join(", ", idsFirma);
            doc.Signed = true;
            //memo.FechaFirma = DateTime.Now;
            //memo.DocumentoId = documento.DocumentoId;
            _repository.Update(doc);

            ////actualizar documento con contenido firmado
            //documento.File = _hsmResponse;
            //documento.FileName = memo.DocumentoConFirmaFilename;
            //documento.Signed = true;
            //_repository.Update(doc);

            //guardar cambios
            _repository.Save();

            return response;
        }


        public ResponseMessage SignReso(Documento obj, string email, int? programacionHorasExtraordinariasId)
        {
            var response = new ResponseMessage();
            var persona = new Sigper();

            try
            {
                var documento = _repository.GetById<Documento>(obj.DocumentoId);
                if (documento == null)
                    response.Errors.Add("Documento no encontrado");

                if (obj.Signed)
                    response.Errors.Add("Documento ya se encuentra firmado");

                var rubrica = _repository.GetFirst<Rubrica>(q => q.Email == email && q.HabilitadoFirma);
                if (rubrica == null)
                    response.Errors.Add("No se encontraron firmas habilitadas para el usuario");

                var HSMUser = _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.HSMUser);
                if (HSMUser == null)
                    response.Errors.Add("No se encontró la configuración de usuario de HSM.");
                if (HSMUser != null && string.IsNullOrWhiteSpace(HSMUser.Valor))
                    response.Errors.Add("La configuración de usuario de HSM es inválida.");

                var HSMPassword = _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.HSMPassword);
                if (HSMPassword == null)
                    response.Errors.Add("No se encontró la configuración de usuario de HSM.");
                if (HSMPassword != null && string.IsNullOrWhiteSpace(HSMPassword.Valor))
                    response.Errors.Add("La configuración de password de HSM es inválida.");

                var url_tramites_en_linea = _repository.GetFirst<Configuracion>(q => q.Nombre == nameof(Util.Enum.Configuracion.url_tramites_en_linea));
                if (url_tramites_en_linea == null)
                    response.Errors.Add("No se encontró la configuración de la url de verificación de documentos");
                if (url_tramites_en_linea != null && url_tramites_en_linea.Valor.IsNullOrWhiteSpace())
                    response.Errors.Add("No se encontró la configuración de la url de verificación de documentos");


                if (response.IsValid)
                {
                    /*se buscar la persona para determinar la subsecretaria*/
                    if (!string.IsNullOrEmpty(email))
                    {
                        persona = _sigper.GetUserByEmail(email);
                        if (persona == null)
                            response.Errors.Add("No se encontró usuario firmante en sistema Sigper");

                        if (persona != null && string.IsNullOrWhiteSpace(persona.SubSecretaria))
                            response.Errors.Add("No se encontró la subsecretaría del firmante");
                    }

                    /*Se busca cometido para determinar tipo de documento*/
                    string TipoDocto;
                    var com = _repository.GetFirst<ProgramacionHorasExtraordinarias>(c => c.ProgramacionHorasExtraordinariasId == programacionHorasExtraordinariasId.Value);

                    if (!string.IsNullOrEmpty(obj.TipoDocumentoFirma))
                        TipoDocto = obj.TipoDocumentoFirma;
                    else
                        TipoDocto = "OTRO";

                    //if (com != null)
                    //{
                    //    if (com.IdCalidad == 10)
                    //    {
                    //        TipoDocto = "RAEX";/*"ORPA";*/
                    //    }
                    //    else
                    //    {
                    //        switch (com.IdGrado)
                    //        {
                    //            case "B":/*Resolución Ministerial Exenta*/
                    //                TipoDocto = "RMEX";
                    //                break;
                    //            case "C": /*Resolución Ministerial Exenta*/
                    //                TipoDocto = "RMEX";
                    //                break;
                    //            default:
                    //                TipoDocto = "RAEX";/*Resolución Administrativa Exenta*/
                    //                break;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    if (!string.IsNullOrEmpty(obj.TipoDocumentoFirma))
                    //        TipoDocto = obj.TipoDocumentoFirma;
                    //    else
                    //        TipoDocto = "OTRO";
                    //}


                    //listado de id de firmantes
                    var idsFirma = new List<string>();
                    idsFirma.Add(rubrica.IdentificadorFirma);

                    //generar código QR
                    byte[] QR = _file.CreateQr(string.Concat(url_tramites_en_linea.Valor, "/GPDocumentoVerificacion/Details/", documento.DocumentoId));

                    //si el documento ya tiene folio no solicitarlo nuevamente
                    if (string.IsNullOrWhiteSpace(documento.Folio))
                    {
                        try
                        {
                            //var _folioResponse = _folio.GetFolio(string.Join(", ", email),documento.TipoDocumentoId);
                            //var _folioResponse = _folio.GetFolio(string.Join(", ", email), TipoDocto);
                            var _folioResponse = _folio.GetFolio(string.Join(", ", email), TipoDocto, persona.SubSecretaria);
                            if (_folioResponse == null)
                                response.Errors.Add("Servicio de folio no entregó respuesta");

                            if (_folioResponse != null && _folioResponse.status == "ERROR")
                                response.Errors.Add(_folioResponse.error);

                            documento.Folio = _folioResponse.folio;

                            _repository.Update(documento);
                            _repository.Save();

                            /*Se agregan lo datos del folio al objeto cometido*/
                            if (com != null)
                            {
                                com.Folio = _folioResponse.folio;
                                //com.FechaResolucion = DateTime.Now;
                                com.Firma = "";
                                //if (com.IdEscalafon == 1 && com.IdEscalafon != null)
                                //    com.TipoActoAdministrativo = "Resolución Ministerial Exenta";
                                //else if (com.CalidadDescripcion.Contains("HONORARIOS"))
                                //    com.TipoActoAdministrativo = "Orden de Pago";
                                //else
                                //    com.TipoActoAdministrativo = "Resolución Administrativa Exenta";

                                _repository.Update(com);
                                _repository.Save();
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Errors.Add(ex.Message);
                        }
                    }

                    //var doc = _hsm.Sign(documento.File, rubrica.IdentificadorFirma, rubrica.UnidadOrganizacional, null,null);
                    var docto = _hsm.Sign(documento.File, idsFirma, documento.DocumentoId, documento.Folio, url_tramites_en_linea.Valor, QR);
                    documento.File = docto;
                    documento.Signed = true;

                    _repository.Update(documento);
                    _repository.Save();

                    /*se notifica por correo la firma de la resolucion*/
                    //_email.
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
                if (workflowActual.DefinicionWorkflow.DefinicionProcesoId != (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                    if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any())
                        throw new Exception("Debe adjuntar documentos.");

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
