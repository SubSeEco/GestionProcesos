using App.Model.Core;
using System.Collections.Generic;

namespace App.Core.Interfaces
{
    public interface IEmail 
    {
        void NotificarNuevoWorkflow(Workflow workflow, Configuracion plantillaCorreo, Configuracion asunto);
        void NotificarInicioProceso(Proceso proceso, Configuracion plantillaCorreo, Configuracion asunto);
        void NotificarFinProceso(Proceso proceso, Configuracion plantillaCorreo, Configuracion asunto);
        void NotificarAnulacionProceso(Proceso proceso, Configuracion plantillaCorreo, Configuracion asunto);
        //void NotificarRespuestaSIAC(Proceso proceso, Configuracion plantillaCorreo, Configuracion asunto);
        //void NotificarFirmaResolucionCometido(Workflow workflow, Configuracion plantillaCorreo, Configuracion asunto, List<string> Mails);
        void NotificacionesCometido(Workflow workflow, Configuracion plantillaCorreo, string asunto, List<string> mails, int idCometido, string fechaSolicitud, string observaciones, string url, Documento documento, string folio, string fechaFirma, string tipoActoAdm);
        //void NotificacionesMemorandum(Workflow workflow, Configuracion plantillaCorreo, string asunto, List<string> mails, int idMemorandum, Documento documento);
        void NotificacionesHorasExtras(Workflow workflow, Configuracion plantillaCorreo, string asunto, List<string> mails, int idHorasExtras, string fechaSolicitud, string observaciones, string url, Documento documento, string folio, string fechaFirma, string tipoActoAdm);
        //void Send(MailMessage message);
    }
}