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


        public List<PPUWriteEvent> FrameWriteEvents
        {
            get
            {
                if (_nes == null || _nes.DebugInfo == null || _nes.DebugInfo.PPU == null) return new List<PPUWriteEvent>();

                return _nes.DebugInfo.PPU.FrameWriteEvents;
            }
        }
    }
}
