using System;
using System.Net;
using System.Linq;
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

        protected void BindButtons(FrameworkElement control)
        {

            Button butt = control as Button;
            if (butt != null)
            {
                AttachButton(butt);
                return;
            }

            ContentControl contControl = control as ContentControl;
            if (contControl != null)
            {
                var child = contControl.Content as FrameworkElement;
                if (child != null)
                {
                    BindButtons(child);
                }
                return;
            }


            Panel panel = control as Panel;

            if (panel != null)
            {

                foreach (FrameworkElement p in panel.Children)
                {
                    BindButtons(p);
                }
            }

            
        }

        private void AttachButton(Button b)
        {
            if (!string.IsNullOrEmpty(b.Name))
            {
                b.Click += CommandButton_Click;
            }
        }

        public void CommandButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            SendCommand(button.Name, button.Tag);
            OnCommandSent(button.Name, button.Tag);
        }

        protected virtual void OnCommandSent(string name, object param)
        {
        }
    }
}
