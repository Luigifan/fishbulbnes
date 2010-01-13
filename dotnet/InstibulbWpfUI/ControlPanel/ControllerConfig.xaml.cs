using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InstiBulb
{
    /// <summary>
    /// Interaction logic for ControllerConfig.xaml
    /// </summary>
    public partial class ControllerConfig : UserControl
    {
        public ControllerConfig()
        {
            InitializeComponent();
            
        }


        private void KeyGrab_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox)
            {
                var p = (TextBox)sender;
                p.Text = e.Key.ToString();
                e.Handled = true;
            }
        }

    }
}
