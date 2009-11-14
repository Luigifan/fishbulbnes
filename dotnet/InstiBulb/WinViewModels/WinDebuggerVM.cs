using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fishbulb.Common.UI;
using System.Collections.ObjectModel;
using NES.CPU.PPUClasses;
using NES.CPU.nitenedo;
using NES.CPU.FastendoDebugging;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;

namespace InstiBulb.WinViewModels
{

    public class TileInformation
    {
        int[] lineAddresses;
        int tileIndex;

        public int TileIndex
        {
            get { return tileIndex; }
            set { tileIndex = value; }
        }

        public int[] LineAddresses
        {
            get { return lineAddresses; }
            set { lineAddresses = value; }
        }

        public int[] TileData
        {
            get;
            private set;
        }

        WriteableBitmap bitmap = new WriteableBitmap(8, 8, 96, 96, PixelFormats.Pbgra32, null);

        public WriteableBitmap Bitmap
        {
            get { return bitmap; }
            set { bitmap = value; }
        }

        public TileInformation(int tileIndex, int[] data, int[] addresses)
        {
            int stride = (8 * 32 + 7) / 8;

            uint[] colors = new uint[64];
            for (int i = 0; i < 64; ++i)
            {
                switch (data[i])
                {
                    case 0:
                        colors[i] = 0xFF000000;
                        break;
                    case 1:
                        colors[i] = 0xFFFF0000;
                        break;
                    case 2:
                        colors[i] = 0xFF0000FF;
                        break;
                }
            }
            TileData = data;
            
            bitmap.WritePixels(new Int32Rect(0, 0, 8, 8), colors, stride, 0);
            this.tileIndex = tileIndex;
            lineAddresses = addresses;
        }

    }

    public class WinDebuggerVM : DebuggerVM
    {

        public WinDebuggerVM(NESMachine nes)
            : base(nes)
        {

            base.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(WinDebuggerVM_PropertyChanged);
            this.DrawNameTables = new DisplayNameTablesCommand(this);
            this.DrawTiles = new DisplayTilesCommand(this) ;
        }

        void WinDebuggerVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FrameWriteEvents")
            {
                _PPUWriteEvents.Clear();
                foreach (var p in FrameWriteEvents)
                {
                    _PPUWriteEvents.Add(p);
                }

            }
            if (e.PropertyName == "DebuggerInformation")
            {
                futureOps.Clear();
                foreach (var p in DebuggerInformation.FutureOps)
                {
                    futureOps.Add(p);
                }
                instructionHistory.Clear();
                if (DebuggerInformation.InstructionHistory != null)
                    instructionHistory.Concat(DebuggerInformation.InstructionHistory);
            }
        }

        ObservableCollection<PPUWriteEvent> _PPUWriteEvents = new ObservableCollection<PPUWriteEvent>();
        public ObservableCollection<PPUWriteEvent> PPUWriteEvents
        {
            get { return _PPUWriteEvents; }
        }

        ObservableCollection<InstructionRolloutItem> futureOps = new ObservableCollection<InstructionRolloutItem>();
        
        public ObservableCollection<InstructionRolloutItem> FutureOps
        {
            get
            {
                return futureOps;
            }
        }

        ObservableCollection<string> instructionHistory = new ObservableCollection<string>();

        public ObservableCollection<string> InstructionHistory
        {
            get
            {
                return instructionHistory;
            }
        }


        private WriteableBitmap patternTable0 = null, patternTable1 = null;

        public WriteableBitmap PatternTable1
        {
            get { return patternTable1; }
            set { patternTable1 = value; }
        }

        public WriteableBitmap PatternTable0
        {
            get { return patternTable0; }
            set { patternTable0 = value; }
        }
        private WriteableBitmap nameTable0 = null, nameTable1 = null;
        public BitmapSource NameTable0
        {
            get { return nameTable0; }
        }
        public WriteableBitmap NameTable1
        {
            get { return nameTable1; }
        }
        private WriteableBitmap nameTable2 = null, nameTable3 = null;

        public WriteableBitmap NameTable3
        {
            get { return nameTable3; }
        }

        public WriteableBitmap NameTable2
        {
            get { return nameTable2; }
        }

        public BitmapSource DrawPatternTableZero()
        {
            if (!(DebugTarget  == null))
            {
                int[] table = DebugTarget.Tiler.DoodlePatternTable(0);
                DebugTarget .PPU .SetupBufferForDisplay(ref table);
                patternTable0 = new WriteableBitmap(128, 128, 96, 96, PixelFormats.Pbgra32, null);
                int stride = (128 * 32 + 7) / 8;

                patternTable0.WritePixels(new Int32Rect(0, 0, patternTable0.PixelWidth, patternTable0.PixelHeight), table, stride, 0);
                NotifyPropertyChanged("PatternTable0");   
            }
            return patternTable0;

        }

        public BitmapSource DrawPatternTableOne()
        {
            if (!(DebugTarget  == null))
            {
                int[] table = DebugTarget.Tiler.DoodlePatternTable(0x1000);
                DebugTarget .PPU .SetupBufferForDisplay(ref table);
                patternTable1 = new WriteableBitmap(128, 128, 96, 96, PixelFormats.Pbgra32, null);
                int stride = (128 * 32 + 7) / 8;

                patternTable1.WritePixels(new Int32Rect(0, 0, patternTable1.PixelWidth, patternTable1.PixelHeight), table, stride, 0);
                NotifyPropertyChanged("PatternTable1");   

            }
            return patternTable1;

        }

        //MirrorMasks currentMask = MirrorMasks.FourScreenMask;

        public BitmapSource DrawNameTableZero()
        {
            if (!(DebugTarget == null))
            {
                int[] table = DebugTarget.Tiler.DoodleNameTable(0);
                DebugTarget.PPU .SetupBufferForDisplay(ref table);
                nameTable0 = new WriteableBitmap(256, 240, 96, 96, PixelFormats.Pbgra32, null);
                int stride = (256 * 32 + 7) / 8;

                nameTable0.WritePixels(new Int32Rect(0, 0, nameTable0.PixelWidth, nameTable0.PixelHeight), table, stride, 0);
                NotifyPropertyChanged("NameTable0");   

            }
            return nameTable0;

        }

        public BitmapSource DrawNameTableOne()
        {
            if (!(DebugTarget  == null))
            {
                int[] table = DebugTarget.Tiler.DoodleNameTable(0x400);
                DebugTarget .PPU .SetupBufferForDisplay(ref table);
                nameTable1 = new WriteableBitmap(256, 240, 96, 96, PixelFormats.Pbgra32, null);
                int stride = (256 * 32 + 7) / 8;

                nameTable1.WritePixels(new Int32Rect(0, 0, nameTable1.PixelWidth, nameTable1.PixelHeight), table, stride, 0);
                NotifyPropertyChanged("NameTable1");   

            }
            return nameTable1;


        }

        public BitmapSource DrawNameTableTwo()
        {
            if (!(DebugTarget  == null))
            {
                int[] table = DebugTarget.Tiler.DoodleNameTable(0x800);
                DebugTarget .PPU.SetupBufferForDisplay(ref table);
                nameTable2 = new WriteableBitmap(256, 240, 96, 96, PixelFormats.Pbgra32, null);
                int stride = (256 * 32 + 7) / 8;

                nameTable2.WritePixels(new Int32Rect(0, 0, nameTable2.PixelWidth, nameTable2.PixelHeight), table, stride, 0);
                NotifyPropertyChanged("NameTable2");   

            }
            return nameTable2;

        }

        public void DrawNameTableThree()
        {
            if (!(DebugTarget  == null))
            {
                int[] table = DebugTarget.Tiler.DoodleNameTable(0xC00);
                DebugTarget .PPU.SetupBufferForDisplay(ref table);
                nameTable3 = new WriteableBitmap(256, 240, 96, 96, PixelFormats.Pbgra32, null);
                int stride = (256 * 32 + 7) / 8;

                nameTable3.WritePixels(new Int32Rect(0, 0, nameTable3.PixelWidth, nameTable3.PixelHeight), table, stride, 0);
                NotifyPropertyChanged("NameTable3");   

            }
        }

        public ICommand DrawTiles
        {
            get;
            private set;
        }

        public ICommand DrawNameTables
        {
            get;
            private set;
        }

        public string WhichTileAddress(int table, int x, int y)
        {
            if (DebugTarget == null) return "Unknown";

            return string.Format("Address: {0}", DebugTarget.Tiler.GetPatternEntryLocation(table, x, y));
        }

        public TileInformation GetTileInfo(int table, int tileIndex)
        {
            if (DebugTarget == null) return null;
            int[][] result = new int[2][];
            result[0] = new int[64];
            result[1] = new int[8];

            result[0] = DebugTarget.Tiler.GetPatternTableEntry(table, tileIndex, 0, out result[1]);

            return new TileInformation(tileIndex, result[0], result[1]);

        }

    }
}
