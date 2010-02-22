using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace InstiBulb.Windowing
{
    internal class WindowTracker
    {
        public Window Window
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public double Width
        {
            get;
            set;
        }

        public double Height
        {
            get;
            set;
        }

        public double Left
        {
            get;
            set;
        }

        public double Right
        {
            get;
            set;
        }
        
    }
}
