using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NES.CPU.Machine.BeepsBoops;
using Tao.Sdl;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WPFamicom.Sound
{
    public unsafe class SDLInlineWavStreamer : IDisposable, IWavStreamer
    {
        // each buffer should contain 1/60th of a second of audio
        //  lag sound behind up to _count frames
        const int BUFFER_STREAM_SIZE = 4096;
        const int BUFFER_COUNT = 8;
		class bufDef
		{
			public byte[] buffer;
			public int length;
		}

		Queue<bufDef> buffersToPlay = new Queue<bufDef>(BUFFER_COUNT);

		byte[][] buffers = new byte[BUFFER_COUNT][];
        int frequency;
		int currentBuffer=0;

        Sdl.AudioSpecCallbackDelegate audioCallback;

		bufDef playingBuf = null;
		int lastPlayPos = 0;
		
        void DoCallback(IntPtr userData, IntPtr stream, int length)
        {
			// Console.WriteLine("SDLInlineWavStreamer wants bytes: " + length.ToString());

            try
            {
                byte* ptr = (byte*)stream;

				while (length > 0)
                {
					length = PlayoutCurrentBuffer(ref ptr, length);
					if (length > 0)
					{
                        // Console.WriteLine("  " + length.ToString() + " bytes remaining");
                        GetNextBuffer();
					}
					
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with SDL audio callback " + Sdl.SDL_GetError());
                Console.WriteLine(e.ToString());
            }
        }

        private void GetNextBuffer()
        {
            if (buffersToPlay.Count > 0)
            {
                playingBuf = buffersToPlay.Dequeue();
                lastPlayPos = 0;
                BufferEmptyResetEvent.Set();
            }
            else
            {
                playingBuf = null;
                // Console.WriteLine("SDLInlineWavStreamer ran out of data (NES too slow)");
                SamplesAvailableResetEvent.WaitOne();
            }
        }

		int PlayoutCurrentBuffer(ref byte* ptr, int length)
		{
			if (playingBuf != null)
			{
				while  (lastPlayPos < playingBuf.length)
				{
					length--;
                    *(ptr++) = playingBuf.buffer[lastPlayPos++];
                    if (length <= 0)
					{
						return 0;
					}
				}
				playingBuf = null;
				lastPlayPos=0;
			}
			return length;

		}
		
        public SDLInlineWavStreamer(IWavReader wavSource)
        {
			//frequency = (int)_wavSource.Frequency;
            _wavSource = wavSource;
			_wavSource.BytesWritten += _wavSource_BytesWritten;
			Console.WriteLine(String.Format("SDLInlineWavStreamer() {0}", wavSource.Frequency));
			
            for (int i = 0; i < BUFFER_COUNT; ++i)
            {
                buffers[i] = new byte[BUFFER_STREAM_SIZE];
            }

            Sdl.SDL_Init(Sdl.SDL_INIT_AUDIO);
            audioCallback = DoCallback;
			
            Sdl.SDL_AudioSpec spec = new Sdl.SDL_AudioSpec()
            {
                freq = (int)_wavSource.Frequency,
                format = (short)Sdl.AUDIO_S16SYS,
                channels = 1,
                samples = 2048,
                callback = Marshal.GetFunctionPointerForDelegate(audioCallback),
            };

            IntPtr specPtr= Marshal.AllocHGlobal(Marshal.SizeOf(spec));
            try
            {
                Marshal.StructureToPtr(spec, specPtr, false);

                IntPtr obtained = IntPtr.Zero;
                if (Sdl.SDL_OpenAudio((IntPtr)specPtr, obtained) < 0)
                {
                    Console.WriteLine("Error opening sdl_audio");
                    Console.WriteLine(Sdl.SDL_GetError());
                }
            }
            finally
            {
                Marshal.FreeHGlobal(specPtr);
            }

        }

        void _wavSource_BytesWritten(object sender, EventArgs e)
        {
			while(buffersToPlay.Count > 5)
			{
				BufferEmptyResetEvent.WaitOne();
			}
			
			buffersToPlay.Enqueue(
			   new bufDef() 
			    { 
					buffer = _wavSource.SharedBuffer, 
                    length = _wavSource.SharedBufferLength 
				}
			);

			SamplesAvailableResetEvent.Set();

			currentBuffer++;
			currentBuffer %= BUFFER_COUNT;
			_wavSource.SharedBuffer = buffers[currentBuffer];
			_wavSource.ReadWaves();
        }

        IWavReader _wavSource;

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

		public void CheckSamples()
        {
            BufferEmptyResetEvent.Set();
        }

        #region IDisposable Members

        public void Dispose()
        {

            BufferEmptyResetEvent.Close();
			SamplesAvailableResetEvent.Set();
            SamplesAvailableResetEvent.Close();

        }


        #endregion

        #region IWavStreamer implementation
        public void PlayPCM ()
        {
            Sdl.SDL_PauseAudio(0);
			SamplesAvailableResetEvent.WaitOne();
        }
        #endregion
    }

}
