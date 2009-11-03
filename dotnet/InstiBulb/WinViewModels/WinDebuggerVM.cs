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
                (DebugTarget .PPU as NES.CPU.PPUClasses.PixelWhizzler).SetupBufferForDisplay(ref table);
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
                (DebugTarget .PPU as NES.CPU.PPUClasses.PixelWhizzler).SetupBufferForDisplay(ref table);
                patternTable1 = new WriteableBitmap(128, 128, 96, 96, PixelFormats.Pbgra32, null);
                int stride = (128 * 32 + 7) / 8;

                patternTable1.WritePixels(new Int32Rect(0, 0, patternTable1.PixelWidth, patternTable1.PixelHeight), table, stride, 0);
                NotifyPropertyChanged("PatternTable1");   

            }
            return patternTable1;

        }

        MirrorMasks currentMask = MirrorMasks.FourScreenMask;

        public BitmapSource DrawNameTableZero()
        {
            if (!(DebugTarget == null))
            {
                int[] table = DebugTarget.Tiler.DoodleNameTable(0, currentMask);
                (DebugTarget.PPU as NES.CPU.PPUClasses.PixelWhizzler).SetupBufferForDisplay(ref table);
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
                int[] table = DebugTarget.Tiler.DoodleNameTable(0x400, currentMask);
                (DebugTarget .PPU as NES.CPU.PPUClasses.PixelWhizzler).SetupBufferForDisplay(ref table);
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
                int[] table = DebugTarget.Tiler.DoodleNameTable(0x800, currentMask);
                (DebugTarget .PPU as NES.CPU.PPUClasses.PixelWhizzler).SetupBufferForDisplay(ref table);
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
                int[] table = DebugTarget.Tiler.DoodleNameTable(0xC00, currentMask);
                (DebugTarget .PPU as NES.CPU.PPUClasses.PixelWhizzler).SetupBufferForDisplay(ref table);
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

    }
}
