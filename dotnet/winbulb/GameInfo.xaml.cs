using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace InstiBulb
{
	/// <summary>
	/// Interaction logic for GameInfo.xaml
	/// </summary>
	public partial class GameInfo : Window
	{
		public GameInfo()
		{
			this.InitializeComponent();
            this.Visibility = Visibility.Hidden;
			// Insert code required on object creation below this point.
		}

        private void HideWindow(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void MoveWindow(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            this.Left += (int)e.HorizontalChange;

            this.Top += (int)e.VerticalChange;
            
        }
	}
}