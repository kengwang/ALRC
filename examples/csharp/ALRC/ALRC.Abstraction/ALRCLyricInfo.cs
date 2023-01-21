using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ALRC.Abstraction;

public class ALRCLyricInfo
{
    [JsonProperty("lng")]
    [JsonPropertyName("lng")]
    public string? Language { get; set; }

    [JsonProperty("author")]
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonProperty("translation")]
    [JsonPropertyName("translation")]
    public string? Translation { get; set; }

    [JsonProperty("timeline")]
    [JsonPropertyName("timeline")]
    public string? Timeline { get; set; }

    [JsonProperty("transliteration")]
    [JsonPropertyName("transliteration")]
    public string? Transliteration { get; set; }

    [JsonProperty("proofread")]
    [JsonPropertyName("proofread")]
    public string? Proofread { get; set; }

    [JsonProperty("offset")]
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    [JsonProperty("duration")]
    [JsonPropertyName("duration")]
    public long? Duration { get; set; }
}