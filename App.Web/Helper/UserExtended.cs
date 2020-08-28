using System.Security.Claims;
using System.Security.Principal;

namespace App.Web
{
    public static class UserExtended
    {
        public static string Email(this IPrincipal user)
        {
            var claim = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.Email);
            return claim == null ? null : claim.Value;
        }
    }
}
