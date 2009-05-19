using NES.CPU.nitenedo;
using System;
using System.Linq;
using System.Collections.Generic;
using NES.CPU.Machine.FastendoDebugging;
using Fishbulb.Common.UI;
using Fishbulb.Common.Disassembly;

namespace GtkNes
{
	
	/// <summary>
	/// Encapsulates the future instruction rollout
	/// </summary>
	public class FutureInstructions : IViewModel
	{

		NESMachine machine;
		public FutureInstructions(NESMachine machine)
		{
			this.machine = machine;
			this.machine.DebugInfoChanged += HandleDebugInfoChanged;
			
		}

		void HandleDebugInfoChanged(object sender, BreakEventArgs e)
		{
			Console.WriteLine("FutureInstructions.HandleDebugInfoChanged " );
			NotifyPropertyChanged("DataModel");	
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
				return "Future Instructions";
			}
		}
		
		public object DataModel {
			get {
				if (machine != null && machine.DebugInfo != null)
					return from op in machine.DebugInfo.FutureOps select op.ToString() + " " + op.Instruction.Disassemble() ;
				else
					return new List<string>();
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
