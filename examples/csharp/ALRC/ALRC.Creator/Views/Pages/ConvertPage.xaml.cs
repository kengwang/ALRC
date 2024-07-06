using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ALRC.Abstraction;
using ALRC.Converters;
using ALRC.Creator.Models;
using ALRC.Creator.Models.ViewModels;
using Microsoft.Win32;

namespace ALRC.Creator.Views.Pages;

public partial class ConvertPage : Page
{
    private readonly ConvertPageViewModel _viewModel;

    public ConvertPage(ConvertPageViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
        _viewModel = viewModel;
    }

    private void ConvertToALRC_Click(object sender, RoutedEventArgs e)
    {
        var converter = new EditableALRCConverter();
        var alrc = converter.Convert(_viewModel.Alrc);
        var jsonOption = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };
        var result = JsonSerializer.Serialize(alrc, jsonOption);
        var dialog = new SaveFileDialog();
        dialog.Filter = "ALRC 文件|*.alrc";
        if (dialog.ShowDialog() is true)
        {
            File.WriteAllText(dialog.FileName, result);
        }
    }

    private void ConvertFromALRC_Click(object sender, MouseButtonEventArgs e)
    {
        var converter = new EditableALRCConverter();
        var dialog = new OpenFileDialog();
        dialog.Filter = "ALRC 文件|*.alrc";
        if (dialog.ShowDialog() is true)
        {
            var alrcText = File.ReadAllText(dialog.FileName);
            var alrc = JsonSerializer.Deserialize<ALRCFile>(alrcText);
        }
    }

    private void QQLyric_Convert_Click(object sender, RoutedEventArgs e)
    {
        var alrc = new EditableALRCConverter().Convert(_viewModel.Alrc);
        var converter = new QQLyricConverter();
        var qlrcText = converter.ConvertBack(alrc);
        var dialog = new SaveFileDialog();
        dialog.Filter = "QRC 文件|*.qrc";
        if (dialog.ShowDialog() is true)
        {
            File.WriteAllText(dialog.FileName, qlrcText);
        }
    }

    private void ConvertToLrc_Click(object sender, RoutedEventArgs e)
    {
        var alrc = new EditableALRCConverter().Convert(_viewModel.Alrc);
        var converter = new LrcConverter();
        var lrcText = converter.ConvertBack(alrc);
        var dialog = new SaveFileDialog();
        dialog.Filter = "LRC 文件|*.lrc";
        if (dialog.ShowDialog() is true)
        {
            File.WriteAllText(dialog.FileName, lrcText);
        }
    }

    private void ConvertTranslationToLrc_Click(object sender, RoutedEventArgs e)
    {
        var alrc = new EditableALRCConverter().Convert(_viewModel.Alrc);
        var converter = new LrcTranslationEnhancer();
        var trLine =
            alrc.Lines.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.Translation));

        var lrcText = converter.Enhance(alrc);
        var dialog = new SaveFileDialog();
        dialog.Filter = "LRC 文件|*.lrc";
        if (dialog.ShowDialog() is true)
        {
            File.WriteAllText(dialog.FileName, lrcText);
        }
    }

    private void ImportFromLrc_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.Filter = "LRC 文件|*.lrc";
        if (dialog.ShowDialog() is true)
        {
            var lrc = File.ReadAllText(dialog.FileName);
            var converter = new LrcConverter();
            var alrcFile = converter.Convert(lrc);
            var result = new EditableALRCConverter().ConvertBack(alrcFile);
            _viewModel.Alrc.Lines = result.Lines;
            _viewModel.Alrc.Styles = result.Styles;
            _viewModel.Alrc.SongInfos = result.SongInfos;
            _viewModel.Alrc.LyricInfo = result.LyricInfo;
        }
    }

    private void ConvertFromTTML_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.Filter = "TTML 文件|*.ttml";
        if (dialog.ShowDialog() is true)
        {
            var lrc = File.ReadAllText(dialog.FileName);
            var converter = new AppleSyllableConverter();
            var alrcFile = converter.Convert(lrc);
            var result = new EditableALRCConverter().ConvertBack(alrcFile);
            _viewModel.Alrc.Lines = result.Lines;
            _viewModel.Alrc.Styles = result.Styles;
            _viewModel.Alrc.SongInfos = result.SongInfos;
            _viewModel.Alrc.LyricInfo = result.LyricInfo;
        }
    }

    private void ConvertFromLyricifySyllable_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.Filter = "Lyricify Syllable 文件|*.txt";
        if (dialog.ShowDialog() is true)
        {
            var lrc = File.ReadAllText(dialog.FileName);
            var converter = new LyricifySyllableConverter();
            var alrcFile = converter.Convert(lrc);
            var result = new EditableALRCConverter().ConvertBack(alrcFile);
            _viewModel.Alrc.Lines = result.Lines;
            _viewModel.Alrc.Styles = result.Styles;
            _viewModel.Alrc.SongInfos = result.SongInfos;
            _viewModel.Alrc.LyricInfo = result.LyricInfo;
        }
    }

    private void ConverFromALRC_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.Filter = "ALRC 文件|*.alrc";
        if (dialog.ShowDialog() is true)
        {
            var lrc = File.ReadAllText(dialog.FileName);
            var alrcFile = JsonSerializer.Deserialize<ALRCFile>(lrc);
            if (alrcFile is null)
            {
                MessageBox.Show("Invalid ALRC File");
                return;
            }
            var result = new EditableALRCConverter().ConvertBack(alrcFile);
            _viewModel.Alrc.Lines = result.Lines;
            _viewModel.Alrc.Styles = result.Styles;
            _viewModel.Alrc.SongInfos = result.SongInfos;
            _viewModel.Alrc.LyricInfo = result.LyricInfo;
        }
    }

    private void ConvertFromQRC_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.Filter = "QRC 文件|*.qrc";
        if (dialog.ShowDialog() is true)
        {
            var lrc = File.ReadAllText(dialog.FileName);
            var converter = new QQLyricConverter();
            var alrcFile = converter.Convert(lrc);
            var result = new EditableALRCConverter().ConvertBack(alrcFile);
            _viewModel.Alrc.Lines = result.Lines;
            _viewModel.Alrc.Styles = result.Styles;
            _viewModel.Alrc.SongInfos = result.SongInfos;
            _viewModel.Alrc.LyricInfo = result.LyricInfo;
        }
    }

    private void ConvertToYRC_Click(object sender, RoutedEventArgs e)
    {
        var alrc = new EditableALRCConverter().Convert(_viewModel.Alrc);
        var converter = new NeteaseYrcConverter();
        var lrcText = converter.ConvertBack(alrc);
        var dialog = new SaveFileDialog();
        dialog.Filter = "YRC 文件|*.yrc";
        if (dialog.ShowDialog() is true)
        {
            File.WriteAllText(dialog.FileName, lrcText);
        }
    }

    private void ConvertFromYrc_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.Filter = "YRC 文件|*.yrc";
        if (dialog.ShowDialog() is true)
        {
            var lrc = File.ReadAllText(dialog.FileName);
            var converter = new NeteaseYrcConverter();
            var alrcFile = converter.Convert(lrc);
            var result = new EditableALRCConverter().ConvertBack(alrcFile);
            _viewModel.Alrc.Lines = result.Lines;
            _viewModel.Alrc.Styles = result.Styles;
            _viewModel.Alrc.SongInfos = result.SongInfos;
            _viewModel.Alrc.LyricInfo = result.LyricInfo;
        }
    }

    private void ConvertToTTML_Click(object sender, RoutedEventArgs e)
    {
        var alrc = new EditableALRCConverter().Convert(_viewModel.Alrc);
        var converter = new AppleSyllableConverter();
        var lrcText = converter.ConvertBack(alrc);
        var dialog = new SaveFileDialog();
        dialog.Filter = "TTML 文件|*.ttml";
        if (dialog.ShowDialog() is true)
        {
            File.WriteAllText(dialog.FileName, lrcText);
        }
    }

    private void ConvertToLyricifySyllable_Click(object sender, RoutedEventArgs e)
    {   
        var alrc = new EditableALRCConverter().Convert(_viewModel.Alrc);
        var converter = new LyricifySyllableConverter();
        var lrcText = converter.ConvertBack(alrc);
        var dialog = new SaveFileDialog();
        dialog.Filter = "Lyricify Syllable 文件|*.txt";
        if (dialog.ShowDialog() is true)
        {
            File.WriteAllText(dialog.FileName, lrcText);
        }
    }

    private void ConvertFromSRT_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();
        dialog.Filter = "SRT 文件|*.srt";
        if (dialog.ShowDialog() is true)
        {
            var lrc = File.ReadAllText(dialog.FileName);
            var converter = new SrtConverter();
            var alrcFile = converter.Convert(lrc);
            var result = new EditableALRCConverter().ConvertBack(alrcFile);
            _viewModel.Alrc.Lines = result.Lines;
            _viewModel.Alrc.Styles = result.Styles;
            _viewModel.Alrc.SongInfos = result.SongInfos;
            _viewModel.Alrc.LyricInfo = result.LyricInfo;
        }
    }

    private void ConvertToSRT_Click(object sender, RoutedEventArgs e)
    {
        var alrc = new EditableALRCConverter().Convert(_viewModel.Alrc);
        var converter = new SrtConverter();
        var lrcText = converter.ConvertBack(alrc);
        var dialog = new SaveFileDialog();
        dialog.Filter = "SRT 文件|*.srt";
        if (dialog.ShowDialog() is true)
        {
            File.WriteAllText(dialog.FileName, lrcText);
        }
    }
}