using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InstagramMVC.Globals;
using InstagramMVC.Models;
using InstagramMVC.Models.AccountModels;
using InstagramMVC.Models.UserModel;
using System;
using System.Text;

namespace InstagramMVC.DataManagers
{
    public class UserManager
    {
        /// <summary>
        /// Выбрать пользователей
        /// </summary>
        /// <param name="Roles">Роли пользователей через запятую</param>
        public static IList<AppUser> GetUsers(string Roles = "")
        {
            IList<AppUser> res = new List<AppUser>();
            string SQLStr = "SELECT * FROM APPUSER ORDER BY USER_LOGIN";
            if (!string.IsNullOrEmpty(Roles))
            {
                SQLStr = string.Format(@"SELECT APPUSER.*
FROM APPUSER WHERE USER_ROLE_ID IN ({0}) ORDER BY USER_LOGIN", Roles);
            }
            
            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(SQLStr, con))
                {
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            res.Add(GetUser(Convert.ToInt32(rdr["USER_ID"])));
                        }
                    }
                    rdr.Close();
                    con.Close();
                }
            }

            return res;
        }

        /// <summary>
        /// Роли пользователя
        /// </summary>
        public static string GetUserRole(int user_id)
        {
            string res = string.Empty;

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT ROLE.ROLE_NAME FROM APPUSER INNER JOIN ROLE ON APPUSER.USER_ROLE_ID = ROLE.ROLE_ID WHERE APPUSER.USER_ID=@user_id", con))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);

                    try
                    {
                        con.Open();
                        res = AppUtils.ConvertToString(cmd.ExecuteScalar());
                        con.Close();
                    }
                    catch { }
                }
            }

            return res;
        }

        /// <summary>
        /// Роли пользователя
        /// </summary>
        public static string GetUserRole(string user_login)
        {
            string res = string.Empty;

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT  ROLE.ROLE_NAME FROM APPUSER INNER JOIN ROLE ON APPUSER.USER_ROLE_ID = ROLE.ROLE_ID WHERE APPUSER.USER_ID=" +
                                                "(SELECT USER_ID FROM APPUSER WHERE LOWER(USER_LOGIN)=@user_login)", con))
                {
                    cmd.Parameters.AddWithValue("@user_login", user_login.ToLower());

                    try
                    {
                        con.Open();
                        res = AppUtils.ConvertToString(cmd.ExecuteScalar());
                        con.Close();
                    }
                    catch { }
                }
            }

            return res;
        }

        public static AppUser GetUser(int user_id)
        {
            AppUser res = null;

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT * FROM APPUSER WHERE USER_ID=@user_id", con))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);

                    try
                    {
                        con.Open();

                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            res = new AppUser()
                            {
                                USER_ID = user_id,
                                USER_LASTNAME = Convert.ToString(rdr["USER_LASTNAME"]),
                                USER_FIRSTNAME = Convert.ToString(rdr["USER_FIRSTNAME"]),
                                USER_PATR = Convert.ToString(rdr["USER_PATR"]),
                                USER_EMAIL = Convert.ToString(rdr["USER_EMAIL"]),
                                USER_LOGIN = Convert.ToString(rdr["USER_LOGIN"]),
                                USER_PHONE = Convert.ToString(rdr["USER_PHONE"]),
                                USER_ROLE_ID = Convert.ToInt32(rdr["USER_ROLE_ID"]),
                                USER_MAX_TAG_COUNT = Convert.ToInt32(rdr["USER_MAX_TAG_COUNT"])
                            };
                        }
                        rdr.Close();
                        con.Close();
                    }
                    catch { }
                }
            }

            return res;
        }

        public static AppUser GetUser(string user_login)
        {
            AppUser res = null;

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT * FROM APPUSER WHERE LOWER(USER_LOGIN)=@user_login", con))
                {
                    cmd.Parameters.AddWithValue("@user_login", user_login.ToLower());

                    try
                    {
                        con.Open();

                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            res = new AppUser()
                            {
                                USER_ID = Convert.ToInt32(rdr["USER_ID"]),
                                USER_LASTNAME = Convert.ToString(rdr["USER_LASTNAME"]),
                                USER_FIRSTNAME = Convert.ToString(rdr["USER_FIRSTNAME"]),
                                USER_PATR = Convert.ToString(rdr["USER_PATR"]),
                                USER_EMAIL = Convert.ToString(rdr["USER_EMAIL"]),
                                USER_LOGIN = Convert.ToString(rdr["USER_LOGIN"]),
                                USER_PHONE = Convert.ToString(rdr["USER_PHONE"]),
                                USER_ROLE_ID = Convert.ToInt32(rdr["USER_ROLE_ID"]),
                                USER_MAX_TAG_COUNT = Convert.ToInt32(rdr["USER_MAX_TAG_COUNT"])
                            };
                        }
                        rdr.Close();
                        con.Close();
                    }
                    catch { }
                }
            }

            return res;
        }

        public static TeleUser GetTeleUser(string user_login, string tag_caption)
        {
            TeleUser res = null;
            //если у пользователя нет такого тега, то вернуть null
            var RegisteredTags = GetUserTags(user_login);
            if (!RegisteredTags.Any(x => x.TAG_CAPTION.Equals(tag_caption, StringComparison.InvariantCultureIgnoreCase)))
            {
                return res;
            }

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT * FROM APPUSER WHERE LOWER(USER_LOGIN)=@user_login", con))
                {
                    cmd.Parameters.AddWithValue("@user_login", user_login.ToLower());

                    try
                    {
                        con.Open();

                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            res = new TeleUser()
                            {
                                USER_ID = Convert.ToInt32(rdr["USER_ID"]),
                                TagCaption = tag_caption,
                                USER_LASTNAME = Convert.ToString(rdr["USER_LASTNAME"]),
                                USER_FIRSTNAME = Convert.ToString(rdr["USER_FIRSTNAME"]),
                                USER_PATR = Convert.ToString(rdr["USER_PATR"]),
                                USER_EMAIL = Convert.ToString(rdr["USER_EMAIL"]),
                                USER_LOGIN = Convert.ToString(rdr["USER_LOGIN"]),
                                USER_PHONE = Convert.ToString(rdr["USER_PHONE"])
                            };
                        }
                        rdr.Close();
                        con.Close();
                    }
                    catch { }
                }
            }

            return res;
        }

        public static int GetTeleUserMediatagCount(string user_login, string tag_caption)
        {
            return HashTagManager.GetDataBaseMediaTags(user_login, tag_caption).Count();
        }
        
        /// <summary>
        /// Удалить пользователя
        /// </summary>
        public static SQLReturnResult DeleteUser(int user_id)
        {
            SQLReturnResult res = new SQLReturnResult();

            StringBuilder sql = new StringBuilder();
            sql.AppendLine(AppConst.SQLBeginTran);
            sql.AppendLine("DELETE FROM APPUSER WHERE USER_ID=@user_id");
            sql.AppendLine(AppConst.SQLCommitTran);

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(sql.ToString(), con))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);                    

                    con.Open();

                    try
                    {
                        SqlDataReader rdr = cmd.ExecuteReader();
                        rdr.Read();
                        res.Result = (AppEnums.SQLExecResult)Convert.ToInt32(rdr["RES"]);
                        rdr.Close();
                    }
                    catch (Exception ex)
                    {
                        res.Result = AppEnums.SQLExecResult.SyntaxError;
                        res.Message = ex.Message;
                    }

                    con.Close();
                }
            }

            return res;
        }

        /// <summary>
        /// Обновить информацию о пользователе
        /// </summary>
        public static SQLReturnResult UpdateUser(AppUser user)
        {
            int role_id = AppUtils.ConvertToInteger(user.USER_ROLE_ID, 0);
            int max_tag_count = AppUtils.ConvertToInteger(user.USER_MAX_TAG_COUNT, 0);
            SQLReturnResult res = new SQLReturnResult();

            StringBuilder sql = new StringBuilder();
            sql.AppendLine(AppConst.SQLBeginTran);
            sql.AppendLine("UPDATE APPUSER");
            sql.AppendLine("SET USER_LASTNAME=@lname,");
            sql.AppendLine("    USER_FIRSTNAME=@fname,");
            sql.AppendLine("    USER_PATR=@patr,");
            sql.AppendLine("    USER_EMAIL=@email,");
            if (role_id >0)
            {
                sql.AppendLine("    USER_ROLE_ID=@role_id,");
            }
            if (max_tag_count > 0)
            {
                sql.AppendLine("    USER_MAX_TAG_COUNT=@max_tag_count,");
            }
            sql.AppendLine("    USER_PHONE=@phone");            
            sql.AppendLine("WHERE LOWER(USER_LOGIN)=@user_login");
            sql.AppendLine(AppConst.SQLCommitTran);

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(sql.ToString(), con))
                {
                    cmd.Parameters.AddWithValue("@user_login", user.USER_LOGIN.ToLower());
                    cmd.Parameters.AddWithValue("@lname", user.USER_LASTNAME);
                    cmd.Parameters.AddWithValue("@fname", user.USER_FIRSTNAME);
                    cmd.Parameters.AddWithValue("@patr", AppUtils.ConvertToString(user.USER_PATR));
                    cmd.Parameters.AddWithValue("@email", Convert.ToString(user.USER_EMAIL));
                    cmd.Parameters.AddWithValue("@phone", AppUtils.ConvertToString(user.USER_PHONE));
                    if (role_id > 0)
                    {
                        cmd.Parameters.AddWithValue("@role_id", role_id);
                    }
                    if (max_tag_count > 0)
                    {
                        cmd.Parameters.AddWithValue("@max_tag_count", max_tag_count);
                    }

                    con.Open();

                    try
                    {
                        SqlDataReader rdr = cmd.ExecuteReader();
                        rdr.Read();
                        res.Result = (AppEnums.SQLExecResult)Convert.ToInt32(rdr["RES"]);
                        rdr.Close();
                    }
                    catch (Exception ex)
                    {
                        res.Result = AppEnums.SQLExecResult.SyntaxError;
                        res.Message = ex.Message;
                    }

                    con.Close();
                }
            }

            return res;
        }

        /// <summary>
        /// Информация о заказе
        /// </summary>
        public static Show GetOrder(int show_id)
        {
            Show res = null;

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT * FROM SHOW WHERE SHOW_ID=@show_id", con))
                {
                    cmd.Parameters.AddWithValue("@show_id", show_id);

                    con.Open();                    
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        rdr.Read();

                        res.SHOW_ID = Convert.ToInt32(rdr["SHOW_ID"]);
                        res.USER_ID = Convert.ToInt32(rdr["USER_ID"]);
                        res.SHOW_START = Convert.ToDateTime(rdr["SHOW_START"]);
                        res.SHOW_END = Convert.ToDateTime(rdr["SHOW_END"]);
                        res.PAID = Convert.ToBoolean(rdr["PAID"]);
                        res.ALLOWMOD = Convert.ToBoolean(rdr["ALLOWMOD"]);
                    }
                    rdr.Close();
                    con.Close();
                }
            }

            return res;
        }

        /// <summary>
        /// Заказы пользователя
        /// </summary>
        /// <param name="user_id">ID пользователя</param>
        public static IList<Show> GetUserShows(int user_id)
        {
            var res = new List<Show>();

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT * FROM SHOW WHERE USER_ID=@user_id", con))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            res.Add(new Show()
                            {
                                SHOW_ID = Convert.ToInt32(rdr["SHOW_ID"]),
                                USER_ID = Convert.ToInt32(rdr["USER_ID"]),
                                SHOW_START = Convert.ToDateTime(rdr["SHOW_START"]),
                                SHOW_END = Convert.ToDateTime(rdr["SHOW_END"]),
                                PAID = Convert.ToBoolean(rdr["PAID"]),
                                ALLOWMOD = Convert.ToBoolean(rdr["ALLOWMOD"])
                            });
                        }
                    }
                    rdr.Close();
                    con.Close();
                }
            }

            return res;
        }

        /// <summary>
        /// Заказы пользователя
        /// </summary>
        /// <param name="user_login">Логин пользователя</param>
        public static IList<Show> GetUserShows(string user_login)
        {
            var res = new List<Show>();

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT * FROM SHOW WHERE USER_ID=(SELECT USER_ID FROM APPUSER WHERE LOWER(USER_LOGIN)=@user_login)", con))
                {
                    cmd.Parameters.AddWithValue("@user_login", user_login);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            res.Add(new Show()
                            {
                                SHOW_ID = Convert.ToInt32(rdr["SHOW_ID"]),
                                USER_ID = Convert.ToInt32(rdr["USER_ID"]),
                                SHOW_START = Convert.ToDateTime(rdr["SHOW_START"]),
                                SHOW_END = Convert.ToDateTime(rdr["SHOW_END"]),
                                PAID = Convert.ToBoolean(rdr["PAID"]),
                                ALLOWMOD = Convert.ToBoolean(rdr["ALLOWMOD"])
                            });
                        }
                    }
                    rdr.Close();
                    con.Close();
                }
            }

            return res;
        }

        /// <summary>
        /// Заказы пользователя
        /// </summary>
        public static IList<Show> GetUserOrders(AppUser user)
        {
            return GetUserShows(user.USER_ID);
        }

        public static IList<HashTag> GetUserTags(int user_id)
        {
            var res = new List<HashTag>();

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT * FROM HASHTAG WHERE [USER_ID]=@user_id", con))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            res.Add(new HashTag() {
                                                    TAG_ID = Convert.ToInt32(rdr["TAG_ID"]),
                                                    USER_ID = user_id,
                                                    TAG_CAPTION = rdr["TAG_CAPTION"].ToString()
                                                  });
                        }
                    }

                    con.Close();
                }
            }

            return res;
        }

        public static IList<HashTag> GetUserTags(string user_login)
        {            
            var res = new List<HashTag>();
            var user = GetUser(user_login);
            if (user == null)
            {
                return res;
            }

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT * FROM HASHTAG WHERE [USER_ID]=@user_id", con))
                {
                    cmd.Parameters.AddWithValue("@user_id", user.USER_ID);
                    try
                    {
                        con.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                res.Add(new HashTag()
                                {
                                    TAG_ID = Convert.ToInt32(rdr["TAG_ID"]),
                                    USER_ID = user.USER_ID,
                                    TAG_CAPTION = rdr["TAG_CAPTION"].ToString()
                                });
                            }
                        }
                    }
                    catch { }
                    finally
                    { 
                        con.Close();
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Может ли пользователь транслировать шоу
        /// </summary>
        public static bool CanUserTranslateShow(int user_id)
        {
            bool res = false;

            //должно быть выполнено:
            //1. Какой-либо из заказов соответствует текущему времени
            //2. Этот заказ оплачен (PAID=1)

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("CanUserTranslateShow", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@user_id", SqlDbType.Int, 4).Value = user_id;

                    var par = new SqlParameter("@res", SqlDbType.Bit, 1);
                    par.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(par);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    res = Convert.ToBoolean(par.Value);
                }
            }

            return res;
        }

        public static bool CanUserTranslateShow(string user_login)
        {
            bool res = false;

            AppUser user = GetUser(user_login);
            if (user != null)
            {
                res = CanUserTranslateShow(user.USER_ID);
            }

            return res;
        }

        /// <summary>
        /// Может ли пользователь модерировать шоу
        /// </summary>
        public static bool CanUserModerateShow(int user_id)
        {
            bool res = false;

            //должно быть выполнено:
            //1. Какой-либо из заказов соответствует текущему времени
            //2. Заказ (PAID=1) и (ALLOWMOD=1)

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("CanUserModerateShow", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@user_id", SqlDbType.Int, 4).Value = user_id;

                    var par = new SqlParameter("@res", SqlDbType.Bit, 1);
                    par.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(par);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    res = Convert.ToBoolean(par.Value);
                }
            }

            return res;
        }

        public static bool CanUserModerateShow(string user_login)
        {
            bool res = false;

            AppUser user = GetUser(user_login);
            if (user != null)
            {
                res = CanUserModerateShow(user.USER_ID);
            }

            return res;
        }

        /// <summary>
        /// Получить настройки пользователя
        /// </summary>
        public static UserOptions GetUserOptions(int user_id)
        {
            UserOptions res = null;

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT * FROM APPUSER WHERE USER_ID=@user_id", con))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);

                    try
                    {
                        con.Open();

                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            res = new UserOptions()
                            {
                                USER_ID = user_id,
                                USER_MAX_TAG_COUNT = Convert.ToInt32(rdr["USER_MAX_TAG_COUNT"]),
                                USER_SLIDE_ROTATION = Convert.ToInt32(rdr["USER_SLIDE_ROTATION"]),
                                USER_SLIDE_BATCH_SIZE = Convert.ToInt32(rdr["USER_SLIDE_BATCH_SIZE"]),
                                USER_BACKGROUND_IMG_URL = Convert.ToString(rdr["USER_BACKGROUND_IMG_URL"]),
                                USER_LOGO_IMG_URL = Convert.ToString(rdr["USER_LOGO_IMG_URL"])
                            };
                        }
                        rdr.Close();
                        con.Close();
                    }
                    catch { }
                }
            }

            return res;
        }

        /// <summary>
        /// Получить настройки пользователя
        /// </summary>
        /// <param name="user_login"></param>
        /// <returns></returns>
        public static UserOptions GetUserOptions(string user_login)
        {
            UserOptions res = null;

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT * FROM APPUSER WHERE USER_ID=(SELECT USER_ID FROM APPUSER WHERE LOWER(USER_LOGIN)=@user_login)", con))
                {
                    cmd.Parameters.AddWithValue("@user_login", user_login);

                    try
                    {
                        con.Open();

                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            res = new UserOptions()
                            {
                                USER_ID = Convert.ToInt32(rdr["USER_ID"]),
                                USER_MAX_TAG_COUNT = Convert.ToInt32(rdr["USER_MAX_TAG_COUNT"]),
                                USER_SLIDE_ROTATION = Convert.ToInt32(rdr["USER_SLIDE_ROTATION"]),
                                USER_SLIDE_BATCH_SIZE = Convert.ToInt32(rdr["USER_SLIDE_BATCH_SIZE"]),
                                USER_BACKGROUND_IMG_URL = Convert.ToString(rdr["USER_BACKGROUND_IMG_URL"]),
                                USER_LOGO_IMG_URL = Convert.ToString(rdr["USER_LOGO_IMG_URL"])
                            };
                        }
                        rdr.Close();
                        con.Close();
                    }
                    catch { }
                }
            }

            return res;
        }

        public static SQLReturnResult SaveUserOptions(UserOptions opts)
        {
            SQLReturnResult res = new SQLReturnResult();

            StringBuilder sql = new StringBuilder();
            sql.AppendLine(AppConst.SQLBeginTran);
            sql.AppendLine("UPDATE APPUSER");
            sql.AppendLine("SET USER_SLIDE_ROTATION=@usr,");
            if (!string.IsNullOrEmpty(opts.USER_BACKGROUND_IMG_URL))
            {
                sql.AppendLine("    USER_BACKGROUND_IMG_URL=@ubiu,");
            }
            if (!string.IsNullOrEmpty(opts.USER_LOGO_IMG_URL))
            {
                sql.AppendLine("    USER_LOGO_IMG_URL=@uliu,");
            }   
            sql.AppendLine("    USER_SLIDE_BATCH_SIZE=@usbs");
            sql.AppendLine("WHERE [USER_ID]=@user_id");
            sql.AppendLine(AppConst.SQLCommitTran);

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(sql.ToString(), con))
                {
                    cmd.Parameters.AddWithValue("@user_id", opts.USER_ID);
                    if (opts.USER_SLIDE_ROTATION != null)
                    { 
                        cmd.Parameters.AddWithValue("@usr", opts.USER_SLIDE_ROTATION);
                    }
                    if (opts.USER_SLIDE_BATCH_SIZE != null)
                    {
                        cmd.Parameters.AddWithValue("@usbs", opts.USER_SLIDE_BATCH_SIZE);
                    }                    
                    if (!string.IsNullOrEmpty(opts.USER_BACKGROUND_IMG_URL))
                    {
                        cmd.Parameters.AddWithValue("@ubiu", opts.USER_BACKGROUND_IMG_URL);
                    }
                    if (!string.IsNullOrEmpty(opts.USER_LOGO_IMG_URL))
                    {
                        cmd.Parameters.AddWithValue("@uliu", opts.USER_LOGO_IMG_URL);
                    }
                    con.Open();

                    try
                    {
                        SqlDataReader rdr = cmd.ExecuteReader();
                        rdr.Read();
                        res.Result = (AppEnums.SQLExecResult)Convert.ToInt32(rdr["RES"]);
                        rdr.Close();
                    }
                    catch (Exception ex)
                    {
                        res.Result = AppEnums.SQLExecResult.SyntaxError;
                        res.Message = ex.Message;
                    }

                    con.Close();
                }
            }

            return res;
        }

        /// <summary>
        /// Кол-во Ticks с момента последнего cобытия пользователя
        /// </summary>
        public static long TicksFromLastUserEvent(int user_id, int event_id, string description_must_contain = "")
        {
            long res = 0;
            string SQLStr = "SELECT TOP 1 LOG_TIME FROM LOG WHERE USER_ID=@user_id AND EVENT_ID=@event_id";
            if (!string.IsNullOrEmpty(description_must_contain))
            {
                SQLStr += string.Format(" AND LOG_DESCRIPTION LIKE '%{0}%'", description_must_contain);
            }

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(SQLStr + " ORDER BY DESC", con))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@event_id", event_id);

                    con.Open();

                    try
                    {
                        res = DateTime.Now.Ticks - Convert.ToDateTime(cmd.ExecuteScalar()).Ticks;
                    }
                    catch
                    { }

                    con.Close();
                }
            }

            return res;
        }

        /// <summary>
        /// Кол-во Ticks с момента последнего cобытия пользователя
        /// </summary>
        public static long TicksFromLastUserEvent(string user_login, int event_id, string description_must_contain = "")
        {
            long res = 0;
            string SQLStr = "SELECT TOP 1 LOG_TIME FROM LOG WHERE USER_ID=(SELECT USER_ID FROM APPUSER WHERE LOWER(USER_LOGIN)=@user_login) AND EVENT_ID=@event_id";
            if (!string.IsNullOrEmpty(description_must_contain))
            {
                SQLStr += string.Format(" AND LOG_DESCRIPTION LIKE '%{0}%'", description_must_contain);
            }

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(SQLStr + " ORDER BY DESC", con))
                {
                    cmd.Parameters.AddWithValue("@user_login", user_login);
                    cmd.Parameters.AddWithValue("@event_id", event_id);

                    con.Open();

                    try
                    {
                        res = DateTime.Now.Ticks - Convert.ToDateTime(cmd.ExecuteScalar()).Ticks;
                    }
                    catch
                    { }

                    con.Close();
                }
            }

            return res;
        }
    }
}