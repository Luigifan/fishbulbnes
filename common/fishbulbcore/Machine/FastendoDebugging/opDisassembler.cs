using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU;
using NES.CPU.Fastendo;
using System.Xml;
using System.Reflection;
using System.Xml.Linq;

namespace NES.CPU.FastendoDebugging
{
    public static class DisassemblyExtensions
    {

        public static byte[] ToBytes(this NES.CPU.Fastendo.CPU2A03.Instruction inst)
        {
            switch (inst.Length)
            {
                case 1:
                    return new byte[] { (byte)inst.OpCode };
                case 2:
                    return new byte[] { (byte)inst.OpCode, (byte)inst.Parameters0  };
                case 3:
                    return new byte[] { (byte)inst.OpCode, (byte)inst.Parameters0, (byte)inst.Parameters1 };
                default:
                    return new byte[0];

            }
        }

        static DisassemblyExtensions()
        {
            SetupOpcodes();
        }

        public static AddressingModes[] addressmode = new AddressingModes[0x100];

        static void setupticks()
        {

            //clock[0x0] = 7;
            //instruction(0x0] = INS_BRK;
            addressmode[0x0] = AddressingModes.Implicit;
            //clock[0x1] = 6;
            //instruction(0x1] = INS_ORA;
            addressmode[0x1] = AddressingModes.IndexedIndirect;
            //clock[0x2] = 2;
            //instruction(0x2] = INS_NOP;
            addressmode[0x2] = AddressingModes.Implicit;
            //clock[0x3] = 2;
            //instruction(0x3] = INS_NOP;
            addressmode[0x3] = AddressingModes.Bullshit;
            //clock[0x4] = 3;
            //instruction(0x4] = INS_NOP;
            addressmode[0x4] = AddressingModes.Bullshit;
            //clock[0x5] = 3;
            //instruction(0x5] = INS_ORA;
            addressmode[0x5] = AddressingModes.ZeroPage;
            //clock[0x6] = 5;
            //instruction(0x6] = INS_ASL;

            addressmode[0x6] = AddressingModes.ZeroPage;

            // asl-ora
            //clock[0x7] = 2;
            //instruction(0x7] = INS_NOP;
            addressmode[0x7] = AddressingModes.Bullshit;

            //clock[0x8] = 3;
            //instruction(0x8] = INS_PHP;
            addressmode[0x8] = AddressingModes.Implicit;
            //clock[0x9] = 3;
            //instruction(0x9] = INS_ORA;
            addressmode[0x9] = AddressingModes.Immediate;
            //clock[0xa] = 2;
            //instruction(0xa] = INS_ASLA;
            addressmode[0xa] = AddressingModes.Accumulator;
            //clock[0xb] = 2;
            //instruction(0xb] = INS_NOP;
            addressmode[0xb] = AddressingModes.Implicit;
            //clock[0xc] = 4;
            //instruction(0xc] = INS_NOP;
            addressmode[0xc] = AddressingModes.Absolute;
            //clock[0xd] = 4;
            //instruction(0xd] = INS_ORA;
            addressmode[0xd] = AddressingModes.Absolute;
            //clock[0xe] = 6;
            //instruction(0xe] = INS_ASL;
            addressmode[0xe] = AddressingModes.Absolute;
            //clock[0xf] = 2;
            //instruction(0xf] = INS_NOP;
            addressmode[0xf] = AddressingModes.Implicit;
            //clock[0x10] = 2;
            //instruction(0x10] = INS_BPL;

            addressmode[0x10] = AddressingModes.Relative;
            //clock[0x11] = 5;
            //instruction(0x11] = INS_ORA;

            addressmode[0x11] = AddressingModes.IndirectIndexed;
            //clock[0x12] = 3;
            //instruction(0x12] = INS_ORA;

            addressmode[0x12] = AddressingModes.IndirectZeroPage;
            //clock[0x13] = 2;
            //instruction(0x13] = INS_NOP;
            addressmode[0x13] = AddressingModes.Implicit;
            //clock[0x14] = 3;
            //instruction(0x14] = INS_NOP;
            addressmode[0x14] = AddressingModes.ZeroPage;
            //clock[0x15] = 4;
            //instruction(0x15] = INS_ORA;
            addressmode[0x15] = AddressingModes.ZeroPageX;
            //clock[0x16] = 6;
            //instruction(0x16] = INS_ASL;
            addressmode[0x16] = AddressingModes.ZeroPageX;
            //clock[0x17] = 2;
            //instruction(0x17] = INS_NOP;
            addressmode[0x17] = AddressingModes.Implicit;
            //clock[0x18] = 2;
            //instruction(0x18] = INS_CLC;
            addressmode[0x18] = AddressingModes.Implicit;
            //clock[0x19] = 4;
            //instruction(0x19] = INS_ORA;

            addressmode[0x19] = AddressingModes.AbsoluteY;
            //clock[0x1a] = 2;
            //instruction(0x1a] = INS_INA;
            addressmode[0x1a] = AddressingModes.Implicit;
            //clock[0x1b] = 2;
            //instruction(0x1b] = INS_NOP;
            addressmode[0x1b] = AddressingModes.Implicit;
            //clock[0x1c] = 4;
            //instruction(0x1c] = INS_NOP;
            addressmode[0x1c] = AddressingModes.Absolute;
            //clock[0x1d] = 4;
            //instruction(0x1d] = INS_ORA;
            addressmode[0x1d] = AddressingModes.AbsoluteX;
            //clock[0x1e] = 7;
            //instruction(0x1e] = INS_ASL;
            addressmode[0x1e] = AddressingModes.AbsoluteX;
            //clock[0x1f] = 2;
            //instruction(0x1f] = INS_NOP;
            addressmode[0x1f] = AddressingModes.Implicit;
            //clock[0x20] = 6;
            //instruction(0x20] = INS_JSR;
            addressmode[0x20] = AddressingModes.Absolute;
            //clock[0x21] = 6;
            //instruction(0x21] = INS_AND;

            addressmode[0x21] = AddressingModes.IndexedIndirect;
            //clock[0x22] = 2;
            //instruction(0x22] = INS_NOP;
            addressmode[0x22] = AddressingModes.Implicit;
            //clock[0x23] = 2;
            //instruction(0x23] = INS_NOP;
            addressmode[0x23] = AddressingModes.Implicit;
            //clock[0x24] = 3;
            //instruction(0x24] = INS_BIT;
            addressmode[0x24] = AddressingModes.ZeroPage;
            //clock[0x25] = 3;
            //instruction(0x25] = INS_AND;
            addressmode[0x25] = AddressingModes.ZeroPage;
            //clock[0x26] = 5;
            //instruction(0x26] = INS_ROL;
            addressmode[0x26] = AddressingModes.ZeroPage;
            //clock[0x27] = 2;
            //instruction(0x27] = INS_NOP;
            addressmode[0x27] = AddressingModes.Implicit;
            //clock[0x28] = 4;
            //instruction(0x28] = INS_PLP;
            addressmode[0x28] = AddressingModes.Implicit;
            //clock[0x29] = 3;
            //instruction(0x29] = INS_AND;

            addressmode[0x29] = AddressingModes.Immediate;
            //clock[0x2a] = 2;
            //instruction(0x2a] = INS_ROLA;
            addressmode[0x2a] = AddressingModes.Accumulator;
            //clock[0x2b] = 2;
            //instruction(0x2b] = INS_NOP;
            addressmode[0x2b] = AddressingModes.Implicit;
            //clock[0x2c] = 4;
            //instruction(0x2c] = INS_BIT;
            addressmode[0x2c] = AddressingModes.Absolute;
            //clock[0x2d] = 4;
            //instruction(0x2d] = INS_AND;
            addressmode[0x2d] = AddressingModes.Absolute;
            //clock[0x2e] = 6;
            //instruction(0x2e] = INS_ROL;
            addressmode[0x2e] = AddressingModes.Absolute;
            //clock[0x2f] = 2;
            //instruction(0x2f] = INS_NOP;
            addressmode[0x2f] = AddressingModes.Implicit;
            //clock[0x30] = 2;
            //instruction(0x30] = INS_BMI;
            addressmode[0x30] = AddressingModes.Relative;
            //clock[0x31] = 5;
            //instruction(0x31] = INS_AND;
            addressmode[0x31] = AddressingModes.IndirectIndexed;
            //clock[0x32] = 3;
            //instruction(0x32] = INS_AND;
            addressmode[0x32] = AddressingModes.IndirectZeroPage;
            //clock[0x33] = 2;
            //instruction(0x33] = INS_NOP;
            addressmode[0x33] = AddressingModes.Implicit;
            //clock[0x34] = 4;
            //instruction(0x34] = INS_BIT;
            addressmode[0x34] = AddressingModes.ZeroPageX;
            //clock[0x35] = 4;
            //instruction(0x35] = INS_AND;
            addressmode[0x35] = AddressingModes.ZeroPageX;
            //clock[0x36] = 6;
            //instruction(0x36] = INS_ROL;
            addressmode[0x36] = AddressingModes.ZeroPageX;
            //clock[0x37] = 2;
            //instruction(0x37] = INS_NOP;
            addressmode[0x37] = AddressingModes.Implicit;
            //clock[0x38] = 2;
            //instruction(0x38] = INS_SEC;
            addressmode[0x38] = AddressingModes.Implicit;
            //clock[0x39] = 4;
            //instruction(0x39] = INS_AND;
            addressmode[0x39] = AddressingModes.AbsoluteY;
            //clock[0x3a] = 2;
            //instruction(0x3a] = INS_DEA;
            addressmode[0x3a] = AddressingModes.Implicit;
            //clock[0x3b] = 2;
            //instruction(0x3b] = INS_NOP;
            addressmode[0x3b] = AddressingModes.Implicit;
            //clock[0x3c] = 4;
            //instruction(0x3c] = INS_BIT;
            addressmode[0x3c] = AddressingModes.AbsoluteX;
            //clock[0x3d] = 4;
            //instruction(0x3d] = INS_AND;
            addressmode[0x3d] = AddressingModes.AbsoluteX;
            //clock[0x3e] = 7;
            //instruction(0x3e] = INS_ROL;
            addressmode[0x3e] = AddressingModes.AbsoluteX;
            //clock[0x3f] = 2;
            //instruction(0x3f] = INS_NOP;
            addressmode[0x3f] = AddressingModes.Implicit;
            //clock[0x40] = 6;
            //instruction(0x40] = INS_RTI;
            addressmode[0x40] = AddressingModes.Implicit;
            //clock[0x41] = 6;
            //instruction(0x41] = INS_EOR;
            addressmode[0x41] = AddressingModes.IndexedIndirect;
            //clock[0x42] = 2;
            //instruction(0x42] = INS_NOP;
            addressmode[0x42] = AddressingModes.Implicit;
            //clock[0x43] = 2;
            //instruction(0x43] = INS_NOP;
            addressmode[0x43] = AddressingModes.Implicit;
            //clock[0x44] = 2;
            //instruction(0x44] = INS_NOP;
            addressmode[0x44] = AddressingModes.Implicit;
            //clock[0x45] = 3;
            //instruction(0x45] = INS_EOR;
            addressmode[0x45] = AddressingModes.ZeroPage;
            //clock[0x46] = 5;
            //instruction(0x46] = INS_LSR;
            addressmode[0x46] = AddressingModes.ZeroPage;
            //clock[0x47] = 2;
            //instruction(0x47] = INS_NOP;
            addressmode[0x47] = AddressingModes.Implicit;
            //clock[0x48] = 3;
            //instruction(0x48] = INS_PHA;
            addressmode[0x48] = AddressingModes.Implicit;
            //clock[0x49] = 3;
            //instruction(0x49] = INS_EOR;
            addressmode[0x49] = AddressingModes.Immediate;
            //clock[0x4a] = 2;
            //instruction(0x4a] = INS_LSRA;
            addressmode[0x4a] = AddressingModes.Accumulator;
            //clock[0x4b] = 2;
            //instruction(0x4b] = INS_NOP;
            addressmode[0x4b] = AddressingModes.Implicit;
            //clock[0x4c] = 3;
            //instruction(0x4c] = INS_JMP;
            addressmode[0x4c] = AddressingModes.Absolute;
            //clock[0x4d] = 4;
            //instruction(0x4d] = INS_EOR;
            addressmode[0x4d] = AddressingModes.Absolute;
            //clock[0x4e] = 6;
            //instruction(0x4e] = INS_LSR;
            addressmode[0x4e] = AddressingModes.Absolute;
            //clock[0x4f] = 2;
            //instruction(0x4f] = INS_NOP;
            addressmode[0x4f] = AddressingModes.Implicit;
            //clock[0x50] = 2;
            //instruction(0x50] = INS_BVC;
            addressmode[0x50] = AddressingModes.Relative;
            //clock[0x51] = 5;
            //instruction(0x51] = INS_EOR;
            addressmode[0x51] = AddressingModes.IndirectIndexed;
            //clock[0x52] = 3;
            //instruction(0x52] = INS_EOR;
            addressmode[0x52] = AddressingModes.IndirectZeroPage;
            //clock[0x53] = 2;
            //instruction(0x53] = INS_NOP;
            addressmode[0x53] = AddressingModes.Implicit;
            //clock[0x54] = 2;
            //instruction(0x54] = INS_NOP;
            addressmode[0x54] = AddressingModes.Implicit;
            //clock[0x55] = 4;
            //instruction(0x55] = INS_EOR;
            addressmode[0x55] = AddressingModes.ZeroPageX;
            //clock[0x56] = 6;
            //instruction(0x56] = INS_LSR;
            addressmode[0x56] = AddressingModes.ZeroPageX;
            //clock[0x57] = 2;
            //instruction(0x57] = INS_NOP;
            addressmode[0x57] = AddressingModes.Implicit;
            //clock[0x58] = 2;
            //instruction(0x58] = INS_CLI;
            addressmode[0x58] = AddressingModes.Implicit;
            //clock[0x59] = 4;
            //instruction(0x59] = INS_EOR;
            addressmode[0x59] = AddressingModes.AbsoluteY;
            //clock[0x5a] = 3;
            //instruction(0x5a] = INS_PHY;
            addressmode[0x5a] = AddressingModes.Implicit;
            //clock[0x5b] = 2;
            //instruction(0x5b] = INS_NOP;
            addressmode[0x5b] = AddressingModes.Implicit;
            //clock[0x5c] = 2;
            //instruction(0x5c] = INS_NOP;
            addressmode[0x5c] = AddressingModes.Implicit;
            //clock[0x5d] = 4;
            //instruction(0x5d] = INS_EOR;
            addressmode[0x5d] = AddressingModes.AbsoluteX;
            //clock[0x5e] = 7;
            //instruction(0x5e] = INS_LSR;
            addressmode[0x5e] = AddressingModes.AbsoluteX;
            //clock[0x5f] = 2;
            //instruction(0x5f] = INS_NOP;
            addressmode[0x5f] = AddressingModes.Implicit;
            //clock[0x60] = 6;
            //instruction(0x60] = INS_RTS;
            addressmode[0x60] = AddressingModes.Implicit;
            //clock[0x61] = 6;
            //instruction(0x61] = INS_ADC;
            addressmode[0x61] = AddressingModes.IndexedIndirect;
            //clock[0x62] = 2;
            //instruction(0x62] = INS_NOP;
            addressmode[0x62] = AddressingModes.Implicit;
            //clock[0x63] = 2;
            //instruction(0x63] = INS_NOP;
            addressmode[0x63] = AddressingModes.Implicit;
            //clock[0x64] = 3;
            //instruction(0x64] = INS_NOP;
            addressmode[0x64] = AddressingModes.ZeroPage;
            //clock[0x65] = 3;
            //instruction(0x65] = INS_ADC;
            addressmode[0x65] = AddressingModes.ZeroPage;
            //clock[0x66] = 5;
            //instruction(0x66] = INS_ROR;
            addressmode[0x66] = AddressingModes.ZeroPage;
            //clock[0x67] = 2;
            //instruction(0x67] = INS_NOP;
            addressmode[0x67] = AddressingModes.Implicit;
            //clock[0x68] = 4;
            //instruction(0x68] = INS_PLA;
            addressmode[0x68] = AddressingModes.Implicit;
            //clock[0x69] = 3;
            //instruction(0x69] = INS_ADC;
            addressmode[0x69] = AddressingModes.Immediate;
            //clock[0x6a] = 2;
            //instruction(0x6a] = INS_RORA;
            addressmode[0x6a] = AddressingModes.Accumulator;
            //clock[0x6b] = 2;
            //instruction(0x6b] = INS_NOP;
            addressmode[0x6b] = AddressingModes.Implicit;
            //clock[0x6c] = 5;
            //instruction(0x6c] = INS_JMP;

            addressmode[0x6c] = AddressingModes.Indirect;
            //clock[0x6d] = 4;
            //instruction(0x6d] = INS_ADC;
            addressmode[0x6d] = AddressingModes.Absolute;
            //clock[0x6e] = 6;
            //instruction(0x6e] = INS_ROR;
            addressmode[0x6e] = AddressingModes.Absolute;
            //clock[0x6f] = 2;
            //instruction(0x6f] = INS_NOP;
            addressmode[0x6f] = AddressingModes.Implicit;
            //clock[0x70] = 2;
            //instruction(0x70] = INS_BVS;
            addressmode[0x70] = AddressingModes.Relative;
            //clock[0x71] = 5;
            //instruction(0x71] = INS_ADC;
            addressmode[0x71] = AddressingModes.IndirectIndexed;
            //clock[0x72] = 3;
            //instruction(0x72] = INS_ADC;
            addressmode[0x72] = AddressingModes.IndirectZeroPage;
            //clock[0x73] = 2;
            //instruction(0x73] = INS_NOP;
            addressmode[0x73] = AddressingModes.Implicit;
            //clock[0x74] = 4;
            //instruction(0x74] = INS_NOP;
            addressmode[0x74] = AddressingModes.ZeroPageX;
            //clock[0x75] = 4;
            //instruction(0x75] = INS_ADC;
            addressmode[0x75] = AddressingModes.ZeroPageX;
            //clock[0x76] = 6;
            //instruction(0x76] = INS_ROR;
            addressmode[0x76] = AddressingModes.ZeroPageX;
            //clock[0x77] = 2;
            //instruction(0x77] = INS_NOP;
            addressmode[0x77] = AddressingModes.Implicit;
            //clock[0x78] = 2;
            //instruction(0x78] = INS_SEI;
            addressmode[0x78] = AddressingModes.Implicit;
            //clock[0x79] = 4;
            //instruction(0x79] = INS_ADC;
            addressmode[0x79] = AddressingModes.AbsoluteY;
            //clock[0x7a] = 4;
            //instruction(0x7a] = INS_PLY;
            addressmode[0x7a] = AddressingModes.Implicit;
            //clock[0x7b] = 2;
            //instruction(0x7b] = INS_NOP;
            addressmode[0x7b] = AddressingModes.Implicit;
            //clock[0x7c] = 6;
            //instruction(0x7c] = INS_JMP;

            addressmode[0x7c] = AddressingModes.IndirectAbsoluteX;
            //clock[0x7d] = 4;
            //instruction(0x7d] = INS_ADC;
            addressmode[0x7d] = AddressingModes.AbsoluteX;
            //clock[0x7e] = 7;
            //instruction(0x7e] = INS_ROR;
            addressmode[0x7e] = AddressingModes.AbsoluteX;
            //clock[0x7f] = 2;
            //instruction(0x7f] = INS_NOP;
            addressmode[0x7f] = AddressingModes.Implicit;
            //clock[0x80] = 2;
            //instruction(0x80] = INS_BRA;
            addressmode[0x80] = AddressingModes.Relative;
            //clock[0x81] = 6;
            //instruction(0x81] = INS_STA;
            addressmode[0x81] = AddressingModes.IndexedIndirect;
            //clock[0x82] = 2;
            //instruction(0x82] = INS_NOP;
            addressmode[0x82] = AddressingModes.Implicit;
            //clock[0x83] = 2;
            //instruction(0x83] = INS_NOP;
            addressmode[0x83] = AddressingModes.Implicit;
            //clock[0x84] = 2;
            //instruction(0x84] = INS_STY;
            addressmode[0x84] = AddressingModes.ZeroPage;
            //clock[0x85] = 2;
            //instruction(0x85] = INS_STA;
            addressmode[0x85] = AddressingModes.ZeroPage;
            //clock[0x86] = 2;
            //instruction(0x86] = INS_STX;
            addressmode[0x86] = AddressingModes.ZeroPage;
            //clock[0x87] = 2;
            //instruction(0x87] = INS_NOP;
            addressmode[0x87] = AddressingModes.Implicit;
            //clock[0x88] = 2;
            //instruction(0x88] = INS_DEY;
            addressmode[0x88] = AddressingModes.Implicit;
            //clock[0x89] = 2;
            //instruction(0x89] = INS_BIT;
            addressmode[0x89] = AddressingModes.Immediate;
            //clock[0x8a] = 2;
            //instruction(0x8a] = INS_TXA;
            addressmode[0x8a] = AddressingModes.Implicit;
            //clock[0x8b] = 2;
            //instruction(0x8b] = INS_NOP;
            addressmode[0x8b] = AddressingModes.Implicit;
            //clock[0x8c] = 4;
            //instruction(0x8c] = INS_STY;
            addressmode[0x8c] = AddressingModes.Absolute;
            //clock[0x8d] = 4;
            //instruction(0x8d] = INS_STA;
            addressmode[0x8d] = AddressingModes.Absolute;
            //clock[0x8e] = 4;
            //instruction(0x8e] = INS_STX;
            addressmode[0x8e] = AddressingModes.Absolute;
            //clock[0x8f] = 2;
            //instruction(0x8f] = INS_NOP;
            addressmode[0x8f] = AddressingModes.Implicit;
            //clock[0x90] = 2;
            //instruction(0x90] = INS_BCC;
            addressmode[0x90] = AddressingModes.Relative;
            //clock[0x91] = 6;
            //instruction(0x91] = INS_STA;
            addressmode[0x91] = AddressingModes.IndirectIndexed;
            //clock[0x92] = 3;
            //instruction(0x92] = INS_STA;
            addressmode[0x92] = AddressingModes.IndirectZeroPage;
            //clock[0x93] = 2;
            //instruction(0x93] = INS_NOP;
            addressmode[0x93] = AddressingModes.Implicit;
            //clock[0x94] = 4;
            //instruction(0x94] = INS_STY;
            addressmode[0x94] = AddressingModes.ZeroPageX;
            //clock[0x95] = 4;
            //instruction(0x95] = INS_STA;
            addressmode[0x95] = AddressingModes.ZeroPageX;
            //clock[0x96] = 4;
            //instruction(0x96] = INS_STX;
            addressmode[0x96] = AddressingModes.ZeroPageY;
            //clock[0x97] = 2;
            //instruction(0x97] = INS_NOP;
            addressmode[0x97] = AddressingModes.Implicit;
            //clock[0x98] = 2;
            //instruction(0x98] = INS_TYA;
            addressmode[0x98] = AddressingModes.Implicit;
            //clock[0x99] = 5;
            //instruction(0x99] = INS_STA;
            addressmode[0x99] = AddressingModes.AbsoluteY;
            //clock[0x9a] = 2;
            //instruction(0x9a] = INS_TXS;
            addressmode[0x9a] = AddressingModes.Implicit;
            //clock[0x9b] = 2;
            //instruction(0x9b] = INS_NOP;
            addressmode[0x9b] = AddressingModes.Implicit;
            //clock[0x9c] = 4;
            //instruction(0x9c] = INS_NOP;
            addressmode[0x9c] = AddressingModes.Absolute;
            //clock[0x9d] = 5;
            //instruction(0x9d] = INS_STA;
            addressmode[0x9d] = AddressingModes.AbsoluteX;
            //clock[0x9e] = 5;
            //instruction(0x9e] = INS_NOP;
            addressmode[0x9e] = AddressingModes.AbsoluteX;
            //clock[0x9f] = 2;
            //instruction(0x9f] = INS_NOP;
            addressmode[0x9f] = AddressingModes.Implicit;
            //clock[0xa0] = 3;
            //instruction(0xa0] = INS_LDY;
            addressmode[0xa0] = AddressingModes.Immediate;
            //clock[0xa1] = 6;
            //instruction(0xa1] = INS_LDA;
            addressmode[0xa1] = AddressingModes.IndexedIndirect;
            //clock[0xa2] = 3;
            //instruction(0xa2] = INS_LDX;
            addressmode[0xa2] = AddressingModes.Immediate;
            //clock[0xa3] = 2;
            //instruction(0xa3] = INS_NOP;
            addressmode[0xa3] = AddressingModes.Implicit;
            //clock[0xa4] = 3;
            //instruction(0xa4] = INS_LDY;
            addressmode[0xa4] = AddressingModes.ZeroPage;
            //clock[0xa5] = 3;
            //instruction(0xa5] = INS_LDA;
            addressmode[0xa5] = AddressingModes.ZeroPage;
            //clock[0xa6] = 3;
            //instruction(0xa6] = INS_LDX;
            addressmode[0xa6] = AddressingModes.ZeroPage;
            //clock[0xa7] = 2;
            //instruction(0xa7] = INS_NOP;
            addressmode[0xa7] = AddressingModes.Implicit;
            //clock[0xa8] = 2;
            //instruction(0xa8] = INS_TAY;
            addressmode[0xa8] = AddressingModes.Implicit;
            //clock[0xa9] = 3;
            //instruction(0xa9] = INS_LDA;
            addressmode[0xa9] = AddressingModes.Immediate;
            //clock[0xaa] = 2;
            //instruction(0xaa] = INS_TAX;
            addressmode[0xaa] = AddressingModes.Implicit;
            //clock[0xab] = 2;
            //instruction(0xab] = INS_NOP;
            addressmode[0xab] = AddressingModes.Implicit;
            //clock[0xac] = 4;
            //instruction(0xac] = INS_LDY;
            addressmode[0xac] = AddressingModes.Absolute;
            //clock[0xad] = 4;
            //instruction(0xad] = INS_LDA;
            addressmode[0xad] = AddressingModes.Absolute;
            //clock[0xae] = 4;
            //instruction(0xae] = INS_LDX;
            addressmode[0xae] = AddressingModes.Absolute;
            //clock[0xaf] = 2;
            //instruction(0xaf] = INS_NOP;
            addressmode[0xaf] = AddressingModes.Implicit;
            //clock[0xb0] = 2;
            //instruction(0xb0] = INS_BCS;
            addressmode[0xb0] = AddressingModes.Relative;
            //clock[0xb1] = 5;
            //instruction(0xb1] = INS_LDA;
            addressmode[0xb1] = AddressingModes.IndirectIndexed;
            //clock[0xb2] = 3;
            //instruction(0xb2] = INS_LDA;
            addressmode[0xb2] = AddressingModes.IndirectZeroPage;
            //clock[0xb3] = 2;
            //instruction(0xb3] = INS_NOP;
            addressmode[0xb3] = AddressingModes.Implicit;
            //clock[0xb4] = 4;
            //instruction(0xb4] = INS_LDY;
            addressmode[0xb4] = AddressingModes.ZeroPageX;
            //clock[0xb5] = 4;
            //instruction(0xb5] = INS_LDA;
            addressmode[0xb5] = AddressingModes.ZeroPageX;
            //clock[0xb6] = 4;
            //instruction(0xb6] = INS_LDX;

            addressmode[0xb6] = AddressingModes.ZeroPageY;
            //clock[0xb7] = 2;
            //instruction(0xb7] = INS_NOP;
            addressmode[0xb7] = AddressingModes.Implicit;
            //clock[0xb8] = 2;
            //instruction(0xb8] = INS_CLV;
            addressmode[0xb8] = AddressingModes.Implicit;
            //clock[0xb9] = 4;
            //instruction(0xb9] = INS_LDA;
            addressmode[0xb9] = AddressingModes.AbsoluteY;
            //clock[0xba] = 2;
            //instruction(0xba] = INS_TSX;
            addressmode[0xba] = AddressingModes.Implicit;
            //clock[0xbb] = 2;
            //instruction(0xbb] = INS_NOP;
            addressmode[0xbb] = AddressingModes.Implicit;
            //clock[0xbc] = 4;
            //instruction(0xbc] = INS_LDY;
            addressmode[0xbc] = AddressingModes.AbsoluteX;
            //clock[0xbd] = 4;
            //instruction(0xbd] = INS_LDA;
            addressmode[0xbd] = AddressingModes.AbsoluteX;
            //clock[0xbe] = 4;
            //instruction(0xbe] = INS_LDX;
            addressmode[0xbe] = AddressingModes.AbsoluteY;
            //clock[0xbf] = 2;
            //instruction(0xbf] = INS_NOP;
            addressmode[0xbf] = AddressingModes.Implicit;
            //clock[0xc0] = 3;
            //instruction(0xc0] = INS_CPY;
            addressmode[0xc0] = AddressingModes.Immediate;
            //clock[0xc1] = 6;
            //instruction(0xc1] = INS_CMP;
            addressmode[0xc1] = AddressingModes.IndexedIndirect;
            //clock[0xc2] = 2;
            //instruction(0xc2] = INS_NOP;
            addressmode[0xc2] = AddressingModes.Implicit;
            //clock[0xc3] = 2;
            //instruction(0xc3] = INS_NOP;
            addressmode[0xc3] = AddressingModes.Implicit;
            //clock[0xc4] = 3;
            //instruction(0xc4] = INS_CPY;
            addressmode[0xc4] = AddressingModes.ZeroPage;
            //clock[0xc5] = 3;
            //instruction(0xc5] = INS_CMP;
            addressmode[0xc5] = AddressingModes.ZeroPage;
            //clock[0xc6] = 5;
            //instruction(0xc6] = INS_DEC;
            addressmode[0xc6] = AddressingModes.ZeroPage;
            //clock[0xc7] = 2;
            //instruction(0xc7] = INS_NOP;
            addressmode[0xc7] = AddressingModes.Implicit;
            //clock[0xc8] = 2;
            //instruction(0xc8] = INS_INY;
            addressmode[0xc8] = AddressingModes.Implicit;
            //clock[0xc9] = 3;
            //instruction(0xc9] = INS_CMP;
            addressmode[0xc9] = AddressingModes.Immediate;
            //clock[0xca] = 2;
            //instruction(0xca] = INS_DEX;
            addressmode[0xca] = AddressingModes.Implicit;
            //clock[0xcb] = 2;
            //instruction(0xcb] = INS_NOP;
            addressmode[0xcb] = AddressingModes.Implicit;
            //clock[0xcc] = 4;
            //instruction(0xcc] = INS_CPY;
            addressmode[0xcc] = AddressingModes.Absolute;
            //clock[0xcd] = 4;
            //instruction(0xcd] = INS_CMP;
            addressmode[0xcd] = AddressingModes.Absolute;
            //clock[0xce] = 6;
            //instruction(0xce] = INS_DEC;
            addressmode[0xce] = AddressingModes.Absolute;
            //clock[0xcf] = 2;
            //instruction(0xcf] = INS_NOP;
            addressmode[0xcf] = AddressingModes.Implicit;
            //clock[0xd0] = 2;
            //instruction(0xd0] = INS_BNE;
            addressmode[0xd0] = AddressingModes.Relative;
            //clock[0xd1] = 5;
            //instruction(0xd1] = INS_CMP;
            addressmode[0xd1] = AddressingModes.IndirectIndexed;
            //clock[0xd2] = 3;
            //instruction(0xd2] = INS_CMP;
            addressmode[0xd2] = AddressingModes.IndirectZeroPage;
            //clock[0xd3] = 2;
            //instruction(0xd3] = INS_NOP;
            addressmode[0xd3] = AddressingModes.Implicit;
            //clock[0xd4] = 2;
            //instruction(0xd4] = INS_NOP;
            addressmode[0xd4] = AddressingModes.Implicit;
            //clock[0xd5] = 4;
            //instruction(0xd5] = INS_CMP;
            addressmode[0xd5] = AddressingModes.ZeroPageX;
            //clock[0xd6] = 6;
            //instruction(0xd6] = INS_DEC;
            addressmode[0xd6] = AddressingModes.ZeroPageX;
            //clock[0xd7] = 2;
            //instruction(0xd7] = INS_NOP;
            addressmode[0xd7] = AddressingModes.Implicit;
            //clock[0xd8] = 2;
            //instruction(0xd8] = INS_CLD;
            addressmode[0xd8] = AddressingModes.Implicit;
            //clock[0xd9] = 4;
            //instruction(0xd9] = INS_CMP;
            addressmode[0xd9] = AddressingModes.AbsoluteY;
            //clock[0xda] = 3;
            //instruction(0xda] = INS_PHX;
            addressmode[0xda] = AddressingModes.Implicit;
            //clock[0xdb] = 2;
            //instruction(0xdb] = INS_NOP;
            addressmode[0xdb] = AddressingModes.Implicit;
            //clock[0xdc] = 2;
            //instruction(0xdc] = INS_NOP;
            addressmode[0xdc] = AddressingModes.Implicit;
            //clock[0xdd] = 4;
            //instruction(0xdd] = INS_CMP;
            addressmode[0xdd] = AddressingModes.AbsoluteX;
            //clock[0xde] = 7;
            //instruction(0xde] = INS_DEC;
            addressmode[0xde] = AddressingModes.AbsoluteX;
            //clock[0xdf] = 2;
            //instruction(0xdf] = INS_NOP;
            addressmode[0xdf] = AddressingModes.Implicit;
            //clock[0xe0] = 3;
            //instruction(0xe0] = INS_CPX;
            addressmode[0xe0] = AddressingModes.Immediate;
            //clock[0xe1] = 6;
            //instruction(0xe1] = INS_SBC;
            addressmode[0xe1] = AddressingModes.IndexedIndirect;
            //clock[0xe2] = 2;
            //instruction(0xe2] = INS_NOP;
            addressmode[0xe2] = AddressingModes.Implicit;
            //clock[0xe3] = 2;
            //instruction(0xe3] = INS_NOP;
            addressmode[0xe3] = AddressingModes.Implicit;
            //clock[0xe4] = 3;
            //instruction(0xe4] = INS_CPX;
            addressmode[0xe4] = AddressingModes.ZeroPage;
            //clock[0xe5] = 3;
            //instruction(0xe5] = INS_SBC;
            addressmode[0xe5] = AddressingModes.ZeroPage;
            //clock[0xe6] = 5;
            //instruction(0xe6] = INS_INC;
            addressmode[0xe6] = AddressingModes.ZeroPage;
            //clock[0xe7] = 2;
            //instruction(0xe7] = INS_NOP;
            addressmode[0xe7] = AddressingModes.Implicit;
            //clock[0xe8] = 2;
            //instruction(0xe8] = INS_INX;
            addressmode[0xe8] = AddressingModes.Implicit;
            //clock[0xe9] = 3;
            //instruction(0xe9] = INS_SBC;
            addressmode[0xe9] = AddressingModes.Immediate;
            //clock[0xea] = 2;
            //instruction(0xea] = INS_NOP;
            addressmode[0xea] = AddressingModes.Implicit;
            //clock[0xeb] = 2;
            //instruction(0xeb] = INS_NOP;
            addressmode[0xeb] = AddressingModes.Implicit;
            //clock[0xec] = 4;
            //instruction(0xec] = INS_CPX;
            addressmode[0xec] = AddressingModes.Absolute;
            //clock[0xed] = 4;
            //instruction(0xed] = INS_SBC;
            addressmode[0xed] = AddressingModes.Absolute;
            //clock[0xee] = 6;
            //instruction(0xee] = INS_INC;
            addressmode[0xee] = AddressingModes.Absolute;
            //clock[0xef] = 2;
            //instruction(0xef] = INS_NOP;
            addressmode[0xef] = AddressingModes.Implicit;
            //clock[0xf0] = 2;
            //instruction(0xf0] = INS_BEQ;
            addressmode[0xf0] = AddressingModes.Relative;
            //clock[0xf1] = 5;
            //instruction(0xf1] = INS_SBC;
            addressmode[0xf1] = AddressingModes.IndirectIndexed;
            //clock[0xf2] = 3;
            //instruction(0xf2] = INS_SBC;
            addressmode[0xf2] = AddressingModes.IndirectZeroPage;
            //clock[0xf3] = 2;
            //instruction(0xf3] = INS_NOP;
            addressmode[0xf3] = AddressingModes.Implicit;
            //clock[0xf4] = 2;
            //instruction(0xf4] = INS_NOP;
            addressmode[0xf4] = AddressingModes.Implicit;
            //clock[0xf5] = 4;
            //instruction(0xf5] = INS_SBC;
            addressmode[0xf5] = AddressingModes.ZeroPageX;
            //clock[0xf6] = 6;
            //instruction(0xf6] = INS_INC;
            addressmode[0xf6] = AddressingModes.ZeroPageX;
            //clock[0xf7] = 2;
            //instruction(0xf7] = INS_NOP;
            addressmode[0xf7] = AddressingModes.Implicit;
            //clock[0xf8] = 2;
            //instruction(0xf8] = INS_SED;
            addressmode[0xf8] = AddressingModes.Implicit;
            //clock[0xf9] = 4;
            //instruction(0xf9] = INS_SBC;
            addressmode[0xf9] = AddressingModes.AbsoluteY;
            //clock[0xfa] = 4;
            //instruction(0xfa] = INS_PLX;
            addressmode[0xfa] = AddressingModes.Implicit;
            //clock[0xfb] = 2;
            //instruction(0xfb] = INS_NOP;
            addressmode[0xfb] = AddressingModes.Implicit;
            //clock[0xfc] = 2;
            //instruction(0xfc] = INS_NOP;
            addressmode[0xfc] = AddressingModes.Implicit;
            //clock[0xfd] = 4;
            //instruction(0xfd] = INS_SBC;
            addressmode[0xfd] = AddressingModes.AbsoluteX;
            //clock[0xfe] = 7;
            //instruction(0xfe] = INS_INC;
            addressmode[0xfe] = AddressingModes.AbsoluteX;
            //clock[0xff] = 2;
            //instruction(0xff] = INS_NOP;
            addressmode[0xff] = AddressingModes.Implicit;
        }

        public struct OpcodeInfo
        {
            public int OpCode;
            public string Text;
            public AddressingModes addressMode;
            public int Length;
        }
        static List<OpcodeInfo> opcodes = new List<OpcodeInfo>();

        public static List<OpcodeInfo> OpCodes
        {
            get { return DisassemblyExtensions.opcodes; }
            set { DisassemblyExtensions.opcodes = value; }
        }

        public static string GetResourceName(this string res)
        {

            foreach (string s in System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                if (s.EndsWith(res))
                {
                    return s;
                }
            }
            return null;
        }

        private static XDocument doc;
        private static string[] mnemnonics = new string[0x100];

        public static XDocument OpCodeXML
        {
            get
            {
                if (doc == null)
                {
                    SetupOpcodes();
                }
                return doc;
            }
            set { doc = value; }
        }

        public static void SetupOpcodes()
        {

            setupticks();

            doc = XDocument.Load(
                XmlReader.Create(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream("6502OpCodes.xml".GetResourceName())
                    )
                );

            var codes = (from op in doc.Elements("Instructions").Elements("Instruction").Elements("Opcode")
                         orderby op.Attribute("Hex").Value
                         select
                         new
                         {
                             Name = op.Parent.Attribute("Name").Value,
                             Hex = op.Attribute("Hex").Value
                         }).ToList();

            // initialize all mnemonics to UNK, then overwrite with real names
            for (int i = 0; i < 0x100; ++i)
            {
                mnemnonics[i] = "UNK";
            }


            foreach (var kode in codes)
            {
                int code = 0;
                if (int.TryParse(kode.Hex.Trim('$'), System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture, out code))
                {
                    opcodes.Add(new OpcodeInfo() { addressMode = addressmode[code], OpCode = code, Text = kode.Name  });
                    mnemnonics[code] = kode.Name;
                }
            }
        }

        public static string GetMnemnonic(this NES.CPU.Fastendo.CPU2A03.Instruction inst)
        {

            return GetMnemnonic(inst.OpCode);
        }

        public static string GetMnemnonic(int opCode)
        {
            if (OpCodeXML == null) SetupOpcodes();
            return mnemnonics[opCode];
        }

        public static string Disassemble(this NES.CPU.Fastendo.CPU2A03.Instruction inst)
        {
            string parms = string.Empty;
            parms = parms + string.Format("{0,2:x}, ", inst.Parameters0);
            parms = parms + string.Format("{0,2:x}, ", inst.Parameters1);


            string result = string.Empty;
            switch (inst.AddressingMode)
            {
                case AddressingModes.Accumulator:
                    result = string.Format("{0} A", inst.GetMnemnonic());
                    break;
                case AddressingModes.Implicit:
                    result = string.Format("{0}", inst.GetMnemnonic());
                    break;
                case AddressingModes.Immediate:
                    result = string.Format("{0} #${1:x2}", inst.GetMnemnonic(), inst.Parameters0);
                    break;
                case AddressingModes.ZeroPage:
                    result = string.Format("{0} ${1:x2}", inst.GetMnemnonic(), inst.Parameters0);
                    break;
                case AddressingModes.ZeroPageX:
                    result = string.Format("{0} ${1:x},X", inst.GetMnemnonic(), inst.Parameters0);
                    break;
                case AddressingModes.ZeroPageY:
                    result = string.Format("{0} ${1:x},Y", inst.GetMnemnonic(), inst.Parameters0);
                    break;
                case AddressingModes.Relative:
                    if ((inst.Parameters0 & 128) == 128)
                    {
                        result = string.Format("{0} *{1}", inst.GetMnemnonic(), (byte)inst.Parameters0);
                    }
                    else
                    {
                        result = string.Format("{0} *+{1}", inst.GetMnemnonic(), inst.Parameters0);
                    }
                    break;
                case AddressingModes.Absolute:
                    int addr = (inst.Parameters1 * 256 | inst.Parameters0);
                    result = string.Format("{0} ${1:x4}", inst.GetMnemnonic(), addr);
                    break;
                case AddressingModes.AbsoluteX:
                    result = string.Format("{0} ${1:x4},X", inst.GetMnemnonic(), (inst.Parameters1 * 256) | inst.Parameters0);
                    break;
                case AddressingModes.AbsoluteY:
                    result = string.Format("{0} ${1:x4},Y", inst.GetMnemnonic(), (inst.Parameters1 * 256) | inst.Parameters0);
                    break;
                case AddressingModes.Indirect:
                    result = string.Format("{0} (${1:x4})", inst.GetMnemnonic(), (inst.Parameters1 * 256) | inst.Parameters0);
                    break;
                case AddressingModes.IndexedIndirect:
                    result = string.Format("{0} (${1:x2},X)", inst.GetMnemnonic(), inst.Parameters0);
                    break;
                case AddressingModes.IndirectIndexed:
                    result = string.Format("{0} (${1:x2}),Y", inst.GetMnemnonic(), inst.Parameters0);
                    break;

            }
            return result;
        }
    }


}
