using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace tabeeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                 "Default",
                 "",
                 new { area = "users", controller = "index", action = "Index", id = UrlParameter.Optional, lang = "ar-EG" },
                 null,
                 new[] { "tabeeb.Areas.users.Controllers" }
            ).DataTokens.Add("area","users");
        }
    }
}
