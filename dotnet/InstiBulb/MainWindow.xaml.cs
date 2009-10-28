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

        }
        /// <summary>
        /// This is so that DI Container can Resolve the MainWindow, but can defer resolution of the components
        /// 
        /// </summary>
        /// <returns></returns>
        public MainWindow Initialize()
        {
            IUnityContainer container = (IUnityContainer)FindResource("Container");
            this.AllowsTransparency = false;

            InitializeComponent();

            UIElement element = container.Resolve<InstiBulb.Integration.NESDisplay>();
            element.SetValue(Grid.RowSpanProperty, OuterGrid.RowDefinitions.Count);
            element.SetValue(Grid.RowProperty, 0);
            OuterGrid.Children.Insert(0,element);
            this.InvalidateArrange();
            return this;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowStyle = WindowStyle.None;
            }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.ResizeMode = ResizeMode.CanResize;
                this.WindowStyle = WindowStyle.ThreeDBorderWindow;
            }
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


    }
}
