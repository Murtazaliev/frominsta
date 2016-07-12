using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using InstagramMVC.Models;
using InstagramMVC.Models.AccountModels;
using InstagramMVC.Models.UserModel;
using InstagramMVC.Models.InstagramModels;
using InstagramMVC.DataManagers;
using InstagramMVC.Globals;
using InstagramMVC.InstagramEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing;
using System.IO;


namespace InstagramMVC.Controllers
{
    [Authorize(Roles = "Admin,TagModerator,PremiumModerator")]
    public class UserController : Controller
    {
        private AppUser _account;
        
        public UserController()
        {
            //if (System.Web.HttpContext.Current.Session["PROFILEPANEL"] == null)
            //{
            //    UtilManager.CreateUserPanel(System.Web.HttpContext.Current);
            //}
            _account = UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name);
        }

        public ActionResult Index()
        {
            //ViewBag.TabIdx = 0;
            return View();
        }

        public ActionResult EditUser()
        {
            return View(_account);
        }

        [HttpPost]
        public ActionResult EditUser(AppUser User)
        {
            TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Некорректные данные!" };

            if (ModelState.IsValid)
            {
                SQLReturnResult res = UserManager.UpdateUser(User);
                switch (res.Result)
                {
                    case AppEnums.SQLExecResult.RollBack:
                        TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Ошибка! Откат транзакции!" };
                        break;
                    case AppEnums.SQLExecResult.SyntaxError:
                        TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Синтаксическая ошибка!" };
                        break;
                    default:
                        TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Success, Message = "Данные успешно сохранены!" };
                        UtilManager.RegisterEvent(_account.USER_ID, AppEnums.Event.Изменение_личных_регистрационных_данных);
                        break;
                }                
                //return RedirectToAction("EditProfileSuccess");
            }
            return View(User);
        }

        /// <summary>
        /// Сменить пароль
        /// </summary>
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Сменить пароль
        /// </summary>
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (AccountManager.ChangeUserPassword(_account.USER_LOGIN,
                                                        model.OldPassword,
                                                        model.NewPassword)
                                                        )
                {
                    TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Success, Message = "Пароль успешно изменен!" };
                    UtilManager.RegisterEvent(_account.USER_ID, AppEnums.Event.Сменить_пароль);
                }
                else
                {
                    TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Неверный старый пароль!" };
                    ModelState.AddModelError("OldPassword", "Неверный старый пароль!");
                }
            }

            return View();
        }

        /// <summary>
        /// Отобразить пользовательские хэштеги
        /// </summary>
        /// <returns></returns>
        public ActionResult Tags()
        {
            return View(HashTagManager.GetUserTags(_account.USER_ID));
        }

        /// <summary>
        /// Добавить пользовательский хэштег
        /// </summary>
        public ActionResult AddTag(string hashtag = "")
        {
            if (!string.IsNullOrEmpty(hashtag))
            {
                hashtag = hashtag.Trim().ToLower();
                if (HashTagManager.IsTagExist(hashtag))
                {
                    TempData["result"] = new OperationResult()
                    {
                        Status = AppEnums.OperationStatus.Warning,
                        Message = "Такой хэштег уже существует!"
                    };
                    return RedirectToAction("Tags");
                }

                SQLReturnResult res = HashTagManager.InsertHashTag(_account.USER_ID, hashtag);
                switch (res.Result)
                {
                    case AppEnums.SQLExecResult.RollBack:
                        TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Ошибка! Откат транзакции!" };
                        break;
                    case AppEnums.SQLExecResult.SyntaxError:
                        TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Ошибка! Синтаксическая ошибка!" };
                        break;
                    default:
                        TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Success, Message = "Новый тег добавлен!" };
                        UtilManager.RegisterEvent(_account.USER_ID, AppEnums.Event.Добавить_хэштег, string.Format("Добавлен хэштег '{0}'", hashtag));
                        break;
                }
            }
            return RedirectToAction("Tags");
        }

        /// <summary>
        /// Удалить пользовательский хэштег
        /// </summary>
        public ActionResult DelTag(string hashtag = "")
        {
            if (!string.IsNullOrEmpty(hashtag))
            {
                hashtag = hashtag.Trim().ToLower();
                var res = HashTagManager.DeleteTag(hashtag);
                switch (res.Result)
                {
                    case AppEnums.SQLExecResult.RollBack:
                        TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Ошибка! Откат транзакции!" };
                        break;
                    case AppEnums.SQLExecResult.SyntaxError:
                        TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Ошибка! Синтаксическая ошибка!" };
                        break;
                    default:
                        TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Success, Message = string.Format("Тег '#{0}' удален!", hashtag) };
                        UtilManager.RegisterEvent(_account.USER_ID, AppEnums.Event.Удалить_хэштег, string.Format("Хэштег '{0}' удален", hashtag));
                        break;
                }

                if (string.Compare((string)Session["ACTIVETAG"], hashtag, true) == 0)
                {
                    Session["ACTIVETAG"] = null;
                }
            }

            return RedirectToAction("Tags");
        }

        /// <summary>
        /// Активировать пользовательский хэштег
        /// </summary>
        public ActionResult SelTag(string hashtag = "")
        {
            if (!string.IsNullOrEmpty(hashtag))
            {
                Session["ACTIVETAG"] = hashtag.Trim().ToLower();
            }

            Session["NEXT_MEDIA_ID"] = null;            
            //TODO заполнить табл MEDIATAG
            return RedirectToAction("Tags");
        }

        /// <summary>
        /// Модерирование изображений по хэштегу
        /// </summary>
        public ActionResult Mod(string hashtag = "", int page = 1)
        {
            if (string.IsNullOrEmpty(hashtag))
            {
                hashtag = Convert.ToString(Session["ACTIVETAG"]);
            }
            
            IList<InstagramMVC.Models.UserModel.HashTag> usertags = HashTagManager.GetUserTags(_account.USER_ID);
            if (usertags != null && usertags.Count > 0)
            {
                IEnumerable<SelectListItem> tags = usertags.Select(x => new SelectListItem
                {
                    Text = x.TAG_CAPTION,
                    Value = x.TAG_CAPTION,
                    Selected = x.TAG_CAPTION.Equals(hashtag)
                });

                if (string.IsNullOrEmpty(hashtag))
                {
                    hashtag = tags.First().Text;
                }

                ViewBag.tags = tags;
            }

            //новые фото будут отображаться в начале из-за OrderByDescending
            ViewData["page"] = page;
            ViewData["hashtag"] = hashtag;
            var res = HashTagManager.GetDataBaseMediaTags(_account.USER_ID, hashtag, page, AppEnums.SelectMediaAttr.HideDeleted).OrderByDescending(x => x.MEDIA_ID).ToList();            
            
            return View(res);
        }

        public ActionResult UpdateMediaTags(string hashtag = "", int updtype = (int)AppEnums.UpdateHashTagType.Recent30)
        {
            //todo в зав-ти от updtype делать запрос SaveMediaTagsToDataBase
            if (string.IsNullOrEmpty(hashtag))
            {
                hashtag = (string)Session["ACTIVETAG"];
            }

            //TODO async
            HashTagManager.SaveMediaTagsToDataBase(_account.USER_ID, hashtag, (AppEnums.UpdateHashTagType)updtype);
            UtilManager.RegisterEvent(_account.USER_ID, AppEnums.Event.Запуск_обновления_медиатегов, string.Format("Обновление медиатега '{0}'", hashtag));
            return RedirectToAction("Mod", "User", new {hashtag = hashtag });
        }

        public ActionResult ClearMediaTags(string hashtag)
        {
            HashTagManager.ClearMediaTags(_account.USER_ID, hashtag);
            UtilManager.RegisterEvent(_account.USER_ID, AppEnums.Event.Очистить_медиатеги_в_БД, string.Format("Очистка медиатега '{0}'", hashtag));
            return RedirectToAction("Mod", "User", new { hashtag = hashtag });
        }

        public ActionResult SetBan(int media_id, bool value)
        {
            HashTagManager.SetBan(media_id, value);
            return new EmptyResult();
        }

        public ActionResult DelMedia(int media_id)
        {
            HashTagManager.DeleteMedia(media_id);
            return new EmptyResult();
        }

        public ActionResult GetNextBatchTags(string tag)
        {
            //TODO делаешь unban - виснет может из-за Session["NEXT_MEDIA_ID"]
            if (!UserManager.CanUserTranslateShow(_account.USER_ID))
            {
                List<MediaTag> res = new List<MediaTag>() { new MediaTag { INSTAGRAM_MEDIA_ID = "0",
                                                                           INSTAGRAM_USER_ID = "0",
                                                                           INSTAGRAM_USER_NAME = UtilManager.GetVarValue("app.caption"),
                                                                           INSTAGRAM_MEDIA_STANDARD_RES_URL = Url.Content("~/Content/img/trans_show_end.png"),
                                                                           INSTAGRAM_USER_PROFILEPICTURE = Url.Content("~/Content/img/logo.png"),
                                                                           INSTAGRAM_CAPTION = "Трансляция шоу завершена!!!"
                                                                         }   
                                                           };
                return Json(res, JsonRequestBehavior.AllowGet);
            }

            UserOptions opts;
            if (Session["OPTIONS"] == null)
            {
                opts = UserManager.GetUserOptions(_account.USER_ID);
                Session["OPTIONS"] = opts;                
            }
            else
            {
                opts = (UserOptions)Session["OPTIONS"];
            }
            //auction
            int next_media_id = (Session["NEXT_MEDIA_ID"] == null ? 0 : Convert.ToInt32(Session["NEXT_MEDIA_ID"]));

            var batch = HashTagManager.GetNextBatchDataBaseMediaTags(_account.USER_ID, tag, next_media_id, opts.USER_SLIDE_BATCH_SIZE);
            
            if (batch.Count == 0)
            {
                if (next_media_id == 0) //значит в базе нет нужных тегов
                {
                    return new EmptyResult();
                }
                else
                {
                    //запрос с начала базы
                    next_media_id = 0;
                    batch = HashTagManager.GetNextBatchDataBaseMediaTags(_account.USER_ID, tag, next_media_id, opts.USER_SLIDE_BATCH_SIZE);
                }
            }

            if (batch.Count > 0)
            {
                if (opts.USER_SLIDE_BATCH_SIZE > batch.Count)
                {
                    next_media_id = 0;
                    batch.AddRange(HashTagManager.GetNextBatchDataBaseMediaTags(_account.USER_ID, tag, next_media_id, opts.USER_SLIDE_BATCH_SIZE - batch.Count));
                }

                batch = batch.GroupBy(x => x.MEDIA_ID, (key, group) => group.FirstOrDefault()).ToList();
                Session["NEXT_MEDIA_ID"] = batch.Last().MEDIA_ID;

                //для того, чтобы работала BS Carousel нужно мин. 5 элементов в писке
                int batch_size = batch.Count;
                var rnd = new Random((int)DateTime.Now.Ticks);
                while (batch.Count < 5)
                {
                    batch.Add(new MediaTag()
                    {
                        MEDIA_ID = rnd.Next(1, 10000),
                        INSTAGRAM_MEDIA_ID = batch[batch.Count - batch_size].INSTAGRAM_MEDIA_ID,
                        INSTAGRAM_USER_ID =  batch[batch.Count - batch_size].INSTAGRAM_USER_ID,
                        INSTAGRAM_USER_NAME = batch[batch.Count - batch_size].INSTAGRAM_USER_NAME,
                        INSTAGRAM_MEDIA_STANDARD_RES_URL = batch[batch.Count - batch_size].INSTAGRAM_MEDIA_STANDARD_RES_URL,
                        INSTAGRAM_USER_PROFILEPICTURE = batch[batch.Count - batch_size].INSTAGRAM_USER_PROFILEPICTURE,
                        INSTAGRAM_CAPTION = batch[batch.Count - batch_size].INSTAGRAM_CAPTION
                    });
                    batch_size--;
                    if (batch_size == 0)
                    {
                        batch_size = batch.Count;
                    }
                }
                //@Url.Action("Translate", "Show")
                
                if (Session["ACTIONTAG"] != null)
                {
                    string actionTag = (string)Session["ACTIONTAG"];
                    int action_next_media_id = Session["ACTION_NEXT_MEDIA_ID"] == null ? 0 : (int)Session["ACTION_NEXT_MEDIA_ID"];
                    List<MediaTag> actionMediaTag = HashTagManager.GetNextBatchDataBaseMediaTags(_account.USER_ID, actionTag, action_next_media_id, 1);
                    if (actionMediaTag == null || actionMediaTag.Count == 0)
                    {
                        if (action_next_media_id > 0)
                        {
                            //значит достигнут конец списка аукционного тега
                            action_next_media_id = 0;
                            actionMediaTag = HashTagManager.GetNextBatchDataBaseMediaTags(_account.USER_ID, actionTag, action_next_media_id, 1);
                        }
                        else
                        {
                            //значит список пуст
                        }
                    }

                    if (actionMediaTag.Count > 0)
                    {
                        action_next_media_id = actionMediaTag[0].MEDIA_ID;
                        Session["ACTION_NEXT_MEDIA_ID"] = action_next_media_id;
                        batch.Add(actionMediaTag[0]);
                    }                    
                }

                return Json(batch, JsonRequestBehavior.AllowGet);//.GroupBy(x => x.MEDIA_ID, (key, group) => group.FirstOrDefault()).ToList()
            }

            //var total = HashTagManager.GetNextBatchDataBaseMediaTags(_account.USER_ID, tag, next_media_id, opts.USER_SLIDE_BATCH_SIZE);
            //if (total.Count > 0)
            //{
            //    List<MediaTag> res = total.Where(x => x.MEDIA_ID > next_media_id).Take(opts.USER_SLIDE_BATCH_SIZE).ToList<MediaTag>();
            //    if (res.Count() < opts.USER_SLIDE_BATCH_SIZE)
            //    {
            //        //дошли до конца
            //        //res = total.Skip(Math.Max(0, total.Count - opts.USER_SLIDE_BATCH_SIZE)).ToList<MediaTag>();
            //        res.AddRange(total.Take(opts.USER_SLIDE_BATCH_SIZE - res.Count()));
            //    }

            //    Session["NEXT_MEDIA_ID"] = res.Last().MEDIA_ID;

            //    return Json(res, JsonRequestBehavior.AllowGet);
            //}

            return new EmptyResult();
        }

        public ActionResult Options()
        {
            var opts = UserManager.GetUserOptions(_account.USER_ID);
            return View(opts);
        }

        [HttpPost]
        public ActionResult Options(UserOptions opts, HttpPostedFileBase bg_img, HttpPostedFileBase lg_img)
        {
            if (bg_img != null)
            {
                string ext = Path.GetExtension(bg_img.FileName);
                if (AppConst.AllowedImageFileFormats.Contains(ext))
                {
                    string bg_img_url = string.Format("~/Content/img/{0}_bg_img{1}", _account.USER_LOGIN, ext);
                    string SaveToFile = Server.MapPath(string.Format("~/Content/img/{0}_bg_img{1}", _account.USER_LOGIN, ext));
                    System.IO.File.Delete(SaveToFile);
                    bg_img.SaveAs(SaveToFile);
                    opts.USER_BACKGROUND_IMG_URL = bg_img_url;                    
                }
                else
                {
                    TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Данный формат файла фонового рисунка не поддерживается!" };
                    return View(opts);
                }
            }

            if (lg_img != null)
            {
                string ext = Path.GetExtension(lg_img.FileName);
                if (AppConst.AllowedImageFileFormats.Contains(ext))
                {
                    string lg_img_url = string.Format("~/Content/img/{0}_lg_img{1}", _account.USER_LOGIN, ext);
                    string SaveToFile = Server.MapPath(string.Format("~/Content/img/{0}_lg_img{1}", _account.USER_LOGIN, ext));
                    System.IO.File.Delete(SaveToFile);
                    lg_img.SaveAs(SaveToFile);
                    opts.USER_LOGO_IMG_URL = lg_img_url;
                }
                else
                {
                    TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Данный формат файла логотипа не поддерживается!" };
                    return View(opts);
                }
            }

            var res = UserManager.SaveUserOptions(opts);
            switch (res.Result)
            {
                case AppEnums.SQLExecResult.RollBack:
                    TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Ошибка! Откат транзакции!" };
                    break;
                case AppEnums.SQLExecResult.SyntaxError:
                    TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Ошибка! Синтаксическая ошибка!" };
                    break;
                default:
                    TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Success, Message = "Настройки успешно сохранены!" };
                    UtilManager.RegisterEvent(_account.USER_ID, AppEnums.Event.Сохранить_параметры_трансляции);
                    break;
            }

            return View(UserManager.GetUserOptions(_account.USER_ID));
        }

        /// <summary>
        /// Для отображения модераторской страницы с телефона по ссылке Telegram Bot'а
        /// </summary>
        [Authorize(Roles = "Admin,PremiumModerator")]
        public ActionResult BotMod(string user_login, string hashtag, int page = 1)
        {
            var res = HashTagManager.GetDataBaseMediaTags(user_login, hashtag, page, AppEnums.SelectMediaAttr.HideDeleted).OrderByDescending(x => x.MEDIA_ID).ToList();
            ViewData["page"] = page;
            ViewData["hashtag"] = hashtag;
            return View(res);
        }
    }
}
