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
using Microsoft.Practices.Unity;

namespace InstiBulb
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ControlPanel.UpdateKeyhandlingEvent += new EventHandler<EventArgs>(ControlPanel_UpdateKeyhandlingEvent);

            
        }

        void ControlPanel_UpdateKeyhandlingEvent(object sender, EventArgs e)
        {
            if (ControlPanel.SuppressKeystrokes)
            {
                Keyboard.AddPreviewKeyDownHandler(this, Window1_KeyDown);
                Keyboard.AddPreviewKeyUpHandler(this, Window1_KeyUp);
            }
            else
            {
                
                Keyboard.RemovePreviewKeyDownHandler(this, Window1_KeyDown);
                Keyboard.RemovePreviewKeyUpHandler(this, Window1_KeyUp);
            }

        }

        void Window1_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        void Window1_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

    }
}
