using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.XAudio2;
using SlimDX.Multimedia;
using System.Threading;
using NES.CPU.Machine.BeepsBoops;
using System.IO;

namespace InstiBulb.Sound
{
    public class InlineWavStreamer : IDisposable, InstiBulb.Sound.IWavStreamer
    {
        // each buffer should contain 1/60th of a second of audio
        //  lag sound behind up to _count frames
        const int BUFFER_STREAM_SIZE = 4096;
        const int BUFFER_COUNT = 8;
        int buffersInPlay = 0;
        byte[][] buffers = new byte[BUFFER_COUNT][];

        XAudio2 device;
        MasteringVoice masteringVoice;
        SourceVoice sourceVoice = null;
        AudioBuffer buffer;
        public InlineWavStreamer(IWavReader wavSource)
        {
            _wavSource = wavSource;
            _wavSource.BytesWritten += new EventHandler(_wavSource_BytesWritten);

            for (int i = 0; i < BUFFER_COUNT; ++i)
            {
                buffers[i] = new byte[BUFFER_STREAM_SIZE];
            }
            device = new XAudio2();
            buffer = new AudioBuffer();
            buffer.AudioData = new MemoryStream();
            buffer.Flags = BufferFlags.EndOfStream;

            masteringVoice = new MasteringVoice(device);

            
            CreateVoice();

        }

        private void CreateVoice()
        {
            WaveFormat format;

            // read in the wav file
            format = new WaveFormat();
            format.FormatTag = SlimDX.WaveFormatTag.Pcm;
            format.BitsPerSample = 16;
            format.Channels = 1;
            format.SamplesPerSecond = (int)_wavSource.Frequency;
            format.BlockAlignment = 2;
            format.AverageBytesPerSecond = 2 * (int)_wavSource.Frequency;

            try
            {
                sourceVoice = new SourceVoice(device, format);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        void _wavSource_BytesWritten(object sender, EventArgs e)
        {
            if (buffersInPlay < BUFFER_COUNT)
            {
                SendBuffer();
                
                buffersInPlay++;
                if (buffersInPlay > 3)
                {
                    BufferEmptyResetEvent.WaitOne();
                }
            }
        }

        IWavReader _wavSource;

        int currentBuffer = 0;

        private AutoResetEvent BufferEmptyResetEvent = new AutoResetEvent(false);
        private AutoResetEvent SamplesAvailableResetEvent = new AutoResetEvent(false);

        private bool _isRunning = true;

        public bool IsRunning
        {
            set { _isRunning = value; }
        }

        private bool muted;
        private float volume;

        public float Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                sourceVoice.Volume = volume;
                //if (volume < 0) volume = 0;
                //sourceVoice.Volume = volume;
                //device.CommitChanges();
                //CreateVoice();
                //sourceVoice.Volume = volume;
            }
        }

        public bool Muted
        {
            get { return muted; }
            set
            {

                muted = value;

                //if (muted)
                //{
                //    volume = sourceVoice.Volume;
                //    sourceVoice.Volume = 0;
                //}
                //else
                //{
                //    sourceVoice.Volume = volume;
                //}
            }
        }
        bool ended = false;
        public void PlayPCM()
        {
            //sourceVoice.SetOutputMatrix(1, 2, new float[] { 1.0f, 0.0f });

            sourceVoice.SubmitSourceBuffer(buffer);
            sourceVoice.Start(PlayFlags.None);
            sourceVoice.BufferEnd += new EventHandler<ContextEventArgs>(sourceVoice_BufferEnd);
            sourceVoice.VoiceError += new EventHandler<SlimDX.XAudio2.ErrorEventArgs>(sourceVoice_VoiceError);

        }

        private void SendBuffer()
        {
            buffer.AudioBytes = _wavSource.SharedBufferLength;
            buffer.PlayLength = _wavSource.SharedBufferLength / 2;
            buffer.AudioData
                    = new MemoryStream(_wavSource.SharedBuffer, 0, _wavSource.SharedBufferLength);

            sourceVoice.SubmitSourceBuffer(buffer);
            currentBuffer++;
            currentBuffer %= BUFFER_COUNT;
            _wavSource.SharedBuffer = buffers[currentBuffer];

            // tells the nes the waves have been read out
            _wavSource.ReadWaves();
        }

        void sourceVoice_VoiceError(object sender, SlimDX.XAudio2.ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        void sourceVoice_BufferEnd(object sender, ContextEventArgs e)
        {
            BufferEmptyResetEvent.Set();
            buffersInPlay--;
        }

        void sourceVoice_LoopEnd(object sender, ContextEventArgs e)
        {

        }


        #region IDisposable Members

        public void Dispose()
        {

            BufferEmptyResetEvent.Close();
            SamplesAvailableResetEvent.Close();
            buffer.Dispose();
            sourceVoice.Dispose();
            masteringVoice.Dispose();
            device.Dispose();
        }

        #endregion


        public void CheckSamples()
        {
        }
    }

}
