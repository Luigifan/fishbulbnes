using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.CPU.Fastendo
{
    public enum AddressingModes
    {
        Bullshit,
        Implicit,
        Accumulator,
        Immediate,
        ZeroPage,
        ZeroPageX,
        ZeroPageY,
        Relative,
        Absolute,
        AbsoluteX,
        AbsoluteY,
        Indirect,
        IndexedIndirect,
        IndirectIndexed,
        // funky ass undocumented modes
        IndirectZeroPage,
        IndirectAbsoluteX
    }

    public enum CPUStatusBits
    {
        Carry = 0,
        ZeroResult = 1,
        InterruptDisable = 2,
        DecimalMode = 3,
        BreakCommand = 4,
        Expansion = 5,
        Overflow = 6,
        NegativeResult = 7
    }

    public enum CPUStatusMasks
    {
        CarryMask =  0x01,
        ZeroResultMask = 0x02,
        InterruptDisableMask = 0x04,
        DecimalModeMask = 0x08,
        BreakCommandMask = 0x10,
        ExpansionMask = 0x20,
        OverflowMask = 0x40,
        NegativeResultMask = 0x80
    }
}
