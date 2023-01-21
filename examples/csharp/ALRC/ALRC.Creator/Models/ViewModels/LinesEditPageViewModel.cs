using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ALRC.Creator.Interfaces;

namespace ALRC.Creator.Models.ViewModels;

public class LinesEditPageViewModel : IViewModel
{
    private EditingALRC _editingAlrc;
    private EditingALRCLine _focusingLine = null!;
    private MusicPlayerModel _musicPlayer;
    private string? _quickInputTexts;
    private bool _isPreviewMode;

    public bool IsPreviewMode
    {
        get => _isPreviewMode;
        set => SetField(ref _isPreviewMode, value);
    }

    public string? QuickInputTexts
    {
        get => _quickInputTexts;
        set => SetField(ref _quickInputTexts, value);
    }

    public MusicPlayerModel MusicPlayer
    {
        get => _musicPlayer;
        set => SetField(ref _musicPlayer, value);
    }

    public EditingALRC EditingAlrc
    {
        get => _editingAlrc;
        set => SetField(ref _editingAlrc, value);
    }

    public EditingALRCLine FocusingLine
    {
        get => _focusingLine;
        set => SetField(ref _focusingLine, value);
    }

    public LinesEditPageViewModel(EditingALRC editingAlrc, MusicPlayerModel musicPlayer)
    {
        _editingAlrc = editingAlrc;
        _musicPlayer = musicPlayer;
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