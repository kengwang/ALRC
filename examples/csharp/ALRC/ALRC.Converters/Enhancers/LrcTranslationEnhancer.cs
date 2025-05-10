using System.Text;
using ALRC.Abstraction;
using Opportunity.LrcParser;

namespace ALRC.Converters.Enhancers;

public class LrcTranslationEnhancer : ILyricEnhancer<string>
{
    public string Extract(ALRCFile input)
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

    public ALRCFile Enhance(string input, ALRCFile target)
    {
        var parseResult = Lyrics.Parse(input);
        foreach (var lyricsLine in parseResult.Lyrics.Lines)
        {
            var lrcTime = (long)(lyricsLine.Timestamp - DateTime.MinValue).TotalMilliseconds;
            var line = target.Lines.FirstOrDefault(t => t.Start == lrcTime);
            if (line is null)
                continue;
            line.Translation = lyricsLine.Content;
        }

        return target;
    }
}