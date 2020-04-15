using System;
using App.Model.Core;
using App.Model.GestionDocumental;
using App.Core.Interfaces;

namespace App.Core.UseCases
{
    public class UseCaseGD
    {
        protected readonly IGestionProcesos _repository;
        protected readonly IEmail _email;
        protected readonly IHSM _hsm;
        protected readonly ISIGPER _sigper;
        protected readonly IFile _file;
        protected readonly IFolio _folio;

        public UseCaseGD(IGestionProcesos repository, IFile file, IFolio folio)
        {
            _repository = repository;
            _file = file;
            _folio = folio;
        }

        public ResponseMessage Insert(GDIngreso obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (response.IsValid)
                {
                    obj.GetFolio();
                    obj.BarCode = _file.CreateBarCode(obj.Folio);

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
        public ResponseMessage Update(GDIngreso obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (response.IsValid)
                {
                    obj.GetFolio();
                    obj.BarCode = _file.CreateBarCode(obj.Folio);

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
        public ResponseMessage Delete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<GDIngreso>(id);
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