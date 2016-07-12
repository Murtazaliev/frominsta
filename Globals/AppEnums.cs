using System;

namespace InstagramMVC.Globals
{
    public static class AppEnums
    {
        public enum SQLExecResult : int
        {
            Success = 0,
            SyntaxError = -1,
            RollBack = -2,
            Unknown = -100
        }

        public enum Role : int
        {
            Admin = 1,
            TagModerator = 2,
            Guest = 3,
            PremiumModerator = 4
        }

        public enum OperationStatus : int
        { 
            Success = 1,
            Error = 2,
            Warning = 3,
            Info = 4
        }

        public enum Event : int
        {
            Регистрация = 1,
            Вход = 2,
            Изменение_личных_регистрационных_данных = 3,
            Добавить_хэштег = 4,
            Удалить_хэштег = 5,
            Начало_трансяции = 6,
            Запуск_обновления_медиатегов = 7,
            Добавить_заказ = 8,
            Удалить_заказ = 9,
            Очистить_медиатеги_в_БД = 10,
            Сохранить_параметры_трансляции = 11,
            Сменить_пароль =12,
            Выход = 13,
            Сохранить_глобальные_переменные = 14,
            Удалить_пользователя = 15,
            Сделать_пользователя_администратором = 16,
            Забрать_права_администратора = 17,
            Неудачная_попытка_входа = 18,
            Попытка_входа_в_ограниченную_зону = 19,
            Изменение_данных_пользователя = 20,
            Пометить_заказ_как_оплаченный = 21,
            Пометить_заказ_как_неоплаченный = 22,
            Пометить_заказ_как_модерируемый = 23,
            Пометить_заказ_как_немодерируемый = 24,
            Сохранить_параметры_заказа = 25
        }

        public enum UpdateHashTagType : int
        { 
            Recent30 = 1,
            FromLast30 = 2
        }

        public enum SelectMediaAttr
        { 
            HideBaned = 1,
            HideDeleted = 2,
            HideBanedDeleted = 3,
            HideNothing = 4
        }
    }
}