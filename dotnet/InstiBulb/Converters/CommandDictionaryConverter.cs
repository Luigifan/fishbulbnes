using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using Fishbulb.Common.UI;
using System.Globalization;
using System.ComponentModel;

namespace InstiBulb.Converters
{
    public class WrappedCommandErrorStateManager
    {
        Dictionary<string, Exception> commandResults = new Dictionary<string, Exception>();
        public event EventHandler ErrorStateChanged;

        public void AddError(string commandName, Exception ErrorText)
        {
            if (commandResults.ContainsKey(commandName))
                commandResults.Remove(commandName);
            commandResults.Add(commandName, ErrorText);

            if (ErrorStateChanged != null)
            {
                ErrorStateChanged(this, EventArgs.Empty);
            }
        }

        public string GetErrorText(string commandName)
        {
            if (commandResults.ContainsKey(commandName))
                return commandResults[commandName] == null ? null : commandResults[commandName].Message;

            return null;
        }
    }

    public class WrappedCommand : ICommand
    {
        readonly WrappedCommandErrorStateManager manager;
        readonly ICommandWrapper wrapCommand;
        readonly string commandName;

        public WrappedCommand(string name, ICommandWrapper wrapCommand, WrappedCommandErrorStateManager manager)
        {
            commandName = name;
            this.wrapCommand = wrapCommand;
            this.manager = manager;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return wrapCommand.CanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Exception error = null;
            try
            {

                wrapCommand.Execute(parameter);
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                manager.AddError(commandName, error);
            }
        }

        #endregion
    }

    public class CommandDictionaryConverter : IValueConverter, INotifyPropertyChanged
    {
        public CommandDictionaryConverter()
        {
            errorTextManager.ErrorStateChanged +=new EventHandler(errorTextManager_ErrorStateChanged);
        }

        void  errorTextManager_ErrorStateChanged(object sender, EventArgs e)
        {
            if (PropertyChanged != null) 
                PropertyChanged(this, new PropertyChangedEventArgs("ErrorTextManager"));
        }

        private WrappedCommandErrorStateManager errorTextManager = new WrappedCommandErrorStateManager();

        public WrappedCommandErrorStateManager ErrorTextManager
        {
            get { return errorTextManager; }
            set { errorTextManager = value; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string commandName = parameter as string;
            if (commandName == null) return null;
            
            var dict = value as Dictionary<string, ICommandWrapper>;
            if (dict != null)
            {
                
                ICommandWrapper realCommand;
                if (dict.TryGetValue(commandName, out realCommand))
                {
                    return new WrappedCommand(commandName, realCommand, errorTextManager);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler  PropertyChanged;

        #endregion
}

    public class ErrorTextConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// Converts a ErrorTextManager object, into displayable error results, based on command name
        /// </summary>
        /// <param name="value">a ErrorTextManager object</param>
        /// <param name="targetType">string</param>
        /// <param name="parameter">command name (string)</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var manager = value as WrappedCommandErrorStateManager;
            var cmdName = parameter as string;

            if (manager != null && cmdName != null)
            {
                return manager.GetErrorText(cmdName);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


}
