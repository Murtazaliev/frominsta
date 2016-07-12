using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstagramMVC.Models;
using InstagramMVC.DataManagers;
using InstagramMVC.Globals;

namespace InstagramMVC.Controllers
{
    [Authorize(Roles = "Admin,TagModerator,PremiumModerator")]
    public class ShowController : Controller
    {
        private bool IsAdmin;

        public ShowController()
        {
            IsAdmin = System.Web.HttpContext.Current.User.IsInRole("Admin");
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult User(string user_login)
        {
            string current_user = System.Web.HttpContext.Current.User.Identity.Name;
            if (string.IsNullOrEmpty(user_login))
            {
                user_login = current_user;
            }
            IList<Show> shows;
            if (string.Compare(current_user, user_login, true) != 0)
            {
                if (System.Web.HttpContext.Current.User.IsInRole("Admin"))
                {
                    shows = UserManager.GetUserShows(user_login);
                }
                else
                {
                    shows = UserManager.GetUserShows(current_user);
                }
            }
            else
            {
                shows = UserManager.GetUserShows(current_user);
            }
            return View(shows);
        }

        public ActionResult Edit(int show_id)
        {
            //проверить относится ли show_id к текущему пользователю, кроме Admin
            string current_user = System.Web.HttpContext.Current.User.Identity.Name;
            if (UserManager.GetUserShows(current_user).Any(x => x.SHOW_ID == show_id) || IsAdmin)
            { 
                var show = ShowManager.GetShow(show_id);
                return View(show);
            }

            return RedirectToAction("User", "Show", new { user_login = current_user });
        }

        [HttpPost]
        public ActionResult Edit(Show show)
        {
            if (ModelState.IsValid)
            {
                //Если не Admin, то поле ALLOWMOD и PAID брать из базы
                if (!IsAdmin)
                {
                    Show sh = ShowManager.GetShow(show.SHOW_ID);
                    if (sh != null)
                    { 
                        show.ALLOWMOD = sh.ALLOWMOD;
                        show.PAID = sh.PAID;
                    }                    
                }

                SQLReturnResult res = ShowManager.SaveShow(show);                
                switch (res.Result)
                {
                    case AppEnums.SQLExecResult.Success:
                        UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID,
                                AppEnums.Event.Сохранить_параметры_заказа,
                                string.Format("Сохранение параметров заказа № {0}", show.SHOW_ID));
                        //если заказ добавлял админ, то перенаправить в /Admin/UserShows, иначе /User/UserShows
                        if (IsAdmin)
                        {
                            AppUser user = UserManager.GetUser(show.USER_ID);                            
                            return RedirectToAction("User", "Show", new { user_login = user.USER_LOGIN });
                        }                        
                        TempData["result"] = "Операция прошла успешно!";
                        return RedirectToAction("User", "Show");                                                
                    case AppEnums.SQLExecResult.SyntaxError:
                        TempData["result"] = "При сохранении данных произошла ошибка!" + res.Message;
                        break;
                }
            }
            return View(show);
        }

        public ActionResult Add(string user_login)
        {
            AppUser user = UserManager.GetUser(user_login);
            UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID, AppEnums.Event.Добавить_заказ);
            return View("Edit", new Show() { USER_ID = user.USER_ID, SHOW_START = DateTime.Now, SHOW_END = DateTime.Now.AddDays(1)});
        }

        public ActionResult Delete(int show_id)
        {
            SQLReturnResult res = ShowManager.DeleteShow(show_id);
            switch (res.Result)
            {
                case AppEnums.SQLExecResult.Success:
                    //если заказ добавлял админ, то перенаправить в /Admin/UserShows, иначе /User/UserShows
                    UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID, AppEnums.Event.Удалить_заказ);
                    if (IsAdmin)
                    {
                        AppUser user = UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name);
                        return RedirectToAction("User", "Show", new { user_login = user.USER_LOGIN });
                    }
                    TempData["result"] = "Заказ успешно удален!";
                    break;
                case AppEnums.SQLExecResult.SyntaxError:
                    TempData["result"] = "При удалении заказа произошла ошибка!" + res.Message;
                    break;
            }

            return RedirectToAction("User", "Show");
        }

        public ActionResult Translate(string hashtag = "", string ahashtag="")
        {
            if (!string.IsNullOrEmpty(hashtag))
            {
                Session["ACTIVETAG"] = hashtag;
            }

            Session["ACTIONTAG"] = string.IsNullOrEmpty(ahashtag) ? null : ahashtag;

            if (Session["ACTIVETAG"] != null)
            {
                UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID, 
                    AppEnums.Event.Начало_трансяции,
                    string.Format("Начало трансляции по хэштегу '{0}'", Convert.ToString(Session["ACTIVETAG"])));
            }            
            return View();
        }
    }
}
