﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FishbulbAssembler
{
    public enum ChunkTypes
    {
        Instruction,
        Raw,
    }

    public class AssemblyChunk : List<byte>
    {
        public string Label
        {
            get;
            set;
        }

    }
}
