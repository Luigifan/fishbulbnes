using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Fishbulb.Common.UI;

namespace InstiBulb.Commanding
{
    public class CommandSender
    {
        public CommandSender()
        {
        }

        public IUnityContainer Container
        {
            get;
            set;
        }

        public bool CanExecuteCommand(string ViewModelName, string CommandName, object param)
        {
            if (Container == null) return false;
            try
            {
                IViewModel viewModel = Container.Resolve<IViewModel>(ViewModelName);
                if (viewModel == null) return false;
                if (viewModel.Commands.ContainsKey(CommandName))
                {
                    return viewModel.Commands[CommandName].CanExecute(param);
                }
                else
                {
                    return false;
                }
            }
            catch (ResolutionFailedException)
            {
                
            }
            return false;
        }

        public void ExecuteCommand(string ViewModelName, string CommandName, object param)
        {
            if (Container == null) return ;
            try
            {
                IViewModel viewModel = Container.Resolve<IViewModel>(ViewModelName);
                if (viewModel == null) return;
                if (viewModel.Commands.ContainsKey(CommandName))
                {
                    viewModel.Commands[CommandName].Execute(param);
                }
            }
            catch (ResolutionFailedException)
            {
            }
        }
    }
}
