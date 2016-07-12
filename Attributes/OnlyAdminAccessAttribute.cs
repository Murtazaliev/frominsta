using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Routing;

namespace InstagramMVC.Attributes
{
    /// <summary>
    /// Контроллер AdminController только для роли Admin
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class OnlyAdminAccessAttribute : AuthorizeAttribute
    {
        //protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //{
        //    base.HandleUnauthorizedRequest(filterContext);
        //    if (filterContext.HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Inedx", controller = "Home" }));
        //    }
        //    else
        //    {
                
                
        //    }
        //}

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.HttpContext.User.Identity.IsAuthenticated && !filterContext.HttpContext.User.IsInRole("Admin"))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "AccessDenied", controller = "Account" }));
            }
        }
    }
}