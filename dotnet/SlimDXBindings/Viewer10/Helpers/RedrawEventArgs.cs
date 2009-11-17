using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlimDXBindings.Viewer10.Helpers
{
    public class RedrawEventArgs : EventArgs
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
