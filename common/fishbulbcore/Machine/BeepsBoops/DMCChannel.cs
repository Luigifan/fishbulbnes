using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.Machine.BeepsBoops
{
    public class DMCChannel
    {

        int _chan;
        private Blip _bleeper;

        public DMCChannel(Blip bleeper, int chan)
        {
            _bleeper = bleeper;
            _chan = chan;
        }

        byte[] LengthCounts = new byte[] 
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

        private int _dutyCycle;

        private int _length;

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        /// <summary>
        /// Duty cycle of current square wave
        /// </summary>
        public int DutyCycle
        {
            get { return _dutyCycle; }
            set { _dutyCycle = value; }
        }

        private int _timer;
        /// <summary>
        /// Period of current waveform
        /// </summary>
        public int Period
        {
            get { return _timer; }
            set { _timer = value; }
        }

        private int _rawTimer;

        private int _volume;

        /// <summary>
        /// Volume envelope for current waveform
        /// </summary>
        public int Volume
        {
            get { return _volume; }
            set { _volume = value; }
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

        private byte[] doodies = new byte[] { 0x02, 0x6, 0x1E, 0xF9 };

        private int _sweepShift, _sweepCounter, _sweepDivider = 1;
        bool _sweepNegateFlag, _sweepEnabled, _startSweep, _sweepInvalid;

        bool _irqEnabled = false;
        private int rate;
        private int dCounter;
        int sampleAddress;
        public void WriteRegister(int register, int data, int time)
        {
            // Run(time);

            switch (register)
            {
                case 0:
                    _irqEnabled = (data & 0x80) == 0x80;
                    _looping = (data & 0x40) == 0x40;
                    rate = data & 0xF;
                    break;
                // sweep
                case 1:
                    dCounter = data & 0x7F;
                    break;
                // period low
                case 2:
                    sampleAddress = (data << 6) | 0xC000;

                    break;
                // period high
                case 3:
                    _timer = data & 0xFF;
                    _timer <<= 4;
                    _timer &= 1;

                    break;

            }
        }


        int _phase; // phase of current waveform (0-7)

        int _gain;  // master gain of channel

        /// <summary>
        /// Master gain
        /// </summary>
        public int Gain
        {
            get { return _gain; }
            set { _gain = value; }
        }

        private void Run(int end_time)
        {

            for (; _time < end_time; _time ++)
            {
                UpdateAmplitude((_dutyCycle >> (_phase & 0x7) & 1) );
            }
            _phase &= 7;
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

        private int _envTimer = 0xF;
        private bool _envStart;
        private bool _envConstantVolume;
        private int _envVolume;

        private bool _sweepComplement;
        /// <summary>
        /// True for ones complement, false for twos complement
        /// </summary>
        public bool SweepComplement
        {
            get { return _sweepComplement; }
            set { _sweepComplement = value; }
        }


        public void FrameClock(int time, int step)
        {
            Run(time);

            if (!_envStart)
            {
                _envTimer--;
                if (_envTimer == 0)
                {
                    _envTimer = _volume + 1;
                    if (_envVolume > 0)
                    {
                        _envVolume--;
                    }
                    else
                    {
                        _envVolume = _looping ? 0xF : 0;
                    }
                }
            }
            else
            {
                _envStart = false;
                _envTimer = _volume + 1;
                _envVolume = 0xF;
            }

            switch (step)
            {
                case 1:
                case 3:
                    --_sweepCounter;
                    if (_sweepCounter == 0)
                    {
                        _sweepCounter = _sweepDivider + 1;
                        if (_sweepEnabled && _sweepShift > 0)
                        {
                            int sweep = _timer >> _sweepShift;
                            if (_sweepComplement)
                            {
                                _timer += _sweepNegateFlag ? ~sweep : sweep;
                            }
                            else
                            {
                                _timer += _sweepNegateFlag ? ~sweep + 1 : sweep;
                            }
                            _sweepInvalid = (_rawTimer < 8 ||
                                (_timer & 0x800) == 0x800);
                            //if (_sweepInvalid)
                            //{
                            //    _sweepInvalid = true;
                            //}
                        }
                    }
                    if (_startSweep)
                    {
                        _startSweep = false;
                        _sweepCounter = _sweepDivider + 1;

                    }
                    if (!_looping && _length > 0)
                    {
                        _length--;
                    }

                    break;
            }
        }
    }

}
