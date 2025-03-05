using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ALRC.Creator.Models.ViewModels;
using ALRC.QualityChecker.Models;

namespace ALRC.Creator.Views.Pages;

public partial class QualityCheckPage : Page
{
    private readonly QualityCheckPageViewModel _viewModel;

    public QualityCheckPage(QualityCheckPageViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = viewModel;
        InitializeComponent();
    }

    private void BtnStartScan_OnClick(object sender, RoutedEventArgs e)
    {
        _ = _viewModel.StartScan();
    }

    private void IssueListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        _viewModel.NavigateToCurrentIssue();
    }
}