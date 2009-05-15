using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.Machine.BeepsBoops
{
    public class TriangleChannel
    {

        Blip _bleeper;
        int _chan;

        public TriangleChannel(Blip bleeper, int chan)
        {
            _bleeper = bleeper;
            _chan = chan;
        }

        int[] LengthCounts = new int[] 
        {
	        0x0A,0xFE,
	        0x14,0x02,
	        0x28,0x04,
	        0x50,0x06,
	        0xA0,0x08,
	        0x3C,0x0A,
	        0x0E,0x0C,
	        0x1A,0x0E,

	        0x0C,0x10,
	        0x18,0x12,
	        0x30,0x14,
	        0x60,0x16,
	        0xC0,0x18,
	        0x48,0x1A,
	        0x10,0x1C,
	        0x20,0x1E
        };


        private int _length;

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        private int _period;
        /// <summary>
        /// Period of current waveform
        /// </summary>
        public int Period
        {
            get { return _period; }
            set { _period = value; }
        }

        private int _time;

        /// <summary>
        ///  current time in channel
        /// </summary>
        public int Time
        {
            get { return _time; }
            set { _time = value; }
        }

        private int _envelope;

        public int Envelope
        {
            get { return _envelope; }
            set { _envelope = value; }
        }

        private bool _looping;

        public bool Looping
        {
            get { return _looping; }
            set { _looping = value; }
        }

        private bool _enabled = true;

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        private int _amplitude;

        public int Amplitude
        {
            get { return _amplitude; }
            set { _amplitude = value; }
        }

        private int _gain;
        public int Gain
        {
            get { return _gain; }
            set { _gain = value; }
        }

        private int _linCtr, _phase;
        private int _linVal;

        private bool _linStart;

        public void WriteRegister(int register, int data, int time)
        {
            //Run(time);

            switch (register)
            {
                case 0:
                    _looping = (data & 0x80) == 0x80;
                    _linVal = data & 0x7F;
                    break;
                // sweep
                case 1:
                    break;
                // period low
                case 2:
                    _period &= 0x700;
                    _period |= data;
                    break;
                case 3:
                    _period &= 0xFF;
                    _period |= (data & 0x7) << 8;
                    // setup lengthhave
                    if (_enabled)
                    {
                        _length = LengthCounts[(data >> 3) & 0x1f];
                    }
                    _linStart = true;
                    break;
                case 4:
                    _enabled = (data != 0);
                    if (!_enabled)
                    {
                        _length = 0;
                    }
                    break;
            }
        }

        void Run(int end_time)
        {
            
            int period = _period + 1;
            if (_linCtr == 0 || _length == 0 || _period < 4)
            {
                // leave it at it's current phase
                _time = end_time;
                return;
            }

            for (; _time < end_time; _time += period, _phase = (_phase + 1) % 32)
            {
                UpdateAmplitude(_phase < 16 ? _phase : 31 - _phase);
            }
        }

        private void UpdateAmplitude(int new_amp)
        {
            int delta = new_amp * _gain - _amplitude;
            _amplitude += delta;
            _bleeper.blip_add_delta(_time, delta);
        }

        public void EndFrame(int time)
        {
            Run(time);
            _time = 0;
        }

        public void FrameClock(int time, int step)
        {
            Run(time);

            if (_linStart)
            {
                _linCtr = _linVal;

            }
            else
            {
                if (_linCtr > 0)
                {
                    _linCtr--;
                }
            }

            if (!_looping) _linStart = false;

            switch (step)
            {
                case 1:
                case 3:
                    if (_length > 0 && !_looping)
                    {
                        _length--;
                    }
                    break;
            }
        }

    }

}
