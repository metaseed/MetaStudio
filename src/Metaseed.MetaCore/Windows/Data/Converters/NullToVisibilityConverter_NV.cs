using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
namespace Metaseed.Windows.Data.Converters
{
public class NullToVisibilityConverter_NV : IValueConverter
{

    object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return value == null ? Visibility.Visible : Visibility.Collapsed;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
}
