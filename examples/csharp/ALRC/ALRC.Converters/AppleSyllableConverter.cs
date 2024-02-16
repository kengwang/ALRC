using System.Text;
using System.Xml;
using ALRC.Abstraction;

namespace ALRC.Converters;

public class AppleSyllableConverter : ILyricConverter<string>
{
    public ALRCFile Convert(string input)
    {
        var ttml = new XmlDocument();
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
        }

        foreach (var p in ttml.GetElementsByTagName("p").Cast<XmlElement>().ToList())
        {
            var alrcLine = new ALRCLine();
            alrcLine.LineStyle = p.GetAttribute("ttm:agent");
            var begin = p.GetAttribute("begin");
            var end = p.GetAttribute("end");
            if (TimeSpan.TryParseExact(begin, @"mm\:ss\.fff", null, out var startLine))
                alrcLine.Start = (int)startLine.TotalMilliseconds;
            if (TimeSpan.TryParseExact(end, @"mm\:ss\.fff", null, out var endLine))
                alrcLine.End = (int)endLine.TotalMilliseconds;
            alrcLine.Words = new List<ALRCWord>();
            var words = p.ChildNodes.Cast<XmlElement>().Where(t => t.Name == "span" && !t.HasAttribute("ttm:role"))
                .ToList();
            var sb = new StringBuilder();
            foreach (var span in words)
            {
                var word = new ALRCWord
                {
                    Word = span.InnerText
                };
                var wordBegin = span.GetAttribute("begin");
                var wordEnd = span.GetAttribute("end");
                if (span.HasAttribute(""))
                {
                }

                if (TimeSpan.TryParseExact(wordBegin, @"mm\:ss\.fff", null, out var start))
                    word.Start = (int)start.TotalMilliseconds;
                if (TimeSpan.TryParseExact(wordEnd, @"mm\:ss\.fff", null, out var e))
                    word.End = (int)e.TotalMilliseconds;
                alrcLine.Words.Add(word);
                sb.Append(word.Word);
            }

            alrcLine.RawText = sb.ToString();
            lines.Add(alrcLine);
            var subLineElements = p.GetElementsByTagName("span").Cast<XmlElement>()
                .Where(t => t.HasAttribute("ttm:role")).ToList();
            if (subLineElements is { Count: > 0 })
                foreach (var subLineElement in subLineElements)
                {
                    var bgalrcLine = new ALRCLine();
                    bgalrcLine.LineStyle = p.GetAttribute("ttm:agent") + "_bg";
                    begin = subLineElement.GetAttribute("begin");
                    end = subLineElement.GetAttribute("end");
                    if (TimeSpan.TryParseExact(begin, @"mm\:ss\.fff", null, out startLine))
                        bgalrcLine.Start = (int)startLine.TotalMilliseconds;
                    if (TimeSpan.TryParseExact(end, @"mm\:ss\.fff", null, out endLine))
                        bgalrcLine.End = (int)endLine.TotalMilliseconds;
                    var bgWords = new List<ALRCWord>();
                    bgalrcLine.Words = bgWords;
                    var bgSb = new StringBuilder();
                    var subWords = subLineElement.GetElementsByTagName("span").Cast<XmlElement>()
                        .Where(t => !t.HasAttribute("ttm:role"))
                        .ToList();
                    var isStart = true;
                    foreach (var span in subWords)
                    {
                        var word = new ALRCWord
                        {
                            Word = isStart ? span.InnerText.TrimStart('(') : span.InnerText
                        };
                        isStart = false;
                        var wordBegin = span.GetAttribute("begin");
                        var wordEnd = span.GetAttribute("end");
                        if (TimeSpan.TryParseExact($"00:{wordBegin}", @"mm\:ss\.fff", null, out var start))
                            word.Start = (int)start.TotalMilliseconds;
                        if (TimeSpan.TryParseExact(wordEnd, @"mm\:ss\.fff", null, out var e))
                            word.End = (int)e.TotalMilliseconds;
                        bgWords.Add(word);
                        bgSb.Append(word.Word);
                    }

                    if (bgWords.LastOrDefault() is { } last)
                        last.Word = last.Word.TrimEnd(')');
                    bgalrcLine.RawText = bgSb.Remove(bgSb.Length - 1, 1).ToString();
                    lines.Add(bgalrcLine);
                }
        }

        return alrc;
    }

    public string ConvertBack(ALRCFile input)
    {
        var ttml = new XmlDocument();
        var root = ttml.CreateElement("tt");
        root.SetAttribute("xmlns", "http://www.w3.org/ns/ttml");
        root.SetAttribute("xmlns:ttm", "http://www.w3.org/ns/ttml#metadata");
        root.SetAttribute("xmlns:itunes", "http://music.apple.com/lyric-ttml-internal");
        ttml.AppendChild(root);

        var head = ttml.CreateElement("head");
        root.AppendChild(head);
        var metadata = ttml.CreateElement("metadata");
        head.AppendChild(metadata);
        var agentsA = ttml.CreateElement("ttm", "agent","http://www.w3.org/ns/ttml#metadata");
        agentsA.SetAttribute("type", "person");
        agentsA.SetAttribute("xml:id", "v1");
        metadata.AppendChild(agentsA);
        var agentsB = ttml.CreateElement("ttm", "agent","http://www.w3.org/ns/ttml#metadata");
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
            bool isSubline = false;
            var parent = div;
            var lineElement = ttml.CreateElement("p");
            lineElement.SetAttribute("key","http://music.apple.com/lyric-ttml-internal" ,$"L{key++}");
            lineElement.SetAttribute("agent","http://www.w3.org/ns/ttml#metadata", "v1");
            if (input.Header?.Styles?.FirstOrDefault(t => t.Id == line.LineStyle) is {  } alrcStyle)
            {
                // 存在样式
                if (alrcStyle.Type == ALRCStyleAccent.Background)
                {
                    lineElement = ttml.CreateElement("span");
                    lineElement.SetAttribute("role","http://www.w3.org/ns/ttml#metadata", "x-bg");
                    parent = parentElements[line.ParentLineId!];
                    isSubline = true;
                }

                var agent = alrcStyle.Position switch
                {
                    ALRCStylePosition.Right => "v2",
                    _ => "v1"
                };
                lineElement.SetAttribute("agent","http://www.w3.org/ns/ttml#metadata", agent);

            }

            parent.AppendChild(lineElement);
            if (!string.IsNullOrWhiteSpace(line.Id))
            {
                parentElements[line.Id] = lineElement;
            }
            lineElement.SetAttribute("begin", TimeSpan.FromMilliseconds(line.Start??0).ToString(@"mm\:ss\.fff"));
            lineElement.SetAttribute("end", TimeSpan.FromMilliseconds(line.End??0).ToString(@"mm\:ss\.fff"));
            bool isFirst = true;
            if (line.Words is not null)
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