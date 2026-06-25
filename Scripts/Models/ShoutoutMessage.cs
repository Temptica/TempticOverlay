using Newtonsoft.Json;

namespace Temptica.Overlay.Scripts.Models;

public class ShoutoutMessage
{
    [JsonProperty("twitch_id")]
    public string TwitchId { get; set; }
    [JsonProperty("message")]
    public string Message { get; set; }
    [JsonProperty("owner_twitch_id")]
    public string OwnerTwitchId { get; set; }
    [JsonProperty("color")]
    public string Color { get; set; }
}