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
using InstiBulb.Commanding;
using System.Threading;

namespace SlimDXBindings
{
    public class SlimDXKeyboardControlPad : IControlPad, IDisposable, IKeyBindingConfigTarget, ISendCommands
    {
        #region IControlPad Members

        DirectInput dInput = new DirectInput();
        Keyboard keyboard;

        bool exclusive = false, foreground = true, disable = false;

        public class AvailableCommand
        {
            string viewModel;

            public string ViewModel
            {
                get { return viewModel; }
                set { viewModel = value; }
            }

            string command;

            public string Command
            {
                get { return command; }
                set { command = value; }
            }
        }

        Dictionary<Key, AvailableCommand> _keyCommandBindings = new Dictionary<Key, AvailableCommand>();


        public Dictionary<Key, AvailableCommand> KeyCommandBindings
        {
            get 
            { 
                return _keyCommandBindings; 
            }
        }


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

        private DXKeyToWpfKey keyConverter = new DXKeyToWpfKey();

        Timer timer;

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

            _keyCommandBindings.Add(Key.F12, new AvailableCommand() { ViewModel = "DisplayViewModel", Command = "FullScreenCommand" });
            _keyCommandBindings.Add(Key.Backslash, new AvailableCommand() { ViewModel = "SoundPanel", Command = "MuteToggle" });
            _keyCommandBindings.Add(Key.Pause, new AvailableCommand() { ViewModel = "ControlPanel", Command = "PauseToggle" });

            RemapKeysToDX();

            timer = new Timer(new TimerCallback(RefreshKeys), null, TimeSpan.Zero, new TimeSpan(0,0,0,0,16) );
            
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
        }

        Key cmdKey = Key.Unknown;

        public void RefreshKeys(object o)
        {
            try
            {
                if (keyboard == null)
                    return;

                if (keyboard.Acquire().IsFailure)
                    return;

                if (keyboard.Poll().IsFailure)
                    return;

                keyboard.GetCurrentState(ref state);
                if (Result.Last.IsFailure)
                    return;

                PadOneState = 0;

                foreach (Key key in state.PressedKeys)
                {
                    if (DXKeyBindings.ContainsKey(key))
                    {
                        PadValues val = DXKeyBindings[key];
                        switch (val)
                        {
                            default:
                                PadOneState |= (int)val & 0xFF;
                                break;
                        }
                    }

                    if (_keyCommandBindings.ContainsKey(key))
                    {
                        cmdKey = key;
                    }

                }

                if (cmdKey != Key.Unknown && state.ReleasedKeys.Contains(cmdKey))
                {
                    string vm = _keyCommandBindings[cmdKey].ViewModel;
                    string cmd = _keyCommandBindings[cmdKey].Command;
                    if (this.CommandSender != null && this.CommandSender.CanExecuteCommand(vm, cmd, null))
                        this.CommandSender.ExecuteCommand(vm, cmd, null);
                    cmdKey = Key.Unknown;
                }

                if (NextControlByteSet != null)
                    NextControlByteSet(this, new ControlByteEventArgs((byte)PadOneState));
            } catch(Exception e)
            {
                Console.WriteLine("Error in keyboard loop");
                Console.WriteLine(e.ToString());
            }
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
            timer.Dispose();
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

        public CommandSender CommandSender
        {
            get;
            set;
        }
    }
}
