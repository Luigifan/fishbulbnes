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
	/// Interaction logic for BreakpointsGridControl.xaml
	/// </summary>
	public partial class BreakpointsGridControl
	{
		public BreakpointsGridControl()
		{
			this.InitializeComponent();
		}

        private void AddBreakpoint(object sender, RoutedEventArgs e)
        {
            DebuggerVM viewModel = (this.DataContext as DebuggerVM);
            viewModel.AddBreakpoint(int.Parse(this.BreakpointAddress.Text, System.Globalization.NumberStyles.HexNumber));
        }
	}
}