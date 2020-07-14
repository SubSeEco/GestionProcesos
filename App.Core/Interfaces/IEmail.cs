using App.Model.Core;
using System.Collections.Generic;

namespace App.Core.Interfaces
{
    public interface IEmail 
    {
        void NotificarCambioWorkflow(Model.Core.Workflow workflow, Model.Core.Configuracion plantillaCorreo, Model.Core.Configuracion asunto);
        void NotificarInicioProceso(Model.Core.Proceso proceso, Model.Core.Configuracion plantillaCorreo, Model.Core.Configuracion asunto);
        void NotificarRespuestaSIAC(Model.Core.Proceso proceso, Model.Core.Configuracion plantillaCorreo, Model.Core.Configuracion asunto);
        void NotificarFirmaResolucionCometido(Model.Core.Workflow workflow, Model.Core.Configuracion plantillaCorreo, Model.Core.Configuracion asunto, List<string> Mails);
        void NotificacionesCometido(Model.Core.Workflow workflow, Model.Core.Configuracion plantillaCorreo, string asunto, List<string> Mails, int IdCometido, string FechaSolicitud, string Observaciones, string Url, Documento documento, string Folio, string FechaFirma, string TipoActoAdm);
        void NotificacionesMemorandum(Model.Core.Workflow workflow, Model.Core.Configuracion plantillaCorreo, string asunto, List<string> Mails, int IdMemorandum, Documento documento);

        void Send();
    }
}