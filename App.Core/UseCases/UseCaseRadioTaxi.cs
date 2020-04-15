using System;
using App.Model.Core;
using App.Model.RadioTaxi;
using App.Core.Interfaces;

namespace App.Core.UseCases
{
    public class UseCaseRadioTaxi
    {
        protected readonly IGestionProcesos _repository;
        protected readonly IEmail _email;
        protected readonly IHSM _hsm;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IFolio _folio;

        public UseCaseRadioTaxi(IGestionProcesos repositoryGestionProcesos, IHSM hsm)
        {
            _repository = repositoryGestionProcesos;
            _hsm = hsm;
        }

        public ResponseMessage RadioTaxiInsert(RadioTaxi obj)
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
        public ResponseMessage RadioTaxiUpdate(RadioTaxi obj)
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
        public ResponseMessage RadioTaxiDelete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<RadioTaxi>(id);
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
    }
}