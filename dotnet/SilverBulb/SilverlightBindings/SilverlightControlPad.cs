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
using System.Collections.Generic;

namespace SilverlightBindings
{
    public class SilverlightControlPad : IControlPad
    {

        UserControl boundControl;

        public UserControl BoundControl
        {
            get { return boundControl; }
            set { boundControl = value;
                boundControl.KeyDown += new KeyEventHandler(boundControl_KeyDown);
                boundControl.KeyUp += new KeyEventHandler(boundControl_KeyUp);
            }

        }

        Dictionary<Key, PadValues> NesKeyBindings = new Dictionary<Key, PadValues>();


        public SilverlightControlPad()
        {
            NesKeyBindings = new Dictionary<Key, PadValues>();
            NesKeyBindings.Add(Key.X, PadValues.A);
            NesKeyBindings.Add(Key.Z, PadValues.B);
            NesKeyBindings.Add(Key.W, PadValues.Select);
            NesKeyBindings.Add(Key.Q, PadValues.Start);
            NesKeyBindings.Add(Key.Up, PadValues.Up);
            NesKeyBindings.Add(Key.Down, PadValues.Down);
            NesKeyBindings.Add(Key.Left, PadValues.Left);
            NesKeyBindings.Add(Key.Right, PadValues.Right);

            NesKeyBindings.Add(Key.I, PadValues.Up);
            NesKeyBindings.Add(Key.K, PadValues.Down);
            NesKeyBindings.Add(Key.J, PadValues.Left);
            NesKeyBindings.Add(Key.L, PadValues.Right);
        }

        int PadOneState = 0;

        void boundControl_KeyUp(object sender, KeyEventArgs e)
        {

            if (NesKeyBindings.ContainsKey(e.Key))
            {
                var key = NesKeyBindings[e.Key];
                switch (key)
                {
                    default:
                        PadOneState &= ~(int)key;
                        break;
                }
            }

        }

        void boundControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (NesKeyBindings.ContainsKey(e.Key))
            {
                int key = (int)NesKeyBindings[e.Key];
                PadOneState |= key & 0xFF;
            }
        }

        public int CurrentByte
        {
            get
            {
                return PadOneState;
            }
            set
            {
            }
        }

        public void Refresh()
        {
            NextControlByteSet(this, new ControlByteEventArgs((byte)PadOneState));
        }


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

        public event EventHandler<ControlByteEventArgs> NextControlByteSet;

        public void Dispose()
        {
            
        }
    }
}
