using System.Text.Json;
using ALRC.Abstraction;

namespace ALRC.Converters;

public class ALRCConverter : ILyricConverter<string>
{
    public ALRCFile Convert(string input)
    {
        return JsonSerializer.Deserialize<ALRCFile>(input) ?? throw new FormatException();
    }

    public string ConvertBack(ALRCFile input)
    {
        return JsonSerializer.Serialize(input);
    }
}