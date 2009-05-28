using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.nitenedo;
using System.Threading;
using System.ComponentModel;

namespace WPFamicom.Sound
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
            NESMachine _nes;
            public SoundThreader(NESMachine nes)
            {

                _nes = nes;
                _nes.SoundStatusChanged += new EventHandler<NES.CPU.Machine.BeepsBoops.SoundStatusChangeEventArgs>(_nes_SoundStatusChanged);
                _wavePlayer = new SDLInlineWavStreamer(_nes.WaveForms);

                ThreadPool.QueueUserWorkItem(PlaySound, null);

            }

            void _nes_SoundStatusChanged(object sender, NES.CPU.Machine.BeepsBoops.SoundStatusChangeEventArgs e)
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
