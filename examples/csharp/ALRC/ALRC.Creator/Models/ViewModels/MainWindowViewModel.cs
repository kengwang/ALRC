using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ALRC.Creator.Interfaces;
using ALRC.Creator.Views.Controls;
using ALRC.Creator.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace ALRC.Creator.Models.ViewModels;

public class MainWindowViewModel : IViewModel, INotifyPropertyChanged
{
    private Frame _rootFrame = new();
    private NavigationCompact? _rootNavigation;

    private readonly IServiceProvider _services;
    private PlayController _playController;

    public Frame RootFrame
    {
        get => _rootFrame;
        set => SetField(ref _rootFrame, value);
    }

    public NavigationCompact? RootNavigation
    {
        get => _rootNavigation;
        set => SetField(ref _rootNavigation, value);
    }

    public PlayController PlayController
    {
        get => _playController;
        set => SetField(ref _playController, value);
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

    public MainWindowViewModel(IServiceProvider services, PlayController playController)
    {
        _services = services;
        _playController = playController;
    }

    public void NavigateToLyricInfoPage()
    {
        RootNavigation!.Navigate(nameof(LyricInfoPage));
        RootFrame.Navigate(_services.GetRequiredService<LyricInfoPage>());
    }

    public void NavigateToHome()
    {
        RootNavigation!.Navigate(nameof(HomePage));
        RootFrame.Navigate(_services.GetRequiredService<HomePage>());
    }

    public void NavigateToStylesEdit()
    {
        RootNavigation!.Navigate(nameof(StylesEditPage));
        RootFrame.Navigate(_services.GetRequiredService<StylesEditPage>());
    }

    public void NavigateToSettings()
    {
        RootNavigation!.Navigate(nameof(SettingsPage));
        RootFrame.Navigate(_services.GetRequiredService<SettingsPage>());
    }

    public void NavigateToLinesEdit()
    {
        RootNavigation!.Navigate(nameof(LinesEditPage));
        RootFrame.Navigate(_services.GetRequiredService<LinesEditPage>());
    }

    public void NavigateToWordEdit()
    {
        RootNavigation!.Navigate(nameof(WordEditPage));
        RootFrame.Navigate(_services.GetRequiredService<WordEditPage>());
    }

    public void NavigateToExport()
    {
        RootNavigation!.Navigate(nameof(ExportPage));
        RootFrame.Navigate(_services.GetRequiredService<ExportPage>());
    }

    public void NavigateToConvert()
    {
        RootNavigation!.Navigate(nameof(ConvertPage));
        RootFrame.Navigate(_services.GetRequiredService<ConvertPage>());
    }

    public void NavigateToPreview()
    {
        RootNavigation!.Navigate(nameof(PreviewPage));
        RootFrame.Navigate(_services.GetRequiredService<PreviewPage>());
    }
}