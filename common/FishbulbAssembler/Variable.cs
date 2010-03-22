using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FishbulbAssembler
{
    public class AssemblerVariable
    {
        public AssemblerVariable(string s)
        {
            Decode();
        }

        public string Text
        {
            get;
            set;
        }

        public byte[] Data
        {
            get;
            set;
        }

        void Decode()
        {
            // a label will be STRING(doesnt start with a number) EQU Value
            string regex = @"(?<label>\w*)\s*EQU\s*(?<data>\w*)";
            Regex r = new Regex(regex);
        }
    }
}
