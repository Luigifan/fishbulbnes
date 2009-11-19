using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SlimDXBindings.Viewer10.ControlPanel
{
    public interface IAm3DEmbeddable
    {
        bool NeedsRefreshing { get; }
        void HandleKeystroke(Key key);

    }
}
