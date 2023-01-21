using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ALRC.Creator.Interfaces;

namespace ALRC.Creator.Models.ViewModels;

public class WordEditViewModel : IViewModel
{
    private EditingALRC _alrc;
    private EditingALRCLine? _focusingLine;
    private EditingALRCWord? _focusingWord;
    private bool _isPreviewMode;
    private MusicPlayerModel _musicPlayer;

    public MusicPlayerModel MusicPlayer
    {
        get => _musicPlayer;
        set => SetField(ref _musicPlayer, value);
    }

    public bool IsPreviewMode
    {
        get => _isPreviewMode;
        set => SetField(ref _isPreviewMode, value);
    }

    public EditingALRCWord? FocusingWord
    {
        get => _focusingWord;
        set => SetField(ref _focusingWord, value);
    }

    public EditingALRCLine? FocusingLine
    {
        get => _focusingLine;
        set => SetField(ref _focusingLine, value);
    }

    public WordEditViewModel(EditingALRC alrc, MusicPlayerModel musicPlayer)
    {
        _alrc = alrc;
        _musicPlayer = musicPlayer;
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