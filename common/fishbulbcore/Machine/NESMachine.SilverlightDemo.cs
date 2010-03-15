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
using System.Reflection;

namespace NES.CPU.nitenedo
{
    public partial class NESMachine
    {
        const string cartResName = "CPU6502.Silverlight.testcart.nes";

        private Stream GetMeAStream(string filter)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            Stream filename = null;
            dlg.Multiselect = false;
            //dlg.InitialDirectory = NESConfigManager.LastROMFolder;
            //dlg.DefaultExt = defaultExt; // Default file extension
            dlg.Filter = filter; // Filter files by extension
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                if (dlg.File.Extension.ToLower() == ".zip")
                {
                    using (var zipStream = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(dlg.File.OpenRead()))
                    {
                        var entry = zipStream.GetNextEntry();

                        if (entry.Name.IndexOf(".nes") > 0)
                        {
                            byte[] data;//= new byte[entry.Size];

                            BinaryReader reader = new BinaryReader(zipStream);
                            data = reader.ReadBytes((int)entry.Size);

                            //int len = zipStream.Read(data, 0, (int)entry.Size);
                            //reader.Close();
                            filename = new MemoryStream(data);
                        }
                    }

                }
                else
                {

                    filename = dlg.File.OpenRead();
                }
            }
            return filename;

        }

        public void SilverlightStartDemo()
        {
            EjectCart();

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(cartResName) )
            {

                if (stream == null) return;

                if (runState == NES.Machine.ControlPanel.RunningStatuses.Running) ThreadStoptendo();

                _cart = iNESFileHandler.LoadROM(PPU, stream);
                
                if (SRAMReader != null && _cart.UsesSRAM)
                    _cart.SRAM = SRAMReader(_cart.CheckSum);

                if (_cart == null)
                    return;

                _cpu.Cart = (IClockedMemoryMappedIOElement)_cart;
                
                _ppu.ChrRomHandler = _cart;
                PowerOn();
                //while (runState != NES.Machine.ControlPanel.RunningStatuses.Running)
                ThreadRuntendo();

            }
        }

        public void SilverlightStart()
        {
            EjectCart();
            
            using (Stream stream = GetMeAStream("NES Files (*.nes,*.zip)|*.nes;*.zip"))
            {

                if (stream == null) return;

                if (runState == NES.Machine.ControlPanel.RunningStatuses.Running) ThreadStoptendo();

                _cart = iNESFileHandler.LoadROM(PPU, stream);

                if (_cart == null)
                    return;

                _cpu.Cart = (IClockedMemoryMappedIOElement)_cart;
                _ppu.ChrRomHandler = _cart;
                PowerOn();
                //while (runState != NES.Machine.ControlPanel.RunningStatuses.Running)
                ThreadRuntendo();

            }
            
        }

        
    }
}
