using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ALRC.Abstraction;

namespace ALRC.QualityChecker.Models;

public abstract class QualityCheckerBase
{
    public abstract Task<List<Issue>> CheckAsync(ALRCFile file, CancellationToken token);
}