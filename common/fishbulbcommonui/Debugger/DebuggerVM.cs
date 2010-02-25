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
using fishbulbcommonui;
using System.Collections.ObjectModel;

namespace Fishbulb.Common.UI
{

    public class DebuggerVM : BaseNESViewModel
    {

        void _nes_BreakpointHit(object sender, BreakEventArgs e)
        {
            _breakpointHit = true;
            NotifyPropertyChanged("BreakpointHit");
        }

        protected override void OnAttachTarget()
        {
            TargetMachine.IsDebugging = true;
            TargetMachine.DebugInfoChanged -= _nes_DebugInfoChanged;
            TargetMachine.DebugInfoChanged += _nes_DebugInfoChanged;
            base.OnAttachTarget();
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
                if (TargetMachine == null || TargetMachine.DebugInfo == null)
                    return null;
                //return new DebugInformation { CPU = new DebuggerCPUState(), PPU = new DebuggerPPUState() };
                return TargetMachine.DebugInfo;
            }
        }

        void _nes_DebugInfoChanged(object sender, BreakEventArgs e)
        {
            UpdateDebugInfo();
        }


        public DebuggerVM()
        {
            Commands.Add ("Step", new InstigatorCommand( 
                (o) => Step(), 
                (o) => true ));
            Commands.Add("StepFrame", new InstigatorCommand(
                (o) => StepFrame(),
                (o) => true));
            Commands.Add("Continue", new InstigatorCommand(
                (o) => Continue(),
                (o) => true));

        }


        public void UpdateDebugInfo()
        {

            if (TargetMachine == null) return;
            if (TargetMachine.DebugInfo == null) return;
            _frameWrites.Clear();
            if (TargetMachine.DebugInfo.PPU != null)
            {
                foreach (PPUWriteEvent pEv in TargetMachine.DebugInfo.PPU.FrameWriteEvents)
                {
                    _frameWrites.Add(pEv);
                }
            }
            NotifyPropertyChanged("DebuggerInformation");
            NotifyPropertyChanged("FrameWriteEvents");
        }

        public void Step()
        {
            if (TargetMachine == null) return;
            TargetMachine.IsDebugging = true;
            TargetMachine.ThreadStep();

            UpdateDebugInfo();
        }

        public void StepFrame()
        {
            if (TargetMachine == null) return;
            TargetMachine.IsDebugging = true;
            TargetMachine.ThreadFrame();

            UpdateDebugInfo();
        }

        public void Continue()
        {
            if (TargetMachine == null) return;
            TargetMachine.IsDebugging = false;
            TargetMachine.ThreadRuntendo();

            UpdateDebugInfo();
        }
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
            if (TargetMachine != null)
            {
                CPUBreakpoint newCPUBreakpoint = new CPUBreakpoint() { Address = address };

                TargetMachine.BreakPoints.Add(newCPUBreakpoint);
                _breakpoints.Add(string.Format("{0:x4}", newCPUBreakpoint.Address));
                NotifyPropertyChanged("Breakpoints");
            }
        }


        public override string CurrentView
        {
            get
            {
                return "DebugStepper";
            }
        }


        public override string CurrentRegion
        {
            get
            {
                return "debugger.1";
            }
        }

        public override string Header
        {
            get
            {
                return "Step";
				
            }
        }


        List<PPUWriteEvent> _frameWrites = new List<PPUWriteEvent>();
        public List<PPUWriteEvent> FrameWriteEvents
        {
            get
            {
                return _frameWrites;
            }
        }
    }
}
