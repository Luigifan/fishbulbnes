using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

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
    }
}
