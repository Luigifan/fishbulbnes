using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.nitenedo;
using System.Threading;
using System.ComponentModel;

namespace NES.Sound
{
    public class SoundThreader : IDisposable
    {
        IWavStreamer _wavePlayer;

        public IWavStreamer WavePlayer
        {
            get { return _wavePlayer; }
            set { _wavePlayer = value; }
        }

        Thread myThread;
        public SoundThreader(IWavStreamer streamer)
        {

            _wavePlayer = streamer;

            ThreadPool.QueueUserWorkItem(PlaySound, null);

        }

        public void OnSoundStatusChanged(object sender, NES.CPU.Machine.BeepsBoops.SoundStatusChangeEventArgs e)
        {
            _wavePlayer.Muted = e.Muted;
        }

        public void PlaySound(object o)
        {
            _wavePlayer.PlayPCM();
        }

        #region IDisposable Members

        public void Dispose()
        {
            _wavePlayer.Dispose();
        }

        #endregion
    }
}
