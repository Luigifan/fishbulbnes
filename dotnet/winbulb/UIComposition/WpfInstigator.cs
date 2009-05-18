using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Fishbulb.Common.UI;
using System.Windows.Controls;
using Microsoft.Practices.Unity;

namespace WPFamicom.UIComposition
{
    public class WpfInstigator : UIInstigator<FrameworkElement>
    {
        private void FrameworkElementAddChild(FrameworkElement parent, FrameworkElement child, int row, int col)
        {
            Panel p = (parent as Panel);
            if (p != null)
            {
                p.Children.Add(child);
                child.SetValue(Grid.RowProperty , row);
                child.SetValue(Grid.ColumnProperty, col);
            }
        }

        private FrameworkElement FrameworkElementFindChild(FrameworkElement parent, string child)
        {
            Panel p = (parent as Panel);

            if (p == null) return null;
            return (from FrameworkElement e in p.Children where e.Name == child select e).FirstOrDefault();
        }

        private void BindFrameworkElementToData(FrameworkElement elem, IProfileViewModel data)
        {
            elem.DataContext = data;
        }

        public WpfInstigator(IUnityContainer container)
            : base(container, FrameworkElementAddChild, BindFrameworkElementToData, FrameworkElementFindChild)
        {
        }
    }
}
