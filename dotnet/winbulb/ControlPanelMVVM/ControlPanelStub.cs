using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.Machine.ControlPanel;
using NES.CPU.nitenedo;
using Microsoft.Win32;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using NES.CPU.Machine.BeepsBoops;
using InstiBulb.ControlPanelMVVM.SoundUI;
using InstiBulb.Sound;

namespace InstiBulb.ControlPanelMVVM
{
    public class ControlPanelModel : IFrontControlPanel
    {

        CartInfo _cartInfo;

        public CartInfo CartInfo
        {
            get { return _cartInfo; }
            set { _cartInfo = value; }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IFrontControlPanel Members

        public ControlPanelModel(NESMachine target, IWavStreamer streamer)
        {
            _target = target;
            _target.SRAMWriter += WriteSRAM;
            _target.SRAMReader += ReadSRAM;
            soundControls = new SoundController(_target, streamer);
        }

        private void WriteSRAM(string romID, byte[] sram)
        {
            string fileName = 
                Path.Combine(
                System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "JoeNES");

            if (!Directory.Exists(fileName))
            {
                Directory.CreateDirectory(fileName);
            }
            fileName = Path.Combine(fileName, romID + ".sram");

            using (BinaryWriter writer = new BinaryWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write)))
            {
                writer.Write(sram);
                writer.Flush();
            }
        }

        private byte[] ReadSRAM(string romID)
        {
            string fileName =
                Path.Combine(
                System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "JoeNES");

            if (!Directory.Exists(fileName))
            {
                Directory.CreateDirectory(fileName);
            }
            fileName = Path.Combine(fileName, romID + ".sram");
            
            byte[] sram = new byte[0x2000];

            try
            {
                using (BinaryReader reader = new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read)))
                {
                    reader.Read(sram, 0, 0x2000);
                }
            }
            catch (FileNotFoundException)
            {
                // do nothing, sram will be created later
            }
            return sram;
        }

        private bool CanChangeState(RunningStatuses from, RunningStatuses to)
        {
            return false;
        }

        private readonly NESMachine _target;

        public NESMachine Target
        {
            get { return _target; }
        } 

        public string CurrentCartName
        {
            get {
                if (_target != null )
                {
                    if (string.IsNullOrEmpty(_target.CurrentCartName))
                    {
                        return "Load Game";
                    }
                    return _target.CurrentCartName;
                }
                return "Load Game"; 
            }
        }

        RunningStatuses runstate = RunningStatuses.Unloaded;

        public RunningStatuses RunState
        {
            get
            {
                return runstate;
            }
        }

        public void InsertCart(string fileName)
        {
            _target.GoTendo(fileName);


            this.CartInfo = new CartInfo()
            {
                CartName = _target.CurrentCartName,
                MapperID = _target.Cart.MapperID,
                Mirroring = _target.Cart.Mirroring,
                RomInfoString = string.Format("Prg Rom Count: {0}, Chr Rom Count: {1}", _target.Cart.NumberOfPrgRoms, _target.Cart.NumberOfChrRoms)
            };

            runstate = RunningStatuses.Off;
            NotifyPropertyChanged("CurrentCartName");
            NotifyPropertyChanged("CartInfo");
        }

        public void PowerOn()
        {
            _target.ThreadRuntendo();
            runstate = RunningStatuses.Running;
        }

        public void PowerOff()
        {
            _target.Paused = !_target.Paused;
        }

        public bool Paused
        {
            get { return _target.Paused; }
            set { _target.Paused = value; }
        }

        public void Reset()
        {
            _target.Reset();
        }

        public void RemoveCart()
        {
            _target.ThreadStoptendo();
            _target.EjectCart();
            runstate = RunningStatuses.Unloaded;
        }

        private SoundUI.SoundController soundControls;

        public SoundUI.SoundController SoundControls
        {
            get { 
                    return soundControls; 
            }
            set { soundControls = value; 
            }
        }





        public void Debug(bool value)
        {
            _target.IsDebugging = value;
        }

        #endregion


        //private SampleRates _sampleRate = SampleRates.high;

        //public SampleRates SampleRate
        //{
        //    get { return _sampleRate; }
        //    set 
        //    {
        //        if (_sampleRate != value)
        //        {
        //            _sampleRate = value;
        //            if (_target != null)
        //                _target.SoundBopper.SampleRate = (int)_sampleRate;
        //        }
                
        //    }
        //}

        #region IDisposable Members

        public void Dispose()
        {

            if (soundControls != null)
                soundControls.Dispose();
        }

        #endregion


        
    }
}
