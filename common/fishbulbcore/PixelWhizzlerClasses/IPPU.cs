using System;
using NES.CPU.Machine;
namespace NES.CPU.PixelWhizzlerClasses
{
    public interface IPPU
    {
        bool BackgroundVisible { get; }
        void CheckVBlank();
        NES.CPU.Machine.Carts.INESCart ChrRomHandler { get; set; }
        void ClearVINT();
        void CopySprites(ref byte[] source, int copyFrom);
        byte[] CurrentFrame { get; }
        int CurrentPalette { get; }
        int CurrentXPosition { get; }
        int CurrentYPosition { get; }
        void DrawTo(int cpuClockNum);
        System.Collections.Generic.Queue<NES.CPU.PPUClasses.PPUWriteEvent> Events { get; }
        bool FillRGB { get; set; }
        NES.CPU.Fastendo.MachineEvent FrameFinishHandler { get; set; }
        bool FrameOn { get; set; }
        int Frames { get; }
        int GetByte(int Clock, int address);
        void HandleEvent(int Clock);
        bool HandleVBlankIRQ { get; set; }
        event EventHandler HBlank;
        int HScroll { get; }
        void Initialize();
        void InitSprites();
        bool IRQAsserted { get; set; }
        bool IsDebugging { get; set; }
        bool IsRendering { get; }
        int LastcpuClock { get; set; }
        int[] LoadPalABGR();
        void LoadPalRGBA();
        int MaxSpritesPerScanline { get; set; }
        int NameTableMemoryStart { get; set; }
        bool NeedToDraw { get; }
        int NextEventAt { get; }
        NES.CPU.Fastendo.MachineEvent NMIHandler { get; set; }
        bool NMIIsThrown { get; }
        int[] OutBuffer { get; }
        byte[][] PalCache { get; set; }
        byte[] Palette { get; set; }
        int PatternTableIndex { get; }
        int[] PixelEffectBuffer { get; set; }
        int PixelWidth { get; set; }
        int PPUAddress { get; set; }
        int PPUControlByte0 { get; set; }
        int PPUControlByte1 { get; set; }
        int PPUStatus { get; set; }
        void PreloadSprites(int scanline);
        void ReadState(System.Collections.Generic.Queue<int> state);
        void RenderScanline(int scanlineNum);
        void ResetClock(int Clock);
        int ScanlineNum { get; }
        int ScanlinePos { get; }
        void SetByte(int Clock, int address, int data);
        void SetupBufferForDisplay(ref int[] buffer);
        void SetupVINT();
        void SetVideoBuffer(int[] inBuffer);
        bool ShouldRender { get; set; }
        bool SpriteCopyHasHappened { get; set; }
        byte[] SpriteRam { get; }
        bool SpritesAreVisible { get; }
        int[] SpritesOnLine { get; }
        void UnpackSprites();
        int[] VideoBuffer { get; }
        int VidRAM_GetNTByte(int address);
        int VScroll { get; }
        void WriteState(System.Collections.Generic.Queue<int> writer);
        IPixelAwareDevice PixelAwareDevice { get; set; }
    }
}
