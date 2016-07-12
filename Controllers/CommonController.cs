using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstagramMVC.DataManagers;
using InstagramMVC.Globals;
using InstagramMVC.Models;

namespace InstagramMVC.Controllers
{
    public class CommonController : Controller
    {
        public ActionResult GetSessionValue(string key)
        {
            string value = (Session[key] == null ? "" : Session[key].ToString());
            return Json(new { value = value}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Получить информацию о пользователе и хэштеге по запросу с Telegram
        /// </summary>
        public JsonResult GetTeleUser(string user_login, string tag_caption)
        {
            TeleUser user = UserManager.GetTeleUser(user_login, tag_caption);
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Запустить обновление медиатегов пользователя по команде с Telegram
        /// </summary>
        /// <returns>Кол-во добавленных тегов</returns>
        public JsonResult UpdateTeleUserMediatags(string user_login, string tag_caption)
        {
            var user = UserManager.GetUser(user_login);
            int addedMeditagCount = HashTagManager.SaveMediaTagsToDataBase(user.USER_ID, tag_caption);
            UtilManager.RegisterEvent(user.USER_ID, AppEnums.Event.Запуск_обновления_медиатегов, string.Format("Обновление медиатега '{0}'", tag_caption));
            return Json(new { MeditagCount = addedMeditagCount }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Получить список зарегистрированных пользователей
        /// </summary>
        public JsonResult GetWebUsers()
        {
            var users = UserManager.GetUsers(AppEnums.Role.PremiumModerator.ToString("d") + "," + AppEnums.Role.TagModerator.ToString("d"))
                .Select(x => new InstagramMVC.Models.TeleUser() 
                { 
                    USER_ID = x.USER_ID,
                    USER_LOGIN = x.USER_LOGIN,
                    USER_EMAIL = x.USER_EMAIL,
                    USER_FIRSTNAME = x.USER_FIRSTNAME,
                    USER_LASTNAME = x.USER_LASTNAME,
                    USER_PATR = x.USER_PATR,
                    USER_PHONE = x.USER_PHONE,
                    TagCaption = ""
                });
            return Json(users, JsonRequestBehavior.AllowGet);
        }

    }
}
