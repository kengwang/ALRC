using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ALRC.Abstraction;

public class ALRCHeader
{
    [JsonProperty("s")]
    [JsonPropertyName("s")]
    public List<ALRCStyle>? Styles { get; set; }
}

public class ALRCStyle
{
    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonProperty("p")]
    [JsonPropertyName("p")]
    public ALRCStylePosition? Position { get; set; } = ALRCStylePosition.Undefined;

    [JsonProperty("c")]
    [JsonPropertyName("c")]
    public string? Color { get; set; }

    [JsonProperty("t")]
    [JsonPropertyName("t")]
    public ALRCStyleAccent? Type { get; set; } = ALRCStyleAccent.Normal;
    
    [JsonProperty("h")]
    [JsonPropertyName("h")]
    public bool HiddenOnBlur { get; set; } = false;
}

public enum ALRCStylePosition
{
    Undefined = 0,
    Left = 1,
    Center = 2,
    Right = 3
}

public enum ALRCStyleAccent
{
    Normal = 0,
    Background = 1,
    Whisper = 2,
    Emphasise = 3
}