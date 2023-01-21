using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using ALRC.Creator.Interfaces;

namespace ALRC.Creator.Models.ViewModels;

public class StylesPageViewModel : IViewModel
{
    public EditingALRCStyle FocusingStyle
    {
        get => _focusingStyle;
        set => SetField(ref _focusingStyle, value);
    }

    private EditingALRC _alrc;
    private EditingALRCStyle _focusingStyle = null!;

    public StylesPageViewModel(EditingALRC alrc)
    {
        _alrc = alrc;
    }

    public EditingALRC ALRC
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