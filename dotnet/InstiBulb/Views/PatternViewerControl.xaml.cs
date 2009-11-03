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
using System.Windows.Navigation;
using System.Windows.Shapes;
using InstiBulb.WinViewModels;

namespace InstiBulb.Views
{
	/// <summary>
	/// Interaction logic for PatternViewerControl.xaml
	/// </summary>
	public partial class PatternViewerControl
	{
		public PatternViewerControl()
		{
			this.InitializeComponent();
		}

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var p = DataContext as WinDebuggerVM;
            var img = sender as Image;
            if (p != null && img != null)
            {
                Point pt = e.GetPosition(img);
                int x = (int)((pt.X / img.ActualWidth) * 255.0);
                int y = (int)((pt.Y / img.ActualHeight) * 255.0);

                MessageBox.Show(p.WhichTileAddress(0, x, y));
            }
        }


	}
}