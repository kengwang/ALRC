using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ALRC.Creator.Models;
using ALRC.Creator.Models.ViewModels;

namespace ALRC.Creator.Views.Pages;

public partial class LinesEditPage : Page
{
    private readonly LinesEditPageViewModel _viewModel;

    public LinesEditPage(LinesEditPageViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = viewModel;
        InitializeComponent();
    }

    private void Btn_AddLine_Click(object sender, RoutedEventArgs e)
    {
        var newLine = new EditingALRCLine();
        _viewModel.EditingAlrc.Lines.Insert(LineSelector.SelectedIndex + 1, newLine);
        _viewModel.FocusingLine = newLine;
    }

    private void Btn_RemoveLine_Click(object sender, RoutedEventArgs e)
    {
        if (LineSelector.SelectedItem is EditingALRCLine line)
            _viewModel.EditingAlrc.Lines.Remove(line);
    }

    private void LineSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LineSelector.SelectedItem is EditingALRCLine line)
        {
            LineInfoEditor.Visibility = Visibility.Visible;
            _viewModel.FocusingLine = line;
        }
        else
        {
            LineInfoEditor.Visibility = Visibility.Collapsed;
        }
    }

    private void EasyTimelineEditorTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.NumPad5:
            case Key.S:
                ChangePlayStatus();
                break;
            case Key.NumPad4:
            case Key.A:
                SetStartPoint();
                break;
            case Key.NumPad6:
            case Key.D:
                SetEndPoint();
                break;
            case Key.NumPad1:
            case Key.Q:
                SeekBackward();
                break;
            case Key.NumPad3:
            case Key.E:
                SeekForward();
                break;
            case Key.NumPad8:
            case Key.LeftShift:
                FocusPreviousLine();
                break;
            case Key.NumPad2:
            case Key.LeftCtrl:
                FocusNextLine();
                break;
            case Key.Enter:
            case Key.Space:
                SetEndPoint();
                FocusNextLine();
                SetStartPoint();
                break;
            case Key.RightShift:
                return;
        }

        e.Handled = true;
    }

    private void FocusNextLine()
    {
        if (LineSelector.SelectedIndex < _viewModel.EditingAlrc.Lines.Count)
            LineSelector.SelectedIndex += 1;
        LineSelector.ScrollIntoView(LineSelector.SelectedItem);
    }

    private void FocusPreviousLine()
    {
        if (LineSelector.SelectedIndex > 0)
            LineSelector.SelectedIndex -= 1;
        LineSelector.ScrollIntoView(LineSelector.SelectedItem);
    }

    private void SeekForward()
    {
        if (_viewModel.MusicPlayer.TotalTimeInMilliseconds >
            5000 + _viewModel.MusicPlayer.CurrentTimeInMilliseconds)
            _viewModel.MusicPlayer.Seek(
                TimeSpan.FromMilliseconds(_viewModel.MusicPlayer.CurrentTimeInMilliseconds + 5000));
    }

    private void SeekBackward()
    {
        if (_viewModel.MusicPlayer.CurrentTimeInMilliseconds > 5000)
            _viewModel.MusicPlayer.Seek(
                TimeSpan.FromMilliseconds(_viewModel.MusicPlayer.CurrentTimeInMilliseconds - 5000));
        else
            _viewModel.MusicPlayer.Seek(TimeSpan.Zero);
    }

    private void SetEndPoint()
    {
        _viewModel.FocusingLine.End = (int)_viewModel.MusicPlayer.CurrentTimeInMilliseconds;
    }

    private void SetStartPoint()
    {
        _viewModel.FocusingLine.Start = (int)_viewModel.MusicPlayer.CurrentTimeInMilliseconds;
    }

    private void ChangePlayStatus()
    {
        if (_viewModel.MusicPlayer.IsPlaying) _viewModel.MusicPlayer.Pause();
        else _viewModel.MusicPlayer.Play();
        return;
    }

    private void Btn_FastInsertLine_Click(object sender, RoutedEventArgs e)
    {
        if (QuickInputBox.Visibility == Visibility.Visible && !string.IsNullOrWhiteSpace(_viewModel.QuickInputTexts))
        {
            _viewModel.EditingAlrc.Lines.Clear();
            string? lang = null;
            if (_viewModel.QuickInputTexts.StartsWith("[lang:"))
            {

                lang = _viewModel.QuickInputTexts.Substring(6, _viewModel.QuickInputTexts.IndexOf(']') - 6);
            }

            var convLines = _viewModel.QuickInputTexts.Replace("\r\n", "\n").Replace("\r", "\n");
            var lines = convLines.Split('\n');
            foreach (var line in lines)
            {
                var alrcLine = new EditingALRCLine
                {
                    Type = string.IsNullOrWhiteSpace(line) ? 2 : 0
                };
                if (!string.IsNullOrWhiteSpace(lang) && line.Contains("[["))
                {
                    alrcLine.Text = line.Substring(0, line.IndexOf("[[", StringComparison.Ordinal));
                    var translation = line.Substring(line.IndexOf("[[", StringComparison.Ordinal) + 2,
                        line.IndexOf("]]", StringComparison.Ordinal) - line.IndexOf("[[", StringComparison.Ordinal) -2);
                    alrcLine.Translations.Add(new EditingALRCTranslation
                    {
                        LanguageTag = lang,
                        TranslationText = translation
                    });
                }
                else
                {
                    alrcLine.Text = line;
                }
               
                _viewModel.EditingAlrc.Lines.Add(alrcLine);
            }
        }

        QuickInputBox.Visibility =
            QuickInputBox.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        LineSelector.Visibility =
            LineSelector.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!LineSelector.SelectedItems.Contains(_viewModel.FocusingLine) || LineSelector.SelectedItems.Count <= 0 || _viewModel.IsPreviewMode) return;
        foreach (var editingAlrcLine in LineSelector.SelectedItems.Cast<EditingALRCLine>().ToList())
        {
            editingAlrcLine.Type = TypeSelector.SelectedIndex;
        }
    }

    private void PreviewMode_Checked(object sender, RoutedEventArgs e)
    {
        _viewModel.MusicPlayer.Timer.Tick += OnPlayPositionChanged;
    }

    private void OnPlayPositionChanged(object? sender, EventArgs e)
    {
        if (!_viewModel.IsPreviewMode) return;
        foreach (var editingAlrcLine in _viewModel.EditingAlrc.Lines)
        {
            if (editingAlrcLine.Start < _viewModel.MusicPlayer.CurrentTimeInMilliseconds &&
                editingAlrcLine.End > _viewModel.MusicPlayer.CurrentTimeInMilliseconds)
            {
                if (!LineSelector.SelectedItems.Contains(editingAlrcLine))
                    LineSelector.SelectedItems.Add(editingAlrcLine);
            }
            else
            {
                if (LineSelector.SelectedItems.Contains(editingAlrcLine))
                    LineSelector.SelectedItems.Remove(editingAlrcLine);
            }
        }
    }

    private void PreviewMode_Unchecked(object sender, RoutedEventArgs e)
    {
        _viewModel.MusicPlayer.Timer.Tick -= OnPlayPositionChanged;
    }

    private void StyleBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (LineSelector.SelectedItems.Count <= 1 || _viewModel.IsPreviewMode) return;
        foreach (var editingAlrcLine in LineSelector.SelectedItems.Cast<EditingALRCLine>().ToList())
        {
            editingAlrcLine.LineStyle = StyleBox.Text;
        }
    }

    private void Btn_SeekBackward_Click(object sender, RoutedEventArgs e)
    {
        SeekBackward();
    }

    private void Btn_SeekForward_Click(object sender, RoutedEventArgs e)
    {
        SeekForward();
    }

    private void Btn_PreviousLine_Click(object sender, RoutedEventArgs e)
    {
        FocusPreviousLine();
    }

    private void Btn_NextLineClick_Click(object sender, RoutedEventArgs e)
    {
        FocusNextLine();
    }

    private void Btn_StartTime_Click(object sender, RoutedEventArgs e)
    {
        SetStartPoint();
    }

    private void Btn_EndTime_Click(object sender, RoutedEventArgs e)
    {
        SetEndPoint();
    }

    private void Btn_PlayStatus_Click(object sender, RoutedEventArgs e)
    {
        ChangePlayStatus();
    }
}