using ALRC.Abstraction;

namespace ALRC.QualityChecker.Models;

public class LineIssue : Issue
{
    public int LineIndex { get; set; }
    public required ALRCLine Line { get; set; }
}