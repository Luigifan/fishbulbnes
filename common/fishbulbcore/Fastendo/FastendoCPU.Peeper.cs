using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.Fastendo
{
    public partial class CPU2A03
    {
        int[] instructionUsage = new int[256];
        bool _debugging = false;

        public bool Debugging
        {
            get { return _debugging; }
            set { _debugging = value; }
        }

        public int[] InstructionUsage
        {
            get { return instructionUsage; }
        }

        // track last 256 instructions
        private int instructionHistoryPointer = 0xFF;

        public int InstructionHistoryPointer
        {
            get { return instructionHistoryPointer; }
        }
        private Instruction[] _instructionHistory = new Instruction[0x100];

        public Instruction[] InstructionHistory
        {
            get { return _instructionHistory; }
        }

        public void WriteInstructionHistoryAndUsage()
        {
            _instructionHistory[(instructionHistoryPointer--) & 0xFF] = new Instruction(_currentInstruction);
            instructionUsage[_currentInstruction.OpCode]++;

            
        }

        public Instruction PeekInstruction(int address)
        {
            Instruction inst = new Instruction();

            inst.OpCode = GetByte(address++);
            inst.AddressingMode = addressmode[inst.OpCode];
            inst.Length = 1;
            FetchInstructionParameters(ref inst, address);
            return inst;
        }
    }
}
