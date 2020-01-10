using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "About", id = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "Products",
              url: "Home/Products",
              defaults: new { controller = "Home", action = "Products", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "Index",
            url: "Home/Index",
            defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
        );

            routes.MapRoute(
            name: "Product",
            url: "Home/Product",
            defaults: new { controller = "Home", action = "Product", id = UrlParameter.Optional}
        );

            routes.MapRoute(
            name: "FinalizareComanda",
            url: "Home/FinalizareComanda",
            defaults: new { controller = "Home", action = "FinalizareComanda", id = UrlParameter.Optional }
            );

        }
    }
}
