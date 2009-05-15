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
	/// Interaction logic for InstructionRolloutControl.xaml
	/// </summary>
	public partial class InstructionRolloutControl
	{
		public InstructionRolloutControl()
		{
			this.InitializeComponent();
		}

        private void UpdateRollout(object sender, RoutedEventArgs e)
        {
            //DebuggerVM viewModel = ((this.FindResource("DebuggerVMDS") as ObjectDataProvider).ObjectInstance as DebuggerVM);
            //            viewModel.DebuggerInformation.UpdateFutureRollout();
        }
	}
}