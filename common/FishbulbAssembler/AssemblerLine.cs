using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FishbulbAssembler
{
    public class AssemblerLine
    {

        public AssemblerLine(string s)
        {
            StringBuilder sb = new StringBuilder();
            // select any starting garbage as an error
            //sb.Append(@"(?<error>[\d])*");
            // select any word characters into label
            sb.Append(@"(?<label>\w*?)?\s*");
            // select code (starts with code in list, goes until ;)
            sb.Append(@"(?<code>");
            sb.Append("(").Append(Assembler.ValidOpsPattern()).Append(@"){1}(\s+?|$)*[^\;]*)?");
            sb.Append(@"(?<comments>\;.*)?");

            labelRegexPattern = sb.ToString();
            this.Text = s;
            this.Decode();
        }

        static string labelRegexPattern = @"";


        public string Text { get; set; }
        public int LineNumber { get; set; }
        public string ErrorText { get; set; }

        /// <summary>
        /// the mapped instruction
        /// </summary>
        public AssemblerInstruction Instruction
        {
            get;
            set;
        }
        /// <summary>
        /// optional label for second pass
        /// </summary>
        public string Label { get; set; }
        public string Comments { get; set; }
        public string Code { get; set; }

        /// <summary>
        /// Final address for output
        /// </summary>
        public int Address { get; set; }
        public byte[] Data { get; set; }

        public void Decode()
        {
            // identify line type, break up into semantic chunks
            string line = Text.ToUpper().Trim();
            var textParts = line.Split(';');
            if (textParts.Count() > 1)
            {
                Comments = textParts[1];
                line = textParts[0].Trim();
            }

            var lineParts = line.Split(' ');
            if (lineParts.Count() > 0)
            {
                if (AssemblerInstruction.ValidOps.Contains(lineParts[0].Trim()))
                {
                    Code = line;
                }
                else
                {
                    Label = lineParts[0];
                    Code = line.Substring(lineParts[0].Length);
                }
            }
            else
            {
                Label = lineParts[0];
            }

            //Regex regex = new Regex(labelRegexPattern);
            //var match = regex.Match(line.ToUpper());

            //Label = match.Groups["label"].Success ? match.Groups["label"].Value : null;
            //Code = match.Groups["code"].Success ? match.Groups["code"].Value : null;
            //ErrorText = match.Groups["error"].Success ? string.Format( "Invalid text: {0}", match.Groups["error"].Value) : null;
        }

        public static AssemblerLine CloneAndUpdate(AssemblerLine line)
        {
            AssemblerLine newLine = Clone(line);

            if (newLine.Instruction != null )
            {
                newLine.Data = new byte[newLine.Instruction.Length];
                switch (newLine.Instruction.Length)
                {
                    case 1:
                        newLine.Data[0] = (byte)newLine.Instruction.OpCode;
                        break;
                    case 2:
                        newLine.Data[0] = (byte)newLine.Instruction.OpCode;
                        newLine.Data[1] = (byte)newLine.Instruction.Parameters0;
                        break;
                    case 3:
                        newLine.Data[0] = (byte)newLine.Instruction.OpCode;
                        newLine.Data[1] = (byte)newLine.Instruction.Parameters0;
                        newLine.Data[2] = (byte)newLine.Instruction.Parameters1;
                        break;
                }
            }


            return newLine;
        }

        public static AssemblerLine Clone(AssemblerLine line)
        {
            AssemblerLine newLine = new AssemblerLine(line.Text);
            newLine.Instruction = line.Instruction;
            newLine.Label = line.Label;
            newLine.Address = line.Address;
            newLine.Data = line.Data;
            newLine.ErrorText = line.ErrorText;
            newLine.Text = line.Text;
            newLine.LineNumber = line.LineNumber;
            return newLine;
        }
    }
}
