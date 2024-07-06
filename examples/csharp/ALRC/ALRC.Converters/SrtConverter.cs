using System.Text;
using System.Text.RegularExpressions;
using ALRC.Abstraction;

namespace ALRC.Converters;

public class SrtConverter : ILyricConverter<string>
{
    public ALRCFile Convert(string input)
    {
        var lines = new List<ALRCLine>();
        var alrc = new ALRCFile
        {
            Schema = "https://github.com/kengwang/ALRC/blob/main/schemas/v1.json",
            LyricInfo = null,
            SongInfo = null,
            Header = null,
            Lines = lines
        };
        const string regex = @"(?<id>\d+)\n(?<start>[\d\:,]*)\s?-->\s?(?<end>[\d\:,]*)\n(?<line>(?>.+\r?\n)+(?=(?>\r?\n)?))";
        var matches = Regex.Matches(input, regex);
        foreach (Match match in matches)
        {
            var line = match.Groups["line"].Value;
            var start = match.Groups["start"].Value;
            var end = match.Groups["end"].Value;
            // 00:00:11,269 to Milliseconds
            start = start.Replace(',', '.');
            end = end.Replace(',', '.');
            var startTime = TimeSpan.Parse(start).TotalMilliseconds;
            var endTime = TimeSpan.Parse(end).TotalMilliseconds;
            var lineContents = line.Replace("\r\n", "\n").Split('\n');
            var alrcLine = new ALRCLine
            {
                Id = match.Groups["id"].Value,
                Start = (int)startTime,
                End = (int)endTime
            };
            if (lineContents.Length > 0)
            {
                alrcLine.RawText = lineContents[0];
                if (lineContents.Length > 1)
                {
                    alrcLine.Translation = lineContents[1];
                }
                
                if (lineContents.Length > 2)
                {
                    alrcLine.Comment = lineContents[2];
                }
            }
            lines.Add(alrcLine);
        }

        return alrc;
    }

    public string ConvertBack(ALRCFile input)
    {
        var builder = new StringBuilder();
        if (input.Lines is { Count: > 0 })
        {
            var id = 1;
            for (var index = 0; index < input.Lines.Count; index++)
            {
                var inputLine = input.Lines[index];
                var startTimeString = TimeSpan.FromMilliseconds(inputLine.Start ?? 0)
                    .ToString(@"hh\:mm\:ss\,fff");
                var endTimeString = TimeSpan.FromMilliseconds(inputLine.End ?? 0)
                    .ToString(@"hh\:mm\:ss\,fff");
                builder.AppendLine($"{id++}\n{startTimeString} --> {endTimeString}\n{inputLine.RawText}");
                if (!string.IsNullOrWhiteSpace(inputLine.Translation))
                {
                    builder.AppendLine(inputLine.Translation);
                }

                if (!string.IsNullOrWhiteSpace(inputLine.Comment))
                {
                    builder.AppendLine(inputLine.Comment);
                }

                builder.AppendLine();
            }
        }

        return builder.ToString();
    }
}