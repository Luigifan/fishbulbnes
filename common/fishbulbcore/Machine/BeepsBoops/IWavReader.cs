using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.Machine.BeepsBoops
{
    public interface IWavReader
    {
        // void ReadWaves(ref byte[] destBuffer);
        void StartReadWaves();
        void ReadWaves();
        //byte[] ReadWaveBytes();
        byte[] SharedBuffer { get; set; }
        int SharedBufferLength { get; set; }
		float Frequency { get; }
		
        bool BufferAvailable { get; }

        event EventHandler BytesWritten;
    }
}
