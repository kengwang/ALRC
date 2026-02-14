using System.Text;
using System.Xml;
using ALRC.Abstraction;

namespace ALRC.Converters;

public class AppleSyllableConverter : ILyricConverter<string>
{
    public const bool FEATURE_TRANSLITERATION_PARSE_EXP = false;
    public const bool FEATURE_ITUNESMETADATA_PARSE_EXP = true;
    // 部分遵循 AMLL 规范
    public ALRCFile Convert(string input)
    {
        var ttml = new XmlDocument
        {
            PreserveWhitespace = true
        };
        var lines = new List<ALRCLine>();
        Dictionary<string,string>? translations = null;
        Dictionary<string,string>? transliterations = null;
        var alrc = new ALRCFile
        {
            Schema = "https://github.com/kengwang/ALRC/blob/main/schemas/v1.json",
            LyricInfo = null,
            SongInfo = null,
            Header = null,
            Lines = lines
        };
        ttml.LoadXml(input);
        var head = ttml.GetElementsByTagName("head").Cast<XmlElement>().FirstOrDefault();
        if (head != null)
        {
            alrc.Header ??= new ALRCHeader();
            alrc.Header.Styles = new List<ALRCStyle>();
            var isAdded = false;
            foreach (var agent in head.GetElementsByTagName("ttm:agent").Cast<XmlElement>().ToList())
            {
                var name = agent.GetAttribute("xml:id");
                var style = new ALRCStyle()
                {
                    Id = name,
                    Position = isAdded ? ALRCStylePosition.Right : ALRCStylePosition.Left
                };
                var singers = agent.ChildNodes.OfType<XmlElement>().Select(t=>t.InnerText).ToArray();
                if (singers.Length > 0)
                {
                    style.Singers = singers.Select((t, i) => new ALRCStyleSinger()
                    {
                        Id = $"{name}_s{i}",
                        Name = t
                    }).ToList();
                }
                alrc.Header.Styles.Add(style);
                var bgStyle = new ALRCStyle()
                {
                    Id = name + "_bg",
                    Position = style.Position,
                    Type = ALRCStyleAccent.Background,
                    Singers = style.Singers
                };
                alrc.Header.Styles.Add(bgStyle);
                isAdded = true;
            }


            List<KeyValuePair<string, string>> pairs = [];
            var author = string.Empty;
            foreach (var metadata in head.GetElementsByTagName("amll:meta").Cast<XmlElement>().ToList())
            {
                var key = metadata.GetAttribute("key");
                var value = metadata.GetAttribute("value");
                pairs.Add(new KeyValuePair<string, string>(key, value));
                if (key == "ttmlAuthorGithubLogin")
                {
                    author = value;
                }
            }

            alrc.SongInfo = pairs.ToArray();

            // set author info
            if (!string.IsNullOrWhiteSpace(author))
            {
                alrc.LyricInfo = new ALRCLyricInfo
                {
                    Author = author
                };
            }

            if (FEATURE_ITUNESMETADATA_PARSE_EXP)
            {
                var itunesMetadata = head.GetElementsByTagName("metadata").OfType<XmlElement>().FirstOrDefault()
                    ?.GetElementsByTagName("iTunesMetadata").OfType<XmlElement>().FirstOrDefault();
                foreach (var xmlElement in itunesMetadata?.ChildNodes.OfType<XmlElement>() ?? [])
                {
                    if (xmlElement.Name == "translations")
                    {
                        translations = xmlElement.GetElementsByTagName("text").Cast<XmlElement>().ToDictionary(
                            t => t.GetAttribute("for"),
                            t => t.InnerText
                        );
                    }

                    if (xmlElement.Name == "transliterations")
                    {
                        transliterations = xmlElement.GetElementsByTagName("text").Cast<XmlElement>().ToDictionary(
                            t => t.GetAttribute("for"),
                            t => t.InnerText
                        );
                    }

                }
            }
        }

        foreach (var p in ttml.GetElementsByTagName("p").Cast<XmlElement>().ToList())
        {
            ParseElement(p, p.GetAttribute("ttm:agent"), false);
            continue;

            void ParseElement(XmlElement element, string style, bool isBackground, string? parentLineId = null)
            {
                var alrcLine = new ALRCLine();
                alrcLine.LineStyle = style + (isBackground ? "_bg" : string.Empty);
                var begin = element.GetAttribute("begin");
                var end = element.GetAttribute("end");
                alrcLine.Start = ParseTime(begin);
                alrcLine.End = ParseTime(end);
                alrcLine.ParentLineId = parentLineId;
                var lineKey = element.HasAttribute("itunes:key") ? element.GetAttribute("itunes:key") : null;
                alrcLine.Id = lineKey?.TrimStart('L');
                alrcLine.Translation = element.ChildNodes.OfType<XmlElement>().FirstOrDefault(t =>
                        t.HasAttribute("ttm:role") && t.GetAttribute("ttm:role") == "x-translation")
                    ?.InnerText;
                alrcLine.Transliteration = element.ChildNodes.OfType<XmlElement>()
                    .FirstOrDefault(t => t.HasAttribute("ttm:role") && t.GetAttribute("ttm:role") == "x-roman")
                    ?.InnerText;
                if (FEATURE_ITUNESMETADATA_PARSE_EXP)
                {
                    if (lineKey is not null)
                    {
                        if (translations?.TryGetValue(lineKey, out var tr) is true)
                        {
                            alrcLine.Translation = tr;
                        }

                        if (transliterations?.TryGetValue(lineKey, out var tl) is true)
                        {
                            alrcLine.Transliteration = tl;
                        }
                    }
                }

                alrcLine.Words = new List<ALRCWord>();
                var words = element.ChildNodes;
                var sb = new StringBuilder();
                ALRCWord? lastWord = null;
                var isStart = isBackground;
                var transliterationElement = Array.Empty<string>();
                if (FEATURE_TRANSLITERATION_PARSE_EXP)
                    transliterationElement = alrcLine.Transliteration?.Split([' '], StringSplitOptions.RemoveEmptyEntries) ?? [];
                var transliterationBeat = 0;
                foreach (var wordEle in words)
                {
                    if (wordEle is XmlElement span)
                    {
                        if (span.HasAttribute("ttm:role")) continue;
                        var word = new ALRCWord
                        {
                            Word = isStart ? span.InnerText.TrimStart('(') : span.InnerText
                        };
                        isStart = false;
                        lastWord = word;
                        var wordBegin = span.GetAttribute("begin");
                        var wordEnd = span.GetAttribute("end");


                        if (TimeSpan.TryParseExact(wordBegin, @"mm\:ss\.fff", null, out var start))
                            word.Start = (int)start.TotalMilliseconds;
                        if (TimeSpan.TryParseExact(wordEnd, @"mm\:ss\.fff", null, out var e))
                            word.End = (int)e.TotalMilliseconds;
                        alrcLine.Words.Add(word);
                        sb.Append(word.Word);

                        if (FEATURE_TRANSLITERATION_PARSE_EXP)
                        {
                            // 这个地方完全不太规范, 曾尝试解析, 但是歌词太杂了, 干不动!
                            // append the transliteration
                            var aggregate = word.Word.Trim().Length;
                            if (span.HasAttribute("amll:empty-beat"))
                            {
                                aggregate = int.Parse(span.GetAttribute("amll:empty-beat")) + 1;
                            }

                            if (transliterationBeat + aggregate <= transliterationElement.Length)
                            {
                                var wordTransliteration = string.Join(" ",
                                    transliterationElement.Skip(transliterationBeat).Take(aggregate));
                                word.Transliteration = wordTransliteration;
                            }

                            transliterationBeat += aggregate;
                        }
                    }

                    if (wordEle is XmlText textEle)
                    {
                        lastWord?.Word += textEle.InnerText;
                        sb.Append(isStart ? textEle.InnerText.TrimStart('(') : textEle.InnerText);
                    }

                    if (wordEle is XmlWhitespace wsEle)
                    {
                        lastWord?.Word += wsEle.InnerText;
                        sb.Append(wsEle.InnerText);
                    }
                }

                if (isBackground && lastWord is not null)
                    lastWord.Word = lastWord.Word.TrimEnd(')');
                if (!isBackground)
                {
                    alrcLine.RawText = sb.ToString();
                }
                else
                {
                    alrcLine.RawText = sb.Remove(sb.Length - 1, 1).ToString();
                }

                lines.Add(alrcLine);

                var subLineElements = element.ChildNodes.OfType<XmlElement>()
                    .Where(t => t.HasAttribute("ttm:role") && t.GetAttribute("ttm:role") == "x-bg").ToList();
                if (subLineElements is { Count: > 0 })
                {
                    foreach (var subLineElement in subLineElements)
                    {
                        ParseElement(subLineElement, style, true, alrcLine.Id);
                    }
                }
            }
        }

        return alrc;
    }

    private static long ParseTime(ReadOnlySpan<char> span)
    {
        long totalSeconds = 0;
        long currentVal = 0;
        bool isFraction = false;
        int fracDigits = 0;
        
        foreach (var c in span)
        {
            switch (c)
            {
                case >= '0' and <= '9':
                {
                    currentVal = currentVal * 10 + (c - '0');
                    if (isFraction) fracDigits++;
                    break;
                }
                case ':':
                    totalSeconds = (totalSeconds * 60) + currentVal;
                    currentVal = 0;
                    break;
                case '.':
                    totalSeconds = (totalSeconds * 60) + currentVal;
                    currentVal = 0;
                    isFraction = true;
                    break;
            }
        }

        if (!isFraction)
        {
            totalSeconds = (totalSeconds * 60) + currentVal;
            return totalSeconds * 1000;
        }

        var milliseconds = currentVal;
        switch (fracDigits)
        {
            case 1:
                milliseconds *= 100;
                break;
            case 2:
                milliseconds *= 10;
                break;
            case > 3:
                break;
        }

        return (totalSeconds * 1000) + milliseconds;
    }
    
    public string ConvertBack(ALRCFile input)
    {
        var ttml = new XmlDocument();
        ttml.PreserveWhitespace = true;
        var root = ttml.CreateElement("tt");
        root.SetAttribute("xmlns", "http://www.w3.org/ns/ttml");
        root.SetAttribute("xmlns:ttm", "http://www.w3.org/ns/ttml#metadata");
        root.SetAttribute("xmlns:itunes", "http://music.apple.com/lyric-ttml-internal");
        root.SetAttribute("xmlns:amll", "http://www.example.com/ns/amll");
        if (!string.IsNullOrEmpty(input.LyricInfo?.Language))
            root.SetAttribute("xml:lang", input.LyricInfo?.Language);
        if (input.Lines.Any(t => t.Words is { Count: > 0 }))
        {
            root.SetAttribute("itunes:timing", "word");
        }
        else
        {
            root.SetAttribute("itunes:timing", "line");
        }

        ttml.AppendChild(root);

        var head = ttml.CreateElement("head");
        root.AppendChild(head);
        var metadata = ttml.CreateElement("metadata");
        head.AppendChild(metadata);
        var agentsA = ttml.CreateElement("ttm", "agent", "http://www.w3.org/ns/ttml#metadata");
        agentsA.SetAttribute("type", "person");
        agentsA.SetAttribute("xml:id", "v1");
        metadata.AppendChild(agentsA);
        var agentsB = ttml.CreateElement("ttm", "agent", "http://www.w3.org/ns/ttml#metadata");
        agentsB.SetAttribute("type", "other");
        agentsB.SetAttribute("xml:id", "v2");
        metadata.AppendChild(agentsB);
        var body = ttml.CreateElement("body");
        var totalDuration = TimeSpan.FromMilliseconds(input.LyricInfo?.Duration ?? input.Lines.Last().End ?? 0)
            .ToString(@"mm\:ss\.fff");
        body.SetAttribute("dur", totalDuration);
        root.AppendChild(body);
        var div = ttml.CreateElement("div");
        body.AppendChild(div);
        div.SetAttribute("begin", "00:00.000");
        div.SetAttribute("end", totalDuration);
        int key = 1;
        Dictionary<string, XmlElement> parentElements = new Dictionary<string, XmlElement>();
        foreach (var line in input.Lines)
        {
            // 为了适应 AMLL-TTML 的要求, 加上了这个
            if (string.IsNullOrWhiteSpace(line.RawText) && line.Words is not { Count: > 0 }) continue;
            
            bool isSubline = false;
            var parent = div;
            var lineElement = ttml.CreateElement("p");
            lineElement.SetAttribute("key", "http://music.apple.com/lyric-ttml-internal", $"L{key++}");
            lineElement.SetAttribute("agent", "http://www.w3.org/ns/ttml#metadata", "v1");
            if (input.Header?.Styles?.FirstOrDefault(t => t.Id == line.LineStyle) is { } alrcStyle)
            {
                // 存在样式
                if (alrcStyle.Type == ALRCStyleAccent.Background)
                {
                    lineElement = ttml.CreateElement("span");
                    lineElement.SetAttribute("role", "http://www.w3.org/ns/ttml#metadata", "x-bg");
                    if (string.IsNullOrEmpty(line.ParentLineId) || !parentElements.TryGetValue(line.ParentLineId!, out var parentElement))
                    {
                        parentElement = parent.LastChild as XmlElement;
                    }

                    parent = parentElement;
                    isSubline = true;
                }

                var agent = alrcStyle.Position switch
                {
                    ALRCStylePosition.Right => "v2",
                    _ => "v1"
                };
                lineElement.SetAttribute("agent", "http://www.w3.org/ns/ttml#metadata", agent);
            }

            parent!.AppendChild(lineElement);
            if (!string.IsNullOrWhiteSpace(line.Id))
            {
                parentElements[line.Id!] = lineElement;
            }

            lineElement.SetAttribute("begin", TimeSpan.FromMilliseconds(line.Start ?? 0).ToString(@"mm\:ss\.fff"));
            lineElement.SetAttribute("end", TimeSpan.FromMilliseconds(line.End ?? 0).ToString(@"mm\:ss\.fff"));
            if (!string.IsNullOrWhiteSpace(line.Transliteration))
            {
                var span = ttml.CreateElement("span");
                span.SetAttribute("role", "http://www.w3.org/ns/ttml#metadata",
                    "x-roman"); // romaji not roman, but AMLL use it.
                span.InnerText = line.Transliteration!;
                lineElement.AppendChild(span);
            }

            if (!string.IsNullOrWhiteSpace(line.Translation))
            {
                var span = ttml.CreateElement("span");
                span.SetAttribute("role", "http://www.w3.org/ns/ttml#metadata",
                    "x-translation");
                span.InnerText = line.Translation!;
                lineElement.AppendChild(span);
            }

            bool isFirst = true;
            if (line.Words is { Count: > 0 })
            {
                foreach (var word in line.Words)
                {
                    var trimmedWord = word.Word;
                    // check trim start
                    var delta = trimmedWord.Length - trimmedWord.TrimStart().Length;
                    for (var i = 0; i < delta; i++)
                    {
                        lineElement.AppendChild(ttml.CreateWhitespace(" "));
                    }
                    trimmedWord = trimmedWord.TrimStart();
                    
                    
                    
                    var span = ttml.CreateElement("span");
                    span.SetAttribute("begin", TimeSpan.FromMilliseconds(word.Start).ToString(@"mm\:ss\.fff"));
                    span.SetAttribute("end", TimeSpan.FromMilliseconds(word.End).ToString(@"mm\:ss\.fff"));
                    span.InnerText = isFirst && isSubline ? $"({trimmedWord.TrimEnd()}" : trimmedWord.TrimEnd();
                    lineElement.AppendChild(span);
                    // check trim end
                    delta = trimmedWord.Length - trimmedWord.TrimEnd(' ').Length;
                    for (var i = 0; i < delta; i++)
                    {
                        lineElement.AppendChild(ttml.CreateWhitespace(" "));
                    }
                    
                    isFirst = false;
                    var trArr = word.Transliteration?.Trim().Split(' ');
                    if (trArr is { Length: > 0 })
                    {
                        // add empty beat
                        var actualWordLength = word.Word.Trim().Length;
                        var emptyBeat = trArr.Length - actualWordLength;
                        if (emptyBeat > 0)
                        {
                            span.SetAttribute("empty-beat", "http://www.example.com/ns/amll", emptyBeat.ToString());
                        }
                    }
                }

                if (isSubline && lineElement.LastChild is not null)
                {
                    lineElement.LastChild.InnerText += ")";
                }
            }
            else
            {
                lineElement.InnerText = line.RawText ?? string.Empty;
            }
        }


        return ttml.InnerXml;
    }
}