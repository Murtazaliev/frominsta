using System;
using System.Web;
using System.ComponentModel.DataAnnotations;
using InstagramMVC.Attributes;

namespace InstagramMVC.Models.AccountModels
{
    public class ChangePasswordModel
    {
        [DataType(DataType.Password)]
        [Required]
        public string OldPassword { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Пароль не должен быть пустым!")]
        [MinRequiredPasswordLength]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароль и его подтверждение не совпадают!")]
        public string ConfirmPassword { get; set; }
    }
}