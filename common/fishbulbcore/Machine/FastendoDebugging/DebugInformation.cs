using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.CPUDebugging;
using NES.CPU.Fastendo;

namespace NES.CPU.FastendoDebugging
{
    public class DebugInformation
    {
        private DebuggerCPUState _cpu;

        public DebuggerCPUState CPU
        {
            get { return _cpu; }
            set { _cpu = value; }
        }

        private DebuggerPPUState _ppu;

        public DebuggerPPUState PPU
        {
            get { return _ppu; }
            set { _ppu = value; }
        }

        private List<string> _instructionUsage = new List<string>();

        public List<string> InstructionUsage
        {
            get
            {
                if (_cpu != null)
                {
                    _instructionUsage.Clear();
                    for (int i = 0; i < 256; ++i)
                    {
                        if (_cpu.InstructionUsage[i] > 0)
                        {
                            _instructionUsage.Add(string.Format("{0:x2} {1} {2} {3}",
                                i,
                                DisassemblyExtensions.GetMnemnonic(i),
                                _cpu.InstructionUsage[i],
                                _cpu.addressmode[i]));
                        }
                    }
                }
                return _instructionUsage;
            }
        }

        private List<string> _instructionHistory = new List<string>();

        public List<string> InstructionHistory
        {
            get
            {
                if (_cpu == null) return null;
                _instructionHistory.Clear();

                for (int i = _cpu.InstructionHistoryPointer & 0xFF; i >= 0; --i)
                {
                    string nesCPUInstructionHistoryDisassemble =  _cpu.InstructionHistory[i].Disassemble();
                    if (nesCPUInstructionHistoryDisassemble.Trim() != string.Empty)
                        _instructionHistory.Add(nesCPUInstructionHistoryDisassemble);
                }
                for (int i = 0xFF; i > (_cpu.InstructionHistoryPointer & 0xFF); --i)
                {
                    string nesCPUInstructionHistoryDisassemble = _cpu.InstructionHistory[i].Disassemble();
                    if (nesCPUInstructionHistoryDisassemble.Trim() != string.Empty)
                        _instructionHistory.Add(nesCPUInstructionHistoryDisassemble);
                }

                return _instructionHistory;
            }
        }

        private bool autoRollout = false;

        public bool AutoRollout
        {
            get { return autoRollout; }
            set { autoRollout = value; }
        }

        private InstructionRollout _futureOps = new InstructionRollout();

        public InstructionRollout FutureOps
        {
            get { return _futureOps; }
        }

        public void UpdateFutureRollout(CPU2A03 cpu)
        {
            _futureOps.CreateRollout(cpu, new List<CPUBreakpoint>(), 32);
        }

    }
}
