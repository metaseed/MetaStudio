using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Data;
using System.Windows.Media;
namespace Metaseed.Windows.Data.Converters
{
    public class BrushToColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color rv = Colors.DarkGray;
            if (value is SolidColorBrush)
            {
                rv = ((SolidColorBrush)value).Color;
            }
            return rv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color col = Colors.Black;
            if (value is Color)
            {
                col = (Color)value;
            }
            return new SolidColorBrush(col);
        }

        #endregion
    }
}
