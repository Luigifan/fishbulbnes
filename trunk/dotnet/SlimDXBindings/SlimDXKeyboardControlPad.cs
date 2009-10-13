﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Machine;
using SlimDX.DirectInput;
using System.Windows.Interop;
using SlimDX;
using System.Windows;

namespace SlimDXBindings
{
    public class SlimDXKeyboardControlPad: IControlPad, IDisposable
    {
        #region IControlPad Members

        DirectInput dInput = new DirectInput();
        Device<KeyboardState> keyboard;

        bool exclusive = false, foreground = true, disable = false;

        public SlimDXKeyboardControlPad()
        {
            // make sure that DirectInput has been initialized
            
            
            keyboard = new Device<KeyboardState>(dInput, SystemGuid.Keyboard);
            
        }

        public void CreateDevice(Window host)
        {

            // build up cooperative flags
            CooperativeLevel cooperativeLevel;

            if (exclusive)
                cooperativeLevel = CooperativeLevel.Exclusive;
            else
                cooperativeLevel = CooperativeLevel.Nonexclusive;

            if (foreground)
                cooperativeLevel |= CooperativeLevel.Foreground;
            else
                cooperativeLevel |= CooperativeLevel.Background;

            if (disable)
                cooperativeLevel |= CooperativeLevel.NoWinKey;

            // create the device
            try
            {
                WindowInteropHelper helper = new WindowInteropHelper(host);

                IntPtr windowHandle = helper.Handle;
                
                keyboard.SetCooperativeLevel(windowHandle, cooperativeLevel);
            }
            catch (DirectInputException e)
            {
                System.Windows.MessageBox.Show(e.Message);
                return;
            }
            keyboard.Acquire();
        }

        int PadOneState=0;

        KeyboardState state = new KeyboardState();
        
        public void Refresh()
        {
            if (keyboard.Acquire().IsFailure)
                return ;

            if (keyboard.Poll().IsFailure)
                return ;
            
            keyboard.GetCurrentState(ref state);
            if (Result.Last.IsFailure)
                return ;

            PadOneState = 0;

            foreach (Key key in state.PressedKeys)
            {
                switch (key)
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
                    case Key.UpArrow:
                        PadOneState = PadOneState | 16;
                        PadOneState = PadOneState & ~32;
                        break;
                    case Key.DownArrow:
                        PadOneState = PadOneState | 32;
                        PadOneState = PadOneState & ~16;
                        break;
                    case Key.LeftArrow:
                        PadOneState = PadOneState | 64;
                        PadOneState = PadOneState & ~128;
                        break;
                    case Key.RightArrow:
                        PadOneState = PadOneState | 128;
                        PadOneState = PadOneState & ~64;
                        break;
                }
            }

            if (NextControlByteSet != null)
                NextControlByteSet(this, new ControlByteEventArgs((byte) PadOneState));
        }

        void ReleaseDevice()
        {
            if (keyboard != null)
            {
                keyboard.Unacquire();
                keyboard.Dispose();
            }
            keyboard = null;
        }

        int currentByte;

        public int CurrentByte
        {
            get
            {
                return PadOneState;
            }
            set
            {
                PadOneState = value;
            }
        }


        public event EventHandler<ControlByteEventArgs> NextControlByteSet;

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            ReleaseDevice();
            dInput.Dispose();
        }

        #endregion
    }
}
