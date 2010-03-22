using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.PPUClasses;
using System.IO;
using NES.CPU.Fastendo;
using System.Threading;
using NES.CPU.Machine.Carts;
using NES.CPU.Machine.BeepsBoops;
using NES.CPU.Machine;
using NES.CPU.Machine.ROMLoader;
using NES.Sound;
using NES.CPU.PixelWhizzlerClasses;
using CPU6502.Machine;

namespace NES.CPU.nitenedo
{
    public partial class NESMachine
    {
        private CPU2A03 _cpu;

        public CPU2A03 Cpu
        {
            get { return _cpu; }
            set { _cpu = value; }
        }
        private IPPU _ppu;
        private INESCart _cart;

        public INESCart Cart
        {
            get { return _cart; }
        }

        public void WriteWAVToFile(IWavWriter writer)
        {
            _sharedWave.AppendFile(writer);
        }

        public void StopWritingWAV()
        {
            _sharedWave.AppendFile(null);
        }

        WavSharer _sharedWave;
        Bopper soundBopper;

        public Bopper SoundBopper
        {
            get { return soundBopper; }
            set { soundBopper = value; }
        }

        public event EventHandler<SoundStatusChangeEventArgs> SoundStatusChanged;


        public IWavReader WaveForms
        {
            get { return _sharedWave; }
        }

        private bool _enableSound = true;

        public bool EnableSound
        {
            get { return _enableSound; }
            set {

                if (_enableSound != value)
                {
                    if (SoundStatusChanged != null)
                        SoundStatusChanged(this, new SoundStatusChangeEventArgs() { Muted = !value });
                    soundBopper.Muted = !value;
                    _enableSound = value;
                }
            }
        }

        public void SetupSound()
        {
            _sharedWave = new WavSharer();
            //writer = new wavwriter(44100, "d:\\nesout.wav");
            soundBopper = new Bopper(_sharedWave);
        }


        bool breakpointHit = false;

        TileDoodler tiler;

        public TileDoodler Tiler
        {
            get { return tiler; }
        }
        
        TextWriter debug;

        SoundThreader soundThreader;
        bool doDraw = false;
        //zzzz bloop;
        public NESMachine(CPU2A03 cpu, IPPU ppu, TileDoodler tiler, WavSharer wavSharer, Bopper soundBopper, SoundThreader soundThread)
        {

            machineWorkQueue = new MachineQueue(UpdateQueue);

            _cpu = cpu;
            _ppu = ppu;
            _ppu.FrameFinishHandler = new MachineEvent(FrameFinished);
            this.tiler = tiler;

            DebugStream = null;

            _sharedWave = wavSharer;
            this.soundBopper = soundBopper;
            _cpu.SoundBopper = soundBopper;

            soundThreader = soundThread;
            this.SoundStatusChanged += soundThreader.OnSoundStatusChanged;

            Initialize();
        }

        //public NESMachine()
        //{
        //    // create new memory map
        //    // add cpu, ppu, input etc to it
        //    DebugStream = null;

        //    _cpu = new CPU2A03();
        //    _ppu = new PixelWhizzler();
        //    _ppu.FrameFinishHandler = StartDraw;

        //    //TODO: only hook this up when debugging, it doesnt need to live in ram
        //    tiler = new TileDoodler(_ppu);

        //    _cpu.PixelWhizzler = _ppu;
            
        //    SetupSound();

        //    _cpu.SoundBopper = soundBopper;
        //    //Blip blipper = new Blip();
        //    //debug = new StreamWriter(new FileStream("d:\\soundout.txt", FileMode.Create, FileAccess.Write));
        //    //bloop = new Blooper(blipper, debug);

        //    //_cpu.Blooper = bloop;
            
        //    Initialize();
        //}

        public int FrameCount
        {
            get { return frameCount; }
            set { frameCount = value; }
        }

        public event EventHandler Drawscreen;
        

        public StreamWriter DebugStream
        {
            get;
            set;
        }

        public bool IsRunning
        {
            get { return true; }
        }

        public IPPU PPU
        {
            get { return _ppu; }
        }

        public IControlPad PadOne
        {
            get { return _cpu.PadOne.ControlPad; }
            set { _cpu.PadOne.ControlPad = value; }
        }

        public IControlPad PadTwo
        {
            get { return _cpu.PadTwo.ControlPad; }
            set { _cpu.PadTwo.ControlPad = value;
                    PPU.PixelAwareDevice = value as IPixelAwareDevice;
            }
        }
        

    }
}
