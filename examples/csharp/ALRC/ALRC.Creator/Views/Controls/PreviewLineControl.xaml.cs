using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ALRC.Creator.Models;
using ALRC.Creator.Models.ViewModels;
using ALRC.Creator.Models.ViewModels.Converters;

namespace ALRC.Creator.Views.Controls;

public partial class PreviewLineControl : UserControl
{
    public PreviewLineControlViewModel ViewModel { get; set; }

    private List<ParsedWord> _words = new();

    private SolidColorBrush _defBrush = new SolidColorBrush(Colors.Gray);

    private StyleColorToColorConverter _colorConverter =
        ((StyleColorToColorConverter)App.Current.Resources[nameof(StyleColorToColorConverter)]);
    
    private StyleIntToHorizontalAlignmentConverter _alignmentConverter =
        ((StyleIntToHorizontalAlignmentConverter)App.Current.Resources[nameof(StyleIntToHorizontalAlignmentConverter)]);

    
    
    public PreviewLineControl(PreviewLineControlViewModel viewModel)
    {
        DataContext = viewModel;
        ViewModel = viewModel;
        InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        ViewModel.LineStyle = ViewModel.Alrc?.Styles.FirstOrDefault(t => t.Id == ViewModel.Line.LineStyle) ?? new();
        if (ViewModel.Line.Words is null or { Count: <= 0 }) return;
        LineTextBlock.Visibility = Visibility.Collapsed;
        foreach (var word in ViewModel.Line.Words)
        {
            var curWordStyle = ViewModel.Alrc?.Styles.FirstOrDefault(style => style.Id == word.WordStyle) ??
                               ViewModel.LineStyle;
            var tb = new TextBlock();
            tb.Text = word.Word;
            tb.Foreground = _defBrush;
            tb.FontSize = curWordStyle.Type switch
            {
                1 => 20,
                2 => 15,
                3 => 38,
                _ => 28
            };
            tb.FontWeight = curWordStyle.Type switch
            {
                1 => FontWeights.Light,
                2 => FontWeights.Thin,
                3 => FontWeights.Bold,
                _ => FontWeights.Normal
            };
            _words.Add(new ParsedWord
            {
                Word = word,
                Style = curWordStyle,
                TextBlock = tb
            });
            WordsContainer.Children.Add(tb);
        }
    }

    public void RefreshTime(double curTime)
    {
        if (_words is null or { Count: <= 0 }) return;
        var newPosition = ViewModel.LineStyle.Position;
        foreach (var word in _words)
        {
            if (word.Word.Start <= curTime)
            {
                if (string.IsNullOrWhiteSpace(word.Style?.Color))
                    word.TextBlock.Foreground =
                        (SolidColorBrush)_colorConverter.Convert(ViewModel.LineStyle.Color!, null!, null!, null!);
                else
                    word.TextBlock.Foreground =
                        (SolidColorBrush)_colorConverter.Convert(word.Style?.Color!, null!, null!, null!);

                if (word.Style is not null && word.Style.Position != newPosition) newPosition = word.Style.Position;
            }
        }

        if (ViewModel.LineStyle.Position != newPosition)
        {
            WordsContainer.HorizontalAlignment = (HorizontalAlignment)_alignmentConverter.Convert(newPosition,null!,null!,null!);
        }
    }
}

internal class ParsedWord
{
    public required EditingALRCWord Word { get; set; }
    public  EditingALRCStyle? Style { get; set; }
    public required TextBlock TextBlock { get; set; }
}