using InstagramMVC.Globals;
using InstagramMVC.Models.AdminModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace InstagramMVC.DataManagers
{
    public class EventManager
    {
        /// <summary>
        /// Получить список событий
        /// </summary>
        public static IList<Event> GetEvents()
        {
            var res = new List<Event>();

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT * FROM EVENT", con))
                {
                    con.Open();

                    var rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            res.Add(new Event()
                            {
                                EVENT_ID = Convert.ToInt32(rdr["EVENT_ID"]),
                                EVENT_NAME = Convert.ToString(rdr["EVENT_NAME"])
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
        /// Получить название события
        /// </summary>
        public static string GetEventName(int event_id)
        {
            var res = "";

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT EVENT_NAME FROM EVENT WHERE EVENT_ID=@event_id", con))
                {
                    cmd.Parameters.AddWithValue("@event_id", event_id);

                    con.Open();
                    res = AppUtils.ConvertToString(cmd.ExecuteScalar());
                    con.Close();
                }
            }

            return res;
        }

        /// <summary>
        /// Получить количество событий в логе
        /// </summary>
        public static int GetLogEventsCount(int user_id, int event_id)
        {
            int res = 0;
            string SQLCmd = "SELECT COUNT(*) AS CNT FROM LOG";
            string WhereClause = (user_id > 0 ? 
                                  string.Format(" WHERE USER_ID={0}", user_id) + (event_id > 0 ? string.Format(" AND EVENT_ID={0}", event_id) : "") : 
                                  (event_id > 0 ? string.Format(" WHERE EVENT_ID={0}", event_id) : ""));

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(SQLCmd + WhereClause, con))
                {
                    con.Open();
                    res = AppUtils.ConvertToInteger(cmd.ExecuteScalar());
                    con.Close();
                }
            }

            return res;
        }

        /// <summary>
        /// Получить кол-во страниц в логе событий
        /// </summary>
        public static int GetLogEventTotalPages(int user_id, int event_id)
        {
            int evtCnt = GetLogEventsCount(user_id, event_id);
            return evtCnt / AppConst.LogPageSize + ((evtCnt % AppConst.LogPageSize > 0) ? 1 : 0);
        }

        /// <summary>
        /// Получить указанную страницу событий в логе
        /// </summary>
        /// <param name="page">№ страницы в Pager</param>
        /// <param name="user_id">ID пользователя, инициировавшего события (0 - все пользователи)</param>
        /// <param name="event_id">ID события (0 - все события)</param>
        public static IList<Log> GetLogEvents(int page, int user_id, int event_id)
        {
            var res = new List<Log>();

            int totalPages = GetLogEventTotalPages(user_id, event_id);
            if (page > totalPages)
            {
                page = totalPages;
            }

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(string.Format("SELECT * FROM " +
"(SELECT *, ROW_NUMBER() OVER (ORDER BY LOG_ID DESC) AS RowNum FROM LOG "+
(user_id > 0 ?
             string.Format(" WHERE USER_ID={0}", user_id) + (event_id > 0 ? string.Format(" AND EVENT_ID={0}", event_id) : "") :
             (event_id > 0 ? string.Format(" WHERE EVENT_ID={0}", event_id) : "")) + ") AS tbl " +
"WHERE tbl.RowNum BETWEEN {0} AND {1} " +
"ORDER BY LOG_ID DESC", AppConst.LogPageSize * (page - 1)+1, AppConst.LogPageSize * page), con))
//                (user_id > 0 ? string.Format(" AND USER_ID={0}", user_id) : "") +
//(event_id > 0 ? string.Format(" AND EVENT_ID={0}", event_id) : "") +
                {
                    con.Open();

                    var rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            res.Add(new Log()
                            {
                                LOG_ID = Convert.ToInt32(rdr["LOG_ID"]),
                                EVENT_ID = Convert.ToInt32(rdr["EVENT_ID"]),
                                LOG_TIME = Convert.ToDateTime(rdr["LOG_TIME"]),
                                USER_ID = Convert.ToInt32(rdr["USER_ID"]),
                                LOG_DESCRIPTION = AppUtils.ConvertToString(rdr["LOG_DESCRIPTION"])
                            });
                        }
                    }

                    rdr.Close();
                    con.Close();
                }
            }

            return res;
        }
    }
}