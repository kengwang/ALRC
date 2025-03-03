using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ALRC.Abstraction;
using ALRC.Converters;

namespace ALRC.Creator.Models;

public class EditableALRCConverter : ILyricConverter<EditingALRC>
{
    public ALRCFile Convert(EditingALRC input)
    {
        var alrc = Activator.CreateInstance<ALRCFile>();
        alrc.Schema = "https://github.com/kengwang/ALRC/schemas/v1.json";

        // 别问我下面问什么写这么复杂, 为了节省转换出来的 ALRC 文件大小, 该 null 的 null 掉
        if (input.Styles is { Count: > 0 })
        {
            alrc.Header = new ALRCHeader();

            foreach (var style in input.Styles)
            {
                if (string.IsNullOrWhiteSpace(style.Id)) continue;
                alrc.Header.Styles ??= new List<ALRCStyle>();
                alrc.Header.Styles.Add(new ALRCStyle
                {
                    Id = style.Id,
                    Position = (ALRCStylePosition)style.Position,
                    Color = string.IsNullOrWhiteSpace(style.Color) ? null : style.Color,
                    Type = (ALRCStyleAccent)style.Type,
                    HiddenOnBlur = style.Hidden
                });
            }
        }
        var songInfoList = new List<KeyValuePair<string, string>>();
        foreach (var songInfo in input.SongInfos)
        {
            if (string.IsNullOrWhiteSpace(songInfo.Param) || string.IsNullOrWhiteSpace(songInfo.Value)) continue;
            alrc.SongInfo ??= [];
            songInfoList.Add(new KeyValuePair<string, string>(songInfo.Param, songInfo.Value));
        }
        alrc.SongInfo = songInfoList.ToArray();

        // shallow copy start
        {
            if (!string.IsNullOrWhiteSpace(input.LyricInfo.Author))
            {
                alrc.LyricInfo ??= new ALRCLyricInfo();
                alrc.LyricInfo.Author = input.LyricInfo.Author;
            }

            if (!string.IsNullOrWhiteSpace(input.LyricInfo.Language))
            {
                alrc.LyricInfo ??= new ALRCLyricInfo();
                alrc.LyricInfo.Language = input.LyricInfo.Language;
            }
            
            if (!string.IsNullOrWhiteSpace(input.LyricInfo.Translation))
            {
                alrc.LyricInfo ??= new ALRCLyricInfo();
                alrc.LyricInfo.Language = input.LyricInfo.Translation;
            }

            if (!string.IsNullOrWhiteSpace(input.LyricInfo.Proofread))
            {
                alrc.LyricInfo ??= new ALRCLyricInfo();
                alrc.LyricInfo.Proofread = input.LyricInfo.Proofread;
            }

            if (!string.IsNullOrWhiteSpace(input.LyricInfo.Transliteration))
            {
                alrc.LyricInfo ??= new ALRCLyricInfo();
                alrc.LyricInfo.Transliteration = input.LyricInfo.Transliteration;
            }

            if (!string.IsNullOrWhiteSpace(input.LyricInfo.Timeline))
            {
                alrc.LyricInfo ??= new ALRCLyricInfo();
                alrc.LyricInfo.Timeline = input.LyricInfo.Timeline;
            }

            if (input.LyricInfo.Duration is > 0)
            {
                alrc.LyricInfo ??= new ALRCLyricInfo();
                alrc.LyricInfo.Duration = input.LyricInfo.Duration;
            }

            if (input.LyricInfo.Offset is > 0)
            {
                alrc.LyricInfo ??= new ALRCLyricInfo();
                alrc.LyricInfo.Offset = input.LyricInfo.Offset;
            }
        }


        if (input.Lines is { Count: > 0 })
        {
            alrc.Lines = new();
            foreach (var inputLine in input.Lines)
            {
                var lineToBeAdded = new ALRCLine();
                lineToBeAdded.Id = string.IsNullOrWhiteSpace(inputLine.Id) ? null : inputLine.Id;
                lineToBeAdded.LineStyle = string.IsNullOrWhiteSpace(inputLine.LineStyle) ? null : inputLine.LineStyle;
                lineToBeAdded.ParentLineId =
                    string.IsNullOrWhiteSpace(inputLine.ParentLineId) ? null : inputLine.ParentLineId;
                lineToBeAdded.Comment = string.IsNullOrWhiteSpace(inputLine.Comment) ? null : inputLine.Comment;
                lineToBeAdded.RawText = (inputLine.Type != 0 || string.IsNullOrWhiteSpace(inputLine.Text))
                    ? null
                    : inputLine.Text;
                lineToBeAdded.Start = inputLine.Start;
                lineToBeAdded.End = inputLine.End;
                lineToBeAdded.Translation = inputLine.Translation;
                lineToBeAdded.Transliteration = inputLine.Transliteration;

                if (inputLine.Type == 1)
                {
                    //Words
                    if (inputLine.Words is { Count: > 0 })
                    {
                        lineToBeAdded.Words ??= new List<ALRCWord>();
                        var textSb = new StringBuilder();
                        var transliterationSb = new StringBuilder();
                        foreach (var word in inputLine.Words)
                        {
                            var wordToBeAdd = new ALRCWord()
                            {
                                Start = word.Start,
                                End = word.End,
                                WordStyle = string.IsNullOrWhiteSpace(word.WordStyle)
                                    ? null
                                    : word.WordStyle,
                                Word = word.Word ?? string.Empty,
                                Transliteration = word.Transliteration
                            };
                            lineToBeAdded.Words.Add(wordToBeAdd);
                            textSb.Append(wordToBeAdd.Word);
                            transliterationSb.Append(wordToBeAdd.Transliteration);
                        }
                        if (string.IsNullOrWhiteSpace(lineToBeAdded.RawText))
                        {
                            var res = textSb.ToString();
                            lineToBeAdded.RawText = string.IsNullOrWhiteSpace(res) ? null : res;
                        }

                        if (string.IsNullOrWhiteSpace(lineToBeAdded.Transliteration))
                        {
                            var res = transliterationSb.ToString();
                            lineToBeAdded.Transliteration = string.IsNullOrWhiteSpace(res) ? null : res;
                        }
                    }

                    if (lineToBeAdded.Transliteration is not { Length: > 0 })
                    {
                        var computedTransliteration = string.Join(" ",
                            inputLine.Words?.Select(t => t.Transliteration).ToList() ?? new());
                        if (!string.IsNullOrWhiteSpace(computedTransliteration))
                            inputLine.Transliteration = computedTransliteration;
                    }
                }

                alrc.Lines.Add(lineToBeAdded);
            }
        }

        return alrc;
    }

    public EditingALRC ConvertBack(ALRCFile input)
    {
        var ealrc = Activator.CreateInstance<EditingALRC>();
        if (input.Header?.Styles is { Count: > 0 })
        {
            ealrc.Styles = new ObservableCollection<EditingALRCStyle>();

            foreach (var style in input.Header.Styles)
            {
                if (string.IsNullOrWhiteSpace(style.Id)) continue;
                ealrc.Styles.Add(new EditingALRCStyle()
                {
                    Id = style.Id,
                    Position = (int)(style.Position ?? ALRCStylePosition.Undefined),
                    Color = string.IsNullOrWhiteSpace(style.Color) ? null : style.Color,
                    Type = (int)(style.Type ?? ALRCStyleAccent.Normal),
                    Hidden = style.HiddenOnBlur
                });
            }
        }

        if (input.SongInfo is not null)
            foreach (var songInfo in input.SongInfo)
            {
                if (string.IsNullOrWhiteSpace(songInfo.Key) || string.IsNullOrWhiteSpace(songInfo.Value)) continue;
                ealrc.SongInfos.Add(new SongInfoItem(songInfo.Key, songInfo.Value));
            }

        // shallow copy start
        {
            if (!string.IsNullOrWhiteSpace(input.LyricInfo?.Author))
            {
                ealrc.LyricInfo.Author = input.LyricInfo.Author;
            }
            
            if (!string.IsNullOrWhiteSpace(input.LyricInfo?.Translation))
            {
                ealrc.LyricInfo.Author = input.LyricInfo.Translation;
            }

            if (!string.IsNullOrWhiteSpace(input.LyricInfo?.Language))
            {
                ealrc.LyricInfo.Language = input.LyricInfo.Language;
            }

            if (!string.IsNullOrWhiteSpace(input.LyricInfo?.Proofread))
            {
                ealrc.LyricInfo.Proofread = input.LyricInfo.Proofread;
            }

            if (!string.IsNullOrWhiteSpace(input.LyricInfo?.Transliteration))
            {
                ealrc.LyricInfo.Transliteration = input.LyricInfo.Transliteration;
            }

            if (!string.IsNullOrWhiteSpace(input.LyricInfo?.Timeline))
            {
                ealrc.LyricInfo.Timeline = input.LyricInfo.Timeline;
            }

            if (input.LyricInfo?.Duration is > 0)
            {
                ealrc.LyricInfo.Duration = input.LyricInfo.Duration;
            }

            if (input.LyricInfo?.Offset is > 0)
            {
                ealrc.LyricInfo.Offset = input.LyricInfo.Offset;
            }
        }


        if (input.Lines is { Count: > 0 })
        {
            ealrc.Lines = new();
            foreach (var inputLine in input.Lines)
            {
                var lineToBeAdded = new EditingALRCLine();
                lineToBeAdded.Id = string.IsNullOrWhiteSpace(inputLine.Id) ? null : inputLine.Id;
                lineToBeAdded.LineStyle = string.IsNullOrWhiteSpace(inputLine.LineStyle) ? null : inputLine.LineStyle;
                lineToBeAdded.ParentLineId =
                    string.IsNullOrWhiteSpace(inputLine.ParentLineId) ? null : inputLine.ParentLineId;
                lineToBeAdded.Comment = string.IsNullOrWhiteSpace(inputLine.Comment) ? null : inputLine.Comment;
                lineToBeAdded.Text = inputLine.RawText;
                lineToBeAdded.Type = lineToBeAdded.Words is { Count: > 0 } ? 1 : 0;
                lineToBeAdded.Start = inputLine.Start ?? 0;
                lineToBeAdded.End = inputLine.End ?? 0;
                lineToBeAdded.Translation = inputLine.Translation;
                lineToBeAdded.Transliteration = inputLine.Transliteration;


                //Words
                if (inputLine.Words is { Count: > 0 })
                {
                    lineToBeAdded.Type = 1;
                    lineToBeAdded.Words ??= new();
                    foreach (var word in inputLine.Words)
                    {
                        var wordToBeAdd = new EditingALRCWord()
                        {
                            Start = word.Start,
                            End = word.End,
                            WordStyle = string.IsNullOrWhiteSpace(word.WordStyle)
                                ? null
                                : word.WordStyle,
                            Word = word.Word ?? string.Empty
                        };
                        lineToBeAdded.Words.Add(wordToBeAdd);
                    }
                }


                ealrc.Lines.Add(lineToBeAdded);
            }
        }

        return ealrc;
    }
}