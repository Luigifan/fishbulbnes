using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml;
using System.Reflection;

namespace SilverBulb.Web
{
    public static class OpcodeDissassembler
    {

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

        public static string GetHelpText(string opName)
        {
            return (from op in OpCodeInfo.Element("Instructions").Elements("Instruction") where op.Attribute("Name").Value == opName select op.Value).FirstOrDefault<string>();
        }

        public static XDocument OpCodeInfo
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
                    mnemnonics[code] = kode.Name;
                }
            }
        }

        //public static string GetMnemnonic(this NES.CPU.Fastendo.CPU2A03.Instruction inst)
        //{

        //    return GetMnemnonic(inst.OpCode);
        //}

        public static string GetMnemnonic(int opCode)
        {
            if (OpCodeInfo == null) SetupOpcodes();
            return mnemnonics[opCode];
        }

        //public static string Disassemble(this NES.CPU.Fastendo.CPU2A03.Instruction inst)
        //{
        //    string parms = string.Empty;
        //    parms = parms + string.Format("{0,2:x}, ", inst.Parameters0);
        //    parms = parms + string.Format("{0,2:x}, ", inst.Parameters1);


        //    string result = string.Empty;
        //    switch (inst.AddressingMode)
        //    {
        //        case AddressingModes.Accumulator:
        //            result = string.Format("{0} A", inst.GetMnemnonic());
        //            break;
        //        case AddressingModes.Implicit:
        //            result = string.Format("{0}", inst.GetMnemnonic());
        //            break;
        //        case AddressingModes.Immediate:
        //            result = string.Format("{0} #${1:x2}", inst.GetMnemnonic(), inst.Parameters0);
        //            break;
        //        case AddressingModes.ZeroPage:
        //            result = string.Format("{0} ${1:x2}", inst.GetMnemnonic(), inst.Parameters0);
        //            break;
        //        case AddressingModes.ZeroPageX:
        //            result = string.Format("{0} ${1:x},X", inst.GetMnemnonic(), inst.Parameters0);
        //            break;
        //        case AddressingModes.ZeroPageY:
        //            result = string.Format("{0} ${1:x},Y", inst.GetMnemnonic(), inst.Parameters0);
        //            break;
        //        case AddressingModes.Relative:
        //            if ((inst.Parameters0 & 128) == 128)
        //            {
        //                result = string.Format("{0} *{1}", inst.GetMnemnonic(), (byte)inst.Parameters0);
        //            }
        //            else
        //            {
        //                result = string.Format("{0} *+{1}", inst.GetMnemnonic(), inst.Parameters0);
        //            }
        //            break;
        //        case AddressingModes.Absolute:
        //            int addr = (inst.Parameters1 * 256 | inst.Parameters0);
        //            result = string.Format("{0} ${1:x4}", inst.GetMnemnonic(), addr);
        //            break;
        //        case AddressingModes.AbsoluteX:
        //            result = string.Format("{0} ${1:x4},X", inst.GetMnemnonic(), (inst.Parameters1 * 256) | inst.Parameters0);
        //            break;
        //        case AddressingModes.AbsoluteY:
        //            result = string.Format("{0} ${1:x4},Y", inst.GetMnemnonic(), (inst.Parameters1 * 256) | inst.Parameters0);
        //            break;
        //        case AddressingModes.Indirect:
        //            result = string.Format("{0} (${1:x4})", inst.GetMnemnonic(), (inst.Parameters1 * 256) | inst.Parameters0);
        //            break;
        //        case AddressingModes.IndexedIndirect:
        //            result = string.Format("{0} (${1:x2},X)", inst.GetMnemnonic(), inst.Parameters0);
        //            break;
        //        case AddressingModes.IndirectIndexed:
        //            result = string.Format("{0} (${1:x2}),Y", inst.GetMnemnonic(), inst.Parameters0);
        //            break;

        //    }
        //    return result;
        //}
    }
}