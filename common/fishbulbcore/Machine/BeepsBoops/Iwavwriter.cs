using System;
namespace NES.CPU.Machine.BeepsBoops
{
    public interface IWavWriter : IDisposable
    {
        void WriteWaves(byte[] inBuff, int remain);
    }
}
