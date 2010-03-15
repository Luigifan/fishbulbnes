using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SilverlightBindings.ViewModels;

namespace SilverlightBindings.Views
{
    public partial class ToolBar : UserControl
    {
        public ToolBar()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var p = this.DataContext as ToolstripViewModel;
            if (p != null)
            {
                p.ShowWindow((sender as Button).Tag);
            }
        }

        private void BumpOutShownEvent(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null) return;

            ContentControl host = element.DataContext as ContentControl;
            if (host == null) return;

            if (host.Visibility == System.Windows.Visibility.Collapsed)
            {
                var p = this.DataContext as ToolstripViewModel;
                if (p != null)
                {
                    host.Visibility = System.Windows.Visibility.Visible;
                    p.FillBumpout(host, (sender as Button).Tag);
                }
            }
            else
            {
                host.Visibility = System.Windows.Visibility.Collapsed;
                host.Content = null;
            }
        }
    }
}
