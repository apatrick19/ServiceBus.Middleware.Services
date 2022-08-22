using ServicBus.Logic.Implementations.Memory;
using ServiceBus.Web.Portal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace ServiceBus.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);

            var logPath = AppDomain.CurrentDomain.BaseDirectory + @"\log4net.config";
            log4net.Config.XmlConfigurator.Configure(new FileInfo(logPath));

            Trace.TraceInformation("Loading all context into memory");

            MemoryManager.FetchLga();
            MemoryManager.FetchState();
            MemoryManager.FetchNationality();
            MemoryManager.FetchProducts();
            MemoryManager.FetchOfficer();          
            MemoryManager.FetchBanks();          
            MemoryManager.FetchBillerCategories();          
            MemoryManager.FetchBillers();          
            MemoryManager.FetchPaymentItems();          

            //InMemory.LoadCodeDescription();           
            Trace.TraceInformation("All types successfully loaded, MQ service running jobs ");
        }

       
    }
}
