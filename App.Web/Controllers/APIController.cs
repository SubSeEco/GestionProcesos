using App.Model.Core;
using App.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using App.Core.UseCases;

namespace App.Web.Controllers
{
    [Audit]
    public class APIController : Controller
    {
        private readonly ISigper _sigper;
        
        public APIController(ISigper sigper)
        {
            _sigper = sigper;
        }

        public JsonResult SigperGetUnidades()
        {
            var result = _sigper.GetUnidades();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SigperGetUsuariosDeUnidad(int id)
        {
            var result = _sigper.GetUserByUnidad(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
