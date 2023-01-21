using System;
using System.Collections.Generic;
using System.Linq;
using ALRC.Abstraction;
using ALRC.Abstraction.Words;
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
                    Type = (ALRCStyleAccent)style.Type
                });
            }
        }

        foreach (var songInfo in input.SongInfos)
        {
            if (string.IsNullOrWhiteSpace(songInfo.Param) || string.IsNullOrWhiteSpace(songInfo.Value)) continue;
            alrc.SongInfo ??= new Dictionary<string, string>();
            alrc.SongInfo[songInfo.Param] = songInfo.Value;
        }

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
                foreach (var lineTranslation in inputLine.Translations)
                {
                    if (string.IsNullOrWhiteSpace(lineTranslation.LanguageTag)) continue;
                    lineToBeAdded.LineTranslations ??= new Dictionary<string, string?>();
                    lineToBeAdded.LineTranslations[lineTranslation.LanguageTag] =
                        lineTranslation.TranslationText;
                }

                if (inputLine.Type == 1)
                {
                    //Words
                    if (inputLine.Words is { Count: > 0 })
                    {
                        lineToBeAdded.Words ??= new List<ALRCWord>();
                        foreach (var word in inputLine.Words)
                        {
                            var wordToBeAdd = new ALRCWord()
                            {
                                Start = word.Start,
                                End = word.End,
                                WordStyle = string.IsNullOrWhiteSpace(word.WordStyle) ? null : word.WordStyle,
                                Word = word.Word ?? string.Empty
                            };
                            lineToBeAdded.Words.Add(wordToBeAdd);
                        }
                    }
                }

                alrc.Lines.Add(lineToBeAdded);
            }
        }

        return alrc;
    }

    public EditingALRC ConvertBack(ALRCFile input)
    {
        var alrc = Activator.CreateInstance<EditingALRC>();
        return alrc;
    }
}