using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ALRC.Creator.Interfaces;

namespace ALRC.Creator.Models.ViewModels;

public class PlayControllerViewModel : IViewModel
{
    private MusicPlayerModel _musicPlayerModel;

    public PlayControllerViewModel(MusicPlayerModel musicPlayerModel)
    {
        _musicPlayerModel = musicPlayerModel;
    }

    public MusicPlayerModel MusicPlayerModel
    {
        get => _musicPlayerModel;
        set => SetField(ref _musicPlayerModel, value);
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