using ALRC.Converters;
using FluentAssertions;

namespace ALRC.Tests;

public class LyricConvertToALRCTest
{
    [Theory]
    [DirectoryFileData("Case/qrc")]
    public void QrcToALRCTest(string lyric)
    {
        // Arrange
        var converter = new QQLyricConverter();
        // Act
        var result = converter.Convert(lyric);
        // Assert
        result.Should().NotBeNull();
        result.Lines.Should().NotBeEmpty();
    }
    
    [Theory]
    [DirectoryFileData("Case/lrc")]
    public void LrcToALRCTest(string lyric)
    {
        // Arrange
        var converter = new LrcConverter();
        // Act
        var result = converter.Convert(lyric);
        // Assert
        result.Should().NotBeNull();
        result.Lines.Should().NotBeEmpty();
    }
    
    [Theory]
    [DirectoryFileData("Case/yrc")]
    public void YrcToALRCTest(string lyric)
    {
        // Arrange
        var converter = new NeteaseYrcConverter();
        // Act
        var result = converter.Convert(lyric);
        // Assert
        result.Should().NotBeNull();
        result.Lines.Should().NotBeEmpty();
    }
    
    [Theory]
    [DirectoryFileData("Case/lyricify")]
    public void LyricifySyllableToALRCTest(string lyric)
    {
        // Arrange
        var converter = new LyricifySyllableConverter();
        // Act
        var result = converter.Convert(lyric);
        // Assert
        result.Should().NotBeNull();
        result.Lines.Should().NotBeEmpty();
    }
    
    [Theory]
    [DirectoryFileData("Case/apple")]
    public void TTMLToALRCTest(string lyric)
    {
        // Arrange
        var converter = new AppleSyllableConverter();
        // Act
        var result = converter.Convert(lyric);
        // Assert
        result.Should().NotBeNull();
        result.Lines.Should().NotBeEmpty();
    }
    
}