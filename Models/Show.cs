using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InstagramMVC.Models
{
    public class Show
    {
        public int SHOW_ID { get; set; }
        public int USER_ID { get; set; }
        public string USER_LOGIN { get; set; }
        [Required(ErrorMessage = "Укажите дату!")]
        [UIHint("DateAndTime")]
        public DateTime SHOW_START { get; set; }
        [Required(ErrorMessage = "Укажите дату!")]
        [UIHint("DateAndTime")]
        public DateTime SHOW_END { get; set; }
        public bool PAID { get; set; }
        public bool ALLOWMOD { get; set; }
    }
}