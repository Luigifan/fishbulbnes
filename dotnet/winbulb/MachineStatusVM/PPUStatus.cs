using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Fastendo;
using NES.CPU.PPUClasses;

namespace WPFamicom.MachineStatusModel
{
    public class PPUStatusViewer
    {

        private PixelWhizzler _ppu;
        public PPUStatusViewer(PixelWhizzler ppu)
        {
            _ppu = ppu;
            
        }

        public bool IsRendering
        {
            get { return _ppu.IsRendering; }
        }
        public string PPUStatus
        {
            get { return string.Format("{0:x2}", _ppu.PPUStatus); }
        }

        public string PPUControl
        {
            get { return string.Format("{0:x2}" ,_ppu.PPUControlByte0 ); }
        }

        public int Scanline
        {
            get { return _ppu.ScanlineNum; }
        }


        public int ScanlinePos
        {
            get { return _ppu.ScanlinePos; }
        }

    }
}
