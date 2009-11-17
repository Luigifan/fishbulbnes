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
using SlimDXBindings.Viewer10.Helpers;

namespace SlimDXBindings.Viewer10.ControlPanel
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ButtonsAndBoxes : EmbeddableUserControl
    {
        public ButtonsAndBoxes()
        {
            InitializeComponent();
            Checkle.MouseDown += new MouseButtonEventHandler(Checkle_MouseDown);
        }

        void Checkle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Content as string == "Clicked")
                (sender as Button).Content = "";
            else
                (sender as Button).Content = "Clicked";
        }

        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CheckDown(object sender, MouseButtonEventArgs e)
        {
            (sender as CheckBox).IsChecked = !(sender as CheckBox).IsChecked;
        }

        private void CheckClick(object sender, RoutedEventArgs e)
        {
            (sender as CheckBox).IsChecked = !(sender as CheckBox).IsChecked;

        }

        private void Button_MouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
