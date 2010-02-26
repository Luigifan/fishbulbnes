using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Machine;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NES.CPU.nitenedo.Interaction;

namespace InstiBulb.WpfKeyboardInput
{
    public enum PadValues : int
    {
        A = 1,
        B = 2,
        Select = 4,
        Start = 8,
        Up = 16,
        Down = 32,
        Left = 64,
        Right = 128,
        FullScreen=256,
    }

    public class WpfKeyboardControlPad : IControlPad,INotifyPropertyChanged, IBindToDisplayContext, IKeyBindingConfigTarget
    {
        int PadOneState = 0;

        DependencyObject handler;
        public WpfKeyboardControlPad()
        {

            NesKeyBindings = new Dictionary<Key, PadValues>();
            NesKeyBindings.Add(Key.X, PadValues.A);
            NesKeyBindings.Add(Key.Z, PadValues.B);
            NesKeyBindings.Add(Key.Space, PadValues.Select);
            NesKeyBindings.Add(Key.Enter, PadValues.Start);
            NesKeyBindings.Add(Key.Up, PadValues.Up);
            NesKeyBindings.Add(Key.Down, PadValues.Down);
            NesKeyBindings.Add(Key.Left, PadValues.Left);
            NesKeyBindings.Add(Key.Right, PadValues.Right);
            
            NesKeyBindings.Add(Key.F12, PadValues.FullScreen);

        }



        public Dictionary<Key, PadValues> NesKeyBindings
        {
            get;
            set;
        }

        public void SetKeyBinding(NesKeyBinding binding)
        {
            if (!NesKeyBindings.ContainsKey(binding.Key))
            {
                NesKeyBindings.Add(binding.Key, binding.BoundValue);
            }
            else
            {
                NesKeyBindings.Remove(binding.Key);
                NesKeyBindings.Add(binding.Key, binding.BoundValue);
            }
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

            if (NesKeyBindings.ContainsKey(e.Key))
            {
                int key = (int)NesKeyBindings[e.Key];
                PadOneState |= key & 0xFF;
            }

        }

        void KeyUpHandler(object sender, KeyEventArgs e)
        {
            if (NesKeyBindings.ContainsKey(e.Key))
            {
                var key = NesKeyBindings[e.Key];
                switch (key)
                {
                    case PadValues.FullScreen:
                        if (DisplayContext != null)
                            DisplayContext.ToggleFullScreen();
                        break;
                    default:
                        PadOneState &= ~(int)key;
                        break;
                }
            }
        }

        public IDisplayContext DisplayContext
        {
            get;
            set;
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

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propName)

        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        #endregion

        #region IControlPad Members

        int readNumber, currentByte;

        public int GetByte(int clock)
        {
            int result = (currentByte >> readNumber) & 0x01;
            readNumber = (readNumber + 1) & 7;
            return (result | 0x40) & 0xFF;
        }

        public void SetByte(int clock, int data)
        {
            if ((data & 1) == 1)
            {
                currentByte = PadOneState;
                // if im pushing up, i cant be pushing down
                if ((currentByte & 16) == 16) currentByte = currentByte & ~32;
                // if im pushign left, i cant be pushing right.. seriously, the nes will glitch
                if ((currentByte & 64) == 64) currentByte = currentByte & ~128;

                readNumber = 0;
            }
            if (data == 0) // strobed this port, get the next byte
            {
            }
        }

        #endregion
    }
}
