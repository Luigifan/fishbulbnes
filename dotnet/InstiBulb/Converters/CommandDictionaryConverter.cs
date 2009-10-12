using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using Fishbulb.Common.UI;
using System.Globalization;

namespace InstiBulb.Converters
{
    public class WrappedCommand : ICommand
    {
        ICommandWrapper wrapCommand;
        public WrappedCommand(ICommandWrapper wrapCommand)
        {
            this.wrapCommand = wrapCommand;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return wrapCommand.CanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            wrapCommand.Execute(parameter);
        }

        #endregion
    }

    public class CommandDictionaryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dict = value as Dictionary<string, ICommandWrapper>;
            if (dict != null)
            {
                
                ICommandWrapper realCommand;
                if (dict.TryGetValue(parameter as string, out realCommand))
                {
                    return new WrappedCommand(realCommand);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
