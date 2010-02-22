using System;
using Fishbulb.Common.UI;
using NES.Machine;
using NES.CPU.nitenedo;
using NES.CPU.Machine.BeepsBoops;
using NES.Sound;
using System.Collections.Generic;

namespace fishbulbcommonui
{


    public class SoundViewModel : BaseNESViewModel
    {

        IWavStreamer streamer;
        Bopper SoundBopper;

        protected override void OnAttachTarget()
        {
            SoundBopper = TargetMachine.SoundBopper;
        }

        public IWavStreamer Streamer
        {
            get { return streamer; }
            set { streamer = value; }
        }


        string currentView = "SoundView";
        public override string CurrentView
        {
            get { return currentView; }
        }

        public override string CurrentRegion
        {
            get { return "controlPanel.0"; }
        }

        public override string Header
        {
            get { return "Sound Controls"; }
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
                NotifyPropertyChanged("Volume");
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
            get { return SoundBopper.EnableSquare0; }
            set
            {
                SoundBopper.EnableSquare0 = value;
            }
        }

        public bool EnableSquareChannel1
        {
            get { return SoundBopper.EnableSquare1; }
            set { SoundBopper.EnableSquare1 = value; }
        }

        public bool EnableTriangleChannel
        {
            get { return SoundBopper.EnableTriangle; }
            set { SoundBopper.EnableTriangle = value; }
        }

        public bool EnableNoiseChannel
        {
            get { return SoundBopper.EnableNoise; }
            set { SoundBopper.EnableNoise = value; }
        }

    }
}
