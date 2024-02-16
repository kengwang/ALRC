using System.Reflection;
using Xunit.Sdk;

namespace ALRC.Tests;

public class DirectoryFileDataAttribute(string path, string pattern = "*") : DataAttribute
{
    private readonly List<string> _contents = Directory.GetFiles(path, pattern).Select(File.ReadAllText).ToList();

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        return _contents.Select(t => new object[] { t });
    }
}