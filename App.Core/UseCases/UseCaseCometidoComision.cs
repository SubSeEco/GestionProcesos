using App.Core.Interfaces;
using App.Model.Cometido;
using App.Model.Comisiones;
using App.Model.Core;
using App.Model.Pasajes;
using App.Model.Shared;
using App.Model.Sigper;
using App.Util;
using System;
using System.Collections.Generic;
using System.Linq;

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

        //public ResponseMessage PatenteVehiculoInsert(PatenteVehiculo obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (string.IsNullOrEmpty(obj.PlacaPatente))
        //        {
        //            response.Errors.Add("Debe especificar la Patente del Vehiculo.");
        //        }
        //        if (obj.PlacaPatente.Count() > 6 || obj.PlacaPatente.Count() < 6)
        //        {
        //            response.Errors.Add("La patente debe contener 6 caracteres minimo o maximo, en formato ABCD12");
        //        }
        //        if (!obj.SIGPERTipoVehiculoId.HasValue)
        //        {
        //            response.Errors.Add("Debe especificar el tipo de vehiculo.");
        //        }
        //        if (string.IsNullOrEmpty(obj.Codigo))
        //        {
        //            response.Errors.Add("Debe especificar la Región del Vehiculo.");
        //        }

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

        //public ResponseMessage PatenteVehiculoDelete(int id)
        //{
        //    var response = new ResponseMessage();
        //    try
        //    {
        //        var obj = _repository.GetById<PatenteVehiculo>(id);
        //        if (obj == null)
        //        {
        //            response.Errors.Add("Dato de Vehiculo no encontrado.");
        //        }

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

        //public ResponseMessage PatenteVehiculoUpdate(PatenteVehiculo obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (string.IsNullOrEmpty(obj.PlacaPatente))
        //        {
        //            response.Errors.Add("Debe especificar la Patente del Vehiculo.");
        //        }
        //        if (obj.PlacaPatente.Count() > 6 || obj.PlacaPatente.Count() < 6)
        //        {
        //            response.Errors.Add("La patente debe contener 6 caracteres minimo o maximo, en formato ABCD12");
        //        }
        //        if (!obj.SIGPERTipoVehiculoId.HasValue)
        //        {
        //            response.Errors.Add("Debe especificar el tipo de vehiculo.");
        //        }
        //        if (string.IsNullOrEmpty(obj.Codigo))
        //        {
        //            response.Errors.Add("Debe especificar la Región del Vehiculo.");
        //        }

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
                    if (item.FileName == string.Empty)
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

                    /*if (obj.PlacaVehiculo.IsNullOrWhiteSpace())
                        response.Errors.Add("Se debe señalar la placa patente del vehiculo.");*/
                }

                if (!string.IsNullOrEmpty(obj.GradoDescripcion))
                {
                    obj.IdGrado = obj.GradoDescripcion;
                }

                if (string.IsNullOrEmpty(obj.Jefatura) || obj.Jefatura == "Sin jefatura definida")
                    response.Errors.Add("No se ha definido la jefatura del funcionario.");

                for (int i = 0; i < listaDestinosCometido.Count; i++)
                {
                    //variable para contar fechas.
                    var helper = new List<int>();

                    var fecha = obj.FechaSolicitud.Date.Subtract(listaDestinosCometido[i].FechaInicio.Date).Days;
                    var fechahelp = listaDestinosCometido[i].FechaInicio.Date.Subtract(obj.FechaSolicitud.Date).Days;

                    if (fechahelp < 7)
                    {
                        helper.Add(fecha);
                    }

                    if (helper.Any())
                    {
                        obj.Atrasado = true;
                    }
                    else
                    {
                        obj.Atrasado = false;
                    }

                    /*if (obj.IdGrado == "C" || obj.IdGrado == "B")
                    {
                        obj.Atrasado = false;
                    }*/
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

        public void SwitchLocalidad(int IdConglomerado, int local, ResponseMessage response, Destinos dest, bool adyacente)
        {
            switch (IdConglomerado)
            {
                case 1:
                    #region Primera Region
                    if (dest.IdComuna == "01101")
                    {
                        if (local == 3944)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "01401")
                    {
                        if (local == 10 || local == 11 || local == 1497 || local == 1498 ||
                            local == 1499 || local == 1500 || local == 1501 || local == 1502 ||
                            local == 1503 || local == 1504 || local == 1504 || local == 1505 ||
                            local == 1506 || local == 1507 || local == 1508)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "01402")
                    {
                        if (local == 3 || local == 1509 || local == 1510 || local == 1511 ||
                            local == 1512 || local == 1513 || local == 1514 || local == 1515 ||
                            local == 1516 || local == 1517 || local == 1518 || local == 1519 ||
                            local == 1520)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "01403")
                    {
                        if (local == 4 || local == 1521 || local == 1522)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "01404")
                    {
                        if (local == 5 || local == 1523 || local == 1524 || local == 1525 ||
                            local == 1526 || local == 1527 || local == 1528 || local == 1529 ||
                            local == 1530 || local == 1531 || local == 1532 || local == 1533 ||
                            local == 1534 || local == 1535 || local == 1536 || local == 1537 ||
                            local == 1538 || local == 1539 || local == 1540 || local == 1541 ||
                            local == 1542 || local == 1543 || local == 1544 || local == 1545)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "01405")
                    {
                        if (local == 6 || local == 8 || local == 1546 ||
                            local == 1547 || local == 1548 || local == 1549)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    break;
                #endregion
                case 2:
                    #region Segunda Región
                    if (dest.IdComuna == "02101")
                    {
                        if (local == 13 || local == 3945 ||
                            local == 3946 || local == 3947 || local == 3948)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "02102")
                    {
                        if (local == 3949)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "02103")
                    {
                        if (local == 17)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "02104")
                    {
                        if (local == 18 || local == 19 || local == 20 || local == 1559
                            || local == 1560 || local == 1561)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "02201")
                    {
                        if (local == 21 || local == 22 || local == 23 ||
                            local == 24 || local == 25 || local == 26 ||
                            local == 1562 || local == 1563 || local == 1564 ||
                            local == 1565)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "02202")
                    {
                        if (local == 27)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "02203")
                    {
                        if (local == 28 || local == 29 || local == 30 ||
                            local == 31 || local == 32 || local == 33 ||
                            local == 34 || local == 35 || local == 1566)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "02301")
                    {
                        if (local == 37 || local == 1567 ||
                            local == 1568 || local == 1569)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "02302")
                    {
                        if (local == 36)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            metodoMensaje(dest); adyacente = true;
                        }
                    }*/
                    break;
                #endregion
                case 3:
                    #region Tercera Región
                    if (dest.IdComuna == "03101")
                    {
                        if (local == 52 || local == 1578 || local == 3950 || local == 3951 ||
                            local == 3952 || local == 3953 || local == 3954 || local == 3955 ||
                            local == 3956 || local == 3957 || local == 52 || local == 1578)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "03102")
                    {
                        if (local == 45 || local == 46 || local == 47 ||
                            local == 1580 || local == 1581 || local == 1582 ||
                            local == 1583 || local == 1584 || local == 1585 ||
                            local == 1586 || local == 1587 || local == 1588 ||
                            local == 1589 || local == 1590 || local == 1591)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "03103")
                    {
                        if (local == 53 || local == 25 || local == 55 ||
                            local == 56 || local == 57 || local == 1592 ||
                            local == 1593 || local == 1594 || local == 1595)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "03201")
                    {
                        if (local == 38 || local == 39 || local == 40 || local == 41 ||
                            local == 1596 || local == 1597 || local == 1598 ||
                            local == 1599 || local == 1600 || local == 1601 ||
                            local == 1601)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "03202")
                    {
                        if (local == 42 || local == 43 ||
                            local == 44 || local == 1603)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "03301")
                    {
                        if (local == 66 || local == 67 || local == 1604 || local == 1605 ||
                            local == 1606 || local == 1607 || local == 1608 || local == 1609 ||
                            local == 1610 || local == 1611 || local == 1612 || local == 1613 ||
                            local == 1614 || local == 1615 || local == 1617 || local == 1618)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "03302")
                    {
                        if (local == 58 || local == 59 || local == 60 || local == 61 ||
                            local == 1645 || local == 1646 || local == 1647 || local == 1648 ||
                            local == 1649 || local == 1650 || local == 1651 || local == 1652 ||
                            local == 1653 || local == 1654 || local == 1655 || local == 1656 ||
                            local == 1657 || local == 1658 || local == 1659 || local == 1660 ||
                            local == 1661)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "03303")
                    {
                        if (local == 62 || local == 1619 || local == 1620 || local == 1621 ||
                            local == 1622 || local == 1623 || local == 1624 || local == 1625 ||
                            local == 1626 || local == 1627 || local == 1628 || local == 1629 ||
                            local == 1630 || local == 1631 || local == 1632 || local == 1633 ||
                            local == 1634 || local == 1635 || local == 1636 || local == 1637 ||
                            local == 1638 || local == 1639)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "03304")
                    {
                        if (local == 63 || local == 64 || local == 65 || local == 1640 ||
                            local == 1641 || local == 1642 || local == 1643 || local == 1644)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            adyacente = true;
                            metodoMensaje(dest);
                        }
                    }*/
                    break;
                #endregion
                case 4:
                    #region Cuarta Región
                    if (dest.IdComuna == "04101")
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            metodoMensaje(dest); adyacente = true;
                        }
                    }
                    else if (dest.IdComuna == "04102")
                    {
                        if (local == 3959 || local == 3960)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "04103")
                    {
                        if (local == 101 || local == 1725 ||
                            local == 1726 || local == 1727)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "04104")
                    {
                        if (local == 110 || local == 111 || local == 112 || local == 113 ||
                            local == 114 || local == 115 || local == 116 || local == 1728 ||
                            local == 1729 || local == 1730 || local == 1731 || local == 1732 ||
                            local == 1733 || local == 1734 || local == 1735 || local == 1736 ||
                            local == 1737)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "04105")
                    {
                        if (local == 126 || local == 127 || local == 128 || local == 1976 ||
                            local == 1977 || local == 1978 || local == 1979 || local == 1980 ||
                            local == 1981 || local == 1982 || local == 1983 || local == 1984 ||
                            local == 1985 || local == 1986 || local == 1987 || local == 1988 ||
                            local == 1989 || local == 1990 || local == 1991 || local == 1992 ||
                            local == 1993 || local == 1994 || local == 1995 || local == 1996 ||
                            local == 1997 || local == 1998 || local == 1999)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "04106")
                    {
                        if (local == 129 || local == 130 || local == 131 || local == 132 ||
                            local == 133 || local == 134 || local == 135 || local == 136 ||
                            local == 1738 || local == 1739 || local == 1740 || local == 1741 ||
                            local == 1742 || local == 1743 || local == 1744 || local == 1745 ||
                            local == 1746 || local == 1747 || local == 1748 || local == 1749 ||
                            local == 1750 || local == 1751 || local == 1752 || local == 1753 ||
                            local == 1754 || local == 1755 || local == 1756 || local == 1757 ||
                            local == 1758 || local == 1759 || local == 1760 || local == 1761 ||
                            local == 1762 || local == 1763 || local == 1764 || local == 1765 ||
                            local == 1766 || local == 1767 || local == 1768 || local == 1769 ||
                            local == 1770)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "04201")
                    {
                        if (local == 73 || local == 74 || local == 75 || local == 76 ||
                            local == 77 || local == 78 || local == 79 || local == 80 ||
                            local == 81 || local == 82 || local == 1771 || local == 1772 ||
                            local == 1773 || local == 1774 || local == 1775 || local == 1776 ||
                            local == 1777 || local == 1778 || local == 1779 || local == 1780 ||
                            local == 1781 || local == 1782 || local == 1783 || local == 1784 ||
                            local == 1785)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "04202")
                    {
                        if (local == 68 || local == 69 || local == 70 || local == 71 ||
                            local == 72 || local == 1786 || local == 1787 || local == 1788 ||
                            local == 1789 || local == 1790 || local == 1791 || local == 1792 ||
                            local == 1793)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "04203")
                    {
                        if (local == 83 || local == 84 || local == 85 || local == 86 ||
                            local == 87 || local == 1794 || local == 1795 || local == 1796 ||
                            local == 1797 || local == 1798 || local == 1799)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "04204")
                    {
                        if (local == 88 || local == 89 || local == 90 || local == 91 ||
                            local == 92 || local == 93 || local == 94 || local == 95 ||
                            local == 96 || local == 97 || local == 98 || local == 99 ||
                            local == 100 || local == 1800 || local == 1801 || local == 1802 ||
                            local == 1803 || local == 1804 || local == 1805 || local == 1806 ||
                            local == 1807 || local == 1808 || local == 1809 || local == 1810 ||
                            local == 1811 || local == 1812 || local == 1813)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "04301")
                    {
                        if (local == 157 || local == 158 || local == 159 || local == 160 ||
                            local == 161 || local == 162 || local == 163 || local == 164 ||
                            local == 165 || local == 166 || local == 167 || local == 168 ||
                            local == 169 || local == 170 || local == 171 || local == 172 ||
                            local == 173 || local == 174 || local == 175 || local == 176 ||
                            local == 177 || local == 178 || local == 179 || local == 1814 ||
                            local == 1815 || local == 1816 || local == 1817 || local == 1818 ||
                            local == 1819 || local == 1820 || local == 1821 || local == 1822 ||
                            local == 1823 || local == 1824 || local == 1825 || local == 1826 ||
                            local == 1827 || local == 1828 || local == 1829 || local == 1830 ||
                            local == 1831 || local == 1832 || local == 1833 || local == 1834 ||
                            local == 1835 || local == 1836 || local == 1837 || local == 1838 ||
                            local == 1839 || local == 1840 || local == 1841 || local == 1842 ||
                            local == 1843 || local == 1844 || local == 1845 || local == 1846 ||
                            local == 1847 || local == 1848 || local == 1849 || local == 1850 ||
                            local == 1851 || local == 1852 || local == 1853 || local == 1854 ||
                            local == 1855 || local == 1856 || local == 1857 || local == 1858 ||
                            local == 1859 || local == 1860 || local == 1861 || local == 1862 ||
                            local == 1863 || local == 1864 || local == 1865 || local == 1866 ||
                            local == 1867 || local == 1868 || local == 1869 || local == 1870 ||
                            local == 1871 || local == 1872 || local == 1873 || local == 1874 ||
                            local == 1875 || local == 1876 || local == 1877 || local == 1878 ||
                            local == 1879 || local == 1880 || local == 1881 || local == 1882 ||
                            local == 1883 || local == 1884 || local == 1885 || local == 1886 ||
                            local == 1887 || local == 1888 || local == 1889 || local == 1890 ||
                            local == 1891 || local == 1892 || local == 1893 || local == 1894 ||
                            local == 1895)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "04302")
                    {
                        if (local == 137 || local == 138 || local == 139 || local == 140 ||
                            local == 1896 || local == 1897 || local == 1898 || local == 1899 ||
                            local == 1900 || local == 1901 || local == 1902 || local == 1903 ||
                            local == 1904 || local == 1905 || local == 1906 || local == 1907 ||
                            local == 1908 || local == 1909 || local == 1910 || local == 1911 ||
                            local == 1912 || local == 1913 || local == 1914 || local == 1915 ||
                            local == 1916 || local == 1917 || local == 1918 || local == 1919)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "04303")
                    {
                        if (local == 141 || local == 142 || local == 143 || local == 144 ||
                            local == 145 || local == 146 || local == 147 || local == 148 ||
                            local == 149 || local == 150 || local == 151 || local == 152 ||
                            local == 153 || local == 154 || local == 155 || local == 156 ||
                            local == 1920 || local == 1921 || local == 1922 || local == 1923 ||
                            local == 1924 || local == 1925 || local == 1926 || local == 1927 ||
                            local == 1928 || local == 1929 || local == 1930 || local == 1931 ||
                            local == 1932 || local == 1933 || local == 1934 || local == 1935 ||
                            local == 1936 || local == 1937 || local == 1938 || local == 1939 ||
                            local == 1940 || local == 1941 || local == 1942 || local == 1943 ||
                            local == 1944 || local == 1945 || local == 1946 || local == 1947 ||
                            local == 1948 || local == 1949 || local == 1950 || local == 1951 ||
                            local == 1952 || local == 1953 || local == 1954 || local == 1955 ||
                            local == 1956 || local == 1957 || local == 1958 || local == 1959 ||
                            local == 1960 || local == 1961 || local == 1962 || local == 1963)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "04304")
                    {
                        if (local == 180 || local == 1964 || local == 1965 || local == 1966 ||
                            local == 1967 || local == 1968 || local == 1969 || local == 1970 ||
                            local == 1971 || local == 1972 || local == 1973 || local == 1974 ||
                            local == 1975)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "04305")
                    {
                        if (local == 181 || local == 182 || local == 183 || local == 184 ||
                            local == 185 || local == 186 || local == 187 || local == 188)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            metodoMensaje(dest); adyacente = true;
                        }
                    }*/
                    break;
                #endregion
                case 5:
                    #region Quinta Región
                    if (dest.IdComuna == "05102")
                    {
                        if (local == 331 || local == 332 || local == 333 || local == 334 ||
                            local == 335 || local == 336 || local == 337 || local == 2004 ||
                            local == 2005 || local == 2006 || local == 2007 || local == 2008 ||
                            local == 2009 || local == 2010 || local == 2011 || local == 2012 ||
                            local == 2013)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05104")
                    {
                        if (local == 2344 || local == 2345)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05105")
                    {
                        if (local == 338 || local == 339 || local == 340 || local == 2026 ||
                            local == 2027 || local == 2028 || local == 2029 || local == 2030 ||
                            local == 2031)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05201")
                    {
                        if (local == 2047 || local == 2048 || local == 2049)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05301")
                    {
                        if (local == 195 || local == 196 || local == 197 || local == 198 ||
                            local == 2050 || local == 2051 || local == 2052 || local == 2053 ||
                            local == 2054 || local == 2055 || local == 2056 || local == 2057 ||
                            local == 2058 || local == 2059 || local == 2060)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05302")
                    {
                        if (local == 189 || local == 190 || local == 191 || local == 192 ||
                            local == 193 || local == 194 || local == 2061 || local == 2062 ||
                            local == 2063 || local == 2064 || local == 2065 || local == 2066 ||
                            local == 2067 || local == 2068 || local == 2069 || local == 2070 ||
                            local == 2071 || local == 2072 || local == 2073 || local == 2074 ||
                            local == 2075)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05303")
                    {
                        if (local == 199 || local == 200 || local == 2076 || local == 2077 ||
                            local == 2078 || local == 2079 || local == 2080)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05304")
                    {
                        if (local == 201 || local == 202 || local == 203 || local == 2081 ||
                            local == 2082 || local == 2083 || local == 2084 || local == 2085 ||
                            local == 2086 || local == 2087 || local == 2088 || local == 2089 ||
                            local == 2090 || local == 2091 || local == 2092 || local == 2093 ||
                            local == 2094 || local == 2095 || local == 2096 || local == 2097 ||
                            local == 2098 || local == 2099 || local == 2100 || local == 2101 ||
                            local == 2102 || local == 2103 || local == 2104 || local == 2105 ||
                            local == 2106 || local == 2107 || local == 2108)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05401")
                    {
                        if (local == 213 || local == 214 || local == 215 || local == 216 ||
                            local == 217 || local == 218 || local == 219 || local == 220 ||
                            local == 221 || local == 222 || local == 223 || local == 224 ||
                            local == 225 || local == 2109 || local == 2110 || local == 2111 ||
                            local == 2112 || local == 2113 || local == 2114 || local == 2115 ||
                            local == 2116 || local == 2117 || local == 2118 || local == 2119 ||
                            local == 2120 || local == 2121 || local == 2122 || local == 2123 ||
                            local == 2124 || local == 2125 || local == 2126 || local == 2127)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05402")
                    {
                        if (local == 204 || local == 205 || local == 206 || local == 207 ||
                            local == 208 || local == 209 || local == 210 || local == 211 ||
                            local == 212 || local == 2128 || local == 2129 || local == 2130 ||
                            local == 2131 || local == 2132 || local == 2133 || local == 2134 ||
                            local == 2135 || local == 2136 || local == 2137 || local == 2138 ||
                            local == 2139 || local == 2140 || local == 2141 || local == 2142)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05403")
                    {
                        if (local == 226 || local == 227 || local == 228 || local == 2143 ||
                            local == 2144 || local == 2145 || local == 2146)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05404")
                    {
                        if (local == 229 || local == 230 || local == 231 || local == 232 ||
                            local == 233 || local == 234 || local == 235 || local == 236 ||
                            local == 237 || local == 2147 || local == 2148 || local == 2149 ||
                            local == 2150 || local == 2151 || local == 2152 || local == 2153 ||
                            local == 2154 || local == 2155 || local == 2156 || local == 2157)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05405")
                    {
                        if (local == 238 || local == 239 || local == 240 || local == 241 ||
                            local == 2158 || local == 2159 || local == 2160 || local == 2161 ||
                            local == 2162 || local == 2163 || local == 2164)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05502")
                    {
                        if (local == 242 || local == 243 || local == 2183 || local == 2184 ||
                            local == 2185 || local == 2186)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05503")
                    {
                        if (local == 244 || local == 245 || local == 246 || local == 247 ||
                            local == 248 || local == 249 || local == 250)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05504")
                    {
                        if (local == 251 || local == 252 || local == 253 || local == 2195 ||
                            local == 2196 || local == 2197 || local == 2198 || local == 2199 ||
                            local == 2200)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05506")
                    {
                        if (local == 262 || local == 263 || local == 264)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05601")
                    {
                        if (local == 2203 || local == 2206)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05603")
                    {
                        if (local == 3961 || local == 3962)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "05606")
                    {
                        if (local == 3963 || local == 284)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "05701")
                    {
                        if (local == 316 || local == 317 || local == 318 || local == 319 ||
                            local == 320 || local == 321 || local == 322 || local == 323 ||
                            local == 324 || local == 2236 || local == 2237 || local == 2238 ||
                            local == 2239 || local == 2240 || local == 2241 || local == 2242 ||
                            local == 2243 || local == 2244 || local == 2245 || local == 2246 ||
                            local == 2247 || local == 2248 || local == 2249 || local == 2250 ||
                            local == 2251 || local == 2252 || local == 2253)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05702")
                    {
                        if (local == 285 || local == 286 || local == 287 || local == 288 ||
                            local == 289 || local == 290 || local == 291 || local == 292 ||
                            local == 2254 || local == 2255 || local == 2256 || local == 2257 ||
                            local == 2258 || local == 2259 || local == 2260 || local == 2261 ||
                            local == 2262 || local == 2263)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05703")
                    {
                        if (local == 293 || local == 294 || local == 295 || local == 296 ||
                            local == 297 || local == 298 || local == 299 || local == 300 ||
                            local == 2264 || local == 2265 || local == 2266 || local == 2267 ||
                            local == 2268 || local == 2269 || local == 2270 || local == 2271 ||
                            local == 2272)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05704")
                    {
                        if (local == 301 || local == 302 || local == 303 || local == 304 ||
                            local == 305 || local == 306 || local == 2273)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05705")
                    {
                        if (local == 307 || local == 308 || local == 309 || local == 310 ||
                            local == 311 || local == 312 || local == 313 || local == 314 ||
                            local == 315 || local == 2274 || local == 2275 || local == 2276 ||
                            local == 2277 || local == 2278 || local == 2279 || local == 2280 ||
                            local == 2281 || local == 2282 || local == 2283 || local == 2284 ||
                            local == 2285 || local == 2286 || local == 2287 || local == 2288 ||
                            local == 2289 || local == 2290 || local == 2291 || local == 2292 ||
                            local == 2293 || local == 2294 || local == 2295 || local == 2296 ||
                            local == 2297 || local == 2298)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "05706")
                    {
                        if (local == 325 || local == 326 || local == 327 || local == 328 ||
                            local == 329 || local == 330 || local == 2299 || local == 2300 ||
                            local == 2301 || local == 2302 || local == 2303 || local == 2304 ||
                            local == 2305 || local == 2306)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            metodoMensaje(dest); adyacente = true;
                        }
                    }*/

                    break;
                #endregion
                case 6:
                    #region Sexta Región                            
                    if (dest.IdComuna == "06102")
                    {
                        if (local == 2396)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "06104")
                    {
                        if (local == 2419 || local == 2420)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "06107")
                    {
                        if (local == 379 || local == 380 || local == 381 || local == 382 || local == 383 ||
                            local == 384 || local == 385 || local == 386 || local == 387 || local == 2457 ||
                            local == 2458 || local == 2459 || local == 2460 || local == 2461 || local == 2462 ||
                            local == 2463 || local == 2464 || local == 2465 || local == 2466 || local == 2467 ||
                            local == 2468 || local == 2469 || local == 2470 || local == 2471 || local == 2472 ||
                            local == 2473 || local == 2474 || local == 2475 || local == 2476 || local == 2477 ||
                            local == 2478 || local == 2479 || local == 2480 || local == 2481 || local == 2482 ||
                            local == 2483 || local == 2484 || local == 2485 || local == 2486 || local == 2487 ||
                            local == 2488 || local == 2489 || local == 2490 || local == 2491 || local == 2492 ||
                            local == 2493 || local == 2494 || local == 2495 || local == 2496 || local == 2497 ||
                            local == 2498 || local == 2499 || local == 2500 || local == 2501 || local == 2502 ||
                            local == 2503 || local == 2504 || local == 2505 || local == 2506)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06108")
                    {
                        if (local == 3965 || local == 3966 ||
                            local == 3967 || local == 2508 ||
                            local == 388)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "06109")
                    {
                        if (local == 391 || local == 392 || local == 393 || local == 394 || local == 395 ||
                            local == 396 || local == 397 || local == 398 || local == 399 || local == 400 ||
                            local == 401 || local == 2512 || local == 2513 || local == 2514 || local == 2515 ||
                            local == 2516 || local == 2517 || local == 2518 || local == 2519 || local == 2520 ||
                            local == 2521 || local == 2522 || local == 2523 || local == 2524 || local == 2525 ||
                            local == 2526)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06110")
                    {
                        if (local == 3968 || local == 403)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "06112")
                    {
                        if (local == 408 || local == 409 || local == 410 || local == 411 || local == 2548 ||
                            local == 2549 || local == 2550 || local == 2551 || local == 2552 || local == 2553 ||
                            local == 2554 || local == 2555 || local == 2556 || local == 2557)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06113")
                    {
                        if (local == 412 || local == 413 || local == 414 || local == 415 || local == 416 ||
                            local == 417 || local == 418 || local == 419 || local == 2558 || local == 2559 ||
                            local == 2560 || local == 2561 || local == 2562 || local == 2563 || local == 2564 ||
                            local == 2565 || local == 2566 || local == 2567 || local == 2568 || local == 2569 ||
                            local == 2570 || local == 2571 || local == 2572 || local == 2573 || local == 2574 ||
                            local == 2575 || local == 2576 || local == 2577 || local == 2578 || local == 2579 ||
                            local == 2580 || local == 2581)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06114")
                    {
                        if (local == 420 || local == 421 || local == 422 || local == 423 || local == 424 ||
                            local == 425 || local == 426 || local == 427 || local == 2582 || local == 2583 ||
                            local == 2584 || local == 2585 || local == 2586 || local == 2587 || local == 2588 ||
                            local == 2589 || local == 2590 || local == 2591 || local == 2592 || local == 2593 ||
                            local == 2594)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06115")
                    {
                        if (local == 402 || local == 434 || local == 435 || local == 436 || local == 437 ||
                            local == 438 || local == 439 || local == 440 || local == 441 || local == 442 ||
                            local == 443 || local == 444 || local == 445 || local == 446 || local == 447 ||
                            local == 448 || local == 449 || local == 450 || local == 451 || local == 2595 ||
                            local == 2596 || local == 2597 || local == 2598 || local == 2599 || local == 2600 ||
                            local == 2601 || local == 2602 || local == 2603 || local == 2604 || local == 2605 ||
                            local == 2606 || local == 2607 || local == 2608 || local == 2609 || local == 2610 ||
                            local == 2611 || local == 2612 || local == 2613 || local == 2614 || local == 2615 ||
                            local == 2616 || local == 2617 || local == 2618 || local == 2619 || local == 2620 ||
                            local == 2621 || local == 2622)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06116")
                    {
                        if (local == 3964)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "06117")
                    {
                        if (local == 465 || local == 466 || local == 467 || local == 468 || local == 469 ||
                            local == 470 || local == 471 || local == 472 || local == 473 || local == 474 ||
                            local == 475 || local == 476 || local == 477 || local == 478 || local == 479 ||
                            local == 480 || local == 481 || local == 482 || local == 483 || local == 484 ||
                            local == 2641 || local == 2642 || local == 2643 || local == 2644 || local == 2645 ||
                            local == 2646 || local == 2647 || local == 2648 || local == 2649 || local == 2650 ||
                            local == 2651 || local == 2652 || local == 2653 || local == 2654 || local == 2655 ||
                            local == 2656 || local == 2657 || local == 2658 || local == 2659 || local == 2660 ||
                            local == 2661 || local == 2662 || local == 2663 || local == 2664 || local == 2665 ||
                            local == 2666 || local == 2667 || local == 2668 || local == 2669 || local == 2670 ||
                            local == 2671 || local == 2672 || local == 2673 || local == 2674 || local == 2675 ||
                            local == 2676 || local == 2677 || local == 2678 || local == 2679 || local == 2680 ||
                            local == 2681 || local == 2682 || local == 2683 || local == 2684 || local == 2685 ||
                            local == 2686 || local == 2687 || local == 2688 || local == 2689 || local == 2690 ||
                            local == 2691 || local == 2692 || local == 2693 || local == 2694 || local == 2695 ||
                            local == 2696 || local == 2697 || local == 2698 || local == 2699 || local == 2700)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06201")
                    {
                        if (local == 498 || local == 2701 || local == 2702 || local == 2703 ||
                            local == 2704 || local == 2705 || local == 2706 || local == 2707)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06202")
                    {
                        if (local == 485 || local == 2708 || local == 2709 || local == 2710 || local == 2711 ||
                            local == 2712 || local == 2713 || local == 2714 || local == 2715 || local == 2716 ||
                            local == 2717 || local == 2718 || local == 2719 || local == 2720 || local == 2721 ||
                            local == 2722)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06203")
                    {
                        if (local == 486 || local == 2723 || local == 2724 || local == 2725 || local == 2726 ||
                            local == 2727 || local == 2728 || local == 2729 || local == 2730)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06204")
                    {
                        if (local == 2731 || local == 2732 || local == 2733 || local == 2734 || local == 2735 ||
                            local == 2736 || local == 2737 || local == 2738 || local == 2739 || local == 2740 ||
                            local == 2741 || local == 2742 || local == 2743 || local == 2744)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06205")
                    {
                        if (local == 2974 || local == 2975 || local == 2976 || local == 2977 || local == 2978 ||
                            local == 2979 || local == 2980 || local == 2981 || local == 2982 || local == 2983 ||
                            local == 2984 || local == 2985 || local == 2986 || local == 2987 || local == 2988 ||
                            local == 2989 || local == 2990 || local == 2991 || local == 2992 || local == 2993 ||
                            local == 2994 || local == 2995 || local == 2996 || local == 2997 || local == 2998 ||
                            local == 2999 || local == 3000 || local == 3001 || local == 3002)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06206")
                    {
                        if (local == 2745 || local == 2746 || local == 2747 || local == 2748 || local == 2749 ||
                            local == 2750 || local == 2751 || local == 2752 || local == 2753 || local == 2754 ||
                            local == 2755 || local == 2756 || local == 2757 || local == 2758 || local == 2759 ||
                            local == 2760 || local == 2761 || local == 2762 || local == 2763 || local == 2764 ||
                            local == 2765 || local == 2766 || local == 2767 || local == 2768 || local == 2769 ||
                            local == 2770 || local == 2771 || local == 2772)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06301")
                    {
                        if (local == 563 || local == 564 || local == 565 || local == 566 || local == 567 ||
                            local == 568 || local == 569 || local == 570 || local == 571 || local == 572 ||
                            local == 573 || local == 574 || local == 575 || local == 576 || local == 2773 ||
                            local == 2774 || local == 2775 || local == 2776 || local == 2777 || local == 2778 ||
                            local == 2779 || local == 2780 || local == 2781 || local == 2782 || local == 2783 ||
                            local == 2784 || local == 2785 || local == 2786)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06302")
                    {
                        if (local == 499 || local == 500 || local == 501 || local == 502 || local == 503 ||
                            local == 504 || local == 505 || local == 506 || local == 507 || local == 2787 ||
                            local == 2788 || local == 2789 || local == 2790 || local == 2791 || local == 2792 ||
                            local == 2793 || local == 2794 || local == 2795 || local == 2796 || local == 2797 ||
                            local == 2798 || local == 2799 || local == 2800 || local == 2801 || local == 2802 ||
                            local == 2803 || local == 2804 || local == 2805)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06303")
                    {
                        if (local == 508 || local == 509 || local == 510 || local == 511 || local == 512 ||
                            local == 513 || local == 514 || local == 515 || local == 516 || local == 517 ||
                            local == 518 || local == 519 || local == 520 || local == 521 || local == 522 ||
                            local == 523 || local == 2806 || local == 2807 || local == 2808 || local == 2809 ||
                            local == 2810 || local == 2811 || local == 2812 || local == 2813 || local == 2814 ||
                            local == 2815 || local == 2816 || local == 2817 || local == 2818 || local == 2819 ||
                            local == 2820 || local == 2821 || local == 2822 || local == 2823 || local == 2824 ||
                            local == 2825 || local == 2826 || local == 2827 || local == 2828 || local == 2829 ||
                            local == 2830 || local == 2831 || local == 2832 || local == 2833 || local == 2834)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06304")
                    {
                        if (local == 524 || local == 525 || local == 526 || local == 2835 || local == 2836 ||
                            local == 2837 || local == 2838 || local == 2839 || local == 2840 || local == 2841 ||
                            local == 2842 || local == 2843 || local == 2844 || local == 2845 || local == 2846 ||
                            local == 2847 || local == 2848 || local == 2849 || local == 2850 || local == 2851 ||
                            local == 2852 || local == 2853 || local == 2854 || local == 2855 || local == 2856 ||
                            local == 2857 || local == 2858)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06305")
                    {
                        if (local == 527 || local == 528 || local == 529 || local == 530 || local == 531 ||
                            local == 532 || local == 533 || local == 534 || local == 535 || local == 2859 ||
                            local == 2860 || local == 2861 || local == 2862 || local == 2863 || local == 2864 ||
                            local == 2865 || local == 2866 || local == 2867 || local == 2868 || local == 2869 ||
                            local == 2870 || local == 2871 || local == 2872 || local == 2873 || local == 2874 ||
                            local == 2875 || local == 2876)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06306")
                    {
                        if (local == 536 || local == 537 || local == 538 || local == 539 || local == 540 ||
                            local == 541 || local == 542 || local == 543 || local == 2877 || local == 2878 ||
                            local == 2879 || local == 2880 || local == 2881 || local == 2882 || local == 2883 ||
                            local == 2884 || local == 2885 || local == 2886 || local == 2887 || local == 2888 ||
                            local == 2889 || local == 2890 || local == 2891 || local == 2892 || local == 2893 ||
                            local == 2894 || local == 2895 || local == 2896 || local == 2897)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06307")
                    {
                        if (local == 544 || local == 545 || local == 546 || local == 547 || local == 548 ||
                            local == 549 || local == 550 || local == 2898 || local == 2899 || local == 2900 ||
                            local == 2901 || local == 2902 || local == 2903 || local == 2904 || local == 2905 ||
                            local == 2906 || local == 2907 || local == 2908 || local == 2909 || local == 2910 ||
                            local == 2911 || local == 2912 || local == 2913 || local == 2914 || local == 2915)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06308")
                    {
                        if (local == 551 || local == 552 || local == 553 || local == 554 || local == 555 ||
                            local == 556 || local == 557 || local == 558 || local == 559 || local == 2916 ||
                            local == 2917 || local == 2918 || local == 2919 || local == 2920 || local == 2921 ||
                            local == 2922 || local == 2923 || local == 2924 || local == 2925 || local == 2926 ||
                            local == 2927 || local == 2928)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06309")
                    {
                        if (local == 560 || local == 561 || local == 562 || local == 2969 ||
                            local == 2970 || local == 2971 || local == 2972 || local == 2973)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "06310")
                    {
                        if (local == 577 || local == 578 || local == 579 || local == 580 || local == 581 ||
                            local == 582 || local == 583 || local == 584 || local == 585 || local == 586 ||
                            local == 587 || local == 588 || local == 2929 || local == 2930 || local == 2931 ||
                            local == 2932 || local == 2933 || local == 2934 || local == 2935 || local == 2936 ||
                            local == 2937 || local == 2938 || local == 2939 || local == 2940 || local == 2941 ||
                            local == 2942 || local == 2943 || local == 2944 || local == 2945 || local == 2946 ||
                            local == 2947 || local == 2948 || local == 2949 || local == 2950 || local == 2951 ||
                            local == 2952 || local == 2953 || local == 2954 || local == 2955 || local == 2956 ||
                            local == 2957 || local == 2958 || local == 2959 || local == 2960 || local == 2961 ||
                            local == 2962 || local == 2963 || local == 2964 || local == 2965 || local == 2966 ||
                            local == 2967 || local == 2968)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            metodoMensaje(dest); adyacente = true;
                        }
                    }*/
                    break;
                #endregion
                case 7:
                    #region Septima Región
                    if (dest.IdComuna == "07102")
                    {
                        if (local == 810 || local == 811 || local == 812 || local == 813 || local == 814 ||
                            local == 815 || local == 816 || local == 817 || local == 818 || local == 819 ||
                            local == 3017 || local == 3018 || local == 3019 || local == 3020 || local == 3021 ||
                            local == 3022 || local == 3023 || local == 3024 || local == 3025 || local == 3026 ||
                            local == 3027 || local == 3028 || local == 3029 || local == 3030 || local == 3031 ||
                            local == 3032 || local == 3033 || local == 3034 || local == 3035 || local == 3036 ||
                            local == 3037 || local == 3038 || local == 3039 || local == 3040 || local == 3041 ||
                            local == 3042 || local == 3043 || local == 3044 || local == 3045 || local == 3046 ||
                            local == 3047 || local == 3048 || local == 3049 || local == 3050 || local == 3051 ||
                            local == 3052 || local == 3053)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07103")
                    {
                        if (local == 820 || local == 821 || local == 822 || local == 823 || local == 824 ||
                            local == 825 || local == 826 || local == 827 || local == 828 || local == 829 ||
                            local == 3054 || local == 3055 || local == 3056 ||
                            local == 3057 || local == 3058 || local == 3059)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07104")
                    {
                        if (local == 830 || local == 831 || local == 832 ||
                            local == 3060 || local == 3061 || local == 3062 || local == 3063)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07108")
                    {
                        if (local == 862 || local == 863 || local == 864 || local == 865 || local == 866 ||
                            local == 867 || local == 868 || local == 869 || local == 870 || local == 871 ||
                            local == 872 || local == 873 || local == 874 || local == 875 || local == 876 ||
                            local == 877 || local == 878 || local == 879 || local == 880 ||
                            local == 3101 || local == 3102 || local == 3103 || local == 3104 || local == 3105 ||
                            local == 3106 || local == 3107 || local == 3108 || local == 3109 || local == 3110 ||
                            local == 3111 || local == 3112 || local == 3113 || local == 3114 || local == 3115 ||
                            local == 3116 || local == 3117 || local == 3118 || local == 3119 || local == 3120)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07109")
                    {
                        if (local == 3969 || local == 3970 || local == 3971 || local == 3972 ||
                            local == 3973 || local == 896 || local == 882 || local == 895 ||
                            local == 3983)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "07201")
                    {
                        if (local == 589 || local == 590 || local == 591 || local == 592 || local == 593 ||
                            local == 594 || local == 595 || local == 596 || local == 3171 || local == 3172 ||
                            local == 3173 || local == 3174 || local == 3175 || local == 3176 || local == 3177)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07202")
                    {
                        if (local == 597 || local == 598 || local == 599 || local == 600 || local == 601 ||
                            local == 602 || local == 603 || local == 604 || local == 3178 || local == 3179 ||
                            local == 3180)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }

                    }
                    else if (dest.IdComuna == "07203")
                    {
                        if (local == 605 || local == 606 || local == 607 || local == 608 || local == 609 ||
                            local == 3181 || local == 3182 || local == 3183 || local == 3184 || local == 3185 ||
                            local == 3186 || local == 3187 || local == 3188 || local == 3189)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07301")
                    {
                        if (local == 610 || local == 611 || local == 612 || local == 613 || local == 614 || local == 615 ||
                            local == 616 || local == 617 || local == 618 || local == 619 || local == 620 || local == 621 ||
                            local == 622 || local == 623 || local == 624 || local == 3190 || local == 3191 || local == 3192 ||
                            local == 3193 || local == 3194 || local == 3195 || local == 3196 || local == 3197 || local == 3198 ||
                            local == 3199 || local == 3200 || local == 3201 || local == 3202 || local == 3203 || local == 3204 ||
                            local == 3205 || local == 3206 || local == 3207 || local == 3208 || local == 3209 || local == 3210 ||
                            local == 3211 || local == 3212 || local == 3213 || local == 3214 || local == 3215 || local == 3216 ||
                            local == 3217 || local == 3218 || local == 3219 || local == 3220 || local == 3221 || local == 3222 ||
                            local == 3223 || local == 3224 || local == 3225 || local == 3226 || local == 3227 || local == 3228 ||
                            local == 3229 || local == 3230 || local == 3231 || local == 3232 || local == 3233 || local == 3234 ||
                            local == 3235 || local == 3236 || local == 3237 || local == 3238 || local == 3239 || local == 3240 ||
                            local == 3241)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07302")
                    {
                        if (local == 625 || local == 626 || local == 627 || local == 628 || local == 3242 ||
                            local == 3243 || local == 3244 || local == 3245 || local == 3246 || local == 3247)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07303")
                    {
                        if (local == 629 || local == 630 || local == 631 || local == 632 || local == 633 ||
                            local == 634 || local == 3248 || local == 3249 || local == 3250 || local == 3251 ||
                            local == 3252)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07304")
                    {
                        if (local == 635 || local == 636 || local == 637 || local == 638 || local == 639 ||
                            local == 640 || local == 641 || local == 642 || local == 643 || local == 644 ||
                            local == 744 || local == 3253 || local == 3254 || local == 3255 || local == 3256 ||
                            local == 3257 || local == 3258 || local == 3259 || local == 3260 || local == 3261 ||
                            local == 3262 || local == 3263 || local == 3264 || local == 3265 || local == 3266 ||
                            local == 3267 || local == 3268 || local == 3269)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07305")
                    {
                        if (local == 645 || local == 646 || local == 647 || local == 648 || local == 649 ||
                            local == 650 || local == 651 || local == 652 || local == 653 || local == 3270 ||
                            local == 3271 || local == 3272 || local == 3273 || local == 3274 || local == 3275 ||
                            local == 3276 || local == 3277 || local == 3278 || local == 3279)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07306")
                    {
                        if (local == 654 || local == 655 || local == 656 || local == 657 || local == 658 ||
                            local == 659 || local == 660 || local == 661 || local == 662 || local == 663 ||
                            local == 664 || local == 665 || local == 3280 || local == 3281 || local == 3282 ||
                            local == 3283 || local == 3284 || local == 3285 || local == 3286 || local == 3287 ||
                            local == 3288 || local == 3289 || local == 3290 || local == 3291 || local == 3292 ||
                            local == 3293 || local == 3294 || local == 3295 || local == 3296 || local == 3297 ||
                            local == 3298 || local == 3299 || local == 3300)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07307")
                    {
                        if (local == 666 || local == 667 || local == 668 || local == 669 || local == 670 ||
                            local == 671 || local == 672 || local == 673 || local == 674 || local == 675 ||
                            local == 676 || local == 677 || local == 678 || local == 679 || local == 3301 ||
                            local == 3302 || local == 3303 || local == 3304 || local == 3305 || local == 3306 ||
                            local == 3307 || local == 3308 || local == 3309 || local == 3310 || local == 3311 ||
                            local == 3312 || local == 3313 || local == 3314 || local == 3315 || local == 3316 ||
                            local == 3317 || local == 3318 || local == 3319 || local == 3320 || local == 3321 ||
                            local == 3322 || local == 3323 || local == 3324 || local == 3325 || local == 3326 ||
                            local == 3327)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07308")
                    {
                        if (local == 680 || local == 681 || local == 682 || local == 683 || local == 684 || local == 685 ||
                            local == 686 || local == 687 || local == 688 || local == 689 || local == 690 || local == 691 ||
                            local == 692 || local == 693 || local == 694 || local == 695 || local == 696 || local == 697 ||
                            local == 3328 || local == 3329 || local == 3330 || local == 3331 || local == 3332 || local == 3333 ||
                            local == 3334 || local == 3335 || local == 3336 || local == 3337 || local == 3338 || local == 3339 ||
                            local == 3340 || local == 3341 || local == 3342 || local == 3343 || local == 3344 || local == 3345 ||
                            local == 3346 || local == 3347 || local == 3348 || local == 3349 || local == 3350 || local == 3351 ||
                            local == 3352 || local == 3353 || local == 3354 || local == 3355 || local == 3356 || local == 3357 ||
                            local == 3358 || local == 3359 || local == 3360 || local == 3361 || local == 3362 || local == 3363 ||
                            local == 3364 || local == 3365 || local == 3366 || local == 3367 || local == 3368)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07309")
                    {
                        if (local == 698 || local == 699 || local == 700 || local == 701 || local == 702 ||
                            local == 3369 || local == 3370 || local == 3371 || local == 3372 || local == 3373 ||
                            local == 3374 || local == 3375 || local == 3376 || local == 3377 || local == 3378)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07401")
                    {
                        if (local == 717 || local == 718 || local == 719 || local == 720 || local == 721 || local == 722 ||
                            local == 723 || local == 724 || local == 725 || local == 726 || local == 727 || local == 3379 ||
                            local == 3380 || local == 3381 || local == 3382 || local == 3383 || local == 3384 || local == 3385 ||
                            local == 3386 || local == 3387 || local == 3388 || local == 3389 || local == 3390 || local == 3391 ||
                            local == 3392 || local == 3393 || local == 3394 || local == 3395 || local == 3396 || local == 3397 ||
                            local == 3398 || local == 3399 || local == 3400 || local == 3401 || local == 3402 || local == 3403 ||
                            local == 3404 || local == 3405 || local == 3406 || local == 3407 || local == 3408 || local == 3409 ||
                            local == 3410 || local == 3411 || local == 3412 || local == 3413 || local == 3414 || local == 3415 ||
                            local == 3416 || local == 3417 || local == 3418)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07402")
                    {
                        if (local == 703 || local == 704 || local == 705 || local == 706 || local == 707 ||
                            local == 708 || local == 709 || local == 710 || local == 711 || local == 712 ||
                            local == 713 || local == 714 || local == 715 || local == 716 || local == 3419 ||
                            local == 3420 || local == 3421 || local == 3422 || local == 3423 || local == 3424 ||
                            local == 3425 || local == 3426 || local == 3427 || local == 3428 || local == 3429 ||
                            local == 3430 || local == 3431 || local == 3432 || local == 3433 || local == 3434 ||
                            local == 3435 || local == 3436 || local == 3437 || local == 3438 || local == 3439 ||
                            local == 3440 || local == 3441 || local == 3442 || local == 3443 || local == 3444 ||
                            local == 3445 || local == 3446 || local == 3447 || local == 3448 || local == 3449 ||
                            local == 3450 || local == 3451 || local == 3452 || local == 3453 || local == 3454)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07403")
                    {
                        if (local == 728 || local == 729 || local == 730 || local == 731 || local == 732 ||
                            local == 733 || local == 734 || local == 735 || local == 736 || local == 737 ||
                            local == 738 || local == 739 || local == 740 || local == 741 || local == 742 ||
                            local == 743 || local == 3455 || local == 3456 || local == 3457 || local == 3458 ||
                            local == 3459 || local == 3460 || local == 3461 || local == 3462 || local == 3463 ||
                            local == 3464 || local == 3465 || local == 3466 || local == 3467 || local == 3468 ||
                            local == 3469 || local == 3470 || local == 3471 || local == 3472 || local == 3473 ||
                            local == 3474 || local == 3475 || local == 3476 || local == 3477 || local == 3478 ||
                            local == 3479 || local == 3480 || local == 3481 || local == 3482 || local == 3483 ||
                            local == 3484)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07404")
                    {
                        if (local == 745 || local == 746 || local == 747 || local == 748 || local == 749 ||
                            local == 750 || local == 751 || local == 752 || local == 753 || local == 754 ||
                            local == 3485 || local == 3486 || local == 3487 || local == 3488 || local == 3489 ||
                            local == 3490 || local == 3491 || local == 3492 || local == 3493 || local == 3494 ||
                            local == 3495 || local == 3496 || local == 3497)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07405")
                    {
                        if (local == 755 || local == 756 || local == 757 || local == 758 || local == 759 ||
                            local == 760 || local == 761 || local == 762 || local == 3498 || local == 3499 ||
                            local == 3500 || local == 3501 || local == 3502 || local == 3503 || local == 3504 ||
                            local == 3505 || local == 3506 || local == 3507 || local == 3508 || local == 3509 ||
                            local == 3510 || local == 3511 || local == 3512 || local == 3513 || local == 3514 ||
                            local == 3515 || local == 3516 || local == 3517)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07406")
                    {
                        if (local == 763 || local == 764 || local == 765 || local == 766 || local == 767 ||
                            local == 768 || local == 769 || local == 770 || local == 772 || local == 773 ||
                            local == 774 || local == 775 || local == 776 || local == 777 || local == 778 ||
                            local == 3518 || local == 3519 || local == 3520 || local == 3521 || local == 3522 ||
                            local == 3523 || local == 3524 || local == 3525 || local == 3526 || local == 3527 ||
                            local == 3528 || local == 3529 || local == 3530 || local == 3531 || local == 3532 ||
                            local == 3533 || local == 3534 || local == 3535 || local == 3536 || local == 3537 ||
                            local == 3538 || local == 3539 || local == 3540 || local == 3541 || local == 3542 ||
                            local == 3543 || local == 3544 || local == 3545 || local == 3546 || local == 3547 ||
                            local == 911)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07407")
                    {
                        if (local == 779 || local == 780 || local == 781 || local == 782 || local == 783 ||
                            local == 784 || local == 785 || local == 786 || local == 787 || local == 788 ||
                            local == 789 || local == 790 || local == 791 || local == 792 || local == 3548 ||
                            local == 3549 || local == 3550 || local == 3551 || local == 3552 || local == 3553 ||
                            local == 3554)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "07408")
                    {
                        if (local == 793 || local == 794 || local == 795 || local == 796 || local == 797 ||
                            local == 798 || local == 799 || local == 800 || local == 801 || local == 802 ||
                            local == 803 || local == 804 || local == 805 || local == 806 || local == 807 ||
                            local == 808 || local == 809 || local == 3555 || local == 3556 || local == 3557 ||
                            local == 3558 || local == 3559 || local == 3560 || local == 3561 || local == 3562 ||
                            local == 3563 || local == 3564 || local == 3565 || local == 3566 || local == 3567 ||
                            local == 3568 || local == 3569 || local == 3570 || local == 3571 || local == 3572 ||
                            local == 3573 || local == 3574 || local == 3575 || local == 3576 || local == 3577 ||
                            local == 3578 || local == 3579)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            metodoMensaje(dest); adyacente = true;
                        }
                    }*/
                    break;
                #endregion
                case 8:
                    #region Octava Región
                    if (dest.IdComuna == "08104")
                    {
                        if (local == 1044 || local == 1045 || local == 3599 || local == 3600 || local == 3601 || local == 3602 || local == 3603)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08105")
                    {
                        if (local == 1047 || local == 1048 || local == 1049 || local == 1050 || local == 3604)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08109")
                    {
                        if (local == 1053 || local == 3611 || local == 3612 || local == 3613 ||
                            local == 3614 || local == 3615 || local == 3616 || local == 3617)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08111")
                    {
                        if (local == 1055 || local == 1056 || local == 1057 || local == 1058 || local == 1059 ||
                            local == 1060 || local == 3621 || local == 3622 || local == 3623 || local == 3624 ||
                            local == 3625 || local == 3626 || local == 3627 || local == 3628 || local == 3629 ||
                            local == 3630 || local == 3631 || local == 3632 || local == 3633 || local == 3634 ||
                            local == 3635 || local == 3636 || local == 3637 || local == 3638 || local == 3639 ||
                            local == 3640 || local == 3641 || local == 3642 || local == 3643)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08201")
                    {
                        if (local == 945 || local == 946 || local == 947 || local == 3646 || local == 3647)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08202")
                    {
                        if (local == 928 || local == 929 || local == 930 || local == 931 || local == 932 ||
                            local == 933 || local == 934 || local == 3648 || local == 3649 || local == 3650 ||
                            local == 3651 || local == 3652 || local == 3653 || local == 3654 || local == 3655 ||
                            local == 3656 || local == 3657)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08203")
                    {
                        if (local == 935 || local == 936 || local == 937 || local == 938 || local == 939 ||
                            local == 940 || local == 941 || local == 3658 || local == 3659 || local == 3660 ||
                            local == 3661 || local == 3662 || local == 3663 || local == 3664 || local == 3665 ||
                            local == 3666 || local == 3667 || local == 3668 || local == 3669 || local == 3670 ||
                            local == 3671 || local == 3672)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08204")
                    {
                        if (local == 942 || local == 943 || local == 3673 || local == 3674 || local == 3675 || local == 3676)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08205")
                    {
                        if (local == 944 || local == 3677 || local == 3678 || local == 3679 || local == 3680 || local == 3681)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08206")
                    {
                        if (local == 948 || local == 949 || local == 950 || local == 951 || local == 952)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08207")
                    {
                        if (local == 953 || local == 954 || local == 955 || local == 956)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08301")
                    {
                        if (local == 980 || local == 981 || local == 982 || local == 983 || local == 984 ||
                            local == 985 || local == 986 || local == 987 || local == 988 || local == 989 ||
                            local == 990 || local == 992 || local == 993 || local == 994 || local == 995 ||
                            local == 996 || local == 997 || local == 998 || local == 999 || local == 1000 ||
                            local == 1001 || local == 1002 || local == 1003 || local == 1004 || local == 1005 ||
                            local == 1006 || local == 1007 || local == 1008 || local == 1009 ||
                            local == 3682 || local == 3683 || local == 3684 || local == 3685 || local == 3686 ||
                            local == 3687 || local == 3688 || local == 3689 || local == 3690 || local == 3691 ||
                            local == 3692 || local == 3693 || local == 3694 || local == 3695 || local == 3696 ||
                            local == 3697 || local == 3698 || local == 3699 || local == 3700 || local == 3701 ||
                            local == 3702 || local == 3703 || local == 3704 || local == 3705 || local == 3706 ||
                            local == 3707 || local == 3708 || local == 3709 || local == 3710 || local == 3711 ||
                            local == 3712 || local == 3713 || local == 3714 || local == 3715 || local == 3716 ||
                            local == 3717 || local == 3718 || local == 3719 || local == 3720 || local == 3721 ||
                            local == 3722 || local == 3723 || local == 3724 || local == 3725 || local == 3726 ||
                            local == 3727 || local == 3728 || local == 3729 || local == 3730 || local == 3731 ||
                            local == 3732 || local == 3733 || local == 3734 || local == 3735 || local == 3736 ||
                            local == 3737 || local == 3738 || local == 3739 || local == 3740 || local == 3741 ||
                            local == 3742 || local == 3743 || local == 3744 || local == 3745 || local == 3746 ||
                            local == 3747 || local == 3748 || local == 3749 || local == 3750 || local == 3751)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08302")
                    {
                        if (local == 970 || local == 971 || local == 972 || local == 3752 ||
                            local == 3753 || local == 3754 || local == 3755)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08303")
                    {
                        if (local == 973 || local == 974 || local == 975 || local == 976 || local == 977 ||
                            local == 978 || local == 979 || local == 3756 || local == 3757 || local == 3758
                            || local == 3759 || local == 3760 || local == 3761 || local == 3762)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08304")
                    {
                        if (local == 957 || local == 958 || local == 959 || local == 960 || local == 961 ||
                            local == 962 || local == 963 || local == 964 || local == 965 || local == 966 ||
                            local == 3763 || local == 3764 || local == 3765 || local == 3766 || local == 3767 ||
                            local == 3768 || local == 3769 || local == 3770)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08305")
                    {
                        if (local == 1010 || local == 1011 || local == 1012 || local == 3771 ||
                            local == 3772 || local == 3773 || local == 3774 || local == 3775)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08306")
                    {
                        if (local == 1013 || local == 3776 || local == 3777 || local == 3778 || local == 3779 || local == 3780)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08307")
                    {
                        if (local == 1014 || local == 1015 || local == 1016 || local == 1017 || local == 3781 ||
                            local == 3782 || local == 3783 || local == 3784 || local == 3785 || local == 3786 ||
                            local == 3787 || local == 3788 || local == 3789)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08308")
                    {
                        if (local == 1018 || local == 3790 || local == 3791)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08309")
                    {
                        if (local == 1019 || local == 1020 || local == 1021 ||
                            local == 3792 || local == 3793 || local == 3794 ||
                            local == 3795 || local == 3796 || local == 3797)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08310")
                    {
                        if (local == 1022 || local == 3798 || local == 3799 || local == 3800 || local == 3801)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08311")
                    {
                        if (local == 1023 || local == 1024 || local == 1025 ||
                            local == 3802 || local == 3803 || local == 3804)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08312")
                    {
                        if (local == 1026 || local == 1027 || local == 1028 || local == 1029 ||
                            local == 1030 || local == 1031 || local == 3805)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08313")
                    {
                        if (local == 1032 || local == 1033 || local == 1034 || local == 1035 || local == 1036 ||
                            local == 1037 || local == 1038 || local == 3806 || local == 3807 || local == 3808 ||
                            local == 3809 || local == 3810)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "08314")
                    {
                        if (local == 967 || local == 968 || local == 969)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            adyacente = true;
                            metodoMensaje(dest);
                        }
                    }*/
                    break;
                #endregion
                case 9:
                    #region Novena Región
                    if (dest.IdComuna == "09102")
                    {
                        if (local == 1185 || local == 1186 || local == 3818)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09103")
                    {
                        if (local == 1188 || local == 3819)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09104")
                    {
                        if (local == 1189 || local == 1190)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09105")
                    {
                        if (local == 1191 || local == 1192)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09106")
                    {
                        if (local == 1193)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09107")
                    {
                        if (local == 1194 || local == 1195)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09108")
                    {
                        if (local == 1196 || local == 1197 || local == 1198)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09109")
                    {
                        if (local == 1199 || local == 3814)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09110")
                    {
                        if (local == 1200)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09111")
                    {
                        if (local == 1201)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09113")
                    {
                        if (local == 1203)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09114")
                    {
                        if (local == 1204)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09115")
                    {
                        if (local == 1205)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09116")
                    {
                        if (local == 1206 || local == 1207)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09117")
                    {
                        if (local == 1209 || local == 1210)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09118")
                    {
                        if (local == 1211 || local == 3824)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09119")
                    {
                        if (local == 1212 || local == 1213 || local == 3825 || local == 3826)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09120")
                    {
                        if (local == 1214 || local == 1215 || local == 3827)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09121")
                    {
                        if (local == 1187)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09201")
                    {
                        if (local == 1216)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09202")
                    {
                        if (local == 1225 || local == 3812)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09203")
                    {
                        if (local == 1217 || local == 1218)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09204")
                    {
                        if (local == 1219 || local == 3813)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09205")
                    {
                        if (local == 1220 || local == 1221 || local == 1222)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09206")
                    {
                        if (local == 1223)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09207")
                    {
                        if (local == 1224 || local == 1226)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09208")
                    {
                        if (local == 1227)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09209")
                    {
                        if (local == 1228 || local == 3815)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09210")
                    {
                        if (local == 1229)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "09211")
                    {
                        if (local == 1230 || local == 1231 || local == 1232 || local == 3816 || local == 3817)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            metodoMensaje(dest); adyacente = true;
                        }
                    }*/
                    break;
                #endregion
                case 10:
                    #region Decima Región                            
                    if (dest.IdComuna == "10101")
                    {
                        if (local == 3974 || local == 3975 || local == 3976 ||
                            local == 3977 || local == 1251 || local == 1254 ||
                            local == 1248)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "10102")
                    {
                        if (local == 1233 || local == 1234)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10103")
                    {
                        if (local == 1235 || local == 1236 || local == 1237)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10104")
                    {
                        if (local == 1239 || local == 1238)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10106")
                    {
                        if (local == 1240 || local == 1241 || local == 1242)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10108")
                    {
                        if (local == 1243 || local == 1244)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10109")
                    {
                        if (local == 3978 || local == 3979 || local == 1256 ||
                            local == 1258)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "10201")
                    {
                        if (local == 3835)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10202")
                    {
                        if (local == 3834)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10203")
                    {
                        if (local == 3837)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10204")
                    {
                        if (local == 3838)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10205")
                    {
                        if (local == 3839)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10206")
                    {
                        if (local == 3851)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10207")
                    {
                        if (local == 3853)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10208")
                    {
                        if (local == 3854)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10209")
                    {
                        if (local == 3855)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10210")
                    {
                        if (local == 3856)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10301")
                    {
                        if (local == 3846 || local == 3847)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10302")
                    {
                        if (local == 1259 || local == 3849)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10303")
                    {
                        if (local == 1260 || local == 1261 || local == 3852)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10304")
                    {
                        if (local == 1262 || local == 1263)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10305")
                    {
                        if (local == 1264 || local == 1265 || local == 1266)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10306")
                    {
                        if (local == 1267 || local == 1268 || local == 1269 || local == 1270)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10307")
                    {
                        if (local == 1271)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10401")
                    {
                        if (local == 3836)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10402")
                    {
                        if (local == 3842)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10403")
                    {
                        if (local == 1272 || local == 3843)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "10404")
                    {
                        if (local == 3848)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            metodoMensaje(dest); adyacente = true;
                        }
                    }*/
                    break;
                #endregion
                case 11:
                    #region Decimo Primera Región
                    if (dest.IdComuna == "11101")
                    {
                        if (local == 1284 || local == 1285)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "11102")
                    {
                        if (local == 1286)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "11201")
                    {
                        if (local == 3857 || local == 3858)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "11202")
                    {
                        if (local == 1276 || local == 1277 || local == 1278 || local == 1279)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "11203")
                    {
                        if (local == 1280)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "11301")
                    {
                        if (local == 1281)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "11302")
                    {
                        if (local == 1282)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "11303")
                    {
                        if (local == 1283)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "11401")
                    {
                        if (local == 1287 || local == 1288 || local == 1289)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "11402")
                    {
                        if (local == 1290 || local == 1291 || local == 1292 || local == 1293)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            adyacente = true;
                            metodoMensaje(dest);
                        }
                    }*/
                    break;
                #endregion
                case 12:
                    #region Decimo Segunda Región
                    if (dest.IdComuna == "12101")
                    {
                        if (local == 1297 || local == 1298 || local == 1299 || local == 1300 ||
                            local == 1301 || local == 1302 || local == 1303)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "12102")
                    {
                        if (local == 1296)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "12103")
                    {
                        if (local == 1304)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "12104")
                    {
                        if (local == 1305)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "12201")
                    {
                        if (local == 1294 || local == 1295)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "12202")
                    {
                        if (local == 3859)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "12301")
                    {
                        if (local == 1306)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "12302")
                    {
                        if (local == 1307)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "12303")
                    {
                        if (local == 1308)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "12401")
                    {
                        if (local == 1309 || local == 1310)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "12402")
                    {
                        if (local == 1311 || local == 1312)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            adyacente = true;
                            metodoMensaje(dest);
                        }
                    }*/
                    break;
                #endregion
                case 13:
                    #region Región Metropolitana
                    if (dest.IdComuna == "13301")
                    {
                        if (local == 3980 || local == 1313 || local == 3982)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                metodoMensaje(dest); adyacente = true;
                            }
                        }
                    }
                    else if (dest.IdComuna == "13115")
                    {
                        if (local == 3943)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            adyacente = true;
                            metodoMensaje(dest);
                        }
                    }*/
                    break;
                #endregion
                case 14:
                    #region Decimo Cuarta Región
                    if (dest.IdComuna == "14101")
                    {
                        if (local == 1453 || local == 1454 || local == 1455 || local == 1456 || local == 1457 ||
                            local == 1458 || local == 1459 || local == 1460 || local == 1461 || local == 1462 ||
                            local == 1463 || local == 1464)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "14102")
                    {
                        if (local == 1383 || local == 1384 || local == 1385 || local == 1386)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "14103")
                    {
                        if (local == 1387 || local == 1388 || local == 1389 || local == 1390 || local == 1391 ||
                            local == 1392 || local == 1393 || local == 1394 || local == 1395 || local == 1396)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "14104")
                    {
                        if (local == 1397 || local == 1398 || local == 1399 || local == 1400 ||
                            local == 1401 || local == 1402 || local == 1403 || local == 1404 ||
                            local == 1405 || local == 1406 || local == 1407 || local == 1408)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "14105")
                    {
                        if (local == 1409 || local == 1410 || local == 1411 || local == 1412 || local == 1413)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "14106")
                    {
                        if (local == 1414 || local == 1415 || local == 1416 || local == 1417 || local == 1418)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "14107")
                    {
                        if (local == 1419 || local == 1420 || local == 1421 ||
                            local == 1422 || local == 1423 || local == 1424)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "14108")
                    {
                        if (local == 1425 || local == 1426 || local == 1427 || local == 1428 || local == 1429 ||
                            local == 1430 || local == 1431 || local == 1432 || local == 1433 || local == 1434 ||
                            local == 1435 || local == 1436 || local == 1437 || local == 1438 || local == 1439 ||
                            local == 1440 || local == 1441 || local == 1442 || local == 1443 || local == 1444 ||
                            local == 1445 || local == 1446 || local == 1447 || local == 1448 || local == 1449 ||
                            local == 1450 || local == 1451 || local == 1452)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "14201")
                    {
                        if (local == 1360 || local == 1361 || local == 1362 || local == 1363 ||
                            local == 1364 || local == 1365 || local == 1366)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "14202")
                    {
                        if (local == 1355 || local == 1356 || local == 1357 || local == 1358 || local == 1359)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "14203")
                    {
                        if (local == 1367 || local == 1368 || local == 1369 || local == 1370)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "14204")
                    {
                        if (local == 1371 || local == 1372 || local == 1373 || local == 1374 || local == 1375 ||
                            local == 1376 || local == 1377 || local == 1378 || local == 1379 || local == 1380 ||
                            local == 1381 || local == 1382)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            adyacente = true;
                            metodoMensaje(dest);
                        }
                    }*/
                    break;
                #endregion
                case 15:
                    #region Decimo Quinta Región
                    if (dest.IdComuna == "15101")
                    {
                        if (local == 1465 || local == 1466 || local == 1467 ||
                            local == 1468 || local == 3909 || local == 3910)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "15102")
                    {
                        if (local == 1469 || local == 1470 || local == 1471)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "15201")
                    {
                        if (local == 1473 || local == 1474 || local == 1475)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "15202")
                    {
                        if (local == 1472)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            adyacente = true;
                            metodoMensaje(dest);
                        }
                    }*/
                    break;
                #endregion
                case 16:
                    #region Decimo Sexta Región
                    if (dest.IdComuna == "16101")
                    {
                        if (local == 3911 || local == 3912 || local == 3913 || local == 3914)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16102")
                    {
                        if (local == 3915 || local == 3916)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16103")
                    {
                        if (local == 3917)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16104")
                    {
                        if (local == 3918)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16105")
                    {
                        if (local == 3919)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16106")
                    {
                        if (local == 3920 || local == 3921)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16107")
                    {
                        if (local == 3922)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16108")
                    {
                        if (local == 3923 || local == 3924 || local == 3925)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16109")
                    {
                        if (local == 3926 || local == 3927)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16201")
                    {
                        if (local == 3928)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16202")
                    {
                        if (local == 3929)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16203")
                    {
                        if (local == 3930 || local == 3931)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16204")
                    {
                        if (local == 3932)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16205")
                    {
                        if (local == 3933)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16206")
                    {
                        if (local == 3934)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16207")
                    {
                        if (local == 3935)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16301")
                    {
                        if (local == 3936 || local == 3937)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16302")
                    {
                        if (local == 3938)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16303")
                    {
                        if (local == 3939)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16304")
                    {
                        if (local == 3940)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    else if (dest.IdComuna == "16305")
                    {
                        if (local == 3941 || local == 3942)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente exceptuada por el Decreto 90.");
                        }
                        else
                        {
                            if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                            {
                                response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                                adyacente = true;
                                metodoMensaje(dest);
                            }
                        }
                    }
                    /*else
                    {
                        if (dest.Dias100 + dest.Dias60 + dest.Dias40 > 0)
                        {
                            response.Warnings.Add("El destino señalado es una local adyacente, por lo tanto no le corresponde viatico");
                            adyacente = true;
                            metodoMensaje(dest);
                        }
                    }*/
                    break;
                    #endregion

            }
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
                    var localidad = obj.LocalidadId.Value;
                    SwitchLocalidad(cometido.IdConglomerado.Value, localidad, response, obj, adyacente);
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

                var ListaCometidos = _repository.GetAll<Cometido>();

                /*Se valida la cantidad de dias al 100% dentro del mes, no puede superar los 10 dias. Y dentro del año no puede superar los 90 dias*/
                if (obj.Dias100 > 0)
                {
                    int Totaldias100Mes = 0;
                    int Totaldias100Ano = 0;
                    var mes = obj.FechaInicio.Month; //DateTime.Now.Month;
                    var year = obj.FechaInicio.Year; //DateTime.Now.Year;
                    var ListaDestino = _repository.Get<Destinos>(d => d.Cometido != null);
                    foreach (var destinos in ListaDestino)
                    {
                        var solicitanteDestino = ListaCometidos.FirstOrDefault(q => q.CometidoId == destinos.CometidoId).NombreId;
                        //_repository.Get<Cometido>(c => c.CometidoId == destinos.CometidoId).FirstOrDefault().NombreId;
                        var solicitante = ListaCometidos.FirstOrDefault(q => q.CometidoId == obj.CometidoId).NombreId;
                        //_repository.Get<Cometido>(c => c.CometidoId == obj.CometidoId).FirstOrDefault().NombreId;

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

                //var DestinosActivos = _repository.Get<Destinos>(d => d.CometidoId != null && d.DestinoActivo == true);

                /*se valida que los rangos de fecha no se topen con otros destinos*/
                //var ListaDestinos = _repository.Get<Destinos>(c => c.CometidoId == obj.CometidoId).ToList();
                var DestinosActivos = _repository.Get<Destinos>(d => d.CometidoId != null && d.DestinoActivo == true);

                foreach (var destinos in DestinosActivos)
                {
                    var solicitanteDestino = ListaCometidos.FirstOrDefault(q => q.CometidoId == destinos.CometidoId).NombreId;
                    //_repository.Get<Cometido>(c => c.CometidoId == destinos.CometidoId).FirstOrDefault().NombreId;
                    var solicitante = ListaCometidos.FirstOrDefault(q => q.CometidoId == obj.CometidoId).NombreId;
                    //_repository.Get<Cometido>(c => c.CometidoId == obj.CometidoId).FirstOrDefault().NombreId;

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

                if (obj.LocalidadId.ToString() != null)
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
                    var listaCometido = _repository.GetAll<Cometido>();

                    int Totaldias100Mes = 0;
                    int Totaldias100Ano = 0;
                    var mes = obj.FechaInicio.Month; //DateTime.Now.Month;
                    var year = obj.FechaInicio.Year; //DateTime.Now.Year;
                    foreach (var destinos in des)
                    {
                        var solicitanteDestino = listaCometido.FirstOrDefault(q => q.CometidoId == destinos.CometidoId).NombreId;

                        //_repository.Get<Cometido>(c => c.CometidoId == destinos.CometidoId).FirstOrDefault().NombreId;

                        var solicitante = listaCometido.FirstOrDefault(q => q.CometidoId == obj.CometidoId).NombreId;

                        //_repository.Get<Cometido>(c => c.CometidoId == objController.CometidoId).FirstOrDefault().NombreId;

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
                    var localidad = objController.LocalidadId.Value;
                    SwitchLocalidad(com.IdConglomerado.Value, localidad, response, objController, adyacente);

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
                var definicionWorkflowList = _repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == 13 /*workflowActual.Proceso.DefinicionProcesoId*/).OrderBy(q => q.Secuencia).ThenBy(q => q.DefinicionWorkflowId) ?? null;
                if (!definicionWorkflowList.Any())
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
                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionDocGP)
                    {
                        if (obj.TipoAprobacionId != (int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            var docs = workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == (int)Util.Enum.TipoDocumento.Resolucion).ToList();

                            if (docs.Count == 0)
                            {
                                throw new Exception("Se debe generar Documento de Acto Administrativo");
                            }

                            /*if(comet.Vehiculo && comet.PlacaVehiculo.Trim().IsNullOrWhiteSpace())
                            {
                                response.Errors.Add("Falta ingresar Patente del " + comet.TipoVehiculoDescripcion);
                            }*/

                        }
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.EncargadoPresupuesto)
                    {
                        if (obj.TipoAprobacionId != (int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            var docs = workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == (int)Util.Enum.TipoDocumento.RefrendacionPresupuesto).ToList();

                            if (docs.Count == 0)
                            {
                                throw new Exception("Se debe adjuntar Archivo de Refrendación");
                            }

                            var doc = _repository.GetById<Documento>(workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == (int)Util.Enum.TipoDocumento.RefrendacionPresupuesto).FirstOrDefault().DocumentoId).Signed;
                            foreach (var item in comet.GeneracionCDP)
                            {
                                if (item.VtcIdCompromiso.IsNullOrWhiteSpace())
                                {
                                    throw new Exception("Falta ingresar el ID del Compromiso.");
                                }
                            }
                            //TODO Validar documento de Firma de Encargado                            
                            if (!doc)
                            {
                                throw new Exception("Falta firmar Documento de Refrendación Presupuestaria.");
                            }
                        }
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaActoAdministrativo
                        || workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaMinistro
                        || workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaSubsecretario)
                    {
                        if (obj.TipoAprobacionId != (int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            var doc = _repository.GetById<Documento>(workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == (int)Util.Enum.TipoDocumento.Resolucion && c.Activo).FirstOrDefault().DocumentoId).Signed;
                            if (doc == false)
                                throw new Exception("El documento del acto administrativo debe estar firmado electronicamente");

                            /*se valida si existe una resolucion revocatoria, esta se debe firmar*/
                            if (comet.ResolucionRevocatoria == true)
                            {
                                var res = _repository.GetById<Documento>(workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == (int)Util.Enum.TipoDocumento.ResolucionRevocatoriaCometido && c.Activo).FirstOrDefault().DocumentoId).Signed;
                                if (res == false)
                                    throw new Exception("El documento resolucion revocatoria debe estar firmado electronicamente");
                            }
                        }
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)App.Util.Enum.CometidoSecuencia.AnalistaContabilidad)
                    {
                        if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any(c => c.TipoDocumentoId.Value == 4 && c.TipoDocumentoId != null))
                            throw new Exception("Debe adjuntar documentos en la tarea de analista de contabilidad.");
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)App.Util.Enum.CometidoSecuencia.EncargadoContabilidad)
                    {
                        if (obj.TipoAprobacionId != (int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            var doc = _repository.GetById<Documento>(workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == 4 && c.Activo).FirstOrDefault().DocumentoId).Signed;
                            if (doc == false)
                                throw new Exception("El documento cargado por el analista de contabilidad debe estar firmado electronicamente");
                        }
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)App.Util.Enum.CometidoSecuencia.AnalistaTesoreria)
                    {
                        if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any(c => c.TipoDocumentoId.Value == 5 && c.TipoDocumentoId != null))
                            throw new Exception("Debe adjuntar documentos en la tarea de analista tesoreria.");
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)App.Util.Enum.CometidoSecuencia.EncargadoTesoreria)
                    {
                        if (obj.TipoAprobacionId != (int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            var doc = _repository.GetById<Documento>(workflowActual.Proceso.Documentos.Where(c => c.TipoDocumentoId == 5 && c.Type == "application/pdf" && c.Activo).FirstOrDefault().DocumentoId).Signed;
                            if (doc == false)
                                throw new Exception("El documento cargado por el analista de tesorería debe estar firmado electronicamente");
                        }
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)App.Util.Enum.CometidoSecuencia.EncargadoFinanzas)
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
                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)App.Util.Enum.CometidoSecuencia.RevisaCotizacion)
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
                                    if (cotizacionDocumento.Selected && cotizacionDocumento.Type.Contains("pdf"))
                                    {
                                        cotiza = true;
                                    }
                                }

                            }

                            if (cotiza == false)
                                throw new Exception("Se debe seleccionar una cotizacion para los pasajes solicitados.");
                        }
                    }
                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.SolicitudCometido)
                    {
                        var com = _repository.GetFirst<Cometido>(q => q.WorkflowId == obj.WorkflowId);
                        var pasajes = _repository.Get<Pasaje>(q => q.ProcesoId == com.ProcesoId).ToList();
                        var pasaje = _repository.GetFirst<Pasaje>(q => q.ProcesoId == com.ProcesoId);


                        if (!com.Destinos.Any())
                            throw new Exception("Se deben ingresar destinos al cometido.");

                        if (com.Atrasado)
                        {
                            if (com.JustificacionAtraso.IsNullOrWhiteSpace())
                            {
                                throw new Exception("Falta ingresar Justificación de Atraso.");
                            }
                            if (com.JustificacionAtraso == null)
                            {
                                throw new Exception("Falta ingresar Justificación de Atraso.");
                            }
                        }

                        /*if(com.Vehiculo && com.PlacaVehiculo.IsNullOrWhiteSpace())
                        {
                            response.Errors.Add("Falta agregar Patente del Vehiculo.");
                        }*/

                        /*var lista = new List<Destinos>();
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
                        }*/

                        if (com.ReqPasajeAereo && pasajes.Count < 0)
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
                                                definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionJuridica);
                                            }
                                            else
                                                definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaActoAdministrativo);
                                        }
                                        else
                                            definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == 8); //10 /*workflowActual.DefinicionWorkflow.Secuencia*/);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaActoAdministrativo
                                            || workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaMinistro
                                            || workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaSubsecretario)
                                    {
                                        if (Cometido.SolicitaViatico != true || Cometido.TotalViatico == 0)
                                            definicionWorkflow = null;/*despues de la firma de resolucion, sino existe viatico el proceso finaliza*/
                                        else if (Cometido.ResolucionRevocatoria)
                                            definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.EncargadoFinanzas);/*si corresponde a una resoucion revocatoria se envia a finanzas*/
                                        else
                                            definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AnalistaContabilidad);
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
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.VisacionSubsecretaria)
                                    {
                                        if (Cometido.ReqPasajeAereo)
                                        {
                                            definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.IngresoCotizacion);
                                        }
                                        else
                                        {
                                            definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionDocGP);
                                        }
                                    }/*else if(workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.VisacionMinistro)
                                    {
                                        if(Cometido.ReqPasajeAereo)
                                        {
                                            definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.IngresoCotizacion);
                                        }
                                        else
                                        {
                                            definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionDocGP);
                                        }
                                    }*/

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
                                    /*else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.SolicitudCometido && Cometido.IdEscalafon == 1 && Cometido.GradoDescripcion == "B" && Cometido.ReqPasajeAereo == false)
                                    {
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionJefatura);
                                        //definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionDocGP);
                                    }*/
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionJefatura)
                                    {
                                        /*05-08-2022 - Se agrego modificacion de flujo para aprobacion de jefatura*/
                                        // agregar if de pasajes
                                        if (Cometido.Atrasado)
                                        {
                                            if (Cometido.GradoDescripcion == "C" || Cometido.GradoDescripcion == "B")
                                            {
                                                if (Cometido.ReqPasajeAereo)
                                                {
                                                    definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.IngresoCotizacion);
                                                }
                                                else
                                                {
                                                    definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionDocGP); /*cometido no posee pasaje por lo tanto sigue a las tarea de gestion personas*/
                                                }
                                            }
                                            else
                                            {
                                                definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.VisacionSubsecretaria);
                                            }
                                        }
                                        else
                                        {
                                            if (Cometido.ReqPasajeAereo)
                                            {
                                                definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.IngresoCotizacion);
                                            }
                                            else
                                            {
                                                definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionDocGP); /*cometido no posee pasaje por lo tanto sigue a las tarea de gestion personas*/
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
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaActoAdministrativo);
                                        /*if(comet.Nombre.Trim() == "PAULA NABILA CATTAN CASTILLO")
                                        {
                                            definicionWorkflow.Email = "gjorqueras@economia.cl";
                                        }*/
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionJuridica && Cometido.GradoDescripcion == "C")/*Verifica si coemtido es del subse se va a la aprobacion de ministro*/
                                    {
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionMinistro);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionJuridica && Cometido.GradoDescripcion == "B")/*Verifica si coemtido es del ministro se va a la aprobacion del subse*/
                                    {
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionSubse);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionMinistro && Cometido.GradoDescripcion == "C")/*Verifica si coemtido es del subse se va a la aprobacion de ministro*/
                                    {
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaMinistro);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionSubse && Cometido.GradoDescripcion == "B")/*Verifica si coemtido es del ministro se va a la aprobacion del subse*/
                                    {
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaSubsecretario);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.EncargadoFinanzas && Cometido.ResolucionRevocatoria == true) /*si cometido posee resolucion revocatoria no va a subir certificado de pago*/
                                    {
                                        definicionWorkflow = null;
                                    }
                                    else
                                    {
                                        if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.IngresoCotizacion && Cometido.IdEscalafon == 1 && Cometido.GradoDescripcion == "B" && Cometido.ReqPasajeAereo)
                                        {
                                            definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                            definicionWorkflow.Email = _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.JefeGabineteMinistro).Valor;
                                        }
                                        else
                                        {
                                            definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                        }
                                    }
                                }
                                else
                                {
                                    if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionJefatura && Cometido.SolicitaViatico != true)
                                    {
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionDocGP/*6*/); //8 /*workflowActual.DefinicionWorkflow.Secuencia*/);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaActoAdministrativo && Cometido.SolicitaViatico != true)
                                    {
                                        //definicionWorkflow = null;  /*workflow se deja null para terminar el proceso*/
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == (int)Util.Enum.CometidoSecuencia.AnalistaContabilidad/*16*/); //15 /*workflowActual.DefinicionWorkflow.Secuencia*/);
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.EncargadoFinanzas)
                                    {
                                        definicionWorkflow = null;  /*workflow se deja null para terminar el proceso*/
                                    }
                                    else if (workflowActual.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.SolicitudCometido && Cometido.ReqPasajeAereo)
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
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);

                                    }
                                    else
                                    {
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                    }
                                }
                            }
                            else
                            {
                                definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                            }
                        }
                        else if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                        {
                            if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                            {
                                if (workflowActual.DefinicionWorkflow.Secuencia == 19 || workflowActual.DefinicionWorkflow.Secuencia == 20)
                                {
                                    if (Cometido.ResolucionRevocatoria == true)/*si corresponde a una resolucion revocatoria se envia a firma de jefe depto adminstrativo*/
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == 6);
                                    else
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);
                                }
                                else
                                {
                                    if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje && workflowActual.DefinicionWorkflow.Secuencia == 3 && Cometido.ReqPasajeAereo)
                                    {
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == 1); /*Al ser rechazado va a la tarea de ingreso*/
                                    }
                                    else
                                    {
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);
                                    }
                                }
                            }
                            else
                            {
                                if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)Util.Enum.DefinicionProceso.SolicitudCometidoPasaje && workflowActual.DefinicionWorkflow.Secuencia == 3 && Cometido.ReqPasajeAereo)
                                    definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == 1); /*Al ser rechazado va a la tarea de ingreso*/
                                else
                                    definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);
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
                                definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == 1); /*Al ser rechazado va a la tarea de ingreso*/
                            else
                                definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);
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
                                    definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);// continua con el proceso de cometido con pasaje
                                }
                                else if (workflowActual.DefinicionWorkflow.Secuencia == 4 /*&& Pasaje.TipoDestino == true*/)
                                {
                                    definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == 9); // Pasaje Nacional                                   
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
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia == 8);
                                    }
                                    else
                                    {
                                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                    }
                                }
                                else
                                {
                                    definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);// Paseje Internacional
                                }
                            }
                            else
                            {
                                definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                            }
                        }
                        else
                            definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);
                }
                else
                {
                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                    {
                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                    }
                    else
                    {
                        definicionWorkflow = definicionWorkflowList.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);
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
                        // TODO Mejorar cambio de ejecutante si el jefe de servicio es el funcionario que viaja.

                        #region Funcion Original de ejecucion para acto administrativo
                        if (definicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaActoAdministrativo && comet.Nombre.Trim() == "PAULA NABILA CATTAN CASTILLO")
                        {
                            persona = _sigper.GetUserByEmail("gjorqueras@economia.cl");
                        }
                        else
                        {
                            persona = _sigper.GetUserByEmail(definicionWorkflow.Email);
                        }

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
                        #endregion
                    }

                    //guardar información
                    _repository.Create(workflow);
                    /*
                    _repository.Save();*/

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
                    if (response.IsValid)
                    {
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

                                    _repository.Save();

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

                                    _repository.Save();

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

                                    _repository.Save();

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
                                    _repository.Save();
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
                                    _repository.Save();
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
                                    _repository.Save();
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
                                        if (cometido.ResolucionRevocatoria)
                                        {
                                            doc = cometido.Proceso.Documentos.FirstOrDefault(d => d.ProcesoId == cometido.ProcesoId && d.TipoDocumentoId == 16);
                                        }

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
                                    _repository.Save();
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
                                    _repository.Save();
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
                                    _repository.Save();
                                    break;
                                case 10:
                                    emailMsg = new List<string>();
                                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                    {
                                        //Tipo C
                                        if (cometido.GradoDescripcion == "C")
                                        {
                                            emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionMinistro && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "irocha@economia.cl");
                                            _email.NotificacionesCometido(workflow,
                                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAprobacionJuridica),
                                                "Tiene el cometido N°:" + cometido.CometidoId + " " + "para aprobación.",
                                                emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                        }
                                        //Tipo B
                                        if (cometido.GradoDescripcion == "B")
                                        {
                                            emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionSubse && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "irocha@economia.cl");
                                            _email.NotificacionesCometido(workflow,
                                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAprobacionJuridica),
                                                "Tiene el cometido N°:" + cometido.CometidoId + " " + "para aprobación.",
                                                emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                                _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                        }
                                    }

                                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.EncargadoPresupuesto && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "irocha@economia.cl");

                                        _email.NotificacionesCometido(workflow,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaRechazoJuridica),
                                            "El Cometido N°: " + cometido.CometidoId + " fue rechazado por parte de Juridica, favor revisar las OBSERVACIONES",
                                            emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                    }

                                    _repository.Save();
                                    break;
                                case 11:
                                    emailMsg = new List<string>();
                                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                    {
                                        Documento doc = cometido.Proceso.Documentos.FirstOrDefault(d => d.ProcesoId == cometido.ProcesoId && d.TipoDocumentoId == 1);
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaSubsecretario && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "irocha@economia.cl");
                                        _email.NotificacionesCometido(workflow,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAprobacionGabineteSubsecretario),
                                            "El cometido N°: " + cometido.CometidoId + " fue aprobado por parte de el(la) Jefe(a) de Gabinete de el(la) Subsecretario(a).",
                                            emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, doc, "", "", "");
                                    }
                                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionJuridica && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "irocha@economia.cl");
                                        _email.NotificacionesCometido(workflow,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaRechazoGabineteSubsecretario),
                                            "El cometido N°: " + cometido.CometidoId + " fue rechazado por parte de el(la) Jefe(a) de Gabinete de el(la) Subsecretario(a), favor revisar las OBSERVACIONES.",
                                            emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                    }

                                    _repository.Save();
                                    break;
                                case 12:
                                    emailMsg = new List<string>();
                                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                    {
                                        Documento doc = cometido.Proceso.Documentos.FirstOrDefault(d => d.ProcesoId == cometido.ProcesoId && d.TipoDocumentoId == 1);
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.FirmaMinistro && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "irocha@economia.cl");
                                        _email.NotificacionesCometido(workflow,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAprobacionGabineteMinistro),
                                            "El cometido N°: " + cometido.CometidoId + " fue firmado por parte de el(la) Jefe(a) de Gabinete de el(la) Ministro(a).",
                                            emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, doc, "", "", "");
                                    }
                                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionJuridica && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "irocha@economia.cl");
                                        _email.NotificacionesCometido(workflow,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaRechazoGabineteMinistro),
                                            "El cometido N°: " + cometido.CometidoId + " fue rechazado por parte de el(la) Jefe(a) de Gabinete de el(la) Ministro(a), favor revisar las OBSERVACIONES.",
                                            emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                    }

                                    _repository.Save();
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
                                    _repository.Save();
                                    break;
                                case 14:
                                    emailMsg = new List<string>();
                                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AnalistaContabilidad && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "irocha@economia.cl");
                                        _email.NotificacionesCometido(workflow,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAprobacionFirmaMinistro),
                                            "El cometido N°: " + cometido.CometidoId + " fue firmado por parte de el(la) Ministro(a).",
                                            emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                    }
                                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionSubse && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "irocha@economia.cl");
                                        _email.NotificacionesCometido(workflow,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaRechazoFirmaMinistro),
                                            "El cometido N°: " + cometido.CometidoId + " fue rechazado por parte de el(la) Ministro(a), favor revisar las OBSERVACIONES.",
                                            emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                    }
                                    _repository.Save();
                                    break;
                                case 15:
                                    emailMsg = new List<string>();
                                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AnalistaContabilidad && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "irocha@economia.cl");
                                        _email.NotificacionesCometido(workflow,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaRechazoFirmaSubsecretario),
                                            "El cometido N°: " + cometido.CometidoId + " fue firmado por parte de el(la) Subsecretario(a).",
                                            emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), "",
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                    }
                                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionMinistro && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "irocha@economia.cl");
                                        _email.NotificacionesCometido(workflow,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaRechazoFirmaSubsecretario),
                                            "El cometido N°: " + cometido.CometidoId + " fue rechazado por parte de el(la) Subsecretario(a), favor reisar las OBSERVACIONES.",
                                            emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                    }
                                    _repository.Save();
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
                                    _repository.Save();
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

                                    _repository.Save();
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
                                    _repository.Save();
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
                                        !string.IsNullOrEmpty(cometido.ObservacionesPagoSigfeTesoreria) ? cometido.ObservacionesPagoSigfeTesoreria + "-" + workflowActual.Observacion : "",
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
                                    _repository.Save();
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
                                    _repository.Save();
                                    break;
                                case 21:
                                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                    {
                                        emailMsg = new List<string>();
                                        emailMsg.Add(QuienViaja);
                                        emailMsg.Add(solicitante.Trim());

                                        _email.NotificacionesCometido(workflow,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEnvíoSolicitudCometido),
                                            "Se adjunto Comprobante de pago al cometido N° " + cometido.CometidoId + " por parte de el/la Encargado/a de Tesorería.",
                                            emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                    }

                                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                    {
                                        emailMsg = new List<string>();
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == 20 && workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "irocha@economia.cl");

                                        _email.NotificacionesCometido(workflow,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaEnvíoSolicitudCometido),
                                            "Su cometido N° " + cometido.CometidoId + " tiene OBSERVACIONES para adjuntar el Comprobante de Pago",
                                            emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                            _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                    }
                                    break;
                                case 22:
                                    emailMsg = new List<string>();
                                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Aprobada)
                                    {
                                        emailMsg.Add(QuienViaja);//quien viaja
                                        emailMsg.Add(solicitante.Trim()); //solicitante

                                        _email.NotificacionesCometido(workflow,
                                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaAprobacionCometidoJustificacionAtraso),
                                        "Fue aprobada la justificación del cometido N° " + cometido.CometidoId + " " + " por la Jefa de Gabinete",
                                        emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                    }

                                    if (workflowActual.TipoAprobacionId == (int)Util.Enum.TipoAprobacion.Rechazada)
                                    {
                                        emailMsg.Add(workflow.DefinicionWorkflow.Secuencia == (int)Util.Enum.CometidoSecuencia.AprobacionJefatura &&
                                            workflow.DefinicionWorkflow.Email != null ? workflow.DefinicionWorkflow.Email : "mmontoya@economia.cl");

                                        _email.NotificacionesCometido(workflow,
                                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.PlantillaRechazoCometidoJustificacionAtraso),
                                        "Fue rechazada la justificación de cometido N° " + cometido.CometidoId + " por la Jefa de Gabinete",
                                        emailMsg, cometido.CometidoId, cometido.FechaSolicitud.ToString(), workflowActual.Observacion,
                                        _repository.GetById<Configuracion>((int)Util.Enum.Configuracion.UrlSistema).Valor, null, "", "", "");
                                    }
                                    _repository.Save();
                                    break;
                            }
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