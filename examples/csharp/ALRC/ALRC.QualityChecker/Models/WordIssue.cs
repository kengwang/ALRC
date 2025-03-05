using ALRC.Abstraction;

namespace ALRC.QualityChecker.Models;

public class WordIssue : LineIssue
{
    public required int WordIndex { get; set; }
    public required ALRCWord Word { get; set; }
}