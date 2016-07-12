using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using InstagramMVC.Attributes;

namespace InstagramMVC.Models.AccountModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage="Укажите фамилию!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Укажите имя!")]
        public string FirstName { get; set; }

        public string Patr { get; set; }

        [Required(ErrorMessage = "Укажите логин!")]
        [NoSpaceField]
        [NoRegionalKey]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Укажите e-mail!")]
        [EmailAddress(ErrorMessage = "Неверный формат e-mail!")]
        public string EMail { get; set; }

        [Required(ErrorMessage = "Пароль не должен быть пустым!")]
        [DataType(DataType.Password)]
        [MinRequiredPasswordLength]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "Пароль и его подтверждение не совпадают!")]
        public string ConfirmPassword { get; set; }

        public string Phone { get; set; }
    }
}