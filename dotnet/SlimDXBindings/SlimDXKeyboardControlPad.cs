using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Machine;
using SlimDX.DirectInput;
using System.Windows.Interop;
using SlimDX;
using System.Windows;
using NES.CPU.nitenedo.Interaction;
using SlimDXBindings.Viewer10;
using InstiBulb.WpfKeyboardInput;

namespace SlimDXBindings
{
    public class SlimDXKeyboardControlPad : IControlPad, IDisposable, IBindToDisplayContext,  IKeyBindingConfigTarget
    {
        #region IControlPad Members

        DirectInput dInput = new DirectInput();
        Keyboard keyboard;

        bool exclusive = false, foreground = true, disable = false;


        public Dictionary<Key, PadValues> DXKeyBindings
        {
            get;
            set;
        }

        public Dictionary<System.Windows.Input.Key, PadValues> NesKeyBindings
        {
            get;
            set;
        }

        //SlimDXBindings.Viewer10.DirectX10NesViewer viewer;

        //public SlimDXBindings.Viewer10.DirectX10NesViewer Viewer
        //{
        //    get { return viewer; }
        //    set { 
        //        viewer = value;
        //        if (viewer is DirectX10NesViewer)
        //        {
        //            CreateDevice(null);
        //        }
        //    }
        //}

        private DXKeyToWpfKey keyConverter = new DXKeyToWpfKey();

        public SlimDXKeyboardControlPad()
        {
            // make sure that DirectInput has been initialized

            
            keyboard = new Keyboard(dInput);

            DXKeyBindings = new Dictionary<Key, PadValues>();
            NesKeyBindings = new Dictionary<System.Windows.Input.Key, PadValues>();
            
            NesKeyBindings.Add(System.Windows.Input.Key.X, PadValues.A);
            NesKeyBindings.Add(System.Windows.Input.Key.Z, PadValues.B);
            NesKeyBindings.Add(System.Windows.Input.Key.Space, PadValues.Select);
            NesKeyBindings.Add(System.Windows.Input.Key.Enter, PadValues.Start);
            NesKeyBindings.Add(System.Windows.Input.Key.Up, PadValues.Up);
            NesKeyBindings.Add(System.Windows.Input.Key.Down, PadValues.Down);
            NesKeyBindings.Add(System.Windows.Input.Key.Left, PadValues.Left);
            NesKeyBindings.Add(System.Windows.Input.Key.Right, PadValues.Right);

            NesKeyBindings.Add(System.Windows.Input.Key.F12, PadValues.FullScreen);

            RemapKeysToDX();
        }

        private void RemapKeysToDX()
        {
            DXKeyBindings.Clear();
            foreach (var p in NesKeyBindings)
            {
                UpdateKeyBinding(new NesKeyBinding() { BoundValue = p.Value, Key = p.Key });
            }
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
                IntPtr windowHandle = IntPtr.Zero;
                WindowInteropHelper helper = new WindowInteropHelper(host);

                windowHandle = helper.Handle;
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
            if (keyboard == null)
                return;

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
                if (DXKeyBindings.ContainsKey(key))
                {
                    PadValues val = DXKeyBindings[key];
                    switch (val)
                    {
                        case PadValues.FullScreen:
                            if (DisplayContext != null)
                                DisplayContext.ToggleFullScreen();
                            break;
                        default:
                            PadOneState |= (int)val & 0xFF;
                            break;
                    }
                }
                //switch (key)
                //{
                //    case Key.X:
                //        PadOneState = PadOneState | 1;
                //        break;
                //    case Key.Z:
                //        PadOneState = PadOneState | 2;
                //        break;
                //    case Key.Space:
                //        PadOneState = PadOneState | 4;
                //        break;
                //    case Key.Return:
                //        PadOneState = PadOneState | 8;
                //        break;
                //    case Key.UpArrow:
                //        PadOneState = PadOneState | 16;
                //        PadOneState = PadOneState & ~32;
                //        break;
                //    case Key.DownArrow:
                //        PadOneState = PadOneState | 32;
                //        PadOneState = PadOneState & ~16;
                //        break;
                //    case Key.LeftArrow:
                //        PadOneState = PadOneState | 64;
                //        PadOneState = PadOneState & ~128;
                //        break;
                //    case Key.RightArrow:
                //        PadOneState = PadOneState | 128;
                //        PadOneState = PadOneState & ~64;
                //        break;
                //}
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

        public void Dispose()
        {
            ReleaseDevice();
            dInput.Dispose();
        }

        #endregion

        #region IControlPad Members

        
        int readNumber;

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

        public IDisplayContext DisplayContext
        {
            get;
            set;
        }

        public void SetKeyBinding(NesKeyBinding binding)
        {
            NesKeyBindings.Add(binding.Key, binding.BoundValue);
            RemapKeysToDX();
        }

        void UpdateKeyBinding(NesKeyBinding binding)
        {
            var p = keyConverter.ConvertWPFKey(binding.Key);

            if (p != null)
            {
                if (DXKeyBindings.ContainsKey(p.Value))
                    DXKeyBindings.Remove(p.Value);
                DXKeyBindings.Add(p.Value, binding.BoundValue);
            }

        }
    }
}
