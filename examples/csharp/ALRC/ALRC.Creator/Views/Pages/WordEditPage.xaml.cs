using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ALRC.Creator.Extensions;
using ALRC.Creator.Models;
using ALRC.Creator.Models.ViewModels;
using Kawazu;
using NMeCab.Specialized;

namespace ALRC.Creator.Views.Pages;

public partial class WordEditPage : Page
{
    private readonly WordEditViewModel _viewModel;

    public WordEditPage(WordEditViewModel viewModel)
    {
        DataContext = viewModel;
        _viewModel = viewModel;
        InitializeComponent();
    }

    private void FocusNextWord()
    {
        if (WordsListView.SelectedIndex + 1 < (_viewModel.FocusingLine?.Words?.Count ?? -1))
            WordsListView.SelectedIndex += 1;
        else
            FocusNextLine();
        WordsListView.ScrollIntoView(WordsListView.SelectedItem);
    }

    private void FocusPreviousWord()
    {
        if (WordsListView.SelectedIndex > 0)
            WordsListView.SelectedIndex -= 1;
        else
        {
            FocusPreviousLine();
            EditingALRCWord lastItem = null!;
            if (_viewModel.FocusingLine?.Words is { Count: > 0 })
                lastItem = _viewModel.FocusingLine.Words[^1];
            _viewModel.FocusingWord = lastItem;
        }

        if (WordsListView.SelectedItem is not null)
            WordsListView.ScrollIntoView(WordsListView.SelectedItem);
    }

    private void FocusNextLine()
    {
        if (LineSelector.SelectedIndex < _viewModel.Alrc.Lines.Count)
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
            3000 + _viewModel.MusicPlayer.CurrentTimeInMilliseconds)
            _viewModel.MusicPlayer.Seek(
                TimeSpan.FromMilliseconds(_viewModel.MusicPlayer.CurrentTimeInMilliseconds + 3000));
    }

    private void SeekBackward()
    {
        if (_viewModel.MusicPlayer.CurrentTimeInMilliseconds > 3000)
            _viewModel.MusicPlayer.Seek(
                TimeSpan.FromMilliseconds(_viewModel.MusicPlayer.CurrentTimeInMilliseconds - 3000));
        else
            _viewModel.MusicPlayer.Seek(TimeSpan.Zero);
    }

    private void SetEndPoint()
    {
        if (_viewModel.FocusingWord != null)
            _viewModel.FocusingWord.End = (int)_viewModel.MusicPlayer.CurrentTimeInMilliseconds;
    }

    private void SetStartPoint()
    {
        if (_viewModel.FocusingWord != null)
            _viewModel.FocusingWord.Start = (int)_viewModel.MusicPlayer.CurrentTimeInMilliseconds;
    }

    private void ChangePlayStatus()
    {
        if (_viewModel.MusicPlayer.IsPlaying) _viewModel.MusicPlayer.Pause();
        else _viewModel.MusicPlayer.Play();
    }

    private void EasyTimelineEditorTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        Debug.WriteLine(e.Key);
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
                FocusPreviousWord();
                break;
            case Key.NumPad2:
            case Key.LeftCtrl:
                FocusNextWord();
                break;
            case Key.Enter:
            case Key.Space:
                SetEndPoint();
                FocusNextWord();
                SetStartPoint();
                break;
            case Key.NumPad7:
            case Key.CapsLock:
                FocusPreviousLine();
                break;
            case Key.NumPad9:
            case Key.F:
                FocusNextLine();
                break;
            case Key.RightShift:
                return;
        }

        e.Handled = true;
    }

    private void Btn_QuickAdd_Words_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.FocusingLine is null) return;
        if (WordFastInsertBox.Visibility == Visibility.Visible)
        {
            WordFastInsertBox.Visibility = Visibility.Collapsed;
            WordsListView.Visibility = Visibility.Visible;
            _viewModel.FocusingLine.Words!.Clear();
            foreach (var word in WordFastInsertBox.Text.Replace("\r\n", "\n").Replace("\r", "\n").Split("\n"))
            {
                _viewModel.FocusingLine.Words.Add(new EditingALRCWord
                {
                    Word = word
                });
            }
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(_viewModel.FocusingLine?.Text))
                WordFastInsertBox.Text = string.Join(" \n", _viewModel.FocusingLine.Text.Split(' '));
            WordFastInsertBox.Visibility = Visibility.Visible;
            WordsListView.Visibility = Visibility.Collapsed;
        }
    }

    private void Btn_Word_Add(object sender, RoutedEventArgs e)
    {
        if (_viewModel.FocusingLine is null) return;
        _viewModel.FocusingLine.Words ??= new();
        _viewModel.FocusingLine.Words.Insert(WordsListView.SelectedIndex + 1, new EditingALRCWord());
    }

    private void Btn_Word_Remove(object sender, RoutedEventArgs e)
    {
        if (_viewModel.FocusingLine?.Words is null) return;
        if (WordsListView.SelectedItem is EditingALRCWord word)
            _viewModel.FocusingLine.Words.Remove(word);
    }

    private void Btn_AutoParseAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var line in LineSelector.SelectedItems.Cast<EditingALRCLine>())
        {
            if (string.IsNullOrEmpty(line?.Text)) continue;
            var first = false;
            var words = Regex.Replace(line.Text.Replace("\r\n", "\n").Replace("\r", "\n").Replace(" ", " \n"),
                "([\\u4e00-\\u9fa5\\u3040-\\u30ff\\uac00-\\ud7a30-9])", "$1\n").Split('\n');
            foreach (var word in words)
            {
                var lrcWord = new EditingALRCWord
                {
                    Word = word
                };
                line.Words ??= new();

                if (first)
                {
                    line.Words.Clear();
                    lrcWord.Start = line.Start;
                    first = false;
                }

                line.Words.Add(lrcWord);
            }
        }
    }

    private void LineSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_viewModel.FocusingLine?.Words?.Count > 0)
            WordsListView.SelectedIndex = 0;
    }

    private void PreviewMode_Checked(object sender, RoutedEventArgs e)
    {
        _viewModel.MusicPlayer.Timer.Tick += TimerOnTick;
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        if (!_viewModel.IsPreviewMode) return;
        var curLine = _viewModel.Alrc.Lines.FirstOrDefault(line =>
            line.Start <= _viewModel.MusicPlayer.CurrentTimeInMilliseconds &&
            line.End > _viewModel.MusicPlayer.CurrentTimeInMilliseconds);
        _viewModel.FocusingLine = curLine;
        if (curLine?.Words is { Count: > 0 })
        {
            _viewModel.FocusingWord = curLine.Words.FirstOrDefault(word =>
                word.Start <= _viewModel.MusicPlayer.CurrentTimeInMilliseconds &&
                word.End > _viewModel.MusicPlayer.CurrentTimeInMilliseconds);
        }
    }

    private void Btn_Line_Align(object sender, RoutedEventArgs e)
    {
        var lines = LineSelector.SelectedItems.Cast<EditingALRCLine>().Where(t => t.Words is { Count: > 0 }).ToList();
        foreach (var line in lines)
        {
            line.Words![0].Start = line.Start;
            line.Words[^1].End = line.End;
        }
    }

    private void StyleBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (WordsListView.SelectedItems.Count <= 1 || _viewModel.IsPreviewMode) return;
        foreach (var word in WordsListView.SelectedItems.Cast<EditingALRCWord>().ToList())
        {
            word.WordStyle = StyleBox.Text;
        }
    }

    private void PreviewMode_Unchecked(object sender, RoutedEventArgs e)
    {
        _viewModel.MusicPlayer.Timer.Tick -= TimerOnTick;
    }

    private async void Btn_AutoParseAll_RightClick(object sender, MouseButtonEventArgs e)
    {
        var kawazu = new KawazuConverter();
        foreach (var line in LineSelector.SelectedItems.Cast<EditingALRCLine>())
        {
            if (string.IsNullOrWhiteSpace(line?.Text)) continue;
            if (line.Words is not { Count: > 0 })
            {
                var first = false;
                // 日文分词
                var divisions = await kawazu.GetDivisions(line.Text, To.Romaji, Mode.Spaced);
                var elements = new List<JapaneseElement>();
                foreach (var division in divisions)
                {
                    elements.AddRange(division);
                }

                foreach (var element in elements)
                {
                    var romaji = KawazuExtendedUtilities.ToRawRomaji(element.HiraNotation);
                    var lrcWord = new EditingALRCWord
                    {
                        Word = element.Element,
                        Transliteration = string.IsNullOrWhiteSpace(romaji) ? null : $"{romaji.Trim()} ",
                    };
                    line.Words ??= new();

                    if (first)
                    {
                        line.Words.Clear();
                        lrcWord.Start = line.Start;
                        first = false;
                    }

                    line.Words.Add(lrcWord);
                }
                
                line.Transliteration = string.Join("", line.Words?.Select(t => t.Transliteration) ?? []);


            }
            else
            {
                var divitions = await kawazu.GetDivisions(line.Text, To.Romaji, Mode.Spaced);
                SetRomajiKaraoke(divitions, line.Words);
            }
        }
    }


    static void SetRomajiKaraoke(List<Division> romajiInfo, ObservableCollection<EditingALRCWord> wordInfo)
    {
        var elements = new List<JapaneseElement>();
        foreach (var division in romajiInfo)
        {
            elements.AddRange(division);
        }

        int delta = 0;
        for (var i = 0; i < elements.Count; i++)
        {
            var curElement = elements[i].Element;
            var curHiraNotation = elements[i].HiraNotation;
            parseOneChar:
            if (i + delta >= wordInfo.Count)
            {
                if (!string.IsNullOrWhiteSpace(curHiraNotation))
                {
                    wordInfo[^1].Transliteration +=
                        KawazuExtendedUtilities.ToRawRomaji(curHiraNotation);
                }

                break;
            }

            if (curElement.Contains(wordInfo[i + delta].Word?.Trim() ?? ""))
            {
                wordInfo[i + delta].Transliteration =
                    KawazuExtendedUtilities.ToRawRomaji(curHiraNotation);
                if (!string.IsNullOrWhiteSpace(wordInfo[i + delta].Word))
                {
                    var trimmedWord = wordInfo[i + delta].Word?.Trim() ?? "";
                    var idx = curElement.IndexOf(trimmedWord, StringComparison.Ordinal);
                    if (idx >= 0)
                        curElement = curElement.Remove(idx, trimmedWord.Length);
                }

                if (curElement.Trim().Length > 0)
                {
                    wordInfo[i + delta].Transliteration =
                        KawazuExtendedUtilities.ToRawRomaji(curHiraNotation.Substring(0, 1));
                    curHiraNotation = curHiraNotation.Substring(1);
                    delta++;
                    goto parseOneChar;
                }
            }
        }
    }

    private void Btn_Line_AlignRightClick(object sender, MouseButtonEventArgs e)
    {
        // 行轴对齐词头尾
        var lines = LineSelector.SelectedItems.Cast<EditingALRCLine>().Where(t => t.Words is { Count: > 0 }).ToList();
        foreach (var line in lines)
        {
            line.Start = line.Words![0].Start ;
            line.End = line.Words[^1].End;
        }
    }
}