using System.Text;
using System.Text.RegularExpressions;
using ALRC.Abstraction;

namespace ALRC.Converters;

public class LyricifySyllableConverter : ILyricConverter<string>
{
    public ALRCFile Convert(string input)
    {
        var lines = new List<ALRCLine>();
        var alrcFile = new ALRCFile
        {
            Schema = "https://github.com/kengwang/ALRC/blob/main/schemas/v1.json",
            Lines = lines,
            Header = new ALRCHeader()
            {
                Styles = new List<ALRCStyle>()
                {
                    new()
                    {
                        Id = "1",
                        Type = null,
                        Position = ALRCStylePosition.Left
                    },
                    new()
                    {
                        Id = "2",
                        Type = null,
                        Position = ALRCStylePosition.Right
                    },
                    new()
                    {
                        Id = "3",
                        Type = ALRCStyleAccent.Normal,
                        Position = null
                    },
                    new()
                    {
                        Id = "4",
                        Type = ALRCStyleAccent.Normal,
                        Position = ALRCStylePosition.Left
                    },
                    new()
                    {
                        Id = "5",
                        Type = ALRCStyleAccent.Normal,
                        Position = ALRCStylePosition.Right
                    },
                    new()
                    {
                        Id = "6",
                        Type = ALRCStyleAccent.Background,
                        HiddenOnBlur = true,
                        Position = null
                    },
                    new()
                    {
                        Id = "7",
                        Type = ALRCStyleAccent.Background,
                        HiddenOnBlur = true,
                        Position = ALRCStylePosition.Left
                    },
                    new()
                    {
                        Id = "8",
                        Type = ALRCStyleAccent.Background,
                        HiddenOnBlur = true,
                        Position = ALRCStylePosition.Right
                    },
                }
            }
        };
        var originalLines = input.Replace("\r\n", "\n").Split('\n');

        foreach (var originalLine in originalLines)
        {
            var line = new ALRCLine()
            {
                Words = new List<ALRCWord>()
            };
            var propertyEnd = originalLine.IndexOf(']');
            var property = originalLine.Substring(1, propertyEnd - 1);
            if (property.Length != 1 || !char.IsNumber(property[0])) continue;
            line.LineStyle = property;
            var words = originalLine[(propertyEnd + 1)..];
            var wordMatches = Regex.Matches(words, @"(.*?)\((\d+),(\d+)\)");
            var sb = new StringBuilder();
            foreach (Match wordMatch in wordMatches)
            {
                var word = wordMatch.Groups[1].Value;
                var start = int.Parse(wordMatch.Groups[2].Value);
                var end = int.Parse(wordMatch.Groups[3].Value);
                line.Words.Add(new ALRCWord
                {
                    Word = word,
                    Start = start,
                    End = start + end
                });
                sb.Append(word);
            }

            line.RawText = sb.ToString();
            line.Start = line.Words[0].Start;
            line.End = line.Words[^1].End;
            lines.Add(line);
        }

        return alrcFile;
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

        if (input.Lines.Any(line => line.Words is { Count: > 0 }))
        {
            // 逐词模式
            foreach (var alrcLine in input.Lines)
            {
                int attribute = 0;
                if (input.Header?.Styles is not null && input.Header.Styles.FirstOrDefault(t=>t.Id == alrcLine.LineStyle) is { } style)
                {
                    attribute += style.Type switch
                    {
                        ALRCStyleAccent.Normal => 1,
                        ALRCStyleAccent.Background => 2,
                        _ => 0
                    } * 3;
                    attribute += style.Position switch
                    {
                        ALRCStylePosition.Left => 1,
                        ALRCStylePosition.Right => 2,
                        _ => 0
                    };
                }

                builder.Append($"[{attribute}]");

                if (alrcLine.Words is { Count: > 0 })
                    for (var index = 0; index < alrcLine.Words.Count; index++)
                    {
                        var alrcLineWord = alrcLine.Words[index];
                        builder.Append(
                            $"{alrcLineWord.Word}" +
                            $"({alrcLineWord.Start},{alrcLineWord.End - alrcLineWord.Start})");
                    }

                builder.AppendLine();
            }
        }

        return builder.ToString();
    }
}