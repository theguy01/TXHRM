﻿using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using TXHRM.Data;
using TXHRM.Identity;
using TXHRM.Model.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using TXHRM.WebAPI.Providers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;

[assembly: OwinStartup(typeof(TXHRM.WebAPI.App_Start.Startup))]

namespace TXHRM.WebAPI.App_Start
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(TXHRMDbContext.Create);

            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<AppSignInManager>(AppSignInManager.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);
            app.CreatePerOwinContext<UserManager<AppUser>>(CreateManager);

            //Allow Cross origin for API
            app.UseCors(CorsOptions.AllowAll);



            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/oauth/token"),
                Provider = new AuthorizationServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                AllowInsecureHttp = true
            });
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Branch the pipeline here for requests that start with "/signalr"
            //app.Map("/signalr", map =>
            //{
            //    // Setup the CORS middleware to run before SignalR.
            //    // By default this will allow all origins. You can 
            //    // configure the set of origins and/or http verbs by
            //    // providing a cors options with a different policy.
            //    map.UseCors(CorsOptions.AllowAll);

            //    map.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions()
            //    {
            //        Provider = new QueryStringOAuthBearerProvider()
            //    });

            //    var hubConfiguration = new HubConfiguration
            //    {
            //        // You can enable JSONP by uncommenting line below.
            //        // JSONP requests are insecure but some older browsers (and some
            //        // versions of IE) require JSONP to work cross domain
            //        EnableJSONP = true,
            //        EnableDetailedErrors = true
            //    };
            //    // Run the SignalR pipeline. We're not using MapSignalR
            //    // since this branch already runs under the "/signalr"
            //    // path.
            //    map.RunSignalR(hubConfiguration);
            //});

        }

        private static UserManager<AppUser> CreateManager(IdentityFactoryOptions<UserManager<AppUser>> options, IOwinContext context)
        {
            var userStore = new UserStore<AppUser>(context.Get<TXHRMDbContext>());
            var owinManager = new UserManager<AppUser>(userStore);
            return owinManager;
        }
    }
}
