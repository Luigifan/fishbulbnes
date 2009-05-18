using NES.CPU.nitenedo;
using System;
using System.Collections.Generic;

using Fishbulb.Common.UI;

namespace GtkNes
{
	
	
	public class FutureInstructions : IProfileViewModel
	{

		NESMachine machine;
		public FutureInstructions(NESMachine machine)
		{
			this.machine = machine;
			machine.DebugInfoChanged+= (o,a) => Console.WriteLine(a);
		}
		
		#region IProfileViewModel implementation
		public string CurrentView {
			get {
			 	return "AsmViewer";
			}
		}
		
		Dictionary<string, ICommandWrapper> commands = new Dictionary<string, ICommandWrapper>();
		
		public Dictionary<string, ICommandWrapper> Commands {
			get {
				return commands;
			}
		}
		
		
		public System.Collections.Generic.IEnumerable<IProfileViewModel> ChildViewModels {
			get {
				return new List<IProfileViewModel>();
			}
		}
		
		public string CurrentRegion {
			get {
			 return "debugger.0";
			}
		}
		
		public string Header {
			get {
				return "Future Instructions";
			}
		}
		
		public object DataModel {
			get {
				return machine.DebugInfo;
			}
		}

		#endregion

		#region INotifyPropertyChanged implementation
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		public void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged!=null)
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
		}
		#endregion
		
	}
}
