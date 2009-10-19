using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace InstiBulb.ControlPanelMVVM
{
    public class ShmindowManager
    {
        int selectedShmindowZIndex = 1;

        public int SelectedShmindowZIndex
        {
            get { return selectedShmindowZIndex; }
            set { selectedShmindowZIndex = value; }
        }

        int deselectedShmindowZIndex = 0;

        public int DeselectedShmindowZIndex
        {
            get { return deselectedShmindowZIndex; }
            set { deselectedShmindowZIndex = value; }
        }

    }

    public class ShmindowStateToZindex: IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter.GetType() != typeof(ShmindowManager))
            {
                // leave it alone?
                return 0;
            }

            if (value.GetType() == typeof(bool))
            {
                if ((bool)value == true)
                {
                    return (parameter as ShmindowManager).SelectedShmindowZIndex;
                }

            }
            return (parameter as ShmindowManager).DeselectedShmindowZIndex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
