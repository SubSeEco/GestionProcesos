using App.Core.Interfaces;
using System.Web.Mvc;

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

        [HttpGet]
        public JsonResult GetUserByTerm(string term)
        {
            var result = _sigper.GetUserByTermUnidad(term);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
