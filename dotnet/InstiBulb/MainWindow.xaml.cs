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
using _3DTools;

namespace InstiBulb
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {

        }
        /// <summary>
        /// This is so that DI Container can Resolve the MainWindow, but can defer resolution of the components
        /// 
        /// </summary>
        /// <returns></returns>
        public MainWindow Initialize()
        {
            InitializeComponent();
            return this;
        }

        private void OuterGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if (ControlBoxer.Visibility != Visibility.Visible)
                {
                    Dispatcher.BeginInvoke(ControlBoxer.WhizOnHandler, System.Windows.Threading.DispatcherPriority.Render, null);
                }
            }
        }

        private void Windows_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Maximize(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

    }
}
