using System;
namespace WPFamicom.Sound
{
    public interface IWavStreamer : IDisposable
    {
        bool IsRunning { set; }
        bool Muted { get; set; }
        void PlayPCM();
        float Volume { get; set; }
        void CheckSamples();
    }
}
