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
using NES.CPU.Machine;

namespace SilverlightBindings
{
    public class SilverlightControlPad : IControlPad
    {

        public int CurrentByte
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public void Refresh()
        {
        }

        public int GetByte(int clock)
        {
            return 0;
        }

        public void SetByte(int clock, int data)
        {
            
        }

        public event EventHandler<ControlByteEventArgs> NextControlByteSet;

        public void Dispose()
        {
            
        }
    }
}
