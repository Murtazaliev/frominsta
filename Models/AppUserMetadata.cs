using System;
using System.ComponentModel.DataAnnotations;
using InstagramMVC.Attributes;

namespace InstagramMVC.Models
{
    public class AppUserMetadata
    {
        public int USER_ID { get; set; }
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Укажите логин!")]
        [NoRegionalKey]
        [NoSpaceField]
        public string USER_LOGIN { get; set; }
        [Required(ErrorMessage = "Укажите фамилию!")]
        public string USER_LASTNAME { get; set; }
        [Required(ErrorMessage = "Укажите имя!")]
        public string USER_FIRSTNAME { get; set; }
        [Required(ErrorMessage = "Укажите e-mail!")]
        [EmailAddress(ErrorMessage = "Неверный формат e-mail!")]
        public string USER_EMAIL { get; set; }
        public string USER_PATR { get; set; }
        public string USER_PHONE { get; set; }
    }
}