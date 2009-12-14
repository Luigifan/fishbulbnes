using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.Machine.BeepsBoops
{
    public class NoiseChannel
    {

        Blip _bleeper;
        int _chan;
        public NoiseChannel(Blip bleeper, int chan)
        {
            _bleeper = bleeper;
            _chan = chan;
        }

        int[] NoisePeriods = new int[]
        {
            4, 8, 16, 32, 64, 96, 128, 160, 202, 254, 380, 508, 762, 1016, 2034, 4068
        };


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

        private bool _envConstantVolume;
        private int _envVolume;

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

        private int amplitude;

        private int _phase = 1;

        private int gain;

        public int Gain
        {
            get { return gain; }
            set { gain = value; }
        }


        public void WriteRegister(int register, int data, int time)
        {
           // Run(time);

            switch (register)
            {
                case 0:
                    _envConstantVolume = (data & 0x10) == 0x10;
                    _volume = data & 0xF;
                    _looping = (data & 0x80) == 0x80;
                    break;
                // sweep
                case 1:

                    break;
                // period low
                case 2:
                    _period = NoisePeriods[data & 0xF];
                    // _period |= data;

                    break;
                case 3:
                    // setup length
                    if (_enabled)
                    {
                        _length = LengthCounts[(data >> 3) & 0x1f];
                    }
                    _envStart = true;
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
            int volume = _envConstantVolume ? _volume : _envVolume;
            if (_length == 0 ) volume = 0;
            if ( _period == 0 )
            {
                _time = end_time;
                UpdateAmplitude(0);
                return;
            }
            
            if (_phase == 0)
                _phase = 1;

            for (; _time < end_time; _time += _period)
            {
                int new15;
                if (_looping)
                {
                    new15 = ((_phase & 1) ^ ((_phase >> 6) & 1)) ;
                }
                else
                {
                    new15 = ((_phase & 1) ^ ((_phase >> 1) & 1));
                }
                UpdateAmplitude(_phase & 1 * volume);
                _phase = ((_phase >> 1) | (new15 << 14)) & 0xFFFF;
                
                
                
            }
        }

        private void UpdateAmplitude(int amp)
        {
            int delta = amp * gain - amplitude;
            amplitude += delta;
            _bleeper.blip_add_delta(_time, delta);
        }

        public void EndFrame(int time)
        {
            Run(time);
            _time = 0;
        }

        private int _envTimer = 0xF;
        private bool _envStart;

        public void FrameClock(int time, int step)
        {
            Run(time);

            if (!_envStart)
            {
                _envTimer--;
                if (_envTimer == 0 )
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
                case 1: case 2:
                if (!_looping & _length > 0)
                {
                    _length--;
                }
                break;
            }
        }


    }
}
