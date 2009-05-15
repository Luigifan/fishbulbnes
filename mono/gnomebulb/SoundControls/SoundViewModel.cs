using System;
using Fishbulb.Common.UI;
using NES.Machine;
using NES.CPU.nitenedo;
using NES.CPU.Machine.BeepsBoops;
using GtkNes;
using WPFamicom.Sound;
using System.Collections.Generic;

namespace GtkNes
{


    public class SoundViewModel : IProfileViewModel
    {
        NESMachine nes;
        IWavStreamer streamer;
        public SoundViewModel(NESMachine nes, IWavStreamer streamer)
        {
            this.nes = nes;
            this.streamer = streamer;

        }

        #region IProfileViewModel Members

        string currentView;
        public string CurrentView
        {
            get { return currentView; }
        }

        public Dictionary<string, ICommandWrapper> Commands
        {
            get { return new System.Collections.Generic.Dictionary<string,ICommandWrapper>(); }
        }

        public IEnumerable<IProfileViewModel> ChildViewModels
        {
            get { return new List<IProfileViewModel>(); }
        }

        public string CurrentRegion
        {
            get { return "soundControlRegion"; }
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

        public float Volume
        {
            get { return streamer.Volume; }
            set
            {
                streamer.Volume = value;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
