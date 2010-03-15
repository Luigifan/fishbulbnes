using System;
using System.Net;

using NES.Sound;
using NES.CPU.Machine.BeepsBoops;
using System.Threading;


namespace WPFamicom.Sound
{
    public class DummyWavStreamer : IWavStreamer
    {

        AutoResetEvent resetEvent = new AutoResetEvent(false);

        Timer t;

        public DummyWavStreamer(IWavReader wavSource)
        {
            wavSource.BytesWritten += new EventHandler(_wavSource_BytesWritten);
            t = new Timer(new TimerCallback(FrameTick), null, 0, 15 );
            
        }

        void FrameTick(object o)
        {
            resetEvent.Set();
        }

        void _wavSource_BytesWritten(object sender, EventArgs e)
        {
            resetEvent.WaitOne();
        }


        public bool IsRunning
        {
            set {  }
        }

        public bool Muted
        {
            get
            {
                return false;
            }
            set
            {
                
            }
        }

        public void PlayPCM()
        {
            
        }

        public float Volume
        {
            get
            {
                return 0;
            }
            set
            {
                
            }
        }

        public void CheckSamples()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}
