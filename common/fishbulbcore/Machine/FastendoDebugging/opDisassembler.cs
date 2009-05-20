using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU;
using NES.CPU.Fastendo;
using System.Xml;
using System.Reflection;

namespace NES.CPU.FastendoDebugging
{
    public static class DisassemblyExtensions
    {
        private static XmlDocument doc;
        private static string[] mnemnonics = new string[0x100];
        
        public static XmlDocument OpCodeInfo
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

        private static void SetupOpcodes()
        {
            doc = new XmlDocument();
            doc.Load(
                XmlReader.Create(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream("NES.CPU.6502OpCodes.xml")
                    )
                );

            for (int i = 0; i < 0x100; ++i)
            {
                string result;
                string opcode = string.Format("${0:x2}", i).ToUpper();
                string format = string.Format("/Instructions/Instruction[Opcode/@Hex='{0}']/@Name", opcode);
                XmlNode n = doc.SelectSingleNode(format);
                if (n != null)
                    result = doc.SelectSingleNode(format).Value;
                else
                    result = "UNK";
                mnemnonics[i] = result;

            }
        }

        public static string GetMnemnonic(this NES.CPU.Fastendo.CPU2A03.Instruction inst)
        {

            return GetMnemnonic(inst.OpCode);
        }

        public static string GetMnemnonic(int opCode)
        {
            if (OpCodeInfo == null) SetupOpcodes();
            return mnemnonics[opCode];
            //string result;
            //string opcode = string.Format("${0:x2}", opCode).ToUpper();
            //string format = string.Format("/Instructions/Instruction[Opcode/@Hex='{0}']/@Name", opcode);
            //XmlNode n = OpCodeInfo.SelectSingleNode(format);
            //if (n != null)
            //    result = OpCodeInfo.SelectSingleNode(format).Value;
            //else
            //    result = "UNK";
            //return result;
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
                    result =  string.Format("{0} A", inst.GetMnemnonic());
                    break;
                case AddressingModes.Implicit:
                    result =  string.Format("{0}", inst.GetMnemnonic());
                    break;
                case AddressingModes.Immediate:
                    result =  string.Format("{0} #${1:x2}", inst.GetMnemnonic(), inst.Parameters0);
                    break;
                case AddressingModes.ZeroPage:
                    result =  string.Format("{0} ${1:x2}", inst.GetMnemnonic(), inst.Parameters0);
                    break;
                case AddressingModes.ZeroPageX:
                    result =  string.Format("{0} ${1:x},X", inst.GetMnemnonic(), inst.Parameters0);
                    break;
                case AddressingModes.ZeroPageY:
                    result =  string.Format("{0} ${1:x},Y", inst.GetMnemnonic(), inst.Parameters0);
                    break;
                case AddressingModes.Relative:
                    if ((inst.Parameters0 & 128) == 128)
                    {
                        result =  string.Format("{0} *{1}", inst.GetMnemnonic(), (byte)inst.Parameters0);
                    }
                    else
                    {
                        result =  string.Format("{0} *+{1}", inst.GetMnemnonic(), inst.Parameters0);
                    }
                    break;
                case AddressingModes.Absolute:
                    int addr = (inst.Parameters1 * 256 | inst.Parameters0 ) ;
                    result =  string.Format("{0} ${1:x4}", inst.GetMnemnonic(), addr);
                    break;
                case AddressingModes.AbsoluteX:
                    result =  string.Format("{0} ${1:x4},X", inst.GetMnemnonic(), (inst.Parameters1 * 256) | inst.Parameters0);
                    break;
                case AddressingModes.AbsoluteY:
                    result =  string.Format("{0} ${1:x4},Y", inst.GetMnemnonic(), (inst.Parameters1 * 256) | inst.Parameters0);
                    break;
                case AddressingModes.Indirect:
                    result =  string.Format("{0} (${1:x4})", inst.GetMnemnonic(), (inst.Parameters1 * 256) | inst.Parameters0);
                    break;
                case AddressingModes.IndexedIndirect:
                    result =  string.Format("{0} (${1:x2},X)", inst.GetMnemnonic(), inst.Parameters0);
                    break;
                case AddressingModes.IndirectIndexed:
                    result =  string.Format("{0} (${1:x2}),Y", inst.GetMnemnonic(), inst.Parameters0);
                    break;

            }
            return  result;
        }
    }
}
