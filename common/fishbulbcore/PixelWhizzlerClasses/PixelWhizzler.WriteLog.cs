using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.PPUClasses
{
    public class PPUWriteEvent
    {
        // note: there's some redundancy here for the purpose of sanity checks
        private int scanlineNum;

        public int ScanlineNum
        {
            get { return scanlineNum; }
            set { scanlineNum = value; }
        }
        private int scanlinePos;

        public int ScanlinePos
        {
            get { return scanlinePos; }
            set { scanlinePos = value; }
        }
        private int frameClock;

        public int FrameClock
        {
            get { return frameClock; }
            set { frameClock = value; }
        }

        // 
        private int registerAffected;

        public int RegisterAffected
        {
            get { return registerAffected; }
            set { registerAffected = value; }
        }
        private int dataWritten;

        public int DataWritten
        {
            get { return dataWritten; }
            set { dataWritten = value; }
        }
        public override string ToString()
        {
            return string.Format(" {0:x2} written to {1:x4} at {2}, {3}", registerAffected, dataWritten, scanlineNum, scanlinePos);
        }
    }

    public partial class PixelWhizzler
    {


        public Queue<PPUWriteEvent> Events = new Queue<PPUWriteEvent>();
        /// <summary>
        /// A class to encapsulate a PPU "event" for debugging/tracing purposes
        /// </summary>

    }
}
