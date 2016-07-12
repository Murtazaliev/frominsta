using System;

namespace InstagramMVC.Globals
{
    /// <summary>
    /// Класс возвращает результат выполнения SQL-запросов INSERT, UPDATE, DELETE
    /// </summary>
    public class SQLReturnResult
    {
        public Globals.AppEnums.SQLExecResult Result { get; set; }
        public string Message { get; set; }

        public SQLReturnResult()
        {
            Result = Globals.AppEnums.SQLExecResult.Unknown;
            Message = "Неизвестная ошибка!";
        }
    }
}