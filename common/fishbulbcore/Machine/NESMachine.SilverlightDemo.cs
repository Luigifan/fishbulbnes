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
using NES.CPU.Machine.ROMLoader;
using NES.CPU.Fastendo;
using System.IO;

namespace NES.CPU.nitenedo
{
    public partial class NESMachine
    {
        const string cartResName = "CPU6502.Silverlight.testcart.nes";

        public void StartDemo()
        {

            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(cartResName))
            {
                _cart = iNESFileHandler.LoadROM(PPU, stream);

                _cpu.Cart = (IClockedMemoryMappedIOElement)_cart;
                _ppu.ChrRomHandler = _cart;
                PowerOn();
                ThreadRuntendo();
            }
        }
    }
}
