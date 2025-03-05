using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ALRC.Abstraction;
using ALRC.QualityChecker.Models;

namespace ALRC.QualityChecker.Checkers;

public class TimeMinMaxQualityChecker : QualityCheckerBase
{
    public override async Task<List<Issue>> CheckAsync(ALRCFile file, CancellationToken token = default)
    {
        var issues = new List<Issue>();
        for (var lineIndex = 0; lineIndex < file.Lines.Count; lineIndex++)
        {
            var alrcLine = file.Lines[lineIndex];
            if (token.IsCancellationRequested)
            {
                break;
            }

            if (alrcLine.Start > alrcLine.End)
            {
                issues.Add(
                    new LineIssue
                    {
                        IssueType = IssueType.Line,
                        LineIndex = lineIndex,
                        ResourceIdentifier = $"L{lineIndex}",
                        Message = "Start time is greater than end time",
                        File = file,
                        Line = alrcLine
                    }
                );
            }

            if (alrcLine.Words is { Count: > 0 })
            {
                for (var wordIndex = 0; wordIndex < alrcLine.Words.Count; wordIndex++)
                {
                    var alrcWord = alrcLine.Words[wordIndex];
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (alrcWord.Start > alrcWord.End)
                    {
                        issues.Add(
                            new WordIssue
                            {
                                IssueType = IssueType.Word,
                                LineIndex = lineIndex,
                                Message = "Start time is greater than end time",
                                File = file,
                                ResourceIdentifier = $"L{lineIndex},W{wordIndex}",
                                Line = alrcLine,
                                WordIndex = wordIndex,
                                Word = alrcWord
                            }
                        );
                    }

                    if (alrcWord.Start < alrcLine.Start || alrcWord.End > alrcLine.End)
                    {
                        issues.Add(
                            new WordIssue
                            {
                                IssueType = IssueType.Word,
                                LineIndex = lineIndex,
                                ResourceIdentifier = $"L{lineIndex},W{wordIndex}",
                                WordIndex = wordIndex,
                                Message = "Word time is outside of line time",
                                File = file,
                                Line = alrcLine,
                                Word = alrcWord
                            }
                        );
                    }
                }
            }
        }

        return issues;
    }
}