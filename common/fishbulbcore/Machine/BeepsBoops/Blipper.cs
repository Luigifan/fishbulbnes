using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.Machine.BeepsBoops
{
    public class Blip
    {
        const int bass_shift = 8; /* affects high-pass filter breakpoint frequency */
        const int end_frame_extra = 2; /* allows deltas slightly after frame length */
        const int time_bits = 21;

        const int half_width = 8;
        const int phase_bits = 5;
        const int delta_bits = 15;
        static readonly int buf_extra;
        static readonly int phase_count;
        static readonly int time_unit;

        static Blip()
        {
            time_unit = 1 << time_bits;
            buf_extra = half_width * 2 + end_frame_extra;
            phase_count = 1 << phase_bits;
        }

        public Blip(int size)
        {
            blip_new(size);
        }

        public class blip_buffer_t
        {
            public blip_buffer_t(int size)
            {
                samples = new int[size];
                arrayLength = size;

            }
            internal int factor;
            internal int offset;
            internal int avail;
            internal int size;
            internal int integrator;

            internal int arrayLength;
            public int[] samples;
        };

        private blip_buffer_t _blipBuffer;

        public blip_buffer_t BlipBuffer
        {
            get { return _blipBuffer; }
            set { _blipBuffer = value; }
        }

        private void blip_new(int size)
        {
            _blipBuffer = new blip_buffer_t(size);
            _blipBuffer.size = size;
            _blipBuffer.factor = 0;
            blip_clear();
        }

        public void blip_set_rates(double clock_rate, int sample_rate)
        {
            _blipBuffer.factor = (int)(time_unit / clock_rate * sample_rate +
                    (1 - 1.0 / 65536));

            /* Fails if clock_rate exceeds maximum, relative to sample_rate */
            System.Diagnostics.Debug.Assert(_blipBuffer.factor > 0);
        }


        //#define BLIP_SAMPLES( buf ) ((buf_t*) ((buf) + 1))


        void blip_clear()
        {
            _blipBuffer.offset = 0;
            _blipBuffer.avail = 0;
            _blipBuffer.integrator = 0;
            _blipBuffer.samples = new int[_blipBuffer.size + buf_extra];
            //memset(BLIP_SAMPLES(s), 0, (s.size + buf_extra) * sizeof(buf_t));
        }

        int blip_clocks_needed(int samples)
        {
            uint needed = (uint)(samples * time_unit - _blipBuffer.offset);

            /* Fails if buffer can't hold that many more samples */
            //assert( s->avail + samples <= s->size );

            return (int)((needed + _blipBuffer.factor - 1) / _blipBuffer.factor);

        }
        public void blip_end_frame( int t)
        {
            int off = t * _blipBuffer.factor + _blipBuffer.offset;
            _blipBuffer.avail += (int)(off >> time_bits);
            _blipBuffer.offset = (int)(off & (time_unit - 1));

            /* Fails if buffer size was exceeded */
            //assert(s->avail <= s->size);
        }

        public int blip_samples_avail
        {
            get
            {
                return _blipBuffer.avail;
            }
        }

        public void remove_samples(int count)
        {
            int remain = _blipBuffer.avail + buf_extra - count;
            _blipBuffer.avail -= count;
            //for (int i = 0; i < remain; ++i)
            //{
            //    _blipBuffer.samples[i] = _blipBuffer.samples[i + count];
            //}
            //for (int i = 0; i < count; ++i)
            //{
            //    _blipBuffer.samples[i + remain] = 0;
            //}
            
            Array.Copy(_blipBuffer.samples, count, _blipBuffer.samples, 0, remain);
            Array.Clear(_blipBuffer.samples, remain, count);

            _blipBuffer.arrayLength = count;
            //memmove( &buf [0], &buf [count], remain * sizeof buf [0] );
            //memset( &buf [remain], 0, count * sizeof buf [0] );
        }

        public int blip_read_samples(short[] outbuf, int count, int stereo)
        {
            if (count > _blipBuffer.avail)
                count = _blipBuffer.avail;

            if (count != 0)
            {
                int step = (stereo != 0) ? 2 : 1;
                //int inPtr  = BLIP_SAMPLES( s );
                //buf_t const* end = in + count;
                int inPtr = 0, outPtr = 0;
                int endPtr = inPtr + count;
                int sum = _blipBuffer.integrator;

                do
                {
                    int st = sum >> delta_bits; /* assumes right shift preserves sign */
                    sum = sum + _blipBuffer.samples[inPtr];
                    inPtr++;
                    if ((short)st != st) /* assumes signed cast merely truncates */
                        st = (st >> 31) ^ 0x7FFF;
                    outbuf[outPtr] = (short)(st);
                    outPtr += step;
                    sum = sum - (st << (delta_bits - bass_shift));
                }
                while (inPtr != endPtr);

                _blipBuffer.integrator = sum;

                remove_samples(count);
            }

            return count;
        }

        public int ReadBytes(byte[] outbuf, int count, int stereo)
        {
            if (count > _blipBuffer.avail)
                count = _blipBuffer.avail;

            if (count != 0)
            {
                const int step =  2;
                //int inPtr  = BLIP_SAMPLES( s );
                //buf_t const* end = in + count;
                int inPtr = 0, outPtr = 0;
                int endPtr = inPtr + count;
                int sum = _blipBuffer.integrator;

                do
                {
                    int st = sum >> delta_bits; /* assumes right shift preserves sign */
                    sum = sum + _blipBuffer.samples[inPtr];
                    inPtr++;
                    if ((short)st != st) /* assumes signed cast merely truncates */
                        st = (st >> 31) ^ 0x7FFF;
                    outbuf[outPtr] = (byte)(st);
                    outbuf[outPtr+ 1] = (byte)(st >> 8);
                    outPtr += step;
                    sum = sum - (st << (delta_bits - bass_shift));
                }
                while (inPtr != endPtr);

                _blipBuffer.integrator = sum;

                remove_samples(count);
            }

            return count;
        }

        /* Sinc_Generator( 0.9, 0.55, 4.5 ) */
        static int[,] bl_step = new int[,] 
        {
            {   43, -115,  350, -488, 1136, -914, 5861,21022},
            {   44, -118,  348, -473, 1076, -799, 5274,21001},
            {   45, -121,  344, -454, 1011, -677, 4706,20936},
            {   46, -122,  336, -431,  942, -549, 4156,20829},
            {   47, -123,  327, -404,  868, -418, 3629,20679},
            {   47, -122,  316, -375,  792, -285, 3124,20488},
            {   47, -120,  303, -344,  714, -151, 2644,20256},
            {   46, -117,  289, -310,  634,  -17, 2188,19985},
            {   46, -114,  273, -275,  553,  117, 1758,19675},
            {   44, -108,  255, -237,  471,  247, 1356,19327},
            {   43, -103,  237, -199,  390,  373,  981,18944},
            {   42,  -98,  218, -160,  310,  495,  633,18527},
            {   40,  -91,  198, -121,  231,  611,  314,18078},
            {   38,  -84,  178,  -81,  153,  722,   22,17599},
            {   36,  -76,  157,  -43,   80,  824, -241,17092},
            {   34,  -68,  135,   -3,    8,  919, -476,16558},
            {   32,  -61,  115,   34,  -60, 1006, -683,16001},
            {   29,  -52,   94,   70, -123, 1083, -862,15422},
            {   27,  -44,   73,  106, -184, 1152,-1015,14824},
            {   25,  -36,   53,  139, -239, 1211,-1142,14210},
            {   22,  -27,   34,  170, -290, 1261,-1244,13582},
            {   20,  -20,   16,  199, -335, 1301,-1322,12942},
            {   18,  -12,   -3,  226, -375, 1331,-1376,12293},
            {   15,   -4,  -19,  250, -410, 1351,-1408,11638},
            {   13,    3,  -35,  272, -439, 1361,-1419,10979},
            {   11,    9,  -49,  292, -464, 1362,-1410,10319},
            {    9,   16,  -63,  309, -483, 1354,-1383, 9660},
            {    7,   22,  -75,  322, -496, 1337,-1339, 9005},
            {    6,   26,  -85,  333, -504, 1312,-1280, 8355},
            {    4,   31,  -94,  341, -507, 1278,-1205, 7713},
            {    3,   35, -102,  347, -506, 1238,-1119, 7082},
            {    1,   40, -110,  350, -499, 1190,-1021, 6464},
            {    0,   43, -115,  350, -488, 1136, -914, 5861}
        };

        public void blip_add_delta(int time, int delta)
        {
            if (delta == 0) return;
            long fixedTime = (long)(time * _blipBuffer.factor + _blipBuffer.offset);

            int outPtr = (int)(_blipBuffer.avail + (fixedTime >> time_bits));

            int phase_shift = time_bits - phase_bits;
            int phase = (int)(fixedTime >> phase_shift & (phase_count - 1));

            int inStep = phase; // bl_step[phase];
            int rev = phase_count - phase; // bl_step[phase_count - phase];

            int interp_bits = 15;
            int interp = (int)(fixedTime >> (phase_shift - interp_bits) & ((1 << interp_bits) - 1));
            int delta2 = (delta * interp) >> interp_bits;
            delta -= delta2;

            /* Fails if buffer size was exceeded */
            //assert( out <= &BLIP_SAMPLES( s ) [s->size] );

            for (int i = 0; i < 8; ++i)
            {
                _blipBuffer.samples[outPtr + i] += bl_step[inStep, i] * delta + bl_step[inStep + 1, i] * delta2;
                _blipBuffer.samples[outPtr + (15 - i)] += bl_step[rev, i] * delta + bl_step[rev - 1, i] * delta2;
            }

        }

        public void blip_add_delta_fast( int time, int delta)
        {
            int fixedTime = time * _blipBuffer.factor + _blipBuffer.offset;

            int outPtr = (int)(_blipBuffer.avail + (fixedTime >> time_bits));

            int delta_unit = 1 << delta_bits;
            int phase_shift = time_bits - delta_bits;
            int phase = (int)(fixedTime >> phase_shift & (delta_unit - 1));
            int delta2 = delta * phase;

            /* Fails if buffer size was exceeded */
            //assert( out <= &BLIP_SAMPLES( s ) [s->size] );


            _blipBuffer.samples[outPtr + 8] += delta * delta_unit - delta2;
            _blipBuffer.samples[outPtr + 9] += delta2;
            //out [8] += delta * delta_unit - delta2;
            //out [9] += delta2;
        }


    }
}
