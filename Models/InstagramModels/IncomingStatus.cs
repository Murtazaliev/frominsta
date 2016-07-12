using Newtonsoft.Json;

namespace InstagramMVC.Models.InstagramModels
{
    public enum IncomingStatus
    {
        [JsonProperty("followed_by")]
        FollowedBy,
        [JsonProperty("requested_by")]
        RequestedBy,
        [JsonProperty("blocked_by_you")]
        BlockedbyYou,
        [JsonProperty("none")]
        None
    }
}