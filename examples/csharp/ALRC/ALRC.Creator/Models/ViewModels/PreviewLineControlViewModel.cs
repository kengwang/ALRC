using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ALRC.Creator.Interfaces;

namespace ALRC.Creator.Models.ViewModels;

public class PreviewLineControlViewModel : DependencyObject, IModelBase
{


    public PreviewLineControlViewModel(EditingALRC alrc, EditingALRCLine line)
    {
        Alrc = alrc;
        Line = line;
    }

    // ReSharper disable once UnusedMember.Global
#pragma warning disable CS8618
    public PreviewLineControlViewModel()
#pragma warning restore CS8618
    {
    }


    public static readonly DependencyProperty LineStyleProperty = DependencyProperty.Register(
        nameof(LineStyle), typeof(EditingALRCStyle), typeof(PreviewLineControlViewModel), new PropertyMetadata(default(EditingALRCStyle)));

    public EditingALRCStyle LineStyle
    {
        get => (EditingALRCStyle)GetValue(LineStyleProperty);
        set => SetValue(LineStyleProperty, value);
    }
    

    public static readonly DependencyProperty LineProperty = DependencyProperty.Register(
        nameof(Line), typeof(EditingALRCLine), typeof(PreviewLineControlViewModel), new PropertyMetadata(default(EditingALRCLine)));

    public EditingALRCLine Line
    {
        get => (EditingALRCLine)GetValue(LineProperty);
        set => SetValue(LineProperty, value);
    }

    public static readonly DependencyProperty AlrcProperty = DependencyProperty.Register(
        nameof(Alrc), typeof(EditingALRC), typeof(PreviewLineControlViewModel),
        new PropertyMetadata(default(EditingALRC)));

    public EditingALRC? Alrc
    {
        get => (EditingALRC)GetValue(AlrcProperty);
        set => SetValue(AlrcProperty, value);
    }
}