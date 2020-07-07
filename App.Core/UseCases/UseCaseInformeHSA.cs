using System;
using App.Model.Core;
using App.Core.Interfaces;
using App.Model.InformeHSA;
using App.Util;

namespace App.Core.UseCases
{
    public class UseCaseInformeHSA
    {
        protected readonly IGestionProcesos _repository;

        public UseCaseInformeHSA(IGestionProcesos repositoryGestionProcesos)
        {
            _repository = repositoryGestionProcesos;
        }

        public ResponseMessage Insert(InformeHSA obj)
        {
            var response = new ResponseMessage();

            if (!obj.FechaDesde.HasValue)
                response.Errors.Add("Debe especificar la fecha desde");
            if (!obj.FechaHasta.HasValue)
                response.Errors.Add("Debe especificar la fecha hasta");
            if (obj.Funciones.IsNullOrWhiteSpace())
                response.Errors.Add("Debe especificar las funciones");
            if (obj.Actividades.IsNullOrWhiteSpace())
                response.Errors.Add("Debe especificar las actividades");
            if (obj.NumeroBoleta.IsNullOrWhiteSpace())
                response.Errors.Add("Debe especificar las actividades");

            if (response.IsValid)
            {
                try
                {
                    _repository.Create(obj);
                    _repository.Save();
                }
                catch (Exception ex)
                {
                    response.Errors.Add(ex.Message);
                }
            }

            return response;
        }
        public ResponseMessage Update(InformeHSA obj)
        {
            var response = new ResponseMessage();

            if (!obj.FechaDesde.HasValue)
                response.Errors.Add("Debe especificar la fecha desde");
            if (!obj.FechaHasta.HasValue)
                response.Errors.Add("Debe especificar la fecha hasta");
            if (obj.Funciones.IsNullOrWhiteSpace())
                response.Errors.Add("Debe especificar las funciones");
            if (obj.Actividades.IsNullOrWhiteSpace())
                response.Errors.Add("Debe especificar las actividades");
            if (obj.NumeroBoleta.IsNullOrWhiteSpace())
                response.Errors.Add("Debe especificar las actividades");

            if (response.IsValid)
            {
                try
                {
                    _repository.Update(obj);
                    _repository.Save();
                }
                catch (Exception ex)
                {
                    response.Errors.Add(ex.Message);
                }
            }

            return response;
        }
    }
}