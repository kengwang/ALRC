using System.Text;
using ALRC.Abstraction;

namespace ALRC.Converters;

public class NeteaseYrcConverter : ILyricConverter<string>
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

        if (input.Lines.Any(line => line.Words is { Count: > 0 }))
        {
            // 逐词模式
            foreach (var alrcLine in input.Lines)
            {
                bool isBackground = false;
                builder.Append($"[{alrcLine.Start},{alrcLine.End - alrcLine.Start}]");
                if (input.Header?.Styles?.FirstOrDefault(t => t.Id == alrcLine.LineStyle) is
                    { Type: ALRCStyleAccent.Background or ALRCStyleAccent.Whisper }) isBackground = true;
                if (isBackground) builder.Append('(');
                if (alrcLine.Words is { Count: > 0 })
                    for (var index = 0; index < alrcLine.Words.Count; index++)
                    {
                        var alrcLineWord = alrcLine.Words[index];
                        builder.Append(
                            $"({alrcLineWord.Start},{alrcLineWord.End - alrcLineWord.Start})" +
                            $"{alrcLineWord.Word}" +
                            (index == alrcLine.Words.Count - 1 && isBackground
                                ? ")"
                                : ""));
                    }

                builder.AppendLine();
            }
        }

        return builder.ToString();
    }
}