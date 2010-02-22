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

namespace InstiBulb
{
	/// <summary>
    /// Interaction logic for NameTableViewerControl.xaml
	/// </summary>
	public partial class NameTableViewerControl
	{
        public NameTableViewerControl()
		{
			this.InitializeComponent();
		}

        //private void DisplayTiles(object sender, RoutedEventArgs e)
        //{
        //    DebuggerVM viewModel = (this.DataContext as DebuggerVM);
        //    imgPatternTableZero.Source = viewModel.DrawNameTableZero();
        //    imgPatternTableOne.Source = viewModel.DrawNameTableOne();
        //    imgPatternTableZero1.Source = viewModel.DrawNameTableTwo();
        //    imgPatternTableOne1.Source = viewModel.DrawNameTableThree();
        //}
	}
}