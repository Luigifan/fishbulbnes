using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Fishbulb.Common.UI;

namespace InstiBulb.WinViewModels
{


    public class DisplayTilesCommand : ICommand
    {

        readonly WinDebuggerVM debuggerVM;

        public DisplayTilesCommand(WinDebuggerVM debuggerVM)
        {
            this.debuggerVM = debuggerVM;
        }



        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void UpdateCanExecute()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            debuggerVM.DrawPatternTableZero();
            debuggerVM.DrawPatternTableOne();
        }


        #endregion
    }
}
