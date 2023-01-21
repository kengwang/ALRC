using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using ALRC.Creator.Models;
using ALRC.Creator.Models.ViewModels;
using Microsoft.Win32;

namespace ALRC.Creator.Views.Pages;

public partial class ExportPage : Page
{
    private readonly ExportPageViewModel _viewModel;


    public ExportPage(ExportPageViewModel viewModel)
    {
        DataContext = viewModel;
        _viewModel = viewModel;
        InitializeComponent();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var dialog = new SaveFileDialog();
        dialog.Filter = "ALRC 项目|*.ealrc";
        if (dialog.ShowDialog() is true)
        {
            File.WriteAllText(dialog.FileName, JsonSerializer.Serialize(_viewModel.Alrc, options));
        }
    }

    private void Import_Click(object sender, RoutedEventArgs e)
    {
        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var dialog = new OpenFileDialog();
        dialog.Filter = "ALRC 项目|*.ealrc";
        if (dialog.ShowDialog() is true)
        {
            var fileContent = File.ReadAllText(dialog.FileName);
            var file = JsonSerializer.Deserialize<EditingALRC>(fileContent, options)!;
            _viewModel.Alrc.Lines = file.Lines;
            _viewModel.Alrc.Styles = file.Styles;
            _viewModel.Alrc.LyricInfo = file.LyricInfo;
            _viewModel.Alrc.SongInfos = file.SongInfos;
        }
    }
}