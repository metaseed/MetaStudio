using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Metaseed.Data;
using System.Windows;
namespace Metaseed.Windows.Data.Converters
{
    public class UInt16ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToString((System.Convert.ToUInt16(value)), 16) + "H";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
return StringConvert.ToUInt16(value.ToString());
            }
            catch (Exception)
            {
                return DependencyProperty.UnsetValue;
            }
            
        }
    }
}
