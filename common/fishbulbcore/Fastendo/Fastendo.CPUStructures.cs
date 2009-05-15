using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.Fastendo
{
    public partial class CPU2A03
    {
        private struct smallInstruction
        {
            uint instructionByte;

            public static Instruction UnpackInstruction(uint instruction)
            {
                Instruction inst = new Instruction();
                inst.OpCode = (int)instruction & 0xFF;
                inst.Parameters0 = (int)(instruction >> 8) & 0xFF;
                inst.Parameters1 = (int)(instruction >> 16) & 0xFF;
                return inst;
            }

        }


        public class Instruction
        {
            public Instruction(){}
            public Instruction(Instruction inst)
            {
                AddressingMode = inst.AddressingMode;
                Address = inst.Address;
                OpCode = inst.OpCode;
                Parameters0 = inst.Parameters0;
                Parameters1 = inst.Parameters1;
                ExtraTiming = inst.ExtraTiming;
                Length = inst.Length;
            }

            public AddressingModes AddressingMode;
            // 2 bytes
            public int Address;
            // one byte
            public int OpCode;
            // one byte
            public int Parameters0;
            // one byte
            public int Parameters1;

            // lookedup
            public int ExtraTiming;
            public int Length;
        }

        public class CPUStatus
        {
            public int StatusRegister;
            public int ProgramCounter;
            public int Accumulator;
            public int IndexRegisterX;
            public int IndexRegisterY;
        }

    }
}
