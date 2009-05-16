
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Fishbulb.Common.UI;
using NES.Machine;
using NES.CPU.nitenedo;

namespace Fishbulb.Common.UI
{
	
	
	public class ControlPanelVM : IProfileViewModel
	{

		#region IProfileViewModel implementation
		public string CurrentView {
			get {
				return "FrontPanel";
			}
		}
		
		public Dictionary<string, ICommandWrapper> Commands {
			get {
				return new Dictionary<string, ICommandWrapper>();
			}
		}
		
		public IEnumerable<IProfileViewModel> ChildViewModels {
			get {
				return new IProfileViewModel[0];
			}
		}
		
		public string CurrentRegion {
			get {
				return "controlPanel.0";
			}
		}
		
		public string Header {
			get {
				return null;
			}
		}
		
		public object DataModel {
			get {
				throw new System.NotImplementedException();
			}
		}

		#endregion

		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		
		private void NotifyPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}
		#endregion
		
		public ControlPanelVM(NESMachine machine)
		{
		}
		
		
	}
}
