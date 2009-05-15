using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.Fastendo
{

    public delegate void MachineEvent();
    
    public interface IMemoryMappedIOElement
    {
        int GetByte(int address);
        void SetByte(int address, int data);
    }

    public interface IClockedMemoryMappedIOElement
    {
        int GetByte(int Clock, int address);
        void SetByte(int Clock, int address, int data);
        MachineEvent NMIHandler { get; set; }
        
        bool IRQAsserted { get; set; }
        int NextEventAt { get; }
        void HandleEvent(int Clock);
        void ResetClock(int Clock);
    }

    public interface IPPU : IClockedMemoryMappedIOElement
    {
        // needed for dma transfers
        bool SpriteCopyHasHappened { get; set; }
        void CopySprites(ref byte[] ram, int from);
        MachineEvent FrameFinishHandler { get; set; }
    }

    
}
