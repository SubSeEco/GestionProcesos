using App.Model.Core;
using System.Collections.Generic;
using System.Net.Mail;

namespace App.Core.Interfaces
{
    public interface IEmail 
    {
        void NotificarNuevoWorkflow(Workflow workflow, Configuracion plantillaCorreo, Configuracion asunto);
        void NotificarInicioProceso(Proceso proceso, Configuracion plantillaCorreo, Configuracion asunto);
        void NotificarFinProceso(Proceso proceso, Configuracion plantillaCorreo, Configuracion asunto);
        void NotificarAnulacionProceso(Proceso proceso, Configuracion plantillaCorreo, Configuracion asunto);
        void NotificarRespuestaSIAC(Proceso proceso, Configuracion plantillaCorreo, Configuracion asunto);
        void NotificarFirmaResolucionCometido(Workflow workflow, Configuracion plantillaCorreo, Configuracion asunto, List<string> Mails);
        void NotificacionesCometido(Workflow workflow, Configuracion plantillaCorreo, string asunto, List<string> Mails, int IdCometido, string FechaSolicitud, string Observaciones, string Url, Documento documento, string Folio, string FechaFirma, string TipoActoAdm);
        void NotificacionesMemorandum(Workflow workflow, Configuracion plantillaCorreo, string asunto, List<string> Mails, int IdMemorandum, Documento documento);
        void NotificacionesHorasExtras(Workflow workflow, Configuracion plantillaCorreo, string asunto, List<string> Mails, int IdHorasExtras, string FechaSolicitud, string Observaciones, string Url, Documento documento, string Folio, string FechaFirma, string TipoActoAdm);
        void Send(MailMessage message);
    }
}