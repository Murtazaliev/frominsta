using InstagramMVC.DataManagers;
using System.Linq;

namespace InstagramMVC.Models
{
    /// <summary>
    /// Класс для экспорта в систему win32 InstagramAppUpdater
    /// </summary>
    public class TeleUser : AppUserMin
    {
        public bool CAN_USER_TRANSLATE_SHOW
        {
            get
            {
                return UserManager.CanUserTranslateShow(this.USER_ID);
            }
            set { }
        }

        public bool CAN_USER_MODERATE_SHOW
        {
            get
            {
                return UserManager.CanUserModerateShow(this.USER_ID);
            }
            set { }
        }

        public string TagCaption { get; set; }

        public int MediaTagCount
        {
            get 
            {
                return UserManager.GetTeleUserMediatagCount(this.USER_LOGIN, this.TagCaption);
            }
            set { }
        }

        public long TicksFromLastUpdate
        {
            get
            {
                return UserManager.TicksFromLastUserEvent(this.USER_ID, (int)InstagramMVC.Globals.AppEnums.Event.Запуск_обновления_медиатегов, string.Format("'{0}'", this.TagCaption));
            }
            set { }
        }

        public bool IsTelegramAllowedUser
        {
            get
            {
                string[] allowedRoles = InstagramMVC.DataManagers.UtilManager.GetVarValue("telegram.allowedroles").Split(',');
                return allowedRoles.Contains(UserManager.GetUserRole(this.USER_ID));
            }
            set { }
        }
    }
}