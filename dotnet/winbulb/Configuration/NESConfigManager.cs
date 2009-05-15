using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFamicom.Configuration
{
    public static class NESConfigManager
    {

        public static string LastROMFolder
        {
            get { return Properties.Settings.Default.LastROMFolder; }
            set
            {
                if (Properties.Settings.Default.LastROMFolder != value)
                {
                    Properties.Settings.Default.LastROMFolder = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public static string LastWAVFolder
        {
            get { return Properties.Settings.Default.LastWAVFolder; }
            set {
                if (Properties.Settings.Default.LastWAVFolder != value)
                {
                    Properties.Settings.Default.LastWAVFolder = value;
                    Properties.Settings.Default.Save();

                }
            }
        }
    }
}
