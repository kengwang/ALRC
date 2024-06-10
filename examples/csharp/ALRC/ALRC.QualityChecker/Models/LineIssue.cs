using ALRC.Abstraction;

namespace ALRC.QualityChecker.Models;

public class LineIssue : Issue
{
    public required ALRCLine Line { get; set; }
}