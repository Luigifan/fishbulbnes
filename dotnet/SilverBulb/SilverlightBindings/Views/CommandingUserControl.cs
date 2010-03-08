using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Fishbulb.Common.UI;

namespace SilverlightBindings.Views
{
    public class CommandingUserControl : UserControl
    {
        public CommandingUserControl()
        {
        }

        public void SendCommand(string name, object parameter)
        {
            var dc = DataContext as IViewModel;
            if (dc != null)
            {
                if (dc.Commands.ContainsKey(name))
                {
                    ICommandWrapper command = dc.Commands[name];
                    if (command.CanExecute(parameter))
                    {
                        command.Execute(parameter);
                    }
                }
            }
        }



    }
}
