using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Fishbulb.Common.UI;

namespace InstiBulb.Commands
{
    public class DelegateCommand : ICommand
    {
        CommandExecuteHandler exectutor;
        CommandCanExecuteHandler canExecutor;


        public DelegateCommand(CommandExecuteHandler exectutor, CommandCanExecuteHandler canExecutor)
        {
            this.exectutor = exectutor;
            this.canExecutor = canExecutor;
        }


        public void Execute(object param)
        {
            exectutor(param);
        }

        public bool CanExecute(object param)
        {
            return canExecutor(param);
        }

        public void UpdateCanExecute()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }


        public event EventHandler CanExecuteChanged;
    }

}
