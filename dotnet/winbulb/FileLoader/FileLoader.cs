using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace InstiBulb.FileLoader
{
    public interface IFileLoader
    {
        BinaryReader BrowseForFile();
    }

    public interface IFileSaver
    {
        BinaryWriter BrowseForFile();
    }

    public class StateLoader : IFileLoader
    {
        #region IFileLoader Members

        public BinaryReader BrowseForFile()
        {
            BinaryReader resultFile; 
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "*.nes"; // Default file extension
            dlg.Filter = "SAV Files (*.sav)|*.sav;|All Files (*.*)|*.*"; // Filter files by extension
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                resultFile = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            }
            else
            {
                resultFile = new BinaryReader(new MemoryStream());
            }
            return resultFile;
        }

        #endregion
    }

    public class StateSaver : IFileSaver
    {
        #region IFileLoader Members

        public BinaryWriter BrowseForFile()
        {
            BinaryWriter resultFile;
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = "*.nes"; // Default file extension
            dlg.Filter = "SAV Files (*.sav)|*.sav;|All Files (*.*)|*.*"; // Filter files by extension
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                resultFile = new BinaryWriter(new FileStream(filename, FileMode.Create, FileAccess.Write));
            }
            else
            {
                resultFile = new BinaryWriter(new MemoryStream());
            }
            return resultFile;
        }

        #endregion
    }
}
