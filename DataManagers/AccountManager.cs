using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InstagramMVC.Globals;
using InstagramMVC.Models;
using InstagramMVC.Models.AccountModels;
using System;
using System.Text;
using System.Web.Security;

namespace InstagramMVC.DataManagers
{
    public class AccountManager
    {
        /// <summary>
        /// Проверка соответствия логина и пароля
        /// </summary>
        public static AppUser ValidateEnter(string user_login, string user_password)
        {
            AppUser res = null;

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT USER_ID FROM APPUSER WHERE (lower(USER_LOGIN)=@user_login OR lower(USER_EMAIL)=@user_login) AND USER_PASSWORD=@user_password", con))
                {
                    cmd.Parameters.AddWithValue("@user_login", user_login.ToLower());
                    cmd.Parameters.AddWithValue("@user_password", user_password);

                    try
                    {
                        con.Open();

                        int user_id = Convert.ToInt32(cmd.ExecuteScalar());
                        if (user_id > 0)
                        {
                            res = InstagramMVC.DataManagers.UserManager.GetUser(user_id);
                        }

                        con.Close();
                    }
                    catch { }
                }
            }

            return res;
        }

        /// <summary>
        /// Зарегистрировать нового пользователя
        /// </summary>
        public static AppUser RegisterNewUser(RegisterModel rm, int role_id = (int)AppEnums.Role.Guest)
        {
            var res = new AppUser();

            if (IsUserLoginEMailExists(rm.Login, rm.EMail))
            {
                res = null;
            }
            else
            {
                using (var con = new SqlConnection(AppConst.ConnStr))
                {
                    using (var cmd = new SqlCommand("RegisterNewUser", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@user_login", SqlDbType.VarChar, 50).Value = rm.Login.ToLower().Trim();
                        cmd.Parameters.Add("@user_email", SqlDbType.VarChar, 50).Value = rm.EMail.ToLower().Trim();
                        cmd.Parameters.Add("@user_lastname", SqlDbType.VarChar, 50).Value = rm.LastName.Trim();
                        cmd.Parameters.Add("@user_firstname", SqlDbType.VarChar, 50).Value = rm.FirstName.Trim();
                        cmd.Parameters.Add("@user_patr", SqlDbType.VarChar, 50).Value = AppUtils.ConvertToString(rm.Patr).Trim();
                        cmd.Parameters.Add("@user_phone", SqlDbType.VarChar, 500).Value = AppUtils.ConvertToString(rm.Phone).Trim();
                        cmd.Parameters.Add("@user_password", SqlDbType.VarChar, 50).Value = rm.NewPassword;
                        cmd.Parameters.Add("@role_id", SqlDbType.Int, 4).Value = role_id;

                        SqlParameter par = new SqlParameter("@user_id", SqlDbType.Int, 4);
                        par.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(par);

                        //try
                        //{
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        int user_id = Convert.ToInt32(par.Value);

                        res = UserManager.GetUser(user_id);
                        //}
                        //catch 
                        //{
                        //    res = null;
                        //}
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Зарегистрирован ли пользователь с таким логином или e-mail
        /// </summary>
        public static bool IsUserLoginEMailExists(string user_login, string user_email)
        {
            bool res = false;

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT USER_ID FROM APPUSER WHERE lower(USER_LOGIN)=@user_login OR lower(USER_EMAIL)=@user_email", con))
                {
                    cmd.Parameters.AddWithValue("@user_login", user_login.ToLower());
                    cmd.Parameters.AddWithValue("@user_email", user_login.ToLower());
                    try
                    {
                        con.Open();
                        res = (Convert.ToInt32(cmd.ExecuteScalar()) > 0);
                        con.Close();
                    }
                    catch { }
                }
            }

            return res;
        }

        /// <summary>
        /// Зарегистрирован ли пользователь с таким e-mail
        /// </summary>
        public static bool IsUserEMailExists(string user_email)
        {
            bool res = false;

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT USER_ID FROM APPUSER lower(USER_EMAIL)=@user_email", con))
                {
                    cmd.Parameters.AddWithValue("@user_email", user_email.ToLower());
                    try
                    {
                        con.Open();
                        res = (Convert.ToInt32(cmd.ExecuteScalar()) > 0);
                        con.Close();
                    }
                    catch { }
                }
            }

            return res;
        }

        public static bool ChangeUserPassword(string user_login, string oldpassword, string newpassword)
        {
            bool res = false;

            try
            {
                using (SqlConnection con = new SqlConnection(AppConst.ConnStr))
                {
                    using (SqlCommand cmd = new SqlCommand("ChangeUserPassword", con))
                    {
                        cmd.Parameters.Add("@user_login", SqlDbType.VarChar, 50).Value = user_login.Trim();
                        cmd.Parameters.Add("@oldpassword", SqlDbType.VarChar, 100).Value = FormsAuthentication.HashPasswordForStoringInConfigFile(oldpassword, "SHA1");
                        cmd.Parameters.Add("@newpassword", SqlDbType.VarChar, 100).Value = FormsAuthentication.HashPasswordForStoringInConfigFile(newpassword, "SHA1");

                        SqlParameter par = new SqlParameter("@res", SqlDbType.Bit, 1);
                        par.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(par);

                        cmd.CommandType = CommandType.StoredProcedure;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        res = (bool)par.Value;
                    }
                }
            }
            catch
            {

            }

            return res;
        }
    }
}