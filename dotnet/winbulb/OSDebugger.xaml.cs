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
using NES.CPU.CPUDebugging;

namespace InstiBulb
{


	/// <summary>
	/// Interaction logic for OSDebugger.xaml
	/// </summary>
	public partial class OSDebugger
	{

        public static readonly DependencyProperty BreakpointHitProperty = DependencyProperty.Register("BreakpointIsHit", typeof(Boolean), typeof(ControlPanel), new PropertyMetadata(false));

        public Boolean BreakpointIsHit
        {
            get { return (Boolean)this.GetValue(BreakpointHitProperty); }
            set { this.SetValue(BreakpointHitProperty, value); }
        }


        public OSDebugger()
		{
			this.InitializeComponent();
		}







        //private void SqueezeBoxMoved(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        //{
        //    if (PositioningGrid.ColumnDefinitions[0].ActualWidth + e.HorizontalChange > 0
        //        && PositioningGrid.ColumnDefinitions[2].ActualWidth - e.HorizontalChange > 0)
        //    {
        //        PositioningGrid.ColumnDefinitions[0].Width = new GridLength(PositioningGrid.ColumnDefinitions[0].ActualWidth + e.HorizontalChange);
        //        PositioningGrid.ColumnDefinitions[2].Width = new GridLength(PositioningGrid.ColumnDefinitions[2].ActualWidth - e.HorizontalChange); ;
        //    }
        //    if (PositioningGrid.RowDefinitions[0].ActualHeight + e.VerticalChange > 0
        //        && PositioningGrid.RowDefinitions[2].ActualHeight - e.VerticalChange > 0)
        //    {
        //        PositioningGrid.RowDefinitions[0].Height = new GridLength(PositioningGrid.RowDefinitions[0].ActualHeight + e.VerticalChange);
        //        PositioningGrid.RowDefinitions[2].Height = new GridLength(PositioningGrid.RowDefinitions[2].ActualHeight - e.VerticalChange);
        //    }
            
        //}

        //private void ResizeDebugger(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        //{
        //    if (PositioningGrid.ColumnDefinitions[2].ActualWidth - e.HorizontalChange > 0)
        //        PositioningGrid.ColumnDefinitions[2].Width = new GridLength(PositioningGrid.ColumnDefinitions[2].ActualWidth - e.HorizontalChange); 
        //    if (PositioningGrid.RowDefinitions[2].ActualHeight - e.VerticalChange > 0)
        //        PositioningGrid.RowDefinitions[2].Height = new GridLength(PositioningGrid.RowDefinitions[2].ActualHeight - e.VerticalChange);

        //}

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            //this.Visibility
        }




	}
}