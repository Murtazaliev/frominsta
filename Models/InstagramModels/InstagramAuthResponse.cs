using System;
using Newtonsoft.Json;

namespace InstagramMVC.Models.InstagramModels
{
    public class InstagramAuthResponse
    {
        public string access_token { get; set; }
        public UserInfo user { get; set; }
    }
}