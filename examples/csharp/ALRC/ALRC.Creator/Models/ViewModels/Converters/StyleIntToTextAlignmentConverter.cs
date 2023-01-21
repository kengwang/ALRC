using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ALRC.Creator.Models.ViewModels.Converters;

public class StyleIntToTextAlignmentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int position)
        {
            return position switch
            {
                2 => TextAlignment.Center,
                3 => TextAlignment.Right,
                _ => TextAlignment.Left
            };
        }
        return TextAlignment.Left;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}