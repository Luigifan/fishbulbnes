using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU;
using System.ComponentModel;
using System.Security;
using NES.CPU.nitenedo;
using NES.CPU.CPUDebugging;
//using WPFamicom.MachineStatusModel;
using NES.CPU.FastendoDebugging;
using NES.CPU.Machine.FastendoDebugging;
using NES.CPU.PPUClasses;

namespace Fishbulb.Common.UI
{

    public class DebuggerVM : IViewModel
    {
        private NESMachine _nes = null;


        public NESMachine DebugTarget
        {
            get { return _nes; }
            //set
            //{
            //    _nes = value;
            //    _nes.DebugInfoChanged += new EventHandler<BreakEventArgs>(_nes_DebugInfoChanged);
            //    if (_nes != null)
            //    {
            //        UpdateDebugInfo();
            //        _breakpoints = new List<string>(
            //            from b in _nes.BreakPoints select b.Address.ToString()
            //            );
            //        _nes.BreakpointHit += new EventHandler<BreakEventArgs>(_nes_BreakpointHit);
            //    }
            //}
        }

        void _nes_BreakpointHit(object sender, BreakEventArgs e)
        {
            _breakpointHit = true;
            NotifyPropertyChanged("BreakpointHit");
        }

        private bool _breakpointHit;

        public bool BreakpointHit
        {
            get { return _breakpointHit; }
        }

        /// <summary>
        /// returns the current debuginformation object, or an empty object
        /// </summary>
        public DebugInformation DebuggerInformation
        {
            get
            {
                if (_nes == null || _nes.DebugInfo == null)
                    return null;
                //return new DebugInformation { CPU = new DebuggerCPUState(), PPU = new DebuggerPPUState() };
                return _nes.DebugInfo;
            }
        }

        void _nes_DebugInfoChanged(object sender, BreakEventArgs e)
        {
            UpdateDebugInfo();
        }

        //        public DebuggerVM()
        //        {
        //            _nes = null;
        //            
        //            NotifyPropertyChanged("StepCmd");
        //        }

        public DebuggerVM(NESMachine nes)
        {
            _nes = nes;
            commands.Add ("Step", new InstigatorCommand( 
                (o) => Step(), 
                (o) => true ));
            commands.Add("StepFrame", new InstigatorCommand(
                (o) => StepFrame(),
                (o) => true));
        }


        public void UpdateDebugInfo()
        {

            if (_nes == null) return;
            if (_nes.DebugInfo == null) return;

            NotifyPropertyChanged("DebuggerInformation");
            NotifyPropertyChanged("FrameWriteEvents");
        }

        public void Step()
        {
            if (_nes == null) return;
			_nes.IsDebugging=true;
            _nes.ThreadStep();

            UpdateDebugInfo();
        }

        public void StepFrame()
        {
            if (_nes == null) return;
			_nes.IsDebugging=true;
            _nes.ThreadFrame();

            UpdateDebugInfo();
        }

        #region INotifyPropertyChanged Members

        public void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private List<string> _breakpoints = null;

        public IEnumerable<string> Breakpoints
        {
            get
            {
                return _breakpoints;
            }
        }

        public void AddBreakpoint(int address)
        {
            if (_nes != null)
            {
                CPUBreakpoint newCPUBreakpoint = new CPUBreakpoint() { Address = address };

                _nes.BreakPoints.Add(newCPUBreakpoint);
                _breakpoints.Add(string.Format("{0:x4}", newCPUBreakpoint.Address));
                NotifyPropertyChanged("Breakpoints");
            }
        }


        public string CurrentView
        {
            get
            {
                return "DebugStepper";
            }
        }

        Dictionary<string, ICommandWrapper> commands = new Dictionary<string,ICommandWrapper>();

        public Dictionary<string, ICommandWrapper> Commands
        {
            get
            {
                return commands;
            }
        }

        public IEnumerable<IViewModel> ChildViewModels
        {
            get
            {
                return new List<IViewModel>();
            }
        }

        public string CurrentRegion
        {
            get
            {
                return "debugger.1";
            }
        }

        public string Header
        {
            get
            {
                return "Step";
				
            }
        }

        public object DataModel
        {
            get
            {
                return null;
            }
        }

        //        private WriteableBitmap patternTable0 = null, patternTable1= null;
        //        private WriteableBitmap nameTable0 = null, nameTable1 = null;
        //        private WriteableBitmap nameTable2 = null, nameTable3 = null;
        //
        //        public BitmapSource DrawPatternTableZero()
        //        {
        //            if (!(_nes == null))
        //            {
        //                int[] table = _nes.Tiler.DoodlePatternTable(0);
        //                (_nes.PPU as NES.CPU.PPUClasses.PixelWhizzler).SetupBufferForDisplay(ref table);
        //                patternTable0 = new WriteableBitmap(128, 128, 96, 96, PixelFormats.Pbgra32, null);
        //                int stride = (128 * 32 + 7) / 8;
        //
        //                patternTable0.WritePixels(new Int32Rect(0, 0, patternTable0.PixelWidth, patternTable0.PixelHeight), table, stride, 0);
        //            }
        //            return patternTable0;
        //
        //        }
        //
        //        public BitmapSource DrawPatternTableOne()
        //        {
        //            if (!(_nes == null))
        //            {
        //                int[] table = _nes.Tiler.DoodlePatternTable(0x1000);
        //                (_nes.PPU as NES.CPU.PPUClasses.PixelWhizzler).SetupBufferForDisplay(ref table);
        //                patternTable1 = new WriteableBitmap(128, 128, 96, 96, PixelFormats.Pbgra32, null);
        //                int stride = (128 * 32 + 7) / 8;
        //
        //                patternTable1.WritePixels(new Int32Rect(0, 0, patternTable1.PixelWidth, patternTable1.PixelHeight), table, stride, 0);
        //            }
        //            return patternTable1;
        //
        //        }
        //
        //        MirrorMasks currentMask = MirrorMasks.FourScreenMask;
        //
        //        public BitmapSource DrawNameTableZero()
        //        {
        //            if (!(_nes == null))
        //            {
        //                int[] table = _nes.Tiler.DoodleNameTable(0, currentMask);
        //                (_nes.PPU as NES.CPU.PPUClasses.PixelWhizzler).SetupBufferForDisplay(ref table);
        //                nameTable0 = new WriteableBitmap(256, 240, 96, 96, PixelFormats.Pbgra32, null);
        //                int stride = (256 * 32 + 7) / 8;
        //
        //                nameTable0.WritePixels(new Int32Rect(0, 0, nameTable0.PixelWidth, nameTable0.PixelHeight), table, stride, 0);
        //            }
        //            return nameTable0;
        //
        //        }
        //
        //        public BitmapSource DrawNameTableOne()
        //        {
        //            if (!(_nes == null))
        //            {
        //                int[] table = _nes.Tiler.DoodleNameTable(0x400, currentMask);
        //                (_nes.PPU as NES.CPU.PPUClasses.PixelWhizzler).SetupBufferForDisplay(ref table);
        //                nameTable1 = new WriteableBitmap(256, 240, 96, 96, PixelFormats.Pbgra32, null);
        //                int stride = (256 * 32 + 7) / 8;
        //
        //                nameTable1.WritePixels(new Int32Rect(0, 0, nameTable1.PixelWidth, nameTable1.PixelHeight), table, stride, 0);
        //            }
        //            return nameTable1;
        //
        //
        //        }
        //
        //        public BitmapSource DrawNameTableTwo()
        //        {
        //            if (!(_nes == null))
        //            {
        //                int[] table = _nes.Tiler.DoodleNameTable(0x800, currentMask);
        //                (_nes.PPU as NES.CPU.PPUClasses.PixelWhizzler).SetupBufferForDisplay(ref table);
        //                nameTable2 = new WriteableBitmap(256, 240, 96, 96, PixelFormats.Pbgra32, null);
        //                int stride = (256 * 32 + 7) / 8;
        //
        //                nameTable2.WritePixels(new Int32Rect(0, 0, nameTable2.PixelWidth, nameTable2.PixelHeight), table, stride, 0);
        //            }
        //            return nameTable2;
        //
        //        }
        //
        //        public BitmapSource DrawNameTableThree()
        //        {
        //            if (!(_nes == null))
        //            {
        //                int[] table = _nes.Tiler.DoodleNameTable(0xC00, currentMask);
        //                (_nes.PPU as NES.CPU.PPUClasses.PixelWhizzler).SetupBufferForDisplay(ref table);
        //                nameTable3 = new WriteableBitmap(256, 240, 96, 96, PixelFormats.Pbgra32, null);
        //                int stride = (256 * 32 + 7) / 8;
        //
        //                nameTable3.WritePixels(new Int32Rect(0, 0, nameTable3.PixelWidth, nameTable3.PixelHeight), table, stride, 0);
        //            }
        //            return nameTable3;
        //        }

        public List<PPUWriteEvent> FrameWriteEvents
        {
            get
            {
                if (_nes == null) return new List<PPUWriteEvent>();
                return _nes.DebugInfo.PPU.FrameWriteEvents;
            }
        }
    }
}
