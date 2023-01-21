using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ALRC.Creator.Models.ViewModels;
using Microsoft.Win32;

namespace ALRC.Creator.Views.Controls;

public partial class PlayController : UserControl
{
    public PlayControllerViewModel ViewModel { get; set; }

    public bool Slider_Binded { get; set; }

    public PlayController(PlayControllerViewModel viewModel)
    {
        DataContext = viewModel;
        ViewModel = viewModel;
        InitializeComponent();
    }

    private void SliderDragCompleted(object sender, DragCompletedEventArgs e)
    {
        ViewModel.MusicPlayerModel.Seek(TimeSpan.FromMilliseconds(Slider.Value));
        ;
        ViewModel.MusicPlayerModel.IsSliding = false;
    }

    private void SliderDragStarted(object sender, DragStartedEventArgs e)
    {
        ViewModel.MusicPlayerModel.IsSliding = true;
    }


    private void Btn_Open_Click(object sender, RoutedEventArgs e)
    {
        if (!Slider_Binded)
        {
            Slider_Binded = true;
            var thumb = (Thumb)Slider.Template.FindName("Thumb", Slider);
            thumb.DragStarted += SliderDragStarted;
            thumb.DragCompleted += SliderDragCompleted;
        }

        var dialog = new OpenFileDialog();
        if (dialog.ShowDialog() is true)
        {
            ViewModel.MusicPlayerModel.LoadMusic(new Uri(dialog.FileName));
        }
    }

    private void Btn_PlayStatus_Change(object sender, RoutedEventArgs e)
    {
        if (ViewModel.MusicPlayerModel.IsPlaying)
            ViewModel.MusicPlayerModel.Pause();
        else
            ViewModel.MusicPlayerModel.Play();
    }

    private void SpeedSwitchBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ViewModel.MusicPlayerModel.Player.SpeedRatio = SpeedSwitchBox.SelectedIndex switch
        {
            0 => 0.1,
            1 => 0.25,
            2 => 0.5,
            3 => 0.75,
            _ => 1
        };
    }
}