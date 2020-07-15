﻿using System;
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
        protected readonly IHSM _hsm;
        protected readonly ISIGPER _sigper;
        protected readonly IEmail _email;
        protected readonly IFolio _folio;
        protected readonly IFile _file;

        public UseCaseMemorandum(IGestionProcesos repository)
        {
            _repository = repository;
        }
        public UseCaseMemorandum(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IHSM hsm, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
            _email = email;
        }

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

        //public ResponseMessage Sign(int id, string firmante)
        //{
        //    var response = new ResponseMessage();

        //    //validaciones...
        //    if (id == 0)
        //        response.Errors.Add("Documento a firmar no encontrado");
        //    if (string.IsNullOrWhiteSpace(firmante))
        //        response.Errors.Add("No se especificó el firmante");
        //    if (!string.IsNullOrWhiteSpace(firmante) && !_repository.GetExists<Rubrica>(q => q.Email == firmante && q.HabilitadoFirma))
        //        response.Errors.Add("No se encontró rúbrica habilitada para el firmante");

        //    var documento = _repository.GetById<Memorandum>(id);
        //    if (documento == null)
        //        response.Errors.Add("Documento a firmar no encontrado");

        //    if (response.IsValid)
        //    {
        //        //si el documento ya tiene folio no solicitarlo nuevamente
        //        if (string.IsNullOrWhiteSpace(documento.Folio))
        //        {
        //            try
        //            {
        //                var _folioResponse = _folio.GetFolio(firmante, documento.Folio);
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
        //            //var _hsmResponse = _hsm.Sign(documento.DocumentoSinFirma, rubrica.IdentificadorFirma, rubrica.UnidadOrganizacional, documento.Folio, null);

        //            //documento.DocumentoConFirma = _hsmResponse;
        //            //documento.DocumentoConFirmaFilename = "FIRMADO - " + documento.DocumentoSinFirmaFilename;
        //            documento.Firmante = firmante;
        //            //documento.Firmado = true;
        //            documento.FechaSolicitud = DateTime.Now;

        //            _repository.Update(documento);
        //            _repository.Create(new Documento()
        //            {
        //                //ProcesoId = documento.ProcesoId,
        //                //WorkflowId = documento.WorkflowId,
        //                Fecha = DateTime.Now,
        //                //Email = documento.Autor,
        //                //FileName = documento.DocumentoConFirmaFilename,
        //                //File = documento.DocumentoConFirma,
        //                Signed = true,
        //                Type = "application/pdf",
        //                TipoPrivacidadId = 1,
        //                TipoDocumentoId = 6
        //            });

        //            _repository.Save();
        //        }
        //        catch (Exception ex)
        //        {
        //            response.Errors.Add("Error al firmar el documento:" + ex.Message);
        //        }
        //    }

        //    return response;
        //}

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
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso && Memorandum.EmailVisa1 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailVisa1;
                                    var unidaddes = con.UnidadDescripcionVisa1;
                                    var unidadcod = con.IdUnidadVisa1;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 2)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa2 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                                }
                                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailVisa2;
                                    var unidaddes = con.UnidadDescripcionVisa2;
                                    var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 3)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa3 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                                }
                                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailVisa3;
                                    var unidaddes = con.UnidadDescripcionVisa3;
                                    var unidadcod = con.IdUnidadVisa3;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 4)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa4 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                                }
                                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailVisa4;
                                    var unidaddes = con.UnidadDescripcionVisa4;
                                    var unidadcod = con.IdUnidadVisa4;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 5)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa5 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                                }
                                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailVisa5;
                                    var unidaddes = con.UnidadDescripcionVisa5;
                                    var unidadcod = con.IdUnidadVisa5;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 6)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa6 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                                }
                                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailVisa6;
                                    var unidaddes = con.UnidadDescripcionVisa6;
                                    var unidadcod = con.IdUnidadVisa6;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 7)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa7 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                                }
                                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailVisa7;
                                    var unidaddes = con.UnidadDescripcionVisa7;
                                    var unidadcod = con.IdUnidadVisa7;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 8)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa8 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                                }
                                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailVisa8;
                                    var unidaddes = con.UnidadDescripcionVisa8;
                                    var unidadcod = con.IdUnidadVisa8;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 9)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa9 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                                }
                                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailVisa9;
                                    var unidaddes = con.UnidadDescripcionVisa9;
                                    var unidadcod = con.IdUnidadVisa9;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 10)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.EmailVisa10 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 12);
                                }
                                else if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailVisa10;
                                    var unidaddes = con.UnidadDescripcionVisa10;
                                    var unidadcod = con.IdUnidadVisa10;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 11)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 1);
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

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 12)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk1 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 14);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk1;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 13)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk2 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 15);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk2;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 14)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk3 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 16);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk3;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 15)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk4 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 17);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk4;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 16)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk5 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 18);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk5;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 17)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk6 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 19);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk6;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 18)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk7 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 20);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk7;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 19)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk8 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 21);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk8;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 20)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk9 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 22);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk9;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 21)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk10 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 23);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk10;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 22)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk11 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 24);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk11;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 23)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk12 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 25);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk12;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 24)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk13 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 26);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk13;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 25)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk14 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 27);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk14;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 26)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk15 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 28);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk15;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 27)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk16 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 29);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk16;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 28)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk17 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 30);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk17;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 29)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk18 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 31);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk18;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 30)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk19 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 32);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk19;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 31)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk20 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 33);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk20;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 32)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk21 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 34);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk21;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 33)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk22 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 35);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk22;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 34)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk23 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 36);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk23;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 35)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk24 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 37);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk24;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 36)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk25 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 38);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk25;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 37)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk26 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 39);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk26;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 38)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk27 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 40);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk27;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 39)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk28 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 41);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk28;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 40)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk29 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 42);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk29;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 41)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk30 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 43);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk30;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 42)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk31 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 44);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk31;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 43)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk32 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 45);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk32;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 44)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk33 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 46);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk33;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 45)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk34 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 47);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk34;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 46)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk35 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 48);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk35;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 47)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk36 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 49);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk36;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 48)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk37 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 50);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk37;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 49)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk38 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 51);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk38;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 50)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk39 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 52);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk39;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 51)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk40 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 53);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk40;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 52)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk41 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 54);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk41;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 53)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk42 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 55);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk42;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 54)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk43 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 56);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk43;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 55)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk44 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 57);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk44;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 56)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk45 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 58);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk45;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 57)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk46 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 59);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk46;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 58)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo && Memorandum.ListaChk47 == null)
                                {
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 60);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.ListaChk47;
                                    //var unidaddes = con.UnidadDescripcionVisa2;
                                    //var unidadcod = con.IdUnidadVisa2;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 59)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo)
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailSecre;
                                    var unidaddes = con.UnidadDescripcionSecre;
                                    var unidadcod = con.IdUnidadSecre;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailSecre;
                                    var unidaddes = con.UnidadDescripcionSecre;
                                    var unidadcod = con.IdUnidadSecre;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);

                                    ///*buscar el objeto de negocio condsulta integridad*/
                                    //var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    //var pro = con.ProcesoId;

                                    ////var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    //var correo = con.Vis1To;
                                    //var unidaddes = con.Vis1Pl_UndDes;
                                    //var unidadcod = con.Vis1Pl_UndCod;

                                    ////var correo = work.To;
                                    //workflowActual.Email = correo;
                                    //obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    ////definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 60)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo)
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailDest;
                                    var unidaddes = con.UnidadDescripcionDest;
                                    var unidadcod = con.IdUnidadDest;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 4);

                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);

                                }
                                else
                                {
                                    /*buscar el objeto de negocio condsulta integridad*/
                                    var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    var pro = con.ProcesoId;

                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    var correo = con.EmailDest;
                                    var unidaddes = con.UnidadDescripcionDest;
                                    var unidadcod = con.IdUnidadDest;

                                    //var correo = work.To;
                                    workflowActual.Email = correo;
                                    obj.To = correo;

                                    workflowActual.Pl_UndDes = unidaddes;
                                    obj.Pl_UndDes = unidaddes;

                                    workflowActual.Pl_UndCod = unidadcod;
                                    obj.Pl_UndCod = unidadcod;

                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);

                                    ///*buscar el objeto de negocio condsulta integridad*/
                                    //var con = _repository.Get<Memorandum>(c => c.WorkflowId == obj.WorkflowId).FirstOrDefault();
                                    //var pro = con.ProcesoId;

                                    ////var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId != 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    //var work = _repository.Get<Workflow>(c => c.ProcesoId == con.ProcesoId && c.TipoAprobacionId == 1).OrderByDescending(c => c.WorkflowId).FirstOrDefault();
                                    //var correo = con.Vis1To;
                                    //var unidaddes = con.Vis1Pl_UndDes;
                                    //var unidadcod = con.Vis1Pl_UndCod;

                                    ////var correo = work.To;
                                    //workflowActual.Email = correo;
                                    //obj.To = correo;

                                    //workflowActual.Pl_UndDes = unidaddes;
                                    //obj.Pl_UndDes = unidaddes;

                                    //workflowActual.Pl_UndCod = unidadcod;
                                    //obj.Pl_UndCod = unidadcod;

                                    ////definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 2);
                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
                            else if (workflowActual.DefinicionWorkflow.Secuencia == 6)
                            {
                                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                                {
                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 4);

                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);

                                }
                                else
                                {

                                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia == 8);
                                    //definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                                }
                            }
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
                        var jefatura = _sigper.GetUserByEmail(persona.Jefatura.Rh_Mail.Trim());
                        if (jefatura == null)
                            throw new Exception("No se encontró la jefatura en SIGPER.");
                        workflow.Email = jefatura.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = jefatura.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = jefatura.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = jefatura.Unidad.Pl_UndDes;

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
                        _email.NotificarNuevoWorkflow(workflowActual,
                        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaCorreoCambioEstado),
                        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion));

                    //notificar por email al ejecutor de proxima tarea
                    if (workflow.DefinicionWorkflow.NotificarAsignacion)
                        _email.NotificarNuevoWorkflow(workflow,
                        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaNuevaTarea),
                        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion));

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
                            _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion), emailMsg);
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
                                _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion), emailMsg);
                            }
                        }
                        else if (workflow.DefinicionWorkflow.Secuencia == 13 || workflow.DefinicionWorkflow.Secuencia == 14 || workflow.DefinicionWorkflow.Secuencia == 15)
                        {
                            List<string> emailMsg = new List<string>();
                            emailMsg.Add("mmontoya@economia.cl");
                            emailMsg.Add(persona.Funcionario.Rh_Mail.Trim()); // interesado quien viaja

                            _email.NotificarFirmaResolucionCometido(workflow,
                            _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaNotificacionPago),
                            _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion), emailMsg);
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