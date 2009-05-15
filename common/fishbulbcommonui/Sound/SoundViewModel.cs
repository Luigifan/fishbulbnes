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
        System.Timers.Timer timer = new System.Timers.Timer();
        public SoundViewModel(NESMachine nes, IWavStreamer streamer)
        {
            this.nes = nes;
            this.streamer = streamer;
            timer.Interval = 100f;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        int ticks = 0;

        public string Ticks
        {
            get { return ticks.ToString(); }
            set { ticks = int.Parse(value); }
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ticks++;
            NotifyPropertyChanged("Ticks");
        }

        #region IProfileViewModel Members

        string currentView = "SoundView";
        public string CurrentView
        {
            get { return currentView; }
        }

        public Dictionary<string, ICommandWrapper> Commands
        {
            get { return new System.Collections.Generic.Dictionary<string, ICommandWrapper>(); }
        }

        public IEnumerable<IProfileViewModel> ChildViewModels
        {
            get { return new List<IProfileViewModel>(); }
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

        public void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(PropertyName));
        }

        #endregion
    }
}
