using App.Core.Interfaces;
using App.Model.Core;
using System;

namespace App.Core.UseCases
{
    public class UseCaseDocumento
    {
        private readonly IGestionProcesos _repository;

        public UseCaseDocumento(IGestionProcesos repository)
        {
            _repository = repository;
        }
        public ResponseMessage DeleteActivo(int id, string mail)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<Documento>(id);
                if (obj == null)
                {
                    response.Errors.Add("Dato no encontrado");
                }
                else
                {
                    if (obj.Signed)
                    {
                        response.Errors.Add("No se puede eliminar un documento ya firmado.");
                    }
                }

                if (response.IsValid)
                {
                    if (obj.TipoDocumentoId == (int)Util.Enum.TipoDocumento.RefrendacionPresupuesto ||
                        obj.TipoDocumentoId == (int)Util.Enum.TipoDocumento.Resolucion)
                    {
                        _repository.Delete(obj);
                        _repository.Save();
                    }
                    else
                    {
                        obj.Activo = false;
                        obj.AnuladoPor = mail;
                        obj.FechaAnulacion = DateTime.Now;
                        _repository.Update(obj);
                        _repository.Save();
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