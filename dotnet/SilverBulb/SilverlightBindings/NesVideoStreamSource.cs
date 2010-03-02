using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using NES.CPU.Machine.BeepsBoops;
using System.Threading;
using NES.CPU.nitenedo;

namespace SilverlightBindings
{
    public class NesVideoStreamSource : MediaStreamSource, IDisposable
    {
        private WaveFormatEx _waveFormat;
        private MediaStreamDescription _audioDesc;
        private MediaStreamDescription _videoDesc;

        private long _currentTimeStamp;
        private long _currentVideoTimeStamp;
        private long _frameTime = TimeSpan.FromMilliseconds((double)1000/120).Ticks;
        long _framePosition =0;

        private long _currentPosition;
        private long _startPosition;

        private const int SampleRate = 44100;
        private const int ChannelCount = 1;
        private const int BitsPerSample = 16;
        private const int ByteRate =
            SampleRate * ChannelCount * BitsPerSample / 8;

        private MemoryStream _stream, _frameStream;
        private Random _random = new Random();

        // you only need sample attributes for video
        private Dictionary<MediaSampleAttributeKeys, string> _emptySampleDict =
            new Dictionary<MediaSampleAttributeKeys, string>();

        private Dictionary<MediaSampleAttributeKeys, string> _vidSampleDict =
            new Dictionary<MediaSampleAttributeKeys, string>();


        byte[][] frames = new byte[2][];
        int currentFrame = 0;


        public NesVideoStreamSource()
        {

            _waveFormat = new WaveFormatEx();
            _waveFormat.BitsPerSample = 16;
            _waveFormat.AvgBytesPerSec = (int)ByteRate;
            _waveFormat.Channels = ChannelCount;
            _waveFormat.BlockAlign = ChannelCount * (BitsPerSample / 8);
            _waveFormat.ext = null; // ??
            _waveFormat.FormatTag = WaveFormatEx.FormatPCM;
            _waveFormat.SamplesPerSec = SampleRate;
            _waveFormat.Size = 0; // must be zero

            _waveFormat.ValidateWaveFormat();

            _stream = new MemoryStream();
            _frameStream = new MemoryStream();

            for (int i = 0; i < buffers.Length; ++i)
            {
                buffers[i] = new byte[4096];
            }

            for (int i = 0; i < frames.Length; ++i)
            {
                frames[i] = new byte[256 * 256 * 4];
            }

            base.AudioBufferLength = 30;
        }


        protected override void OpenMediaAsync()
        {
            System.Diagnostics.Debug.WriteLine("Started OpenMediaAsync");

            _startPosition = _currentPosition = 0;


            // Init
            Dictionary<MediaStreamAttributeKeys, string> streamAttributes =
                new Dictionary<MediaStreamAttributeKeys, string>();
            Dictionary<MediaSourceAttributesKeys, string> sourceAttributes =
                new Dictionary<MediaSourceAttributesKeys, string>();
            List<MediaStreamDescription> availableStreams =
                new List<MediaStreamDescription>();

            // Stream Description and WaveFormatEx
            streamAttributes[MediaStreamAttributeKeys.CodecPrivateData] =
                _waveFormat.ToHexString(); // wfx
            MediaStreamDescription msd =
                new MediaStreamDescription(MediaStreamType.Audio,
                                            streamAttributes);
            _audioDesc = msd;
            _videoDesc = PrepareVideo();
            // next, add the description so that Silverlight will
            // actually request samples for it
            availableStreams.Add(_audioDesc);
            availableStreams.Add(_videoDesc);

            // Tell silverlight we have an endless stream
            sourceAttributes[MediaSourceAttributesKeys.Duration] =
                TimeSpan.FromMinutes(0).Ticks.ToString(
                                    CultureInfo.InvariantCulture);

            // we don't support seeking on our stream
            sourceAttributes[MediaSourceAttributesKeys.CanSeek] =
                false.ToString();

            
            // tell Silverlight we're done opening our media
            ReportOpenMediaCompleted(sourceAttributes, availableStreams);

            //System.Diagnostics.Debug.WriteLine("Completed OpenMediaAsync");
        }

        private MediaStreamDescription PrepareVideo()
        {
            _frameStream = new MemoryStream();

            // Stream Description 
            Dictionary<MediaStreamAttributeKeys, string> streamAttributes =
                new Dictionary<MediaStreamAttributeKeys, string>();

            streamAttributes.Add(MediaStreamAttributeKeys.VideoFourCC, "RGBA");
            streamAttributes.Add(MediaStreamAttributeKeys.Height, "240");
            streamAttributes.Add( MediaStreamAttributeKeys.Width , "256");

            return
                new MediaStreamDescription(MediaStreamType.Video, streamAttributes);
        }


        protected override void CloseMedia()
        {
            System.Diagnostics.Debug.WriteLine("CloseMedia");
            // Close the stream
            _startPosition = _currentPosition = 0;
            _audioDesc = null;
            _videoDesc = null;
        }

        protected override void GetDiagnosticAsync(
            MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            throw new NotImplementedException();
        }


        byte[][] buffers = new byte[4][];
        int[] bufferLen = new int[4];
        int bufferPlaying = 0;

        IWavReader reader;

        public IWavReader Reader
        {
          get { return reader; }
          set { reader = value; }
        }

        NESMachine targetMachine;

        public NESMachine TargetMachine
        {
            get { return targetMachine; }
            set { targetMachine = value;
            //    targetMachine.Drawscreen += new EventHandler(targetMachine_Drawscreen);
            }
        }

        void targetMachine_Drawscreen(object sender, EventArgs e)
        {
            Buffer.BlockCopy(TargetMachine.PPU.VideoBuffer, 0, frames[currentFrame], 0, 256 * 240 * 4);
        }

        public void WriteSamples()
        {
            bufferLen[bufferPlaying] = reader.SharedBufferLength;
            Buffer.BlockCopy(TargetMachine.PPU.VideoBuffer, 0, frames[currentFrame], 0, 256 * 240 * 4);
        }

        public void Wait()
        {
            waitEvent.WaitOne();
            waitEvent.Reset();
        }

        ManualResetEvent waitEvent = new ManualResetEvent(false);

        volatile bool ending = false;

        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {

            int bufferByteCount = 0;

            if (ending) return;


            if (mediaStreamType == MediaStreamType.Audio)
            {

                _stream.Write(buffers[bufferPlaying], 0, bufferLen[bufferPlaying]);
                bufferByteCount = bufferLen[bufferPlaying];

                bufferPlaying++;
                if (bufferPlaying >= buffers.Length) bufferPlaying = 0;

                reader.SharedBuffer = buffers[bufferPlaying];

                waitEvent.Set();

                // Send out the next sample
                ReportGetSampleCompleted(
                    new MediaStreamSample(
                    _audioDesc,
                    _stream,
                    _currentPosition,
                    bufferByteCount,
                    _currentTimeStamp,
                    _emptySampleDict)
                );

                // Move our timestamp and position forward
                _currentTimeStamp += _waveFormat.AudioDurationFromBufferSize(
                                        (uint)bufferByteCount);
                _currentPosition += bufferByteCount;

            }
            else if (mediaStreamType == MediaStreamType.Video)
            {
                
                _frameStream.Position = 0;
                _frameStream.Write(frames[currentFrame], 0, 256 * 240 * 4);
                currentFrame++;
                if (currentFrame >= frames.Length)
                    currentFrame = 0;


                ReportGetSampleCompleted(new MediaStreamSample(
                    _videoDesc,
                    _frameStream,
                    0,
                    256 * 240 * 4,
                    _currentVideoTimeStamp,
                    _emptySampleDict));

                _currentVideoTimeStamp += _frameTime;
            }
            
        }

        


        protected override void SeekAsync(long seekToTime)
        {
            ReportSeekCompleted(seekToTime);
        }

        protected override void SwitchMediaStreamAsync(
            MediaStreamDescription mediaStreamDescription)
        {
            throw new NotImplementedException();
        }



        public void Dispose()
        {
            ending = true;
            waitEvent.Set();
            _stream.Dispose();

        }
    }
}
