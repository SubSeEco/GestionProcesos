﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(App.Web.Startup))]

namespace App.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}