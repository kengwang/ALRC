using ALRC.Abstraction;

namespace ALRC.QualityChecker.Models;

public class Issue
{
    public required string Message { get; set; }
    public required ALRCFile File { get; set; }
    public required string ResourceIdentifier { get; set; }
    public required IssueType IssueType { get; set; }
}

public enum IssueType
{
    Word,
    Line,
    Style
}