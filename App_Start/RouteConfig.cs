using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace InstagramMVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "",
                url: "Admin/Log/{page}/{user_id}/{event_id}",
                defaults: new { controller = "Admin", action = "Log", page = 1, user_id = 0, event_id = 0 }
            );

            routes.MapRoute(
                name: "",
                url: "User/Mod/{hashtag}/{page}",
                defaults: new { controller = "User", action = "Mod", hashtag = UrlParameter.Optional, page = 1 }
            );

            routes.MapRoute(
                name: "",
                url: "User/BotMod/{user_login}/{hashtag}/{page}",
                defaults: new { controller = "User", action = "BotMod", page = 1 }
            );

            routes.MapRoute(
                name: "",
                url: "Show/Edit/{show_id}",
                defaults: new { controller = "Show", action = "Edit" },
                constraints: new { show_id = @"^\d+$"}
            );

            routes.MapRoute(
                name: "",
                url: "Show/{action}/{user_login}",                
                defaults: new { controller = "Show", action = "User", user_login = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}