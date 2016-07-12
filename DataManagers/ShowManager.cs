using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using InstagramMVC.Globals;
using InstagramMVC.Models;
using InstagramMVC.Models.AccountModels;
using System.Text;

namespace InstagramMVC.DataManagers
{
    public class ShowManager
    {
        public static IList<Show> GetShows()
        {
            var res = new List<Show>();

            using (var con = new SqlConnection(AppConst.ConnStr))
            { 
                using (var cmd = new SqlCommand("SELECT * FROM SHOW", con))
                {
                    try
                    {
                        con.Open();

                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                res.Add(GetShow(Convert.ToInt32(rdr["SHOW_ID"])));
                            }
                        }

                        rdr.Close();
                        con.Close();
                    }
                    catch { }
                }
            }

            return res;
        }

        public static Show GetShow(int show_id)
        {
            Show res = null;

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT * FROM SHOW WHERE SHOW_ID=@show_id", con))
                {
                    cmd.Parameters.AddWithValue("@show_id", show_id);
                    try
                    {
                        con.Open();

                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            res = new Show()
                            {
                                SHOW_ID = Convert.ToInt32(rdr["SHOW_ID"]),
                                SHOW_START = Convert.ToDateTime(rdr["SHOW_START"]),
                                SHOW_END = Convert.ToDateTime(rdr["SHOW_END"]),
                                USER_ID = Convert.ToInt32(rdr["USER_ID"]),
                                ALLOWMOD = Convert.ToBoolean(rdr["ALLOWMOD"]),
                                PAID = Convert.ToBoolean(rdr["PAID"])
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

        public static SQLReturnResult SaveShow(Show show)
        {
            var res = new SQLReturnResult();

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("ShowSave", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter par = new SqlParameter("@show_id", SqlDbType.Int, 4);
                    par.Value = show.SHOW_ID;
                    par.Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.Add(par);

                    cmd.Parameters.Add("@user_id", SqlDbType.Int, 4).Value = show.USER_ID;
                    cmd.Parameters.Add("@show_start", SqlDbType.DateTime, 8).Value = show.SHOW_START;
                    cmd.Parameters.Add("@show_end", SqlDbType.DateTime, 8).Value = show.SHOW_END;
                    cmd.Parameters.Add("@paid", SqlDbType.Bit, 1).Value = show.PAID;
                    cmd.Parameters.Add("@allowmod", SqlDbType.Bit, 1).Value = show.ALLOWMOD;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        res.Result = AppEnums.SQLExecResult.Success;
                    }
                    catch (Exception ex)
                    {
                        res.Result = AppEnums.SQLExecResult.SyntaxError;
                        res.Message = ex.Message;
                    }
                }
            }

            return res;
        }

        public static void SetPaid(int show_id, bool value)
        {
            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("UPDATE SHOW SET PAID=@value WHERE SHOW_ID=@show_id", con))
                {
                    cmd.Parameters.AddWithValue("@show_id", show_id);
                    cmd.Parameters.AddWithValue("@value", value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        public static void SetMod(int show_id, bool value)
        {
            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("UPDATE SHOW SET ALLOWMOD=@value WHERE SHOW_ID=@show_id", con))
                {
                    cmd.Parameters.AddWithValue("@show_id", show_id);
                    cmd.Parameters.AddWithValue("@value", value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        public static SQLReturnResult DeleteShow(int show_id)
        {
            var res = new SQLReturnResult();

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("DELETE SHOW WHERE SHOW_ID=@show_id", con))
                {
                    cmd.Parameters.AddWithValue("@show_id", show_id);
                    
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        res.Result = AppEnums.SQLExecResult.Success;
                    }
                    catch (Exception ex)
                    {
                        res.Result = AppEnums.SQLExecResult.SyntaxError;
                        res.Message = ex.Message;
                    }
                }
            }

            return res;
        }
    }
}