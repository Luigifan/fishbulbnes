using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;

namespace InstiBulb
{
    public class PlatformDelegates
    {
        public string BrowseForFile(string defaultExt, string filter)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            string filename = null;
            //dlg.InitialDirectory = NESConfigManager.LastROMFolder;
            dlg.DefaultExt = defaultExt; // Default file extension
            dlg.Filter = filter; // Filter files by extension
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                filename = dlg.FileName;
            }
            return filename;
        }

        public string BrowseForFolder()
        {
            return null;
        }

        public void WriteSRAM(string romID, byte[] sram)
        {
            string fileName =
                Path.Combine(
                System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "InstiBulb");

            if (!Directory.Exists(fileName))
            {
                Directory.CreateDirectory(fileName);
            }
            fileName = Path.Combine(fileName, romID + ".sram");

            using (BinaryWriter writer = new BinaryWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write)))
            {
                writer.Write(sram);
                writer.Flush();
            }
        }

        public byte[] ReadSRAM(string romID)
        {
            string fileName =
                Path.Combine(
                System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "InstiBulb");

            if (!Directory.Exists(fileName))
            {
                Directory.CreateDirectory(fileName);
            }
            fileName = Path.Combine(fileName, romID + ".sram");

            byte[] sram = new byte[0x2000];

            try
            {
                using (BinaryReader reader = new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read)))
                {
                    reader.Read(sram, 0, 0x2000);
                }
            }
            catch (FileNotFoundException)
            {
                // do nothing, sram will be created later
            }
            return sram;
        }

    }
}
