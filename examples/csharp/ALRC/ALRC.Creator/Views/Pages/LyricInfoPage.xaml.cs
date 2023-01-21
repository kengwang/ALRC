using System.Windows;
using System.Windows.Controls;
using ALRC.Creator.Models;
using ALRC.Creator.Models.ViewModels;

namespace ALRC.Creator.Views.Pages;

public partial class LyricInfoPage : Page
{
    private readonly LyricInfoPageViewModel _viewModel;

    public LyricInfoPage(LyricInfoPageViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = _viewModel;
        InitializeComponent();
    }
}