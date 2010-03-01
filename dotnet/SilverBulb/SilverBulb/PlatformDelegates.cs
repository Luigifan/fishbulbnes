using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverBulb
{
    public delegate void CommandExecuteHandler(object parm);
    public delegate bool CommandCanExecuteHandler(object parm);
    public delegate string GetFileDelegate(string defaultExt, string Filter);

    public class PlatformDelegates
    {
        public string BrowseForFile(string defaultExt, string Filter)
        {
            return null;
        }

        public void WriteSRAM(string romID, byte[] sram)
        {

        }

        public byte[] ReadSRAM(string romID)
        {
            return new byte[0x4000];
        }

    }
}
