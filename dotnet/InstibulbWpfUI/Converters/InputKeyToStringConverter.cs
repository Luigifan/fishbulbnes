using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;

namespace InstiBulb.Converters
{
    [ValueConversion(typeof(Key),typeof(string))]
    public class InputKeyToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var p = value as Key?;
            if (p.HasValue)
                return p.Value.ToString();
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;
            if (s == null) return Key.None;
            Key result = Key.None;
            try
            {
                result = (Key)Enum.Parse(typeof(Key), s);
            }
            catch
            {
            }
            return result;
        }

        #endregion
    }
}
