using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ALRC.Creator.Interfaces;
using ALRC.QualityChecker.Models;

namespace ALRC.Creator.Models.ViewModels;

public class QualityCheckPageViewModel(
    LinesEditPageViewModel linesEditPageViewModel,
    WordEditViewModel wordEditViewModel,
    MainWindowViewModel mainWindowViewModel,
    EditingALRC editingAlrc,
    IEnumerable<QualityCheckerBase> checkers) : IViewModel
{

    public ObservableCollection<Issue> Issues { get; set; } = [];
    private readonly EditableALRCConverter _editableAlrcConverter = new();
    private Issue _selectedIssue;

    public async Task StartScan(CancellationToken cancellationToken = default)
    {
        Issues.Clear();
        var alrc = _editableAlrcConverter.Convert(editingAlrc);
        foreach (var checker in checkers)
        {
            var issues = await checker.CheckAsync(alrc, cancellationToken);
            foreach (var issue in issues)
            {
                Issues.Add(issue);
            }
        }
    }

    public void NavigateToCurrentIssue()
    {
        if (SelectedIssue is null) return;
        if (SelectedIssue is WordIssue wordIssue)
        {
            linesEditPageViewModel.FocusingLine = editingAlrc.Lines[wordIssue.LineIndex];
            wordEditViewModel.FocusingLine = editingAlrc.Lines[wordIssue.LineIndex];
            wordEditViewModel.FocusingWord = linesEditPageViewModel.FocusingLine.Words?[wordIssue.WordIndex];
            mainWindowViewModel.NavigateToWordEdit();
        }
        else if (SelectedIssue is LineIssue lineIssue)
        {
            linesEditPageViewModel.FocusingLine = editingAlrc.Lines[lineIssue.LineIndex];
            mainWindowViewModel.NavigateToLinesEdit();
        }
        
    }

    public Issue? SelectedIssue
    {
        get => _selectedIssue;
        set => SetField(ref _selectedIssue, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}