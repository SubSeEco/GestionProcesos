using System;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Collections.Generic;

namespace App.Web
{
    public class AuthenticationService
    {
        public static string LDAPServer = "maestrof.economia.cl:389";
        public static string LDAPContainer = "OU=OU,DC=economia,DC=cl";
        public static string LDAPUsername = "leer_ad";
        public static string LDAPPassword = "leer_ad";

        public class AuthenticationResult
        {
            public AuthenticationResult(string errorMessage = null)
            {
                ErrorMessage = errorMessage;
            }

            public String ErrorMessage { get; private set; }
            public Boolean IsSuccess => String.IsNullOrEmpty(ErrorMessage);
        }

        private readonly IAuthenticationManager authenticationManager;

        public AuthenticationService(IAuthenticationManager authenticationManager)
        {
            this.authenticationManager = authenticationManager;
        }

        public static IEnumerable<App.Model.DTO.DTODomainUser> GetDomainUser()
        {
            var principalContext = new PrincipalContext(ContextType.Domain, LDAPServer, LDAPContainer, LDAPUsername, LDAPPassword);
            var searcher = new PrincipalSearcher(new UserPrincipal(principalContext));

            return searcher.FindAll().Select(q => new App.Model.DTO.DTODomainUser() { Email = ((UserPrincipal)q).EmailAddress, User = q.Name });
        }

        public AuthenticationResult SignIn(String username, String password)
        {
            bool isAuthenticated = false;
            UserPrincipal userPrincipal = null;

            try
            {
                PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, LDAPServer,LDAPContainer, LDAPUsername, LDAPPassword);
                //isAuthenticated = principalContext.ValidateCredentials(username, password, ContextOptions.Negotiate);
                isAuthenticated = principalContext.ValidateCredentials(username, password);

                if (isAuthenticated)
                    userPrincipal = UserPrincipal.FindByIdentity(principalContext, username);

                if (!isAuthenticated || userPrincipal == null)
                    return new AuthenticationResult("Intento de acceso inválido");

                if (userPrincipal.IsAccountLockedOut())
                    return new AuthenticationResult("Cuenta bloqueada");

                if (userPrincipal.Enabled.HasValue && userPrincipal.Enabled.Value == false)
                    return new AuthenticationResult("Cuenta deshabilitada");

                ClaimsIdentity identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
                identity.AddClaim(new Claim(ClaimTypes.Name, userPrincipal.DisplayName, "http://www.w3.org/2001/XMLSchema#string"));
                identity.AddClaim(new Claim(ClaimTypes.Email, userPrincipal.EmailAddress, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"));

                authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);
            }
            catch (Exception ex)
            {
                isAuthenticated = false;
                userPrincipal = null;

                return new AuthenticationResult(ex.Message);
            }

            return new AuthenticationResult();
        }
    }
}