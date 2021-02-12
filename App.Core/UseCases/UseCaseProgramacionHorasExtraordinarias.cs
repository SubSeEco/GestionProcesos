using System;
using System.Linq;
using App.Model.Core;
using App.Model.ProgramacionHorasExtraordinarias;
using App.Model.Shared;
using App.Model.SIGPER;
using System.Collections.Generic;
using App.Core.Interfaces;
using App.Util;
//using App.Infrastructure.Extensions;

namespace App.Core.UseCases
{
    public class UseCaseProgramacionHorasExtraordinarias
    {
        protected readonly IGestionProcesos _repository;
        protected readonly IHSM _hsm;
        protected readonly ISIGPER _sigper;
        protected readonly IEmail _email;
        protected readonly IFolio _folio;
        protected readonly IFile _file;

        public UseCaseProgramacionHorasExtraordinarias(IGestionProcesos repository)
        {
            _repository = repository;
        }
        public UseCaseProgramacionHorasExtraordinarias(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IHSM hsm, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
            _email = email;
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
        public ResponseMessage ProgramacionHorasExtraordinariasDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<ProgramacionHorasExtraordinarias>(id);
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

            var doc = _repository.Get<Documento>(c => c.ProcesoId == memo.ProcesoId).FirstOrDefault();

            //generar código QR
            var qr = _file.CreateQR(string.Concat(url_tramites_en_linea.Valor, "/GPDocumentoVerificacion/Details/", doc.DocumentoId));


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
            var persona = new SIGPER();

            try
            {
                var documento = _repository.GetById<Documento>(obj.DocumentoId);
                if (documento == null)
                    response.Errors.Add("Documento no encontrado");

                if (obj.Signed == true)
                    response.Errors.Add("Documento ya se encuentra firmado");

                var rubrica = _repository.GetFirst<Rubrica>(q => q.Email == email && q.HabilitadoFirma);
                /*old firma*/
                //var rubrica = _repository.Get<Rubrica>(q => q.Email == email && q.HabilitadoFirma == true);
                //string IdentificadorFirma = string.Empty;
                //bool habilitado = false;
                //foreach (var fir in rubrica)
                //{
                //    if (fir == null)
                //        response.Errors.Add("Usuario sin información de firma electrónica");
                //    if (fir != null && string.IsNullOrWhiteSpace(fir.IdentificadorFirma))
                //        response.Errors.Add("Usuario no tiene identificador de firma electrónica");

                //    if (documento.Proceso.DefinicionProcesoId == int.Parse(fir.IdProceso))
                //    {
                //        habilitado = true;
                //        IdentificadorFirma = fir.IdentificadorFirma;
                //    }

                //    if (fir.HabilitadoFirma != true)
                //        response.Errors.Add("Usuario no se encuentra habilitado para firmar");
                //}
                /**/

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

                var url_tramites_en_linea = _repository.GetFirst<Configuracion>(q => q.Nombre == nameof(Util.Enum.Configuracion.url_tramites_en_linea));
                if (url_tramites_en_linea == null)
                    response.Errors.Add("No se encontró la configuración de la url de verificación de documentos");
                if (url_tramites_en_linea != null && url_tramites_en_linea.Valor.IsNullOrWhiteSpace())
                    response.Errors.Add("No se encontró la configuración de la url de verificación de documentos");


                if (response.IsValid)
                {
                    //documento.File = _hsm.Sign(documento, rubrica, HSMUser, HSMPassword);
                    //documento.File = _hsm.Sign(documento);
                    //documento.Signed = true;

                    //_repository.Update(documento);
                    //_repository.Save();

                    /*se buscar la persona para determinar la subsecretaria*/
                    if (!string.IsNullOrEmpty(email))
                    {
                        persona = _sigper.GetUserByEmail(email);
                        if (persona == null)
                            response.Errors.Add("No se encontró usuario firmante en sistema SIGPER");

                        if (persona != null && string.IsNullOrWhiteSpace(persona.SubSecretaria))
                            response.Errors.Add("No se encontró la subsecretaría del firmante");
                    }

                    /*Se busca cometido para determinar tipo de documento*/
                    string TipoDocto;
                    var com = _repository.Get<ProgramacionHorasExtraordinarias>(c => c.ProgramacionHorasExtraordinariasId == programacionHorasExtraordinariasId.Value).FirstOrDefault();

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
                    byte[] QR = _file.CreateQR(string.Concat(url_tramites_en_linea.Valor, "/GPDocumentoVerificacion/Details/", documento.DocumentoId));

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
                if (workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar && (obj.TipoAprobacionId == null || obj.TipoAprobacionId == 0))
                    workflowActual.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.Aprobada;
                if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                {
                    //if (workflowActual.DefinicionWorkflow.Secuencia == 3)
                    //{
                    //    if (obj.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                    //    {
                    //        //Se toma valor del pasaje para validar cuantos adjuntos se solicitan
                    //        var Pasaje = new Pasaje();
                    //        Pasaje = _repository.Get<Pasaje>(q => q.WorkflowId == obj.WorkflowId).FirstOrDefault();
                    //        if (Pasaje != null)
                    //        {
                    //            var cotizacion = _repository.Get<Cotizacion>(c => c.PasajeId == Pasaje.PasajeId).LastOrDefault();
                    //            if (cotizacion != null)
                    //            {
                    //                if (Pasaje.TipoDestino == true)
                    //                {
                    //                    if (cotizacion.CotizacionDocumento.Count < 1)
                    //                    {
                    //                        throw new Exception("Debe adjuntar a los menos una cotizaciones.");
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    if (cotizacion.CotizacionDocumento.Count < 3)
                    //                    {
                    //                        throw new Exception("Debe adjuntar a los menos tres cotizaciones.");
                    //                    }
                    //                }
                    //            }
                    //            else
                    //                throw new Exception("No se ha ingresado cotización.");
                    //        }
                    //    }
                    //}
                    //else if (workflowActual.DefinicionWorkflow.Secuencia == 16)
                    //{
                    //    if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any(c => c.TipoDocumentoId.Value == 4 && c.TipoDocumentoId != null))
                    //        throw new Exception("Debe adjuntar documentos en la tarea de analista de contabilidad.");
                    //}
                    //else if (workflowActual.DefinicionWorkflow.Secuencia == 18)
                    //{
                    //    if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any(c => c.TipoDocumentoId.Value == 5 && c.TipoDocumentoId != null))
                    //        throw new Exception("Debe adjuntar documentos en la tarea de analista tesoreria.");
                    //}
                }
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
                    workflowActual.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.Aprobada;

                //determinar siguiente tarea en base a estado y definicion de proceso
                DefinicionWorkflow definicionWorkflow = null;

                //si permite multiple evaluacion generar la misma tarea
                if (workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                    definicionWorkflow = _repository.GetById<DefinicionWorkflow>(workflowActual.DefinicionWorkflowId);

                //if (workflowActual.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.HorasCompensatorias.ToString())
                //{
                //    //Se toma valor de cometidos para definir curso de accion del flujo
                //    var Memorandum = new HorasCompensatorias();
                //    Memorandum = _repository.Get<HorasCompensatorias>(q => q.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //    if (Memorandum != null)
                //    {
                //        //determinar siguiente tarea desde el diseño de proceso
                //        if (!workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                //            //if (workflowActual.DefinicionWorkflow.Secuencia == 1)
                //            //{
                //            //    //if (workflowActual.TipoAprobacionId == (int)Enum.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso && Memorandum.To == null)
                //            //    if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso && Memorandum.EmailVisa1 == null)
                //            //    {
                //            //        //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 7);
                //            //        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //            //    }
                //            //    else
                //            //    {
                //            //        /*buscar el objeto de negocio condsulta integridad*/
                //            //        var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //            //        var pro = con.ProcesoId;

                //            //        //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //            //        var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //            //        var correo = con.EmailVisa1;
                //            //        var unidaddes = con.UnidadDescripcionVisa1;
                //            //        var unidadcod = con.IdUnidadVisa1;

                //            //        //var correo = work.To;
                //            //        workflowActual.Email = correo;
                //            //        obj.To = correo;

                //            //        workflowActual.Pl_UndDes = unidaddes;
                //            //        obj.Pl_UndDes = unidaddes;

                //            //        workflowActual.Pl_UndCod = unidadcod;
                //            //        obj.Pl_UndCod = unidadcod;

                //            //        //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //            //        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //            //    }
                //            //}
                //            if (workflowActual.DefinicionWorkflow.Secuencia == 1)
                //            {
                //                //if (workflowActual.TipoAprobacionId == (int)Enum.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso && Memorandum.To == null)
                //                //if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso && Memorandum.EmailVisa1 != null)
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso && Memorandum.EmailVisa1 == null)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailRem;
                //                    var unidaddes = con.UnidadDescripcion;
                //                    var unidadcod = con.IdUnidad;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                }
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailVisa1;
                //                    var unidaddes = con.UnidadDescripcionVisa1;
                //                    var unidadcod = con.IdUnidadVisa1;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 2)
                //            {
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa2 == null)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailRem;
                //                    var unidaddes = con.UnidadDescripcion;
                //                    var unidadcod = con.IdUnidad;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                }
                //                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //                {
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                //                }
                //                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                //                {
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 4);
                //                }
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailVisa2;
                //                    var unidaddes = con.UnidadDescripcionVisa2;
                //                    var unidadcod = con.IdUnidadVisa2;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 3)
                //            {
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa3 == null)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailRem;
                //                    var unidaddes = con.UnidadDescripcion;
                //                    var unidadcod = con.IdUnidad;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                }
                //                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //                {
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                //                }
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailVisa3;
                //                    var unidaddes = con.UnidadDescripcionVisa3;
                //                    var unidadcod = con.IdUnidadVisa3;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 4)
                //            {
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa4 == null)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailRem;
                //                    var unidaddes = con.UnidadDescripcion;
                //                    var unidadcod = con.IdUnidad;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                }
                //                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //                {
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                //                }
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailVisa4;
                //                    var unidaddes = con.UnidadDescripcionVisa4;
                //                    var unidadcod = con.IdUnidadVisa4;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 5)
                //            {
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa5 == null)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailRem;
                //                    var unidaddes = con.UnidadDescripcion;
                //                    var unidadcod = con.IdUnidad;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                }
                //                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //                {
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                //                }
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailVisa5;
                //                    var unidaddes = con.UnidadDescripcionVisa5;
                //                    var unidadcod = con.IdUnidadVisa5;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 6)
                //            {
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa6 == null)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailRem;
                //                    var unidaddes = con.UnidadDescripcion;
                //                    var unidadcod = con.IdUnidad;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                }
                //                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //                {
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                //                }
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailVisa6;
                //                    var unidaddes = con.UnidadDescripcionVisa6;
                //                    var unidadcod = con.IdUnidadVisa6;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 7)
                //            {
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa7 == null)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailRem;
                //                    var unidaddes = con.UnidadDescripcion;
                //                    var unidadcod = con.IdUnidad;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                }
                //                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //                {
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                //                }
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailVisa7;
                //                    var unidaddes = con.UnidadDescripcionVisa7;
                //                    var unidadcod = con.IdUnidadVisa7;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 8)
                //            {
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa8 == null)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailRem;
                //                    var unidaddes = con.UnidadDescripcion;
                //                    var unidadcod = con.IdUnidad;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                }
                //                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //                {
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                //                }
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailVisa8;
                //                    var unidaddes = con.UnidadDescripcionVisa8;
                //                    var unidadcod = con.IdUnidadVisa8;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 9)
                //            {
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa9 == null)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailRem;
                //                    var unidaddes = con.UnidadDescripcion;
                //                    var unidadcod = con.IdUnidad;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                }
                //                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //                {
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                //                }
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailVisa9;
                //                    var unidaddes = con.UnidadDescripcionVisa9;
                //                    var unidadcod = con.IdUnidadVisa9;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 10)
                //            {
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa10 == null)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailRem;
                //                    var unidaddes = con.UnidadDescripcion;
                //                    var unidadcod = con.IdUnidad;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                }
                //                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //                {
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                //                }
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailVisa10;
                //                    var unidaddes = con.UnidadDescripcionVisa10;
                //                    var unidadcod = con.IdUnidadVisa10;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 11)
                //            {
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailAutorizaFirma1 == null)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailAutorizaFirma2;
                //                    var unidaddes = con.UnidadDescripcionAutorizaFirma2;
                //                    var unidadcod = con.IdUnidadAutorizaFirma2;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 13);
                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                }
                //                //else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //                //{
                //                //    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                //                //}
                //                else
                //                {
                //                    ///*buscar el objeto de negocio condsulta integridad*/
                //                    //var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    //var pro = con.ProcesoId;

                //                    ////var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    //var correo = con.EmailAutorizaFirma1;
                //                    //var unidaddes = con.UnidadDescripcionAutorizaFirma1;
                //                    //var unidadcod = con.IdUnidadAutorizaFirma1;

                //                    ////var correo = work.To;
                //                    //workflowActual.Email = correo;
                //                    //obj.To = correo;

                //                    //workflowActual.Pl_UndDes = unidaddes;
                //                    //obj.Pl_UndDes = unidaddes;

                //                    //workflowActual.Pl_UndCod = unidadcod;
                //                    //obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 12)
                //            {
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailAutorizaFirma2 == null)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailAutorizaFirma3;
                //                    var unidaddes = con.UnidadDescripcionAutorizaFirma3;
                //                    var unidadcod = con.IdUnidadAutorizaFirma3;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 14);
                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                }
                //                //else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //                //{
                //                //    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                //                //}
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailAutorizaFirma2;
                //                    var unidaddes = con.UnidadDescripcionAutorizaFirma2;
                //                    var unidadcod = con.IdUnidadAutorizaFirma3;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 13)
                //            {
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailAutorizaFirma3 == null)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailRem;
                //                    var unidaddes = con.UnidadDescripcion;
                //                    var unidadcod = con.IdUnidad;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 15);
                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                }
                //                //else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //                //{
                //                //    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                //                //}
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailAutorizaFirma3;
                //                    var unidaddes = con.UnidadDescripcionAutorizaFirma3;
                //                    var unidadcod = con.IdUnidadAutorizaFirma3;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 14)
                //            {
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailRem;
                //                    var unidaddes = con.UnidadDescripcion;
                //                    var unidadcod = con.IdUnidad;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailRem;
                //                    var unidaddes = con.UnidadDescripcion;
                //                    var unidadcod = con.IdUnidad;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 15)
                //            {
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //                {
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                //                }
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailSecre;
                //                    var unidaddes = con.UnidadDescripcionSecre;
                //                    var unidadcod = con.IdUnidadSecre;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //            else if (workflowActual.DefinicionWorkflow.Secuencia == 16)
                //            {
                //                //if (workflowActual.TipoAprobacionId == (int)Enum.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso && Memorandum.To == null)
                //                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo)
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailDest;
                //                    var unidaddes = con.UnidadDescripcionDest;
                //                    var unidadcod = con.IdUnidadDest;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //                else
                //                {
                //                    /*buscar el objeto de negocio condsulta integridad*/
                //                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //                    var pro = con.ProcesoId;

                //                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //                    var correo = con.EmailDest;
                //                    var unidaddes = con.UnidadDescripcionDest;
                //                    var unidadcod = con.IdUnidadDest;

                //                    //var correo = work.To;
                //                    workflowActual.Email = correo;
                //                    obj.To = correo;

                //                    workflowActual.Pl_UndDes = unidaddes;
                //                    obj.Pl_UndDes = unidaddes;

                //                    workflowActual.Pl_UndCod = unidadcod;
                //                    obj.Pl_UndCod = unidadcod;

                //                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //                }
                //            }
                //        //else if (workflowActual.DefinicionWorkflow.Secuencia == 17)
                //        //{
                //        //    if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //        //    {
                //        //        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 11);
                //        //        //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //        //    }
                //        //    else
                //        //    {
                //        //        ///*buscar el objeto de negocio condsulta integridad*/
                //        //        //var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //        //        //var pro = con.ProcesoId;

                //        //        ////var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //        //        //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                //        //        //var correo = con.EmailAna;
                //        //        //var unidaddes = con.UnidadDescripcionAna;
                //        //        //var unidadcod = con.IdUnidadAna;

                //        //        ////var correo = work.To;
                //        //        //workflowActual.Email = correo;
                //        //        //obj.To = correo;

                //        //        //workflowActual.Pl_UndDes = unidaddes;
                //        //        //obj.Pl_UndDes = unidaddes;

                //        //        //workflowActual.Pl_UndCod = unidadcod;
                //        //        //obj.Pl_UndCod = unidadcod;

                //        //        //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                //        //        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                //        //    }
                //        //}
                //    }
                //}
                else
                {
                    if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                    {
                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                    }
                    else
                    {
                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);
                    }
                }

                //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);

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
                    workflow.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.SinAprobacion;
                    workflow.Terminada = false;
                    workflow.DefinicionWorkflow = definicionWorkflow;
                    workflow.ProcesoId = workflowActual.ProcesoId;
                    workflow.Mensaje = obj.Observacion;
                    workflow.TareaPersonal = false;

                    ///*Si el proceso corresponde a Cometidos y esta en la tarea de pago tesoreria se notifica con correo a quien viaja*/
                    //if (workflow.DefinicionWorkflow.Entidad.Codigo == (int)App.Util.Enum.Entidad.Cometido.ToString())
                    //{
                    //    if (workflow.DefinicionWorkflow.Secuencia == 15)
                    //    {
                    //        workflowActual.Proceso.Terminada = true;
                    //        workflowActual.Proceso.FechaTermino = DateTime.Now;
                    //        _repository.Save();
                    //    }
                    //}

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo)
                    {
                        if (obj.Pl_UndCod.HasValue)
                        {
                            var unidad = _sigper.GetUnidad(obj.Pl_UndCod.Value);
                            if (unidad == null)
                                throw new Exception("No se encontró la unidad en SIGPER.");

                            workflow.Pl_UndCod = unidad.Pl_UndCod;
                            workflow.Pl_UndDes = unidad.Pl_UndDes;
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

                    //if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaJefaturaDeFuncionarioQueViaja)
                    //{
                    //    if (workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.Cometido.ToString() || workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                    //    {
                    //        var com = _repository.Get<Cometido>(c => c.ProcesoId == workflow.ProcesoId);
                    //        persona = _sigper.GetUserByRut(com.FirstOrDefault().Rut);
                    //        if (persona == null)
                    //            throw new Exception("No se encontró el usuario en SIGPER.");
                    //        workflow.Email = persona.Jefatura.Rh_Mail.Trim();
                    //        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                    //        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                    //        workflow.TareaPersonal = true;
                    //    }
                    //}

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico)
                    {

                        workflow.GrupoId = definicionWorkflow.GrupoId;
                        workflow.Pl_UndCod = definicionWorkflow.Pl_UndCod;
                        workflow.Pl_UndDes = definicionWorkflow.Pl_UndDes;
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

                    ////notificar actualización del estado al dueño
                    //if (workflowActual.DefinicionWorkflow.NotificarAlAutor)
                    //    _email.NotificarCambioWorkflow(workflowActual,
                    //    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaCorreoCambioEstado),
                    //    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacionTarea));

                    ////notificar por email al ejecutor de proxima tarea
                    //if (workflow.DefinicionWorkflow.NotificarAsignacion)
                    //    _email.NotificarCambioWorkflow(workflow,
                    //    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaCorreoNotificacionTarea),
                    //    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacionTarea));

                    /*Si el proceso corresponde a Cometidos y esta en la tarea de firma electronica se notifica con correo*/
                    //if (workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.Memorandum.ToString())
                    //{
                    //    if (workflowActual.DefinicionWorkflow.Secuencia == 12)
                    //    {
                    //        Memorandum memo = _repository.Get<Memorandum>(c => c.ProcesoId == workflow.ProcesoId).FirstOrDefault();

                    //        Documento doc = memo.Proceso.Documentos.Where(d => d.ProcesoId == memo.ProcesoId && d.TipoDocumentoId == 8).FirstOrDefault();

                    //        //Memorandum memo1 = new Memorandum();
                    //        //var list1 = memo1.ListaChk1.Trim();

                    //        List<string> emailMsg = new List<string>();
                    //        //emailMsg.Add("ereyes@economia.cl");
                    //        //emailMsg.Add("acifuentes@economia.cl"); //oficia de partes
                    //        //emailMsg.Add("scid@economia.cl"); //oficia de partes
                    //        //emailMsg.Add(persona.Funcionario.Rh_Mail.Trim()); // interesado

                    //        emailMsg.Add(memo.EmailRem.Trim()); // interesado
                    //        emailMsg.Add(memo.EmailDest.Trim());

                    //        if (string.IsNullOrEmpty(memo.EmailVisa1))
                    //            emailMsg.Add(persona.Funcionario.Rh_Mail.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk1))
                    //            emailMsg.Add(memo.ListaChk1.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk2))
                    //            emailMsg.Add(memo.ListaChk2.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk3))
                    //            emailMsg.Add(memo.ListaChk3.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk4))
                    //            emailMsg.Add(memo.ListaChk4.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk5))
                    //            emailMsg.Add(memo.ListaChk5.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk6))
                    //            emailMsg.Add(memo.ListaChk6.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk7))
                    //            emailMsg.Add(memo.ListaChk7.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk8))
                    //            emailMsg.Add(memo.ListaChk8.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk9))
                    //            emailMsg.Add(memo.ListaChk9.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk10))
                    //            emailMsg.Add(memo.ListaChk10.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk11))
                    //            emailMsg.Add(memo.ListaChk11.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk12))
                    //            emailMsg.Add(memo.ListaChk12.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk13))
                    //            emailMsg.Add(memo.ListaChk13.Trim()); // 

                    //        if (!string.IsNullOrEmpty(memo.ListaChk14))
                    //            emailMsg.Add(memo.ListaChk14.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk15))
                    //            emailMsg.Add(memo.ListaChk15.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk16))
                    //            emailMsg.Add(memo.ListaChk16.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk17))
                    //            emailMsg.Add(memo.ListaChk17.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk18))
                    //            emailMsg.Add(memo.ListaChk18.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk19))
                    //            emailMsg.Add(memo.ListaChk19.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk20))
                    //            emailMsg.Add(memo.ListaChk20.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk21))
                    //            emailMsg.Add(memo.ListaChk21.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk22))
                    //            emailMsg.Add(memo.ListaChk22.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk23))
                    //            emailMsg.Add(memo.ListaChk23.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk24))
                    //            emailMsg.Add(memo.ListaChk24.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk25))
                    //            emailMsg.Add(memo.ListaChk25.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk26))
                    //            emailMsg.Add(memo.ListaChk26.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk27))
                    //            emailMsg.Add(memo.ListaChk27.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk28))
                    //            emailMsg.Add(memo.ListaChk28.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk29))
                    //            emailMsg.Add(memo.ListaChk29.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk30))
                    //            emailMsg.Add(memo.ListaChk30.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk31))
                    //            emailMsg.Add(memo.ListaChk31.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk32))
                    //            emailMsg.Add(memo.ListaChk32.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk33))
                    //            emailMsg.Add(memo.ListaChk33.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk34))
                    //            emailMsg.Add(memo.ListaChk34.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk35))
                    //            emailMsg.Add(memo.ListaChk35.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk36))
                    //            emailMsg.Add(memo.ListaChk36.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk37))
                    //            emailMsg.Add(memo.ListaChk37.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk38))
                    //            emailMsg.Add(memo.ListaChk38.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk39))
                    //            emailMsg.Add(memo.ListaChk39.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk40))
                    //            emailMsg.Add(memo.ListaChk40.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk41))
                    //            emailMsg.Add(memo.ListaChk41.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk42))
                    //            emailMsg.Add(memo.ListaChk42.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk43))
                    //            emailMsg.Add(memo.ListaChk43.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk44))
                    //            emailMsg.Add(memo.ListaChk44.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk45))
                    //            emailMsg.Add(memo.ListaChk45.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk46))
                    //            emailMsg.Add(memo.ListaChk46.Trim()); // interesado

                    //        if (!string.IsNullOrEmpty(memo.ListaChk47))
                    //            emailMsg.Add(memo.ListaChk47.Trim()); // interesado

                    //        //if (!string.IsNullOrEmpty(memo.EmailSecre))
                    //        //    emailMsg.Add(memo.EmailSecre.Trim()); // interesado

                    //        _email.NotificacionesMemorandum(workflow,
                    //        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaMemoFirmado), "Envio Memo", emailMsg, memo.MemorandumId, doc);

                    //    }
                    //}

                    /*Si el proceso corresponde a Cometidos y esta en la tarea de pago tesoreria se notifica con correo a quien viaja*/
                    //if (workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.Cometido.ToString())
                    //{
                    //    if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                    //    {
                    //        if (workflow.DefinicionWorkflow.Secuencia == 20)
                    //        {
                    //            List<string> emailMsg = new List<string>();
                    //            emailMsg.Add("mmontoya@economia.cl");
                    //            emailMsg.Add(persona.Funcionario.Rh_Mail.Trim()); // interesado quien viaja

                    //            _email.NotificarFirmaResolucionCometido(workflow,
                    //            _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaNotificacionPago),
                    //            _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacionTarea), emailMsg);
                    //        }
                    //    }
                    //    else if (workflow.DefinicionWorkflow.Secuencia == 13 || workflow.DefinicionWorkflow.Secuencia == 14 || workflow.DefinicionWorkflow.Secuencia == 15)
                    //    {
                    //        List<string> emailMsg = new List<string>();
                    //        emailMsg.Add("mmontoya@economia.cl");
                    //        emailMsg.Add(persona.Funcionario.Rh_Mail.Trim()); // interesado quien viaja

                    //        _email.NotificarFirmaResolucionCometido(workflow,
                    //        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaNotificacionPago),
                    //        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacionTarea), emailMsg);
                    //    }
                    //}
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
