using System;
using InstagramMVC.Globals;
using InstagramMVC.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using System.Web.Mvc;
using InstagramMVC.Models.NavModel;

namespace InstagramMVC.DataManagers
{
    public class UtilManager
    {

        /// <summary>
        /// Получить значение переменной из табл. VARIABLE
        /// </summary>
        /// <param name="VarName">Название переменной</param>
        public static string GetVarValue(string VarName)
        {
            string res = string.Empty;

            try
            {
                using (var con = new SqlConnection(AppConst.ConnStr))
                {
                    using (var cmd = new SqlCommand("SELECT VAR_VALUE FROM VARIABLE WHERE LOWER(VAR_NAME)=@var_name", con))
                    {
                        cmd.Parameters.AddWithValue("@var_name", VarName.ToLower());

                        con.Open();
                        res = (string)cmd.ExecuteScalar();
                        con.Close();
                    }
                }
            }
            catch {}
            
            return res;
        }

        public static SQLReturnResult SetVarValue(string VarName, string VarValue)
        {
            var res = new SQLReturnResult();

            try
            {
                using (var con = new SqlConnection(AppConst.ConnStr))
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(AppConst.SQLBeginTran);
                    sql.AppendLine("UPDATE VARIABLE SET VAR_VALUE=@var_value WHERE LOWER(VAR_NAME)=@var_name");
                    sql.AppendLine(AppConst.SQLCommitTran);

                    using (var cmd = new SqlCommand(sql.ToString(), con))
                    {
                        cmd.Parameters.AddWithValue("@var_name", VarName.ToLower());
                        cmd.Parameters.AddWithValue("@var_value", VarValue);

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
            }
            catch { }

            return res;
        }

        public static IList<Models.AdminModels.Variable> GetVariables()
        {
            var res = new List<Models.AdminModels.Variable>();

            try
            {
                using (var con = new SqlConnection(AppConst.ConnStr))
                {
                    using (var cmd = new SqlCommand("SELECT * FROM VARIABLE ORDER BY VAR_NAME", con))
                    {
                        con.Open();

                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            res.Add(new Models.AdminModels.Variable() { VarName = Convert.ToString(rdr["VAR_NAME"]), 
                                                                        VarValue = Convert.ToString(rdr["VAR_VALUE"]) });
                        }
                        rdr.Close();
                        con.Close();
                    }
                }
            }
            catch
            { }

            return res;
        }

        /// <summary>
        /// Получить список ролей
        /// </summary>
        /// <param name="Exclude">Исключить роли через ";"</param>
        public static IList<Role> GetRoles(string Exclude = "")
        {
            var res = new List<Role>();
            string[] roles = Exclude.ToLower().Split(';');

            try
            {
                using (var con = new SqlConnection(AppConst.ConnStr))
                {
                    using (var cmd = new SqlCommand("SELECT * FROM ROLE", con))
                    {
                        con.Open();

                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            if (Array.IndexOf(roles, Convert.ToString(rdr["ROLE_NAME"]).ToLower()) == -1)
                            {
                                res.Add(new Role()
                                                 {
                                                     ROLE_ID = Convert.ToInt32(rdr["ROLE_ID"]),
                                                     ROLE_NAME = Convert.ToString(rdr["ROLE_NAME"]),
                                                     ROLE_DISCR = Convert.ToString(rdr["ROLE_DISCR"])
                                                 });
                            }
                            
                        }
                        rdr.Close();
                        con.Close();
                    }
                }
            }
            catch
            { }

            return res;
        }

        public static void CreateUserPanel(System.Web.HttpContext httpContext)
        {
            UrlHelper uh = new UrlHelper(httpContext.Request.RequestContext);
            var _tabPanel = new TabPanel()
            {
                Tabs = new List<TabItem>() { new TabItem() 
                                                         { 
                                                             TabCaption = "Общие данные",
                                                             TabLinkURL = uh.Action("EditUser", "User"),
                                                             TabGroupIDs = AppEnums.Role.Admin.ToString() + "," + AppEnums.Role.TagModerator.ToString()+ "," + AppEnums.Role.PremiumModerator.ToString(),
                                                             TabOrderPos = 0,
                                                             TabActive = false
                                                         },
                                             new TabItem() 
                                                         { 
                                                             TabCaption = "Мои теги",
                                                             TabLinkURL = uh.Action("Tags", "User"),
                                                             TabGroupIDs = AppEnums.Role.TagModerator.ToString()+ "," + AppEnums.Role.PremiumModerator.ToString(),
                                                             TabOrderPos = 1,
                                                             TabActive = false
                                                         },
                                             new TabItem() 
                                                         { 
                                                             TabCaption = "Мои заказы",
                                                             TabLinkURL = uh.Action("User", "Show"),
                                                             TabGroupIDs = AppEnums.Role.TagModerator.ToString()+ "," + AppEnums.Role.PremiumModerator.ToString(),
                                                             TabOrderPos = 2,
                                                             TabActive = false
                                                         },
                                             new TabItem() 
                                                         { 
                                                             TabCaption = "Модерация",
                                                             TabLinkURL = uh.Action("Mod", "User"),
                                                             TabGroupIDs = AppEnums.Role.TagModerator.ToString()+ "," + AppEnums.Role.PremiumModerator.ToString(),
                                                             TabOrderPos = 3,
                                                             TabActive = false
                                                         },
                                             new TabItem() 
                                                         { 
                                                             TabCaption = "Параметры",
                                                             TabLinkURL = uh.Action("Options", "User"),
                                                             TabGroupIDs = AppEnums.Role.TagModerator.ToString()+ "," + AppEnums.Role.PremiumModerator.ToString(),
                                                             TabOrderPos = 4,
                                                             TabActive = false
                                                         },
                                             new TabItem() 
                                                         { 
                                                             TabCaption = "Сменить пароль",
                                                             TabLinkURL = uh.Action("Changepassword", "User"),
                                                             TabGroupIDs = AppEnums.Role.Admin.ToString() + "," + AppEnums.Role.TagModerator.ToString()+ "," + AppEnums.Role.PremiumModerator.ToString(),
                                                             TabOrderPos = 5,
                                                             TabActive = false
                                                         }
                                            }
            };

            httpContext.Session["PROFILEPANEL"] = _tabPanel;
        }

        public static void CreateAdminPanel(System.Web.HttpContext httpContext)
        {
            if (httpContext.User.IsInRole("Admin"))
            {
                UrlHelper uh = new UrlHelper(httpContext.Request.RequestContext);

                var _admPanel = new TabPanel()
                {
                    Tabs = new List<TabItem>() { new TabItem() 
                                                     { 
                                                         TabCaption = "Переменные",
                                                         TabLinkURL = uh.Action("EditVariables", "Admin"),
                                                         TabGroupIDs = AppEnums.Role.Admin.ToString(),
                                                         TabOrderPos = 1,
                                                         TabActive = false
                                                     },
                                                     new TabItem() 
                                                     { 
                                                         TabCaption = "Пользователи",
                                                         TabLinkURL = uh.Action("EditUsers", "Admin"),
                                                         TabGroupIDs = AppEnums.Role.Admin.ToString(),
                                                         TabOrderPos = 2,
                                                         TabActive = false
                                                     },
                                                    new TabItem() 
                                                     { 
                                                         TabCaption = "Администраторы",
                                                         TabLinkURL = uh.Action("EditAdmins", "Admin"),
                                                         TabGroupIDs = AppEnums.Role.Admin.ToString(),
                                                         TabOrderPos = 3,
                                                         TabActive = false
                                                     },
                                                    new TabItem() 
                                                     { 
                                                         TabCaption = "Журнал",
                                                         TabLinkURL = uh.Action("Log", "Admin"),
                                                         TabGroupIDs = AppEnums.Role.Admin.ToString(),
                                                         TabOrderPos = 4,
                                                         TabActive = false
                                                     }
                                            }
                };

                httpContext.Session["ADMINPANEL"] = _admPanel;
            }
        }

        public static void RegisterEvent(int user_id, AppEnums.Event evnt, string description = "")
        {
            if (user_id == 4)
                return;

            try
            {
                using (var con = new SqlConnection(AppConst.ConnStr))
                {
                    using (var cmd = new SqlCommand("LogAddEvent", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@user_id", SqlDbType.Int, 4).Value = user_id;
                        cmd.Parameters.Add("@event_id", SqlDbType.Int, 4).Value = Convert.ToInt32(evnt);
                        cmd.Parameters.Add("@descr", SqlDbType.VarChar, 300).Value = description;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Clone();
                    }
                }
            }
            catch { }
        }
    }
}