using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace InstiBulb.Converters
{
    public class NegeateBooleanConverter : IValueConverter 
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? b = value as bool?;
            if (b.HasValue)
            {
                return !b;
            }
            else
            {
                return true;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? b = value as bool?;
            if (b.HasValue)
            {
                return !b;
            }
            else
            {
                return true;
            }

        }
    }
}
