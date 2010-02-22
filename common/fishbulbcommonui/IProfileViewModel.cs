using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using NES.CPU.nitenedo;

namespace Fishbulb.Common.UI
{
	public interface ICommandWrapper 
	{
		void Execute(object param);
		bool CanExecute(object param);
	}
	
	
    public interface IViewModel: INotifyPropertyChanged
    {
        /// <summary>
        /// specifies the view to generate for this viewmodel
        /// </summary>
        string CurrentView
        {
            get;
        }

        Dictionary<string, ICommandWrapper> Commands { get; }

        IEnumerable<IViewModel> ChildViewModels
        {
            get;
        }

        /// <summary>
        /// specifies the region of the profile host in which to place this view
        /// </summary>
        string CurrentRegion
        {
            get;
        }

        string Header
        {
            get;
        }

        NESMachine TargetMachine
        {
            get;
            set;
        }
    }
}
