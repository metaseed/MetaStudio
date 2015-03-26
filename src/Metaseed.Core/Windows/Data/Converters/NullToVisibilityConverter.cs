using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Data;
using System.Windows;
namespace Metaseed.Windows.Data.Converters
{
 public   class NullToVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value != null)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
