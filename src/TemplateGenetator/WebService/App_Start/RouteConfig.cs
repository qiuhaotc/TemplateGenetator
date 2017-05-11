using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebService
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Razor",
                url: "{controller}/{action}/{tempID}/{tablename}/{forceChange}",
                  defaults: new { controller = "Razor", action = "Index", tablename = UrlParameter.Optional, forceChange=UrlParameter.Optional}
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Razor", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
