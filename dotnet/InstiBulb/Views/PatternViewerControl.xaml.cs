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
using System.ComponentModel;

namespace InstiBulb.Views
{
	/// <summary>
	/// Interaction logic for PatternViewerControl.xaml
	/// </summary>
	public partial class PatternViewerControl : INotifyPropertyChanged
	{

        public static DependencyProperty CurrentTileProperty =
            DependencyProperty.Register("CurrentTile", typeof(TileInformation), typeof(PatternViewerControl));

        public TileInformation CurrentTile
        {
            get { return (TileInformation)GetValue(CurrentTileProperty); }
            set { SetValue(CurrentTileProperty, value); }
        }

		public PatternViewerControl()
		{
			this.InitializeComponent();
		}

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var p = DataContext as WinDebuggerVM;
            var img = sender as Image;
            TileInformation tile; 

            if (p != null && img != null)
            {
                Point pt = e.GetPosition(img);
                int x = (int)((pt.X / img.ActualWidth) * 16.0);
                int y = (int)((pt.Y / img.ActualHeight) * 16.0);


                CurrentTile = p.GetTileInfo(0, x +  y * 16);

                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CurrentTile"));

            }
        }



        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}