using NES.CPU.nitenedo;
using System;
using System.Linq;
using System.Collections.Generic;
using NES.CPU.Machine.FastendoDebugging;
using Fishbulb.Common.UI;
using NES.CPU.FastendoDebugging;

namespace fishbulbcommonui
{
	
	/// <summary>
	/// Encapsulates the future instruction rollout
	/// </summary>
    public class FutureInstructions : BaseNESViewModel
	{

        protected override void OnAttachTarget()
        {
			TargetMachine.DebugInfoChanged += HandleDebugInfoChanged;
		}

		void HandleDebugInfoChanged(object sender, BreakEventArgs e)
		{
			Console.WriteLine("FutureInstructions.HandleDebugInfoChanged " );
			NotifyPropertyChanged("DataModel");	
		}

        public override string CurrentView
        {
			get {
			 	return "AsmViewer";
			}
		}

        public override string CurrentRegion
        {
			get {
			 return "debugger.0";
			}
		}
		
		public override string Header {
			get {
				return "Future Instructions";
			}
		}
		
		public List<string> DataModel {
			get {
                if (TargetMachine != null && TargetMachine.DebugInfo != null)
                    return (from op in TargetMachine.DebugInfo.FutureOps select op.ToString()).ToList<string>();
				else
					return new List<string>();
			}
		}


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
