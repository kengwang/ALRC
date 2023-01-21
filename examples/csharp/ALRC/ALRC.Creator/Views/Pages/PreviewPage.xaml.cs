using System;
using System.Linq;
using System.Windows.Controls;
using ALRC.Creator.Models.ViewModels;
using ALRC.Creator.Views.Controls;

namespace ALRC.Creator.Views.Pages;

public partial class PreviewPage : Page
{
    private readonly PreviewPageViewModel _viewModel;

    public PreviewPage(PreviewPageViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = viewModel;
        InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        _viewModel.MusicPlayer.Timer.Tick += TimerOnTick;
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        foreach (var line in _viewModel.Alrc.Lines)
        {
            if (line.Start <= _viewModel.MusicPlayer.CurrentTimeInMilliseconds &&
                line.End > _viewModel.MusicPlayer.CurrentTimeInMilliseconds)
            {
                var lineControl = _viewModel.PreviewingLines.FirstOrDefault(t => t.ViewModel.Line == line);
                if (lineControl is {})
                {
                    lineControl.RefreshTime(_viewModel.MusicPlayer.CurrentTimeInMilliseconds);
                    continue;
                }
                _viewModel.PreviewingLines.Add(
                    new PreviewLineControl(
                        new PreviewLineControlViewModel(
                            _viewModel.Alrc, line))
                );
            }
            else
            {
                if (_viewModel.PreviewingLines.FirstOrDefault(l => l.ViewModel.Line == line) is { } c)
                {
                    _viewModel.PreviewingLines.Remove(c);
                }

            }
        }
    }
}