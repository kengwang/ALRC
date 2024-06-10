using ALRC.Abstraction;

namespace ALRC.QualityChecker.Models;

public class Issue
{
    public required string Message { get; set; }
    public required ALRCFile File { get; set; }
}