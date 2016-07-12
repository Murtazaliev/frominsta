using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace InstagramMVC.Globals
{
    public class AppConst
    {
        /// <summary>
        /// Строка соединения с БД
        /// </summary>
        public static string ConnStr = ConfigurationManager.ConnectionStrings["InstagramApp"].ToString();

        /// <summary>
        /// Для проверки подтверждения транзакций
        /// </summary>
        public static string SQLErrStr = "if @@ERROR<>0 Begin ROLLBACK TRAN TR GOTO FAIL_EXT End";
                
        public static string SQLBeginTran =
@"SET dateformat dmy
DECLARE @res int
BEGIN TRAN TR
";

        public static string SQLCommitTran =
@"COMMIT TRAN TR
SET @res=1
GOTO EXT
FAIL_EXT:
SET @res=-1
EXT:
SELECT @res AS RES
";

        /// <summary>
        /// разрешенные форматы файлов изображений
        /// </summary>
        public static string[] AllowedImageFileFormats = new string[] { ".jpg", ".jpeg", ".gif", ".bmp", ".png" };

        public static int LogPageSize = 5;
        public static int ModPageSize = 40;
    }       
}