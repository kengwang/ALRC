using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ALRC.Creator.Interfaces;
using ALRC.Creator.Views.Controls;

namespace ALRC.Creator.Models.ViewModels;

public class PreviewPageViewModel : IViewModel
{
    public MusicPlayerModel MusicPlayer
    {
        get => _musicPlayer;
        set => SetField(ref _musicPlayer, value);
    }

    public ObservableCollection<PreviewLineControl> PreviewingLines
    {
        get => _previewingLines;
        set => SetField(ref _previewingLines, value);
    }


    private EditingALRC _alrc;
    private MusicPlayerModel _musicPlayer;
    private ObservableCollection<PreviewLineControl> _previewingLines = new();

    public PreviewPageViewModel(EditingALRC alrc, MusicPlayerModel musicPlayer)
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