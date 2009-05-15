using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NES.Machine.ControlPanel
{
    public enum RunningStatuses
    {
        Unloaded,
        Off,
        Running,
        Paused 
    }

    // state transitions        Cause
    // unloaded to Off          insert cart (dont autoplay)
    // unloaded to Running      insert cart (autoplay)
    // Off to Running           poweron
    // Off to Unloaded          remove cart
    // Running to Off           poweroff
    // Running to Pause         pause
    // Running to Unloaded      remove cart
    // Pause to running         unpause
    // Pause to off             PowerOff
    // Pause to unloaded        remove cart

    // insert cart available on 
    //   unloaded

    // remove cart available on 
    //   !unloaded

    // poweron available on
    //  !unloaded

    // poweroff available on 
    //  running, paused

    // pause available on 
    //   running

    // unpause available on 
    //   paused

    // reset available on 
    //  !off && !unloaded
}
