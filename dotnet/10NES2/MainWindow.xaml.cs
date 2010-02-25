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
using InstiBulb.Windowing;

namespace _10NES2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        


        private void Window_Closed(object sender, EventArgs e)
        {
            

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            nesDisplay.DestroyContext();
            nesDisplay.Dispose();
            
            var p = DataContext as IDisposable;
            if (p != null)
                p.Dispose();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var p = DataContext as MainWindowViewModel;
            if (p != null)
                p.WindowWidth = e.NewSize.Width;
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var p = DataContext as MainWindowViewModel;
            if (p != null)
                p.UserRequestsToolView = !p.UserRequestsToolView;

        }

    }
}
