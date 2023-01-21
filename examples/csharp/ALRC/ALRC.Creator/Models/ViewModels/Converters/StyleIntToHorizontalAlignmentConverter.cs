using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ALRC.Creator.Models.ViewModels.Converters;

public class StyleIntToHorizontalAlignmentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int position)
        {
            return position switch
            {
                2 => HorizontalAlignment.Center,
                3 => HorizontalAlignment.Right,
                _ => HorizontalAlignment.Left
            };
        }
        return HorizontalAlignment.Left;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}