using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Microsoft.Practices.Unity;

namespace InstiBulb.Converters
{
    public class ContainerResolverConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IUnityContainer container = value as IUnityContainer;
            string name = parameter as string;
            if (container == null || name == null) return null;

            string[] parms =  name.Split(new char[] { ';' });

            if (parms.Count() == 2)
                return container.Resolve(Type.GetType(parms[0]), parms[1]);
            else if (parms.Count() == 1)
                return container.Resolve(Type.GetType(parms[0]));
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
