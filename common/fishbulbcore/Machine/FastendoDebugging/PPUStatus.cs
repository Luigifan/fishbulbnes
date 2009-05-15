using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.PPUClasses;

namespace NES.CPU.FastendoDebugging
{
    public class DebuggerPPUState
    {

        public DebuggerPPUState()
        {
        }

        private List<PPUWriteEvent> frameWriteEvents;

        public List<PPUWriteEvent> FrameWriteEvents
        {
            get { return frameWriteEvents; }
            set { frameWriteEvents = value; }
        }

        bool _isRendering;

        public bool IsRendering
        {
            get { return _isRendering; }
            set { _isRendering = value; }
        }

        int _PPUStatus;

        public int PPUStatus
        {
            get { return _PPUStatus; }
            set { _PPUStatus = value; }
        }

        int _PPUControl;

        public int PPUControl
        {
            get { return _PPUControl; }
            set { _PPUControl = value; }
        }
        int _scanLine;

        public int ScanLine
        {
            get { return _scanLine; }
            set { _scanLine = value; }
        }

        private int scanlinePos;

        public int ScanlinePos
        {
            get { return scanlinePos; }
            set { scanlinePos = value; }
        }
        
    }
}
