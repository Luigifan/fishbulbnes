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
using NES.Sound;
using NES.CPU.Machine.BeepsBoops;
using NES.CPU.nitenedo;

namespace SilverlightBindings
{
    public class SilverlightWavStreamer : IWavStreamer
    {

        public SilverlightWavStreamer(IWavReader wavSource)
        {
            mediaSource = new NesMediaStreamSource();
            mediaSource.Reader = wavSource;
            _wavSource = wavSource;
            wavSource.SharedBuffer = new byte[4096];
            _wavSource.BytesWritten += new EventHandler(_wavSource_BytesWritten);
        }

        MediaElement mediaHost;

        public MediaElement MediaHost
        {
            get { return mediaHost; }
            set { mediaHost = value; }
        }

        void _wavSource_BytesWritten(object sender, EventArgs e)
        {
            
            mediaSource.WriteSamples();
            
            mediaSource.Wait();
            
        }

        IWavReader _wavSource;
        NesMediaStreamSource mediaSource;

        public NesMediaStreamSource MediaSource
        {
            get { return mediaSource; }
        }


        public bool IsRunning
        {
            set {  }
        }

        public bool Muted
        {
            get
            {
                return mediaHost.IsMuted;
            }
            set
            {
                mediaHost.IsMuted = value;
            }
        }

        public void PlayPCM()
        {
            
        }

        public float Volume
        {
            get
            {
                return (float)mediaHost.Volume;
            }
            set
            {
                mediaHost.Volume = value;
            }
        }

        public void CheckSamples()
        {
            
        }

        public void Dispose()
        {
            mediaSource.Dispose();
        }
    }
}
