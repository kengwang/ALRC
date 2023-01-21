using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ALRC.Abstraction.Words;

public class ALRCWord
{
    [JsonProperty("f")]
    [JsonPropertyName("f")]
    public long Start { get; set; }

    [JsonProperty("t")]
    [JsonPropertyName("t")]
    public long End { get; set; }

    [JsonProperty("w")]
    [JsonPropertyName("w")]
    public required string Word { get; set; }

    [JsonProperty("s")]
    [JsonPropertyName("s")]
    public string? WordStyle { get; set; }
}