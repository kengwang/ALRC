using System.Text.Json.Serialization;
using ALRC.Abstraction.Words;
using Newtonsoft.Json;

namespace ALRC.Abstraction;

public class ALRCLine
{

    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonProperty("p")]
    [JsonPropertyName("p")]
    public string? ParentLineId { get; set; }
    
    [JsonProperty("f")]
    [JsonPropertyName("f")]
    public long? Start { get; set; }

    [JsonProperty("t")]
    [JsonPropertyName("t")]
    public long? End { get; set; }

    [JsonProperty("s")]
    [JsonPropertyName("s")]
    public string? LineStyle { get; set; }


    [JsonProperty("cm")]
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    [JsonProperty("tx")]
    [JsonPropertyName("tx")]
    public string? RawText { get; set; }
    
    [JsonProperty("lt")]
    [JsonPropertyName("lt")]
    public string? Transliteration { get; set; }

    [JsonProperty("tr")]
    [JsonPropertyName("tr")]
    public Dictionary<string, string?>? LineTranslations { get; set; }

    [JsonProperty("w")]
    [JsonPropertyName("w")]
    public List<ALRCWord>? Words { get; set; }
}