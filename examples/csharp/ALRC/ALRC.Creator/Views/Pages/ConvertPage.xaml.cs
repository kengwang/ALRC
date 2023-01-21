using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ALRC.Abstraction;
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
}