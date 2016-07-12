using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace InstagramMVC.Globals
{
    public class AppUtils
    {
        public static int ConvertToInteger(Object o, int DefaultValue = 0)
        {
            int res;
            try
            {
                res = Convert.ToInt32(o);
            }
            catch
            {
                res = DefaultValue;
            }

            return res;
        }

        public static string ConvertToString(Object o)
        {
            string res = "";

            try
            {
                res = Convert.ToString(o);
            }
            catch
            {
                res = "";
            }

            return res;
        }

        public static string ConvertToDateTime(Object o)
        {
            string res = "";
            try
            {
                res = Convert.ToDateTime(o).ToShortDateString();
            }
            catch
            {
                res = Convert.ToDateTime("01.01.1900").ToShortDateString();
            }

            return res;
        }

        /// <summary>
        /// Вырезать или заменить потенциально опасные теги в сообщении
        /// </summary>
        public static string ReplaceDangerousChars(string InString)
        {
            string res = InString;

            res = res.Replace("<", "&lt;");
            res = res.Replace(">", "&gt;");

            return res;
        }

        public static string TimeAgo(DateTime dt)
        {
            string res = "";
            //60sec*60min*24h
            //TimeSpan diff = DateTime.Now.ToUniversalTime().Subtract(Convert.ToDateTime(dt.AddHours(2)));
            TimeZone curTimeZone = TimeZone.CurrentTimeZone;
            //TimeSpan instagramTimeUTCOffset = TimeZoneInfo.FindSystemTimeZoneById("UTC").GetUtcOffset(dt);
            DateTime curDateTime = DateTime.Now.ToUniversalTime();
            TimeSpan curTimeZoneUTCOffset = curTimeZone.GetUtcOffset(curDateTime);
            
            TimeSpan diff = curDateTime.Subtract(dt.Add(curTimeZoneUTCOffset));//Add( - instagramTimeUTCOffset)

            if (diff.Days > 0)
            {
                res += string.Format("{0} дн., ", diff.Days);
            }
            if (diff.Hours > 0)
            {
                res += string.Format("{0} ч., ", diff.Hours);
            }
            else
            {
                res += (string.IsNullOrEmpty(res) ? "" : "00 ч., ");
            }
            if (diff.Minutes > 0)
            {
                res += string.Format("{0} мин.", diff.Minutes);
            }
            else
            {
                res += string.Format("{0} сек.", diff.Seconds);
            }
            res += " назад";
         


            return res;
        }
    }
}