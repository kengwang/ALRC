using System.Text;
using ALRC.Abstraction;

namespace ALRC.Converters;

public class LrcTranslationConverter : ILrcTranslationConverter<string>
{
    public string Convert(ALRCFile input)
    {
        var builder = new StringBuilder();
        // Order Lines
        var attachedLines = input.Lines.Where(t => !string.IsNullOrWhiteSpace(t.ParentLineId)).ToList();
        foreach (var attachedLine in attachedLines)
        {
            input.Lines.Remove(attachedLine);
            input.Lines.Insert(input.Lines.FindIndex(line => line.Id == attachedLine.ParentLineId) + 1, attachedLine);
        }

        if (input.Lines is { Count: > 0 })
        {
            foreach (var inputLine in input.Lines)
            {
                var startTimeString = TimeSpan.FromMilliseconds(System.Convert.ToDouble(inputLine.Start))
                    .ToString(@"mm\:ss\.ff");
                builder.AppendLine(
                    $"[{startTimeString}] {inputLine.Translation ?? string.Empty}");
            }
        }

        return builder.ToString();
    }

    public ALRCFile ConvertBack(string input, ALRCFile target)
    {
        throw new NotImplementedException();
    }
}