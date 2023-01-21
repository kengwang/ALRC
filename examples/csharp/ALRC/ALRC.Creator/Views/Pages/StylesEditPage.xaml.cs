using System.Windows;
using System.Windows.Controls;
using ALRC.Creator.Models;
using ALRC.Creator.Models.ViewModels;
using ALRC.Creator.Views.Controls;

namespace ALRC.Creator.Views.Pages;

public partial class StylesEditPage : Page
{
    private StylesPageViewModel _viewModel;

    public StylesEditPage(StylesPageViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = viewModel;
        InitializeComponent();
    }

    private void Btn_AddStyle_Click(object sender, RoutedEventArgs e)
    {
        var newStyle = new EditingALRCStyle();
        _viewModel.ALRC.Styles.Insert(StylesSelector.SelectedIndex + 1, newStyle);
        _viewModel.FocusingStyle = newStyle;
    }

    private void Btn_RemoveStyle_Click(object sender, RoutedEventArgs e)
    {
        if (StylesSelector.SelectedItem is EditingALRCStyle style)
            _viewModel.ALRC.Styles.Remove(style);
    }

    private void StylesSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (StylesSelector.SelectedItem is EditingALRCStyle style)
        {
            PreviewControlContainer.Content = new PreviewLineControl(new PreviewLineControlViewModel(_viewModel.ALRC,
                new EditingALRCLine
                {
                    LineStyle = style.Id,
                    Type = 1,
                    Text = "Preview Line",
                }));
        }
    }
}