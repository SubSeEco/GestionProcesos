using App.Core.Interfaces;
using App.Model.Core;
using App.Util;
using System;

namespace App.Core.UseCases
{
    public class UseCaseIntegridad
    {

        private readonly IGestionProcesos _repository;
        private readonly IHsm _hsm;
        private readonly ISigper _sigper;
        private readonly IEmail _email;
        private readonly IFolio _folio;
        private readonly IFile _file;
        //private IGestionProcesos repository;
        //private ISIGPER sigper;

        public UseCaseIntegridad(IGestionProcesos repository)
        {
            _repository = repository;
        }
        public UseCaseIntegridad(IGestionProcesos repository, ISigper sigper, IFile file, IFolio folio, IHsm hsm, IEmail email)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            _hsm = hsm;
            _email = email;
        }

        public UseCaseIntegridad(IGestionProcesos repository, ISigper sigper)
        {
            _repository = repository;
            _sigper = sigper;
        }

        public ResponseMessage DenunciaInsert(Denuncia obj)
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

        public ResponseMessage DenunciaUpdate(Denuncia obj)
        {
            var response = new ResponseMessage();

            if (!obj.AcosoLaboral && !obj.AcosoSexual &&
                !obj.MaltratoLaboral && !obj.DelitoFinanciero && !obj.FaltaProbidad)
            {
                response.Errors.Add("Debe seleccionar un Tipo de Atentado.");
            }

            if (obj.NivelJerarquico.IsNullOrWhiteSpace())
            {
                response.Errors.Add("Debe seleccionar Nivel Jerarquico.");
            }

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

        public ResponseMessage ConsultaInsert(Consulta obj)
        {
            var response = new ResponseMessage();
            try
            {
                if (obj.CorreoElectronico == null && obj.CorreoPostal == null ||
                    !obj.CorreoElectronico.Value && !obj.CorreoPostal.Value)
                {
                    response.Errors.Add("Falto seleccionar Tipo de Respuesta");
                }
                if (obj.ConsultaIntegridad.IsNullOrWhiteSpace())
                {
                    response.Errors.Add("Falto ingresar Consulta");
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

        public ResponseMessage ConsultaUpdate(Consulta obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (obj.CorreoElectronico == null && obj.CorreoPostal == null ||
                    !obj.CorreoElectronico.Value && !obj.CorreoPostal.Value)
                {
                    response.Errors.Add("Falto seleccionar Tipo de Respuesta");
                }
                if (obj.ConsultaIntegridad.IsNullOrWhiteSpace())
                {
                    response.Errors.Add("Falto ingresar Consulta");
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
    }
}
