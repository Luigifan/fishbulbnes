
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NES.CPU.nitenedo;
using NES.CPU.Machine.FastendoDebugging;

namespace Fishbulb.Common.UI
{
	
	
	public class CPUStatusVM : IViewModel
	{

		NESMachine machine;
		public CPUStatusVM(NESMachine machine)
		{
			this.machine = machine;
			machine.DebugInfoChanged += HandleDebugInfoChanged;
		}

		void HandleDebugInfoChanged(object sender, BreakEventArgs e)
		{
			NotifyPropertyChanged("DataModel");
		}

		#region IViewModel implementation
		public string CurrentView {
			get {
				return "CPUStatus";
			}
		}
		
		Dictionary<string, ICommandWrapper> commands = new Dictionary<string, ICommandWrapper>();
		
		public System.Collections.Generic.Dictionary<string, ICommandWrapper> Commands {
			get {
				return commands;
			}
		}
		
		public System.Collections.Generic.IEnumerable<IViewModel> ChildViewModels {
			get {
				return new List<IViewModel>();
			}
		}
		
		public string CurrentRegion {
			get {
				return "debugger.0";
			}
		}
		
		public string Header {
			get {
				return "CPU Status";
			}
		}
		
		public object DataModel {
			get {
				if (machine.DebugInfo != null)
					return machine.DebugInfo.CPU;
				else
					return null;
			}
		}

		#endregion

		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		
        public void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

		#endregion
	}
}
