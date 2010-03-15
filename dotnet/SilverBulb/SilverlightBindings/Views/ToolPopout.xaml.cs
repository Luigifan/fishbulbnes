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

namespace SilverlightBindings.Views
{
    public partial class ToolPopout : ChildWindow
    {
        public ToolPopout()
        {
            InitializeComponent();
        }

        

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {

            
            base.OnClosing(e);
        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (FrameworkElement p in LayoutRoot.Children.Where(p => p is FrameworkElement))
            {
                var dc = p.DataContext as IDisposable;
                if (dc != null)
                    dc.Dispose();
            }
            //this.DialogResult = true;
            this.Close();
        }

    }
}

