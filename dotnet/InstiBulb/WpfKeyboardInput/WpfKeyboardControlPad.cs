using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Machine;
using System.Windows;
using System.Windows.Input;

namespace InstiBulb.WpfKeyboardInput
{
    public class WpfKeyboardControlPad : IControlPad
    {
        int PadOneState = 0;

        DependencyObject handler;
        public WpfKeyboardControlPad()
        {

        }

        public DependencyObject Handler
        {
            get { return handler; }
            set
            {
                this.handler = value;
                Keyboard.AddPreviewKeyDownHandler(handler, KeyDownHandler);
                Keyboard.AddPreviewKeyUpHandler(handler, KeyUpHandler);
            }
        }

        void KeyDownHandler(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.X:
                    PadOneState = PadOneState | 1;
                    break;
                case Key.Z:
                    PadOneState = PadOneState | 2;
                    break;
                case Key.W:
                    PadOneState = PadOneState | 4;
                    break;
                case Key.Q:
                    PadOneState = PadOneState | 8;
                    break;
                case Key.Up:
                    PadOneState = PadOneState | 16;
                    PadOneState = PadOneState & ~32;
                    break;
                case Key.Down:
                    PadOneState = PadOneState | 32;
                    PadOneState = PadOneState & ~16;
                    break;
                case Key.Left:
                    PadOneState = PadOneState | 64;
                    PadOneState = PadOneState & ~128;
                    break;
                case Key.Right:
                    PadOneState = PadOneState | 128;
                    PadOneState = PadOneState & ~64;
                    break;
            }
        }

        void KeyUpHandler(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.X:
                    PadOneState = PadOneState & ~1;
                    break;
                case Key.Z:
                    PadOneState = PadOneState & ~2;
                    break;
                case Key.W:
                    PadOneState = PadOneState & ~4;
                    break;
                case Key.Q:
                    PadOneState = PadOneState & ~8;
                    break;
                case Key.Up:
                    PadOneState = PadOneState & ~16;
                    break;
                case Key.Down:
                    PadOneState = PadOneState & ~32;
                    break;
                case Key.Left:
                    PadOneState = PadOneState & ~64;
                    break;
                case Key.Right:
                    PadOneState = PadOneState & ~128;
                    break;
            }
        }

        #region IControlPad Members

        public int CurrentByte
        {
            get
            {
                return PadOneState;
            }
            set
            {
                PadOneState =value;
            }
        }

        public void Refresh()
        {
            NextControlByteSet(this, new ControlByteEventArgs((byte)PadOneState));
        }

        public event EventHandler<ControlByteEventArgs> NextControlByteSet = delegate { };

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Keyboard.RemovePreviewKeyDownHandler(handler, KeyDownHandler);
            Keyboard.RemovePreviewKeyUpHandler(handler, KeyUpHandler);
        }

        #endregion
    }
}
