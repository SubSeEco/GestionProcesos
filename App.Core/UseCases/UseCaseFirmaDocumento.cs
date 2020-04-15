using System;
using App.Model.Core;
using App.Model.FirmaDocumento;
using App.Core.Interfaces;

namespace App.Core.UseCases
{
    public class UseCaseFirmaDocumento
    {
        protected readonly IGestionProcesos _repository;
        protected readonly IHSM _hsm;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IFolio _folio;

        public UseCaseFirmaDocumento(IGestionProcesos repository, IFile file)
        {
            _repository = repository;
            _file = file;
        }
        public UseCaseFirmaDocumento(IGestionProcesos repository, ISIGPER sigper, IFile file, IFolio folio, IHSM hsm)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
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
                    Signed = true,
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
                        Signed = true,
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
    }
}