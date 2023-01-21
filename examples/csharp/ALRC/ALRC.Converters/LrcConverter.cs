using System.Text;
using ALRC.Abstraction;

namespace ALRC.Converters;

public class LrcConverter : ILyricConverter<string>
{
    public ALRCFile Convert(string input)
    {
        throw new NotImplementedException();
    }

    public string ConvertBack(ALRCFile input)
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
                    .ToString(@"mm\:ss.ff");
                builder.AppendLine($"[{startTimeString}] {inputLine.RawText}");
            }
        }

        return builder.ToString();
    }
}