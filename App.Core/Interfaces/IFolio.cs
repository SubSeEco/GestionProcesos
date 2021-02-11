using System.Collections.Generic;

namespace App.Core.Interfaces
{
    public interface IFolio
    {
        List<Model.FirmaDocumento.DTOTipoDocumento> GetTipoDocumento();
        Model.FirmaDocumento.DTOSolicitud GetFolio(string solicitante, string tipoDocumento, string subSecretaria);
        List<Model.FirmaDocumento.DTOSolicitud> GetSolicitud();

    }
}