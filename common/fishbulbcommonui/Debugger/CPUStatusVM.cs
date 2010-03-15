
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NES.CPU.nitenedo;
using NES.CPU.Machine.FastendoDebugging;
using NES.CPU.FastendoDebugging;
using fishbulbcommonui;
using FishBulb;

namespace Fishbulb.Common.UI
{


    public class CPUStatusVM : BaseNESViewModel
	{


        public CPUStatusVM(IPlatformDelegates delegates) : base(delegates) { }


		protected override void  OnAttachTarget()
		{

			TargetMachine.DebugInfoChanged += HandleDebugInfoChanged;
		}

		void HandleDebugInfoChanged(object sender, BreakEventArgs e)
		{
			UpdateDebugInfo();
			NotifyPropertyChanged("DataModel");
		}

		private string[] debugInfo = new string[9];
		
		private void UpdateDebugInfo()
		{
            if (TargetMachine.DebugInfo != null && TargetMachine.DebugInfo.CPU != null)
			{
                debugInfo[0] = string.Format("Accumulator: {0}", TargetMachine.DebugInfo.CPU.Accumulator);
                debugInfo[1] = string.Format("IndX: {0}", TargetMachine.DebugInfo.CPU.IndexRegisterX);
                debugInfo[2] = string.Format("IndY: {0}", TargetMachine.DebugInfo.CPU.IndexRegisterY);
                debugInfo[3] = string.Format("PC: {0}", TargetMachine.DebugInfo.CPU.ProgramCounter);
                debugInfo[4] = string.Format("SR: {0}", TargetMachine.DebugInfo.CPU.StatusRegister);
                debugInfo[5] = string.Format("SP: {0}", TargetMachine.DebugInfo.CPU.StackPointer);
                debugInfo[6] = string.Format("Current Op: {0}", TargetMachine.DebugInfo.CPU.CurrentInstruction.Disassemble());
                debugInfo[7] = string.Format("Last Address: {0}", TargetMachine.DebugInfo.CPU.CurrentInstruction.Address);
                debugInfo[8] = string.Format("Last Op: {0}", TargetMachine.DebugInfo.CPU.LastInstruction);
				
				
			}
		}
		
		#region IViewModel implementation
		public override string CurrentView {
			get {
				return "CPUStatus";
			}
		}


        public override string CurrentRegion
        {
			get {
				return "debugger.0";
			}
		}

        public override string Header
        {
			get {
				return "CPU Status";
			}
		}
		
		private string[] DataModel {
			get {
				return debugInfo;
			}
		}

		#endregion


	}
}
