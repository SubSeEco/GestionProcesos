using App.Core.Interfaces;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace App.Web
{
    public class AuditAttribute : ActionFilterAttribute
    {
        public IGestionProcesos _repository;

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

            //using (var context = new App.Infrastructure.GestionProcesos.AppContext()) {
            //    context.Log.Add( new Model.Core.Log {
            //        LogId = Guid.NewGuid(),
            //        LogUserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous",
            //        LogIpAddress = request.UserHostAddress ?? request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.ServerVariables["REMOTE_ADDR"],
            //        LogAreaAccessed = request.RawUrl,
            //        LogTimeUtc = DateTime.UtcNow,
            //        LogTimeLocal = DateTime.Now,
            //        LogAgent = request.UserAgent,
            //        LogHttpMethod = request.HttpMethod,
            //        LogHeader = headers,
            //        LogContent = content
            //    });
            //    context.SaveChanges();
            //}

            System.Diagnostics.Debug.WriteLine(headers);

            base.OnActionExecuting(filterContext);
        }
    }
}