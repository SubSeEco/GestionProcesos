using App.Model.SIGPER;
using System.Collections.Generic;

namespace App.Core.Interfaces
{
    public interface ISIGPER
    {
        List<PECARGOS> GetCargos();
        PECARGOS GetCargo(int codigo);
        PLUNILAB GetUnidad(int codigo);
        SIGPER GetUserByEmail(string email);
        SIGPER GetUserByRut(int rut);
        List<PEDATPER> GetUserByTerm(string term);
        List<PEDATPER> GetUserByUnidad(int codigo);

        List<PEDATPER> GetUserByUnidadForFirma(int Rut);
        List<PEDATPER> GetAllUsersForCometido();
        List<PEDATPER> GetAllUsers();

        List<PLUNILAB> GetUnidades();
        SIGPER GetJefaturaByUnidad(int codigo);
        SIGPER GetSecretariaByUnidad(int codigo);
        List<DGREGIONES> GetRegion();
        string GetRegionbyComuna(string codComuna);
        List<DGCOMUNAS> GetComunasbyRegion(string IdRegion);
        List<DGCOMUNAS> GetDGCOMUNAs();
        //List<DGPAISES> GetDGPAISESs();
        List<DGESCALAFONES> GetGESCALAFONEs();
        List<PECARGOS> GetPECARGOs();
        List<DGESTAMENTOS> GetDGESTAMENTOs();
        List<ReContra> GetReContra();
        List<REPYT> GetREPYTs();
        List<DGCONTRATOS> GetDGCONTRATOS();
        //Model.Cometido.Destinos GetMontoViaticos(int CometidoId, int Cantdias);

        List<PLUNILAB> GetUnidadesFirmantes(List<string> listEmailFirmantes);
        List<PEDATPER> GetUserFirmanteByUnidad(int codigoUnidad, List<string> listEmailFirmantes);
        SIGPER NewGetUserByEmail(string email);
        int GetBaseCalculoHorasExtras(int rut, int mes, int anno, int calidad);
    }
}