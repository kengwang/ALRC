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
        var converter = new LrcTranslationConverter();
        var trLine =
            alrc.Lines.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.LineTranslations?.FirstOrDefault().Key));
        string? lang = null;
        if (trLine is not null) lang = trLine.LineTranslations!.FirstOrDefault().Key!;
        else
        {
            MessageBox.Show("No Translation Founded");
            return;
        }
        var lrcText = converter.Convert(alrc, lang);
        var dialog = new SaveFileDialog();
        dialog.Filter = "LRC 文件|*.lrc";
        if (dialog.ShowDialog() is true)
        {
            File.WriteAllText(dialog.FileName, lrcText);
        }
    }
}