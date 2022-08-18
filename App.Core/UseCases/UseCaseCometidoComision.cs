using System;
using System.Linq;
using App.Model.Core;
using App.Model.Cometido;
using App.Model.Comisiones;
using App.Model.Pasajes;
using App.Model.Shared;
using App.Model.Sigper;
using System.Collections.Generic;
using App.Core.Interfaces;
using App.Util;

namespace App.Core.UseCases
{
    public class UseCaseCometidoComision
    {
        private readonly IGestionProcesos _repository;
        private readonly IEmail _email;
        private readonly IHsm _hsm;
        private readonly ISigper _sigper;
        private readonly IFile _file;
        private readonly IFolio _folio;

        public UseCaseCometidoComision(IGestionProcesos repositoryGestionProcesos)
        {
            _repository = repositoryGestionProcesos;
        }
        public UseCaseCometidoComision(IGestionProcesos repository, ISigper sigper)
        {
            _repository = repository;
            _sigper = sigper;
        }
        public UseCaseCometidoComision(IGestionProcesos repositoryGestionProcesos, IHsm hsm, IFile file, IFolio folio, ISigper sigper)
        {
            _repository = repositoryGestionProcesos;
            _hsm = hsm;
            _file = file;
            _folio = folio;
            _sigper = sigper;
        }
        public UseCaseCometidoComision(IGestionProcesos repository, IEmail email, ISigper sigper, IFile file)
        {
            _repository = repository;
            _email = email;
            _sigper = sigper;
            _file = file;
        }

        public ResponseMessage RegionInsert(Region obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(obj.Codigo))
                    response.Errors.Add("Debe especificar el código");
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
        public ResponseMessage RegionUpdate(Region obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(obj.Codigo))
                    response.Errors.Add("Debe especificar el código");
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
        public ResponseMessage RegionDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Region>(id);
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

        public ResponseMessage PaisInsert(Pais obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (obj.PaisId == 0)
                    response.Errors.Add("Debe especificar el código");
                if (string.IsNullOrEmpty(obj.PaisNombre))
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
        public ResponseMessage PaisUpdate(Pais obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (obj.PaisId == 0)
                    response.Errors.Add("Debe especificar el código");
                if (string.IsNullOrEmpty(obj.PaisNombre))
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
        public ResponseMessage PaisDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Pais>(id);
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

        public ResponseMessage CiudadInsert(Ciudad obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (obj.CiudadId == 0)
                    response.Errors.Add("Debe especificar el código");
                if (obj.PaisId == 0)
                    response.Errors.Add("Debe especificar el código");
                if (string.IsNullOrEmpty(obj.CiudadNombre))
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
        public ResponseMessage CiudadUpdate(Ciudad obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (obj.CiudadId == 0)
                    response.Errors.Add("Debe especificar el código");
                if (obj.PaisId == 0)
                    response.Errors.Add("Debe especificar el código");
                if (string.IsNullOrEmpty(obj.CiudadNombre))
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
        public ResponseMessage CiudadDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Ciudad>(id);
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

        public ResponseMessage EmpresaAerolineaInsert(EmpresaAerolinea obj)
        {
            var response = new ResponseMessage();

            try
            {
                //if (obj.EmpresaAerolineaId == null)
                //    response.Errors.Add("Debe especificar el Id");
                if (string.IsNullOrEmpty(obj.NombreEmpresa))
                    response.Errors.Add("Debe especificar el nombre");
                if (obj.ActivoEmpresa == false)
                    response.Errors.Add("Debe especificar el Id");

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
        public ResponseMessage EmpresaAerolineaUpdate(EmpresaAerolinea obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (obj.EmpresaAerolineaId == 0)
                    response.Errors.Add("Debe especificar el Id");
                if (string.IsNullOrEmpty(obj.NombreEmpresa))
                    response.Errors.Add("Debe especificar el nombre");
                if (obj.ActivoEmpresa == false)
                    response.Errors.Add("Debe especificar el Id");

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
        public ResponseMessage EmpresaAerolineaDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<EmpresaAerolinea>(id);
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

        public ResponseMessage CotizacionInsert(Cotizacion obj)
        {
            var response = new ResponseMessage();
            var cotizacionDoc = new CotizacionDocumento();

            try
            {
                if (obj.ValorPasaje == 0)
                    response.Errors.Add("Debe especificar el valor del pasaje");
                if (obj.EmpresaAerolineaId != null)
                {
                    var nombre = _repository.Get<EmpresaAerolinea>(e => e.EmpresaAerolineaId == obj.EmpresaAerolineaId).FirstOrDefault().NombreEmpresa;
                    obj.NombreEmpresa = nombre.Trim();
                }
                else
                {
                    response.Errors.Add("Debe especificar el nombre de la empresa");
                }

                foreach (var item in obj.CotizacionDocumento)
                {
                    if(item.FileName==string.Empty)
                    {
                        response.Errors.Add("Debe agregar PDF.");
                    }
                    else
                    {
                        cotizacionDoc = item;
                    }
                }

                if (response.IsValid)
                {
                    obj.Seleccion = false;

                    _repository.Create(obj);
                    _repository.Create(cotizacionDoc);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage CotizacionUpdate(Cotizacion obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (obj.ValorPasaje == 0)
                    response.Errors.Add("Debe especificar el valor del pasaje");
                if (obj.EmpresaAerolineaId != null)
                {
                    var nombre = _repository.Get<EmpresaAerolinea>(e => e.EmpresaAerolineaId == obj.EmpresaAerolineaId).FirstOrDefault().NombreEmpresa;
                    obj.NombreEmpresa = nombre.Trim();
                }
                else
                {
                    response.Errors.Add("Debe especificar el nombre de la empresa");
                }

                if (response.IsValid)
                {
                    obj.Pasaje = _repository.GetById<Pasaje>(obj.PasajeId.Value);

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
        public ResponseMessage CotizacionUpdateSeleccion(Cotizacion obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (obj.ValorPasaje == 0)
                    response.Errors.Add("Debe especificar el valor del pasaje");
                if (obj.EmpresaAerolineaId != null)
                {
                    var nombre = _repository.Get<EmpresaAerolinea>(e => e.EmpresaAerolineaId == obj.EmpresaAerolineaId).FirstOrDefault().NombreEmpresa;
                    obj.NombreEmpresa = nombre.Trim();
                }
                else
                {
                    response.Errors.Add("Debe especificar el nombre de la empresa");
                }

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
        public ResponseMessage CotizacionDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Cotizacion>(id);
                if (obj == null)
                    response.Errors.Add("Dato no encontrado");

                if (response.IsValid)
                {
                    //eliminar los documentos de cotizaciones asociados
                    var documentos = _repository.Get<CotizacionDocumento>(q => q.CotizacionId == obj.CotizacionId);
                    foreach (var item in documentos)
                        _repository.Delete(item);

                    //eliminar la cotizacion
                    _repository.Delete(obj);

                    //guardar
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public ResponseMessage GeneroInsert(Genero obj)
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
        public ResponseMessage GeneroUpdate(Genero obj)
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
        public ResponseMessage GeneroDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Genero>(id);
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


        public ResponseMessage SubsecretariaInsert(Subsecretaria obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(obj.Codigo))
                    response.Errors.Add("Debe especificar el código");
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
        public ResponseMessage SubsecretariaUpdate(Subsecretaria obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (string.IsNullOrEmpty(obj.Codigo))
                    response.Errors.Add("Debe especificar el código");
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
        public ResponseMessage SubsecretariaDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Subsecretaria>(id);
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


        public ResponseMessage ProgramaInsert(Programa obj)
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
        public ResponseMessage ProgramaUpdate(Programa obj)
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
        public ResponseMessage ProgramaDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Programa>(id);
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

        public ResponseMessage DocumentoSign(Documento obj, string email, int? cometidoId)
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

                    /*Se busca cometido para determinar tipo de documento*/
                    string TipoDocto;
                    var com = _repository.GetFirst<Cometido>(c => c.CometidoId == cometidoId.Value);
                    if (com != null)
                    {
                        if (com.IdCalidad == 10)
                        {
                            TipoDocto = "RAEX";/*"ORPA";*/
                        }
                        else
                        {
                            switch (com.IdGrado)
                            {
                                case "B":/*Resolución Ministerial Exenta*/
                                    TipoDocto = "RMEX";
                                    break;
                                case "C": /*Resolución Ministerial Exenta*/
                                    TipoDocto = "RMEX";
                                    break;
                                default:
                                    TipoDocto = "RAEX";/*Resolución Administrativa Exenta*/
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(obj.TipoDocumentoFirma))
                            TipoDocto = obj.TipoDocumentoFirma;
                        else
                            TipoDocto = "OTRO";
                    }


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
                                com.FechaResolucion = DateTime.Now;
                                com.Firma = true;
                                if (com.IdEscalafon == 1 && com.IdEscalafon != null)
                                    com.TipoActoAdministrativo = "Resolución Ministerial Exenta";
                                else if (com.CalidadDescripcion.Contains("HONORARIOS"))
                                    com.TipoActoAdministrativo = "Orden de Pago";
                                else
                                    com.TipoActoAdministrativo = "Resolución Administrativa Exenta";

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
                    var docto = _hsm.SignWSDL(documento.File, idsFirma, documento.DocumentoId, documento.Folio, url_tramites_en_linea.Valor, QR);
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
        public ResponseMessage CotizacionDocumentoInsert(CotizacionDocumento obj)
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
        public ResponseMessage CotizacionDocumentoUpdate(List<CotizacionDocumento> obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (!obj.Any())
                    response.Errors.Add("No existe documento");

                //buscar documento por id y solo actualizar el estado seleccionado
                foreach (var item in obj)
                {
                    var cotizacionDocto = _repository.Get<CotizacionDocumento>().FirstOrDefault(q => q.CotizacionDocumentoId == item.CotizacionDocumentoId);
                    if (cotizacionDocto != null)
                    {
                        cotizacionDocto.Selected = item.Selected;
                        _repository.Update(cotizacionDocto);

                        /*se marca o desmarca la cotizacion seleccionda en lista de cotizaciones*/
                        var cotiza = _repository.GetFirst<Cotizacion>(c => c.CotizacionId == cotizacionDocto.CotizacionId);
                        if (cotiza != null)
                        {
                            cotiza.Seleccion = item.Selected;
                            _repository.Update(cotiza);
                        }
                    }
                }
                _repository.Save();
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        //public ResponseMessage CDPInsert(CDP obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (response.IsValid)
        //        {
        //            _repository.Create(obj);
        //            _repository.Save();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(ex.Message);
        //    }

        //    return response;
        //}
        //public ResponseMessage CDPUpdate(CDP obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (response.IsValid)
        //        {
        //            _repository.Update(obj);
        //            _repository.Save();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(ex.Message);
        //    }

        //    return response;
        //}
        //public ResponseMessage CDPDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<CDP>(id);
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

        //public ResponseMessage InformeHSAInsert(InformeHSA obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (response.IsValid)
        //        {
        //            _repository.Create(obj);
        //            _repository.Save();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(ex.Message);
        //    }

        //    return response;
        //}
        //public ResponseMessage InformeHSAUpdate(InformeHSA obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (response.IsValid)
        //        {
        //            _repository.Update(obj);
        //            _repository.Save();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(ex.Message);
        //    }

        //    return response;
        //}
        //public ResponseMessage InformeHSADelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<InformeHSA>(id);
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

        //public ResponseMessage RadioTaxiInsert(RadioTaxi obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (response.IsValid)
        //        {
        //            _repository.Create(obj);
        //            _repository.Save();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(ex.Message);
        //    }

        //    return response;
        //}
        //public ResponseMessage RadioTaxiUpdate(RadioTaxi obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (response.IsValid)
        //        {
        //            _repository.Update(obj);
        //            _repository.Save();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(ex.Message);
        //    }

        //    return response;
        //}
        //public ResponseMessage RadioTaxiDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<RadioTaxi>(id);
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

        //public ResponseMessage IngresoInsert(GDIngreso obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (response.IsValid)
        //        {
        //            obj.GetFolio();
        //            obj.BarCode = _file.CreateBarCode(obj.Folio);

        //            _repository.Create(obj);
        //            _repository.Save();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(ex.Message);
        //    }

        //    return response;
        //}
        //public ResponseMessage IngresoUpdate(GDIngreso obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (response.IsValid)
        //        {
        //            obj.GetFolio();
        //            obj.BarCode = _file.CreateBarCode(obj.Folio);

        //            _repository.Update(obj);
        //            _repository.Save();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(ex.Message);
        //    }

        //    return response;
        //}
        //public ResponseMessage IngresoDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<GDIngreso>(id);
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

        //public ResponseMessage SIACIngresoInsert(SIACSolicitud obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (response.IsValid)
        //        {
        //            _repository.Create(obj);
        //            _repository.Save();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(ex.Message);
        //    }

        //    return response;
        //}
        //public ResponseMessage SIACIngresoUpdate(SIACSolicitud obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (response.IsValid)
        //        {
        //            _repository.Update(obj);
        //            _repository.Save();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(ex.Message);
        //    }

        //    return response;
        //}
        //public ResponseMessage SIACIngresoDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<SIACSolicitud>(id);
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

        public ResponseMessage CometidoInsert(Cometido obj)
        {
            var response = new ResponseMessage();

            try
            {
                /*validaciones*/
                if (obj.Vehiculo && obj.TipoVehiculoId == 0)
                {
                    response.Errors.Add("Se ha selecionado la opcion de vehiculo, por lo tanto debe señalar el tipo de vehiculo");
                }

                //if (_repository.Get<Destinos>().Count() > 0)
                //{
                //    /*validacion de cantidad de dias al 100% dentro del mes*/
                //    if (_repository.Get<Destinos>(c => c.Cometido.NombreId == obj.NombreId && c.Cometido.FechaSolicitud.Month == DateTime.Now.Month).Sum(d => d.Dias100) > 10)
                //    {
                //        response.Errors.Add("La cantidad de dias al 100%, supera el maximo permitido para el mes señalado");
                //    }
                //    /*validacion de cantidad de dias al 100% dentro del año*/
                //    if (_repository.Get<Destinos>(c => c.Cometido.NombreId == obj.NombreId && c.Cometido.FechaSolicitud.Year == DateTime.Now.Year).Sum(d => d.Dias100) > 90)
                //    {
                //        response.Errors.Add("La cantidad de dias al 100%, supera el maximo permitido para el año señalado");
                //    }
                //} 

                //if (obj.SolicitaReembolso == true && obj.TipoReembolsoId == null)
                //{
                //    response.Errors.Add("Se ha selecionado la opcion de Solicita Reembolso, por lo tanto debe señalar el Tipo de Reembolso");
                //}
                if (!obj.NombreId.HasValue)
                    response.Errors.Add("No se ha señalado el nombre del funcionario que viaja.");

                if (string.IsNullOrEmpty(obj.CometidoDescripcion))
                {
                    response.Errors.Add("Debe ingresar la descripción del cometido.");
                }
                //if ((int)Util.Enum.Cometidos.DiasAnticipacionIngreso > (obj.FechaSolicitud.Date - DateTime.Now.Date).Days)
                //{
                //    response.Errors.Add("La solicitud de cometido se debe realizar con una anticipacion de:" + (int)Util.Enum.Cometidos.DiasAnticipacionIngreso + " " + "dias.");
                //}

                if (obj.Vehiculo && obj.TipoVehiculoId == 0)
                {
                    response.Errors.Add("Se ha selecionado la opcion de vehiculo, por lo tanto debe señalar el tipo de vehiculo");
                }

                if (string.IsNullOrEmpty(obj.IdGrado) && !string.IsNullOrEmpty(obj.GradoDescripcion))
                {
                    obj.IdGrado = obj.GradoDescripcion;
                }

                if (!string.IsNullOrEmpty(obj.ConglomeradoDescripcion))
                    obj.IdConglomerado = Convert.ToInt32(obj.ConglomeradoDescripcion);

                if (obj.IdPrograma == 0)
                    response.Errors.Add("No se ha señalado la fuente de financiamiento (Programa)");


                if (obj.Vehiculo)
                {
                    if (obj.TipoVehiculoId.HasValue)
                    {
                        var vehiculo = _repository.Get<SIGPERTipoVehiculo>().Where(q => q.SIGPERTipoVehiculoId == obj.TipoVehiculoId).FirstOrDefault().Vehiculo.Trim();
                        if (!string.IsNullOrEmpty(vehiculo))
                            obj.TipoVehiculoDescripcion = vehiculo.Trim();
                    }
                    else
                    {
                        response.Errors.Add("Se debe señalar el tipo de vehiculo.");
                    }

                    if (string.IsNullOrEmpty(obj.PlacaVehiculo))
                        response.Errors.Add("Se debe señalar la placa patente del vehiculo.");
                }


                if (response.IsValid)
                {
                    if (obj.NombreId.HasValue)
                    {
                        var nombre = _sigper.GetUserByRut(obj.NombreId.Value).Funcionario.PeDatPerChq;
                        obj.Nombre = nombre.Trim();
                        obj.FechaSolicitud = DateTime.Now;


                        _repository.Create(obj);
                        _repository.Save();

                        /*Se guardan los datos en la tabla mantenedor*/
                        _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Rut", ValorCampo = obj.Rut.ToString() });
                        _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Dv", ValorCampo = obj.DV });
                        _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Cargo", ValorCampo = obj.IdCargo.ToString() });
                        _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Calidad Juridica", ValorCampo = obj.IdCalidad.ToString() });
                        _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Grado", ValorCampo = obj.IdGrado });
                        _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Estamento", ValorCampo = obj.IdEstamento.ToString() });
                        _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Programa", ValorCampo = obj.IdPrograma.ToString() });
                        _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Conglomerado", ValorCampo = obj.IdConglomerado.ToString() });
                        _repository.Save();
                    }
                    else
                        response.Errors.Add("No se ha señalado el nombre del funcionario que viaja");
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage CometidoUpdate(Cometido obj)
        {
            var response = new ResponseMessage();

            try
            {
                //if (_repository.Get<Destinos>(c => c.Cometido.NombreId == obj.NombreId && obj.Destinos.FirstOrDefault().FechaInicio.Month == DateTime.Now.Month).Sum(d => d.Dias100) > 10)
                //{
                //    response.Warnings.Add("La cantidad de dias al 100%, supera el maximo permitido para el mes señalado, por lo tanto se pagaran al 50%");
                //}

                //if (_repository.Get<Destinos>(c => c.Cometido.NombreId == obj.NombreId && obj.Destinos.FirstOrDefault().FechaInicio.Year == DateTime.Now.Year).Sum(d => d.Dias100) > 90)
                //{
                //    response.Warnings.Add("La cantidad de dias al 100%, supera el maximo permitido para el año señalado");
                //}

                /*se buscan los destinos asociados al cometido*/
                var listaDestinosCometido = _repository.Get<Destinos>(c => c.CometidoId == obj.CometidoId).ToList();
                /*se valida que cometido tenga un destino asociado*/
                if (listaDestinosCometido.Count == 0)
                {
                    response.Errors.Add("Se debe agregar por lo menos un destino al cometido");
                }
                else
                {
                    /*validacion de fechas */
                    if (listaDestinosCometido.LastOrDefault().FechaHasta < listaDestinosCometido.FirstOrDefault().FechaInicio)
                    {
                        response.Errors.Add("La fecha de termino no pueder ser mayor a la fecha de inicio");
                    }
                    /*se valida que si existe mas de un destino solo se asigna un viatico*/
                    //if (listaDestinosCometido.Count > 1)
                    //{
                    //    response.Warnings.Add("Usted ha ingresado más de un destino para el cometido, pero solo se le asignara un viático");
                    //}
                    /*valida ingreso de solicitud*/
                    //if ((int)Util.Enum.Cometidos.DiasAnticipacionIngreso > (ListaDestinos.FirstOrDefault().FechaInicio.Date - DateTime.Now.Date).Days)
                    //{
                    //    response.Errors.Add("La solicitud de cometido se debe realizar con una anticipacion de:" + (int)Util.Enum.Cometidos.DiasAnticipacionIngreso + " " + "dias.");                        
                    //}

                    /*se valida que los rangos de fecha no se topen con otros destinos*/
                    //foreach (var destinos in _repository.Get<Destinos>(d => d.CometidoId == obj.CometidoId))
                    //var other = _repository.Get<Destinos>(d => d.Cometido.Rut == obj.Rut && d.DestinoActivo == true && d.FechaInicio.Year == DateTime.Now.Year);
                    //foreach (var otrosDestinos in other)
                    //{
                    //    foreach (var destinoCometido in listaDestinosCometido)
                    //    {
                    //        if (otrosDestinos.FechaInicio.Date == destinoCometido.FechaInicio.Date && otrosDestinos.CometidoId != destinoCometido.CometidoId)
                    //        {
                    //            //response.Errors.Add("El rango de fechas señalados esta en conflicto con los destinos del cometido " + otrosDestinos.CometidoId + "(" +  otrosDestinos.FechaInicio  + ")");
                    //            response.Errors.Add(string.Format("El rango de fechas señalados esta en conflicto con los destinos del cometido {0}, inicio {1}, término {2}", otrosDestinos.CometidoId, otrosDestinos.FechaInicio, otrosDestinos.FechaHasta));
                    //            /*se elimina el destino que topa con otro destino que ya ha sido creado*/
                    //            //DestinosDelete(destinoCometido.DestinoId);
                    //            DestinosAnular(destinoCometido.DestinoId);
                    //        }
                    //    }
                    //}

                    //var other = _repository.Get<Destinos>(d => d.Cometido.Rut == obj.Rut && d.DestinoActivo == true && d.FechaInicio.Year == DateTime.Now.Year).ToList();
                    //foreach (var destinoCometido in listaDestinosCometido)
                    //{
                    //    if(other.Exists(c => c.FechaInicio.Date == destinoCometido.FechaInicio.Date))
                    //    {
                    //        response.Errors.Add(string.Format("La fecha {0}, ya se encuentra solicitada para otro cometido", destinoCometido.FechaInicio.Date.ToShortDateString()));
                    //        //response.Errors.Add(string.Format("El rango de fechas señalados esta en conflicto con los destinos del cometido {0}, inicio {1}, término {2}", otrosDestinos.CometidoId, otrosDestinos.FechaInicio, otrosDestinos.FechaHasta));
                    //        DestinosDelete(destinoCometido.DestinoId);
                    //    }
                    //}


                    /*se suma el valor total del viatico*/
                    obj.TotalViatico = 0;
                    foreach (var des in listaDestinosCometido)
                    {
                        obj.TotalViatico += des.Total;
                    }
                }

                if (!string.IsNullOrEmpty(obj.ConglomeradoDescripcion))
                    obj.IdConglomerado = Convert.ToInt32(obj.ConglomeradoDescripcion);

                if (obj.IdPrograma == null)
                {
                    if (!string.IsNullOrEmpty(obj.ProgramaDescripcion))
                    {
                        var prog = _sigper.GetREPYTs().FirstOrDefault(c => c.RePytDes.Trim() == obj.ProgramaDescripcion.Trim());
                        if (prog != null)
                            obj.IdPrograma = int.Parse(prog.RePytCod.ToString());
                    }
                }

                if (obj.Vehiculo)

                {
                    if (obj.TipoVehiculoId.HasValue)
                    {
                        var vehiculo = _repository.Get<SIGPERTipoVehiculo>().Where(q => q.SIGPERTipoVehiculoId == obj.TipoVehiculoId).FirstOrDefault().Vehiculo.Trim();
                        if (vehiculo != null)
                            obj.TipoVehiculoDescripcion = vehiculo.Trim();
                    }
                    else
                    {
                        response.Errors.Add("Se debe señalar el tipo de vehiculo.");
                    }
                }

                if (!string.IsNullOrEmpty(obj.GradoDescripcion))
                {
                    obj.IdGrado = obj.GradoDescripcion;
                }

                if (string.IsNullOrEmpty(obj.Jefatura) || obj.Jefatura == "Sin jefatura definida")
                    response.Errors.Add("No se ha definido la jefatura del funcionario.");

                for (int i = 0; i < listaDestinosCometido.Count; i++)
                {
                    var fecha = obj.FechaSolicitud.Date.Subtract(listaDestinosCometido[i].FechaInicio.Date).Days;
                    var fechahelp = listaDestinosCometido[i].FechaInicio.Date.Subtract(obj.FechaSolicitud.Date).Days;

                    if (fechahelp < 20)
                    {
                        obj.Atrasado = true;
                    }
                    else
                    {
                        obj.Atrasado = false;
                    }

                    if (obj.IdGrado == "C" || obj.IdGrado == "B")
                    {
                        obj.Atrasado = false;
                    }
                }

                if (response.IsValid)
                {
                   obj.CometidoOk = true;

                    _repository.Update(obj);
                    _repository.Save();

                    /*Se guardan los datos en la tabla mantenedor*/
                    _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Rut", ValorCampo = obj.Rut.ToString() });
                    _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Dv", ValorCampo = obj.DV });
                    _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Cargo", ValorCampo = obj.IdCargo.ToString() });
                    _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Calidad Juridica", ValorCampo = obj.IdCalidad.ToString() });
                    _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Grado", ValorCampo = obj.IdGrado });
                    _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Estamento", ValorCampo = obj.IdEstamento.ToString() });
                    _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Programa", ValorCampo = obj.IdPrograma.ToString() });
                    _repository.Create(new Mantenedor { IdCometido = obj.CometidoId.ToString(), NombreCampo = "Conglomerado", ValorCampo = obj.IdConglomerado.ToString() });
                    _repository.Save();

                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage CometidoDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Cometido>(id);
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

        public ResponseMessage CometidoAnular(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Cometido>(id);
                if (obj == null)
                    response.Errors.Add("Dato no encontrado");
                else
                    obj.Activo = false;

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

        private void metodoMensaje(Destinos obj)
        {
            obj.Dias100 = 0;
            obj.Dias60 = 0;
            obj.Dias40 = 0;
            obj.Dias100Aprobados = 0;
            obj.Dias60Aprobados = 0;
            obj.Dias40Aprobados = 0;
            obj.Dias100Monto = 0;
            obj.Dias60Monto = 0;
            obj.Dias40Monto = 0;
            obj.TotalViatico = 0;
            obj.Total = 0;
        }

        public ResponseMessage DestinosInsert(Destinos obj)
        {
            var response = new ResponseMessage();

            try
            {
                var cometido = _repository.GetFirst<Cometido>(c => c.CometidoId == obj.CometidoId);
                //reglas de negocio
                /*validacion de fechas*/
                if (obj.FechaInicio == null)
                {
                    response.Errors.Add("La fecha de inicio no es válida.");
                }
                if (obj.FechaHasta == null)
                {
                    response.Errors.Add("La fecha de término no es válida.");
                }

                /*se valida que la fecha de inicio no se superior que la de termino*/
                if (obj.FechaHasta < obj.FechaInicio)
                {
                    response.Errors.Add("La fecha de inicio no puede ser superior o igual a la de término");
                }

                /*Validacion de viaticos por localidades adyacentes*/
                bool adyacente = false;
                //bool adyacente = true;
                if (cometido != null)
                {
                    switch (cometido.IdConglomerado.Value)
                    {
                        case 1:
                            if (obj.IdComuna == "01101" || obj.IdComuna == "01107")
                            {
                                if (obj.LocalidadId.Value == 3944)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente exceptuada por el Decreto 90.");
                                }
                                else
                                {
                                    if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                        metodoMensaje(obj); adyacente = true;
                                    }
                                }
                            }
                            break;
                        case 2:
                            if (obj.IdComuna == "02101")
                            {
                                if(obj.LocalidadId.Value==3945 || obj.LocalidadId.Value==3946 || obj.LocalidadId.Value == 3947 || obj.LocalidadId.Value == 3948)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente exceptuada por el Decreto 90.");
                                }
                                else
                                {
                                    if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                        adyacente = true;
                                        metodoMensaje(obj);
                                    }
                                }
                            }else if(obj.IdComuna=="02102")
                            {
                                if (obj.LocalidadId.Value == 3949)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente exceptuada por el Decreto 90.");
                                }
                                else
                                {
                                    if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                        adyacente = true;
                                        metodoMensaje(obj);
                                    }
                                }
                            }
                            else
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj); adyacente = true;
                                }
                            }
                            break;
                        case 3:
                            if (obj.IdComuna == "03101")
                            {
                                if (obj.LocalidadId.Value == 3949 || obj.LocalidadId.Value == 3950
                                || obj.LocalidadId.Value == 3951
                                || obj.LocalidadId.Value == 3952
                                || obj.LocalidadId.Value == 3953
                                || obj.LocalidadId.Value == 3954
                                || obj.LocalidadId.Value == 3955
                                || obj.LocalidadId.Value == 3956
                                || obj.LocalidadId.Value == 3957)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente exceptuada por el Decreto 90.");
                                }
                                else
                                {
                                    if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                        metodoMensaje(obj); adyacente = true;
                                    }
                                }
                            }
                            break;
                        case 4:
                            if (obj.IdComuna == "04101")
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj); adyacente = true;
                                }
                            }else if(obj.IdComuna == "04102")
                            {
                                if(obj.LocalidadId.Value == 3959 || obj.LocalidadId.Value==3960)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente exceptuada por el Decreto 90.");
                                }
                                else
                                {
                                    if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                        metodoMensaje(obj); adyacente = true;
                                    }
                                }
                            }
                            else
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj); adyacente = true;
                                }
                            }
                            break;
                        case 5:
                            if (obj.IdComuna == "05101"
                                || obj.IdComuna == "05109"
                                || obj.IdComuna == "05103"
                                || obj.IdComuna == "05801"
                                || obj.IdComuna == "05804"
                                || obj.IdComuna == "05802"
                                || obj.IdComuna == "05803"
                                || obj.IdComuna == "05501"
                                || obj.IdComuna == "05107"
                                || obj.IdComuna == "05602"
                                || obj.IdComuna == "05603"
                                || obj.IdComuna == "05604"
                                || obj.IdComuna == "05605"
                                || obj.IdComuna == "05601"
                                || obj.IdComuna == "05606")
                            {
                                if(obj.IdComuna=="05603")
                                {
                                    if(obj.LocalidadId.Value==3961 || obj.LocalidadId.Value==3962)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente exceptuada por el Decreto 90.");
                                    }
                                    else
                                    {
                                        if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                        {
                                            response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                            metodoMensaje(obj); adyacente = true;
                                        }
                                    }
                                }else if(obj.IdComuna== "05606")
                                {
                                    if(obj.LocalidadId.Value==3963)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente exceptuada por el Decreto 90.");
                                    }
                                    else
                                    {
                                        if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                        {
                                            response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                            metodoMensaje(obj); adyacente = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                        metodoMensaje(obj); adyacente = true;
                                    }
                                }
                            }

                            break;
                        case 6:
                            if (obj.IdComuna == "06101"
                           || obj.IdComuna == "06108"
                           || obj.IdComuna == "06102"
                           || obj.IdComuna == "06110"
                           || obj.IdComuna == "06106"
                           || obj.IdComuna == "06111"
                           || obj.IdComuna == "06116"
                           || obj.IdComuna == "06103"
                           || obj.IdComuna == "06104"
                           || obj.IdComuna == "06105")
                            {
                                if(obj.IdComuna== "06108")
                                {
                                    if(obj.LocalidadId.Value==3965||obj.LocalidadId.Value==3966||obj.LocalidadId.Value==3967)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente exceptuada por el Decreto 90.");
                                    }
                                    else
                                    {
                                        if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                        {
                                            response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                            metodoMensaje(obj); adyacente = true;
                                        }
                                    }
                                }else if(obj.IdComuna == "06110")
                                {
                                    if (obj.LocalidadId.Value == 3968)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente exceptuada por el Decreto 90.");
                                    }
                                    else
                                    {
                                        if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                        {
                                            response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                            metodoMensaje(obj); adyacente = true;
                                        }
                                    }
                                }else if(obj.IdComuna=="06116")
                                {
                                    if (obj.LocalidadId.Value == 3964)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente exceptuada por el Decreto 90.");
                                    }
                                    else
                                    {
                                        if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                        {
                                            response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                            metodoMensaje(obj); adyacente = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                        metodoMensaje(obj); adyacente = true;
                                    }
                                }
                            }
                            break;
                        case 7:
                            if (obj.IdComuna == "07101" || obj.IdComuna == "07105"
                                || obj.IdComuna == "07106" || obj.IdComuna == "07107"
                                || obj.IdComuna == "07109" || obj.IdComuna == "07110")
                            {
                                if(obj.IdComuna=="07109")
                                {
                                    if(obj.LocalidadId == 3969 ||
                                        obj.LocalidadId == 3970 ||
                                        obj.LocalidadId == 3971 ||
                                        obj.LocalidadId == 3972 ||
                                        obj.LocalidadId == 3973 )
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente exceptuada por el Decreto 90.");
                                    }
                                    else
                                    {
                                        if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                        {
                                            response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                            metodoMensaje(obj); adyacente = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                        metodoMensaje(obj); adyacente = true;
                                    }
                                }
                            }
                            break;
                        case 8:
                            if (obj.IdComuna == "08101"
                             || obj.IdComuna == "08108"
                             || obj.IdComuna == "08103"
                             || obj.IdComuna == "08107"
                             || obj.IdComuna == "08110"
                             || obj.IdComuna == "08112"
                             || obj.IdComuna == "08102"
                             || obj.IdComuna == "08106"
                             || obj.IdComuna == "08401"
                             || obj.IdComuna == "08406")
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj); adyacente = true;
                                }
                            }
                            break;
                        case 9:
                            if (obj.IdComuna == "09101" || obj.IdComuna == "09112")
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj); adyacente = true;
                                }
                            }
                            break;
                        case 10:
                            if (obj.IdComuna == "10101" 
                                || obj.IdComuna == "10107"
                                || obj.IdComuna == "10105"
                                || obj.IdComuna == "10109")
                            {
                                if(obj.IdComuna=="10101")
                                {
                                    if(obj.LocalidadId.Value == 3974 || obj.LocalidadId.Value == 3975
                                        || obj.LocalidadId.Value == 3976 || obj.LocalidadId.Value==3977)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente exceptuada por el Decreto 90.");
                                    }
                                    else
                                    {
                                        if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                        {
                                            response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                            metodoMensaje(obj); adyacente = true;
                                        }
                                    }
                                }
                                else if(obj.IdComuna=="10109")
                                {
                                    if(obj.LocalidadId.Value == 3978 || obj.LocalidadId.Value == 3979 )
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente exceptuada por el Decreto 90.");
                                    }
                                    else
                                    {
                                        if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                        {
                                            response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                            metodoMensaje(obj); adyacente = true;
                                        }
                                    }
                                }else
                                {
                                    if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                        metodoMensaje(obj); adyacente = true;
                                    }
                                }                               
                            }
                            break;
                        //case 11: /* hace algo*/; break;
                        //case 12: /* hace algo*/; break;
                        case 13:
                            if (obj.IdComuna == "13101"
                            || obj.IdComuna == "13130"
                            || obj.IdComuna == "13129"
                            || obj.IdComuna == "13131"
                            || obj.IdComuna == "13109"
                            || obj.IdComuna == "13401"
                            || obj.IdComuna == "13201"
                            || obj.IdComuna == "13111"
                            || obj.IdComuna == "13112"
                            || obj.IdComuna == "13110"
                            || obj.IdComuna == "13122"
                            || obj.IdComuna == "13118"
                            || obj.IdComuna == "13120"
                            || obj.IdComuna == "13113"
                            || obj.IdComuna == "13114"
                            || obj.IdComuna == "13123"
                            || obj.IdComuna == "13104"
                            || obj.IdComuna == "13125"
                            || obj.IdComuna == "13128"
                            || obj.IdComuna == "13117"
                            || obj.IdComuna == "13103"
                            || obj.IdComuna == "13126"
                            || obj.IdComuna == "13124"
                            || obj.IdComuna == "13106"
                            || obj.IdComuna == "13119"
                            || obj.IdComuna == "13102"
                            || obj.IdComuna == "13105"
                            || obj.IdComuna == "13127"
                            || obj.IdComuna == "13132"
                            || obj.IdComuna == "13116"
                            || obj.IdComuna == "13108"
                            || obj.IdComuna == "13121"
                            || obj.IdComuna == "13107"
                            || obj.IdComuna == "13115"
                            || obj.IdComuna == "13302"
                            || obj.IdComuna == "13403"
                            || obj.IdComuna == "13604"
                            || obj.IdComuna == "13605"
                            || obj.IdComuna == "13602"
                            || obj.IdComuna == "13603"
                            || obj.IdComuna == "13402"
                            || obj.IdComuna == "13404"
                            || obj.IdComuna == "13202"
                            || obj.IdComuna == "13301")
                            {
                                // Validación para farellones, Peldehue y Colina Oriente
                                if(obj.LocalidadId.Value == 3943 || obj.LocalidadId.Value == 3980|| obj.LocalidadId.Value == 3981)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente exceptuada por el Decreto 90.");
                                }
                                else
                                {
                                    if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                    {
                                        response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                        metodoMensaje(obj); adyacente = true;
                                    }
                                }
                            }
                            break;
                            //case 14: /* hace algo*/; break;
                            //case 15: /* hace algo*/; break;
                            //case 16: /* hace algo*/; break;

                    }
                }

                /*Se valida que la cantidad de dias solicitados correspondan con los viaticos*/
                if (adyacente == false)
                {
                    var dias = (obj.FechaHasta - obj.FechaInicio).Days + 1;
                    var cant = obj.Dias100 + obj.Dias60 + obj.Dias40 + obj.Dias50 + obj.Dias00;
                    if (dias != cant)
                    {
                        if (obj.FechaInicio.Hour < 17)
                        {
                            response.Errors.Add("La cantidad de días solicitados debe(n) ser " + dias + " en total");
                        }
                    }
                }

                /*Se valida la cantidad de dias al 100% dentro del mes, no puede superar los 10 dias. Y dentro del año no puede superar los 90 dias*/
                if (obj.Dias100 > 0)
                {
                    int Totaldias100Mes = 0;
                    int Totaldias100Ano = 0;
                    var mes = obj.FechaInicio.Month; //DateTime.Now.Month;
                    var year = obj.FechaInicio.Year; //DateTime.Now.Year;
                    foreach (var destinos in _repository.Get<Destinos>(d => d.CometidoId != null))
                    {
                        var solicitanteDestino = _repository.Get<Cometido>(c => c.CometidoId == destinos.CometidoId).FirstOrDefault().NombreId;
                        var solicitante = _repository.Get<Cometido>(c => c.CometidoId == obj.CometidoId).FirstOrDefault().NombreId;

                        if (solicitanteDestino == solicitante)
                        {
                            if (destinos.FechaInicio.Month == mes && destinos.Dias100 != 0 && destinos.Dias100 != null)
                            {
                                Totaldias100Mes += destinos.Dias100.Value;
                            }
                            if (destinos.FechaInicio.Year == year && destinos.Dias100 != 0 && destinos.Dias100 != null)
                            {
                                Totaldias100Ano += destinos.Dias100.Value;
                            }
                        }
                    }
                    if (Totaldias100Mes + obj.Dias100 > 10)
                    {
                        obj.Dias50 = obj.Dias100;
                        obj.Dias100 = 0;
                        obj.TotalViatico = (obj.Dias100Monto / 2) + obj.Dias60Monto + obj.Dias40Monto;
                        obj.Total = (obj.Dias100Monto / 2) + obj.Dias60Monto + obj.Dias40Monto;
                        obj.Dias50Monto = obj.Dias100Monto / 2;

                        response.Warnings.Add("Se ha excedido en: " + (Totaldias100Mes + obj.Dias100.Value - 10) + " la cantidad permitida de dias solicitados al 100%, dentro del Mes, por lo tanto se pagaran al 50%");
                    }
                    if (Totaldias100Ano + obj.Dias100 > 90)
                    {
                        response.Errors.Add("Se ha excedido en :" + (Totaldias100Ano + obj.Dias100 - 90) + " la cantidad permitida de dias solicitados al 100%, dentro de un año");
                    }
                }

                /*se valida que los rangos de fecha no se topen con otros destinos*/
                //var ListaDestinos = _repository.Get<Destinos>(c => c.CometidoId == obj.CometidoId).ToList();
                foreach (var destinos in _repository.Get<Destinos>(d => d.CometidoId != null && d.DestinoActivo == true))
                {
                    var solicitanteDestino = _repository.Get<Cometido>(c => c.CometidoId == destinos.CometidoId).FirstOrDefault().NombreId;
                    var solicitante = _repository.Get<Cometido>(c => c.CometidoId == obj.CometidoId).FirstOrDefault().NombreId;

                    if (solicitanteDestino == solicitante)
                    {
                        if (destinos.FechaInicio.Date == obj.FechaInicio.Date)
                        {

                            if (destinos.DestinoId != obj.DestinoId)
                            {
                                if (destinos.Dias40 > 0 && destinos.Dias100 == 0 && destinos.Dias60 == 0)
                                {
                                    if (obj.Dias100 > 0 || obj.Dias40 > 0)
                                    {
                                        response.Warnings.Add("Ya se ha solicitado viatico para las fechas señaladas");
                                        obj.Dias100 = 0;
                                        obj.Dias60 = 0;
                                        obj.Dias40 = 0;
                                        obj.Total = 0;
                                        obj.TotalViatico = 0;
                                    }
                                }

                                if (destinos.Dias40 == 0 && destinos.Dias100 == 0 && destinos.Dias60 > 0)
                                {
                                    if (obj.Dias100 > 0 || obj.Dias60 > 0)
                                    {
                                        response.Warnings.Add("Ya se ha solicitado viatico para las fechas señaladas");
                                        obj.Dias100 = 0;
                                        obj.Dias60 = 0;
                                        obj.Dias40 = 0;
                                        obj.Total = 0;
                                        obj.TotalViatico = 0;
                                    }
                                }

                                if (destinos.Dias40 == 0 && destinos.Dias100 > 0 && destinos.Dias60 == 0)
                                {
                                    response.Warnings.Add("Ya se ha solicitado viatico para las fechas señaladas");
                                    obj.Dias100 = 0;
                                    obj.Dias60 = 0;
                                    obj.Dias40 = 0;
                                    obj.Total = 0;
                                    obj.TotalViatico = 0;
                                }


                                ////response.Errors.Add("El rango de fechas señalados esta en conflicto con otros destinos");
                                //if (obj.Dias100 > 0 || obj.Dias60 > 0 || obj.Dias40 > 0)
                                //{
                                //    response.Warnings.Add("Ya se ha solicitado viatico para las fechas señaladas");
                                //    obj.Dias100 = 0;
                                //    obj.Dias60 = 0;
                                //    obj.Dias40 = 0;
                                //    obj.Total = 0;
                                //    obj.TotalViatico = 0;
                                //}
                            }
                        }
                    }
                }

                /*	Cualquier cometido con duración menor a 4 horas no se le asigna viático*/
                if ((obj.FechaHasta - obj.FechaInicio).TotalHours < 4)
                {
                    //response.Warnings.Add("El cometido tiene una duración menor a 4 horas, por lo tanto no  le corresponde viático");
                    //response.Errors.Add("El cometido tiene una duración menor a 4 horas, por lo tanto no  le corresponde viático");
                    response.Warnings.Add("El cometido tiene una duración menor a 4 horas, por lo tanto no  le corresponde viático");
                    obj.Dias100 = 0;
                    obj.Dias60 = 0;
                    obj.Dias40 = 0;
                    obj.Dias100Aprobados = 0;
                    obj.Dias60Aprobados = 0;
                    obj.Dias40Aprobados = 0;
                    obj.Dias100Monto = 0;
                    obj.Dias60Monto = 0;
                    obj.Dias40Monto = 0;
                    obj.TotalViatico = 0;
                    obj.Total = 0;
                }

                if (cometido.SolicitaViatico == false)
                {
                    response.Warnings.Add("Este cometido tendrá un viático de $0");
                    obj.Dias100 = 0;
                    obj.Dias60 = 0;
                    obj.Dias40 = 0;
                    obj.Dias100Aprobados = 0;
                    obj.Dias60Aprobados = 0;
                    obj.Dias40Aprobados = 0;
                    obj.Dias100Monto = 0;
                    obj.Dias60Monto = 0;
                    obj.Dias40Monto = 0;
                    obj.TotalViatico = 0;
                    obj.Total = 0;
                }

                if (obj.IdComuna != null)
                {
                    var comuna = _sigper.GetDGCOMUNAs().FirstOrDefault(c => c.Pl_CodCom == obj.IdComuna).Pl_DesCom.Trim();
                    obj.ComunaDescripcion = comuna;
                }
                else
                {
                    response.Errors.Add("Se debe señalar la comuna de destino");
                }

                if(obj.LocalidadId.ToString() != null)
                {
                    if (obj.LocalidadId != 0)
                    {
                        var localidad = _repository.GetById<Localidad>(obj.LocalidadId).NombreLocalidad.Trim();
                        obj.NombreLocalidad = localidad;
                    }
                    else
                    {
                        //obj.NombreLocalidad = string.Empty;
                        response.Errors.Add("Se debe señalar la localidad de destino");
                    }
                }
                else
                {
                    response.Errors.Add("Se debe señalar la localidad de destino");
                }

                if (obj.IdRegion != null)
                {
                    var region = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == obj.IdRegion).Pl_DesReg.Trim();
                    obj.RegionDescripcion = region;
                }
                else
                {
                    response.Errors.Add("Se debe señalar la region de destino");
                }
                if (obj.Dias00 == null)
                    obj.Dias00 = 0;
                if (obj.Dias50 == null)
                    obj.Dias50 = 0;

                //if(cometido.ReqPasajeAereo == true)
                //{
                //    if (obj.IdOrigenRegion != null)
                //    {
                //        var region = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == obj.IdOrigenRegion.ToString()).Pl_DesReg.Trim();
                //        obj.OrigenRegion = region;
                //    }
                //    else
                //    {
                //        response.Errors.Add("Se debe señalar la region de origen");
                //    }
                //}

                /*Se dejan los mismo valores de los solicitados como aprobados en la creacion del destino*/
                obj.Dias100Aprobados = obj.Dias100;
                obj.Dias60Aprobados = obj.Dias60;
                obj.Dias40Aprobados = obj.Dias40;
                obj.Dias50Aprobados = obj.Dias50;
                obj.Dias00Aprobados = obj.Dias00;

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
        public ResponseMessage DestinosUpdate(Destinos objController)
        {
            var response = new ResponseMessage();
            Destinos obj = _repository.GetFirst<Destinos>(q => q.DestinoId == objController.DestinoId);
            try
            {
                obj.Dias100 = objController.Dias100;
                obj.Dias60 = objController.Dias60;
                obj.Dias40 = objController.Dias40;
                obj.Dias50 = objController.Dias50;
                obj.Dias00 = objController.Dias00;
                obj.Dias100Aprobados = objController.Dias100Aprobados;
                obj.Dias60Aprobados = objController.Dias60Aprobados;
                obj.Dias40Aprobados = objController.Dias40Aprobados;
                obj.Dias50Aprobados = objController.Dias50Aprobados;
                obj.Dias00Aprobados = objController.Dias00Aprobados;
                obj.Dias100Monto = objController.Dias100Monto;
                obj.Dias60Monto = objController.Dias60Monto;
                obj.Dias40Monto = objController.Dias40Monto;
                obj.Dias50Monto = objController.Dias50Monto;
                obj.Dias00Monto = objController.Dias00Monto;
                obj.Total = objController.Total;

                var des = _repository.Get<Destinos>(d => d.CometidoId != null && d.CometidoId != objController.CometidoId).ToList();
                //reglas de negocio
                /*validacion de fechas*/
                if (obj.FechaInicio == null)
                {
                    response.Errors.Add("La fecha de inicio no es válida.");
                }
                if (obj.FechaHasta == null)
                {
                    response.Errors.Add("La fecha de término no es válida.");
                }

                /*se valida que la cantidad de dias sea igual que lo solicitado por cada destino ingresado*/
                //var dias = (obj.FechaHasta - obj.FechaInicio).Days + 1;
                var dias = (objController.FechaHasta - objController.FechaInicio).Days + 1;
                var cantNew = obj.Dias100Aprobados + obj.Dias60Aprobados + obj.Dias40Aprobados + obj.Dias00Aprobados + obj.Dias50Aprobados;
                var cantOld = objController.Dias100 + objController.Dias60 + objController.Dias40 + objController.Dias00 + objController.Dias50;
                if (objController.EditGP)
                {
                    if (dias != cantNew)
                        response.Errors.Add("La cantidad de días solicitados debe(n) ser " + dias + " en total");
                }
                else
                {
                    if (dias != cantOld)
                    {
                        if (objController.FechaInicio.Hour < 17)
                        {
                            response.Errors.Add("La cantidad de días solicitados debe(n) ser " + dias + " en total");
                        }
                    }
                }

                /*se valida que la fecha de inicio no se superior que la de termino*/
                if (objController.FechaHasta < objController.FechaInicio)
                    response.Errors.Add("La fecha de inicio no puede ser superior o igual a la de término");


                /*Se valida la cantidad de dias al 100% dentro del mes, no puede superar los 10 dias. Y dentro del año no puede superar los 90 dias*/
                if (obj.Dias100 > 0)
                {
                    int Totaldias100Mes = 0;
                    int Totaldias100Ano = 0;
                    var mes = obj.FechaInicio.Month; //DateTime.Now.Month;
                    var year = obj.FechaInicio.Year; //DateTime.Now.Year;                                           
                    foreach (var destinos in des)
                    {
                        var solicitanteDestino = _repository.Get<Cometido>(c => c.CometidoId == destinos.CometidoId).FirstOrDefault().NombreId;
                        var solicitante = _repository.Get<Cometido>(c => c.CometidoId == objController.CometidoId).FirstOrDefault().NombreId;

                        if (solicitanteDestino == solicitante)
                        {
                            if (destinos.FechaInicio.Month == mes && destinos.Dias100 != 0 && destinos.Dias100 != null)
                                Totaldias100Mes += destinos.Dias100.Value;

                            if (destinos.FechaInicio.Year == year && destinos.Dias100 != 0 && destinos.Dias100 != null)
                                Totaldias100Ano += destinos.Dias100.Value;
                        }
                    }
                    if (Totaldias100Mes + obj.Dias100 > 10)
                    {
                        response.Warnings.Add("Se ha excedido en: " + (Totaldias100Mes + obj.Dias100.Value - 10) + " la cantidad permitida de dias solicitados al 100%, dentro del Mes, por lo tanto se pagaran al 50%");

                        obj.Dias50 = obj.Dias100;
                        obj.Dias100 = 0;
                        obj.TotalViatico = (obj.Dias100Monto / 2) + obj.Dias60Monto + obj.Dias40Monto;
                        obj.Total = (obj.Dias100Monto / 2) + obj.Dias60Monto + obj.Dias40Monto;
                        obj.Dias50Monto = obj.Dias100Monto / 2;
                        obj.Dias100 = 0;
                    }
                    if (Totaldias100Ano + obj.Dias100 > 90)
                        response.Errors.Add("Se ha excedido en :" + (Totaldias100Ano + obj.Dias100 - 90) + " la cantidad permitida de dias solicitados al 100%, dentro de un año");

                }

                /*En la tarea de analista gestion personas, se valida la cantidad de dias al 100% dentro del mes, no puede superar los 10 dias. Y dentro del año no puede superar los 90 dias*/
                if (obj.Dias100Aprobados > 0)
                {
                    int Totaldias100MesAprobados = 0;
                    int Totaldias100AnoAprobados = 0;
                    var mes = obj.FechaInicio.Month; //DateTime.Now.Month;
                    var year = obj.FechaInicio.Year; //DateTime.Now.Year;                                           
                    foreach (var destinos in des)
                    {
                        var solicitanteDestino = _repository.Get<Cometido>(c => c.CometidoId == destinos.CometidoId).FirstOrDefault().NombreId;
                        var solicitante = _repository.Get<Cometido>(c => c.CometidoId == objController.CometidoId).FirstOrDefault().NombreId;

                        if (solicitanteDestino == solicitante)
                        {
                            if (destinos.FechaInicio.Month == mes && destinos.Dias100Aprobados != 0 && destinos.Dias100Aprobados != null)
                                Totaldias100MesAprobados += destinos.Dias100Aprobados.Value;

                            if (destinos.FechaInicio.Year == year && destinos.Dias100Aprobados != 0 && destinos.Dias100Aprobados != null)
                                Totaldias100AnoAprobados += destinos.Dias100Aprobados.Value;
                        }
                    }
                    if (Totaldias100MesAprobados + obj.Dias100Aprobados > 10)
                    {
                        response.Warnings.Add("Se ha excedido en: " + ((Totaldias100MesAprobados + obj.Dias100Aprobados.Value) - 10) + " la cantidad permitida de dias solicitados al 100%, dentro del Mes, por lo tanto se pagaran al 50%");

                        obj.Dias50Aprobados = obj.Dias100Aprobados;
                        obj.Dias100Aprobados = 0;
                        obj.TotalViatico = (obj.Dias100Monto / 2) + obj.Dias60Monto + obj.Dias40Monto;
                        obj.Total = (obj.Dias100Monto / 2) + obj.Dias60Monto + obj.Dias40Monto;
                        obj.Dias50Monto = obj.Dias100Monto / 2;
                        obj.Dias100Monto = 0;
                    }
                    if (Totaldias100AnoAprobados + obj.Dias100 > 90)
                        response.Errors.Add("Se ha excedido en :" + (Totaldias100AnoAprobados + obj.Dias100Aprobados - 90) + " la cantidad permitida de dias solicitados al 100%, dentro de un año");

                }


                /*	Cualquier cometido con duración menor a 4 horas no se le asigna viático*/
                if ((objController.FechaHasta - objController.FechaInicio).TotalHours < 4)
                {
                    //response.Warnings.Add("El cometido tiene una duración menor a 4 horas, por lo tanto no  le corresponde viático");
                    response.Errors.Add("El cometido tiene una duración menor a 4 horas, por lo tanto no  le corresponde viático");
                    obj.Dias100 = 0;
                    obj.Dias60 = 0;
                    obj.Dias40 = 0;
                    obj.Dias100Aprobados = 0;
                    obj.Dias60Aprobados = 0;
                    obj.Dias40Aprobados = 0;
                    obj.Dias100Monto = 0;
                    obj.Dias60Monto = 0;
                    obj.Dias40Monto = 0;
                    obj.TotalViatico = 0;
                    obj.Total = 0;
                }

                var com = _repository.GetFirst<Cometido>(c => c.CometidoId == obj.CometidoId);
                if (com.SolicitaViatico == false)
                {
                    response.Warnings.Add("Este cometido tendrá un viático de $0");
                    obj.Dias100 = 0;
                    obj.Dias60 = 0;
                    obj.Dias40 = 0;
                    obj.Dias100Aprobados = 0;
                    obj.Dias60Aprobados = 0;
                    obj.Dias40Aprobados = 0;
                    obj.Dias100Monto = 0;
                    obj.Dias60Monto = 0;
                    obj.Dias40Monto = 0;
                    obj.TotalViatico = 0;
                    obj.Total = 0;
                }

                if (objController.IdComuna != null)
                {
                    var comuna = _sigper.GetDGCOMUNAs().FirstOrDefault(c => c.Pl_CodCom == objController.IdComuna).Pl_DesCom;
                    obj.ComunaDescripcion = comuna;
                }
                else
                    response.Errors.Add("Se debe señalar la comuna de destino");

                if (objController.LocalidadId.ToString() != null)
                {
                    if (objController.LocalidadId != 0)
                    {
                        var localicad = _repository.GetById<Localidad>(objController.LocalidadId).NombreLocalidad.Trim();
                        obj.NombreLocalidad = localicad;
                    }
                    else
                    {
                        response.Errors.Add("Se debe señalar la localidad de destino.");
                    }
                }
                else
                {
                    response.Errors.Add("Se debe señalar la localidad de destino.");
                }

                /*if (obj.LocalidadId.ToString() != null)
                {
                    if(obj.LocalidadId != 0)
                    {
                        var localidad = _repository.GetById<Localidad>(obj.LocalidadId).NombreLocalidad.Trim();
                        obj.NombreLocalidad = localidad;
                    }
                    else
                    {
                        //obj.NombreLocalidad = string.Empty;
                        response.Errors.Add("Se debe señalar la localidad de destino");
                    }   
                }
                else
                    response.Errors.Add("Se debe señalar la localidad de destino");*/


                if (objController.IdRegion != null)
                {
                    var region = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == objController.IdRegion).Pl_DesReg;
                    obj.RegionDescripcion = region;
                }
                else
                    response.Errors.Add("Se debe señalar la region de destino");


                //if (com.ReqPasajeAereo == true)
                //{
                //    if (objController.IdOrigenRegion != null)
                //    {
                //        //var region = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == objController.IdOrigenRegion.ToString()).Pl_DesReg.Trim();
                //        //obj.OrigenRegion = region;
                //    }
                //    else
                //    {
                //        response.Errors.Add("Se debe señalar la region de origen");
                //    }
                //}

                /*Validacion de viaticos por localidades adyacentes*/
                bool adyacente = false;
                //bool adyacente = true;
                if (com != null)
                {
                    switch (com.IdConglomerado.Value)
                    {
                        case 1:
                            if (objController.IdComuna == "01101" || objController.IdComuna == "01107")
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj);
                                    adyacente = true;
                                }
                                else
                                {
                                    adyacente = false;
                                }
                            }
                            break;
                        case 2:
                            if (objController.IdComuna == "02101" || objController.IdComuna == "02102")
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    adyacente = true;
                                    metodoMensaje(obj);
                                }
                                else
                                {
                                    adyacente = false;
                                }
                            }
                            break;
                        case 3:
                            if (objController.IdComuna == "03101")
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj);
                                    adyacente = true;
                                }
                                else
                                {
                                    adyacente = false;
                                }
                            }
                            break;
                        case 4:
                            if (objController.IdComuna == "04101" || objController.IdComuna == "04102")
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj);
                                    adyacente = true;
                                }
                                else
                                {
                                    adyacente = false;
                                }
                            }
                            break;
                        case 5:
                            if (objController.IdComuna == "05101"
                                || objController.IdComuna == "05109"
                                || objController.IdComuna == "05103"
                                || objController.IdComuna == "05801"
                                || objController.IdComuna == "05804"
                                || objController.IdComuna == "05802"
                                || objController.IdComuna == "05803"
                                || objController.IdComuna == "05501"
                                || objController.IdComuna == "05107"
                                || objController.IdComuna == "05602"
                                || objController.IdComuna == "05603"
                                || objController.IdComuna == "05604"
                                || objController.IdComuna == "05605"
                                || objController.IdComuna == "05601"
                                || objController.IdComuna == "05606")
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj); adyacente = true;
                                }
                                else
                                {
                                    adyacente = false;
                                }
                            }

                            break;
                        case 6:
                            if (objController.IdComuna == "06101"
                           || objController.IdComuna == "06108"
                           || objController.IdComuna == "06102"
                           || objController.IdComuna == "06110"
                           || objController.IdComuna == "06106"
                           || objController.IdComuna == "06111"
                           || objController.IdComuna == "06116"
                           || objController.IdComuna == "06103"
                           || objController.IdComuna == "06104"
                           || objController.IdComuna == "06105")
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj); adyacente = true;
                                }
                                else
                                {
                                    adyacente = false;
                                }
                            }
                            break;
                        case 7:
                            if (obj.IdComuna == "07101"
                        || objController.IdComuna == "07105"
                        || objController.IdComuna == "07106"
                        || objController.IdComuna == "07107"
                        || objController.IdComuna == "07109"
                        || objController.IdComuna == "07110")
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj); adyacente = true;
                                }
                                else
                                {
                                    adyacente = false;
                                }
                            }
                            break;
                        case 8:
                            if (objController.IdComuna == "08101"
                             || objController.IdComuna == "08108"
                             || objController.IdComuna == "08103"
                             || objController.IdComuna == "08107"
                             || objController.IdComuna == "08110"
                             || objController.IdComuna == "08112"
                             || objController.IdComuna == "08102"
                             || objController.IdComuna == "08106"
                             || objController.IdComuna == "08401"
                             || objController.IdComuna == "08406")
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj); adyacente = true;
                                }
                                else
                                {
                                    adyacente = false;
                                }
                            }
                            break;
                        case 9:
                            if (objController.IdComuna == "09101" || objController.IdComuna == "09112")
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj); adyacente = true;
                                }
                                else
                                {
                                    adyacente = false;
                                }
                            }
                            break;
                        case 10:
                            if (obj.IdComuna == "10101"
                  || objController.IdComuna == "10107"
                  || objController.IdComuna == "10105"
                  || objController.IdComuna == "10109")
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj); adyacente = true;
                                }
                                else
                                {
                                    adyacente = false;
                                }
                            }
                            break;
                        //case 11: /* hace algo*/; break;
                        //case 12: /* hace algo*/; break;
                        case 13:
                            if (objController.IdComuna == "13101"
                            || objController.IdComuna == "13130"
                            || objController.IdComuna == "13129"
                            || objController.IdComuna == "13131"
                            || objController.IdComuna == "13109"
                            || objController.IdComuna == "13401"
                            || objController.IdComuna == "13201"
                            || objController.IdComuna == "13111"
                            || objController.IdComuna == "13112"
                            || objController.IdComuna == "13110"
                            || objController.IdComuna == "13122"
                            || objController.IdComuna == "13118"
                            || objController.IdComuna == "13120"
                            || objController.IdComuna == "13113"
                            || objController.IdComuna == "13114"
                            || objController.IdComuna == "13123"
                            || objController.IdComuna == "13104"
                            || objController.IdComuna == "13125"
                            || objController.IdComuna == "13128"
                            || objController.IdComuna == "13117"
                            || objController.IdComuna == "13103"
                            || objController.IdComuna == "13126"
                            || objController.IdComuna == "13124"
                            || objController.IdComuna == "13106"
                            || objController.IdComuna == "13119"
                            || objController.IdComuna == "13102"
                            || objController.IdComuna == "13105"
                            || objController.IdComuna == "13127"
                            || objController.IdComuna == "13132"
                            || objController.IdComuna == "13116"
                            || objController.IdComuna == "13108"
                            || objController.IdComuna == "13121"
                            || objController.IdComuna == "13107"
                            || objController.IdComuna == "13115"
                            || objController.IdComuna == "13302"
                            || objController.IdComuna == "13403"
                            || objController.IdComuna == "13604"
                            || objController.IdComuna == "13605"
                            || objController.IdComuna == "13602"
                            || objController.IdComuna == "13603"
                            || objController.IdComuna == "13402"
                            || objController.IdComuna == "13404"
                            || objController.IdComuna == "13202"
                            || objController.IdComuna == "13301")
                            {
                                if (obj.Dias100 + obj.Dias60 + obj.Dias40 > 0)
                                {
                                    response.Warnings.Add("El destino señalado es una localidad adyacente, por lo tanto no le corresponde viatico");
                                    metodoMensaje(obj); adyacente = true;
                                }
                                else
                                {
                                    adyacente = false;
                                }
                            }
                            break;
                            //case 14: /* hace algo*/; break;
                            //case 15: /* hace algo*/; break;
                            //case 16: /* hace algo*/; break;

                    }
                }

                /*Se valida que la cantidad de dias solicitados correspondan con los viaticos*/
                if (adyacente == false)
                {
                    var diasAdyacencia = (objController.FechaHasta - objController.FechaInicio).Days + 1;
                    var cantAdyacenciaNew = obj.Dias100Aprobados + obj.Dias60Aprobados + obj.Dias40Aprobados + obj.Dias00Aprobados + obj.Dias50Aprobados;
                    var cantAdyacenciaOld = objController.Dias100 + objController.Dias60 + objController.Dias40 + objController.Dias00 + objController.Dias50;
                    if (objController.EditGP)
                    {
                        if (diasAdyacencia != cantAdyacenciaNew)
                            response.Errors.Add("La cantidad de días solicitados debe(n) ser " + diasAdyacencia + " en total");
                    }
                    else
                    {
                        if (diasAdyacencia != cantAdyacenciaOld)
                        {
                            if (objController.FechaInicio.Hour < 17)
                            {
                                response.Errors.Add("La cantidad de días solicitados debe(n) ser " + diasAdyacencia + " en total");
                            }
                        }
                    }
                }

                /*se valida que los rangos de fecha no se topen con otros destinos esto no aplica en la tarea de edit GP*/
                if (objController.EditGP != true)
                {
                    foreach (var destinos in _repository.Get<Destinos>(d => d.CometidoId != null && d.DestinoActivo == true))
                    {
                        var solicitanteDestino = _repository.Get<Cometido>(c => c.CometidoId == destinos.CometidoId).FirstOrDefault().NombreId;
                        var solicitante = _repository.Get<Cometido>(c => c.CometidoId == obj.CometidoId).FirstOrDefault().NombreId;

                        if (solicitanteDestino == solicitante)
                        {
                            if (destinos.FechaInicio.Date == objController.FechaInicio.Date)
                            {
                                if (destinos.DestinoId != objController.DestinoId)
                                {
                                    if (destinos.Dias40 > 0 && destinos.Dias100 == 0 && destinos.Dias60 == 0)
                                    {
                                        if (obj.Dias100 > 0 || obj.Dias40 > 0)
                                        {
                                            response.Warnings.Add("Ya se ha solicitado viatico para las fechas señaladas");
                                            obj.Dias100 = 0;
                                            obj.Dias60 = 0;
                                            obj.Dias40 = 0;
                                            obj.Total = 0;
                                            obj.TotalViatico = 0;
                                        }
                                    }

                                    if (destinos.Dias40 == 0 && destinos.Dias100 == 0 && destinos.Dias60 > 0)
                                    {
                                        if (obj.Dias100 > 0 || obj.Dias60 > 0)
                                        {
                                            response.Warnings.Add("Ya se ha solicitado viatico para las fechas señaladas");
                                            obj.Dias100 = 0;
                                            obj.Dias60 = 0;
                                            obj.Dias40 = 0;
                                            obj.Total = 0;
                                            obj.TotalViatico = 0;
                                        }
                                    }

                                    if (destinos.Dias40 == 0 && destinos.Dias100 > 0 && destinos.Dias60 == 0)
                                    {
                                        response.Warnings.Add("Ya se ha solicitado viatico para las fechas señaladas");
                                        obj.Dias100 = 0;
                                        obj.Dias60 = 0;
                                        obj.Dias40 = 0;
                                        obj.Total = 0;
                                        obj.TotalViatico = 0;
                                    }
                                }
                            }
                        }
                    }

                    /*se igualan los dias solicitados con los aprobados, mientras no se encuentre en la edicion de GP*/
                    obj.Dias100Aprobados = objController.Dias100;
                    obj.Dias60Aprobados = objController.Dias60;
                    obj.Dias40Aprobados = objController.Dias40;
                    obj.Dias50Aprobados = objController.Dias50;
                    obj.Dias00Aprobados = objController.Dias00;
                }

                if (response.IsValid)
                {

                    obj.FechaInicio = objController.FechaInicio;
                    obj.FechaHasta = objController.FechaHasta;

                    obj.OrigenRegion = objController.OrigenRegion;
                    obj.IdOrigenRegion = objController.IdOrigenRegion;
                    obj.FechaOrigen = objController.FechaOrigen;
                    obj.ObsOrigen = objController.ObsOrigen;
                    obj.ObsDestino = objController.ObsDestino;

                    obj.IdRegion = objController.IdRegion;
                    obj.IdComuna = objController.IdComuna;
                    obj.LocalidadId = objController.LocalidadId;

                    obj.ObservacionesModificacion = objController.ObservacionesModificacion;

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
        public ResponseMessage DestinosDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Destinos>(id);
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
        public ResponseMessage DestinosAnular(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Destinos>(id);
                if (obj == null)
                    response.Errors.Add("Dato no encontrado");
                else
                    obj.DestinoActivo = false;

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

        public ResponseMessage ViaticoInsert(Viatico obj)
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
        public ResponseMessage ViaticoUpdate(Viatico obj)
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
        public ResponseMessage ViaticoDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Viatico>(id);
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

        public ResponseMessage ViaticoHonorarioInsert(ViaticoHonorario obj)
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
        public ResponseMessage ViaticoHonorarioUpdate(ViaticoHonorario obj)
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
        public ResponseMessage ViaticoHonorarioDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<ViaticoHonorario>(id);
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

        public ResponseMessage TipoAsignacionInsert(TipoAsignacion obj)
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
        public ResponseMessage TipoAsignacionUpdate(TipoAsignacion obj)
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
        public ResponseMessage TipoAsignacionDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<TipoAsignacion>(id);
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

        public ResponseMessage TipoCapituloInsert(TipoCapitulo obj)
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
        public ResponseMessage TipoCapituloUpdate(TipoCapitulo obj)
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
        public ResponseMessage TipoCapituloDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<TipoCapitulo>(id);
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

        public ResponseMessage TipoItemInsert(TipoItem obj)
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
        public ResponseMessage TipoItemUpdate(TipoItem obj)
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
        public ResponseMessage TipoItemDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<TipoItem>(id);
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

        public ResponseMessage TipoPartidaInsert(TipoPartida obj)
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
        public ResponseMessage TipoPartidaUpdate(TipoPartida obj)
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
        public ResponseMessage TipoPartidaDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<TipoPartida>(id);
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

        public ResponseMessage TipoSubAsignacionInsert(TipoSubAsignacion obj)
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
        public ResponseMessage TipoSubAsignacionUpdate(TipoSubAsignacion obj)
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
        public ResponseMessage TipoSubAsignacionDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<TipoSubAsignacion>(id);
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

        public ResponseMessage TipoSubTituloInsert(TipoSubTitulo obj)
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
        public ResponseMessage TipoSubTituloUpdate(TipoSubTitulo obj)
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
        public ResponseMessage TipoSubTituloDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<TipoSubTitulo>(id);
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

        public ResponseMessage CentroCostoInsert(CentroCosto obj)
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
        public ResponseMessage CentroCostoUpdate(CentroCosto obj)
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
        public ResponseMessage CentroCostoDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<CentroCosto>(id);
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

        public ResponseMessage GeneracionCDPInsert(GeneracionCDP obj)
        {
            var response = new ResponseMessage();

            /*Se valida que ppto sea mayor que el compromiso*/
            if (int.Parse(obj.VtcPptoTotal) < int.Parse(obj.VtcCodCompromiso))
                response.Errors.Add("El presupuesto debe ser mayor que el compromiso");
            if (!obj.VtcTipoPartidaId.HasValue)
                response.Errors.Add("Se debe ingresar el tipo de partida");
            if (!obj.VtcTipoCapituloId.HasValue)
                response.Errors.Add("Se debe ingresar el tipo de capitulo");
            if (!obj.VtcCentroCostoId.HasValue)
                response.Errors.Add("Se debe ingresar el programa");
            if (!obj.VtcTipoSubTituloId.HasValue)
                response.Errors.Add("Se debe ingresar el tipo de subtitulo");
            if (!obj.VtcTipoItemId.HasValue)
                response.Errors.Add("Se debe ingresar el tipo de item");
            if (!obj.VtcTipoAsignacionId.HasValue)
                response.Errors.Add("Se debe ingresar el tipo de asignacion");
            if (!obj.VtcTipoSubAsignacionId.HasValue)
                response.Errors.Add("Se debe ingresar el tipo de subasignacion");

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
        public ResponseMessage GeneracionCDPUpdate(GeneracionCDP obj)
        {
            var response = new ResponseMessage();

            /*Se valida que ppto sea mayor que el compromiso*/
            if (int.Parse(obj.VtcPptoTotal) < int.Parse(obj.VtcCodCompromiso))
                response.Errors.Add("El presupuesto debe ser mayor que el compromiso");
            if (!obj.VtcTipoPartidaId.HasValue)
                response.Errors.Add("Se debe ingresar el tipo de partida");
            if (!obj.VtcTipoCapituloId.HasValue)
                response.Errors.Add("Se debe ingresar el tipo de capitulo");
            if (!obj.VtcCentroCostoId.HasValue)
                response.Errors.Add("Se debe ingresar el programa");
            if (!obj.VtcTipoSubTituloId.HasValue)
                response.Errors.Add("Se debe ingresar el tipo de subtitulo");
            if (!obj.VtcTipoItemId.HasValue)
                response.Errors.Add("Se debe ingresar el tipo de item");
            if (!obj.VtcTipoAsignacionId.HasValue)
                response.Errors.Add("Se debe ingresar el tipo de asignacion");
            if (!obj.VtcTipoSubAsignacionId.HasValue)
                response.Errors.Add("Se debe ingresar el tipo de subasignacion");

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
        public ResponseMessage GeneracionCDPDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<GeneracionCDP>(id);
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

        public ResponseMessage GeneracionCDPAnular(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<GeneracionCDP>(id);
                if (obj == null)
                    response.Errors.Add("Dato no encontrado");
                else
                    obj.GeneracionCDPActivo = false;

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
        public ResponseMessage ParrafosInsert(Parrafos obj)
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
        public ResponseMessage ParrafosUpdate(Parrafos obj)
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
        public ResponseMessage ParrafosDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Parrafos>(id);
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

        public ResponseMessage PasajeInsert(Pasaje obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (response.IsValid)
                {
                    if (obj.NombreId.HasValue)
                    {
                        if (string.IsNullOrEmpty(obj.Nombre))
                        {
                            var nombre = _sigper.GetUserByRut(obj.NombreId.Value).Funcionario.PeDatPerChq;
                            obj.Nombre = nombre.Trim();
                        }
                    }

                    _repository.Create(obj);
                    _repository.Save();
                    response.EntityId = obj.PasajeId;
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage PasajeUpdate(Pasaje obj)
        {
            var response = new ResponseMessage();

            try
            {
                /*se buscan los destinos asociados al pasaje*/
                var ListaDestinos = _repository.Get<DestinosPasajes>(c => c.PasajeId == obj.PasajeId).ToList();
                /*se valida que pasaje tenga un destino asociado*/
                if (ListaDestinos.Count == 0)
                {
                    response.Errors.Add("Se debe agregar por lo menos un destino al pasaje");
                }
                //else
                //{
                //    ///*validacion de fechas */
                //    //if (ListaDestinos.LastOrDefault().FechaVuelta < ListaDestinos.FirstOrDefault().FechaIda)
                //    //{
                //    //    response.Errors.Add("La fecha de termino no pueder ser mayor a la fecha de inicio");
                //    //}
                //}

                //if (obj.TipoDestino == true)
                //   {
                //       if (obj.IdComunaOrigen != null)
                //       {
                //           if(string.IsNullOrEmpty(obj.OrigenComunaDescripcion))
                //           {
                //               var comuna = _sigper.GetDGCOMUNAs().FirstOrDefault(c => c.Pl_CodCom == obj.IdComunaOrigen.ToString()).Pl_DesCom.Trim();
                //               obj.OrigenComunaDescripcion = comuna;
                //           }                        
                //       }
                //       else
                //       {
                //           response.Errors.Add("Se debe señalar la comuna de origen");
                //       }

                //       if (obj.IdRegionOrigen != null)
                //       {
                //           if(string.IsNullOrEmpty(obj.OrigenRegionDescripcion))
                //           {
                //               var region = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == obj.IdRegionOrigen.ToString()).Pl_DesReg.Trim();
                //               obj.OrigenRegionDescripcion = region;
                //           }                        
                //       }
                //       else
                //       {
                //           response.Errors.Add("Se debe señalar la region de origen");
                //       }
                //   }
                //   else
                //   {
                //       if (obj.IdCiudadOrigen != null)
                //       {
                //           if (string.IsNullOrEmpty(obj.OrigenCiudadDescripcion))
                //           {
                //               var ciudad = _repository.Get<Ciudad>().FirstOrDefault(c => c.CiudadId == int.Parse(obj.IdCiudadOrigen)).CiudadNombre.Trim();
                //               obj.OrigenCiudadDescripcion = ciudad;
                //           }                        
                //       }
                //       else
                //       {
                //           response.Errors.Add("Se debe señalar la ciudad de origen");
                //       }

                //       if (obj.IdPaisOrigen != null)
                //       {
                //           if(string.IsNullOrEmpty(obj.OrigenPaisDescripcion))
                //           {
                //               var pais = _repository.Get<Pais>().FirstOrDefault(c => c.PaisId == int.Parse(obj.IdPaisOrigen)).PaisNombre.Trim();
                //               obj.OrigenPaisDescripcion = pais;
                //           }                        
                //       }
                //       else
                //       {
                //           response.Errors.Add("Se debe señalar el pais de origen");
                //       }
                //   }

                if (response.IsValid)
                {
                    if (obj.NombreId.HasValue)
                    {
                        if (string.IsNullOrEmpty(obj.Nombre))
                        {
                            //var nombre = _sigper.GetUserByRut(obj.NombreId.Value).Funcionario.PeDatPerChq;
                            //obj.Nombre = nombre.Trim();
                        }
                        obj.PasajeOk = true;
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
        public ResponseMessage PasajeDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Pasaje>(id);
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

        public ResponseMessage ComisionesInsert(Comisiones obj)
        {
            var response = new ResponseMessage();

            try
            {
                /*validaciones*/
                if (obj.Vehiculo && obj.TipoVehiculoId == 0)
                {
                    response.Errors.Add("Se ha selecionado la opcion de vehiculo, por lo tanto debe señalar el tipo de vehiculo");
                }

                if (_repository.GetExists<DestinosComision>())
                {
                    /*validacion de cantidad de dias al 100% dentro del mes*/
                    if (_repository.Get<DestinosComision>(c => c.Comisiones.NombreId == obj.NombreId && c.Comisiones.FechaSolicitud.Month == DateTime.Now.Month).Sum(d => d.Dias100) > 10)
                    {
                        response.Errors.Add("La cantidad de dias al 100%, supera el maximo permitido para el mes señalado");
                    }
                    /*validacion de cantidad de dias al 100% dentro del año*/
                    if (_repository.Get<DestinosComision>(c => c.Comisiones.NombreId == obj.NombreId && c.Comisiones.FechaSolicitud.Year == DateTime.Now.Year).Sum(d => d.Dias100) > 90)
                    {
                        response.Errors.Add("La cantidad de dias al 100%, supera el maximo permitido para el año señalado");
                    }
                }

                if (string.IsNullOrEmpty(obj.ComisionDescripcion))
                {
                    response.Errors.Add("Debe ingresar la descripción de la comision.");
                }

                if (obj.Vehiculo && obj.TipoVehiculoId == 0)
                {
                    response.Errors.Add("Se ha selecionado la opcion de vehiculo, por lo tanto debe señalar el tipo de vehiculo");
                }

                if (string.IsNullOrEmpty(obj.ConglomeradoDescripcion))
                    obj.IdConglomerado = Convert.ToInt32(obj.ConglomeradoDescripcion);

                if (!string.IsNullOrEmpty(obj.ProgramaDescripcion))
                {
                    var pro = _sigper.GetREPYTs().Where(c => c.RePytDes.Trim() == obj.ProgramaDescripcion.Trim()).FirstOrDefault().RePytCod;
                    obj.IdPrograma = int.Parse(pro.ToString());
                }

                if (obj.IdGrado == "0" && !string.IsNullOrEmpty(obj.GradoDescripcion))
                {
                    obj.IdGrado = obj.GradoDescripcion;
                }


                if (obj.Vehiculo)
                {
                    if (obj.TipoVehiculoId.HasValue)
                    {
                        var vehiculo = _repository.Get<SIGPERTipoVehiculo>().Where(q => q.SIGPERTipoVehiculoId == obj.TipoVehiculoId).FirstOrDefault().Vehiculo.Trim();
                        if (!string.IsNullOrEmpty(vehiculo))
                            obj.TipoVehiculoDescripcion = vehiculo.Trim();
                    }
                    else
                    {
                        response.Errors.Add("Se debe señalar el tipo de vehiculo.");
                    }

                    if (string.IsNullOrEmpty(obj.PlacaVehiculo))
                        response.Errors.Add("Se debe señalar la placa patente del vehiculo.");
                }


                if (response.IsValid)
                {
                    if (obj.NombreId.HasValue)
                    {
                        var nombre = _sigper.GetUserByRut(obj.NombreId.Value).Funcionario.PeDatPerChq;
                        obj.Nombre = nombre.Trim();
                        obj.FechaSolicitud = DateTime.Now;
                    }

                    obj.ComisionesOk = false;

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
        public ResponseMessage ComisionesUpdate(Comisiones obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (_repository.Get<DestinosComision>(c => c.Comisiones.NombreId == obj.NombreId && c.Comisiones.FechaSolicitud.Month == DateTime.Now.Month).Sum(d => d.Dias100) > 10)
                {
                    response.Errors.Add("La cantidad de dias al 100%, supera el maximo permitido para el mes señalado");
                }

                if (_repository.Get<DestinosComision>(c => c.Comisiones.NombreId == obj.NombreId && c.Comisiones.FechaSolicitud.Year == DateTime.Now.Year).Sum(d => d.Dias100) > 90)
                {
                    response.Errors.Add("La cantidad de dias al 100%, supera el maximo permitido para el año señalado");
                }

                /*se buscan los destinos asociados al cometido*/
                var destinosCometido = _repository.Get<DestinosComision>(c => c.ComisionesId == obj.ComisionesId).ToList();
                /*se valida que cometido tenga un destino asociado*/
                if (destinosCometido.Count == 0)
                {
                    response.Errors.Add("Se debe agregar por lo menos un destino al cometido");
                }
                else
                {
                    /*validacion de fechas */
                    if (destinosCometido.LastOrDefault().FechaHasta < destinosCometido.FirstOrDefault().FechaInicio)
                    {
                        response.Errors.Add("La fecha de termino no pueder ser mayor a la fecha de inicio");
                    }
                    /*se valida que si existe mas de un destino solo se asigna un viatico*/
                    if (destinosCometido.Count > 1)
                    {
                        //response.Errors.Add("Usted ha ingresado más de un destino para el cometido, pero solo se le asignara un viático");
                        response.Warnings.Add("Usted ha ingresado más de un destino para la comision, pero solo se le asignara un viático");
                    }
                    /*valida ingreso de solicitud*/
                    //if ((int)Util.Enum.Cometidos.DiasAnticipacionIngreso > (ListaDestinos.FirstOrDefault().FechaInicio.Date - DateTime.Now.Date).Days)
                    //{
                    //    response.Errors.Add("La solicitud de cometido se debe realizar con una anticipacion de:" + (int)Util.Enum.Cometidos.DiasAnticipacionIngreso + " " + "dias.");                        
                    //}

                    /*se valida que los rangos de fecha no se topen con otros destrinos*/
                    //foreach (var destinos in _repository.Get<Destinos>(d => d.CometidoId == obj.CometidoId))
                    foreach (var otrosDestinos in _repository.Get<DestinosComision>())
                    {
                        if (otrosDestinos.FechaInicio == destinosCometido.FirstOrDefault().FechaInicio && otrosDestinos.ComisionesId != destinosCometido.FirstOrDefault().ComisionesId)
                        {
                            //response.Errors.Add("El rango de fechas señalados esta en conflicto con otros destinos");
                            response.Errors.Add(string.Format("El rango de fechas señalados esta en conflicto con los destinos de comisión {0}, inicio {1}, término {2}", otrosDestinos.ComisionesId, otrosDestinos.FechaInicio, otrosDestinos.FechaHasta));
                        }
                    }
                }

                if (string.IsNullOrEmpty(obj.ConglomeradoDescripcion))
                    obj.IdConglomerado = Convert.ToInt32(obj.ConglomeradoDescripcion);

                if (!string.IsNullOrEmpty(obj.ProgramaDescripcion) && obj.IdPrograma == null)
                {
                    var prog = _sigper.GetREPYTs().FirstOrDefault(c => c.RePytDes.Trim() == obj.ProgramaDescripcion.Trim());
                    if (prog != null)
                        obj.IdPrograma = int.Parse(prog.RePytCod.ToString());
                }

                if (obj.Vehiculo)
                {
                    if (obj.TipoVehiculoId.HasValue)
                    {
                        var vehiculo = _repository.Get<SIGPERTipoVehiculo>().Where(q => q.SIGPERTipoVehiculoId == obj.TipoVehiculoId).FirstOrDefault().Vehiculo.Trim();
                        if (string.IsNullOrEmpty(vehiculo))
                            obj.TipoVehiculoDescripcion = vehiculo.Trim();
                    }
                    else
                    {
                        response.Errors.Add("Se debe señalar el tipo de vehiculo.");
                    }
                }

                if (obj.IdGrado == "0" && !string.IsNullOrEmpty(obj.GradoDescripcion))
                {
                    obj.IdGrado = obj.GradoDescripcion;
                }


                if (response.IsValid)
                {
                    obj.ComisionesOk = true;

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
        public ResponseMessage ComisionesDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Comisiones>(id);
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

        public ResponseMessage DestinosComisionInsert(DestinosComision obj)
        {
            var response = new ResponseMessage();

            try
            {
                var comision = _repository.GetFirst<Comisiones>(c => c.ComisionesId == obj.ComisionesId);
                //reglas de negocio
                /*validacion de fechas*/
                if (obj.FechaInicio == null)
                {
                    response.Errors.Add("La fecha de inicio no es válida.");
                }
                if (obj.FechaHasta == null)
                {
                    response.Errors.Add("La fecha de término no es válida.");
                }

                /*se valida que la fecha de inicio no se superior que la de termino*/
                if (obj.FechaHasta < obj.FechaInicio)
                {
                    response.Errors.Add("La fecha de inicio no puede ser superior o igual a la de término");
                }

                if (obj.IdPais != null)
                    obj.PaisDescripcion = _repository.Get<Pais>().Where(c => c.PaisId.ToString() == obj.IdPais).FirstOrDefault().PaisNombre;

                if (obj.IdCiudad != null)
                    obj.CiudadDescripcion = _repository.Get<Ciudad>().Where(c => c.CiudadId.ToString() == obj.IdCiudad).FirstOrDefault().CiudadNombre;

                /*Se valida la cantidad de dias al 100% dentro del mes, no puede superar los 10 dias. Y dentro del año no puede superar los 90 dias*/
                if (obj.Dias100 > 0)
                {
                    int Totaldias100Mes = 0;
                    int Totaldias100Ano = 0;
                    var mes = DateTime.Now.Month;
                    var year = DateTime.Now.Year;
                    foreach (var destinos in _repository.Get<DestinosComision>(d => d.ComisionesId != null))
                    {
                        var solicitanteDestino = _repository.Get<Comisiones>(c => c.ComisionesId == destinos.ComisionesId).FirstOrDefault().NombreId;
                        var solicitante = _repository.Get<Comisiones>(c => c.ComisionesId == obj.ComisionesId).FirstOrDefault().NombreId;

                        if (solicitanteDestino == solicitante)
                        {
                            if (destinos.FechaInicio.Month == mes && destinos.Dias100 != 0 && destinos.Dias100 != null)
                            {
                                Totaldias100Mes += destinos.Dias100.Value;
                            }
                            if (destinos.FechaInicio.Year == year && destinos.Dias100 != 0 && destinos.Dias100 != null)
                            {
                                Totaldias100Ano += destinos.Dias100.Value;
                            }
                        }
                    }
                    if (Totaldias100Mes + obj.Dias100 > 10)
                    {
                        response.Errors.Add("Se ha excedido en: " + (Totaldias100Mes + obj.Dias100.Value - 10) + " la cantidad permitida de dias solicitados al 100%, dentro del Mes");
                    }
                    if (Totaldias100Ano + obj.Dias100 > 90)
                    {
                        response.Errors.Add("Se ha excedido en :" + (Totaldias100Ano + obj.Dias100 - 90) + " la cantidad permitida de dias solicitados al 100%, dentro de un año");
                    }
                }

                /*se valida que los rangos de fecha no se topen con otros destinos*/
                //var ListaDestinos = _repository.Get<Destinos>(c => c.CometidoId == obj.CometidoId).ToList();
                foreach (var destinos in _repository.Get<DestinosComision>(d => d.ComisionesId != null))
                {
                    var solicitanteDestino = _repository.Get<Comisiones>(c => c.ComisionesId == destinos.ComisionesId).FirstOrDefault().NombreId;
                    var solicitante = _repository.Get<Comisiones>(c => c.ComisionesId == obj.ComisionesId).FirstOrDefault().NombreId;

                    if (solicitanteDestino == solicitante)
                    {
                        if (destinos.FechaInicio.Date == obj.FechaInicio.Date)
                        {
                            response.Errors.Add("El rango de fechas señalados esta en conflicto con otros destinos");
                        }
                    }
                }

                if (comision.SolicitaViatico == false)
                {
                    response.Warnings.Add("No se ha solicitado viatico para la comision");
                    obj.Dias100 = 0;
                    obj.Dias60 = 0;
                    obj.Dias40 = 0;
                    obj.Dias100Aprobados = 0;
                    obj.Dias60Aprobados = 0;
                    obj.Dias40Aprobados = 0;
                    obj.Dias100Monto = 0;
                    obj.Dias60Monto = 0;
                    obj.Dias40Monto = 0;
                    obj.TotalViatico = 0;
                    obj.Total = 0;
                }
                if (obj.IdCiudad != null)
                {
                    var ciudad = _repository.Get<Ciudad>().FirstOrDefault(c => c.CiudadId == int.Parse(obj.IdCiudad)).CiudadNombre.Trim();
                    obj.CiudadDescripcion = ciudad;
                }
                else
                {
                    response.Errors.Add("Se debe señalar la ciudad de destino");
                }

                if (obj.IdPais != null)
                {
                    var pais = _repository.Get<Pais>().FirstOrDefault(c => c.PaisId == int.Parse(obj.IdPais)).PaisNombre.Trim();
                    obj.PaisDescripcion = pais;
                }
                else
                {
                    response.Errors.Add("Se debe señalar el pais de destino");
                }
                if (obj.Dias00 == null)
                    obj.Dias00 = 0;
                if (obj.Dias50 == null)
                    obj.Dias50 = 0;

                /*Se dejan los mismo valores de los solicitados como aprobados en la creacion del destino*/
                obj.Dias100Aprobados = obj.Dias100;
                obj.Dias60Aprobados = obj.Dias60;
                obj.Dias40Aprobados = obj.Dias40;
                obj.Dias50Aprobados = obj.Dias50;
                obj.Dias00Aprobados = obj.Dias00;


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
        public ResponseMessage DestinosComisionUpdate(DestinosComision obj)
        {
            var response = new ResponseMessage();

            try
            {
                //reglas de negocio
                /*validacion de fechas*/
                if (obj.FechaInicio == null)
                {
                    response.Errors.Add("La fecha de inicio no es válida.");
                }
                if (obj.FechaHasta == null)
                {
                    response.Errors.Add("La fecha de término no es válida.");
                }

                //if (obj.IdPais != null)
                //    obj.PaisDescripcion = _repository.Get<Pais>().Where(c => c.PaisId.ToString() == obj.IdPais).FirstOrDefault().PaisNombre;

                //if (obj.IdCiudad != null)
                //    obj.CiudadDescripcion = _repository.Get<Ciudad>().Where(c => c.CiudadId.ToString() == obj.IdCiudad).FirstOrDefault().CiudadNombre;

                /*se valida que la cantidad de dias sea igual que lo solicitado por cada destino ingresado*/
                var dias = (obj.FechaHasta - obj.FechaInicio).Days + 1;
                var cant = obj.Dias100Aprobados + obj.Dias60Aprobados + obj.Dias40Aprobados + obj.Dias00Aprobados + obj.Dias50Aprobados;
                if (dias != cant)
                {
                    response.Errors.Add("la cantidad de dias no coincide con los viaticos solicitados");
                }
                /*se valida que la fecha de inicio no se superior que la de termino*/
                if (obj.FechaHasta < obj.FechaInicio)
                {
                    response.Errors.Add("La fecha de inicio no puede ser superior o igual a la de término");
                }

                var comision = _repository.GetFirst<Comisiones>(c => c.ComisionesId == obj.ComisionesId);
                if (comision.SolicitaViatico == false)
                {
                    response.Warnings.Add("Este cometido tendrá un viático de $0");
                    obj.Dias100 = 0;
                    obj.Dias60 = 0;
                    obj.Dias40 = 0;
                    obj.Dias100Aprobados = 0;
                    obj.Dias60Aprobados = 0;
                    obj.Dias40Aprobados = 0;
                    obj.Dias100Monto = 0;
                    obj.Dias60Monto = 0;
                    obj.Dias40Monto = 0;
                    obj.TotalViatico = 0;
                    obj.Total = 0;
                }

                if (obj.IdCiudad != null)
                {
                    var ciudad = _repository.Get<Ciudad>().FirstOrDefault(c => c.CiudadId == int.Parse(obj.IdCiudad)).CiudadNombre.Trim();
                    obj.CiudadDescripcion = ciudad;
                }
                else
                {
                    response.Errors.Add("Se debe señalar la ciudad de destino");
                }

                if (obj.IdPais != null)
                {
                    var pais = _repository.Get<Pais>().FirstOrDefault(c => c.PaisId == int.Parse(obj.IdPais)).PaisNombre.Trim();
                    obj.PaisDescripcion = pais;
                }
                else
                {
                    response.Errors.Add("Se debe señalar el pais de destino");
                }
                if (obj.Dias00 == null)
                    obj.Dias00 = 0;
                if (obj.Dias50 == null)
                    obj.Dias50 = 0;



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
        public ResponseMessage DestinosComisionDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<DestinosComision>(id);
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

        public ResponseMessage DestinosPasajesInsert(DestinosPasajes obj)
        {
            var response = new ResponseMessage();

            try
            {
                /*se valida que la fecha de inicio no se superior que la de termino*/
                //if (obj.FechaVuelta < obj.FechaIda)
                //{
                //    response.Errors.Add("La fecha de ida no puede ser superior a la fecha de salida");
                //}

                var pasaje = _repository.GetById<Pasaje>(obj.PasajeId);
                if (pasaje.TipoDestino == true)
                {
                    //if (obj.IdComuna != null)
                    //{
                    //    var comuna = _sigper.GetDGCOMUNAs().FirstOrDefault(c => c.Pl_CodCom == obj.IdComuna.ToString()).Pl_DesCom.Trim();
                    //    obj.ComunaDescripcion = comuna;
                    //}
                    //else
                    //{
                    //    response.Errors.Add("Se debe señalar la comuna de destino");
                    //}

                    if (obj.IdRegionOrigen != null)
                    {
                        if (string.IsNullOrEmpty(obj.OrigenRegionDescripcion))
                        {
                            var region = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == obj.IdRegionOrigen).Pl_DesReg.Trim();
                            obj.OrigenRegionDescripcion = region;
                        }
                    }
                    else
                    {
                        response.Errors.Add("Se debe señalar la region de destino");
                    }

                    if (obj.IdRegion != null)
                    {
                        if (string.IsNullOrEmpty(obj.RegionDescripcion))
                        {
                            var region = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == obj.IdRegion).Pl_DesReg.Trim();
                            obj.RegionDescripcion = region;
                        }
                    }
                    else
                    {
                        response.Errors.Add("Se debe señalar la region de destino");
                    }
                }
                else
                {
                    //if (obj.IdCiudad != null)
                    //{
                    //    var ciudad = _repository.Get<Ciudad>().FirstOrDefault(c => c.CiudadId == int.Parse(obj.IdCiudad)).CiudadNombre.Trim();
                    //    obj.CiudadDescripcion = ciudad;
                    //}
                    //else
                    //{
                    //    response.Errors.Add("Se debe señalar la ciudad de destino");
                    //}

                    if (obj.IdPaisOrigen != null)
                    {
                        if (string.IsNullOrEmpty(obj.OrigenPaisDescripcion))
                        {
                            var pais = _repository.Get<Pais>().FirstOrDefault(c => c.PaisId == int.Parse(obj.IdPaisOrigen)).PaisNombre.Trim();
                            obj.OrigenPaisDescripcion = pais;
                        }
                    }
                    else
                    {
                        response.Errors.Add("Se debe señalar el pais de destino");
                    }


                    if (obj.IdPais != null)
                    {
                        if (string.IsNullOrEmpty(obj.PaisDescripcion))
                        {
                            var pais = _repository.Get<Pais>().FirstOrDefault(c => c.PaisId == int.Parse(obj.IdPais)).PaisNombre.Trim();
                            obj.PaisDescripcion = pais;
                        }
                    }
                    else
                    {
                        response.Errors.Add("Se debe señalar el pais de destino");
                    }
                }

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
        public ResponseMessage DestinosPasajesUpdate(DestinosPasajes obj)
        {
            var response = new ResponseMessage();

            try
            {
                var pasaje = _repository.GetById<Pasaje>(obj.PasajeId);
                if (pasaje.TipoDestino == true)
                {
                    if (obj.IdRegionOrigen != null)
                    {
                        var region = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == obj.IdRegionOrigen).Pl_DesReg.Trim();
                        obj.OrigenRegionDescripcion = region;
                    }
                    else
                    {
                        response.Errors.Add("Se debe señalar la region de destino");
                    }

                    if (obj.IdRegion != null)
                    {
                        var region = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == obj.IdRegion).Pl_DesReg.Trim();
                        obj.RegionDescripcion = region;
                    }
                    else
                    {
                        response.Errors.Add("Se debe señalar la region de destino");
                    }
                }
                else
                {
                    if (obj.IdPaisOrigen != null)
                    {
                        var pais = _repository.Get<Pais>().FirstOrDefault(c => c.PaisId == int.Parse(obj.IdPaisOrigen)).PaisNombre.Trim();
                        obj.OrigenPaisDescripcion = pais;
                    }
                    else
                    {
                        response.Errors.Add("Se debe señalar el pais de destino");
                    }


                    if (obj.IdPais != null)
                    {
                        var pais = _repository.Get<Pais>().FirstOrDefault(c => c.PaisId == int.Parse(obj.IdPais)).PaisNombre.Trim();
                        obj.PaisDescripcion = pais;
                    }
                    else
                    {
                        response.Errors.Add("Se debe señalar el pais de destino");
                    }
                }

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
        public ResponseMessage DestinosPasajesDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<DestinosPasajes>(id);
                var pasaje = _repository.GetById<Pasaje>(obj.PasajeId);

                if (obj == null)
                    response.Errors.Add("Dato no encontrado");
                if (pasaje == null)
                    response.Errors.Add("Pasaje no encontrado");

                if (response.IsValid)
                {
                    _repository.Delete(obj);
                    _repository.Delete(pasaje);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public ResponseMessage GeneracionCDPComisionInsert(GeneracionCDPComision obj)
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
        public ResponseMessage GeneracionCDPComisionUpdate(GeneracionCDPComision obj)
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
        public ResponseMessage GeneracionCDPComisionDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<GeneracionCDPComision>(id);
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

        public ResponseMessage ParametrosComisionesInsert(ParametrosComisiones obj)
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
        public ResponseMessage ParametrosComisionesUpdate(ParametrosComisiones obj)
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
        public ResponseMessage ParametrosComisionesDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<ParametrosComisiones>(id);
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

        public ResponseMessage ParrafoComisionesInsert(ParrafoComisiones obj)
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
        public ResponseMessage ParrafoComisionesUpdate(ParrafoComisiones obj)
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
        public ResponseMessage ParrafoComisionesDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<ParrafoComisiones>(id);
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

        public ResponseMessage ViaticoInternacionalInsert(ViaticoInternacional obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (obj.PaisId != null)
                    obj.PaisNombre = _repository.Get<Pais>().Where(c => c.PaisId == obj.PaisId.Value).FirstOrDefault().PaisNombre;

                if (obj.CiudadId != null)
                    obj.CiudadNombre = _repository.Get<Ciudad>().Where(c => c.CiudadId == obj.CiudadId.Value).FirstOrDefault().CiudadNombre;

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
        public ResponseMessage ViaticoInternacionalUpdate(ViaticoInternacional obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (obj.PaisId != null)
                    obj.PaisNombre = _repository.Get<Pais>().Where(c => c.PaisId == obj.PaisId.Value).FirstOrDefault().PaisNombre;

                if (obj.CiudadId != null)
                    obj.CiudadNombre = _repository.Get<Ciudad>().Where(c => c.CiudadId == obj.CiudadId.Value).FirstOrDefault().CiudadNombre;

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
        public ResponseMessage ViaticoInternacionalDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<ViaticoInternacional>(id);
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

        //public ResponseMessage FirmaDocumentoInsert(FirmaDocumento obj)
        //{
        //    var response = new ResponseMessage();

        //    //validaciones

        //    if (response.IsValid)
        //    {
        //        _repository.Create(obj);
        //        _repository.Save();
        //    }

        //    return response;
        //}
        //public ResponseMessage FirmaDocumentoEdit(FirmaDocumento obj)
        //{
        //    var response = new ResponseMessage();

        //    //validaciones

        //    if (response.IsValid)
        //    {
        //        var ingreso = _repository.GetFirst<FirmaDocumento>(q => q.FirmaDocumentoId == obj.FirmaDocumentoId);
        //        if (ingreso != null)
        //        {
        //            ingreso.TipoDocumentoCodigo = obj.TipoDocumentoCodigo;
        //            ingreso.DocumentoSinFirma = obj.DocumentoSinFirma;
        //            ingreso.DocumentoSinFirmaFilename = obj.DocumentoSinFirmaFilename;
        //            ingreso.Firmado = false;

        //            _repository.Update(ingreso);
        //            _repository.Save();
        //        }
        //    }

        //    return response;
        //}
        //public ResponseMessage FirmaDocumentoFirmar(int id, string firmante)
        //{
        //    var response = new ResponseMessage();

        //    //validaciones...
        //    if (id == 0)
        //        response.Errors.Add("Documento a firmar no encontrado");
        //    if (string.IsNullOrWhiteSpace(firmante))
        //        response.Errors.Add("No se especificó el firmante");
        //    if (!string.IsNullOrWhiteSpace(firmante) && !_repository.GetExists<Rubrica>(q => q.Email == firmante && q.HabilitadoFirma))
        //        response.Errors.Add("No se encontró rúbrica habilitada para el firmante");

        //    var documento = _repository.GetById<FirmaDocumento>(id);
        //    if (documento == null)
        //        response.Errors.Add("Documento a firmar no encontrado");
        //    //if (documento != null && documento.Firmado)
        //    //    response.Errors.Add("Documento ya está firmado");

        //    if (response.IsValid)
        //    {
        //        //si el documento ya tiene folio no solicitarlo nuevamente
        //        if (string.IsNullOrWhiteSpace(documento.Folio))
        //        {
        //            try
        //            {
        //                var _folioResponse = _folio.GetFolio(firmante, documento.TipoDocumentoCodigo);
        //                if (_folioResponse == null)
        //                    throw new Exception("Servicio no entregó respuesta");

        //                if (_folioResponse != null && _folioResponse.status == "ERROR")
        //                    throw new Exception(_folioResponse.status);

        //                documento.Folio = _folioResponse.folio;

        //                _repository.Update(documento);
        //                _repository.Save();

        //            }
        //            catch (Exception ex)
        //            {
        //                response.Errors.Add("Error al obtener folio del documento:" + ex.Message);
        //            }
        //        }
        //    }

        //    //firmar documento
        //    if (response.IsValid)
        //    {
        //        try
        //        {
        //            var rubrica = _repository.GetFirst<Rubrica>(q => q.Email == firmante && q.HabilitadoFirma);
        //            var _hsmResponse = _hsm.Sign(documento.DocumentoSinFirma, rubrica.IdentificadorFirma, rubrica.UnidadOrganizacional, documento.Folio);

        //            documento.DocumentoConFirma = _hsmResponse;
        //            documento.DocumentoConFirmaFilename = "FIRMADO - " + documento.DocumentoSinFirmaFilename;
        //            documento.Firmante = firmante;
        //            documento.Firmado = true;
        //            documento.FechaFirma = DateTime.Now;

        //            _repository.Update(documento);
        //            _repository.Create(new Documento()
        //            {
        //                Proceso = documento.Proceso,
        //                Workflow = documento.Workflow,
        //                Fecha = DateTime.Now,
        //                Email = documento.Autor,
        //                FileName = documento.DocumentoConFirmaFilename,
        //                File = documento.DocumentoConFirma,
        //                Signed = true,
        //                Type = "application/pdf",
        //                TipoPrivacidadId = 1,
        //                TipoDocumentoId = 6
        //            });

        //            _repository.Save();
        //            //asociar el documento firmado al proceso
        //        }
        //        catch (Exception ex)
        //        {
        //            response.Errors.Add("Error al obtener folio del documento:" + ex.Message);
        //        }
        //    }

        //    return response;
        //}

        //metodos migrados desde usecase core

        public ResponseMessage WorkflowUpdate(Workflow obj, string userLoged)
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
                var definicionworkflowlist = _repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == 13 /*workflowActual.Proceso.DefinicionProcesoId*/).OrderBy(q => q.Secuencia).ThenBy(q => q.DefinicionWorkflowId) ?? null;
                if (!definicionworkflowlist.Any())
                    throw new Exception("No se encontró la definición de tarea del proceso asociado al workflow.");
                if (string.IsNullOrWhiteSpace(obj.Email))
                    throw new Exception("No se encontró el usuario que ejecutó el workflow.");
                if (workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar && (obj.TipoAprobacionId == null || obj.TipoAprobacionId == 0))
                    workflowActual.TipoAprobacionId = (int)Util.Enum.TipoAprobacion.Aprobada;
                /*Si la tarea es rechazada se valida que se ingrese una observacion - en el proceso cometido*/
                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                {
                    throw new Exception("Se debe señalar el motivo del rechazo para la tarea.");
                }

                /*generar tags de proceso*/
                workflowActual.Proceso.Tags += workflowActual.Proceso.GetTags();

                /*generar tags de negocio*/
                var comet = _repository.GetFirst<Cometido>(q => q.WorkflowId == workflowActual.WorkflowId);
                if (comet != null)
                    workflowActual.Proceso.Tags += comet.GetTags();


                /*Valida la carga de adjuntos segun el tipo de proceso*/ /*27082020 --> se agrega validacion en firma dr acto administrativo, que el documento debe ser firmado*/
                if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudPasaje)
                {
                    if (workflowActual.DefinicionWorkflow.Secuencia == 4)
                    {
                        //Se toma valor del pasaje para validar cuantos adjuntos se solicitan
                        var Pasaje = _repository.GetFirst<Pasaje>(q => q.WorkflowId == obj.WorkflowId);
                        if (Pasaje.TipoDestino == true)
                        {
                            if (workflowActual.Proceso.Documentos.Count < 1)
                            {
                                throw new Exception("Debe adjuntar a los menos una cotizaciones.");
                            }
                        }
                        else
                        {
                            if (workflowActual.Proceso.Documentos.Count < 3)
                            {
                                throw new Exception("Debe adjuntar a los menos tres cotizaciones.");
                            }
                        }
                    }
                    //else if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any())
                    //{
                    //    throw new Exception("Debe adjuntar a los menos tres cotizaciones.");
                    //}                                         
                }
                else if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                {
                    if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.IngresoCotizacion)
                    {
                        if (obj.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                        {
                            //Se toma valor del pasaje para validar cuantos adjuntos se solicitan
                            var Pasaje = _repository.GetFirst<Pasaje>(q => q.WorkflowId == obj.WorkflowId);
                            if (Pasaje != null)
                            {
                                var cotizacion = _repository.Get<Cotizacion>(c => c.PasajeId == Pasaje.PasajeId).LastOrDefault();
                                if (cotizacion != null)
                                {
                                    if (Pasaje.TipoDestino == true)
                                    {
                                        if (cotizacion.CotizacionDocumento.Count < 1)
                                        {
                                            throw new Exception("Debe adjuntar a los menos una cotizaciones.");
                                        }
                                    }
                                    else
                                    {
                                        if (cotizacion.CotizacionDocumento.Count < 3)
                                        {
                                            throw new Exception("Debe adjuntar a los menos tres cotizaciones.");
                                        }
                                    }
                                }
                                else
                                    throw new Exception("No se ha ingresado cotización.");
                            }
                        }
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AnalistaPresupuesto)
                    {
                        if (obj.TipoAprobacionId != (int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            if (comet.GeneracionCDP.Count == 0)
                            {
                                throw new Exception("Falta generar Certificado de Refrendación.");
                            }
                        }
                    }
                    else if(workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.EncargadoPresupuesto)
                    {
                        if(obj.TipoAprobacionId!=(int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            foreach(var item in comet.GeneracionCDP)
                            {
                                if (item.VtcIdCompromiso.IsNullOrWhiteSpace())
                                {
                                    throw new Exception("Falta ingresar el ID del Compromiso.");
                                }                              
                            }
                        }
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaActoAdministrativo
                        || workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaMinistro
                        || workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaSubsecretario)
                    {
                        if (obj.TipoAprobacionId != (int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            var doc = _repository.GetById<Documento>(workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == (int)Util.Enum.TipoDocumento.Resolucion).FirstOrDefault().DocumentoId).Signed;
                            if (doc == false)
                                throw new Exception("El documento del acto administrativo debe estar firmado electronicamente");

                            /*se valida si existe una resolucion revocatoria, esta se debe firmar*/
                            if (comet.ResolucionRevocatoria == true)
                            {
                                var res = _repository.GetById<Documento>(workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == (int)Util.Enum.TipoDocumento.ResolucionRevocatoriaCometido).FirstOrDefault().DocumentoId).Signed;
                                if (res == false)
                                    throw new Exception("El documento resolucion revocatoria debe estar firmado electronicamente");
                            }
                        }
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == 16)
                    {
                        if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any(c => c.TipoDocumentoId.Value == 4 && c.TipoDocumentoId != null))
                            throw new Exception("Debe adjuntar documentos en la tarea de analista de contabilidad.");
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == 18)
                    {
                        if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any(c => c.TipoDocumentoId.Value == 5 && c.TipoDocumentoId != null))
                            throw new Exception("Debe adjuntar documentos en la tarea de analista tesoreria.");
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == 17)
                    {
                        if (obj.TipoAprobacionId != (int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            var doc = _repository.GetById<Documento>(workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == 4).FirstOrDefault().DocumentoId).Signed;
                            if (doc == false)
                                throw new Exception("El documento cargado por el analista de contabilidad debe estar firmado electronicamente");
                        }
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == 19)
                    {
                        if (obj.TipoAprobacionId != (int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            var doc = _repository.GetById<Documento>(workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == 5).FirstOrDefault().DocumentoId).Signed;
                            if (doc == false)
                                throw new Exception("El documento cargado por el analista de tesorería debe estar firmado electronicamente");
                        }
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == 20)
                    {
                        if (obj.TipoAprobacionId != (int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            var doc = _repository.Get<Documento>(c => c.ProcesoId == workflowActual.ProcesoId && (c.TipoDocumentoId == 4 || c.TipoDocumentoId == 5));
                            if (doc.Any())
                            {
                                foreach (var d in doc)
                                {
                                    if (d.Signed != true)
                                        throw new Exception("Los documento cargados desde contabilidad y tesorería, deben estar firmado electronicamente por el encargado de finanzas");
                                }
                            }
                        }
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == 4)
                    {
                        if (obj.TipoAprobacionId != (int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            //Se toma valor del pasaje para validar cuantos adjuntos se solicitan
                            bool cotiza = false;
                            var Pasaje = _repository.GetFirst<Pasaje>(q => q.WorkflowId == obj.WorkflowId);
                            if (Pasaje != null)
                            {
                                #region OLD
                                /*var cotizacion = _repository.Get<Cotizacion>(c => c.PasajeId == Pasaje.PasajeId);
                                foreach (var c in cotizacion)
                                {
                                    if (c != null)
                                    {
                                        foreach (var d in c.CotizacionDocumento)
                                        {
                                            if (d.Selected)
                                                cotiza = true;
                                        }
                                    }
                                }*/
                                #endregion
                                var cotizacion = _repository.Get<Cotizacion>(c => c.PasajeId == Pasaje.PasajeId);
                                for (int i = 0; i < cotizacion.Count(); i++)
                                {
                                    var cotizacionDocumento = _repository.GetById<CotizacionDocumento>(cotizacion.ToArray()[i].CotizacionDocumento.FirstOrDefault().CotizacionDocumentoId);
                                    if(cotizacionDocumento.Selected && cotizacionDocumento.Type.Contains("pdf"))
                                    {
                                        cotiza = true;
                                    }
                                }

                            }

                            if (cotiza == false)
                                throw new Exception("Se debe seleccionar una cotizacion para los pasajes solicitados.");
                        }
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == 1)
                    {
                        var com = _repository.GetFirst<Cometido>(q => q.WorkflowId == obj.WorkflowId);
                        var pasaje = _repository.GetFirst<Pasaje>(q=> q.WorkflowId == obj.WorkflowId);
                        if (!com.Destinos.Any())
                            throw new Exception("Se deben ingresar destinos al cometido.");

                        var lista = new List<Destinos>();
                        for(int i =0; i < com.Destinos.Count(); i++)
                        {
                            var fecha = com.Destinos[i].FechaInicio.Date.Subtract(com.FechaSolicitud.Date).Days;
                            if(fecha<20)
                            {
                                lista.Add(com.Destinos[i]);
                            }
                        }
                        if(com.GradoDescripcion != "C" && com.GradoDescripcion!="B")
                        {
                            if (lista.Count > 0)
                            {
                                if (com.JustificacionAtraso.IsNullOrWhiteSpace())
                                {
                                    throw new Exception("Falta ingresar Justificación de Atraso");
                                }
                            }

                            if(com.JustificacionAtraso==string.Empty)
                            {
                                throw new Exception("Se debe ingresar justificación de atraso");
                            }
                        }

                        if(com.ReqPasajeAereo && pasaje==null)
                        {
                            throw new Exception("Se debe completar el formulario Datos del Pasaje.");
                        }                        

                        /*validar q se debe ingresar un documento en la solicitud.*/
                        if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any())
                            throw new Exception("Debe adjuntar documentos a la solicitud que se esta generando.");
                    }
                }
                else
                {
                    if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any())
                        throw new Exception("Debe adjuntar documentos.");
                }

                /*terminar workflow actual*/
                var personaEjecutor = _sigper.GetUserByEmail(userLoged);

                workflowActual.FechaTermino = DateTime.Now;
                workflowActual.Observacion = obj.Observacion;
                workflowActual.Terminada = true;
                //workflowActual.Pl_UndCod = obj.Pl_UndCod;
                //workflowActual.Pl_UndDes = obj.Pl_UndDes.Trim();                
                //workflowActual.Email = obj.Email;
                workflowActual.Pl_UndCod = personaEjecutor.Unidad.Pl_UndCod;
                workflowActual.Pl_UndDes = personaEjecutor.Unidad.Pl_UndDes.Trim();
                workflowActual.Email = personaEjecutor.Funcionario.Rh_Mail.Trim();
                workflowActual.NombreFuncionario = personaEjecutor.Funcionario.PeDatPerChq.Trim();
                workflowActual.GrupoId = obj.GrupoId;
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


                if (workflowActual.Proceso.DefinicionProceso.Entidad.Codigo == Util.Enum.Entidad.Cometido.ToString())
                {
                    //Se toma valor de cometidos para definir curso de accion del flujo
                    var Cometido = _repository.GetFirst<Cometido>(q => q.WorkflowId == obj.WorkflowId);
                    //Cometido = _repository.Get<Cometido>(q => q.ProcesoId == obj.ProcesoId).FirstOrDefault();
                    //if (Cometido.SolicitaViatico == true)
                    //    throw new Exception("Solicitud tiene viatico.");

                    /*Si el cometido es nulo pq viene d una tarea de pasaje, se busca por el proceso asociado entre el coemtido y el pasaje*/
                    if (Cometido == null)
                    {
                        var Pasaje = _repository.GetFirst<Pasaje>(q => q.WorkflowId == obj.WorkflowId);
                        Cometido = _repository.GetFirst<Cometido>(q => q.ProcesoId == Pasaje.ProcesoId);
                    }

                    //DETERRMINAR SIGUIENTE TAREA DESDE EL DISEÑO DE PROCESO
                    if (!workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                        if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                        {
                            if (Cometido != null)
                            {
                                if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje) /*validaciones de secuencia del proceso cometido con pasaje*/
                                {
                                    if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionJefaturaGP)
                                    {
                                        if (Cometido.ResolucionRevocatoria == true)/*si corresponde a una resolucion revocatoria se envia a firma de jefe depto adminstrativo*/
                                        {
                                            if (Cometido.IdEscalafon == 1)// || Cometido.IdEscalafon == null)
                                            {
                                                definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionJuridica);
                                            }
                                            else
                                                definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaActoAdministrativo);
                                        }                                            
                                        else
                                            definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 8); //10 /*workflowActual.DefinicionWorkflow.Secuencia*/);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == 13 || workflowActual.DefinicionWorkflow.Secuencia == 14 || workflowActual.DefinicionWorkflow.Secuencia == 15)
                                    {
                                        if(Cometido.JustificacionAtraso!=null)
                                        {
                                            if(Cometido.ReqPasajeAereo)
                                            {

                                            }
                                            else
                                            {
                                                definicionWorkflow=definicionworkflowlist.FirstOrDefault(q=>q.Secuencia==(int)Util.Enum.CometidoSecuencia.AprobacionDocGP);
                                            }
                                        }
                                        else
                                        {
                                            if (Cometido.SolicitaViatico != true || Cometido.TotalViatico == 0)
                                                definicionWorkflow = null;/*despues de la firma de resolucion, sino existe viatico el proceso finaliza*/
                                            else if (Cometido.ResolucionRevocatoria)
                                                definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 20);/*si corresponde a una resoucion revocatoria se envia a finanzas*/
                                            else
                                                definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 16);
                                        }
                                    }
                                    //else if (workflowActual.DefinicionWorkflow.Secuencia == 13 || workflowActual.DefinicionWorkflow.Secuencia == 14 || workflowActual.DefinicionWorkflow.Secuencia == 15 && Cometido.CalidadDescripcion != "TITULAR")/*Despues de la firma, si no es titular continua a contabilidad*/
                                    //{
                                    //    if (Cometido.ResolucionRevocatoria)
                                    //        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 20);/*si corresponde a una resoucion revocatoria se envia a finanzas*/
                                    //    else
                                    //        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 16);
                                    //}
                                    //else if (workflowActual.DefinicionWorkflow.Secuencia == 13  && Cometido.ResolucionRevocatoria == true)
                                    //{
                                    //    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 20);
                                    //}
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.IngresoPagoTesoreria/*20*/)
                                    {
                                        definicionWorkflow = null;  /*workflow se deja null para terminar el proceso*/
                                    }
                                    else if(workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.VisacionSubsecretaria)
                                    {
                                        if(Cometido.ReqPasajeAereo)
                                        {
                                            definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.IngresoCotizacion);
                                        }
                                        else
                                        {
                                            definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionDocGP);
                                        }
                                    }

                                    #region Secuencia hacia Jefatura Antiguo
                                    /*else if (workflowActual.DefinicionWorkflow.Secuencia == 1 *//*2*//* && Cometido.ReqPasajeAereo)
                                    {
                                        var pasaje = _repository.GetFirst<Pasaje>(q => q.ProcesoId == workflowActual.ProcesoId);
                                        if(pasaje != null)
                                        {
                                            definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 3); *//*si tiene pasaje se generan las tareas de solicitud de pasaje*//*
                                        }
                                        else
                                        {
                                            throw new Exception("Falta agregar pasaje");
                                        }                                   
                                        
                                        //genera registro en tabla de pasajes
                                        //Pasaje _pasaje = new Pasaje();
                                        //_pasaje.FechaSolicitud = DateTime.Now;
                                        //_pasaje.Nombre = Cometido.Nombre;
                                        //_pasaje.NombreId = Cometido.NombreId;
                                        //_pasaje.Rut = Cometido.Rut;
                                        //_pasaje.DV = Cometido.DV;
                                        //_pasaje.IdCalidad = Cometido.IdCalidad;
                                        //_pasaje.CalidadDescripcion = Cometido.CalidadDescripcion;
                                        //_pasaje.PasajeDescripcion = Cometido.CometidoDescripcion;
                                        //_pasaje.TipoDestino = true;
                                        //_pasaje.ProcesoId = Cometido.ProcesoId;
                                        //_pasaje.WorkflowId = Cometido.WorkflowId;
                                        //var resp = PasajeInsert(_pasaje);

                                        ///*genera resgistro en tabla destino pasaje, segun los destinos señalados en el cometido*/
                                    //foreach (var com in Cometido.Destinos)
                                    //{
                                    //    DestinosPasajes _destino = new DestinosPasajes();
                                    //    _destino.PasajeId = resp.EntityId;
                                    //    _destino.IdRegion = com.IdRegion;
                                    //    _destino.RegionDescripcion = com.RegionDescripcion;
                                    //    _destino.IdRegionOrigen = com.IdOrigenRegion.ToString();
                                    //    _destino.OrigenRegionDescripcion = com.OrigenRegion;
                                    //    _destino.FechaIda = com.FechaInicio;
                                    //    _destino.FechaOrigen = com.FechaOrigen.HasValue ? com.FechaOrigen.Value : DateTime.Now;
                                    //    _destino.FechaVuelta = com.FechaHasta;
                                    //    _destino.ObservacionesOrigen = com.ObsOrigen;
                                    //    _destino.ObservacionesDestinos = com.ObsDestino;
                                    //    var res = DestinosPasajesInsert(_destino);
                                    //}
                                    #endregion

                                    /*Si cometido corresponde al ministro se va directamente a analista de gestion personas, esto cuando no se solicita con pasaje*/
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.SolicitudCometido && Cometido.IdEscalafon == 1 && Cometido.GradoDescripcion == "B" && Cometido.ReqPasajeAereo == false)
                                    {
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionDocGP);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionJefatura)
                                    {
                                        /*05-08-2022 - Se agrego modificacion de flujo para aprobacion de jefatura*/
                                        // agregar if de pasajes
                                        if (Cometido.Atrasado)
                                        {
                                            definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.VisacionSubsecretaria);
                                        }
                                        else
                                        {
                                            if(Cometido.ReqPasajeAereo)
                                            {
                                                definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.IngresoCotizacion);
                                            }
                                            else
                                            {
                                                definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionDocGP); /*cometido no posee pasaje por lo tanto sigue a las tarea de gestion personas*/
                                            }
                                        }
                                        /*if(Cometido.JustificacionAtraso!=null)
                                        {
                                            definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.VisacionSubsecretaria);
                                        }
                                        else
                                        {
                                            definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 6); *//*cometido no posee pasaje por lo tanto sigue a las tarea de gestion personas*//*
                                        }*/
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.EncargadoPresupuesto && (Cometido.IdEscalafon != 1 || Cometido.IdEscalafon == null)) //Cometido.CalidadDescripcion != "TITULAR")/*Verifica si coemtido es de ministro o subse y se va a la tarea de juridica*/
                                    {
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaActoAdministrativo);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionJuridica && Cometido.GradoDescripcion == "C")/*Verifica si coemtido es del subse se va a la aprobacion de ministro*/
                                    {
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionMinistro);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionJuridica && Cometido.GradoDescripcion == "B")/*Verifica si coemtido es del ministro se va a la aprobacion del subse*/
                                    {
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionSubse);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionMinistro && Cometido.GradoDescripcion == "C")/*Verifica si coemtido es del subse se va a la aprobacion de ministro*/
                                    {
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaMinistro);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionSubse && Cometido.GradoDescripcion == "B")/*Verifica si coemtido es del ministro se va a la aprobacion del subse*/
                                    {
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaSubsecretario);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.EncargadoFinanzas && Cometido.ResolucionRevocatoria == true) /*si cometido posee resolucion revocatoria no va a subir certificado de pago*/
                                    {
                                        definicionWorkflow = null;
                                    }
                                    else
                                    {
                                        if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.IngresoCotizacion && Cometido.IdEscalafon == 1 && Cometido.GradoDescripcion == "B" && Cometido.ReqPasajeAereo)
                                        {
                                            definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                            definicionWorkflow.Email = _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.JefeGabineteMinistro).Valor;
                                        }
                                        else
                                        {
                                            definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                        }
                                    }
                                }
                                else
                                {
                                    if (workflowActual.DefinicionWorkflow.Secuencia ==(int)Util.Enum.CometidoSecuencia.AprobacionJefatura && Cometido.SolicitaViatico != true)
                                    {
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 6); //8 /*workflowActual.DefinicionWorkflow.Secuencia*/);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia ==(int)Util.Enum.CometidoSecuencia.FirmaActoAdministrativo && Cometido.SolicitaViatico != true)
                                    {
                                        //definicionWorkflow = null;  /*workflow se deja null para terminar el proceso*/
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 16); //15 /*workflowActual.DefinicionWorkflow.Secuencia*/);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia ==(int)Util.Enum.CometidoSecuencia.EncargadoFinanzas)
                                    {
                                        definicionWorkflow = null;  /*workflow se deja null para terminar el proceso*/
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia ==(int)Util.Enum.CometidoSecuencia.SolicitudCometido && Cometido.ReqPasajeAereo)
                                    {
                                        /*se inicia un nuevo proceso de solicitud de pasaje*/
                                        //ProcesoInsert(new Proceso(){
                                        //    DefinicionProcesoId = (int)App.Util.Enum.DefinicionProceso.SolicitudPasaje,
                                        //    Observacion = obj.Observacion,
                                        //    FechaCreacion = DateTime.Now,
                                        //    FechaVencimiento = DateTime.Now.AddBusinessDays(1),
                                        //    FechaTermino = null,
                                        //    Email = obj.Email
                                        //});

                                        /*se define que sigue con el proceso de cometido*/
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);

                                    }
                                    else
                                    {
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                    }
                                }
                            }
                            else
                            {
                                definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                            }
                        } 
                        else if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                            {
                                if (workflowActual.DefinicionWorkflow.Secuencia == 19 || workflowActual.DefinicionWorkflow.Secuencia == 20)
                                {
                                    if (Cometido.ResolucionRevocatoria == true)/*si corresponde a una resolucion revocatoria se envia a firma de jefe depto adminstrativo*/
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 6);
                                    else
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);
                                }
                                else
                                {
                                    if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje && workflowActual.DefinicionWorkflow.Secuencia == 3 && Cometido.ReqPasajeAereo)
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1); /*Al ser rechazado va a la tarea de ingreso*/
                                    else
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);
                                }
                            }
                            else
                            {
                                if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje && workflowActual.DefinicionWorkflow.Secuencia == 3 && Cometido.ReqPasajeAereo)
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1); /*Al ser rechazado va a la tarea de ingreso*/
                                else
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);
                            }                                
                        }
                        else
                        {
                            //if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje) /*validaciones de secuencia del proceso cometido con pasaje*/
                            //{
                            //    if (workflowActual.DefinicionWorkflow.Secuencia == 3 && Cometido.ReqPasajeAereo == true) /*si cometido no tiene viatico, no pasa por las tareas de generacion cdp*/
                            //    {
                            //        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1); /*Al ser rechazado va a la tarea de ingreso*/
                            //    }
                            //}
                            //else
                            //    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);

                            if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje && workflowActual.DefinicionWorkflow.Secuencia == 3 && Cometido.ReqPasajeAereo)
                                definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1); /*Al ser rechazado va a la tarea de ingreso*/
                            else
                                definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);
                        }
                }
                else if (workflowActual.Proceso.DefinicionProceso.Entidad.Codigo == Util.Enum.Entidad.Pasaje.ToString())
                {
                    var Pasaje = _repository.GetFirst<Pasaje>(q => q.WorkflowId == obj.WorkflowId);

                    //deterrminar siguiente tarea desde el diseño de proceso
                    if (!workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                        if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                        {
                            if (Pasaje != null)
                            {
                                if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);// continua con el proceso de cometido con pasaje
                                }
                                else if (workflowActual.DefinicionWorkflow.Secuencia == 4 /*&& Pasaje.TipoDestino == true*/)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 9); // Pasaje Nacional                                   
                                }
                                else if (workflowActual.DefinicionWorkflow.Secuencia == 6)// Valida que seleccion de cotizacion sea la mas barata
                                {
                                    var seleccion = new Cotizacion();
                                    var cotizacion = Pasaje.Cotizacion;
                                    bool valor = false;
                                    foreach (var cotiza in cotizacion)
                                    {
                                        if (cotiza.Seleccion)
                                        {
                                            seleccion = cotiza;
                                        }
                                    }

                                    foreach (var cotiza in cotizacion)
                                    {
                                        if (seleccion.ValorPasaje <= cotiza.ValorPasaje)
                                            valor = true;
                                    }


                                    if (valor == false)
                                    {
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 8);
                                    }
                                    else
                                    {
                                        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                    }
                                }
                                else
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);// Paseje Internacional
                                }
                            }
                            else
                            {
                                definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                            }
                        }
                        else
                            definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);
                }
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

                //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);

                //en el caso de no existir mas tareas, cerrar proceso
                if (definicionWorkflow == null)
                {
                    /*se cierra y cambia estado del proceso.*/
                    workflowActual.Proceso.EstadoProcesoId = (int)Util.Enum.EstadoProceso.Terminado;
                    //workflowActual.Proceso.Terminada = true;
                    workflowActual.Proceso.FechaTermino = DateTime.Now;
                    //workflowActual.Pl_UndDes = persona.Unidad.Pl_UndDes.Trim();
                    //workflowActual.Pl_UndCod = persona.Unidad.Pl_UndCod;
                    //workflowActual.Email = persona.Funcionario.Rh_Mail.Trim();

                    _repository.Save();

                    #region ENVIO DE NOTIFICACION TERMINO PROCESO
                    /*si no existen mas tareas se envia correo de notificacion*/
                    var cometido = _repository.GetFirst<Cometido>(c => c.ProcesoId == workflowActual.ProcesoId);
                    /*se trae documento para adjuntar*/
                    Documento doc = cometido.Proceso.Documentos.FirstOrDefault(d => d.ProcesoId == cometido.ProcesoId && d.TipoDocumentoId == 1);
                    var solicitante = _repository.Get<Workflow>(c => c.ProcesoId == workflowActual.ProcesoId && c.DefinicionWorkflow.Secuencia == 1).FirstOrDefault().Email;
                    var QuienViaja = _sigper.GetUserByRut(cometido.Rut).Funcionario.Rh_Mail.Trim();
                    List<string> emailMsg;

                    /*se si la ultima tarea fue la firma de resoucion*/
                    if (workflowActual.DefinicionWorkflow.Secuencia == 13)
                    {
                        /*Aprueba y notifica a Oficina de Partes*/
                        emailMsg = new List<string>();
                        var OfPartes = _repository.GetFirst<Configuracion>(q => q.Nombre == Util.Enum.Configuracion.CorreoOfPartes.ToString());
                        //emailMsg.Add("acifuentes@economia.cl"); //oficia de partes
                        //emailMsg.Add("scid@economia.cl"); //oficia de partes
                        emailMsg.Add(OfPartes.Valor); //oficia de partes
                        emailMsg.Add("mmontoya@economia.cl"); //oficia de partes

                        _email.NotificacionesCometido(workflowActual,
                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEncargadoDeptoAdmin_OfPartes), /*notificacion a oficia de partes*/
                        "Se ha tramitado un cometido nacional",
                        emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor,
                        doc, cometido.Folio, cometido.FechaResolucion.ToString(), cometido.TipoActoAdministrativo);

                        /*Aprueba y notifica a solicitante y quien viaja*/
                        emailMsg = new List<string>();
                        emailMsg.Add(QuienViaja);//quien viaja
                        emailMsg.Add(solicitante.Trim()); //solicitante

                        _email.NotificacionesCometido(workflowActual,
                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEncargadoDeptoAdmin_Solicitante_QuienViaja),
                        "Se ha tramitado el cometido nacional solicitado",
                        emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                         _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, doc, cometido.Folio, cometido.FechaResolucion.ToString(), cometido.TipoActoAdministrativo);

                    }
                    else
                    {

                        if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                        {
                            if (cometido.ObservacionesPagoSigfeTesoreria == null)
                            {
                                /*Aprueba pago y notifica a interesado(a)*/
                                emailMsg = new List<string>();
                                emailMsg.Add(QuienViaja);//quien viaja
                                emailMsg.Add(solicitante.Trim()); //solicitante

                                _email.NotificacionesCometido(workflowActual,
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaFinanzas_Solicitante_QuienViaja),
                                "Su cometido N°" + cometido.CometidoId + " " + "ha sido pagado",
                                emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, doc, "", "", "");
                            }


                            if (cometido.ObservacionesPagoSigfeTesoreria != null)
                            {
                                /*Aprueba pago con observaciones o sin pago y envía a interesado(a)*/
                                emailMsg = new List<string>();
                                emailMsg.Add(QuienViaja);//quien viaja
                                emailMsg.Add(solicitante.Trim()); //solicitante

                                _email.NotificacionesCometido(workflowActual,
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaFinanzas_Solicitante_QuienViaja2),
                                "Su cometido N°" + cometido.CometidoId + " " + "tiene OBSERVACIONES para el pago",
                                emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, doc, "", "", "");
                            }
                        }

                        if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            /*Rechaza pago y notifica a Encargado de Tesorería*/
                            emailMsg = new List<string>();
                            emailMsg.Add(workflowActual.DefinicionWorkflow.Secuencia == 19 && workflowActual.DefinicionWorkflow.Email != null ? workflowActual.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Encargado Tesoreria

                            _email.NotificacionesCometido(workflowActual,
                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaFinanzasRechazo_EncargadoTesoreria),
                            "El pago del cometido N° " + cometido.CometidoId + "ha sido rechazado por el Encargado(a) de Finanzas",
                            emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, doc, "", "", "");
                        }
                    }
                    #endregion                    
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
                    workflow.Asunto = !string.IsNullOrEmpty(workflowActual.Asunto) ? workflowActual.Asunto : workflowActual.DefinicionWorkflow.DefinicionProceso.Nombre + " Nro: " + _repository.Get<Cometido>(c => c.ProcesoId == workflow.ProcesoId).FirstOrDefault().CometidoId;

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

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.CualquierPersonaGrupo)
                    {
                        //if (obj.Pl_UndCod.HasValue)
                        //{
                        //    var unidad = _sigper.GetUnidad(obj.Pl_UndCod.Value);
                        //    if (unidad == null)
                        //        throw new Exception("No se encontró la unidad en SIGPER.");

                        //    workflow.Pl_UndCod = unidad.Pl_UndCod;
                        //    workflow.Pl_UndDes = unidad.Pl_UndDes;
                        //}

                        //if (!string.IsNullOrEmpty(obj.To))
                        //{
                        //    persona = _sigper.GetUserByEmail(obj.To);
                        //    if (persona == null)
                        //        throw new Exception("No se encontró el usuario en SIGPER.");

                        //    workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        //    workflow.TareaPersonal = true;
                        //}

                        /* si seleccionó unidad y usuario... 09112020*/
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
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaJefaturaDeFuncionarioQueViaja)
                    {
                        //if (workflow.Proceso.DefinicionProceso.Entidad.Codigo == App.Util.Enum.Entidad.Cometido.ToString() || workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                        if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                        {
                            var com = _repository.Get<Cometido>(c => c.ProcesoId == workflow.ProcesoId);
                            if (workflowActual.DefinicionWorkflow.Secuencia == 3 && com.FirstOrDefault().IdEscalafon == 1 && com.FirstOrDefault().GradoDescripcion == "B" && com.FirstOrDefault().ReqPasajeAereo)
                            {
                                var rut = _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.JefeGabineteMinistro).Valor;
                                persona = _sigper.GetUserByRut(int.Parse(rut));
                                if (persona == null)
                                    throw new Exception("No se encontró el usuario en Sigper.");
                                workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                                workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                                workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                                workflow.TareaPersonal = true;
                            }
                            else
                            {
                                persona = _sigper.GetUserByRut(com.FirstOrDefault().Rut);
                                if (persona == null)
                                    throw new Exception("No se encontró el usuario en Sigper.");
                                workflow.Email = persona.Jefatura.Rh_Mail.Trim();
                                workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                                workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                                workflow.TareaPersonal = true;
                            }
                        }
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico)
                    {

                        //workflow.GrupoId = definicionWorkflow.GrupoId;
                        //workflow.Pl_UndCod = definicionWorkflow.Pl_UndCod;
                        //workflow.Pl_UndDes = definicionWorkflow.Pl_UndDes;

                        //var grupo = _repository.GetById<Grupo>(definicionWorkflow.GrupoId.Value);
                        //var emails = grupo.Usuarios.Where(q => q.Habilitado).Select(q => q.Email);
                        //if (emails.Any())
                        //    workflow.Email = string.Join(";", emails);

                        /*09112020*/
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
                                workflow.Email = string.Join(",", emails);
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
                                workflow.Email = string.Join(",", emails);
                        }
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)Util.Enum.TipoEjecucion.EjecutaUsuarioEspecifico)
                    {
                        persona = _sigper.GetUserByEmail(definicionWorkflow.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en Sigper.");
                        if (persona.Funcionario == null)
                        {
                            throw new Exception("No se encontró el usuario en Sigper.");
                        }
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


                    #region NOTIFICACIONES DE CORREO PROCESO COMETIDO OLD
                    /*Si el proceso corresponde a Cometidos y esta en la tarea de firma electronica se notifica con correo*/
                    //if (workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.Cometido.ToString())
                    //{
                    //    if (workflow.DefinicionWorkflow.Secuencia == 13 || workflow.DefinicionWorkflow.Secuencia == 14 || workflow.DefinicionWorkflow.Secuencia == 15)
                    //    {
                    //        List<string> emailMsg = new List<string>();
                    //        emailMsg.Add("mmontoya@economia.cl");
                    //        emailMsg.Add("acifuentes@economia.cl"); //oficia de partes
                    //        emailMsg.Add("scid@economia.cl"); //oficia de partes
                    //        emailMsg.Add(persona.Funcionario.Rh_Mail.Trim()); // interesado

                    //        _email.NotificarFirmaResolucionCometido(workflow,
                    //        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaFirmaResolucion),
                    //        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion), emailMsg);
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
                    //            _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion), emailMsg);
                    //        }
                    //    }
                    //    else if (workflow.DefinicionWorkflow.Secuencia == 13 || workflow.DefinicionWorkflow.Secuencia == 14 || workflow.DefinicionWorkflow.Secuencia == 15)
                    //    {
                    //        List<string> emailMsg = new List<string>();
                    //        emailMsg.Add("mmontoya@economia.cl");
                    //        emailMsg.Add(persona.Funcionario.Rh_Mail.Trim()); // interesado quien viaja

                    //        _email.NotificarFirmaResolucionCometido(workflow,
                    //        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaNotificacionPago),
                    //        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion), emailMsg);
                    //    }
                    //}
                    #endregion

                    #region NOTIFICACIONES DE CORREO PROCESO COMETIDO
                    /*Notificaciones de correo proceso cometidos*/
                    if (workflow.Proceso.DefinicionProceso.Entidad.Codigo == Util.Enum.Entidad.Cometido.ToString())
                    {

                        string jefe = string.Empty;
                        var cometido = _repository.GetFirst<Cometido>(c => c.ProcesoId == workflow.ProcesoId);
                        var solicitante = _repository.Get<Workflow>(c => c.ProcesoId == workflow.ProcesoId && c.DefinicionWorkflow.Secuencia == 1).FirstOrDefault().Email.Trim();
                        var QuienViaja = _sigper.GetUserByRut(cometido.Rut).Funcionario.Rh_Mail.Trim();
                        if (_sigper.GetUserByEmail(QuienViaja).Jefatura != null)
                            jefe = _sigper.GetUserByEmail(QuienViaja).Jefatura.Rh_Mail.Trim();
                        else
                            jefe = "mmontoya@economia.cl";

                        List<string> emailMsg;

                        switch (workflowActual.DefinicionWorkflow.Secuencia)
                        {
                            case 1: /*Envío solicitud de cometido*/
                                /*correos si solicitud de cometido incluye pasaje*/
                                if (cometido.ReqPasajeAereo)
                                {
                                    /*A solicitante y a quien viaja*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(solicitante.Trim()); //solicitante
                                    emailMsg.Add(QuienViaja);//quien viaja

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEnvíoSolicitudCometidoPasaje),
                                    "Ha enviado una nueva solicitud de cometido N: " + cometido.CometidoId,
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                    /*Notifica a Analista de Unidad de Abastecimiento*/
                                    emailMsg = new List<string>();
                                    if (workflow.DefinicionWorkflow.Secuencia == 3 && workflow.DefinicionWorkflow.GrupoId != null)
                                    {
                                        var grupo = _repository.GetById<Grupo>(definicionWorkflow.GrupoId.Value);
                                        var emails = grupo.Usuarios.Where(q => q.Habilitado).Select(q => q.Email).ToList();
                                        if (emails.Any())
                                        {
                                            foreach (var c in emails)
                                            {
                                                emailMsg.Add(c.Trim());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 3 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Analista Abastecimiento
                                    }

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEnvioSolicitudAnalistaAbastecimiento),
                                    "Tiene una solicitud de cotización de pasajes para el cometido N°: " + cometido.CometidoId,
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }
                                else
                                {
                                    /*A solicitante y quien viaja:*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(QuienViaja);//quien viaja
                                    emailMsg.Add(solicitante.Trim()); //solicitante

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEnvíoSolicitudCometido),
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.AsuntoSolicitudCometido_Solicitante_QuienViaja).Valor + cometido.CometidoId,
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                    /*A jefatura*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(jefe);//jafatura

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEnvíoSolicitudCometidoJefatura),
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.AsuntoSolicitudCometido_Jefatura).Valor + cometido.CometidoId,
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                break;
                            case 2:/*Aprobación/Rechazo de cometido de la jefatura*/
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                {
                                    /*Aprobación del cometido a solicitante y a quien viaja*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(QuienViaja);//quien viaja
                                    emailMsg.Add(solicitante.Trim()); //solicitante

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAprobaciónRechazoCometidoJefatura_Solicitante_QuienViaja),
                                    "Su solicitud de cometido N°:" + cometido.CometidoId + " " + "ha sido aprobada por su jefatura",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                    /*Aprobación a jefatura */
                                    emailMsg = new List<string>();
                                    emailMsg.Add(jefe);//jefatura

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAprobaciónRechazoCometidoJefatura_Jefatura),
                                    "Usted ha aprobado el cometido N°:" + cometido.CometidoId,
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                    /*Aprobación a Analista de Unidad de Gestión y Desarrollo de Personas*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflowActual.Email.Trim()); //gestión personas

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAprobaciónRechazoCometidoJefatura_GP),
                                    "Tiene el cometido N°:" + cometido.CometidoId + " " + "para revisión",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    /*Rechazo a solicitante*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(QuienViaja);//quien viaja
                                    emailMsg.Add(solicitante.Trim()); //solicitante

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaRechazoCometidoJefatura_Solicitante_QuienViaja),
                                    "Su solicitud de cometido N°: " + cometido.CometidoId + " " + "ha sido rechazada por su jefatura",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                   _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                    /*Rechazo a jefatura */
                                    emailMsg = new List<string>();
                                    emailMsg.Add(jefe);//jafatura

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaRechazoCometidoJefatura_Jefatura),
                                    "Usted ha rechazado el cometido N°: " + cometido.CometidoId,
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                     _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                break;
                            case 3:/*Notifica para selección de pasajes*/
                                /*A jefatura para selección de cotización de pasajes*/
                                emailMsg = new List<string>();
                                emailMsg.Add(jefe);//jefatura

                                _email.NotificacionesCometido(workflow,
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaSeleccionPasaje_Jefatura),
                                "Tiene una cotización de pasajes pendiente para revisión",
                                emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                /*A solicitante para selección de cotización de pasajes*/
                                emailMsg = new List<string>();
                                emailMsg.Add(solicitante.Trim()); //solicitante

                                _email.NotificacionesCometido(workflow,
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaSeleccionPasaje_Solicitante),
                                "Su jefatura tiene una cotización de pasajes pendiente",
                                emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                break;
                            case 4:/*Jefatura revisa cotizaciones*/
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                {
                                    /*Confirmación de aprobación de cotización y cometido*/
                                    /*A Jefatura*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(jefe);//jafatura

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAprobacionPasaje_Jefatura),
                                    "Confirmación de aprobación de cometido y compra de pasajes",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                    /*A abastecimiento*/
                                    emailMsg = new List<string>();
                                    if (workflow.DefinicionWorkflow.Secuencia == 5 && workflow.DefinicionWorkflow.GrupoId != null)
                                    {
                                        var grupo = _repository.GetById<Grupo>(definicionWorkflow.GrupoId.Value);
                                        var emails = grupo.Usuarios.Where(q => q.Habilitado).Select(q => q.Email).ToList();
                                        if (emails.Any())
                                        {
                                            foreach (var c in emails)
                                            {
                                                emailMsg.Add(c.Trim());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 5 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Jefatura Abastecimiento
                                    }


                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAprobacionPasaje_JefaturaAbastecimiento),
                                    "Tiene una compra de pasajes pendiente para el cometido N°" + cometido.CometidoId,
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    emailMsg = new List<string>();
                                    if (workflow.DefinicionWorkflow.Secuencia == 3 && workflow.DefinicionWorkflow.GrupoId != null)
                                    {
                                        var grupo = _repository.GetById<Grupo>(definicionWorkflow.GrupoId.Value);
                                        var emails = grupo.Usuarios.Where(q => q.Habilitado).Select(q => q.Email).ToList();
                                        if (emails.Any())
                                        {
                                            foreach (var c in emails)
                                            {
                                                emailMsg.Add(c.Trim());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 3 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Analista Abastecimiento
                                    }

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaRechazoPasaje_AnalistaAbastecimiento),
                                    "Tiene una cotización de pasajes rechazada para el cometido N° " + cometido.CometidoId,
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                break;
                            case 5:/*Analista de Abastecimiento compra pasajes y adjunta documentación*/
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                {
                                    /*Analista gestion personas*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 6 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Analista Gestion Personas

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaCompraPasajes_AnalistaGP),
                                    "Tiene el cometido N°: " + cometido.CometidoId + " " + "para revisión",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                    /*A solicitante y quien viaja*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(QuienViaja);//quien viaja
                                    emailMsg.Add(solicitante.Trim()); //solicitante

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaCompraPasajes_Solicitante_QuienViaja),
                                    "Se ha realizado la compra de su pasaje",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    /*A solicitante*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(QuienViaja);//quien viaja
                                    emailMsg.Add(solicitante.Trim()); //solicitante

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaRechazoPasaje_Solicitante_QuienViaja),
                                    "Tiene una selección de pasajes rechazada para el cometido N°: " + cometido.CometidoId,
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                   _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                break;
                            case 6:/*Generación de documento por Analista Unidad de Gestión y Desarrollo de Personas*/
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                {
                                    /*A Encargado(a) de Unidad de Gestión y Desarrollo de Personas --> Tarea Aprobada*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 7 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Jefatura Gestion Personas

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaGeneraciónDocumento),
                                    "Tiene el cometido N°: " + cometido.CometidoId + " " + "para aprobación",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    /*A Solicitante y quien viaja --> Tarea Rechazada*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(QuienViaja);//quien viaja
                                    emailMsg.Add(solicitante.Trim()); //solicitante

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaReasignacionSolicitud),
                                    "Su solicitud de cometido N°: " + cometido.CometidoId + " " + "ha sido devuelta",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                break;
                            case 7:/*Encargado(a) Unidad de Gestión y Desarrollo de Personas --> Jefatura GP*/
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                {
                                    /*Aprueba documento generado a Analista de Presupuesto*/
                                    emailMsg = new List<string>();
                                    if (workflow.DefinicionWorkflow.Secuencia == 8 && workflow.DefinicionWorkflow.GrupoId != null)
                                    {
                                        var grupo = _repository.GetById<Grupo>(definicionWorkflow.GrupoId.Value);
                                        var emails = grupo.Usuarios.Where(q => q.Habilitado).Select(q => q.Email).ToList();
                                        if (emails.Any())
                                        {
                                            foreach (var c in emails)
                                            {
                                                emailMsg.Add(c.Trim());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 8 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Analista ppto
                                    }

                                    /*se valida si cometido tiene resolucion de revocatoria*/
                                    Documento doc = null;
                                    if (cometido.ResolucionRevocatoria == true)
                                        doc = cometido.Proceso.Documentos.FirstOrDefault(d => d.ProcesoId == cometido.ProcesoId && d.TipoDocumentoId == 16);


                                    _email.NotificacionesCometido(workflowActual,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEncargadoGP_AnalistaPpto),
                                    "Tiene el cometido N°: " + cometido.CometidoId + " " + "pendiente de compromiso",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    /*Rechaza documento generado a Analista de Unidad de Gestión y Desarrollo de Personas*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 6 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Analista ppto

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEncargadoGP_AnalistaGP),
                                    "Su solicitud de cometido N°:" + cometido.CometidoId + " " + "ha sido devuelta",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                break;
                            case 8:/*Analista de Presupuesto genera CDP*/
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                {
                                    /*Analista envía CDP para firma de Encargado(a) de Presupuesto*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 9 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Jefatura ppto

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAnalistaPppto_JefePpto),
                                    "Tiene el cometido N°:" + cometido.CometidoId + " " + "para aprobación de CDR",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    /*Analista rechaza CDP*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 6 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Analista GP

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAnalistaPppto_AnalistaGP),
                                    "Su solicitud de cometido N°:" + cometido.CometidoId + " " + "ha sido devuelta",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                break;
                            case 9:/*Encargado(a) de Presupuesto firma CDP*/
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                {
                                    /*Encargado(a) de Presupuesto firma CDP (aviso a Encargado(a) Departamento Administrativo) --> Tarea Aprobado*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 13 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Jefatura Administracion

                                    _email.NotificacionesCometido(workflowActual,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEncargadoPPto_JefaturaAdmin),
                                    "Tiene el cometido N°:" + cometido.CometidoId + " " + "para aprobación",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    /*Encargado(a) de Presupuesto rechaza firma de CDP a Analista de Presupuesto --> Tarea Rechazada*/
                                    emailMsg = new List<string>();
                                    if (workflow.DefinicionWorkflow.Secuencia == 8 && workflow.DefinicionWorkflow.GrupoId != null)
                                    {
                                        var grupo = _repository.GetById<Grupo>(definicionWorkflow.GrupoId.Value);
                                        var emails = grupo.Usuarios.Where(q => q.Habilitado).Select(q => q.Email).ToList();
                                        if (emails.Any())
                                        {
                                            foreach (var c in emails)
                                            {
                                                emailMsg.Add(c.Trim());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 8 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Analista PPto
                                    }

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEncargadoPPto_AnalistaPpto),
                                    "Su solicitud de cometido N°:" + cometido.CometidoId + " " + "ha sido devuelta",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                break;
                            case 13: /*Encargado(a) Departamento Administrativo firma de acto administrativo*/
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                {
                                    /*se trae documento para adjuntar*/
                                    Documento doc = cometido.Proceso.Documentos.FirstOrDefault(d => d.ProcesoId == cometido.ProcesoId && d.TipoDocumentoId == 1);

                                    /*Aprueba y notifica a Oficina de Partes*/
                                    emailMsg = new List<string>();
                                    var OfPartes = _repository.GetFirst<Configuracion>(q => q.Nombre == Util.Enum.Configuracion.CorreoOfPartes.ToString());
                                    //emailMsg.Add("acifuentes@economia.cl"); //oficia de partes
                                    //emailMsg.Add("scid@economia.cl"); //oficia de partes
                                    emailMsg.Add(OfPartes.Valor); //oficia de partes
                                    emailMsg.Add("mmontoya@economia.cl"); //oficia de partes

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEncargadoDeptoAdmin_OfPartes),
                                    "Se ha tramitado un cometido nacional",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor,
                                    doc, cometido.Folio, cometido.FechaResolucion.ToString(), cometido.TipoActoAdministrativo);

                                    /*Aprueba y notifica a solicitante y quien viaja*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(QuienViaja);//quien viaja
                                    emailMsg.Add(solicitante.Trim()); //solicitante

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEncargadoDeptoAdmin_Solicitante_QuienViaja),
                                    "Se ha tramitado el cometido nacional solicitado",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                     _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, doc, cometido.Folio, cometido.FechaResolucion.ToString(), cometido.TipoActoAdministrativo);

                                    /*Aprueba y notifica a analista de contabilidad para devengo*/
                                    emailMsg = new List<string>();
                                    if (workflow.DefinicionWorkflow.Secuencia == 16 && workflow.DefinicionWorkflow.GrupoId != null)
                                    {
                                        var grupo = _repository.GetById<Grupo>(definicionWorkflow.GrupoId.Value);
                                        var emails = grupo.Usuarios.Where(q => q.Habilitado).Select(q => q.Email).ToList();
                                        if (emails.Any())
                                        {
                                            foreach (var c in emails)
                                            {
                                                emailMsg.Add(c.Trim());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 16 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Analista Contabilidad
                                    }

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEncargadoDeptoAdmin_AnalistaConta),
                                    "Tiene el cometido N°" + cometido.CometidoId + " " + "para devengo",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, doc, "", "", "");
                                }
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    /*Rechaza y notifica a Encargado(a) de Presupuesto (si cometido irroga gasto)*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 9 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Analista Contabilidad

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEncargadoDeptoAdmin_JefePpto),
                                    "Su solicitud de cometido N°:" + cometido.CometidoId + " " + "ha sido devuelta",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                    /*Rechaza y notifica a Analista de Gestión y Desarrollo de Personas (si cometido no irroga gasto)*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 9 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Analista GP

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEncargadoDeptoAdmin_AnalistaGP),
                                    "Su solicitud de cometido N°:" + cometido.CometidoId + " " + "ha sido devuelta",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                break;
                            case 16: /*Analista contabilidad devenga*/
                                /*Devenga y envía a aprobación de Encargado(a) de Contabilidad*/
                                if (cometido.ObservacionesPagoSigfe == null)
                                {
                                    emailMsg = new List<string>();
                                    if (workflow.DefinicionWorkflow.Secuencia == 17 && workflow.DefinicionWorkflow.GrupoId != null)
                                    {
                                        var grupo = _repository.GetById<Grupo>(definicionWorkflow.GrupoId.Value);
                                        var emails = grupo.Usuarios.Where(q => q.Habilitado).Select(q => q.Email).ToList();
                                        if (emails.Any())
                                        {
                                            foreach (var c in emails)
                                            {
                                                emailMsg.Add(c.Trim());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 17 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Encargado Contabilidad
                                    }

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAnalistaConta_JefeConta),
                                    "Tiene el cometido N°:" + cometido.CometidoId + " " + "para aprobación",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                /*Devengo con observaciones o sin devengo y notifica a Analista de Unidad de Desarrollo y Gestión de Personas*/
                                emailMsg = new List<string>();
                                emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 9 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Analista GP

                                _email.NotificacionesCometido(workflow,
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAnalistaConta_AnalistaGP),
                                "Su solicitud de cometido N°:" + cometido.CometidoId + " " + "tiene OBSERVACIONES",
                                emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                /*Devengo con observaciones o sin devengo y notifica a Encargado(a) de Contabilidad*/
                                if (cometido.ObservacionesPagoSigfe != null)
                                {
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 17 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Encargado Conta

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAnalistaConta_EncargadoConta),
                                    "Tiene el cometido N°:" + cometido.CometidoId + " " + "CON OBSERVACIONES para aprobación",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                break;
                            case 17: /*Encargado(a) de Contabilidad revisa devengo*/
                                /*Aprueba devengo y envía a Analista de Tesorería*/
                                emailMsg = new List<string>();
                                if (workflow.DefinicionWorkflow.Secuencia == 18 && workflow.DefinicionWorkflow.GrupoId != null)
                                {
                                    var grupo = _repository.GetById<Grupo>(definicionWorkflow.GrupoId.Value);
                                    var emails = grupo.Usuarios.Where(q => q.Habilitado).Select(q => q.Email).ToList();
                                    if (emails.Any())
                                    {
                                        foreach (var c in emails)
                                        {
                                            emailMsg.Add(c.Trim());
                                        }
                                    }
                                }
                                else
                                {
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 18 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Analista Tesoreria
                                }

                                _email.NotificacionesCometido(workflow,
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEncargadoConta_AnalistaTesoreria),
                                "Tiene el cometido N°:" + cometido.CometidoId + " " + "para pago",
                                emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                break;

                            case 18:/*Analista de Tesorería ingresa pago*/
                                /*Aprueba pago y envía a Encargado(a) de Tesorería*/
                                emailMsg = new List<string>();
                                emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 19 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Encargado Tesoreria

                                _email.NotificacionesCometido(workflow,
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAnalistaTesoreria_JefeTesoreria),
                                "Tiene el cometido N°:" + cometido.CometidoId + " " + "para aprobación",
                                emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                if (cometido.ObservacionesPagoSigfeTesoreria != null)
                                {
                                    /*Aprueba pago con observaciones o sin pago y notifica a Analista de Unidad de Desarrollo y Gestión de Personas */
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 9 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Analista GP

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAnalistaTesoreria_AnalistaGP),
                                    "Su solicitud de cometido N°:" + cometido.CometidoId + " " + "tiene OBSERVACIONES",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                    /*Aprueba pago con observaciones o sin pago y notifica a Encargado(a) de Tesorería*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 19 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Encargado Tesoreria

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAnalistaTesoreria_EncargadoTesoreria),
                                    "Tiene el cometido N°" + cometido.CometidoId + " " + "CON OBSERVACIONES para aprobación",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                break;
                            case 19: /*Encargado(a) de Tesorería revisa pago*/
                                /*Aprueba pago y envía a Encargado(a) de Unidad de Finanzas */
                                if (cometido.ObservacionesPagoSigfeTesoreria == null)
                                {
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 20 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Encargado Finanzas

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEncargadoTesoreria_EncargadoFinanzas),
                                    "Tiene el cometido N°" + cometido.CometidoId + " " + "para aprobación",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), 
                                    !string.IsNullOrEmpty(cometido.ObservacionesPagoSigfeTesoreria) ? cometido.ObservacionesPagoSigfeTesoreria + "-" + workflowActual.Observacion : "" ,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");

                                }

                                if (cometido.ObservacionesPagoSigfeTesoreria != null)
                                {
                                    /*Aprueba pago con observaciones o sin pago y envía a Encargado(a) de Unidad de Finanzas*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 20 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Encargado Finanzas

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEncargadoTesoreria_EncargadoFinanzas2),
                                    "Tiene el cometido N°" + cometido.CometidoId + " " + "CON OBSERVACIONES para aprobación",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                break;
                            case 20:/*Encargado(a) de Unidad de Finanzas revisa devengo y pago*/
                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                {
                                    if (cometido.ObservacionesPagoSigfeTesoreria == null)
                                    {
                                        /*Aprueba pago y notifica a interesado(a)*/
                                        emailMsg = new List<string>();
                                        emailMsg.Add(QuienViaja);//quien viaja
                                        emailMsg.Add(solicitante.Trim()); //solicitante

                                        _email.NotificacionesCometido(workflow,
                                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaFinanzas_Solicitante_QuienViaja),
                                        "Su cometido N°" + cometido.CometidoId + " " + "ha sido pagado",
                                        emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(),
                                        !string.IsNullOrEmpty(cometido.ObservacionesPagoSigfeTesoreria) ? cometido.ObservacionesPagoSigfeTesoreria + "-" + workflowActual.Observacion : "",
                                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                    }


                                    if (cometido.ObservacionesPagoSigfeTesoreria != null)
                                    {
                                        /*Aprueba pago con observaciones o sin pago y envía a interesado(a)*/
                                        emailMsg = new List<string>();
                                        emailMsg.Add(QuienViaja);//quien viaja
                                        emailMsg.Add(solicitante.Trim()); //solicitante

                                        _email.NotificacionesCometido(workflow,
                                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaFinanzas_Solicitante_QuienViaja2),
                                        "Su cometido N°" + cometido.CometidoId + " " + "tiene OBSERVACIONES para el pago",
                                        emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                    }
                                }

                                if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    /*Rechaza pago y notifica a Encargado de Tesorería*/
                                    emailMsg = new List<string>();
                                    emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 19 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");//Encargado Tesoreria

                                    _email.NotificacionesCometido(workflow,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaFinanzasRechazo_EncargadoTesoreria),
                                    "El pago del cometido N° " + cometido.CometidoId + "ha sido rechazado por el Encargado(a) de Finanzas",
                                    emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                    _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                }

                                break;
                        }
                    }
                    #endregion
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