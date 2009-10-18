using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NES.CPU.Machine.BeepsBoops;
using Tao.OpenAl;
using System.Diagnostics;
using NES.Sound;

namespace InstiBulb.Sound
{
    public class OpenALInlineWavStreamer : IDisposable, IWavStreamer
    {
        // each buffer should contain 1/60th of a second of audio
        //  lag sound behind up to _count frames
        const int BUFFER_STREAM_SIZE = 4096;
        const int BUFFER_COUNT = 8;
        const int BUFFER_LAG = 3;

        int[] bufferIds = new int[BUFFER_COUNT];
        byte[][] buffers = new byte[BUFFER_COUNT][];
        Queue<int> freeBuffers = new Queue<int>(BUFFER_COUNT);
        Dictionary<int, int> bufferIDtoArrayIndex = new Dictionary<int, int>(BUFFER_COUNT);
        int frequency;

        int sourceId = 0, bufferId = 0;
        private float[] listenerOrientation = { 0, 0, -1, 0, 1, 0 };
        private float[] listenerPosition = { 0, 0, 0 };                // Position of the Listener.
        private float[] listenerVelocity = { 0, 0, 0 };

        public OpenALInlineWavStreamer(IWavReader wavSource)
        {
            frequency = (int)wavSource.Frequency;
            _wavSource = wavSource;
            _wavSource.BytesWritten += new EventHandler(_wavSource_BytesWritten);

            int err = 0;

            Alut.alutInit();

            err = Al.alGetError();
            Debug.Assert(err == 0, "Error " + err.ToString());

            // device = new XAudio2();

            Al.alGenSources(1, out sourceId);
            Al.alGenBuffers(BUFFER_COUNT, bufferIds);
            err = Al.alGetError();
            Debug.Assert(err == 0, "Error " + err.ToString());
            for (int i = 0; i < bufferIds.Length; ++i)
            {
                if (bufferIds[i] !=0)
                    bufferIDtoArrayIndex.Add(bufferIds[i], i);
            }



            Al.alListenerfv(Al.AL_POSITION, listenerPosition);
            Al.alListenerfv(Al.AL_VELOCITY, listenerVelocity);
            Al.alListenerfv(Al.AL_ORIENTATION, listenerOrientation);

            err = Al.alGetError();
            Debug.Assert(err == 0, "Error " + err.ToString());
            


            // Set the pitch
            Al.alSourcef(sourceId, Al.AL_PITCH, 1.0f);
            // Set the gain
            Al.alSourcef(sourceId, Al.AL_GAIN, 1.0f);
            // Set looping to loop
            Al.alSourcei(sourceId, Al.AL_LOOPING, 0);
            err = Al.alGetError();
            if (err != 0)
            {
                Debug.Assert(false, "Error " + err.ToString());
            }


            for (int i = 0; i < BUFFER_COUNT; ++i)
            {
                buffers[i] = new byte[BUFFER_STREAM_SIZE];
                Al.alBufferData(bufferIds[i], Al.AL_FORMAT_MONO16, buffers[i], buffers[i].Length, frequency);
                //                Al.alBufferData(bufferIds[1], Al.AL_FORMAT_MONO16, buffers[0], buffers[0].Length, (int)44100);
            }


            Al.alSourceQueueBuffers(sourceId, BUFFER_COUNT, bufferIds);
            Al.alSourcePlay(sourceId);
            err = Al.alGetError();
            if (err != 0)
            {
                Debug.Assert(false, "Error " + err.ToString());
            }

        }

        void _wavSource_BytesWritten(object sender, EventArgs e)
        {
			//return;
			
            if (freeBuffers.Count > 0)
            {
                SendBuffer();
            }

            while (freeBuffers.Count < BUFFER_LAG)
            {
                DequeueBuffers();
            }

            if (!IsPlaying)
            {
                Al.alSourcePlay(sourceId);
            }

        }

        IWavReader _wavSource;

        int currentBuffer = 0;


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
                Al.alSourcef(sourceId, Al.AL_GAIN, volume);

            }
        }

        public bool Muted
        {
            get { return muted; }
            set
            {

                muted = value;

            }
        }
        bool ended = false;


        bool IsPlaying
        {
            get
            {
                int state;

                Al.alGetSourcei(sourceId, Al.AL_SOURCE_STATE, out state);

                return (state == Al.AL_PLAYING);
            }
        }

        public void PlayPCM()
        {
            // nothing to do here!
        }

        TimeSpan timeSpanFromMilliseconds = TimeSpan.FromMilliseconds(1000f / 70f);


        private void DequeueBuffers()
        {
            int processed = 0;
            Al.alGetSourcei(sourceId, Al.AL_BUFFERS_PROCESSED, out processed);

            while (processed <= 0)
            {
                processed = 0;

                int err = Al.alGetError();
                Thread.Sleep(timeSpanFromMilliseconds);
                Al.alGetSourcei(sourceId, Al.AL_BUFFERS_PROCESSED, out processed);
            }

            while (processed-- > 0)
            {
                int buffer = 0;
                Al.alSourceUnqueueBuffers(sourceId, 1, ref buffer);
                freeBuffers.Enqueue(buffer);
            }
        }

        private unsafe void SendBuffer()
        {
            int buffer = freeBuffers.Dequeue();
			fixed (byte* p = _wavSource.SharedBuffer)
	            Al.alBufferData(buffer, Al.AL_FORMAT_MONO16, p, _wavSource.SharedBufferLength, frequency);
            Al.alSourceQueueBuffers(sourceId, 1, ref buffer);
            currentBuffer++;
            currentBuffer %= BUFFER_COUNT;

            _wavSource.SharedBuffer = buffers[currentBuffer];
            _wavSource.ReadWaves();
        }
		
		public void CheckSamples()
		{}

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }

}
