using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace InstagramMVC.Models.AccountModels
{
    public class EnterModel
    {
        [Required(ErrorMessage = "Укажите логин!", AllowEmptyStrings = false)]
        public string Login { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Укажите пароль!", AllowEmptyStrings = false)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}