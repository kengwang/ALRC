using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ALRC.Creator.Interfaces;

namespace ALRC.Creator.Models.ViewModels;

public class ExportPageViewModel : IViewModel
{
    private EditingALRC _alrc;

    public ExportPageViewModel(EditingALRC alrc)
    {
        _alrc = alrc;
    }

    public EditingALRC Alrc
    {
        get => _alrc;
        set => SetField(ref _alrc, value);
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