using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WPFamicom.ControlPanelMVVM
{
    public class WaveFileSaver: IDisposable, NES.CPU.Machine.BeepsBoops.IWavWriter
    {
        
        const int sample_size = 2;
        int sample_count, sample_rate, chan_count = 1;

        string _fileName;
        BinaryWriter outFile;

        public WaveFileSaver(int sampleRate, string fileName)
        {
            wave_open(sampleRate, fileName);
        }

        private void wave_open(int new_sample_rate, string filename)
        {
            sample_rate = new_sample_rate;
            _fileName = filename;
            outFile = new BinaryWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));

            write_header();

        }

        private void wave_close()
        {
            if (outFile != null)
            {
                outFile.Seek(0, SeekOrigin.Begin);
                //rewind( file );
                write_header();

                outFile.Close();

                //fclose( file );
                //file = NULL;
            }

            sample_count = 0;
            chan_count = 1;
        }

        public void WriteWaves(byte[] inBuff, int remain)
        {


            outFile.Write(inBuff, 0, remain);
            sample_count += remain / 2;
        }

        public void set_le32(ref byte[] p, int offset, uint n)
        {
            p[0 + offset] = (byte)(n);
            p[1 + offset] = (byte)(n >> 8);
            p[2 + offset] = (byte)(n >> 16);
            p[3 + offset] = (byte)(n >> 24);
        }

        public void write_header()
        {
            int data_size = sample_size * sample_count;
            int frame_size = sample_size * chan_count;
            byte[] h = new byte[0x2C] 
	        {
		        0,0,0,0 , // where RIFF is written
		        0,0,0,0,        /* length of rest of file */
		        0,0,0,0, // where WAVE is written
		        0,0,0,0, // 'f','m','t',' ',// where 'fmt ' is written
		        16,0,0,0,       /* size of fmt chunk */
		        1,0,            /* uncompressed format */
		        0,0,            /* channel count */
		        0,0,0,0,        /* sample rate */
		        0,0,0,0,        /* bytes per second */
		        0,0,            /* bytes per sample frame */
		        sample_size*8,0,/* bits per sample */
		        0,0,0,0,        //'d','a','t','a',
		        0,0,0,0         /* size of sample data */
		        /* ... */       /* sample data */
	        };
            Buffer.BlockCopy(Encoding.ASCII.GetBytes("RIFF"), 0, h, 0, 4);
            Buffer.BlockCopy(Encoding.ASCII.GetBytes("WAVE"), 0, h, 8, 4);
            Buffer.BlockCopy(Encoding.ASCII.GetBytes("fmt "), 0, h, 12, 4);
            Buffer.BlockCopy(Encoding.ASCII.GetBytes("data"), 0, h, 36, 4);

            set_le32(ref h, 0x04, (uint)(0x2C - 8 + data_size));
            h[0x16] = (byte)chan_count;
            set_le32(ref h, 0x18, (uint)sample_rate);
            set_le32(ref h, 0x1C, (uint)(sample_rate * frame_size));
            h[0x20] = (byte)frame_size;
            set_le32(ref h, 0x28, (uint)data_size);

            // write_data( h, sizeof h );
            outFile.Write(h);
        }

        public void wave_enable_stereo()
        {
            chan_count = 2;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (outFile != null)
            {
                wave_close();
            }
        }

        #endregion

    }
}
