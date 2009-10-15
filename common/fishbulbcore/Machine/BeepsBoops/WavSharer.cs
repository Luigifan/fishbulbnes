using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NES.CPU.Machine.BeepsBoops
{
    public class WavSharer : IWavReader
    {

        // private Queue<byte> pendingWaves;
        private bool _NESTooFast;

        public bool NESTooFast
        {
            get { return _NESTooFast; }
            set { _NESTooFast = value; }
        }

        public object Locker = new object();
		
		public WavSharer(float frequency) : this()
		{
			this.frequency = frequency;
		}
		
        public WavSharer()
        {
            // should hold a frame worth
            //pendingWaves = new Queue<byte>(1500);

            _sharedBuffer = new byte[8192];
        }
		
		private float frequency = 22050;
		public float Frequency
		{	
			get { return frequency;}
			
		}

        #region Iwavwriter Members
        const int sample_size = 2;

        byte[] _sharedBuffer;

        public byte[] SharedBuffer
        {
            get { return _sharedBuffer; }
            set { _sharedBuffer = value; }
        }


        private int _sharedBufferLength;

        public int SharedBufferLength
        {
            get {
                return _sharedBufferLength;
            }
            set { _sharedBufferLength = value; }
        }

        private bool _bufferAvailable;

        public bool BufferAvailable
        {
            get { return _bufferAvailable; }
        }

        public void WavesWritten(int remain)
        {
            int n = _sharedBuffer.Length / sample_size;
            if (n > remain)
                n = remain;
            _sharedBufferLength = n * 2;

            if (fileWriting)
            {
                    appendToFile.WriteWaves(_sharedBuffer, _sharedBufferLength);
            }
            bufferWasRead = false;
            _bufferAvailable = true;
            WroteBytes();
        }

        private IWavWriter appendToFile;

        public void AppendFile(IWavWriter writer)
        {
            appendToFile = writer;
            fileWriting = (appendToFile != null);
        }

        private bool fileWriting;

        bool bufferWasRead;
        private AutoResetEvent bufferReadResetEvent = new AutoResetEvent(false);


        public void SyncUp()
        {
            // wait until buffer is empty

            while (!bufferWasRead)
            {
                bufferReadResetEvent.WaitOne();
            }

        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (null != appendToFile)
                appendToFile.Dispose();
        }

        #endregion

        public void StartReadWaves()
        {
        }

        public void ReadWaves()
        {

            _bufferAvailable = false;
            _sharedBufferLength = 0;
            bufferWasRead = true;
            bufferReadResetEvent.Set();
        }

        //public void ReadWaves(ref byte[] destBuffer)
        //{
        //    lock (Locker)
        //    {
        //        destBuffer = _sharedBuffer;
        //        bufferWasRead = true;
        //    }
        //}


        public event EventHandler BytesWritten;

        private void WroteBytes()
        {
            if (BytesWritten != null) BytesWritten(this, new EventArgs());
        }


        //public byte[] ReadWaveBytes()
        //{
        //    lock (Locker)
        //    {
        //        //byte[] destBuffer = new byte[pendingWaves.Count];
        //        //_NESTooFast = (destBuffer.Length > pendingWaves.Count);

        //        //for (int i = 0; i < destBuffer.Length; i++)
        //        //{
        //        //    destBuffer[i] = pendingWaves.Count > 0 ? pendingWaves.Dequeue() : (byte)0;
        //        //}
        //        bufferWasRead = true;
        //        return _sharedBuffer;                
        //    }
        //}

    }
}
