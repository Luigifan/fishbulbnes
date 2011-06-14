using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FishbulbAssembler
{
    public class AssemblerVariable
    {
        private ValueResolver _valueResolver = new ValueResolver();

        public AssemblerVariable(string s)
        {
            Decode(s);
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

        bool isValid = false;

        public bool IsValid
        {
            get { return isValid; }
        }

        bool _isDecoded = false;

        public bool IsDecoded
        {
            get { return _isDecoded; }
            set { _isDecoded = value; }
        }

        void Decode(string s)
        {
            // a label will be STRING(doesnt start with a number) EQU Value
            string regex = @"(?<label>\w*){1}\s+(?<equ>(EQU|=))\s+(?<datatype>(\$|\%|O)?)(?<data>\w*){1}";
            Regex r = new Regex(regex);
            Match m = r.Match(s);
            Text = m.Groups["label"].Success ? m.Groups["label"].Value : null;

            var data = m.Groups["data"].Success ? m.Groups["data"].Value : null;
            var datatype = m.Groups["datatype"].Success ? m.Groups["datatype"].Value : null;

            _valueResolver.Decode(datatype, data);

            if (Text != null && Text.IsValidLabel())
            {
                if (_valueResolver.IsDecoded)
                {
                    this.Data = _valueResolver.Data;
                    _isDecoded = true;
                }
                isValid = true;
            }
            
        }
    }
}
