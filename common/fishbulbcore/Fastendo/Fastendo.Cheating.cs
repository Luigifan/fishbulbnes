using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.Fastendo.Hacking;

namespace NES.CPU.Fastendo
{
    public partial class CPU2A03
    {
        private Dictionary<int, IMemoryPatch> memoryPatches = new Dictionary<int, IMemoryPatch>();

        public Dictionary<int, IMemoryPatch> MemoryPatches
        {
            get { return memoryPatches; }
            set { memoryPatches = value; }
        }

        private Dictionary<int, int> genieCodes = new Dictionary<int, int>();

        public Dictionary<int, int> GenieCodes
        {
            get { return genieCodes; }
            set { genieCodes = value;
            }
        }
        bool _cheating = false;

        public bool Cheating
        {
            get { return _cheating; }
            set { _cheating = value; }
        } 

        //private int Cheat(int address, int result)
        //{
        //    if (genieCodes.ContainsKey(address))
        //    {
        //        int genieResult = genieCodes[address];
        //        if (genieResult > 0xFF)
        //        {
        //            // its a comparison
        //            int compare = genieResult >> 8;
        //            if (compare == result)
        //            {
        //                result = genieResult & 0xFF;
        //            }
        //        }
        //        else
        //        {
        //            result = genieResult;
        //        }
        //    }
        //    return result;
        //}


    }
}
