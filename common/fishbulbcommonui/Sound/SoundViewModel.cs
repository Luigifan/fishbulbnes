﻿using System;
using Fishbulb.Common.UI;
using NES.Machine;
using NES.CPU.nitenedo;
using NES.CPU.Machine.BeepsBoops;
using NES.Sound;
using System.Collections.Generic;
using FishBulb;

namespace fishbulbcommonui
{


    public class SoundViewModel : BaseNESViewModel
    {

        IWavStreamer streamer;
        Bopper SoundBopper;

        public SoundViewModel(IPlatformDelegates delegates)
            : base(delegates) 
        {
            Commands.Add("MuteToggle", new InstigatorCommand(
                o => ToggleMute(null),
                o => true));
        }


        protected override void OnAttachTarget()
        {
            SoundBopper = TargetMachine.SoundBopper;
            NotifyPropertyChanged("Volume");
        }

        public IWavStreamer Streamer
        {
            get { return streamer; }
            set { streamer = value;
                
            }
        }


        void ToggleMute(object o)
        {
            Muted = !Muted;
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
