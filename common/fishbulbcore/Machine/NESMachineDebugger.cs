using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.CPUDebugging;
using NES.CPU.Fastendo;
using NES.CPU.FastendoDebugging;
using NES.CPU.Machine.FastendoDebugging;
using NES.CPU.PPUClasses;
namespace NES.CPU.nitenedo
{
    public partial class NESMachine
    {
        List<CPUBreakpoint> breakPoints = new List<CPUBreakpoint>();

        DebugInformation _debugInfo = new DebugInformation();

        public event EventHandler<BreakEventArgs> DebugInfoChanged;
        public event EventHandler<BreakEventArgs> BreakpointHit;

        // this routine is called from inside the machine thread, it creates a *new* instance of 
        // _debugInfo, so as to expose it to the UI as an atomic unit and let the property handler
        // deal with the lock
        private void CreateNewDebugInformation()
        {
            DebuggerCPUState _cpuState = new DebuggerCPUState()
            {
                Accumulator = _cpu.Accumulator,
                IndexRegisterX = _cpu.IndexRegisterX,
                IndexRegisterY = _cpu.IndexRegisterY,
                ProgramCounter = _cpu.ProgramCounter,
                StatusRegister = _cpu.StatusRegister,
                StackPointer = _cpu.StackPointer
            };

            _cpuState.CurrentInstruction = _cpu.CurrentInstruction;

            _cpuState.InstructionHistory = new NES.CPU.Fastendo.CPU2A03.Instruction[0x100];
            for (int i = 0; i < 0x100; ++i)
            {
                _cpuState.InstructionHistory[i] = _cpu.InstructionHistory[i];
            }
            _cpuState.InstructionHistoryPointer = _cpu.InstructionHistoryPointer & 0xFF;

            _cpu.InstructionUsage.CopyTo(_cpuState.InstructionUsage, 0);



            DebuggerPPUState _ppuState = new DebuggerPPUState()
            {
                IsRendering = _ppu.IsRendering,
                PPUControl = _ppu.PPUControlByte0,
                PPUStatus = _ppu.PPUStatus,
                ScanLine = _ppu.ScanlineNum,
                ScanlinePos = _ppu.ScanlinePos
            };

            _ppuState.FrameWriteEvents = new List<PPUWriteEvent>();
            while (_ppu.Events.Count > 0)
            {
                _ppuState.FrameWriteEvents.Add(_ppu.Events.Dequeue());
            }

            DebugInfo = new DebugInformation() { CPU = _cpuState, PPU = _ppuState };

            DebugInfo.UpdateFutureRollout(_cpu);
        }

        object debugLock = new object();

        // this is meant to be bound to by the debugging UI, written to by the game thread
        public DebugInformation DebugInfo
        {
            get {
                lock (debugLock)
                {
                    return _debugInfo;
                }
            }
            set {
                lock (debugLock)
                {
                    _debugInfo = value;
                }
            }
        }

        public List<CPUBreakpoint> BreakPoints
        {
            get { return breakPoints; }
            set { breakPoints = value; }
        }

        public List<CPU2A03.Instruction> execHistory = new List<CPU2A03.Instruction>(256);

        private void HandleBreaks()
        {
            if (breakPoints.Count > 0)
            {
                var result = from bp in breakPoints where bp.Address == _cpu.ProgramCounter select bp;
                if (result.Count() > 0)
                {
                    breakpointHit = true;
                }
            }
            //if (_cpu.CurrentInstruction.AddressingMode == AddressingModes.Bullshit)
            //{
            //    breakpointHit = true;
            //}
            //if (_cpu.CurrentInstruction.OpCode == 0x0) //BRK
            //{
            //    breakpointHit = true;
            //}
            if (breakpointHit && DebugInfoChanged != null)
            {
                DebugInfoChanged(this, new BreakEventArgs());
            }
        }

    }
}
