using System.Text;
using System.Text.RegularExpressions;
using ALRC.Abstraction;

namespace ALRC.Converters;

public class QQLyricConverter : ILyricConverter<string>
{
    public ALRCFile Convert(string input)
    {
        var lines = input.Replace("\r\n", "\n").Replace("\r", "\n").Split("\n");
        foreach (var lineText in lines)
        {
            if (string.IsNullOrWhiteSpace(lineText) || !lineText.StartsWith('[')) continue;
            // 获取开始时间
            
        }

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

        if (input.Lines.Any(line => line.Words is { Count: > 0 }))
        {
            // 逐词模式
            foreach (var alrcLine in input.Lines)
            {
                bool isBackground = false;
                if (alrcLine.Start is null || alrcLine is { Start: 0, End: 0 })
                {
                    if (alrcLine.Words is { Count: > 0 })
                        builder.Append($"[{alrcLine.Words[0].Start},{alrcLine.Words[^1].End - alrcLine.Words[0].Start}]");
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
