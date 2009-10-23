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
    /// Interaction logic for PopupPanel.xaml
    /// </summary>
    public partial class PopupPanel : UserControl
    {

        public static DependencyProperty ChildProperty = DependencyProperty.Register("Child", typeof(UIElement), typeof(PopupPanel), new PropertyMetadata(null, new PropertyChangedCallback(ChildPropertyChanged)));

        static void ChildPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            (o as PopupPanel).AddChild(args.NewValue as UIElement);
        }

        internal void AddChild(UIElement child)
        {
            if (child != null)
                PopupPropertiesGrid.Child = child;
        }

        public UIElement Child
        {
            get { return (UIElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        public PopupPanel()
        {
            InitializeComponent();
        }

        private void ClosePopupPropertiesButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            PopupPropertiesGrid.Child = null;
            this.Child = null;
        }

        private void MigrateToDialog_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
