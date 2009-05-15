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
	/// Interaction logic for MachineStatus.xaml
	/// </summary>
	public partial class MachineStatus
	{
		public MachineStatus()
		{
			this.InitializeComponent();
		}

        private void Step_Click(object sender, RoutedEventArgs e)
        {
            DebuggerVM viewModel = (this.DataContext as DebuggerVM);
            if (viewModel != null)
            {
                viewModel.DebugTarget.IsDebugging = true;
                viewModel.Step();
            }
        }

        private void StepFrame_Click(object sender, RoutedEventArgs e)
        {
            DebuggerVM viewModel = (this.DataContext as DebuggerVM);
            if (viewModel != null)
            {
                viewModel.DebugTarget.IsDebugging = true;
                viewModel.StepFrame();
            }
        }
	}
}