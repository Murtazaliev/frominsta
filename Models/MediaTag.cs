using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstagramMVC.Models
{
    public class MediaTag
    {
        //TODO может добавить CREATE_DATE и брать из запроса, а потом по нему фильтровать, хотя запрос идет по recent ?
        public int MEDIA_ID { get; set; }
        public int USER_ID { get; set; }
        public int ORDER_ID { get; set; }
        public string TAG_CAPTION { get; set; }
        public string INSTAGRAM_MEDIA_ID { get; set; }
        public DateTime INSTAGRAM_MEDIA_CREATED_TIME { get; set; }
        public string INSTAGRAM_MEDIA_LOW_RES_URL { get; set; }
        public string INSTAGRAM_MEDIA_STANDARD_RES_URL { get; set; }
        public string INSTAGRAM_MEDIA_THUMBNAIL_URL { get; set; }
        public string INSTAGRAM_USER_ID { get; set; }
        public string INSTAGRAM_USER_NAME { get; set; }
        public string INSTAGRAM_USER_PROFILEPICTURE { get; set; }
        public string INSTAGRAM_CAPTION { get; set; }
        public bool BAN { get; set; }
        public bool DELETED { get; set; }
    }
}