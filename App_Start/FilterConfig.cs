using System.Web;
using System.Web.Mvc;
using InstagramMVC.Attributes;

namespace InstagramMVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}