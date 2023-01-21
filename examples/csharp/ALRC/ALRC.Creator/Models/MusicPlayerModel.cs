using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Threading;
using ALRC.Creator.Extensions;
using ALRC.Creator.Interfaces;

namespace ALRC.Creator.Models;

public class MusicPlayerModel : IModel
{
    private string _nowPlayingTime = "";
    private string _totalTime = "";
    public bool IsPlaying { get; set; }
    public bool IsSliding { get; set; } = false;

    public string NowPlayingTime
    {
        get => _nowPlayingTime;
        set => SetField(ref _nowPlayingTime, value);
    }

    public string TotalTime
    {
        get => _totalTime;
        set => SetField(ref _totalTime, value);
    }

    private double _totalTimeInMilliseconds;
    private double _currentTimeInMilliseconds;

    public double TotalTimeInMilliseconds
    {
        get => _totalTimeInMilliseconds;
        set => SetField(ref _totalTimeInMilliseconds, value);
    }

    public double CurrentTimeInMilliseconds
    {
        get => _currentTimeInMilliseconds;
        set => SetField(ref _currentTimeInMilliseconds, value);
    }

    public double CurrentTimeInMillisecondsToSlider
    {
        get => _currentTimeInMillisecondsToSlider;
        set => SetField(ref _currentTimeInMillisecondsToSlider, value);
    }

    public MediaPlayer Player { get; set; } = new();

    public readonly DispatcherTimer Timer = new DispatcherTimer(DispatcherPriority.Normal);
    private double _currentTimeInMillisecondsToSlider;

    public void LoadMusic(Uri uri)
    {
        Player.Stop();
        IsPlaying = false;
        Player.Open(uri);
    }

    public void Play()
    {
        if (!Player.HasAudio) return;
        TotalTime = Player.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss\.ff");
        TotalTimeInMilliseconds = Player.NaturalDuration.TimeSpan.TotalMilliseconds;
        Timer.Start();
        Player.Play();
        IsPlaying = true;
    }

    public void Pause()
    {
        if (!Player.HasAudio) return;
        Player.Pause();
        IsPlaying = false;
        Timer.Stop();
    }

    public void Seek(TimeSpan timeSpan)
    {
        if (!Player.HasAudio) return;
        Player.Position = timeSpan;
    }

    public MusicPlayerModel()
    {
        Timer.Interval = TimeSpan.FromMilliseconds(100);
        Timer.IsEnabled = true;
        Timer.Tick += TimerOnTick;
        Player.MediaEnded += (_, _) =>
        {
            IsPlaying = false;
            Timer.Stop();
        };
        Player.MediaOpened += (_, _) =>
        {
            TotalTime = Player.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss\.ff");
            CurrentTimeInMilliseconds = 0;
            NowPlayingTime = TimeSpan.FromMilliseconds(0).ToString(@"hh\:mm\:ss\.ff");
        };
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        CurrentTimeInMilliseconds = Player.Position.TotalMilliseconds;
        if (!IsSliding) CurrentTimeInMillisecondsToSlider = CurrentTimeInMilliseconds;
        NowPlayingTime = Player.Position.ToString(@"hh\:mm\:ss\.ff");
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