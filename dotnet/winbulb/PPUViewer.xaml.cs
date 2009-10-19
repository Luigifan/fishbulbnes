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

namespace InstiBulb
{
	/// <summary>
	/// Interaction logic for PPUViewer.xaml
	/// </summary>
	public partial class PPUViewer : Window
	{
		public PPUViewer()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

        private void MoveWindow(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            this.Left += e.HorizontalChange;
            this.Top += e.VerticalChange;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
	}
}