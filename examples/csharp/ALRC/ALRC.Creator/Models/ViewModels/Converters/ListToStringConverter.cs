using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ALRC.Creator.Models.ViewModels.Converters;

public class ListToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ObservableCollection<EditingALRCStyle.EditingALRCStyleSinger> singers)
        {
            return string.Join(", ", singers.Select(t=>t.DisplayName));
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}