using System.Web.Mvc;
//using App.Model.Shared;

namespace App.Web.Controllers
{
    [Audit]
    [Authorize]
    public class CometidoComisionController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}