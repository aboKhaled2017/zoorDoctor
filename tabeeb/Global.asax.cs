using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using tabeeb.Models;
using System.Web.Security;
using System.Web.Optimization;
using tabeeb.App_Start;
using System.Web.Http;
using System.Web.WebPages;
namespace tabeeb
{
    public class MvcApplication : System.Web.HttpApplication
    {

        private tabeebekEntities db = new tabeebekEntities();
        protected void Application_Start()
        {
            DisplayModeProvider.Instance.Modes.Insert(0, new
            DefaultDisplayMode("Tablet")
            {
                ContextCondition = (ctx =>
                ctx.Request.UserAgent.IndexOf("iPad", StringComparison.OrdinalIgnoreCase) >= 0 ||
                ctx.Request.UserAgent.IndexOf("Android", StringComparison.OrdinalIgnoreCase) >= 0 &&
                ctx.Request.UserAgent.IndexOf("mobile", StringComparison.OrdinalIgnoreCase) < 1)
            });

            DisplayModeProvider.Instance.Modes.Insert(1, new DefaultDisplayMode("Mobile")
            {
                ContextCondition = (ctx =>
                    ctx.GetOverriddenBrowser().IsMobileDevice)
            });
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            bundlCreation.RegisterBundles(BundleTable.Bundles);
            ViewEngines.Engines.Clear(); //clear all engines
            ViewEngines.Engines.Add(new RazorViewEngine());
            /*DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("iPhone")
            {
                ContextCondition = (context => context.GetOverriddenUserAgent().IndexOf
                    ("iPhone", StringComparison.OrdinalIgnoreCase) >= 0)
            });*/
            

        }
        protected void Application_Error()
        {
        }
    }
}
