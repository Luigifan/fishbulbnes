using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstiBulb.Commanding
{
    public interface ISendCommands
    {
        CommandSender CommandSender { get; set; }
    }
}
