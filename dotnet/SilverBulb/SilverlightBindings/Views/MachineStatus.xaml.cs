using System;
using System.Linq;
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
using Fishbulb.Common.UI;
using SilverlightBindings.Views;

namespace InstiBulb
{
	/// <summary>
	/// Interaction logic for MachineStatus.xaml
	/// </summary>
    public partial class MachineStatus : CommandingUserControl
	{
		public MachineStatus()
		{
			this.InitializeComponent();
            BindButtons(this.LayoutRoot as Panel);
		}

	}
}