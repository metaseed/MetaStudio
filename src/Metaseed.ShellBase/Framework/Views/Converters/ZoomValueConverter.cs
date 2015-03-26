using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

using System.Windows.Controls;
using System.Windows.Media;

namespace Metaseed.MetaShell.Views
{
    public class ZoomValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return 100 *(Double) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
