//* PixelWhizzler.Mirroring
// Joe Hartrick, 2009
//
// Controls the current ppu mirroring status, and provides the interface by which carts can modify it

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.PPUClasses
{
    public enum MirrorMasks
    {
        OneScreenMask = ~0xC00,
        HorizontalMask = ~0x400,
        VerticalMask = ~0x800,
        FourScreenMask = ~0x00
    }


    public partial class PixelWhizzler
    {
        int currentMirrorMask;

        public int CurrentMirrorMask
        {
            get { return currentMirrorMask; }
        }

        private int oneScreenMirrorOffset = 0x0;

        public int OneScreenMirrorOffset
        {
            get { return oneScreenMirrorOffset; }
            set { 
                oneScreenMirrorOffset = value; 
            }
        }
        private int _mirroring;
        public int Mirroring
        {
            get { return _mirroring; }
            set {
                    if (_mirroring != value)
                    {
                        UpdateMirroring(value);
                        _mirroring = value;
                    }
                }
        }

        private void UpdateMirroring(int val)
        {
            //    //            A11 A10 Effect
            //    //----------------------------------------------------------
            //    // 0   0  All four screen buffers are mapped to the same
            //    //        area of memory which repeats at $2000, $2400,
            //    //        $2800, and $2C00.
            //    // 0   x  "Upper" and "lower" screen buffers are mapped to
            //    //        separate areas of memory at $2000, $2400 and
            //    //        $2800, $2C00. ( horizontal mirroring)
            //    // x   0  "Left" and "right" screen buffers are mapped to
            //    //        separate areas of memory at $2000, $2800 and
            //    //        $2400,$2C00.  (vertical mirroring)
            //    // x   x  All four screen buffers are mapped to separate
            //    //        areas of memory. In this case, the cartridge
            //    //        must contain 2kB of additional VRAM (i got vram up the wazoo)
            //    // 0xC00 = 110000000000
            //    // 0x800 = 100000000000
            //    // 0x400 = 010000000000
            //    // 0x000 = 000000000000
            switch (val)
            {
                    // mask out 
                case 0:
                    currentMirrorMask = (int)MirrorMasks.OneScreenMask;
                    break;
                case 1:
                    oneScreenMirrorOffset = 0;
                    currentMirrorMask = (int)MirrorMasks.VerticalMask;
                    if (_mirroring == 2)
                    {
                        // copy bank from 0x800 to 0x400
                        Buffer.BlockCopy(_vidRAM, 0x2800, _vidRAM, 0x2400, 0x400);
                    }
                    break;
                case 2:
                    oneScreenMirrorOffset = 0;
                    currentMirrorMask = (int)MirrorMasks.HorizontalMask;
                    if (_mirroring == 1)
                    {
                        // copy bank from 0x800 to 0x400
                        Buffer.BlockCopy(_vidRAM, 0x2400, _vidRAM, 0x2800, 0x400);
                    }
                    break;
                case 3:
                    oneScreenMirrorOffset = 0;
                    currentMirrorMask = (int)MirrorMasks.FourScreenMask;
                    break;
            }
        }
    }
}
