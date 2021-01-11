﻿using System;
using App.Model.Core;
using App.Core.Interfaces;


namespace App.Core.UseCases
{
    public class UseCaseDocumento
    {
        protected readonly IGestionProcesos _repository;

        public UseCaseDocumento(IGestionProcesos repository)
        {
            _repository = repository;
        }
        public ResponseMessage Delete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Documento>(id);
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

        public ResponseMessage DeleteActivo(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Documento>(id);
                if (obj == null)
                    response.Errors.Add("Dato no encontrado");

                if (response.IsValid)
                {
                    obj.Activo = false;

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