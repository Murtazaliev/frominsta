using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InstagramMVC.Globals;
using InstagramMVC.Models.UserModel;
using InstagramMVC.Models;
using InstagramMVC.Models.InstagramModels;
using InstagramMVC.InstagramEngine;

namespace InstagramMVC.DataManagers
{
    public class HashTagManager
    {
        public static IList<HashTag> GetUserTags(int user_id)
        {
            var res = new List<HashTag>();
            var opts = UserManager.GetUserOptions(user_id);

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(string.Format("SELECT TOP {0} * FROM HASHTAG WHERE [USER_ID]=@user_id", opts.USER_MAX_TAG_COUNT), con))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            res.Add(new HashTag()
                            {
                                TAG_ID = Convert.ToInt32(rdr["TAG_ID"]),
                                USER_ID = Convert.ToInt32(rdr["USER_ID"]),
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
            var opts = UserManager.GetUserOptions(user_login);

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(string.Format("SELECT TOP {0} * FROM HASHTAG WHERE [USER_ID]=(SELECT [USER_ID] FROM USER WHERE LOWER(USER_LOGIN)=@user_login)", 
                                                               opts.USER_MAX_TAG_COUNT), con))
                {
                    cmd.Parameters.AddWithValue("@user_login", user_login.ToLower());
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            res.Add(new HashTag()
                            {
                                TAG_ID = Convert.ToInt32(rdr["TAG_ID"]),
                                USER_ID = Convert.ToInt32(rdr["USER_ID"]),
                                TAG_CAPTION = rdr["TAG_CAPTION"].ToString()
                            });
                        }
                    }

                    con.Close();
                }
            }

            return res;
        }

        public static SQLReturnResult DeleteTag(string tag_caption)
        {
            SQLReturnResult res = new SQLReturnResult();

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("HashTagDelete", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@tag_caption", tag_caption);
                    
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
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

        public static SQLReturnResult DeleteMedia(int media_id)
        {
            SQLReturnResult res = new SQLReturnResult();

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("UPDATE MEDIATAG SET DELETED=1 WHERE MEDIA_ID=@media_id", con))
                {
                    cmd.Parameters.AddWithValue("@media_id", media_id);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
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

        public static bool IsTagExist(string tag_caption)
        {
            bool res = false;

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("HashTagExists", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@tag_caption", SqlDbType.VarChar, 100).Value = tag_caption;

                    SqlParameter par = new SqlParameter("@res", SqlDbType.Bit, 1);
                    par.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(par);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = Convert.ToBoolean(par.Value);
                    con.Close();
                }
            }

            return res;
        }

        public static SQLReturnResult InsertHashTag(int user_id, string tag_caption)
        {
            SQLReturnResult res = new SQLReturnResult();
            
            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("HashTagInsert", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@user_id", SqlDbType.Int, 4).Value = user_id;
                    cmd.Parameters.Add("@tag_caption", SqlDbType.VarChar, 100).Value = tag_caption;
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
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

        public static MediaTag GetMediaTagByID(int media_id)
        {
            MediaTag res = new MediaTag();

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("SELECT * FROM MEDIATAG WHERE MEDIA_ID=@media_id", con))
                {
                    cmd.Parameters.AddWithValue("@media_id", media_id);
                    con.Open();
                    try
                    {
                        var rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            rdr.Read();

                            res.MEDIA_ID = Convert.ToInt32(rdr["MEDIA_ID"]);
                            res.USER_ID = Convert.ToInt32(rdr["USER_ID"]);
                            res.ORDER_ID = Convert.ToInt32(rdr["ORDER_ID"]);
                            res.TAG_CAPTION = Convert.ToString(rdr["TAG_CAPTION"]);
                            res.INSTAGRAM_MEDIA_ID = Convert.ToString(rdr["INSTAGRAM_MEDIA_ID"]);
                            res.INSTAGRAM_MEDIA_CREATED_TIME = (rdr["INSTAGRAM_MEDIA_CREATED_TIME"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(rdr["INSTAGRAM_MEDIA_CREATED_TIME"]));
                            res.INSTAGRAM_MEDIA_LOW_RES_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_LOW_RES_URL"]);
                            res.INSTAGRAM_MEDIA_STANDARD_RES_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_STANDARD_RES_URL"]);
                            res.INSTAGRAM_MEDIA_THUMBNAIL_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_THUMBNAIL_URL"]);
                            res.INSTAGRAM_USER_ID = Convert.ToString(rdr["INSTAGRAM_USER_ID"]);
                            res.INSTAGRAM_USER_NAME = Convert.ToString(rdr["INSTAGRAM_USER_NAME"]);
                            res.INSTAGRAM_USER_PROFILEPICTURE = Convert.ToString(rdr["INSTAGRAM_USER_PROFILEPICTURE"]);
                            res.INSTAGRAM_CAPTION = Convert.ToString(rdr["INSTAGRAM_CAPTION"]);
                            res.BAN = Convert.ToBoolean(rdr["BAN"]);
                            res.DELETED = Convert.ToBoolean(rdr["DELETED"]);
                        }
                        else
                        {
                            res = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        res = null;
                    }

                    con.Close();
                }
            }

            return res;
        }

        /// <summary>
        /// Добавить медиатег в базу
        /// </summary>
        public static SQLReturnResult InsertMediaTag(MediaTag mediatag)
        {
            SQLReturnResult res = new SQLReturnResult();

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(@"INSERT MEDIATAG ([USER_ID], ORDER_ID, TAG_CAPTION, INSTAGRAM_MEDIA_ID, INSTAGRAM_MEDIA_CREATED_TIME,
INSTAGRAM_MEDIA_LOW_RES_URL, INSTAGRAM_MEDIA_STANDARD_RES_URL, INSTAGRAM_MEDIA_THUMBNAIL_URL, INSTAGRAM_USER_ID, INSTAGRAM_USER_NAME,
INSTAGRAM_USER_PROFILEPICTURE, INSTAGRAM_CAPTION, BAN, DELETED)
VALUES (@user_id, @order_id, @hashtag, @i_media_id, @i_media_created_time, @i_media_low_res_url, @i_media_standard_res_url, @i_media_thumnail_res_url, 
@i_user_id, @i_user_name, @i_user_profilepicture, @i_caption, @ban, @deleted)", con))
                {
                    
                    cmd.Parameters.AddWithValue("@user_id", mediatag.USER_ID);
                    cmd.Parameters.AddWithValue("@order_id", mediatag.ORDER_ID);
                    cmd.Parameters.AddWithValue("@hashtag", mediatag.TAG_CAPTION);
                    cmd.Parameters.AddWithValue("@i_media_id", mediatag.INSTAGRAM_MEDIA_ID);
                    cmd.Parameters.AddWithValue("@i_media_created_time", mediatag.INSTAGRAM_MEDIA_CREATED_TIME);
                    cmd.Parameters.AddWithValue("@i_media_low_res_url", mediatag.INSTAGRAM_MEDIA_LOW_RES_URL);
                    cmd.Parameters.AddWithValue("@i_media_standard_res_url", mediatag.INSTAGRAM_MEDIA_STANDARD_RES_URL);
                    cmd.Parameters.AddWithValue("@i_media_thumnail_res_url", mediatag.INSTAGRAM_MEDIA_THUMBNAIL_URL);
                    cmd.Parameters.AddWithValue("@i_user_id", mediatag.INSTAGRAM_USER_ID);
                    cmd.Parameters.AddWithValue("@i_user_name", mediatag.INSTAGRAM_USER_NAME);
                    cmd.Parameters.AddWithValue("@i_user_profilepicture", mediatag.INSTAGRAM_USER_PROFILEPICTURE);
                    cmd.Parameters.AddWithValue("@i_caption", mediatag.INSTAGRAM_CAPTION);
                    cmd.Parameters.AddWithValue("@ban", mediatag.BAN);
                    cmd.Parameters.AddWithValue("@deleted", mediatag.DELETED);

                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
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

        public static List<Media> LoadMediaByHashTag(string hashtag, int min_tag_id)
        {
            string access_token = InstagramMVC.DataManagers.UtilManager.GetVarValue("instagram.accesstoken");
            
            List<Media> medias = new List<Media>();
            var insta = new Instagram(access_token);
            medias = insta.SearchByTag(hashtag, 0, min_tag_id);
            return medias;
        }

        /// <summary>
        /// Получить медиа-тэги, сохраненные в БД
        /// </summary>
        /// <param name="user_id">ID пользователя</param>
        /// <param name="hashtag">Хэштег</param>
        /// <param name="selattr">ban, deleted selector</param>
        /// <returns></returns>
        public static List<MediaTag> GetDataBaseMediaTags(int user_id, string hashtag, AppEnums.SelectMediaAttr selattr)
        {
            var res = new List<MediaTag>();
            //TODO DELETED игнорировать для сранения есть ли они в базе
            string SQLStr = "SELECT * FROM MEDIATAG WHERE USER_ID=@user_id AND TAG_CAPTION=@hashtag";

            switch (selattr)
            { 
                case AppEnums.SelectMediaAttr.HideBaned:
                    SQLStr += " AND BAN=0";
                    break;
                case AppEnums.SelectMediaAttr.HideDeleted:
                    SQLStr += " AND DELETED=0";
                    break;
                case AppEnums.SelectMediaAttr.HideBanedDeleted:
                    SQLStr += " AND BAN=0 AND DELETED=0";
                    break;
            }
            
            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(SQLStr, con))// + " ORDER BY MEDIA_ID"
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@hashtag", hashtag);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            res.Add(new MediaTag()
                                    {
                                        MEDIA_ID = Convert.ToInt32(rdr["MEDIA_ID"]),                                            
                                        USER_ID = Convert.ToInt32(rdr["USER_ID"]),
                                        ORDER_ID = Convert.ToInt32(rdr["ORDER_ID"]),
                                        TAG_CAPTION = Convert.ToString(rdr["TAG_CAPTION"]),
                                        INSTAGRAM_MEDIA_ID = Convert.ToString(rdr["INSTAGRAM_MEDIA_ID"]),
                                        INSTAGRAM_MEDIA_CREATED_TIME = (rdr["INSTAGRAM_MEDIA_CREATED_TIME"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(rdr["INSTAGRAM_MEDIA_CREATED_TIME"])),
                                        INSTAGRAM_MEDIA_LOW_RES_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_LOW_RES_URL"]),
                                        INSTAGRAM_MEDIA_STANDARD_RES_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_STANDARD_RES_URL"]),
                                        INSTAGRAM_MEDIA_THUMBNAIL_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_THUMBNAIL_URL"]),
                                        INSTAGRAM_USER_ID = Convert.ToString(rdr["INSTAGRAM_USER_ID"]),
                                        INSTAGRAM_USER_NAME = Convert.ToString(rdr["INSTAGRAM_USER_NAME"]),
                                        INSTAGRAM_USER_PROFILEPICTURE = Convert.ToString(rdr["INSTAGRAM_USER_PROFILEPICTURE"]),
                                        INSTAGRAM_CAPTION = Convert.ToString(rdr["INSTAGRAM_CAPTION"]),
                                        BAN = Convert.ToBoolean(rdr["BAN"]),
                                        DELETED = Convert.ToBoolean(rdr["DELETED"])
                                    });
                        }
                    }
                    rdr.Close();
                    con.Close();
                }
            }

            return res;
        }

        public static List<MediaTag> GetDataBaseMediaTags(string user_login, string hashtag, AppEnums.SelectMediaAttr selattr = AppEnums.SelectMediaAttr.HideBanedDeleted)
        {
            var user = UserManager.GetUser(user_login);
            return GetDataBaseMediaTags(user.USER_ID, hashtag, selattr);
        }

        public static List<MediaTag> GetDataBaseMediaTags(int user_id, string hashtag, int page, AppEnums.SelectMediaAttr selattr)
        {
            var res = new List<MediaTag>();
            string aSQL = "";
            switch (selattr)
            {
                case AppEnums.SelectMediaAttr.HideBaned:
                    aSQL = " AND BAN=0";
                    break;
                case AppEnums.SelectMediaAttr.HideDeleted:
                    aSQL = " AND DELETED=0";
                    break;
                case AppEnums.SelectMediaAttr.HideBanedDeleted:
                    aSQL = " AND BAN=0 AND DELETED=0";
                    break;
            }

            string SQLStr = string.Format("SELECT * FROM "+
                "(SELECT *, ROW_NUMBER() OVER (ORDER BY MEDIA_ID DESC) AS RowNum FROM MEDIATAG WHERE USER_ID=@user_id AND TAG_CAPTION=@hashtag{0}) AS tbl "+
                "WHERE tbl.RowNum BETWEEN {1} AND {2} " +
                "ORDER BY MEDIA_ID DESC", aSQL, AppConst.ModPageSize * (page - 1) + 1, AppConst.ModPageSize * page);
            
            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(SQLStr, con))// + " ORDER BY MEDIA_ID"
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@hashtag", hashtag);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            res.Add(new MediaTag()
                            {
                                MEDIA_ID = Convert.ToInt32(rdr["MEDIA_ID"]),
                                USER_ID = Convert.ToInt32(rdr["USER_ID"]),
                                ORDER_ID = Convert.ToInt32(rdr["ORDER_ID"]),
                                TAG_CAPTION = Convert.ToString(rdr["TAG_CAPTION"]),
                                INSTAGRAM_MEDIA_ID = Convert.ToString(rdr["INSTAGRAM_MEDIA_ID"]),
                                INSTAGRAM_MEDIA_CREATED_TIME = (rdr["INSTAGRAM_MEDIA_CREATED_TIME"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(rdr["INSTAGRAM_MEDIA_CREATED_TIME"])),
                                INSTAGRAM_MEDIA_LOW_RES_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_LOW_RES_URL"]),
                                INSTAGRAM_MEDIA_STANDARD_RES_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_STANDARD_RES_URL"]),
                                INSTAGRAM_MEDIA_THUMBNAIL_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_THUMBNAIL_URL"]),
                                INSTAGRAM_USER_ID = Convert.ToString(rdr["INSTAGRAM_USER_ID"]),
                                INSTAGRAM_USER_NAME = Convert.ToString(rdr["INSTAGRAM_USER_NAME"]),
                                INSTAGRAM_USER_PROFILEPICTURE = Convert.ToString(rdr["INSTAGRAM_USER_PROFILEPICTURE"]),
                                INSTAGRAM_CAPTION = Convert.ToString(rdr["INSTAGRAM_CAPTION"]),
                                BAN = Convert.ToBoolean(rdr["BAN"]),
                                DELETED = Convert.ToBoolean(rdr["DELETED"])
                            });
                        }
                    }
                    rdr.Close();
                    con.Close();
                }
            }

            return res;
        }

        public static List<MediaTag> GetDataBaseMediaTags(string user_login, string hashtag, int page, AppEnums.SelectMediaAttr selattr)
        {
            var res = new List<MediaTag>();
            AppUser user = UserManager.GetUser(user_login);
            if (user != null)
            {
                string aSQL = "";
                switch (selattr)
                {
                    case AppEnums.SelectMediaAttr.HideBaned:
                        aSQL = " AND BAN=0";
                        break;
                    case AppEnums.SelectMediaAttr.HideDeleted:
                        aSQL = " AND DELETED=0";
                        break;
                    case AppEnums.SelectMediaAttr.HideBanedDeleted:
                        aSQL = " AND BAN=0 AND DELETED=0";
                        break;
                }

                string SQLStr = string.Format("SELECT * FROM " +
                    "(SELECT *, ROW_NUMBER() OVER (ORDER BY MEDIA_ID DESC) AS RowNum FROM MEDIATAG WHERE USER_ID=@user_id AND TAG_CAPTION=@hashtag{0}) AS tbl " +
                    "WHERE tbl.RowNum BETWEEN {1} AND {2} " +
                    "ORDER BY MEDIA_ID DESC", aSQL, AppConst.ModPageSize * (page - 1) + 1, AppConst.ModPageSize * page);

                using (var con = new SqlConnection(AppConst.ConnStr))
                {
                    using (var cmd = new SqlCommand(SQLStr, con))// + " ORDER BY MEDIA_ID"
                    {
                        cmd.Parameters.AddWithValue("@user_id", user.USER_ID);
                        cmd.Parameters.AddWithValue("@hashtag", hashtag);

                        con.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                res.Add(new MediaTag()
                                {
                                    MEDIA_ID = Convert.ToInt32(rdr["MEDIA_ID"]),
                                    USER_ID = Convert.ToInt32(rdr["USER_ID"]),
                                    ORDER_ID = Convert.ToInt32(rdr["ORDER_ID"]),
                                    TAG_CAPTION = Convert.ToString(rdr["TAG_CAPTION"]),
                                    INSTAGRAM_MEDIA_ID = Convert.ToString(rdr["INSTAGRAM_MEDIA_ID"]),
                                    INSTAGRAM_MEDIA_CREATED_TIME = (rdr["INSTAGRAM_MEDIA_CREATED_TIME"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(rdr["INSTAGRAM_MEDIA_CREATED_TIME"])),
                                    INSTAGRAM_MEDIA_LOW_RES_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_LOW_RES_URL"]),
                                    INSTAGRAM_MEDIA_STANDARD_RES_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_STANDARD_RES_URL"]),
                                    INSTAGRAM_MEDIA_THUMBNAIL_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_THUMBNAIL_URL"]),
                                    INSTAGRAM_USER_ID = Convert.ToString(rdr["INSTAGRAM_USER_ID"]),
                                    INSTAGRAM_USER_NAME = Convert.ToString(rdr["INSTAGRAM_USER_NAME"]),
                                    INSTAGRAM_USER_PROFILEPICTURE = Convert.ToString(rdr["INSTAGRAM_USER_PROFILEPICTURE"]),
                                    INSTAGRAM_CAPTION = Convert.ToString(rdr["INSTAGRAM_CAPTION"]),
                                    BAN = Convert.ToBoolean(rdr["BAN"]),
                                    DELETED = Convert.ToBoolean(rdr["DELETED"])
                                });
                            }
                        }
                        rdr.Close();
                        con.Close();
                    }
                }
            }
            
            return res;
        }

        /// <summary>
        /// Количество медиатегов пользователя
        /// </summary>
        public static int GetMediaTagsCount(int user_id, string hashtag, AppEnums.SelectMediaAttr selattr = AppEnums.SelectMediaAttr.HideDeleted)
        {
            int res = 0;

            string SQLStr = "SELECT COUNT(*) FROM MEDIATAG WHERE USER_ID=@user_id AND TAG_CAPTION=@hashtag";

            switch (selattr)
            {
                case AppEnums.SelectMediaAttr.HideBaned:
                    SQLStr += " AND BAN=0";
                    break;
                case AppEnums.SelectMediaAttr.HideDeleted:
                    SQLStr += " AND DELETED=0";
                    break;
                case AppEnums.SelectMediaAttr.HideBanedDeleted:
                    SQLStr += " AND BAN=0 AND DELETED=0";
                    break;
            }

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(SQLStr, con))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@hashtag", hashtag);

                    con.Open();
                    res = AppUtils.ConvertToInteger(cmd.ExecuteScalar());
                    con.Close();
                }
            }

            return res;
        }

        public static int GetMediaTagsCount(string user_login, string hashtag, AppEnums.SelectMediaAttr selattr = AppEnums.SelectMediaAttr.HideDeleted)
        {
            AppUser user = UserManager.GetUser(user_login);
            if (user == null)
            {
                return 0;
            }
            else
            {
                return GetMediaTagsCount(user.USER_ID, hashtag);
            }
        }


        /// <summary>
        /// Получить кол-во страниц для медиатегов пользователя
        /// </summary>
        public static int GetMediaTagsTotalPages(int user_id, string hashtag, AppEnums.SelectMediaAttr selattr = AppEnums.SelectMediaAttr.HideDeleted)
        {
            int mTagsCnt = GetMediaTagsCount(user_id, hashtag, selattr);
            return mTagsCnt / AppConst.ModPageSize + ((mTagsCnt % AppConst.ModPageSize > 0) ? 1 : 0);
        }

        public static int GetMediaTagsTotalPages(string user_login, string hashtag, AppEnums.SelectMediaAttr selattr = AppEnums.SelectMediaAttr.HideDeleted)
        {
            AppUser user = UserManager.GetUser(user_login);
            if (user == null)
            {
                return 0;                
            }
            else
            {
                return GetMediaTagsTotalPages(user.USER_ID, hashtag, selattr);
            }
        }

       

        public static List<MediaTag> GetNextBatchDataBaseMediaTags(int user_id, string hashtag, int next_media_id, int bathch_size = 5)
        {
            var res = new List<MediaTag>();
            string SQLStr = string.Format("SELECT TOP {0} * FROM MEDIATAG WHERE USER_ID=@user_id AND TAG_CAPTION=@hashtag AND MEDIA_ID>@next_media_id AND BAN=0 AND DELETED=0", bathch_size);

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(SQLStr, con))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@hashtag", hashtag);
                    cmd.Parameters.AddWithValue("@next_media_id", next_media_id);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            res.Add(new MediaTag()
                            {
                                MEDIA_ID = Convert.ToInt32(rdr["MEDIA_ID"]),
                                USER_ID = Convert.ToInt32(rdr["USER_ID"]),
                                ORDER_ID = Convert.ToInt32(rdr["ORDER_ID"]),
                                TAG_CAPTION = Convert.ToString(rdr["TAG_CAPTION"]),
                                INSTAGRAM_MEDIA_ID = Convert.ToString(rdr["INSTAGRAM_MEDIA_ID"]),
                                INSTAGRAM_MEDIA_CREATED_TIME = (rdr["INSTAGRAM_MEDIA_CREATED_TIME"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(rdr["INSTAGRAM_MEDIA_CREATED_TIME"])),
                                INSTAGRAM_MEDIA_LOW_RES_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_LOW_RES_URL"]),
                                INSTAGRAM_MEDIA_STANDARD_RES_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_STANDARD_RES_URL"]),
                                INSTAGRAM_MEDIA_THUMBNAIL_URL = Convert.ToString(rdr["INSTAGRAM_MEDIA_THUMBNAIL_URL"]),
                                INSTAGRAM_USER_ID = Convert.ToString(rdr["INSTAGRAM_USER_ID"]),
                                INSTAGRAM_USER_NAME = Convert.ToString(rdr["INSTAGRAM_USER_NAME"]),
                                INSTAGRAM_USER_PROFILEPICTURE = Convert.ToString(rdr["INSTAGRAM_USER_PROFILEPICTURE"]),
                                INSTAGRAM_CAPTION = Convert.ToString(rdr["INSTAGRAM_CAPTION"]),
                                BAN = Convert.ToBoolean(rdr["BAN"]),
                                DELETED = Convert.ToBoolean(rdr["DELETED"])
                            });
                        }
                    }
                    rdr.Close();
                    con.Close();
                }
            }

            return res;
        }

        public static SQLReturnResult ClearMediaTags(int user_id, string hashtag)
        {
            SQLReturnResult res = new SQLReturnResult();

            StringBuilder sql = new StringBuilder();
            sql.AppendLine(AppConst.SQLBeginTran);
            sql.AppendLine("DELETE FROM MEDIATAG WHERE USER_ID=@user_id AND LOWER(TAG_CAPTION)=@hashtag");
            sql.AppendLine(AppConst.SQLCommitTran);

            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand(sql.ToString(), con))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@hashtag", hashtag.ToLower());

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
        /// Сохранить данные из Instagram в базу
        /// </summary>
        public static int SaveMediaTagsToDataBase(int user_id, string hashtag, AppEnums.UpdateHashTagType updtype = AppEnums.UpdateHashTagType.Recent30)
        {
            int res = 0;

            if (string.IsNullOrEmpty(hashtag))
                return res;

            List<MediaTag> SavedMediaTagResults = GetDataBaseMediaTags(user_id, hashtag, AppEnums.SelectMediaAttr.HideNothing);
            //использовать max_tag_id для добовления к основному запросу &max_tag_id= , чтобы уменьшить кол-во загружаемых элементов
            int min_tag_id = 0;
            if (updtype == AppEnums.UpdateHashTagType.FromLast30)
            {
                min_tag_id = (SavedMediaTagResults.Count == 0 ? 0 : SavedMediaTagResults.OrderBy(x => x.MEDIA_ID).Last().MEDIA_ID);
            }            

            //instagram_media_id = next_max_id+'_'+instagram_user_id
            //TODO если у hashtag в табл HASHTAG есть непустое поле NEXT_MAX_ID(добавить)? т.е. уже сохр в базу(если 1-ый раз , то null), то в LoadMediaByHashTag доб-ть это поле и там формир-ть соотв запрос + &max_tag_id=
            List<Media> NewMediaTagResult = LoadMediaByHashTag(hashtag, min_tag_id);

            foreach (var mr in NewMediaTagResult)
            {
                var MediaExists = (SavedMediaTagResults.Count == 0 ? null : SavedMediaTagResults.FirstOrDefault(x => (x.INSTAGRAM_MEDIA_ID == mr.Id)));
                if (MediaExists == null)
                {
                    try
                    {
                        var NewMediaTag = new MediaTag()
                        {
                            INSTAGRAM_MEDIA_ID = mr.Id,
                            INSTAGRAM_MEDIA_CREATED_TIME = mr.CreatedTime,
                            INSTAGRAM_MEDIA_LOW_RES_URL = mr.Images.LowResolution.Url,
                            INSTAGRAM_MEDIA_STANDARD_RES_URL = mr.Images.StandardResolution.Url,
                            INSTAGRAM_MEDIA_THUMBNAIL_URL = mr.Images.Thumbnail.Url,
                            INSTAGRAM_USER_ID = mr.User.Id.ToString(),
                            INSTAGRAM_USER_NAME = mr.User.Username,
                            INSTAGRAM_USER_PROFILEPICTURE = mr.User.ProfilePicture,
                            INSTAGRAM_CAPTION = AppUtils.ConvertToString(mr.Caption.Text),
                            USER_ID = user_id,
                            TAG_CAPTION = hashtag,
                            ORDER_ID = 0,
                            BAN = (InstagramMVC.DataManagers.UserManager.CanUserModerateShow(user_id)),
                            DELETED = false
                        };
                        InsertMediaTag(NewMediaTag);
                        res += 1;
                    }
                    catch { }
                    
                }
            }

            return res;
        }

        public static void SetBan(int media_id, bool ban)
        {
            using (var con = new SqlConnection(AppConst.ConnStr))
            {
                using (var cmd = new SqlCommand("UPDATE MEDIATAG SET BAN=@ban WHERE MEDIA_ID=@media_id", con))
                {
                    cmd.Parameters.AddWithValue("@media_id", media_id);
                    cmd.Parameters.AddWithValue("@ban", ban);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
    }
}