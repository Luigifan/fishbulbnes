using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Fastendo;
using NES.CPU.Machine.PortQueueing;

namespace NES.CPU.Machine.BeepsBoops
{
    public interface IAPU
    {
        void UpdateFrame(int time);
        void EndFrame(int time);
        bool InterruptRaised { get; set; }
        bool Muted { get; set; }
        bool EnableSquare0 { get; set; }
        bool EnableSquare1 { get; set; }
        bool EnableTriangle { get; set; }
        bool EnableNoise { get; set; }
    }

    public class Bopper : IClockedMemoryMappedIOElement, IAPU
    {

        private SquareChannel square0, square1; // = new SquareChannel();
        private TriangleChannel triangle;
        private NoiseChannel noise;
        private WavSharer writer;
        private DMCChannel dmc;
        
        private Blip myBlipper;
        
        private bool _throwingIRQs;


        private QueuedPort registers = new QueuedPort();

        const int master_vol = 65536 / 15;
        // 44.1 kHz sample rate
        int _sampleRate = 44100;
        // 1.78 MHz clock rate 
        const double clock_rate = 1789772.727; // 1.78 MHz clock rate 
        
        // rough approximation of how its mixed 
        // output = squares + ntd    (noise, triangle, dmc)
        
        int square0Gain = master_vol * 20 / 100;
        int square1Gain = master_vol * 20 / 100;
        int triangleGain = master_vol * 23 / 100;
        int noiseGain = master_vol * 13 / 100;

        public int SampleRate
        {
            get { return _sampleRate; }
            set { 
                _sampleRate = value;
                RebuildSound();
            }
        }
        
        public Bopper(WavSharer output)
        {

            writer = output;
			_sampleRate = (int) output.Frequency;
            RebuildSound();
        }

        public void RebuildSound()
        {
            myBlipper = new Blip(_sampleRate / 5);
            myBlipper.blip_set_rates(clock_rate, _sampleRate);


            registers.Clear();
            _interruptRaised = false;
            square0Gain = master_vol * 20 / 100;
            square1Gain = master_vol * 20 / 100;
            triangleGain = master_vol * 23 / 100;
            noiseGain = master_vol * 13 / 100;

            square0 = new SquareChannel(myBlipper, 0) { Gain = square0Gain, Period = 10, SweepComplement = true };
            square1 = new SquareChannel(myBlipper, 1) { Gain = square1Gain, Period = 10, SweepComplement = false };
            triangle = new TriangleChannel(myBlipper, 2) { Gain = triangleGain, Period = 0 };
            noise = new NoiseChannel(myBlipper, 3) { Gain = noiseGain, Period = 0 };
            dmc = new DMCChannel(myBlipper, 4) { Gain = master_vol * 20 / 100, Period = 10 };
        }

        #region IClockedMemoryMappedIOElement Members

        public int GetByte(int Clock, int address)
        {
            if (address == 0x4000) _interruptRaised= false;
            if (address == 0x4015) return ReadStatus();
            else return 0x42;
        }

        private int ReadStatus()
        {
            return ((square0.Length> 0) ? 0x01 : 0) |
            ((square1.Length > 0) ? 0x02 : 0) |
            ((triangle.Length > 0) ? 0x04 : 0) |
            ((square0.Length > 0) ? 0x08 : 0) |
            (_interruptRaised ? 0x40 : 0) ;
        }

        int reg15;

        public void SetByte(int Clock, int address, int data)
        {
            if (address == 0x4000) _interruptRaised = false;
            //DoSetByte( Clock,  address,  data);
            registers.Enqueue(new PortWriteEntry(Clock, (ushort)address, (byte)data));


        }

        private void DoSetByte(int Clock, int address, int data)
        {
            switch (address)
            {
                case 0x4000:
                case 0x4001:
                case 0x4002:
                case 0x4003:
                    square0.WriteRegister(address - 0x4000, data, Clock);
                    break;
                case 0x4004:
                case 0x4005:
                case 0x4006:
                case 0x4007:
                    square1.WriteRegister(address - 0x4004, data, Clock);
                    break;
                case 0x4008:
                case 0x4009:
                case 0x400A:
                case 0x400B:
                    triangle.WriteRegister(address - 0x4008, data, Clock);
                    break;
                case 0x400C:
                case 0x400D:
                case 0x400E:
                case 0x400F:
                    noise.WriteRegister(address - 0x400C, data, Clock);
                    break;
                case 0x4010:
                case 0x4011:
                case 0x4012:
                case 0x4013:
                    // dmc.WriteRegister(address - 0x40010, data, Clock);
                    break;
                case 0x4015:
                    reg15 = data;

                    square0.WriteRegister(4, data & 0x01, Clock);
                    square1.WriteRegister(4, data & 0x02, Clock);
                    triangle.WriteRegister(4, data & 0x04, Clock);
                    noise.WriteRegister(4, data & 0x08, Clock);
                    break;

                case 0x4017:
                    _throwingIRQs = ((data & 0x40) != 0x40);
                    lastFrameHit = 0;
                    break;
            }
        }

        #endregion

        #region IAPU Members

        // runs all wavs to current time and flushes the buffer

        bool muted = false;

        public bool Muted
        {
            get { return muted; }
            set { muted = value; }
        }

        int lastFrameHit = 0;

        public void UpdateFrame(int time)
        {
            if (muted) return;

            RunFrameEvents(time, lastFrameHit);
            if (lastFrameHit == 3)
            {

                if (_throwingIRQs)
                {
                    _interruptRaised = true;
                }
                lastFrameHit = 0;
                //EndFrame(time);
            }
            else
            {
                lastFrameHit++;
            }


        }

        private void RunFrameEvents(int time, int step)
        {
            triangle.FrameClock(time, step);
            noise.FrameClock(time, step);
            square0.FrameClock(time, step);
            square1.FrameClock(time, step);
            // dmc.FrameClock(time, step);
        }

        short[] _writeBuffer = new short[1024];

        public short[] WriteBuffer
        {
            get { return _writeBuffer; }
            set { _writeBuffer = value; }
        }

        public void EndFrame(int time)
        {

            square0.EndFrame(time);
            square1.EndFrame(time);
            triangle.EndFrame(time);
            noise.EndFrame(time);

            if (!muted)
                myBlipper.blip_end_frame(time);

            lock (writer.Locker)
            {
                int count = myBlipper.ReadBytes(writer.SharedBuffer, writer.SharedBuffer.Length / 2, 0);
                writer.WavesWritten(count);
            }
        }

        public void FlushFrame(int time)
        {
            int currentClock = 0;
            int frameClocker = 0;
            PortWriteEntry currentEntry;
            while (registers.Count > 0) 
            {
                currentEntry = registers.Dequeue();
                if (frameClocker > 7445)
                {
                    frameClocker -= 7445;
                    UpdateFrame(7445);
                }
                DoSetByte(currentEntry.time, currentEntry.address, currentEntry.data);
                currentClock = currentEntry.time;
                frameClocker = currentEntry.time;
            }

            // hit the latest frame boundary, maybe too much math for too little reward
            int clockDelta = currentClock % 7445;

            if (lastFrameHit == 0) UpdateFrame(7445);
            while (lastFrameHit > 0)
            {
                UpdateFrame(7445 * (lastFrameHit + 1));
            }
        }

        private bool _interruptRaised;

        public bool InterruptRaised
        {
            get
            {
                return _interruptRaised;
            }
            set
            {
                _interruptRaised = value;
            }
        }

        #endregion

        public bool EnableSquare0
        {
            get
            {
                return (square0.Gain != 0);
            }
            set
            {
                square0.Gain = (value) ? square0Gain : 0;
            }
        }


        public bool EnableSquare1
        {
            get
            {
                return (square1.Gain != 0);
            }
            set
            {
                square1.Gain = (value) ? square1Gain: 0;
            }
        }

        public bool EnableTriangle
        {
            get
            {
                return (triangle.Gain != 0);
            }
            set
            {
                triangle.Gain = (value) ? triangleGain : 0;
            }
        }

        public bool EnableNoise
        {
            get
            {
                return (noise.Gain != 0);
            }
            set
            {
                noise.Gain = (value) ? noiseGain : 0;
            }
        }


        #region IClockedMemoryMappedIOElement Members


        public MachineEvent NMIHandler
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public bool IRQAsserted
        {
            get
            {
                return false;
            }
            set
            {
                // throw new NotImplementedException();
            }
        }

        public int NextEventAt
        {
            get { return 7445 * (lastFrameHit + 1) - lastClock; }
        }

        int lastClock;

        public void HandleEvent(int Clock)
        {
            UpdateFrame(Clock);
            lastClock = Clock;

            if (Clock > 29780)
            {
                lock (writer)
                {
                    EndFrame(Clock);
                }
            }
        }

        public void ResetClock(int Clock)
        {
            lastClock = Clock;
        }

        #endregion
    }
}
