using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
namespace Metaseed.Windows.Data.Converters
{
    public class NullableToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value==null)
            {
                return string.Empty;
            }
            else
            {
                return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
