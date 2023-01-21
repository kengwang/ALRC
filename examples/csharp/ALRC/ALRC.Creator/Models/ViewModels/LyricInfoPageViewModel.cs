using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ALRC.Creator.Interfaces;

namespace ALRC.Creator.Models.ViewModels;

public class LyricInfoPageViewModel : IViewModel
{
    private EditingALRC _editingAlrc;

    public LyricInfoPageViewModel(EditingALRC editingAlrc)
    {
        _editingAlrc = editingAlrc;
    }

    public EditingALRC EditingAlrc
    {
        get => _editingAlrc;
        set => SetField(ref _editingAlrc, value);
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