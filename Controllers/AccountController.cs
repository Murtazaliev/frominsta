using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstagramMVC.Models;
using InstagramMVC.Models.AccountModels;
using InstagramMVC.DataManagers;
using InstagramMVC.Globals;
using System.Web.Security;

namespace InstagramMVC.Controllers
{    
    public class AccountController : Controller
    {
        /// <summary>
        /// Роли, разрешенные для входа
        /// </summary>
        private string[] AllowedRoles;

        public AccountController()
        {
            AllowedRoles = new string[] { AppEnums.Role.Admin.ToString(), 
                                          AppEnums.Role.PremiumModerator.ToString(),
                                          AppEnums.Role.TagModerator.ToString(),
                                          AppEnums.Role.Guest.ToString()
                                       };
        }

        /// <summary>
        /// Провести аутентификацию
        /// </summary>
        private void SetAuthTicket(string login, bool rememberMe, string roles)
        {
            var authTicket = new FormsAuthenticationTicket(1,
                                                           login,
                                                           DateTime.Now,
                                                           DateTime.Now.AddMinutes(300),
                                                           rememberMe,
                                                           roles);
            //UserData - роли пользователя через ";" потом вытащить в Application_AuthenticateRequest

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
        }

        /// <summary>
        /// Выход из системы
        /// </summary>
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID, AppEnums.Event.Выход);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Вход в систему
        /// </summary>
        public ActionResult Enter()
        {
            return View();
        }

        /// <summary>
        /// Вход в систему
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Enter(EnterModel em, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = AccountManager.ValidateEnter(Convert.ToString(em.Login), FormsAuthentication.HashPasswordForStoringInConfigFile(Convert.ToString(em.Password), "SHA1"));
                if (user == null)
                {
                    UtilManager.RegisterEvent(0, AppEnums.Event.Неудачная_попытка_входа, string.Format("Попытка входа под логином '{0}' с паролем '{1}'!", em.Login, em.Password));
                    ModelState.AddModelError("Login", "Неверный логин или пароль!");
                }
                else
                {
                    string UserRoles = UserManager.GetUserRole(user.USER_ID);
                    if (AllowedRoles.Intersect(UserRoles.Split(',')).ToArray().Length > 0)
                    {
                        SetAuthTicket(user.USER_LOGIN, em.RememberMe, UserRoles);
                        Session["OPTIONS"] = UserManager.GetUserOptions(user.USER_ID);

                        UtilManager.RegisterEvent(user.USER_ID, AppEnums.Event.Вход, "Пользователь успешно вошел в систему");
                        if (Url.IsLocalUrl(ReturnUrl))
                            return Redirect(ReturnUrl);
                        else
                            return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        UtilManager.RegisterEvent(user.USER_ID, AppEnums.Event.Попытка_входа_в_ограниченную_зону);
                        return View("Error", ((object)"У Вас недостаточно прав для входа в систему!"));
                    }
                }
            }
            return View(em);
        }

        /// <summary>
        /// Регистрация
        /// </summary>
        public ActionResult SignIn()
        {
            return View();
        }

        /// <summary>
        /// Регистрация
        /// </summary>
        [HttpPost]
        public ActionResult SignIn(RegisterModel rm)
        {
            if (ModelState.IsValid)
            {
                rm.NewPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(rm.NewPassword, "SHA1");
                var user = AccountManager.RegisterNewUser(rm);
                if (user == null)
                {
                    TempData["result"] = new OperationResult() { Status = AppEnums.OperationStatus.Error, Message = "Такой пользователь или E-Mail уже зарегестрирован!" };
                }
                else
                {
                    SetAuthTicket(user.USER_LOGIN, false, UserManager.GetUserRole(user.USER_ID));
                    UtilManager.RegisterEvent(user.USER_ID, AppEnums.Event.Регистрация);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(rm);
        }

        /// <summary>
        /// Сменить пароль
        /// </summary>
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Сменить пароль
        /// </summary>
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (AccountManager.ChangeUserPassword(System.Web.HttpContext.Current.User.Identity.Name,
                                                        model.OldPassword,
                                                        model.NewPassword)
                                                        )
                {
                    UtilManager.RegisterEvent(UserManager.GetUser(System.Web.HttpContext.Current.User.Identity.Name).USER_ID, AppEnums.Event.Сменить_пароль);
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("OldPassword", "Неверный старый пароль");
                }
            }

            return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
