using System;
using App.Model.Core;
using App.Core.Interfaces;

namespace App.Core.UseCases
{
    public class UseCaseDocumento
    {
        private readonly IGestionProcesos _repository;

        public UseCaseDocumento(IGestionProcesos repository)
        {
            _repository = repository;
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
                    if(!obj.Signed)
                    {
                        if(obj.TipoDocumentoId==(int)Util.Enum.TipoDocumento.RefrendacionPresupuesto)
                        {
                            _repository.Delete(obj);
                            _repository.Save();
                        }
                        else
                        {
                            obj.Activo = false;
                            _repository.Update(obj);
                            _repository.Save();
                        }
                    }
                    else
                    {
                        throw new Exception("No se puede eliminar un documento ya firmado.");
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