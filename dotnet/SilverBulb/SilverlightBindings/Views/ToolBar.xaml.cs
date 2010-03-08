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
    }
}
