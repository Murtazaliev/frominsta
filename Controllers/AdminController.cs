using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InstagramMVC.Models;
using InstagramMVC.Models.InstagramModels;
using Newtonsoft.Json;
using InstagramMVC.DataManagers;
using InstagramMVC.Globals;
using InstagramMVC.Attributes;

namespace InstagramMVC.Controllers
{
    [OnlyAdminAccess]
    [Authorize(Roles="Admin")]    
    public class AdminController : Controller
    {
        //###############################   Параметры соединения с Instagramm Application   ###############################################
        static string clientId = UtilManager.GetVarValue("instagram.clientid");
        static string clientSecret = UtilManager.GetVarValue("instagram.clientsecret");
        static string redirectUri = UtilManager.GetVarValue("instagram.redirecturi");
        static string instagramLoginUri = UtilManager.GetVarValue("instagram.loginuri");
        static string instagramOAuthAccessTokenUri = UtilManager.GetVarValue("instagram.oauth.accesstokenuri");
        //#################################################################################################################################

        private string SuperAdmin = "Admin";

        public AdminController()
        {
            if (System.Web.HttpContext.Current.Session["ADMINPANEL"] == null)
            {
                UtilManager.CreateAdminPanel(System.Web.HttpContext.Current);
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Редактор переменных
        /// </summary>
        /// <returns></returns>
        public ActionResult EditVariables()
        {
            var vars = UtilManager.GetVariables();
            ViewBag.AllowSave = string.Compare(HttpContext.User.Identity.Name, SuperAdmin, true) == 0;
            return View(vars);
        }
                
        /// <summary>
        /// Сохранить переменные
        /// </summary>
        [HttpPost]
        public ActionResult SaveVariables(IList<Models.AdminModels.Variable> vars)
        {
            foreach (var key in vars)
            {
                UtilManager.SetVarValue(key.VarName, key.VarValue);
            }
            TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Success, Message = "Данные сохранены!" };
            UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID, AppEnums.Event.Сохранить_глобальные_переменные);
            return RedirectToAction("Index", "Admin");
        }

        /// <summary>
        /// Редактор пользователей
        /// </summary>
        public ActionResult EditUsers()
        {
            var users = UserManager.GetUsers(string.Format("{0},{1},{2}", 
                                             AppEnums.Role.PremiumModerator.ToString("d"), 
                                             AppEnums.Role.TagModerator.ToString("d"), 
                                             AppEnums.Role.Guest.ToString("d")));
            return View(users);
        }

        /// <summary>
        /// Редактировать пользователя
        /// </summary>
        public ActionResult EditUser(int id)
        {
            var user = UserManager.GetUser(id);
            //ViewBag.roles = UtilManager.GetRoles("Admin").Select(x => new SelectListItem()
            //{
            //    Text = x.ROLE_NAME,
            //    Value = x.ROLE_ID.ToString(),
            //    Selected = x.ROLE_ID == user.USER_ROLE_ID
            //});
            return View(user);
        }

        [HttpPost]
        public ActionResult EditUser(AppUser user)
        {
            if (ModelState.IsValid)
            {
                SQLReturnResult res = UserManager.UpdateUser(user);
                switch (res.Result)
                {
                    case AppEnums.SQLExecResult.RollBack:
                        TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Ошибка! Откат транзакции!\r\n" + res.Message };
                        break;
                    case AppEnums.SQLExecResult.SyntaxError:
                        TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Синтаксическая ошибка!\r\n" + res.Message };
                        break;
                    default:
                        TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Success, Message = "Данные сохранены!" };
                        UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID, AppEnums.Event.Изменение_данных_пользователя);
                        break;
                }

                return RedirectToAction("EditUsers", "Admin");
            }

            return View(user);
        }

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        public ActionResult DeleteUser(int id)
        {
            var deletedUser = UserManager.GetUser(id);
            SQLReturnResult res = UserManager.DeleteUser(deletedUser.USER_ID);
            switch (res.Result)
            {
                case AppEnums.SQLExecResult.RollBack:
                    TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Ошибка! Откат транзакции!\r\n" + res.Message };
                    break;
                case AppEnums.SQLExecResult.SyntaxError:
                    TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Синтаксическая ошибка!\r\n" + res.Message };
                    break;
                default:
                    TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Success, Message = "Пользователь удален!" };
                    UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID, 
                        AppEnums.Event.Удалить_пользователя, 
                        string.Format("Пользователь '{0}' удален", deletedUser.USER_LOGIN));
                    break;
            }
            return RedirectToAction("EditUsers", "Admin");
        }

        /// <summary>
        /// Авторизация зарегистрированного в Instagram приложения
        /// </summary>
        public ActionResult LoginToInstagram()
        {
            string InstagramLoginUrl = string.Format(instagramLoginUri, clientId, redirectUri, "code");
            return Redirect(InstagramLoginUrl);
        }

        public ActionResult InstagramAuth(string code)
        {
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("client_id", clientId);
            parameters.Add("client_secret", clientSecret);
            parameters.Add("grant_type", "authorization_code");
            parameters.Add("redirect_uri", redirectUri);
            parameters.Add("code", code);
            
            using (WebClient client = new WebClient())
            {
                try
                {
                    var result = client.UploadValues(instagramOAuthAccessTokenUri, "POST", parameters);
                    var response = System.Text.Encoding.Default.GetString(result);
                    InstagramAuthResponse iar = JsonConvert.DeserializeObject<InstagramAuthResponse>(response);
                    Session["Instagram"] = iar;
                }
                catch (Exception e)
                {
                    return View("Error", (object)e.Message);
                }
            }

            return RedirectToAction("SearchByTag");
        }

        /// <summary>
        /// Пометить "оплачено"
        /// </summary>
        public ActionResult SetPaid(int show_id, bool value)
        {
            ShowManager.SetPaid(show_id, value);
            if (value)
            {
                UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID, AppEnums.Event.Пометить_заказ_как_оплаченный);
            }
            else
            {
                UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID, AppEnums.Event.Пометить_заказ_как_неоплаченный);
            }
            
            return new EmptyResult();
        }

        /// <summary>
        /// Пометить "модерация"
        /// </summary>
        public ActionResult SetMod(int show_id, bool value)
        {
            ShowManager.SetMod(show_id, value);
            if (value)
            {
                UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID, AppEnums.Event.Пометить_заказ_как_модерируемый);
            }
            else
            {
                UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID, AppEnums.Event.Пометить_заказ_как_немодерируемый);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Сделать/отменить администратора
        /// </summary>
        public ActionResult SetAdmin(int id, bool value)
        {
            var user = UserManager.GetUser(id);
            if (value)
            {
                user.USER_ROLE_ID = (int)AppEnums.Role.Admin;
                UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID, 
                    AppEnums.Event.Сделать_пользователя_администратором,
                    string.Format("Пользователь '{0}' стал администратором", user.USER_LOGIN));
            }
            else
            {
                user.USER_ROLE_ID = (int)AppEnums.Role.TagModerator;
                UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID, 
                    AppEnums.Event.Забрать_права_администратора,
                    string.Format("Пользователь '{0}' потерял права администратора", user.USER_LOGIN));
            }
            UserManager.UpdateUser(user);

            return RedirectToAction("EditAdmins", "Admin");
        }

        public ActionResult EditAdmins()
        {
            var res = UserManager.GetUsers(AppEnums.Role.Admin.ToString("d")).
                Where(x => (string.Compare(x.USER_LOGIN, SuperAdmin, true) != 0)).
                Where(x => (string.Compare(x.USER_LOGIN, HttpContext.User.Identity.Name, true) != 0)).
                ToList();
            return View(res);
        }

        public ActionResult Log(int page = 1, int user_id = 0, int event_id = 0)
        {
            var users = UserManager.GetUsers().Select(x => new SelectListItem { Text = x.USER_LOGIN, Value = x.USER_ID.ToString(), Selected = x.USER_ID == user_id }).ToList<SelectListItem>();
            users.Add(new SelectListItem() { Text = "Все пользователи", Value = "0", Selected = (user_id == 0) });
            ViewBag.users = users;
            ViewData["user_id"] = user_id;

            var events = EventManager.GetEvents().Select(x => new SelectListItem() { Text = x.EVENT_NAME, Value = x.EVENT_ID.ToString(), Selected = x.EVENT_ID == event_id }).ToList<SelectListItem>();
            events.Add(new SelectListItem() { Text = "Все события", Value = "0", Selected = event_id == 0 });
            ViewBag.events = events;
            ViewData["event_id"] = event_id;

            ViewData["page"] = page;

            return View(EventManager.GetLogEvents(page, user_id, event_id));
        }
    }
}
