using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace NES.CPU.PPUClasses
{
    public class PPUWriteEvent : INotifyPropertyChanged
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

        public string Text
        {
            get
            {
                return this.ToString();
            }
        }

        public override string ToString()
        {
            return string.Format(" {0:x2} written to {1:x4} at {2}, {3}", registerAffected, dataWritten, scanlineNum, scanlinePos);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public partial class PixelWhizzler
    {


        private Queue<PPUWriteEvent> events = new Queue<PPUWriteEvent>();

        public Queue<PPUWriteEvent> Events
        {
            get { return events; }
        }
        /// <summary>
        /// A class to encapsulate a PPU "event" for debugging/tracing purposes
        /// </summary>

    }
}
