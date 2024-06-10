using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using ALRC.Abstraction;

namespace ALRC.Converters;

public class QQLyricConverter : ILyricConverter<string>
{
    public ALRCFile Convert(string input)
    {
        return ConvertCore(input, @"(?<word>.*?)\((?<start>\d+),(?<end>\d+)\)",@"(?<=^\[\d+,\d+\])\(|(?<=^\[\d+,\d+\]\(.*)\)$");
    }

    public static ALRCFile ConvertCore(string input, /*[StringSyntax("regex")]*/ string regex, /*[StringSyntax("regex")]*/ string backgroundRegex)
    {
        var alrcLines = new List<ALRCLine>();
        var alrc = new ALRCFile
        {
            Schema = "https://github.com/kengwang/ALRC/blob/main/schemas/v1.json",
            LyricInfo = null,
            SongInfo = null,
            Header = null,
            Lines = alrcLines
        };
        var haveBackground = false;
        var lines = input.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var id = 0;
        foreach (var lineText in lines)
        {
            var alrcLine = new ALRCLine();
            if (string.IsNullOrWhiteSpace(lineText) || !lineText.StartsWith("[")) continue;
            if (!char.IsNumber(lineText[1])) continue;
            // 获取开始时间
            var timeEnd = lineText.IndexOf(']');
            var time = lineText[1..timeEnd].Split(',');
            alrcLine.Start = int.Parse(time[0]);
            alrcLine.End = alrcLine.Start + int.Parse(time[1]);
            // 获取歌词
            var lyric = lineText[(timeEnd + 1)..];
            if (Regex.IsMatch(lyric, backgroundRegex))
            {
                alrcLine.ParentLineId = id.ToString();
                alrcLines.Last().Id = id.ToString();
                alrcLine.Id = (++id).ToString();
                alrcLine.LineStyle = "background";
                haveBackground = true;
                lyric = Regex.Replace(lyric,backgroundRegex, "");
            }

            var words = new List<ALRCWord>();
            var sb = new StringBuilder();
            var wordMatches = Regex.Matches(lyric, regex);
            foreach (Match wordMatch in wordMatches)
            {
                var word = wordMatch.Groups["word"].Value;
                var start = int.Parse(wordMatch.Groups["start"].Value);
                var end = int.Parse(wordMatch.Groups["end"].Value);
                words.Add(new ALRCWord
                {
                    Word = word,
                    Start = start,
                    End = start +  end
                });
                sb.Append(word);
            }

            alrcLine.Words = words;
            alrcLine.RawText = sb.ToString();
            alrcLines.Add(alrcLine);
        }

        if (haveBackground)
        {
            alrc.Header ??= new ALRCHeader();
            alrc.Header.Styles ??= new List<ALRCStyle>();
            alrc.Header.Styles.Add(new ALRCStyle
            {
                Id = "background",
                Type = ALRCStyleAccent.Background,
                HiddenOnBlur = true
            });
        }

        return alrc;
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
                bool isBackground = false;
                if (alrcLine.Start is null || alrcLine is { Start: 0, End: 0 })
                {
                    if (alrcLine.Words is { Count: > 0 })
                        builder.Append(
                            $"[{alrcLine.Words[0].Start},{alrcLine.Words[^1].End - alrcLine.Words[0].Start}]");
                }
                else
                {
                    builder.Append($"[{alrcLine.Start},{alrcLine.End - alrcLine.Start}]");
                }

                if (input.Header?.Styles?.FirstOrDefault(t => t.Id == alrcLine.LineStyle) is
                    { Type: ALRCStyleAccent.Background or ALRCStyleAccent.Whisper }) isBackground = true;
                if (isBackground) builder.Append('(');
                if (alrcLine.Words is { Count: > 0 })
                    for (var index = 0; index < alrcLine.Words.Count; index++)
                    {
                        var alrcLineWord = alrcLine.Words[index];
                        builder.Append(
                            $"{alrcLineWord.Word}" +
                            (index == alrcLine.Words.Count - 1 && isBackground
                                ? ")"
                                : "") +
                            $"({alrcLineWord.Start},{alrcLineWord.End - alrcLineWord.Start})");
                    }

                builder.AppendLine();
            }
        }

        return builder.ToString();
    }
}