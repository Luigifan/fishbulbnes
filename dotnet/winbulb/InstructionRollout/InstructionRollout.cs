using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Fastendo;
using NES.CPU.CPUDebugging;
using NES.CPU.FastendoDebugging;

namespace WPFamicom.InstructionRollout
{
    public class InstructionRollout : List<InstructionRolloutItem>
    {
        public InstructionRollout() { }

        public InstructionRollout(DebuggerCPUState target, IList<CPUBreakpoint> breakpoints, int length)
        {
            CreateRollout(target, breakpoints, length);
        }

        public void CreateRollout(DebuggerCPUState target, IList<CPUBreakpoint> breakpoints, int length)
        {
            //int nextInstruction = target.ProgramCounter;
            //for (int i = 0; i < length; ++i)
            //{

            //    InstructionRolloutItem newInstructionRolloutItem = new InstructionRolloutItem()
            //    {
            //        Address = string.Format("{0:x4}", nextInstruction).ToUpper(),
            //        Instruction = target.PeekInstruction(nextInstruction),
            //        HasBreakpoint = (from br in breakpoints
            //                         where br.Address == nextInstruction
            //                         select true).FirstOrDefault()
            //    };

            //    nextInstruction += newInstructionRolloutItem.Instruction.Length;
                
            //    this.Add(
            //        newInstructionRolloutItem);

            //}
        }

    }
}
