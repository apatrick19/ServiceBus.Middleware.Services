using Hangfire;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using ServiceBus.Web.Injection;
using System;
using System.Web;
using System.Web.Http;
using Unity;

[assembly: OwinStartup(typeof(ServiceBus.Web.Startup))]
namespace ServiceBus.Web
{
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            var container = new UnityContainer();
           // Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage("AiroPayContext");
            //Hangfire.GlobalConfiguration.Configuration.UseActivator(new ContainerJobActivator(container));

          //  app.UseHangfireDashboard("/MicroService/Monitor");
           // app.UseHangfireServer();
            // app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            var myProvider = new CustomAuthProvider();
            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/security/tokenize"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(5),
                Provider = myProvider
            };
            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
        }

    }
}