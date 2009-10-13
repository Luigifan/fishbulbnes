using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace InstiBulb.Converters
{
    public class ShortFileNameConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int len = 8;
            if (parameter is string && int.TryParse(parameter as string, out len)) ;

            
            string fileName = value as string;

            string ext = fileName.Substring(fileName.Length - 4, 4);

            return fileName.Length > len + 4 ? 
                fileName.Substring(0, len) + ext : fileName;


        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
