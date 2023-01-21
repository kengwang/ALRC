using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ALRC.Creator.Models.ViewModels.Converters;

public class StyleColorToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string colorCodeString)
        {
            try
            {
                var colorRet = ColorConverter.ConvertFromString(colorCodeString);
                if (colorRet is Color color)
                    return new SolidColorBrush(color);
            }
            catch
            {
                return new SolidColorBrush(Colors.White);
            }
        }
        return new SolidColorBrush(Colors.White);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}