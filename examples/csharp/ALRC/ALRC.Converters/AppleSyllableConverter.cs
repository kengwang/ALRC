using System.Text;
using System.Xml;
using ALRC.Abstraction;

namespace ALRC.Converters;

public class AppleSyllableConverter : ILyricConverter<string>
{
    // 部分遵循 AMLL 规范
    public ALRCFile Convert(string input)
    {
        var ttml = new XmlDocument();
        ttml.PreserveWhitespace = true;
        var lines = new List<ALRCLine>();
        var alrc = new ALRCFile
        {
            Schema = "https://github.com/kengwang/ALRC/blob/main/schemas/v1.json",
            LyricInfo = null,
            SongInfo = null,
            Header = null,
            Lines = lines
        };
        ttml.LoadXml(input);
        // parse header
        var head = ttml.GetElementsByTagName("head").Cast<XmlElement>().FirstOrDefault();
        if (head != null)
        {
            alrc.Header ??= new ALRCHeader();
            alrc.Header.Styles = new List<ALRCStyle>();
            bool isAdded = false;
            foreach (var agent in head.GetElementsByTagName("ttm:agent").Cast<XmlElement>().ToList())
            {
                var name = agent.GetAttribute("xml:id");
                alrc.Header.Styles.Add(new ALRCStyle()
                {
                    Id = name,
                    Position = isAdded ? ALRCStylePosition.Right : ALRCStylePosition.Left
                });
                alrc.Header.Styles.Add(new ALRCStyle()
                {
                    Id = name + "_bg",
                    Position = isAdded ? ALRCStylePosition.Right : ALRCStylePosition.Left,
                    Type = ALRCStyleAccent.Background,
                    HiddenOnBlur = true
                });
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
                alrc.LyricInfo = new()
                {
                    Author = author
                };
            }
        }

        foreach (var p in ttml.GetElementsByTagName("p").Cast<XmlElement>().ToList())
        {
            void ParseElement(XmlElement element, string style, bool isBackground)
            {
                var alrcLine = new ALRCLine();
                alrcLine.LineStyle = style + (isBackground ? "_bg" : string.Empty);
                var begin = element.GetAttribute("begin");
                var end = element.GetAttribute("end");
                if (TimeSpan.TryParseExact(begin, @"mm\:ss\.fff", null, out var startLine))
                    alrcLine.Start = (int)startLine.TotalMilliseconds;
                if (TimeSpan.TryParseExact(end, @"mm\:ss\.fff", null, out var endLine))
                    alrcLine.End = (int)endLine.TotalMilliseconds;
                alrcLine.Translation = element.GetElementsByTagName("span").Cast<XmlElement>()
                    .FirstOrDefault(t => t.HasAttribute("ttm:role") && t.GetAttribute("ttm:role") == "x-translation")
                    ?.InnerText;
                alrcLine.Transliteration = element.GetElementsByTagName("span").Cast<XmlElement>()
                    .FirstOrDefault(t => t.HasAttribute("ttm:role") && t.GetAttribute("ttm:role") == "x-roman")
                    ?.InnerText;
                alrcLine.Words = new List<ALRCWord>();
                var words = element.ChildNodes;
                var sb = new StringBuilder();
                ALRCWord? lastWord = null;
                var isStart = isBackground;
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
                    }

                    if (wordEle is XmlWhitespace wsEle)
                    {
                        if (lastWord != null) lastWord.Word += wsEle.InnerText;
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

                var subLineElements = element.GetElementsByTagName("span").Cast<XmlElement>()
                    .Where(t => t.HasAttribute("ttm:role") && t.GetAttribute("ttm:role") == "x-bg").ToList();
                if (subLineElements is { Count: > 0 })
                {
                    foreach (var subLineElement in subLineElements)
                    {
                        ParseElement(subLineElement, style, true);
                    }
                }
            }
            
            ParseElement(p, p.GetAttribute("ttm:agent"), false);
        }

        return alrc;
    }

    public string ConvertBack(ALRCFile input)
    {
        var ttml = new XmlDocument();
        ttml.PreserveWhitespace = true;
        var root = ttml.CreateElement("tt");
        root.SetAttribute("xmlns", "http://www.w3.org/ns/ttml");
        root.SetAttribute("xmlns:ttm", "http://www.w3.org/ns/ttml#metadata");
        root.SetAttribute("xmlns:itunes", "http://music.apple.com/lyric-ttml-internal");
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
                    parent = parentElements[line.ParentLineId!];
                    isSubline = true;
                }

                var agent = alrcStyle.Position switch
                {
                    ALRCStylePosition.Right => "v2",
                    _ => "v1"
                };
                lineElement.SetAttribute("agent", "http://www.w3.org/ns/ttml#metadata", agent);
            }

            parent.AppendChild(lineElement);
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
                    "x-translation"); // romaji not roman, but AMLL use it.
                span.InnerText = line.Translation!;
                lineElement.AppendChild(span);
            }

            bool isFirst = true;
            if (line.Words is { Count: > 0 })
            {
                foreach (var word in line.Words)
                {
                    var span = ttml.CreateElement("span");
                    span.SetAttribute("begin", TimeSpan.FromMilliseconds(word.Start).ToString(@"mm\:ss\.fff"));
                    span.SetAttribute("end", TimeSpan.FromMilliseconds(word.End).ToString(@"mm\:ss\.fff"));
                    span.InnerText = isFirst && isSubline ? $"({word.Word}" : word.Word;
                    lineElement.AppendChild(span);
                    isFirst = false;
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