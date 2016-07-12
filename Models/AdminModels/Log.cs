using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InstagramMVC.DataManagers;

namespace InstagramMVC.Models.AdminModels
{
    public class Log
    {
        public int LOG_ID { get; set; }
        public int EVENT_ID { get; set; }
        public DateTime LOG_TIME { get; set; }
        public int USER_ID { get; set; }
        public string LOG_DESCRIPTION { get; set; }

        public string EVENT_NAME
        {
            get
            {
                return EventManager.GetEventName(EVENT_ID);
            }
        }

        public string USER_NAME
        {
            get
            {
                if (USER_ID > 0)
                {
                    return UserManager.GetUser(USER_ID).USER_LOGIN;
                }
                return "UNKNOWN";
            }
        }
    }
}