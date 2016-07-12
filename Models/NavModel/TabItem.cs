namespace InstagramMVC.Models.NavModel
{
    public class TabItem
    {        
        public string TabCaption { get; set; }
        public string TabLinkURL { get; set; }
        public string TabGroupIDs { get; set; }
        public int TabOrderPos { get; set; }
        public bool TabActive { get; set; }
    }
}