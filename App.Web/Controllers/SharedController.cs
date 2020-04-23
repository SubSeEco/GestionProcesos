using System.Web.Mvc;

namespace App.Web.Controllers
{ 
    public class SharedController : Controller {
        public ActionResult Error(string message) {
            ViewBag.Error = message;
            return View(message);
        }
    }
}