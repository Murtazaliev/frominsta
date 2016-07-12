using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using InstagramMVC.Models.NavModel;
using InstagramMVC.Globals;

namespace InstagramMVC
{
    // Примечание: Инструкции по включению классического режима IIS6 или IIS7 
    // см. по ссылке http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(DateTime), new Binders.DateTimeBinder());
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            //получить куки авторизации
            HttpCookie authCookie = Context.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == "")
                return;

            //получить и декодировать билет авторизации, кот был создан при авторизации
            System.Web.Security.FormsAuthenticationTicket authTicket;
            try
            {
                authTicket = System.Web.Security.FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return;
            }

            //вытащить из UserData роли
            string[] roles = authTicket.UserData.Split(';');
            //сформировать новый контекст с ролями
            if (Context.User != null)
                Context.User = new System.Security.Principal.GenericPrincipal(Context.User.Identity, roles);
        }

        void Session_Start(object sender, EventArgs e)
        {
            //HttpContext.Current.Session["PROFILEPANEL"] = null;
            //HttpContext.Current.Session["ADMINPANEL"] = null;
            InstagramMVC.DataManagers.UtilManager.CreateUserPanel(HttpContext.Current);
            InstagramMVC.DataManagers.UtilManager.CreateAdminPanel(HttpContext.Current);
            //UrlHelper uh = new UrlHelper(HttpContext.Current.Request.RequestContext);
            //var _tabPanel = new TabPanel()
            //{
            //    Tabs = new List<TabItem>() { new TabItem() 
            //                                             { 
            //                                                 TabCaption = "Общие данные",
            //                                                 TabLinkURL = uh.Action("EditUser", "User"),
            //                                                 TabGroupIDs = AppEnums.Role.Admin.ToString() + "," + AppEnums.Role.TagModerator.ToString(),
            //                                                 TabOrderPos = 0,
            //                                                 TabActive = false
            //                                             },
            //                                 new TabItem() 
            //                                             { 
            //                                                 TabCaption = "Мои теги",
            //                                                 TabLinkURL = uh.Action("Tags", "User"),
            //                                                 TabGroupIDs = AppEnums.Role.TagModerator.ToString(),
            //                                                 TabOrderPos = 1,
            //                                                 TabActive = false
            //                                             },
            //                                 new TabItem() 
            //                                             { 
            //                                                 TabCaption = "Мои заказы",
            //                                                 TabLinkURL = uh.Action("User", "Show"),
            //                                                 TabGroupIDs = AppEnums.Role.TagModerator.ToString(),
            //                                                 TabOrderPos = 2,
            //                                                 TabActive = false
            //                                             },
            //                                 new TabItem() 
            //                                             { 
            //                                                 TabCaption = "Модерация",
            //                                                 TabLinkURL = uh.Action("Mod", "User"),
            //                                                 TabGroupIDs = AppEnums.Role.TagModerator.ToString(),
            //                                                 TabOrderPos = 3,
            //                                                 TabActive = false
            //                                             },
            //                                 new TabItem() 
            //                                             { 
            //                                                 TabCaption = "Параметры",
            //                                                 TabLinkURL = uh.Action("Options", "User"),
            //                                                 TabGroupIDs = AppEnums.Role.TagModerator.ToString(),
            //                                                 TabOrderPos = 4,
            //                                                 TabActive = false
            //                                             },
            //                                 new TabItem() 
            //                                             { 
            //                                                 TabCaption = "Сменить пароль",
            //                                                 TabLinkURL = uh.Action("Changepassword", "User"),
            //                                                 TabGroupIDs = AppEnums.Role.Admin.ToString() + "," + AppEnums.Role.TagModerator.ToString(),
            //                                                 TabOrderPos = 5,
            //                                                 TabActive = false
            //                                             }
            //                                }
            //};

            //HttpContext.Current.Session["PROFILEPANEL"] = _tabPanel;

            //if (System.Web.HttpContext.Current.User.IsInRole("Admin"))
            //{
            //    var _admPanel = new TabPanel()
            //    {
            //        Tabs = new List<TabItem>() { new TabItem() 
            //                                         { 
            //                                             TabCaption = "Переменные",
            //                                             TabLinkURL = uh.Action("EditVariables", "Admin"),
            //                                             TabGroupIDs = AppEnums.Role.Admin.ToString(),
            //                                             TabOrderPos = 1,
            //                                             TabActive = false
            //                                         },
            //                                         new TabItem() 
            //                                         { 
            //                                             TabCaption = "Пользователи",
            //                                             TabLinkURL = uh.Action("EditUsers", "Admin"),
            //                                             TabGroupIDs = AppEnums.Role.Admin.ToString(),
            //                                             TabOrderPos = 2,
            //                                             TabActive = false
            //                                         },
            //                                        new TabItem() 
            //                                         { 
            //                                             TabCaption = "Администраторы",
            //                                             TabLinkURL = uh.Action("EditAdmins", "Admin"),
            //                                             TabGroupIDs = AppEnums.Role.Admin.ToString(),
            //                                             TabOrderPos = 3,
            //                                             TabActive = false
            //                                         }
            //                                }
            //    };

            //    Session["ADMINPANEL"] = _admPanel;
            //}
        }
    }
}