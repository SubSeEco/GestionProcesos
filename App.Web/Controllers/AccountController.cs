using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Controllers
{
    //[Audit]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new ModelUser());
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(ModelUser model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            var authService = new AuthenticationService(authenticationManager);
            var authenticationResult = authService.SignIn(model.UserName, model.Password);
            if (authenticationResult.IsSuccess)
                return RedirectToLocal(returnUrl);

            ModelState.AddModelError("", authenticationResult.ErrorMessage);
            return View(model);
        }

        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public virtual ActionResult Logoff()
        {
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }

        public class ModelUser
        {
            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Usuario")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Es necesario especificar este dato")]
            [Display(Name = "Password")]
            public string Password { get; set; }
        }
    }
}