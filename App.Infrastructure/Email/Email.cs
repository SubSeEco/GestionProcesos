using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Linq;
using App.Core.Interfaces;
using App.Model.Core;

namespace App.Infrastructure.Email
{
    public class Email : IEmail
    {
        public void Send(MailMessage message)
        {
            try
            {
                var smtpClient = new SmtpClient();
                if (message != null)
                {
                    smtpClient.Send(message);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public void NotificarInicioProceso(Model.Core.Proceso proceso, Model.Core.Configuracion plantillaCorreo, Model.Core.Configuracion asunto)
        {
            if (plantillaCorreo == null || (plantillaCorreo != null && string.IsNullOrWhiteSpace(plantillaCorreo.Valor)))
                throw new Exception("No existe la plantilla de notificación de tareas");
            if (asunto == null || (asunto != null && string.IsNullOrWhiteSpace(asunto.Valor)))
                throw new Exception("No se ha configurado el asunto de los correos electrónicos");

            var body = plantillaCorreo.Valor.Replace("[Id]", proceso.ProcesoId.ToString());
            body = body.Replace("[Proceso]", proceso.DefinicionProceso.Nombre);

            using (MailMessage emailMsg = new MailMessage())
            {
                emailMsg.IsBodyHtml = true;
                emailMsg.Body = body;
                emailMsg.Subject = asunto.Valor;
                emailMsg.To.Add(proceso.Email);
                Send(emailMsg);
            }
        }

        public void NotificarNuevoWorkflow(Model.Core.Workflow workflow, Model.Core.Configuracion plantillaCorreo, Model.Core.Configuracion asunto)
        {
            if (plantillaCorreo == null || (plantillaCorreo != null && string.IsNullOrWhiteSpace(plantillaCorreo.Valor)))
                throw new Exception("No existe la plantilla de notificación de tareas");
            if (asunto == null || (asunto != null && string.IsNullOrWhiteSpace(asunto.Valor)))
                throw new Exception("No se ha configurado el asunto de los correos electrónicos");

            var body = plantillaCorreo.Valor.Replace("[Id]", workflow.WorkflowId.ToString());
            body = body.Replace("[IdProceso]", workflow.ProcesoId.ToString());
            body = body.Replace("[Proceso]", workflow.DefinicionWorkflow.DefinicionProceso.Nombre);
            body = body.Replace("[Tarea]", workflow.DefinicionWorkflow.Nombre);
            if (!string.IsNullOrWhiteSpace(workflow.Mensaje))
                body = body.Replace("[Observacion]", workflow.Mensaje);
            else
                body = body.Replace("[Observacion]", string.Empty);

            using (MailMessage emailMsg = new MailMessage())
            {
                emailMsg.IsBodyHtml = true;
                emailMsg.Body = body;
                emailMsg.Subject = asunto.Valor;

                //copiar a las personas del grupo
                if (!string.IsNullOrEmpty(workflow.Email) && workflow.Email.Contains(';'))
                    foreach (var email in workflow.Email.Split(';'))
                        if (!emailMsg.To.Any(q => q.Address == email))
                            emailMsg.To.Add(email);

                //copiar a la persona
                if (!string.IsNullOrEmpty(workflow.Email) && !workflow.Email.Contains(';'))
                    emailMsg.To.Add(workflow.Email);

                //copiar al dueño del proceso
                if (!string.IsNullOrEmpty(workflow.Proceso.Email))
                    if (!emailMsg.To.Any(q => q.Address == workflow.Proceso.Email))
                        emailMsg.CC.Add(workflow.Proceso.Email);

                Send(emailMsg);
            }
        }

        public void NotificarFinProceso(Model.Core.Proceso proceso, Model.Core.Configuracion plantillaCorreo, Model.Core.Configuracion asunto)
        {
            if (plantillaCorreo == null || (plantillaCorreo != null && string.IsNullOrWhiteSpace(plantillaCorreo.Valor)))
                throw new Exception("No existe la plantilla fin de proceso");
            if (asunto == null || (asunto != null && string.IsNullOrWhiteSpace(asunto.Valor)))
                throw new Exception("No se ha configurado el asunto de los correos electrónicos");

            var body = plantillaCorreo.Valor.Replace("[Id]", proceso.ProcesoId.ToString());
            body = body.Replace("[Proceso]", proceso.DefinicionProceso.Nombre);

            using (MailMessage emailMsg = new MailMessage())
            {
                emailMsg.IsBodyHtml = true;
                emailMsg.Body = body;
                emailMsg.Subject = asunto.Valor;
                emailMsg.To.Add(proceso.Email);
                Send(emailMsg);
            }
        }

        public void NotificarAnulacionProceso(Model.Core.Proceso proceso, Model.Core.Configuracion plantillaCorreo, Model.Core.Configuracion asunto)
        {
            if (plantillaCorreo == null || (plantillaCorreo != null && string.IsNullOrWhiteSpace(plantillaCorreo.Valor)))
                throw new Exception("No existe la plantilla fin de proceso");
            if (asunto == null || (asunto != null && string.IsNullOrWhiteSpace(asunto.Valor)))
                throw new Exception("No se ha configurado el asunto de los correos electrónicos");

            var body = plantillaCorreo.Valor.Replace("[Id]", proceso.ProcesoId.ToString());
            body = body.Replace("[Proceso]", proceso.DefinicionProceso.Nombre);

            using (MailMessage emailMsg = new MailMessage())
            {
                emailMsg.IsBodyHtml = true;
                emailMsg.Body = body;
                emailMsg.Subject = asunto.Valor;
                emailMsg.To.Add(proceso.Email);
                Send(emailMsg);
            }
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
                Send(emailMsg);
            }
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
                Send(emailMsg);
            }
        }

        public void NotificacionesCometido(Model.Core.Workflow workflow, Model.Core.Configuracion plantillaCorreo, string asunto, List<string> Mails, int IdCometido, string FechaSolicitud, string Observaciones, string Url, Documento documento, string Folio, string FechaFirma, string TipoActoAdm)
        {
            if (plantillaCorreo == null || (plantillaCorreo != null && string.IsNullOrWhiteSpace(plantillaCorreo.Valor)))
                throw new Exception("No existe la plantilla de notificación de tareas");
            if (asunto == null || (asunto != null && string.IsNullOrWhiteSpace(asunto)))
                throw new Exception("No se ha configurado el asunto de los correos electrónicos");

            var Mail = plantillaCorreo;

            Mail.Valor = Mail.Valor.Replace("[IdCometido]", IdCometido.ToString());
            Mail.Valor = Mail.Valor.Replace("[FechaSolicitud]", FechaSolicitud);
            Mail.Valor = Mail.Valor.Replace("[Observaciones]", Observaciones);
            Mail.Valor = Mail.Valor.Replace("[Url]", Url);
            Mail.Valor = Mail.Valor.Replace("[Folio]", Folio);
            Mail.Valor = Mail.Valor.Replace("[FechaFirma]", FechaFirma);
            Mail.Valor = Mail.Valor.Replace("[TipoActoAdm]", TipoActoAdm);


            if (Mail.Valor.Contains("[Estado]") && workflow.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                Mail.Valor = Mail.Valor.Replace("[Estado]", "Aprobado");
            if (Mail.Valor.Contains("[Estado]") && workflow.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                Mail.Valor = Mail.Valor.Replace("[Estado]", "Rechazado");

            SmtpClient smtpClient = new SmtpClient();
            MailMessage emailMsg = new MailMessage();
            emailMsg.IsBodyHtml = true;
            emailMsg.Body = Mail.Valor;
            emailMsg.Subject = asunto;

            if (workflow.DefinicionWorkflow.Secuencia == 13 || workflow.DefinicionWorkflow.Secuencia == 14 || workflow.DefinicionWorkflow.Secuencia == 15)
            {

                MemoryStream ms = new MemoryStream(documento.File);
                Attachment attach = new Attachment(ms, documento.FileName);
                emailMsg.Attachments.Add(attach);
            }


            switch (workflow.DefinicionWorkflow.TipoEjecucionId)
            {
                case (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo:
                case (int)App.Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico:
                case (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso:
                case (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso:

                    emailMsg.To.Add(workflow.Email);
                    break;

                //case (int)App.Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico:

                    //return;
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

        public void NotificacionesHorasExtras(Model.Core.Workflow workflow, Model.Core.Configuracion plantillaCorreo, string asunto, List<string> Mails, int IdHorasExtras, string FechaSolicitud, string Observaciones, string Url, Documento documento, string Folio, string FechaFirma, string TipoActoAdm)
        {
            if (plantillaCorreo == null || (plantillaCorreo != null && string.IsNullOrWhiteSpace(plantillaCorreo.Valor)))
                throw new Exception("No existe la plantilla de notificación de tareas");
            if (asunto == null || (asunto != null && string.IsNullOrWhiteSpace(asunto)))
                throw new Exception("No se ha configurado el asunto de los correos electrónicos");

            var Mail = plantillaCorreo;

            Mail.Valor = Mail.Valor.Replace("[IdHorasExtras]", IdHorasExtras.ToString());
            Mail.Valor = Mail.Valor.Replace("[FechaSolicitud]", FechaSolicitud);
            Mail.Valor = Mail.Valor.Replace("[Observaciones]", Observaciones);
            Mail.Valor = Mail.Valor.Replace("[Url]", Url);
            Mail.Valor = Mail.Valor.Replace("[Folio]", Folio);
            Mail.Valor = Mail.Valor.Replace("[FechaFirma]", FechaFirma);
            Mail.Valor = Mail.Valor.Replace("[TipoActoAdm]", TipoActoAdm);


            if (Mail.Valor.Contains("[Estado]") && workflow.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                Mail.Valor = Mail.Valor.Replace("[Estado]", "Aprobado");
            if (Mail.Valor.Contains("[Estado]") && workflow.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                Mail.Valor = Mail.Valor.Replace("[Estado]", "Rechazado");

            SmtpClient smtpClient = new SmtpClient();
            MailMessage emailMsg = new MailMessage();
            emailMsg.IsBodyHtml = true;
            emailMsg.Body = Mail.Valor;
            emailMsg.Subject = asunto;

            if (workflow.DefinicionWorkflow.Secuencia == 7) //|| workflow.DefinicionWorkflow.Secuencia == 14 || workflow.DefinicionWorkflow.Secuencia == 15)
            {

                MemoryStream ms = new MemoryStream(documento.File);
                Attachment attach = new Attachment(ms, documento.FileName);
                emailMsg.Attachments.Add(attach);
            }


            switch (workflow.DefinicionWorkflow.TipoEjecucionId)
            {
                case (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo:
                case (int)App.Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico:
                case (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso:
                case (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso:

                    emailMsg.To.Add(workflow.Email);
                    break;

                    //case (int)App.Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico:

                    //return;
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
        public void NotificacionesMemorandum(Model.Core.Workflow workflow, Model.Core.Configuracion plantillaCorreo, string asunto, List<string> Mails, int MemorandumId, Documento documento)
        {
            if (plantillaCorreo == null || (plantillaCorreo != null && string.IsNullOrWhiteSpace(plantillaCorreo.Valor)))
                throw new Exception("No existe la plantilla de notificación de tareas");
            if (asunto == null || (asunto != null && string.IsNullOrWhiteSpace(asunto)))
                throw new Exception("No se ha configurado el asunto de los correos electrónicos");

            plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[MemorandumId]", MemorandumId.ToString());
            //plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[FechaSolicitud]", FechaSolicitud);

            if (plantillaCorreo.Valor.Contains("[Estado]") && workflow.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Estado]", "Aprobado");
            if (plantillaCorreo.Valor.Contains("[Estado]") && workflow.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Rechazada)
                plantillaCorreo.Valor = plantillaCorreo.Valor.Replace("[Estado]", "Rechazado");

            SmtpClient smtpClient = new SmtpClient();
            MailMessage emailMsg = new MailMessage();
            emailMsg.IsBodyHtml = true;
            emailMsg.Body = plantillaCorreo.Valor;
            emailMsg.Subject = asunto;

            MemoryStream ms = new MemoryStream(documento.File);
            Attachment attach = new Attachment(ms, documento.FileName);
            emailMsg.Attachments.Add(attach);


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