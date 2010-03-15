using System;
using System.Net;
using System.Windows.Data;

namespace FishBulb.Converters
{
    public class BitValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int bitNum = 0;
            if (int.TryParse((parameter as string), out bitNum))
            {
                int data = (int)value;

                return (data & (1 << bitNum)) > 0;

            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
