using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ALRC.Abstraction;

public class ALRCFile
{
    [JsonProperty("$schema")]
    [JsonPropertyName("$schema")]
    public required string Schema { get; set; }

    [JsonProperty("li")]
    [JsonPropertyName("li")]
    public ALRCLyricInfo? LyricInfo { get; set; }

    [JsonProperty("si")]
    [JsonPropertyName("si")]
    public Dictionary<string, string>? SongInfo { get; set; }

    [JsonProperty("h")]
    [JsonPropertyName("h")]
    public ALRCHeader? Header { get; set; }

    [JsonProperty("l")]
    [JsonPropertyName("l")]
    public required List<ALRCLine> Lines { get; set; }
}