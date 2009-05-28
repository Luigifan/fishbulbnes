
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NES.CPU.nitenedo;
using NES.CPU.Machine.FastendoDebugging;
using NES.CPU.FastendoDebugging;

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
			UpdateDebugInfo();
			NotifyPropertyChanged("DataModel");
		}

		private string[] debugInfo = new string[9];
		
		private void UpdateDebugInfo()
		{
			if (machine.DebugInfo != null && machine.DebugInfo.CPU != null)
			{
				debugInfo[0]=  string.Format( "Accumulator: {0}" , machine.DebugInfo.CPU.Accumulator);
				debugInfo[1]=  string.Format( "IndX: {0}" , machine.DebugInfo.CPU.IndexRegisterX);
				debugInfo[2]=  string.Format( "IndY: {0}" , machine.DebugInfo.CPU.IndexRegisterY);
				debugInfo[3]=  string.Format( "PC: {0}" , machine.DebugInfo.CPU.ProgramCounter);
				debugInfo[4]=  string.Format( "SR: {0}" , machine.DebugInfo.CPU.StatusRegister);
				debugInfo[5]=  string.Format( "SP: {0}" , machine.DebugInfo.CPU.StackPointer);
				debugInfo[6]=  string.Format( "Current Op: {0}" , machine.DebugInfo.CPU.CurrentInstruction.Disassemble());
				debugInfo[7]=  string.Format( "Last Address: {0}" , machine.DebugInfo.CPU.CurrentInstruction.Address);
				debugInfo[8]=  string.Format( "Last Op: {0}" , machine.DebugInfo.CPU.LastInstruction);
				
				
			}
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
				return debugInfo;
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
