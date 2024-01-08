using System.Buffers;
using System.Text;
using ALRC.Abstraction;
using ALRC.Abstraction.Words;

namespace ALRC.Converters;

public class LrcConverter : ILyricConverter<string>
{
    public ALRCFile Convert(string input)
    {
        return LrcConverter.ParseLrc(input);
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
                builder.AppendLine($"[{inputLine.Start}, {inputLine.End}] {inputLine.RawText ?? string.Join("",(inputLine.Words ?? new List<ALRCWord>()).Select(t=>t.Word))}");
            }
        }

        return builder.ToString();
    }


    public static ALRCFile ParseLrc(ReadOnlySpan<char> input)
    {
        var alrc = Activator.CreateInstance<ALRCFile>();
        alrc.Schema = "https://github.com/kengwang/ALRC/schemas/v1.json";
        var lines = alrc.Lines = new List<ALRCLine>();
        var attributeName = string.Empty;
        var attributes = new List<KeyValuePair<string, string>>();
        var curStateStartPosition = 0;
        var timeCalculationCache = 0;
        var curTimestamps = ArrayPool<int>.Shared.Rent(16); // Max Count 
        int curTimestamp = 0;
        int currentTimestampPosition = 0;
        var offset = 0;
        var reachesEnd = false;
        var lastCharacterIsLineBreak = false;
        var state = CurrentState.None;
        var timeStampType = TimeStampType.None;
        for (var i = 0; i < input.Length; i++)
        {
            ref readonly var curChar = ref input[i];
            // 剥离开, 方便 分支预测
            if (state == CurrentState.Lyric)
            {
                if (curChar != '\n' && curChar != '\r' && i + 1 < input.Length)
                {
                    continue;
                }
                else
                {
                    if (i + 1 < input.Length)
                    {
                        for (int j = 0; j < curTimestamps.Length; j++)
                        {
                            if (curTimestamps[j] == -1) break;
                            if (lines.Count > 0)
                            {
                                lines[^1].End = curTimestamps[j] + offset;
                            }

                            lines.Add(new ALRCLine
                                      {
                                          Id = null,
                                          ParentLineId = null,
                                          Start = curTimestamps[j] + offset,
                                          End = null,
                                          LineStyle = null,
                                          Comment = null,
                                          RawText = input
                                                    .Slice(curStateStartPosition + 1, i - curStateStartPosition - 1)
                                                    .ToString(),
                                          LineTranslations = null,
                                          Words = null
                                      });
                        }

                        if (input[i + 1] == '\n' || input[i + 1] == '\r') i++;
                        currentTimestampPosition = 0;
                        // Change State
                        state = CurrentState.None;
                        continue;
                    }

                    if (i + 1 == input.Length)
                    {
                        reachesEnd = true;
                        if (curChar == '\r' || curChar == '\n')
                        {
                            lastCharacterIsLineBreak = true;
                        }
                    }
                }
            }

            if (reachesEnd && state == CurrentState.Lyric)
            {
                for (int j = 0; j < curTimestamps.Length; j++)
                {
                    if (curTimestamps[j] == -1) break;
                    if (lines.Count > 0)
                    {
                        lines[^1].End = curTimestamps[j] + offset;
                    }

                    lines.Add(new ALRCLine
                              {
                                  Id = null,
                                  ParentLineId = null,
                                  Start = curTimestamps[j] + offset,
                                  End = null,
                                  LineStyle = null,
                                  Comment = null,
                                  RawText = input.Slice(curStateStartPosition + 1,
                                                        i - curStateStartPosition - (lastCharacterIsLineBreak ? 1 : 0))
                                                 .ToString(),
                                  LineTranslations = null,
                                  Words = null
                              });
                }

                continue;
            }

            switch (state)
            {
                case CurrentState.PossiblyLyric:
                    if (curChar == '[')
                    {
                        state = CurrentState.AwaitingStateLyric;
                    }
                    else
                    {
                        i -= 1;
                        state = CurrentState.Lyric;
                    }

                    break;
                case CurrentState.None:
                    Array.Fill(curTimestamps, -1);
                    if (curChar == '[')
                    {
                        state = CurrentState.AwaitingState;
                    }

                    break;
                case CurrentState.AwaitingState:
                    if ('0' <= curChar && curChar <= '9')
                    {
                        // Time
                        state = CurrentState.Timestamp;
                        timeStampType = TimeStampType.Minutes;
                        curStateStartPosition = i;
                        curTimestamp = 0;
                        i--;
                    }
                    else
                    {
                        state = CurrentState.Attribute;
                        curStateStartPosition = i;
                    }

                    break;
                case CurrentState.AwaitingStateLyric:
                    if ('0' <= curChar && curChar <= '9')
                    {
                        // Time
                        state = CurrentState.Timestamp;
                        timeStampType = TimeStampType.Minutes;
                        curStateStartPosition = i;
                        curTimestamp = 0;
                        i--;
                    }
                    else
                    {
                        state = CurrentState.Lyric;
                        curStateStartPosition = i - 2;
                    }

                    break;
                case CurrentState.Attribute:
                    if (curChar == ':')
                    {
                        attributeName = input.Slice(curStateStartPosition, i - curStateStartPosition).ToString();
                        curStateStartPosition = i + 1;
                        state = CurrentState.AttributeContent;
                    }

                    break;
                case CurrentState.AttributeContent:
                    if (curChar == ']')
                    {
                        string attributeValue;
                        if (attributeName == "offset")
                        {
                            offset = timeCalculationCache;
                            timeCalculationCache = 0;
                            attributeValue = offset.ToString();
                        }
                        else
                        {
                            attributeValue = input.Slice(curStateStartPosition, i - curStateStartPosition).ToString();
                        }

                        attributes.Add(new KeyValuePair<string, string>(attributeName, attributeValue));
                        attributeName = string.Empty;
                        state = CurrentState.None;
                        break;
                    }

                    ;
                    if (attributeName == "offset")
                    {
                        timeCalculationCache = timeCalculationCache * 10 + curChar - '0';
                        continue;
                    }

                    break;
                case CurrentState.Timestamp:
                    if (timeStampType == TimeStampType.Milliseconds)
                    {
                        if (curChar != ']')
                        {
                            timeCalculationCache = timeCalculationCache * 10 + curChar - '0';
                            continue;
                        }
                        else
                        {
                            var pow = i - curStateStartPosition - 1; // 几位小数
                            curTimestamp = curTimestamp + timeCalculationCache * (int)Math.Pow(10, 3 - pow);
                            curTimestamps[currentTimestampPosition++] = curTimestamp;
                            timeStampType = TimeStampType.None;
                            timeCalculationCache = 0;
                            curStateStartPosition = i;
                            curTimestamp = 0;
                            state = CurrentState.PossiblyLyric;
                            continue;
                        }
                    }

                    switch (curChar)
                    {
                        case ':':
                        case '.':
                            if (timeStampType == TimeStampType.Minutes)
                            {
                                curTimestamp = (curTimestamp + timeCalculationCache) * 60;
                                timeCalculationCache = 0;
                                timeStampType = TimeStampType.Seconds;
                                continue;
                            }

                            if (timeStampType == TimeStampType.Seconds)
                            {
                                curTimestamp = (curTimestamp + timeCalculationCache) * 1000;
                                curStateStartPosition = i;
                                timeCalculationCache = 0;
                                timeStampType = TimeStampType.Milliseconds;
                                continue;
                            }

                            throw new ArgumentOutOfRangeException();
                        case ']':
                            // 无毫秒数需要从此跳出
                            curTimestamps[currentTimestampPosition++] = (curTimestamp + timeCalculationCache) * 1000;
                            timeCalculationCache = 0;
                            curStateStartPosition = i;
                            curTimestamp = 0;
                            state = CurrentState.PossiblyLyric;
                            timeStampType = TimeStampType.None;
                            continue;
                        default:
                            timeCalculationCache = timeCalculationCache * 10 + curChar - '0';
                            break;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        ArrayPool<int>.Shared.Return(curTimestamps, true);
        return alrc;
    }

    private enum CurrentState
    {
        None,
        AwaitingState,
        AwaitingStateLyric,
        Attribute,
        AttributeContent,
        Timestamp,
        PossiblyLyric,
        Lyric
    }

    private enum TimeStampType
    {
        Minutes,
        Seconds,
        Milliseconds,
        None
    }
}