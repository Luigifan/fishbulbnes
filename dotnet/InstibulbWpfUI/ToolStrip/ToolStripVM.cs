using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace InstiBulb.ToolStrip
{
    public class ToolStripVM : INotifyPropertyChanged
    {
        public UIElement DisplayedIcon
        {
            get;
            set;
        }

        public FrameworkElement ActivePanel
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
