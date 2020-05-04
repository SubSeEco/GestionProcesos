using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using App.Core.Interfaces;
using App.Infrastructure.File;
using App.Model.Core;

namespace App.Infrastructure.Email
{
    public class Email : IEmail
    {
        public void Send()
        {
            try
            {
                //smtpClient.Send(emailMsg);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void NotificarInicioProceso(Model.Core.Proceso proceso, Model.Core.Configuracion plantillaCorreo, Model.Core.Configuracion asunto)
        {
            if (plantillaCorreo == null || (plantillaCorreo != null && string.IsNullOrWhiteSpace(plantillaCorreo.Valor)))
                throw new Exception("No existe la plantilla de notificación de tareas");
            if (asunto == null || (asunto != null && string.IsNullOrWhiteSpace(asunto.Valor)))
                throw new Exception("No se ha configurado el asunto de los correos electrónicos");

            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Id]", proceso.ProcesoId.ToString());
            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Proceso]", proceso.DefinicionProceso.Nombre);

            using (MailMessage emailMsg = new MailMessage())
            {
                emailMsg.IsBodyHtml = true;
                emailMsg.Body = plantillaCorreo.Valor;
                emailMsg.Subject = asunto.Valor;
                emailMsg.To.Add(proceso.Email);
            }

            Send();
        }

        public void NotificarCambioWorkflow(Model.Core.Workflow workflow, Model.Core.Configuracion plantillaCorreo, Model.Core.Configuracion asunto)
        {
            if (plantillaCorreo == null || (plantillaCorreo != null && string.IsNullOrWhiteSpace(plantillaCorreo.Valor)))
                throw new Exception("No existe la plantilla de notificación de tareas");
            if (asunto == null || (asunto != null && string.IsNullOrWhiteSpace(asunto.Valor)))
                throw new Exception("No se ha configurado el asunto de los correos electrónicos");

            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Id]", workflow.WorkflowId.ToString());
            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Proceso]", workflow.DefinicionWorkflow.DefinicionProceso.Nombre);
            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Tarea]", workflow.DefinicionWorkflow.Nombre);

            if (plantillaCorreo.Valor.Contains("[Estado]") && workflow.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Estado]", "Aprobado");
            if (plantillaCorreo.Valor.Contains("[Estado]") && workflow.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Estado]", "Rechazado");

            using (MailMessage emailMsg = new MailMessage())
            {
                emailMsg.IsBodyHtml = true;
                emailMsg.Body = plantillaCorreo.Valor;
                emailMsg.Subject = asunto.Valor;

                switch (workflow.DefinicionWorkflow.TipoEjecucionId)
                {
                    case (int)Util.Enum.TipoEjecucion.CualquierPersonaGrupo:
                    case (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso:
                    case (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso:

                        emailMsg.To.Add(workflow.Email);
                        break;

                    case (int)App.Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico:

                        return;
                }
            }

            Send();
        }

        public void NotificarRespuestaSIAC(Model.Core.Proceso proceso, Model.Core.Configuracion plantillaCorreo, Model.Core.Configuracion asunto)
        {
            if (plantillaCorreo == null || (plantillaCorreo != null && string.IsNullOrWhiteSpace(plantillaCorreo.Valor)))
                throw new Exception("No existe la plantilla de notificación de tareas");
            if (asunto == null || (asunto != null && string.IsNullOrWhiteSpace(asunto.Valor)))
                throw new Exception("No se ha configurado el asunto de los correos electrónicos");

            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Id]", proceso.ProcesoId.ToString());
            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Proceso]", proceso.DefinicionProceso.Nombre);

            using (MailMessage emailMsg = new MailMessage())
            {
                emailMsg.IsBodyHtml = true;
                emailMsg.Body = plantillaCorreo.Valor;
                emailMsg.Subject = asunto.Valor;
                emailMsg.To.Add(proceso.Email);
            }

            Send();
        }

        public void NotificarFirmaResolucionCometido(Model.Core.Workflow workflow, Model.Core.Configuracion plantillaCorreo, Model.Core.Configuracion asunto, List<string> Mails)
        {
            if (plantillaCorreo == null || (plantillaCorreo != null && string.IsNullOrWhiteSpace(plantillaCorreo.Valor)))
                throw new Exception("No existe la plantilla de notificación de tareas");
            if (asunto == null || (asunto != null && string.IsNullOrWhiteSpace(asunto.Valor)))
                throw new Exception("No se ha configurado el asunto de los correos electrónicos");

            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Id]", workflow.WorkflowId.ToString());
            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Proceso]", workflow.DefinicionWorkflow.DefinicionProceso.Nombre);
            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Tarea]", workflow.DefinicionWorkflow.Nombre);

            if (plantillaCorreo.Valor.Contains("[Estado]") && workflow.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Estado]", "Aprobado");
            if (plantillaCorreo.Valor.Contains("[Estado]") && workflow.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Estado]", "Rechazado");

            using (MailMessage emailMsg = new MailMessage())
            {
                emailMsg.IsBodyHtml = true;
                emailMsg.Body = plantillaCorreo.Valor;
                emailMsg.Subject = asunto.Valor;

                switch (workflow.DefinicionWorkflow.TipoEjecucionId)
                {
                    case (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo:
                    case (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso:
                    case (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso:

                        emailMsg.To.Add(workflow.Email);
                        break;

                    case (int)App.Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico:

                        return;
                }

                foreach (var correo in Mails)
                {
                    emailMsg.To.Add(correo);
                }
            }

            Send();
        }

        public void NotificacionesCometido(Model.Core.Workflow workflow, Model.Core.Configuracion plantillaCorreo, string asunto, List<string> Mails, int IdCometido, string FechaSolicitud, string Observaciones, string Url, Documento documento, string Folio, string FechaFirma, string TipoActoAdm)
        {
            if (plantillaCorreo == null || (plantillaCorreo != null && string.IsNullOrWhiteSpace(plantillaCorreo.Valor)))
                throw new Exception("No existe la plantilla de notificación de tareas");
            if (asunto == null || (asunto != null && string.IsNullOrWhiteSpace(asunto)))
                throw new Exception("No se ha configurado el asunto de los correos electrónicos");

            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[IdCometido]", IdCometido.ToString());
            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[FechaSolicitud]", FechaSolicitud);
            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Observaciones]", Observaciones);
            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Url]", Url);
            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Folio]", Folio);
            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[FechaFirma]", FechaFirma);
            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[TipoActoAdm]", TipoActoAdm);


            if (plantillaCorreo.Valor.Contains("[Estado]") && workflow.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Estado]", "Aprobado");
            if (plantillaCorreo.Valor.Contains("[Estado]") && workflow.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Estado]", "Rechazado");

            SmtpClient smtpClient = new SmtpClient();
            MailMessage emailMsg = new MailMessage();            
            emailMsg.IsBodyHtml = true;
            emailMsg.Body = plantillaCorreo.Valor;
            emailMsg.Subject = asunto;

            if (workflow.DefinicionWorkflow.Secuencia == 16) //|| workflow.DefinicionWorkflow.Secuencia == 14 || workflow.DefinicionWorkflow.Secuencia == 15)
            {
                
                MemoryStream ms = new MemoryStream(documento.File);
                Attachment attach = new Attachment(ms, documento.FileName);
                emailMsg.Attachments.Add(attach);
            }
            

            switch (workflow.DefinicionWorkflow.TipoEjecucionId)
            {
                case (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo:
                case (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso:
                case (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso:

                    emailMsg.To.Add(workflow.Email);
                    break;

                case (int)App.Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico:

                    return;
            }

            foreach (var correo in Mails)
            {
                emailMsg.To.Add(correo);
            }

            try
            {
                smtpClient.Send(emailMsg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}