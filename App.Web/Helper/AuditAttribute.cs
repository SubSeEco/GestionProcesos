using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DapperExtensions;
using System.Data.SqlClient;
using App.Model.Core;

namespace App.Web
{
    public class AuditAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            var headers = string.Empty;
            foreach (var key in request.Headers.AllKeys)
                if (key != null)
                    headers += key + " = " + request.Headers[key] + Environment.NewLine;

            var content = string.Empty;
            if (request.Files.Count == 0)
            {
                var parsed = HttpUtility.ParseQueryString(Encoding.Default.GetString(request.BinaryRead(request.TotalBytes)));
                foreach (var key in parsed.AllKeys)
                    if (!string.IsNullOrEmpty(key))
                        if (!key.ToUpper().Contains("PASSWORD"))
                            content += key + " = " + parsed[key] + Environment.NewLine;
            }

            var log = new CoreLog
            {
                LogId = Guid.NewGuid(),
                LogUserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous",
                LogIpAddress = request.UserHostAddress ?? request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.ServerVariables["REMOTE_ADDR"],
                LogAreaAccessed = request.RawUrl,
                LogTimeUtc = DateTime.UtcNow,
                LogTimeLocal = DateTime.Now,
                LogAgent = request.UserAgent,
                LogHttpMethod = request.HttpMethod,
                LogHeader = headers,
                LogContent = content
            };

            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["GestionProcesos"].ToString()))
                {
                    connection.Insert(log);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}