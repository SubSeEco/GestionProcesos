using System;
using App.Model.Core;
using App.Model.InformeHSA;
using App.Core.Interfaces;

namespace App.Core.UseCases
{
    public class UseCaseContrato
    {
        protected readonly IGestionProcesos _repository;
        protected readonly IEmail _email;
        protected readonly IHSM _hsm;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IFolio _folio;

        public UseCaseContrato(IGestionProcesos repositoryGestionProcesos, IHSM hsm)
        {
            _repository = repositoryGestionProcesos;
            _hsm = hsm;
        }

        public ResponseMessage ContratoInsert(Contrato obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (response.IsValid)
                {
                    if (obj.Pl_CodCar.HasValue)
                    {
                        var cargo = _sigper.GetCargo(obj.Pl_CodCar.Value);
                        if (cargo != null)
                            obj.Pl_DesCar = cargo.Pl_DesCar;
                    }
                    if (obj.Pl_UndCod.HasValue)
                    {
                        var unidad = _sigper.GetUnidad(obj.Pl_UndCod.Value);
                        if (unidad != null)
                            obj.Pl_UndDes = unidad.Pl_UndDes;
                    }

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
        public ResponseMessage ContratoCreatePDF(Contrato obj)
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
        public ResponseMessage ContratoUpdate(Contrato obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (response.IsValid)
                {
                    if (obj.Pl_CodCar.HasValue)
                    {
                        var cargo = _sigper.GetCargo(obj.Pl_CodCar.Value);
                        if (cargo != null)
                            obj.Pl_DesCar = cargo.Pl_DesCar;
                    }
                    if (obj.Pl_UndCod.HasValue)
                    {
                        var unidad = _sigper.GetUnidad(obj.Pl_UndCod.Value);
                        if (unidad != null)
                            obj.Pl_UndDes = unidad.Pl_UndDes;
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

    }
}