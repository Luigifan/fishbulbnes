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

namespace WPFamicom
{
	/// <summary>
	/// Interaction logic for NESController.xaml
	/// </summary>
	public partial class NESController
	{
		public NESController()
		{
			this.InitializeComponent();
		}

        private void ControllerMove(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (PositioningGrid.ColumnDefinitions[0].ActualWidth + e.HorizontalChange > 0
    && PositioningGrid.ColumnDefinitions[2].ActualWidth - e.HorizontalChange > 0)
            {
                PositioningGrid.ColumnDefinitions[0].Width = new GridLength(PositioningGrid.ColumnDefinitions[0].ActualWidth + e.HorizontalChange);
                PositioningGrid.ColumnDefinitions[2].Width = new GridLength(PositioningGrid.ColumnDefinitions[2].ActualWidth - e.HorizontalChange); ;
            }
            if (PositioningGrid.RowDefinitions[0].ActualHeight + e.VerticalChange > 0
                && PositioningGrid.RowDefinitions[2].ActualHeight - e.VerticalChange > 0)
            {
                PositioningGrid.RowDefinitions[0].Height = new GridLength(PositioningGrid.RowDefinitions[0].ActualHeight + e.VerticalChange);
                PositioningGrid.RowDefinitions[2].Height = new GridLength(PositioningGrid.RowDefinitions[2].ActualHeight - e.VerticalChange);
            }

        }
	}
}