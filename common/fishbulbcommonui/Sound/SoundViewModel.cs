using System;
using Fishbulb.Common.UI;
using NES.Machine;
using NES.CPU.nitenedo;
using NES.CPU.Machine.BeepsBoops;
using GtkNes;
using NES.Sound;
using System.Collections.Generic;

namespace GtkNes
{


    public class SoundViewModel : IViewModel
    {
        NESMachine nes;
        IWavStreamer streamer;
        public SoundViewModel(NESMachine nes, IWavStreamer streamer)
        {
            this.nes = nes;
            this.streamer = streamer;

        }



        string currentView = "SoundView";
        public string CurrentView
        {
            get { return currentView; }
        }

        public Dictionary<string, ICommandWrapper> Commands
        {
            get { return new System.Collections.Generic.Dictionary<string, ICommandWrapper>(); }
        }

        public IEnumerable<IViewModel> ChildViewModels
        {
            get { return new List<IViewModel>(); }
        }

        public string CurrentRegion
        {
            get { return "controlPanel.0"; }
        }

        public string Header
        {
            get { return "Sound Controls"; }
        }

        public object DataModel
        {
            get { return null; }
        }

        private int volume;
        /// <summary>
        /// Volume is from 0 to 100
        /// </summary>
        public float Volume
        {
            get { return streamer.Volume ; }
            set
            {
                streamer.Volume = value ;
            }
        }

        public bool Muted
        {
            get
            {
                return streamer.Muted;
            }
            set
            {
                if (streamer.Muted != value)
                {
                    streamer.Muted = value;
                    NotifyPropertyChanged("Muted");
                }
            }
        }

        public bool EnableSquareChannel0
        {
            get { return nes.SoundBopper.EnableSquare0; }
            set
            {
                nes.SoundBopper.EnableSquare0 = value;
            }
        }

        public bool EnableSquareChannel1
        {
            get { return nes.SoundBopper.EnableSquare1; }
            set { nes.SoundBopper.EnableSquare1 = value; }
        }

        public bool EnableTriangleChannel
        {
            get { return nes.SoundBopper.EnableTriangle; }
            set { nes.SoundBopper.EnableTriangle = value; }
        }

        public bool EnableNoiseChannel
        {
            get { return nes.SoundBopper.EnableNoise; }
            set { nes.SoundBopper.EnableNoise = value; }
        }

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(PropertyName));
        }

        #endregion
    }
}
