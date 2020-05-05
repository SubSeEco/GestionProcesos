using System;
using System.Linq;
using App.Model.Core;
using App.Model.Memorandum;
using App.Model.Shared;
using App.Model.SIGPER;
using System.Collections.Generic;
using App.Core.Interfaces;

namespace App.Core.UseCases
{
    public class UseCaseMemorandum
    {
        protected readonly IGestionProcesos _repository;
        protected readonly IEmail _email;
        protected readonly IHSM _hsm;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IFolio _folio;

        public UseCaseMemorandum(IGestionProcesos repositoryGestionProcesos)
        {
            _repository = repositoryGestionProcesos;
        }
        public UseCaseMemorandum(IGestionProcesos repository, ISIGPER sigper)
        {
            _repository = repository;
            _sigper = sigper;
        }
        public UseCaseMemorandum(IGestionProcesos repositoryGestionProcesos, IHSM hsm)
        {
            _repository = repositoryGestionProcesos;
            _hsm = hsm;
        }
        public UseCaseMemorandum(IGestionProcesos repository, IEmail email, ISIGPER sigper)
        {
            _repository = repository;
            _email = email;
            _sigper = sigper;
        }

        //public ResponseMessage DocumentoSign(Documento obj, string email)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var documento = _repository.GetById<Documento>(obj.DocumentoId);
        //        if (documento == null)
        //            response.Errors.Add("Documento no encontrado");

        //        if (obj.Signed == true)
        //            response.Errors.Add("Documento ya se encuentra firmado");

        //        var rubrica = _repository.GetFirst<Rubrica>(q => q.Email == email && q.HabilitadoFirma);
        //        //var rubrica = _repository.Get<Rubrica>(q => q.Email == email && q.HabilitadoFirma == true);
        //        //string IdentificadorFirma = string.Empty;
        //        //bool habilitado = false;
        //        //foreach (var fir in rubrica)
        //        //{
        //        //if (fir == null)
        //        //    response.Errors.Add("Usuario sin información de firma electrónica");
        //        //if (fir != null && string.IsNullOrWhiteSpace(fir.IdentificadorFirma))
        //        //    response.Errors.Add("Usuario no tiene identificador de firma electrónica");

        //        //if (documento.Proceso.DefinicionProcesoId == int.Parse(fir.IdProceso))
        //        //{
        //        //habilitado = true;
        //        //IdentificadorFirma = fir.IdentificadorFirma;
        //        //}

        //        //if (fir.HabilitadoFirma != true)
        //        //    response.Errors.Add("Usuario no se encuentra habilitado para firmar");
        //        //}

        //        if (rubrica == null)
        //            response.Errors.Add("No se encontraron firmas habilitadas para el usuario");

        //        var HSMUser = _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.HSMUser);
        //        if (HSMUser == null)
        //            response.Errors.Add("No se encontró la configuración de usuario de HSM.");
        //        if (HSMUser != null && string.IsNullOrWhiteSpace(HSMUser.Valor))
        //            response.Errors.Add("La configuración de usuario de HSM es inválida.");

        //        var HSMPassword = _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.HSMPassword);
        //        if (HSMPassword == null)
        //            response.Errors.Add("No se encontró la configuración de usuario de HSM.");
        //        if (HSMPassword != null && string.IsNullOrWhiteSpace(HSMPassword.Valor))
        //            response.Errors.Add("La configuración de password de HSM es inválida.");

        //        if (response.IsValid)
        //        {
        //            //documento.File = _hsm.Sign(documento, rubrica, HSMUser, HSMPassword);
        //            //documento.File = _hsm.Sign(documento);
        //            //documento.Signed = true;

        //            //_repository.Update(documento);
        //            //_repository.Save();

        //            var doc = _hsm.Sign(documento.File, rubrica.IdentificadorFirma, rubrica.UnidadOrganizacional, null, null);
        //            documento.File = doc;
        //            documento.Signed = true;

        //            _repository.Update(documento);
        //            _repository.Save();

        //            /*se notifica por correo la firma de la resolucion*/
        //            //_email.
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(ex.Message);
        //    }

        //    return response;
        //}
     
        public ResponseMessage MemorandumInsert(Memorandum obj)
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
        public ResponseMessage MemorandumUpdate(Memorandum obj)
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
        public ResponseMessage MemorandumDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Memorandum>(id);
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

       
        //public ResponseMessage GeneracionCDPInsert(GeneracionCDP obj)
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
        //public ResponseMessage GeneracionCDPUpdate(GeneracionCDP obj)
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
        //public ResponseMessage GeneracionCDPDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<GeneracionCDP>(id);
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

        //public ResponseMessage GeneracionCDPAnular(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<GeneracionCDP>(id);
        //        if (obj == null)
        //            response.Errors.Add("Dato no encontrado");
        //        else
        //            obj.GeneracionCDPActivo = false;

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
        //public ResponseMessage ParrafosInsert(Parrafos obj)
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
        //public ResponseMessage ParrafosUpdate(Parrafos obj)
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
        //public ResponseMessage ParrafosDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<Parrafos>(id);
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

        //public ResponseMessage PasajeInsert(Pasaje obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (response.IsValid)
        //        {
        //            if (obj.NombreId.HasValue)
        //            {
        //                if (string.IsNullOrEmpty(obj.Nombre))
        //                {
        //                    var nombre = _sigper.GetUserByRut(obj.NombreId.Value).Funcionario.PeDatPerChq;
        //                    obj.Nombre = nombre.Trim();
        //                }
        //            }

        //            _repository.Create(obj);
        //            _repository.Save();
        //            response.EntityId = obj.PasajeId;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Errors.Add(ex.Message);
        //    }

        //    return response;
        //}
        //public ResponseMessage PasajeUpdate(Pasaje obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        /*se buscan los destinos asociados al pasaje*/
        //        var ListaDestinos = _repository.Get<DestinosPasajes>(c => c.PasajeId == obj.PasajeId).ToList();
        //        /*se valida que pasaje tenga un destino asociado*/
        //        if (ListaDestinos.Count == 0)
        //        {
        //            response.Errors.Add("Se debe agregar por lo menos un destino al pasaje");
        //        }
        //        //else
        //        //{
        //        //    ///*validacion de fechas */
        //        //    //if (ListaDestinos.LastOrDefault().FechaVuelta < ListaDestinos.FirstOrDefault().FechaIda)
        //        //    //{
        //        //    //    response.Errors.Add("La fecha de termino no pueder ser mayor a la fecha de inicio");
        //        //    //}
        //        //}

        //        //if (obj.TipoDestino == true)
        //        //   {
        //        //       if (obj.IdComunaOrigen != null)
        //        //       {
        //        //           if(string.IsNullOrEmpty(obj.OrigenComunaDescripcion))
        //        //           {
        //        //               var comuna = _sigper.GetDGCOMUNAs().FirstOrDefault(c => c.Pl_CodCom == obj.IdComunaOrigen.ToString()).Pl_DesCom.Trim();
        //        //               obj.OrigenComunaDescripcion = comuna;
        //        //           }                        
        //        //       }
        //        //       else
        //        //       {
        //        //           response.Errors.Add("Se debe señalar la comuna de origen");
        //        //       }

        //        //       if (obj.IdRegionOrigen != null)
        //        //       {
        //        //           if(string.IsNullOrEmpty(obj.OrigenRegionDescripcion))
        //        //           {
        //        //               var region = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == obj.IdRegionOrigen.ToString()).Pl_DesReg.Trim();
        //        //               obj.OrigenRegionDescripcion = region;
        //        //           }                        
        //        //       }
        //        //       else
        //        //       {
        //        //           response.Errors.Add("Se debe señalar la region de origen");
        //        //       }
        //        //   }
        //        //   else
        //        //   {
        //        //       if (obj.IdCiudadOrigen != null)
        //        //       {
        //        //           if (string.IsNullOrEmpty(obj.OrigenCiudadDescripcion))
        //        //           {
        //        //               var ciudad = _repository.Get<Ciudad>().FirstOrDefault(c => c.CiudadId == int.Parse(obj.IdCiudadOrigen)).CiudadNombre.Trim();
        //        //               obj.OrigenCiudadDescripcion = ciudad;
        //        //           }                        
        //        //       }
        //        //       else
        //        //       {
        //        //           response.Errors.Add("Se debe señalar la ciudad de origen");
        //        //       }

        //        //       if (obj.IdPaisOrigen != null)
        //        //       {
        //        //           if(string.IsNullOrEmpty(obj.OrigenPaisDescripcion))
        //        //           {
        //        //               var pais = _repository.Get<Pais>().FirstOrDefault(c => c.PaisId == int.Parse(obj.IdPaisOrigen)).PaisNombre.Trim();
        //        //               obj.OrigenPaisDescripcion = pais;
        //        //           }                        
        //        //       }
        //        //       else
        //        //       {
        //        //           response.Errors.Add("Se debe señalar el pais de origen");
        //        //       }
        //        //   }

        //        if (response.IsValid)
        //        {
        //            if (obj.NombreId.HasValue)
        //            {
        //                if (string.IsNullOrEmpty(obj.Nombre))
        //                {
        //                    //var nombre = _sigper.GetUserByRut(obj.NombreId.Value).Funcionario.PeDatPerChq;
        //                    //obj.Nombre = nombre.Trim();
        //                }
        //                obj.PasajeOk = true;
        //            }

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
        //public ResponseMessage PasajeDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<Pasaje>(id);
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

        //public ResponseMessage ComisionesInsert(Comisiones obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        /*validaciones*/
        //        if (obj.Vehiculo == true && obj.TipoVehiculoId == 0)
        //        {
        //            response.Errors.Add("Se ha selecionado la opcion de vehiculo, por lo tanto debe señalar el tipo de vehiculo");
        //        }

        //        if (_repository.Get<DestinosComision>().Count() > 0)
        //        {
        //            /*validacion de cantidad de dias al 100% dentro del mes*/
        //            if (_repository.Get<DestinosComision>(c => c.Comisiones.NombreId == obj.NombreId && c.Comisiones.FechaSolicitud.Month == DateTime.Now.Month).Sum(d => d.Dias100) > 10)
        //            {
        //                response.Errors.Add("La cantidad de dias al 100%, supera el maximo permitido para el mes señalado");
        //            }
        //            /*validacion de cantidad de dias al 100% dentro del año*/
        //            if (_repository.Get<DestinosComision>(c => c.Comisiones.NombreId == obj.NombreId && c.Comisiones.FechaSolicitud.Year == DateTime.Now.Year).Sum(d => d.Dias100) > 90)
        //            {
        //                response.Errors.Add("La cantidad de dias al 100%, supera el maximo permitido para el año señalado");
        //            }
        //        }

        //        if (string.IsNullOrEmpty(obj.ComisionDescripcion))
        //        {
        //            response.Errors.Add("Debe ingresar la descripción de la comision.");
        //        }

        //        if (obj.Vehiculo == true && obj.TipoVehiculoId == 0)
        //        {
        //            response.Errors.Add("Se ha selecionado la opcion de vehiculo, por lo tanto debe señalar el tipo de vehiculo");
        //        }

        //        if (string.IsNullOrEmpty(obj.ConglomeradoDescripcion))
        //            obj.IdConglomerado = Convert.ToInt32(obj.ConglomeradoDescripcion);

        //        if (!string.IsNullOrEmpty(obj.ProgramaDescripcion))
        //        {
        //            var pro = _sigper.GetREPYTs().Where(c => c.RePytDes.Trim() == obj.ProgramaDescripcion.Trim()).FirstOrDefault().RePytCod;
        //            obj.IdPrograma = int.Parse(pro.ToString());
        //        }

        //        if (obj.IdGrado == "0" && !string.IsNullOrEmpty(obj.GradoDescripcion))
        //        {
        //            obj.IdGrado = obj.GradoDescripcion;
        //        }


        //        if (obj.Vehiculo == true)
        //        {
        //            if (obj.TipoVehiculoId.HasValue)
        //            {
        //                var vehiculo = _repository.Get<SIGPERTipoVehiculo>().Where(q => q.SIGPERTipoVehiculoId == obj.TipoVehiculoId).FirstOrDefault().Vehiculo.Trim();
        //                if (!string.IsNullOrEmpty(vehiculo))
        //                    obj.TipoVehiculoDescripcion = vehiculo.Trim();
        //            }
        //            else
        //            {
        //                response.Errors.Add("Se debe señalar el tipo de vehiculo.");
        //            }

        //            if (string.IsNullOrEmpty(obj.PlacaVehiculo))
        //                response.Errors.Add("Se debe señalar la placa patente del vehiculo.");
        //        }


        //        if (response.IsValid)
        //        {
        //            if (obj.NombreId.HasValue)
        //            {
        //                var nombre = _sigper.GetUserByRut(obj.NombreId.Value).Funcionario.PeDatPerChq;
        //                obj.Nombre = nombre.Trim();
        //                obj.FechaSolicitud = DateTime.Now;
        //            }

        //            obj.ComisionesOk = false;

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
        //public ResponseMessage ComisionesUpdate(Comisiones obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (_repository.Get<DestinosComision>(c => c.Comisiones.NombreId == obj.NombreId && c.Comisiones.FechaSolicitud.Month == DateTime.Now.Month).Sum(d => d.Dias100) > 10)
        //        {
        //            response.Errors.Add("La cantidad de dias al 100%, supera el maximo permitido para el mes señalado");
        //        }

        //        if (_repository.Get<DestinosComision>(c => c.Comisiones.NombreId == obj.NombreId && c.Comisiones.FechaSolicitud.Year == DateTime.Now.Year).Sum(d => d.Dias100) > 90)
        //        {
        //            response.Errors.Add("La cantidad de dias al 100%, supera el maximo permitido para el año señalado");
        //        }

        //        /*se buscan los destinos asociados al cometido*/
        //        var destinosCometido = _repository.Get<DestinosComision>(c => c.ComisionesId == obj.ComisionesId).ToList();
        //        /*se valida que cometido tenga un destino asociado*/
        //        if (destinosCometido.Count == 0)
        //        {
        //            response.Errors.Add("Se debe agregar por lo menos un destino al cometido");
        //        }
        //        else
        //        {
        //            /*validacion de fechas */
        //            if (destinosCometido.LastOrDefault().FechaHasta < destinosCometido.FirstOrDefault().FechaInicio)
        //            {
        //                response.Errors.Add("La fecha de termino no pueder ser mayor a la fecha de inicio");
        //            }
        //            /*se valida que si existe mas de un destino solo se asigna un viatico*/
        //            if (destinosCometido.Count > 1)
        //            {
        //                //response.Errors.Add("Usted ha ingresado más de un destino para el cometido, pero solo se le asignara un viático");
        //                response.Warnings.Add("Usted ha ingresado más de un destino para la comision, pero solo se le asignara un viático");
        //            }
        //            /*valida ingreso de solicitud*/
        //            //if ((int)Util.Enum.Cometidos.DiasAnticipacionIngreso > (ListaDestinos.FirstOrDefault().FechaInicio.Date - DateTime.Now.Date).Days)
        //            //{
        //            //    response.Errors.Add("La solicitud de cometido se debe realizar con una anticipacion de:" + (int)Util.Enum.Cometidos.DiasAnticipacionIngreso + " " + "dias.");                        
        //            //}

        //            /*se valida que los rangos de fecha no se topen con otros destrinos*/
        //            //foreach (var destinos in _repository.Get<Destinos>(d => d.CometidoId == obj.CometidoId))
        //            foreach (var otrosDestinos in _repository.Get<DestinosComision>())
        //            {
        //                if (otrosDestinos.FechaInicio == destinosCometido.FirstOrDefault().FechaInicio && otrosDestinos.ComisionesId != destinosCometido.FirstOrDefault().ComisionesId)
        //                {
        //                    //response.Errors.Add("El rango de fechas señalados esta en conflicto con otros destinos");
        //                    response.Errors.Add(string.Format("El rango de fechas señalados esta en conflicto con los destinos de comisión {0}, inicio {1}, término {2}", otrosDestinos.ComisionesId, otrosDestinos.FechaInicio, otrosDestinos.FechaHasta));
        //                }
        //            }
        //        }

        //        if (string.IsNullOrEmpty(obj.ConglomeradoDescripcion))
        //            obj.IdConglomerado = Convert.ToInt32(obj.ConglomeradoDescripcion);

        //        if (!string.IsNullOrEmpty(obj.ProgramaDescripcion) && obj.IdPrograma == null)
        //        {
        //            var prog = _sigper.GetREPYTs().Where(c => c.RePytDes.Trim() == obj.ProgramaDescripcion.Trim()).FirstOrDefault();
        //            if (prog != null)
        //                obj.IdPrograma = int.Parse(prog.RePytCod.ToString());
        //        }

        //        if (obj.Vehiculo == true)
        //        {
        //            if (obj.TipoVehiculoId.HasValue)
        //            {
        //                var vehiculo = _repository.Get<SIGPERTipoVehiculo>().Where(q => q.SIGPERTipoVehiculoId == obj.TipoVehiculoId).FirstOrDefault().Vehiculo.Trim();
        //                if (string.IsNullOrEmpty(vehiculo))
        //                    obj.TipoVehiculoDescripcion = vehiculo.Trim();
        //            }
        //            else
        //            {
        //                response.Errors.Add("Se debe señalar el tipo de vehiculo.");
        //            }
        //        }

        //        if (obj.IdGrado == "0" && !string.IsNullOrEmpty(obj.GradoDescripcion))
        //        {
        //            obj.IdGrado = obj.GradoDescripcion;
        //        }


        //        if (response.IsValid)
        //        {
        //            obj.ComisionesOk = true;

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
        //public ResponseMessage ComisionesDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<Comisiones>(id);
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

        //public ResponseMessage DestinosComisionInsert(DestinosComision obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var comision = _repository.Get<Comisiones>(c => c.ComisionesId == obj.ComisionesId).FirstOrDefault();
        //        //reglas de negocio
        //        /*validacion de fechas*/
        //        if (obj.FechaInicio == null)
        //        {
        //            response.Errors.Add("La fecha de inicio no es válida.");
        //        }
        //        if (obj.FechaHasta == null)
        //        {
        //            response.Errors.Add("La fecha de término no es válida.");
        //        }

        //        /*se valida que la fecha de inicio no se superior que la de termino*/
        //        if (obj.FechaHasta < obj.FechaInicio)
        //        {
        //            response.Errors.Add("La fecha de inicio no puede ser superior o igual a la de término");
        //        }

        //        if (obj.IdPais != null)
        //            obj.PaisDescripcion = _repository.Get<Pais>().Where(c => c.PaisId.ToString() == obj.IdPais).FirstOrDefault().PaisNombre;

        //        if (obj.IdCiudad != null)
        //            obj.CiudadDescripcion = _repository.Get<Ciudad>().Where(c => c.CiudadId.ToString() == obj.IdCiudad).FirstOrDefault().CiudadNombre;

        //        /*Se valida la cantidad de dias al 100% dentro del mes, no puede superar los 10 dias. Y dentro del año no puede superar los 90 dias*/
        //        if (obj.Dias100 > 0)
        //        {
        //            int Totaldias100Mes = 0;
        //            int Totaldias100Ano = 0;
        //            var mes = DateTime.Now.Month;
        //            var year = DateTime.Now.Year;
        //            foreach (var destinos in _repository.Get<DestinosComision>(d => d.ComisionesId != null))
        //            {
        //                var solicitanteDestino = _repository.Get<Comisiones>(c => c.ComisionesId == destinos.ComisionesId).FirstOrDefault().NombreId;
        //                var solicitante = _repository.Get<Comisiones>(c => c.ComisionesId == obj.ComisionesId).FirstOrDefault().NombreId;

        //                if (solicitanteDestino == solicitante)
        //                {
        //                    if (destinos.FechaInicio.Month == mes && destinos.Dias100 != 0 && destinos.Dias100 != null)
        //                    {
        //                        Totaldias100Mes += destinos.Dias100.Value;
        //                    }
        //                    if (destinos.FechaInicio.Year == year && destinos.Dias100 != 0 && destinos.Dias100 != null)
        //                    {
        //                        Totaldias100Ano += destinos.Dias100.Value;
        //                    }
        //                }
        //            }
        //            if (Totaldias100Mes + obj.Dias100 > 10)
        //            {
        //                response.Errors.Add("Se ha excedido en: " + (Totaldias100Mes + obj.Dias100.Value - 10).ToString() + " la cantidad permitida de dias solicitados al 100%, dentro del Mes");
        //            }
        //            if (Totaldias100Ano + obj.Dias100 > 90)
        //            {
        //                response.Errors.Add("Se ha excedido en :" + (Totaldias100Ano + obj.Dias100 - 90).ToString() + " la cantidad permitida de dias solicitados al 100%, dentro de un año");
        //            }
        //        }

        //        /*se valida que los rangos de fecha no se topen con otros destinos*/
        //        //var ListaDestinos = _repository.Get<Destinos>(c => c.CometidoId == obj.CometidoId).ToList();
        //        foreach (var destinos in _repository.Get<DestinosComision>(d => d.ComisionesId != null))
        //        {
        //            var solicitanteDestino = _repository.Get<Comisiones>(c => c.ComisionesId == destinos.ComisionesId).FirstOrDefault().NombreId;
        //            var solicitante = _repository.Get<Comisiones>(c => c.ComisionesId == obj.ComisionesId).FirstOrDefault().NombreId;

        //            if (solicitanteDestino == solicitante)
        //            {
        //                if (destinos.FechaInicio.Date == obj.FechaInicio.Date)
        //                {
        //                    response.Errors.Add("El rango de fechas señalados esta en conflicto con otros destinos");
        //                }
        //            }
        //        }

        //        if (comision.SolicitaViatico == false)
        //        {
        //            response.Warnings.Add("No se ha solicitado viatico para la comision");
        //            obj.Dias100 = 0;
        //            obj.Dias60 = 0;
        //            obj.Dias40 = 0;
        //            obj.Dias100Aprobados = 0;
        //            obj.Dias60Aprobados = 0;
        //            obj.Dias40Aprobados = 0;
        //            obj.Dias100Monto = 0;
        //            obj.Dias60Monto = 0;
        //            obj.Dias40Monto = 0;
        //            obj.TotalViatico = 0;
        //            obj.Total = 0;
        //        }
        //        if (obj.IdCiudad != null)
        //        {
        //            var ciudad = _repository.Get<Ciudad>().FirstOrDefault(c => c.CiudadId == int.Parse(obj.IdCiudad)).CiudadNombre.Trim();
        //            obj.CiudadDescripcion = ciudad;
        //        }
        //        else
        //        {
        //            response.Errors.Add("Se debe señalar la ciudad de destino");
        //        }

        //        if (obj.IdPais != null)
        //        {
        //            var pais = _repository.Get<Pais>().FirstOrDefault(c => c.PaisId == int.Parse(obj.IdPais)).PaisNombre.Trim();
        //            obj.PaisDescripcion = pais;
        //        }
        //        else
        //        {
        //            response.Errors.Add("Se debe señalar el pais de destino");
        //        }
        //        if (obj.Dias00 == null)
        //            obj.Dias00 = 0;
        //        if (obj.Dias50 == null)
        //            obj.Dias50 = 0;

        //        /*Se dejan los mismo valores de los solicitados como aprobados en la creacion del destino*/
        //        obj.Dias100Aprobados = obj.Dias100;
        //        obj.Dias60Aprobados = obj.Dias60;
        //        obj.Dias40Aprobados = obj.Dias40;
        //        obj.Dias50Aprobados = obj.Dias50;
        //        obj.Dias00Aprobados = obj.Dias00;


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
        //public ResponseMessage DestinosComisionUpdate(DestinosComision obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        //reglas de negocio
        //        /*validacion de fechas*/
        //        if (obj.FechaInicio == null)
        //        {
        //            response.Errors.Add("La fecha de inicio no es válida.");
        //        }
        //        if (obj.FechaHasta == null)
        //        {
        //            response.Errors.Add("La fecha de término no es válida.");
        //        }

        //        //if (obj.IdPais != null)
        //        //    obj.PaisDescripcion = _repository.Get<Pais>().Where(c => c.PaisId.ToString() == obj.IdPais).FirstOrDefault().PaisNombre;

        //        //if (obj.IdCiudad != null)
        //        //    obj.CiudadDescripcion = _repository.Get<Ciudad>().Where(c => c.CiudadId.ToString() == obj.IdCiudad).FirstOrDefault().CiudadNombre;

        //        /*se valida que la cantidad de dias sea igual que lo solicitado por cada destino ingresado*/
        //        var dias = (obj.FechaHasta - obj.FechaInicio).Days + 1;
        //        var cant = obj.Dias100Aprobados + obj.Dias60Aprobados + obj.Dias40Aprobados + obj.Dias00Aprobados + obj.Dias50Aprobados;
        //        if (dias != cant)
        //        {
        //            response.Errors.Add("la cantidad de dias no coincide con los viaticos solicitados");
        //        }
        //        /*se valida que la fecha de inicio no se superior que la de termino*/
        //        if (obj.FechaHasta < obj.FechaInicio)
        //        {
        //            response.Errors.Add("La fecha de inicio no puede ser superior o igual a la de término");
        //        }

        //        var comision = _repository.Get<Comisiones>(c => c.ComisionesId == obj.ComisionesId).FirstOrDefault();
        //        if (comision.SolicitaViatico == false)
        //        {
        //            response.Warnings.Add("Este cometido tendrá un viático de $0");
        //            obj.Dias100 = 0;
        //            obj.Dias60 = 0;
        //            obj.Dias40 = 0;
        //            obj.Dias100Aprobados = 0;
        //            obj.Dias60Aprobados = 0;
        //            obj.Dias40Aprobados = 0;
        //            obj.Dias100Monto = 0;
        //            obj.Dias60Monto = 0;
        //            obj.Dias40Monto = 0;
        //            obj.TotalViatico = 0;
        //            obj.Total = 0;
        //        }

        //        if (obj.IdCiudad != null)
        //        {
        //            var ciudad = _repository.Get<Ciudad>().FirstOrDefault(c => c.CiudadId == int.Parse(obj.IdCiudad)).CiudadNombre.Trim();
        //            obj.CiudadDescripcion = ciudad;
        //        }
        //        else
        //        {
        //            response.Errors.Add("Se debe señalar la ciudad de destino");
        //        }

        //        if (obj.IdPais != null)
        //        {
        //            var pais = _repository.Get<Pais>().FirstOrDefault(c => c.PaisId == int.Parse(obj.IdPais)).PaisNombre.Trim();
        //            obj.PaisDescripcion = pais;
        //        }
        //        else
        //        {
        //            response.Errors.Add("Se debe señalar el pais de destino");
        //        }
        //        if (obj.Dias00 == null)
        //            obj.Dias00 = 0;
        //        if (obj.Dias50 == null)
        //            obj.Dias50 = 0;



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
        //public ResponseMessage DestinosComisionDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<DestinosComision>(id);
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

        //public ResponseMessage DestinosPasajesInsert(DestinosPasajes obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        /*se valida que la fecha de inicio no se superior que la de termino*/
        //        //if (obj.FechaVuelta < obj.FechaIda)
        //        //{
        //        //    response.Errors.Add("La fecha de ida no puede ser superior a la fecha de salida");
        //        //}

        //        var pasaje = _repository.GetById<Pasaje>(obj.PasajeId);
        //        if (pasaje.TipoDestino == true)
        //        {
        //            //if (obj.IdComuna != null)
        //            //{
        //            //    var comuna = _sigper.GetDGCOMUNAs().FirstOrDefault(c => c.Pl_CodCom == obj.IdComuna.ToString()).Pl_DesCom.Trim();
        //            //    obj.ComunaDescripcion = comuna;
        //            //}
        //            //else
        //            //{
        //            //    response.Errors.Add("Se debe señalar la comuna de destino");
        //            //}

        //            if (obj.IdRegionOrigen != null)
        //            {
        //                if (string.IsNullOrEmpty(obj.OrigenRegionDescripcion))
        //                {
        //                    var region = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == obj.IdRegionOrigen.ToString()).Pl_DesReg.Trim();
        //                    obj.OrigenRegionDescripcion = region;
        //                }
        //            }
        //            else
        //            {
        //                response.Errors.Add("Se debe señalar la region de destino");
        //            }

        //            if (obj.IdRegion != null)
        //            {
        //                if (string.IsNullOrEmpty(obj.RegionDescripcion))
        //                {
        //                    var region = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == obj.IdRegion.ToString()).Pl_DesReg.Trim();
        //                    obj.RegionDescripcion = region;
        //                }
        //            }
        //            else
        //            {
        //                response.Errors.Add("Se debe señalar la region de destino");
        //            }
        //        }
        //        else
        //        {
        //            //if (obj.IdCiudad != null)
        //            //{
        //            //    var ciudad = _repository.Get<Ciudad>().FirstOrDefault(c => c.CiudadId == int.Parse(obj.IdCiudad)).CiudadNombre.Trim();
        //            //    obj.CiudadDescripcion = ciudad;
        //            //}
        //            //else
        //            //{
        //            //    response.Errors.Add("Se debe señalar la ciudad de destino");
        //            //}

        //            if (obj.IdPaisOrigen != null)
        //            {
        //                if (string.IsNullOrEmpty(obj.OrigenPaisDescripcion))
        //                {
        //                    var pais = _repository.Get<Pais>().FirstOrDefault(c => c.PaisId == int.Parse(obj.IdPaisOrigen)).PaisNombre.Trim();
        //                    obj.OrigenPaisDescripcion = pais;
        //                }
        //            }
        //            else
        //            {
        //                response.Errors.Add("Se debe señalar el pais de destino");
        //            }


        //            if (obj.IdPais != null)
        //            {
        //                if (string.IsNullOrEmpty(obj.PaisDescripcion))
        //                {
        //                    var pais = _repository.Get<Pais>().FirstOrDefault(c => c.PaisId == int.Parse(obj.IdPais)).PaisNombre.Trim();
        //                    obj.PaisDescripcion = pais;
        //                }
        //            }
        //            else
        //            {
        //                response.Errors.Add("Se debe señalar el pais de destino");
        //            }
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
        //public ResponseMessage DestinosPasajesUpdate(DestinosPasajes obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var pasaje = _repository.GetById<Pasaje>(obj.PasajeId);
        //        if (pasaje.TipoDestino == true)
        //        {
        //            if (obj.IdRegionOrigen != null)
        //            {
        //                var region = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == obj.IdRegionOrigen.ToString()).Pl_DesReg.Trim();
        //                obj.OrigenRegionDescripcion = region;
        //            }
        //            else
        //            {
        //                response.Errors.Add("Se debe señalar la region de destino");
        //            }

        //            if (obj.IdRegion != null)
        //            {
        //                var region = _sigper.GetRegion().FirstOrDefault(c => c.Pl_CodReg == obj.IdRegion.ToString()).Pl_DesReg.Trim();
        //                obj.RegionDescripcion = region;
        //            }
        //            else
        //            {
        //                response.Errors.Add("Se debe señalar la region de destino");
        //            }
        //        }
        //        else
        //        {
        //            if (obj.IdPaisOrigen != null)
        //            {
        //                var pais = _repository.Get<Pais>().FirstOrDefault(c => c.PaisId == int.Parse(obj.IdPaisOrigen)).PaisNombre.Trim();
        //                obj.OrigenPaisDescripcion = pais;
        //            }
        //            else
        //            {
        //                response.Errors.Add("Se debe señalar el pais de destino");
        //            }


        //            if (obj.IdPais != null)
        //            {
        //                var pais = _repository.Get<Pais>().FirstOrDefault(c => c.PaisId == int.Parse(obj.IdPais)).PaisNombre.Trim();
        //                obj.PaisDescripcion = pais;
        //            }
        //            else
        //            {
        //                response.Errors.Add("Se debe señalar el pais de destino");
        //            }
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
        //public ResponseMessage DestinosPasajesDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<DestinosPasajes>(id);
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

        //public ResponseMessage GeneracionCDPComisionInsert(GeneracionCDPComision obj)
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
        //public ResponseMessage GeneracionCDPComisionUpdate(GeneracionCDPComision obj)
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
        //public ResponseMessage GeneracionCDPComisionDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<GeneracionCDPComision>(id);
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

        //public ResponseMessage ParametrosComisionesInsert(ParametrosComisiones obj)
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
        //public ResponseMessage ParametrosComisionesUpdate(ParametrosComisiones obj)
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
        //public ResponseMessage ParametrosComisionesDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<ParametrosComisiones>(id);
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

        //public ResponseMessage ParrafoComisionesInsert(ParrafoComisiones obj)
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
        //public ResponseMessage ParrafoComisionesUpdate(ParrafoComisiones obj)
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
        //public ResponseMessage ParrafoComisionesDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<ParrafoComisiones>(id);
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

        //public ResponseMessage ViaticoInternacionalInsert(ViaticoInternacional obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (obj.PaisId != null)
        //            obj.PaisNombre = _repository.Get<Pais>().Where(c => c.PaisId == obj.PaisId.Value).FirstOrDefault().PaisNombre;

        //        if (obj.CiudadId != null)
        //            obj.CiudadNombre = _repository.Get<Ciudad>().Where(c => c.CiudadId == obj.CiudadId.Value).FirstOrDefault().CiudadNombre;

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
        //public ResponseMessage ViaticoInternacionalUpdate(ViaticoInternacional obj)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        if (obj.PaisId != null)
        //            obj.PaisNombre = _repository.Get<Pais>().Where(c => c.PaisId == obj.PaisId.Value).FirstOrDefault().PaisNombre;

        //        if (obj.CiudadId != null)
        //            obj.CiudadNombre = _repository.Get<Ciudad>().Where(c => c.CiudadId == obj.CiudadId.Value).FirstOrDefault().CiudadNombre;

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
        //public ResponseMessage ViaticoInternacionalDelete(int id)
        //{
        //    var response = new ResponseMessage();

        //    try
        //    {
        //        var obj = _repository.GetById<ViaticoInternacional>(id);
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
                /*Si la tarea es rechazada se valida que se ingrese una observacion - en el proceso cometido*/
                //if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                //{
                //    throw new Exception("Se debe señalra el motivo del rechazo para la tarea.");
                //}
                /*Valida la carga de adjuntos segun el tipo de proceso*/
                if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudPasaje)
                {
                    //if (workflowActual.DefinicionWorkflow.Secuencia == 4)
                    //{
                    //    //Se toma valor del pasaje para validar cuantos adjuntos se solicitan
                    //    var Pasaje = new Pasaje();
                    //    Pasaje = _repository.Get<Pasaje>(q => q.WorkflowId == obj.WorkflowId).FirstOrDefault();
                    //    if (Pasaje.TipoDestino == true)
                    //    {
                    //        if (workflowActual.Proceso.Documentos.Count < 1)
                    //        {
                    //            throw new Exception("Debe adjuntar a los menos una cotizaciones.");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (workflowActual.Proceso.Documentos.Count < 3)
                    //        {
                    //            throw new Exception("Debe adjuntar a los menos tres cotizaciones.");
                    //        }
                    //    }
                    //}
                    //else if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any())
                    //{
                    //    throw new Exception("Debe adjuntar a los menos tres cotizaciones.");
                    //}                                         
                }
                //else if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                //else if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.Memorandum)
                //{
                //    if (workflowActual.DefinicionWorkflow.Secuencia == 3)
                //    {
                //        if (obj.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                //        {
                //            //Se toma valor del pasaje para validar cuantos adjuntos se solicitan
                //            var Pasaje = new Pasaje();
                //            Pasaje = _repository.Get<Pasaje>(q => q.WorkflowId == obj.WorkflowId).FirstOrDefault();
                //            if (Pasaje != null)
                //            {
                //                var cotizacion = _repository.Get<Cotizacion>(c => c.PasajeId == Pasaje.PasajeId).LastOrDefault();
                //                if (cotizacion != null)
                //                {
                //                    if (Pasaje.TipoDestino == true)
                //                    {
                //                        if (cotizacion.CotizacionDocumento.Count < 1)
                //                        {
                //                            throw new Exception("Debe adjuntar a los menos una cotizaciones.");
                //                        }
                //                    }
                //                    else
                //                    {
                //                        if (cotizacion.CotizacionDocumento.Count < 3)
                //                        {
                //                            throw new Exception("Debe adjuntar a los menos tres cotizaciones.");
                //                        }
                //                    }
                //                }
                //                else
                //                    throw new Exception("No se ha ingresado cotización.");
                //            }
                //        }
                //    }
                //    else if (workflowActual.DefinicionWorkflow.Secuencia == 16)
                //    {
                //        if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any(c => c.TipoDocumentoId.Value == 4 && c.TipoDocumentoId != null))
                //            throw new Exception("Debe adjuntar documentos en la tarea de analista de contabilidad.");
                //    }
                //    else if (workflowActual.DefinicionWorkflow.Secuencia == 18)
                //    {
                //        if (workflowActual != null && workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequireDocumentacion && workflowActual.Proceso != null && !workflowActual.Proceso.Documentos.Any(c => c.TipoDocumentoId.Value == 5 && c.TipoDocumentoId != null))
                //            throw new Exception("Debe adjuntar documentos en la tarea de analista tesoreria.");
                //    }
                //}
                else if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
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


                if (workflowActual.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.Memorandum.ToString())
                {
                    //Se toma valor de cometidos para definir curso de accion del flujo
                    var Memorandum = new Memorandum();
                    Memorandum = _repository.Get<Memorandum>(q => q.WorkflowId == obj.WorkflowId).FirstOrDefault();
                    if (Memorandum != null)
                    {
                        //determinar siguiente tarea desde el diseño de proceso
                        if (!workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                            if (workflowActual.DefinicionWorkflow.Secuencia == 1)
                            {
                                //if (workflowActual.TipoAprobacionId == (int)Enum.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso && Memorandum.To == null)
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso && Memorandum.EmailRem == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 4);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailRem;
                                    var unidaddes = con.UnidadDescripcion;
                                    var unidadcod = con.IdUnidad;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            //else if (workflowActual.DefinicionWorkflow.Secuencia == 2)
                            //{
                            //    if (workflowActual.TipoAprobacionId == (int)Enum.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.Vis1To == null)
                            //    {
                            //        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 4);
                            //    }
                            //    else
                            //    {
                            //        /*buscar el objeto de negocio condsulta integridad*/
                            //        var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                            //        var pro = con.ProcesoId;

                            //        //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                            //        var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                            //        var correo = con.Vis1To;
                            //        var unidaddes = con.Vis1Pl_UndDes;
                            //        var unidadcod = con.Vis1Pl_UndCod;

                            //        //var correo = work.To;
                            //        workflowActual.Email = correo;
                            //        obj.To = correo;

                            //        workflowActual.Pl_UndDes = unidaddes;
                            //        obj.Pl_UndDes = unidaddes;

                            //        workflowActual.Pl_UndCod = unidadcod;
                            //        obj.Pl_UndCod = unidadcod;

                            //        //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                            //        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                            //    }
                            //}
                            //else if (workflowActual.DefinicionWorkflow.Secuencia == 3)
                            //{
                            //    if (workflowActual.TipoAprobacionId == (int)Enum.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.To == null)
                            //    {
                            //        /*buscar el objeto de negocio condsulta integridad*/
                            //        var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                            //        var pro = con.ProcesoId;

                            //        //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                            //        var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                            //        var correo = con.To;
                            //        var unidaddes = con.Pl_UndDes;
                            //        var unidadcod = con.Pl_UndCod;

                            //        //var correo = work.To;
                            //        workflowActual.Email = correo;
                            //        obj.To = correo;

                            //        workflowActual.Pl_UndDes = unidaddes;
                            //        obj.Pl_UndDes = unidaddes;

                            //        workflowActual.Pl_UndCod = unidadcod;
                            //        obj.Pl_UndCod = unidadcod;

                            //        //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 4);

                            //        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);

                            //    }
                            //    else
                            //    {
                            //        /*buscar el objeto de negocio condsulta integridad*/
                            //        var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                            //        var pro = con.ProcesoId;

                            //        //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                            //        var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                            //        var correo = con.To;
                            //        var unidaddes = con.Pl_UndDes;
                            //        var unidadcod = con.Pl_UndCod;

                            //        //var correo = work.To;
                            //        workflowActual.Email = correo;
                            //        obj.To = correo;

                            //        workflowActual.Pl_UndDes = unidaddes;
                            //        obj.Pl_UndDes = unidaddes;

                            //        workflowActual.Pl_UndCod = unidadcod;
                            //        obj.Pl_UndCod = unidadcod;

                            //        //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                            //        definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);

                            //        ///*buscar el objeto de negocio condsulta integridad*/
                            //        //var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                            //        //var pro = con.ProcesoId;

                            //        ////var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                            //        //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                            //        //var correo = con.Vis1To;
                            //        //var unidaddes = con.Vis1Pl_UndDes;
                            //        //var unidadcod = con.Vis1Pl_UndCod;

                            //        ////var correo = work.To;
                            //        //workflowActual.Email = correo;
                            //        //obj.To = correo;

                            //        //workflowActual.Pl_UndDes = unidaddes;
                            //        //obj.Pl_UndDes = unidaddes;

                            //        //workflowActual.Pl_UndCod = unidadcod;
                            //        //obj.Pl_UndCod = unidadcod;

                            //        ////definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                            //        //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                            //    }
                            //}
                            else
                            {
                                definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                            }
                    }
                }
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

                    /*Si el proceso corresponde a Cometidos y esta en la tarea de firma electronica se notifica con correo*/
                    if (workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.Cometido.ToString())
                    {
                        if (workflow.DefinicionWorkflow.Secuencia == 13 || workflow.DefinicionWorkflow.Secuencia == 14 || workflow.DefinicionWorkflow.Secuencia == 15)
                        {
                            List<string> emailMsg = new List<string>();
                            emailMsg.Add("mmontoya@economia.cl");
                            emailMsg.Add("acifuentes@economia.cl"); //oficia de partes
                            emailMsg.Add("scid@economia.cl"); //oficia de partes
                            emailMsg.Add(persona.Funcionario.Rh_Mail.Trim()); // interesado

                            _email.NotificarFirmaResolucionCometido(workflow,
                            _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaFirmaResolucion),
                            _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacionTarea), emailMsg);
                        }
                    }

                    /*Si el proceso corresponde a Cometidos y esta en la tarea de pago tesoreria se notifica con correo a quien viaja*/
                    if (workflow.DefinicionWorkflow.Entidad.Codigo == App.Util.Enum.Entidad.Cometido.ToString())
                    {
                        if (workflowActual.DefinicionWorkflow.DefinicionProcesoId == (int)App.Util.Enum.DefinicionProceso.SolicitudCometidoPasaje)
                        {
                            if (workflow.DefinicionWorkflow.Secuencia == 20)
                            {
                                List<string> emailMsg = new List<string>();
                                emailMsg.Add("mmontoya@economia.cl");
                                emailMsg.Add(persona.Funcionario.Rh_Mail.Trim()); // interesado quien viaja

                                _email.NotificarFirmaResolucionCometido(workflow,
                                _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaNotificacionPago),
                                _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacionTarea), emailMsg);
                            }
                        }
                        else if (workflow.DefinicionWorkflow.Secuencia == 13 || workflow.DefinicionWorkflow.Secuencia == 14 || workflow.DefinicionWorkflow.Secuencia == 15)
                        {
                            List<string> emailMsg = new List<string>();
                            emailMsg.Add("mmontoya@economia.cl");
                            emailMsg.Add(persona.Funcionario.Rh_Mail.Trim()); // interesado quien viaja

                            _email.NotificarFirmaResolucionCometido(workflow,
                            _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaNotificacionPago),
                            _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacionTarea), emailMsg);
                        }
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