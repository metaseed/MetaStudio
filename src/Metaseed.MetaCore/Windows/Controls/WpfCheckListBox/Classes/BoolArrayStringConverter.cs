using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace WpfCheckListBox
{
    [ValueConversion(typeof(bool[]), typeof(string))]
    public class BoolArrayStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StringBuilder _result = new StringBuilder();

            if (value is bool[])
            {
                foreach (bool _item in (bool[])value)
                {
                    _result.Append(_item ? "true, " : "false, ");
                }
                return _result.ToString();
            }
            else
            {
                return "null";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        #endregion
    }

}
