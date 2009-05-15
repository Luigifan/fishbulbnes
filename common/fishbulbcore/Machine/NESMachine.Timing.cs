using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Fastendo;

namespace NES.CPU.nitenedo
{
    public partial class NESMachine
    {
        int _totalCPUClocks = 0;
        int frameCount = 0;


        //bool handlingNMI = false;
        /// <summary>
        /// runs a "step", either a pending non-maskable interrupt, maskable interupt, or a sprite DMA transfer,
        ///  or a regular machine cycle, then runs the appropriate number of PPU clocks based on CPU action
        ///  
        ///  ppuclocks = cpuclocks * 3
        ///  
        /// note: this approach relies on very precise cpu timing
        /// </summary>
        
        public void Step()
        {
            _cpu.Step();

            _totalCPUClocks = _cpu.Clock;
            //_cpu.Clock = _totalCPUClocks;
            HandleBreaks();

        }

        bool frameEnded = false;

        public void RunFrame()
        {

            frameOn = true;

            _cpu.FindNextEvent();
            do
            {
                _cpu.Step();
            } while (frameOn);

            _totalCPUClocks = _cpu.Clock;
            lock (_sharedWave)
            {
                soundBopper.FlushFrame(_totalCPUClocks);
                soundBopper.EndFrame(_totalCPUClocks);
                //_sharedWave.SyncUp();
            }
            if (Drawscreen != null)
                Drawscreen(this, new EventArgs());

            _totalCPUClocks = 0;
            _cpu.Clock = 0;
            _ppu.LastcpuClock = 0;

        }

        bool frameOn =true;

        void StartDraw()
        {

            frameOn = false;

            framesRendered = framesRendered + 1;
        }
    }
}
