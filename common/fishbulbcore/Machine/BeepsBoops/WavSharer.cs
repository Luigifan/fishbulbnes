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

            //if (fileWriting)
            //{
            //        appendToFile.WriteWaves(_sharedBuffer, _sharedBufferLength);
            //}
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
           // bufferReadResetEvent.Set();
        }

        private EventHandler bytesWritten;

        public EventHandler BytesWritten
        {
            get { return bytesWritten; }
            set { bytesWritten = value; }
        }

        private void WroteBytes()
        {
            if (bytesWritten != null) bytesWritten(this, new EventArgs());
        }


        public void SetSharedBuffer(byte[] values)
        {
            _sharedBuffer = values;
        }
    }
}
