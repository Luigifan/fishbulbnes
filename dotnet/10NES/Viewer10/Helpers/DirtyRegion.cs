using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace _10NES.Viewer10.Helpers
{
    public class DirtyRegion
    {
        public DateTime DirtyAt
        {
            get;
            set;
        }

        public Rectangle DirtyArea
        {
            get;
            set;
        }


    }
}
