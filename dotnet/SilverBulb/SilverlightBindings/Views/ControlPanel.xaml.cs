﻿using System;
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
    public partial class ControlPanel : CommandingUserControl
    {
        public ControlPanel()
        {
            InitializeComponent();
            
            
        }

        public void CommandButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            SendCommand(button.Name, button.Tag);
        }


    }
}
