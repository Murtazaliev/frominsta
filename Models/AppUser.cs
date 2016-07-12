using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using InstagramMVC.DataManagers;

namespace InstagramMVC.Models
{
    [MetadataType(typeof(AppUserMetadata))]
    public partial class AppUser : AppUserMin
    {
        //public string[] USER_ROLES
        //{
        //    get
        //    {
        //        string roles = UserManager.GetUserRoles(USER_ID);
        //        return roles.Split(';');
        //    }
        //}
        public int USER_ROLE_ID { get; set; }
        public int USER_MAX_TAG_COUNT { get; set; }
    }
}