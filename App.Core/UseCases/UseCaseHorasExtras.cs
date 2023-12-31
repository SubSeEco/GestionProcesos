﻿using System;
using System.Linq;
using App.Model.Core;
using App.Model.Sigper;
using System.Collections.Generic;
using App.Core.Interfaces;
using App.Util;
using App.Model.HorasExtras;

namespace App.Core.UseCases
{
    public class UseCaseHorasExtras
    {
        private readonly IGestionProcesos _repository;
        private readonly IHsm _hsm;
        private readonly ISigper _sigper;
        private readonly IEmail _email;
        private readonly IFolio _folio;
        private readonly IFile _file;
        //private IGestionProcesos repository;
        //private ISIGPER sigper;

        public UseCaseHorasExtras(IGestionProcesos repository)
        {
            _repository = repository;
        }
        public UseCaseHorasExtras(IGestionProcesos repository, ISigper sigper, IFile file, IFolio folio, IHsm hsm, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
            _email = email;
        }

        public UseCaseHorasExtras(IGestionProcesos repository, ISigper sigper)
        {
            _repository = repository;
            _sigper = sigper;
        }

        public ResponseMessage HorasExtrasInsert(HorasExtras obj)
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
        public ResponseMessage HorasExtrasUpdate(HorasExtras obj)
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
        //public ResponseMessage HorasExtrasDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<HorasExtras>(id);
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


        public ResponseMessage ColaboradorInsert(Colaborador obj)
        
        {
            var response = new ResponseMessage();

            try
            {
                if (!obj.NombreId.HasValue)
                    response.Errors.Add("No se ha señalado el nombre del funcionario que solicita las horas extras .");

                /*se valida q no se ingrese mas de una vez un mismo funcionario*/
                var col = _repository.Get<Colaborador>(c => c.HorasExtrasId == obj.HorasExtrasId).ToList();
                foreach(var c in col)
                {
                    if(c.NombreId == obj.NombreId)
                        response.Errors.Add("Ya se han ingresado solicitudes para el funcionario señalado");
                }

                if (response.IsValid)
                {
                    if (obj.NombreId.HasValue)
                    {
                        var nombre = _sigper.GetUserByRut(obj.NombreId.Value).Funcionario;
                        obj.Nombre = nombre.PeDatPerChq.Trim();
                        obj.DV = nombre.RH_DvNuInt.Trim();

                        _repository.Create(obj);
                        _repository.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage ColaboradorUpdate(Colaborador obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (response.IsValid)
                {
                    if (obj.NombreId.HasValue)
                    {
                        var nombre = _sigper.GetUserByRut(obj.NombreId.Value).Funcionario;
                        obj.Nombre = nombre.PeDatPerChq.Trim();
                        obj.DV = nombre.RH_DvNuInt.Trim();

                        _repository.Update(obj);
                        _repository.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage ColaboradorDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Colaborador>(id);
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

        public ResponseMessage SignReso(Documento obj, string email, int? HorasExtrasId)
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
                            response.Errors.Add("No se encontró usuario firmante en sistema Sigper");

                        if (persona != null && string.IsNullOrWhiteSpace(persona.SubSecretaria))
                            response.Errors.Add("No se encontró la subsecretaría del firmante");
                    }

                    /*Se busca proceso para determinar tipo de documento*/
                    string TipoDocto;
                    var horas = _repository.GetFirst<HorasExtras>(c => c.HorasExtrasId == HorasExtrasId.Value);

                    if (!string.IsNullOrEmpty(obj.TipoDocumentoFirma))
                        TipoDocto = obj.TipoDocumentoFirma;
                    else
                        TipoDocto = "OTRO";

                    //listado de id de firmantes
                    var idsFirma = new List<string>();
                    idsFirma.Add(rubrica.IdentificadorFirma);

                    //generar código QR
                    byte[] qr = _file.CreateQr(string.Concat(url_tramites_en_linea.Valor, "/GPDocumentoVerificacion/Details/", documento.DocumentoId));

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

                            /*Se marca programacion de horas como aprobadas*/
                            if (horas != null)
                            {
                                horas.Aprobado = true;

                                _repository.Update(horas);
                                _repository.Save();
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Errors.Add(ex.Message);
                        }
                    }

                    //var doc = _hsm.Sign(documento.File, rubrica.IdentificadorFirma, rubrica.UnidadOrganizacional, null,null);
                    var docto = _hsm.SignWSDL(documento.File, idsFirma, documento.DocumentoId, documento.Folio, url_tramites_en_linea.Valor, qr);
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
                else
                {
                    if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any())
                        throw new Exception("Debe adjuntar documentos.");
                }

                /*VALIDACIONES Y REGLAS DE NEGOCIO*/
                if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.ProgramacionHorasExtraordinarias)
                {
                    if (obj.TipoAprobacionId != (int)Util.Enum.TipoAprobacion.Rechazada)
                    {
                        if (workflowActual.DefinicionWorkflow.Secuencia == 12)/*Valida la creacion de adjuntos segun el tipo de proceso*/
                        {
                            if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.Proceso != null)
                            {
                                if (!workflowActual.Proceso.Documentos.Any(c => c.TipoDocumentoId.Value == 13))
                                {
                                    throw new Exception("Se debe generar la Resolución de Confirmacion de Horas Extraordinarias Pagadas.");
                                }
                                else if (!workflowActual.Proceso.Documentos.Any(c => c.TipoDocumentoId.Value == 14))
                                {
                                    throw new Exception("Se debe generar la Resolución de Confirmacion de Horas Extraordinarias Compensadas.");
                                }
                            }
                        }
                        else if (workflowActual.DefinicionWorkflow.Secuencia == 4)
                        {
                            if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.Proceso != null)
                            {
                                if (!workflowActual.Proceso.Documentos.Any(c => c.TipoDocumentoId.Value == 9))
                                    throw new Exception("Se debe generar la resolución correspondiente a la programación de horas extraordinarias.");
                            }   
                        }
                        else if (workflowActual.DefinicionWorkflow.Secuencia == 7)/*Se valida q la resolucion de programacion se encuentre firmada*/
                        {
                            var doc = _repository.GetById<Documento>(workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == 9).FirstOrDefault().DocumentoId).Signed;
                            if (doc == false)
                                throw new Exception("La Resolción de programación de horas extraordinarias, debe estar firmado electronicamente");
                        }
                        //else if (workflowActual.DefinicionWorkflow.Secuencia == 8)/*Se valida que se suba reporte de asistencia*/
                        //{
                        //    //var doc = _repository.GetById<Documento>(workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == 9).FirstOrDefault().DocumentoId).Signed;
                        //    //if (doc == false)
                        //    //    throw new Exception("La Resolción de programación de horas extraordinarias, debe estar firmado electronicamente");
                        //}
                        else if (workflowActual.DefinicionWorkflow.Secuencia == 18)/*Se valida q la resolucion de horas pagadas se encuentre firmada*/
                        {
                            var doc = _repository.GetById<Documento>(workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == 13).FirstOrDefault().DocumentoId).Signed;
                            if (doc == false)
                                throw new Exception("La Resolción de programación de horas extraordinarias, debe estar firmado electronicamente");
                        }
                        else if (workflowActual.DefinicionWorkflow.Secuencia == 15)/*Se valida q la resolucion de horas compensadas se encuentre firmada*/
                        {
                            var doc = _repository.GetById<Documento>(workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == 14).FirstOrDefault().DocumentoId).Signed;
                            if (doc == false)
                                throw new Exception("La Resolción de programación de horas extraordinarias, debe estar firmado electronicamente");
                        }
                    }
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

                //DETERMINAR SIGUIENTE TAREA EN BASE A ESTADO Y DEFINICION DE PROCESO
                DefinicionWorkflow definicionWorkflow = null;

                /*Se toman los valores de la solicitud, para definir curso de las sgtes tareas*/
                var Horas = _repository.GetFirst<HorasExtras>(h => h.WorkflowId == obj.WorkflowId);

                //si permite multiple evaluacion generar la misma tarea
                if (workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                    definicionWorkflow = _repository.GetById<DefinicionWorkflow>(workflowActual.DefinicionWorkflowId);
                else
                {
                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                    {
                        if (workflowActual.DefinicionWorkflow.Secuencia == 7 && Horas.Aprobado != true) /*Si esta en la tarea de firma programacion y no se aprueba se termina la tramitacion*/
                        {
                            definicionWorkflow = null;
                        }
                        else if (workflowActual.DefinicionWorkflow.Secuencia == 15)
                        {
                            definicionWorkflow = null;
                        }
                        else
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
                    workflowActual.Proceso.EstadoProcesoId = (int)Util.Enum.EstadoProceso.Terminado;
                    //workflowActual.Proceso.Terminada = true;
                    workflowActual.Proceso.FechaTermino = DateTime.Now;
                    _repository.Save();

                    /*notificar al dueño del proceso y oficina de partes*/
                    if (workflowActual.DefinicionWorkflow.Secuencia == 7)
                    {
                        /*si no existen mas tareas se envia correo de notificacion*/
                        //var hrs = _repository.GetFirst<HorasExtras>(c => c.ProcesoId == workflowActual.ProcesoId);
                        /*se trae documento para adjuntar*/
                        //Documento doc = hrs.Proceso.Documentos.FirstOrDefault(d => d.ProcesoId == hrs.ProcesoId && d.TipoDocumentoId == 9);
                        //var solicitante = _repository.Get<Workflow>(c => c.ProcesoId == workflowActual.ProcesoId && c.DefinicionWorkflow.Secuencia == 1).FirstOrDefault().Email;
                        //var jefe = _sigper.GetUserByRut(hrs.jefaturaId.Value).Funcionario.Rh_Mail.Trim();
                        //List<string> emailMsg;

                        _email.NotificarFinProceso(workflowActual.Proceso,
                        _repository.GetFirst<Configuracion>(q => q.Nombre == nameof(Util.Enum.Configuracion.plantilla_fin_proceso)),
                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.AsuntoCorreoNotificacion));

                        //_email.NotificacionesCometido(workflowActual,
                        //_repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaEncargadoDeptoAdmin_OfPartes), /*notificacion a oficia de partes*/
                        //"Se ha tramitado un cometido nacional",
                        //emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                        //_repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.UrlSistema).Valor,
                        //doc, cometido.Folio, cometido.FechaResolucion.ToString(), cometido.TipoActoAdministrativo);
                    }
                }

                //en el caso de existir mas tareas, crearla
                if (definicionWorkflow != null)
                {
                    var workflow = new Workflow();
                    /*Si la ultima tarea es de firma electronica de programacion la sgte se creara para ser ejecutada el mes sgte.*/
                    if (workflowActual.DefinicionWorkflow.Secuencia == 7)
                    {
                        string mes = (Horas.MesBaseCalculo + 1).ToString().Length != 1 ? (Horas.MesBaseCalculo + 1).ToString() : "0" + (Horas.MesBaseCalculo + 1);
                        string year = Horas.Annio;

                        if(Horas.MesBaseCalculo == 12)
                        {
                            mes = "01";
                            year = (int.Parse(Horas.Annio) + 1).ToString(); 
                        }

                        var newDate = "10" + "-" + mes + "-" + year;
                        workflow.FechaCreacion = Convert.ToDateTime(newDate);
                    }
                    else
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

                    #region NOTIFICACIONES DE CORREO PROCESO HORAS EXTRAS

                    if (workflow.Proceso.DefinicionProceso.Entidad.Codigo == Util.Enum.Entidad.HorasExtras.ToString())
                    {
                        var hrs = _repository.GetFirst<HorasExtras>(c => c.ProcesoId == workflow.ProcesoId);
                        //var solicitante = _repository.Get<Workflow>(c => c.ProcesoId == workflow.ProcesoId && c.DefinicionWorkflow.Secuencia == 1).FirstOrDefault().Email;
                        //var QuienViaja = _sigper.GetUserByRut(hrs.NombreId.Value).Funcionario.Rh_Mail.Trim();
                        //string jefe = string.Empty;
                        //if (_sigper.GetUserByEmail(QuienViaja).Jefatura != null)
                        //    jefe = _sigper.GetUserByEmail(QuienViaja).Jefatura.Rh_Mail.Trim();
                        //else
                        //    jefe = "mmontoya@economia.cl";

                        List<string> emailMsg;
                        switch (workflowActual.DefinicionWorkflow.Secuencia)
                        {
                            case 1: break;
                            case 2: break;
                            case 3: /*3.- Recepciona y Analiza Programación de Horas Extraordinarias (Jefatura Departamento Administrativo)*/
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                {
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 4 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Analista Gestion Personas

                                    _email.NotificacionesHorasExtras(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaHorasExtras),
                                    "Tiene una solicitud de programación de horas extraordinarias N°: " + hrs.HorasExtrasId + " " + "para aprobación",
                                    emailMsg, hrs.HorasExtrasId, hrs.FechaSolicitud.ToShortDateString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }
                                break;
                            case 4: /*4.- Recepción Programación de Horas Extraordinarias (Analista Área Personas)*/
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                {
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 5 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Encargada Area Personas

                                    _email.NotificacionesHorasExtras(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaHorasExtras),
                                    "Tiene una solicitud de programación de horas extraordinarias N°: " + hrs.HorasExtrasId + " " + "para aprobación",
                                    emailMsg, hrs.HorasExtrasId, hrs.FechaSolicitud.ToShortDateString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }
                                break;
                            case 5:/*5.- Visar Resolución Programación de Horas Extraordinarias (Encargado(a) Área de Personas)*/
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                {
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 6 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Encargado Unidad GP

                                    _email.NotificacionesHorasExtras(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaHorasExtras),
                                    "Tiene una solicitud de programación de horas extraordinarias N°: " + hrs.HorasExtrasId + " " + "para aprobación",
                                    emailMsg, hrs.HorasExtrasId, hrs.FechaSolicitud.ToShortDateString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }
                                break;
                            case 6: /*6.- Visar Resolución Programación de Horas Extraordinarias (Encargado(a) Unidad de Gestión y Desarrollo de Personas)*/
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                {
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 7 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Encargado Depto Administrativo

                                    _email.NotificacionesHorasExtras(workflowActual,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaHorasExtras),
                                    "Tiene una solicitud de programación de horas extraordinarias N°: " + hrs.HorasExtrasId + " " + "para aprobación",
                                    emailMsg, hrs.HorasExtrasId, hrs.FechaSolicitud.ToShortDateString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }
                                break;
                            case 7: /*7.- Firma Resolución Programación de Horas Extraordinarias (Jefatura Departamento Administrativo)*/
                                //if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                                if (workflowActual.TipoAprobacionId != (int)Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 8 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Secretaria

                                    _email.NotificacionesHorasExtras(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaHorasExtras),
                                    "Tiene una solicitud de programación de horas extraordinarias N°: " + hrs.HorasExtrasId + " " + "para aprobación",
                                    emailMsg, hrs.HorasExtrasId, hrs.FechaSolicitud.ToShortDateString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                    /*nofifica a oficina de partes la resolucion firmada*/
                                    emailMsg = new List<string>();
                                    var OfPartes = _repository.GetFirst<Configuracion>(q => q.Nombre == Util.Enum.Configuracion.CorreoOfPartes.ToString());
                                    //emailMsg.Add("acifuentes@economia.cl"); //oficia de partes
                                    //emailMsg.Add("scid@economia.cl"); //oficia de partes
                                    emailMsg.Add(OfPartes.Valor); //oficia de partes
                                    emailMsg.Add("mmontoya@economia.cl"); //oficia de partes
                                    Documento doc = hrs.Proceso.Documentos.FirstOrDefault(d => d.ProcesoId == hrs.ProcesoId && d.TipoDocumentoId == 9);

                                    _email.NotificacionesHorasExtras(workflowActual,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaHorasExtras),
                                    "Tiene una solicitud de programación de horas extraordinarias N°: " + hrs.HorasExtrasId + " " + "para aprobación",
                                    emailMsg, hrs.HorasExtrasId, hrs.FechaSolicitud.ToShortDateString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, doc, "", "", "");
                                }
                                break;
                            case 8: break;
                            case 9: break;
                            case 10: break;
                            case 11: break;
                            case 12: break;
                            case 13: break;
                            case 14: break;
                            case 15: break;
                            case 16: break;
                            case 17: break;
                            case 18: break;
                        }
                    }

                    #endregion
                }

                /*Si esta en la tarea 14, se crea una segunda tarea de firma resolucion horas compensadas para encargado de GP*/
                if (workflowActual.DefinicionWorkflow.Secuencia == 14)
                {
                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                    {
                        DefinicionWorkflow newdefinicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 16);

                        var workflow = new Workflow();
                        workflow.FechaCreacion = DateTime.Now;
                        workflow.TipoAprobacionId = (int)Util.Enum.TipoAprobacion.SinAprobacion;
                        workflow.Terminada = false;
                        workflow.DefinicionWorkflow = newdefinicionWorkflow;
                        workflow.ProcesoId = workflowActual.ProcesoId;
                        workflow.Mensaje = obj.Observacion;
                        workflow.TareaPersonal = false;
                        workflow.Asunto = !string.IsNullOrEmpty(workflowActual.Asunto) ? workflowActual.Asunto : workflowActual.DefinicionWorkflow.DefinicionProceso.Nombre + " Nro: " + _repository.Get<HorasExtras>(c => c.ProcesoId == workflow.ProcesoId).FirstOrDefault().HorasExtrasId;

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
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
