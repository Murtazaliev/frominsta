namespace InstagramMVC.Models.UserModel
{
    public class UserOptions
    {
        public int USER_ID { get; set; }
        public int USER_MAX_TAG_COUNT { get; set; }
        public int USER_SLIDE_ROTATION { get; set; }
        public int USER_SLIDE_BATCH_SIZE { get; set; }
        public string USER_BACKGROUND_IMG_URL { get; set; }
        public string USER_LOGO_IMG_URL { get; set; }
    }
}