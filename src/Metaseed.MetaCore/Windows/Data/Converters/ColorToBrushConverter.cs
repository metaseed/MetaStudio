using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
namespace Metaseed.Windows.Data.Converters
{
    /// <summary>
    /// Converts the <see cref="Color"/> to the <see cref="SolidColorBrush"/> value.
    /// </summary>
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public class ColorToBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //if (value == null && value.GetType() != typeof(Color))
            //    return value;
            Color col = Colors.Black;
            if (value is Color)
            {
                col = (Color)value;
            }
            return new SolidColorBrush(col);
        }
        /// <summary>
        /// Converts the specified <see cref="SolidColorBrush"/> to the Color.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color rv = Colors.DarkGray;
            if (value is SolidColorBrush)
            {
                rv = ((SolidColorBrush)value).Color;
            }
            return rv;
        }

        #endregion
    }
}
