using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Fishbulb.Common.UI;

namespace SilverlightBindings.Views
{
    public partial class ControlPanel : UserControl
    {
        public ControlPanel()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var dc = DataContext as IViewModel;
            if (button != null && dc != null)
            {
                if (dc.Commands.ContainsKey(button.Name))
                {
                    ICommandWrapper command = dc.Commands[button.Name];
                    if (command.CanExecute(button.Tag))
                    {
                        command.Execute(button.Tag);
                    }
                }
            }
        }
    }
}
