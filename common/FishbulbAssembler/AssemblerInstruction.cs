using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Fastendo;
using System.Text.RegularExpressions;
using NES.CPU.FastendoDebugging;

namespace FishbulbAssembler
{
    public class AssemblerInstruction : NES.CPU.Fastendo.CPU2A03.Instruction
    {
        static string parseOpcode = @"(?<opcode>\w*)\s*(?<openbracket>\()*(?<datatype>[#,\$,%,\s]*)(?<data>[\w]+)*\,*(?<closebracket>\))*\,*(?<index>x|y)*";

        static List<string> validOps = new List<string>();

        public static List<string> ValidOps
        {
            get { return AssemblerInstruction.validOps; }
        }

        static AssemblerInstruction()
        {
            var builder = new StringBuilder()
                .Append(@"(?<opcode>")
                ;
            

            foreach (var p in DisassemblyExtensions.OpCodes)
            {
                if (!validOps.Contains(p.Text))
                {
                    builder.Append(p.Text).Append("|");
                    validOps.Add(p.Text);
                }
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append(@")\s*(?<openbracket>\()*(?<datatype>[#,\$,%,\s]*)(?<data>[\w]+)*\,*(?<closebracket>\))*\,*(?<index>x|y|X|Y)*");

            parseOpcode = builder.ToString();
        }

        bool isDecoded = false;
        public bool IsDecoded { get { return isDecoded; } set { isDecoded = value; } }

        public bool Decode(string res)
        {
            if (res == null)
            {
                return false;
            }

            // create a new instruction

            this.Address = -1;
            // identify addressing mode

            Regex r = new Regex(parseOpcode);
            var match = r.Match(res);
            var opcode = match.Groups["opcode"].Success ? match.Groups["opcode"].Value : null;
            var br1 = match.Groups["openbracket"].Success ? match.Groups["openbracket"].Value : null;
            // if br2 present, addressing mode is IndirectIndexed
            var br2 = match.Groups["closebracket"].Success ? match.Groups["closebracket"].Value : null;

            var index = match.Groups["index"].Success ? match.Groups["index"].Value : null;
            var data = match.Groups["data"].Success ? match.Groups["data"].Value : null;

            // if null, datatype processed as either decimal or as a label, based on what is in data
            var datatype = match.Groups["datatype"].Success ? match.Groups["datatype"].Value : null;

            FindAddressMode(br1, br2, index, data, datatype);

            this.OpCode = GetOpcode(opcode, this.AddressingMode);

            return (this.OpCode > -1) && this.OpCode < 255;
        }

        private void FindAddressMode(string br1, string br2, string index, string data, string datatype)
        {
            bool isImmediate = datatype != null && datatype.StartsWith("#");

            if (isImmediate)
            {
                datatype = datatype.Substring(1, datatype.Length - 1);
                byte[] parameter = DecodeData(datatype, data);
                if (this.isDecoded)
                {
                    this.AddressingMode = AddressingModes.Immediate;
                    this.Parameters0 = parameter[0];
                    this.Length = 2;
                    return;
                }
            }

            // interpret addressing mode
            this.AddressingMode = AddressingModes.Implicit;
            this.Length = 1;

            if (data == "A")
            {
                this.AddressingMode = AddressingModes.Accumulator;
                this.Length = 1;
                this.isDecoded = true;
                return;
            }
            else
            {
                byte[] parameter = DecodeData(datatype, data);

                if (isDecoded)
                {
                    SetAddressMode(br1, br2, index, datatype, isImmediate, parameter);
                }
            }
            return ;
        }

        private void SetAddressMode(string br1, string br2, string index, string datatype, bool isImmediate, byte[] parameter)
        {
            if (br1 != null)
            {
                // if br1 present, addressing is indirect (check for br2 and index for indirectindexed)
                this.AddressingMode = (br2 != null) ? AddressingModes.IndirectIndexed : AddressingModes.IndexedIndirect;

                this.Length = 2;
                this.Parameters0 = parameter[0];

                // if no index, this is bullshit
                if (index == null)
                    this.AddressingMode = AddressingModes.Indirect;
            }
            else
            {
                switch (parameter.Length)
                {
                    case 1:
                        this.Length = 2;
                        // if datatype starts with a #, it is immediate, else, a zeropage instruction
                        if (isImmediate)
                        {
                            this.AddressingMode = AddressingModes.Immediate;
                        }
                        else
                        {
                            if (index == "Y")
                            {
                                this.AddressingMode = AddressingModes.ZeroPageY;
                            }
                            else if (index == "X")
                            {
                                this.AddressingMode = AddressingModes.ZeroPageX;
                            }
                            else
                            {
                                this.AddressingMode = AddressingModes.ZeroPage;
                            }
                        }
                        this.Parameters0 = parameter[0];
                        break;
                    case 2:
                        this.Length = 3;

                        // if datatype starts with a #, it is immediate, else, a zeropage instruction
                        if (datatype != null && datatype.StartsWith("#"))
                        {
                            this.AddressingMode = AddressingModes.Immediate;
                        }
                        else
                        {
                            if (index == "Y")
                            {
                                this.AddressingMode = AddressingModes.AbsoluteY;
                            }
                            else if (index == "X")
                            {
                                this.AddressingMode = AddressingModes.AbsoluteX;
                            }
                            else
                            {
                                this.AddressingMode = AddressingModes.Absolute;
                            }
                        }
                        this.Parameters0 = parameter[0];
                        this.Parameters1 = parameter[1];
                        break;
                }

            }
        }

        static int GetOpcode(string instruction, AddressingModes mode)
        {
            var result = from p in DisassemblyExtensions.OpCodes where p.addressMode == mode && p.Text == instruction select p.OpCode;
            if (result.Count() != 0)
                return result.First();
            return -1;

        }

        // will parse the string in data as a number, returning either a 0-length array (no data), 1 byte, or 2 bytes (low, high)
        byte[] DecodeData(string dataType, string data)
        {
            if (data == null) return new byte[0];

            byte[] outData = new byte[0];
            uint val = 0;
            switch (dataType)
            {
                //hex
                case "$":
                    val = (uint)int.Parse(data, System.Globalization.NumberStyles.HexNumber);
                    this.isDecoded = true;
                    break;
                case "%":
                    val = DecodeBinaryString(data);
                    this.isDecoded = true;
                    break;
                default:
                    this.NeededLabel = data;
                    break;

            }

            if (val <= 255)
            {
                outData = new byte[1];
                outData[0] = (byte)val;
            }
            else
            {
                outData = new byte[2];
                outData[0] = (byte)val;
                outData[1] = (byte)(val >> 8);
            }

            return outData;
        }

        static uint DecodeBinaryString(string data)
        {
            int bit = 0;
            uint result = 0;
            var s = data.Reverse();

            foreach (char c in s)
            {
                switch (c)
                {
                    case '0':
                        break;
                    case '1':
                        result += (uint)1 << bit;
                        break;
                    default:
                        break;
                }
                bit++;
            }
            return result;

        }

        public string NeededLabel { get; set; }

    }
}
