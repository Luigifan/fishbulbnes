using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace InstiBulb.WinViewModels
{
    public class DisplayNameTablesCommand : ICommand
    {

        readonly WinDebuggerVM debuggerVM;

        public DisplayNameTablesCommand(WinDebuggerVM debuggerVM)
        {
            this.debuggerVM = debuggerVM;
        }


        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            debuggerVM.DrawNameTableZero();
            debuggerVM.DrawNameTableOne();
            debuggerVM.DrawNameTableTwo();
            debuggerVM.DrawNameTableThree();
        }


        #endregion
    }

}
