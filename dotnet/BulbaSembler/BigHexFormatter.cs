using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BulbaSembler.ValueConverters
{
    
    public class BigHexFormatter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            StringBuilder builder = new StringBuilder();


            IEnumerable<byte> list = value as IEnumerable<byte>;

            byte[] bytes = list.ToArray<byte>();

            for (int i = 0; i < bytes.Length; i += 256)
            {
                for (int j = 0; j < 256; ++j)
                {
                    if (i + j >= bytes.Length) break;
                    builder.AppendFormat("{0:x2} ", bytes[i+j]);
                }
                builder.AppendLine(":");
            }

            return builder.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
