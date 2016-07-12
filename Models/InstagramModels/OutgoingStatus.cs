using Newtonsoft.Json;

namespace InstagramMVC.Models.InstagramModels
{
    public enum OutgoingStatus
    {
        [JsonProperty("follows")]
        Follows,
        [JsonProperty("requested")]
        Requested,
        [JsonProperty("none")]
        None
    }
}