using System.Buffers;
using System.Text;
using ALRC.Abstraction;
using Opportunity.LrcParser;

namespace ALRC.Converters;

public class LrcConverter : ILyricConverter<string>
{
    public ALRCFile Convert(string input)
    {
        return ParseLrc(input);
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
                var startTimeString = TimeSpan.FromMilliseconds(inputLine.Start ?? 0)
                                              .ToString(@"mm\:ss\.ff");
                builder.AppendLine($"[{startTimeString}] {inputLine.RawText ?? string.Join("",(inputLine.Words ?? new List<ALRCWord>()).Select(t=>t.Word))}");
            }
        }

        return builder.ToString();
    }


    public static ALRCFile ParseLrc(string input)
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
        
        var parseResult = Lyrics.Parse(input);
        if (parseResult.Lyrics.MetaData.Offset.TotalMilliseconds != 0)
        {
            alrc.LyricInfo ??= new ALRCLyricInfo();
            alrc.LyricInfo.Offset = (int)parseResult.Lyrics.MetaData.Offset.TotalMilliseconds;
        }

        ALRCLine lastLine = new ALRCLine();
        var lrcLines = parseResult.Lyrics.Lines.ToList().OrderBy(t => t.Timestamp);
        foreach (var lyricsLine in lrcLines)
        {
            var lyricTime = (long)(lyricsLine.Timestamp - DateTime.MinValue).TotalMilliseconds;
            var line = new ALRCLine
            {
                Start = lyricTime,
                RawText = lyricsLine.Content,
            };
            lines.Add(line);
            lastLine.End = lyricTime;
            lastLine = line;
        }
        
        return alrc;
    }
}