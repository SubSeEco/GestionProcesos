﻿using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using App.Core.Interfaces;
using ExpressiveAnnotations.MvcUnobtrusive.Providers;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;

namespace App.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredIfAttribute),typeof(RequiredAttributeAdapter));
            ModelValidatorProviders.Providers.Remove(
                  ModelValidatorProviders.Providers
                      .FirstOrDefault(x => x is DataAnnotationsModelValidatorProvider));
            ModelValidatorProviders.Providers.Add(
                new ExpressiveAnnotationsModelValidatorProvider());

            //AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimsIdentity.DefaultNameClaimType;

            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            container.Register(typeof(IGestionProcesos), typeof(Infrastructure.GestionProcesos.GestionProcesos));
            container.Register(typeof(IEmail), typeof(Infrastructure.Email.Email));
            container.Register(typeof(IHSM), typeof(Infrastructure.HSM.HSM));
            container.Register(typeof(ISIGPER), typeof(Infrastructure.SIGPER.SIGPER));
            container.Register(typeof(IFile), typeof(Infrastructure.File.File));
            container.Register(typeof(IFolio), typeof(Infrastructure.Folio.Folio));
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }
}