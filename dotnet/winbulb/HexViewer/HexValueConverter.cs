using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace InstiBulb.HexViewer
{
    public class HexValueConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? length = parameter as int?;

            if (length == null)
            {
                length = 2;
            }
            // icky unboxing!
            if (targetType == typeof(string))
            {
                return string.Format("{0:x" + length.ToString() + "}", value);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
