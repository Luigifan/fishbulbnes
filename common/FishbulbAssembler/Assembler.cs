﻿using System;
using System.Linq;
using System.Net;
using System.IO;
using System.Collections.Generic;
using NES.CPU.FastendoDebugging;
using NES.CPU.Fastendo;
using System.Text.RegularExpressions;
using System.Text;
using System.ComponentModel;

namespace FishbulbAssembler
{

    public class Assembler : INotifyPropertyChanged
    {


        public string Text
        {
            get;
            set;
        }

        public static string ValidOpsPattern()
        {
            // all assembly instructions
            StringBuilder builder = new StringBuilder();
            foreach (var p in DisassemblyExtensions.OpCodes)
            {
                builder.Append(p.Text).Append("|");
            }

            // direct assignment
            //builder.Append(@"\.db").Append("|");

            //// equators
            //builder.Append(@"\=").Append("|");
            //builder.Append("EQU").Append("|");

            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }


        List<string> text = new List<string>();
        List<byte[]> _output = new List<byte[]>();


        void LoadText()
        {
            text.Clear();
            using (StringReader r = new StringReader(Text))
            {
                string nextLine = r.ReadLine();
                while (nextLine != null)
                {
                    text.Add(nextLine);
                    nextLine = r.ReadLine();
                }
            }

        }


        List<AssemblerLine> midAssembly = new List<AssemblerLine>();

        public List<AssemblerLine> MidAssembly
        {
            get { return midAssembly; }
            set { midAssembly = value; }
        }

        List<byte> _finalOutput = new List<byte>();

        public void Assemble()
        {
            if (Text == null) return;
            LoadText();
            _output.Clear();
            _finalOutput.Clear();

            // grab all the variable assignments and fully decode them
            var variables = (from fileLine in text 
                            select new AssemblerVariable(fileLine)).ToList();

            int i =0;
            midAssembly = (
                from fileline in
                    from s in text
                    select new AssemblerLine(s) { Instruction = new AssemblerInstruction(variables), LineNumber = i++ } 
                    
                where 
                    fileline.Instruction.Decode(fileline.Code) 
                select AssemblerLine.CloneAndUpdate(fileline)).ToList()
                ;

            // output as List<byte[]>
            _output = (from line in midAssembly where line.Data != null select line.Data).ToList();

            // one big bytearray, ready for running
            // todo: appending headers, etc
            foreach (var bytes in _output)
            {
                foreach (byte b in bytes)
                {
                    _finalOutput.Add(b);
                }
            }
            NotifyPropertyChanged("FinalOutput");
            NotifyPropertyChanged("MidAssembly");
        }

        public List<byte[]> Output
        {
            get { return _output; }
        }

        public IEnumerable<byte> FinalOutput
        {
            get { return _finalOutput; }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
