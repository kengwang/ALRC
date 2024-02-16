using ALRC.Abstraction;
using ALRC.Converters;
using FluentAssertions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ALRC.Tests;

public class ALRCToLyricConvertTest
{
    public static List<object[]> _files = Directory.EnumerateFiles("Case/alrc", "*").Select(File.ReadAllText)
        .Select(t => new object[] { JsonSerializer.Deserialize<ALRCFile>(t)! }).ToList();

    [Theory]
    [MemberData(nameof(_files))]
    public void ALRCToQrcTest(ALRCFile lyric)
    {
        // Arrange
        var converter = new QQLyricConverter();
        // Act
        var result = converter.ConvertBack(lyric);
        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }
    
    [Theory]
    [MemberData(nameof(_files))]
    public void ALRCToLrcTest(ALRCFile lyric)
    {
        // Arrange
        var converter = new LrcConverter();
        // Act
        var result = converter.ConvertBack(lyric);
        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }
    
    [Theory]
    [MemberData(nameof(_files))]
    public void ALRCToYrcTest(ALRCFile lyric)
    {
        // Arrange
        var converter = new NeteaseYrcConverter();
        // Act
        var result = converter.ConvertBack(lyric);
        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }
    
    [Theory]
    [MemberData(nameof(_files))]
    public void ALRCToLyricifySyllableTest(ALRCFile lyric)
    {
        // Arrange
        var converter = new LyricifySyllableConverter();
        // Act
        var result = converter.ConvertBack(lyric);
        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }
    
    [Theory]
    [MemberData(nameof(_files))]
    public void ALRCToAppleSyllableTest(ALRCFile lyric)
    {
        // Arrange
        var converter = new AppleSyllableConverter();
        // Act
        var result = converter.ConvertBack(lyric);
        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }
}