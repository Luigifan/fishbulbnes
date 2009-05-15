using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Machine.BeepsBoops;
using Yeti.MMedia.Mp3;
using System.IO;

namespace WPFamicom.ControlPanelMVVM
{
    public class Mp3FileSaver : IWavWriter
    {
        Mp3Writer mp3Out;

        public Mp3FileSaver(string fileName, int sampleRate, int bitsPerSample, int channels)
        {
            WaveLib.WaveFormat newWaveFormat = new WaveLib.WaveFormat(sampleRate, bitsPerSample, channels)
            {
                wFormatTag = (int)SlimDX.WaveFormatTag.Pcm
            };

            mp3Out = new Mp3Writer(new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write), newWaveFormat);
        }

        #region IWavWriter Members

        public void WriteWaves(byte[] inBuff, int remain)
        {
            mp3Out.Write(inBuff, 0, remain);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            mp3Out.Close();
        }

        #endregion
    }
}
