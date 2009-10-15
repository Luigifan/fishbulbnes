using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
// using System.Windows.Threading;
using NES.CPU.Machine.FastendoDebugging;

namespace NES.CPU.nitenedo
{
    public partial class NESMachine : IDisposable
    {


        public event EventHandler<EventArgs> RunStatusChangedEvent;

        private bool isDebugging;

        public bool IsDebugging
        {
            get { return isDebugging; }
            set 
            { 
                isDebugging = value;
                if (_cpu.Debugging != value)
                {
                    _cpu.Debugging = value;
                }
                if (_ppu.IsDebugging != value)
                {
                    _ppu.IsDebugging = value;
                }
            }
        }

        private enum MachineTasks
        {
            RunOneStep = 0,
            RunOneFrame = 1,
            RunOneScanline = 2,
            RUNFORRESTRUN = 3,
            DummyRun = 4,
            Stoppit = 99
        }

        public enum MachineTaskResults
        {
            RunCompletedOK =0,
            BreakpointHit = 1,
            OopsICrappedMyPants = 99,
        }

        private volatile MachineTaskResults taskResult;

        public MachineTaskResults TaskResult
        {
            get { return taskResult; }
            set { taskResult = value; }
        }

        private volatile MachineTasks task;
        private volatile bool keepRunning = false, paused = false;

        public bool KeepRunning
        {
            get { return keepRunning; }
            set
            {
                if (keepRunning != value)
                {
                    keepRunning = value;
                }
                if (keepRunning)
                {
                    MachineRunningResetEvent.Set();
                }
            }
        }

        public bool Paused
        {
            get { return paused; }
            set 
            {
                if (paused != value)
                {
                    paused = value;
                }
                if (!paused)
                {
                    UnPauseResetEvent.Set();
                }

            }
        }

        private volatile int framesRendered;

        private ManualResetEvent UnPauseResetEvent = new ManualResetEvent(false);
        private ManualResetEvent MachineRunningResetEvent = new ManualResetEvent(false);

        public int FramesRendered
        {
            get { return framesRendered; }
        }

        private void SetupTimer()
        {
            ThreadPool.QueueUserWorkItem(NESThreadStarter, null);

        }

        volatile bool isStopped = false;

        void NESThreadStarter(object o)
        {
            while (true)
            {
                if (!keepRunning)
                {
                    if (RunStatusChangedEvent != null)
                        RunStatusChangedEvent(this, new EventArgs());
                    isStopped = true;
                    MachineRunningResetEvent.WaitOne();
                    MachineRunningResetEvent.Reset();
                    isStopped = false;
                }
                if (paused)
                {
                    if (RunStatusChangedEvent != null)
                        RunStatusChangedEvent(this, new EventArgs());
                    UnPauseResetEvent.WaitOne();
                    UnPauseResetEvent.Reset();
                    if (RunStatusChangedEvent != null)
                        RunStatusChangedEvent(this, new EventArgs());
                }

                if (isDebugging)
                    WorkDebug();
                else
                    Work();
            }
        }


        private void ForceStop()
        {
            while (!isStopped)
            {
                // make sure it isnt stuck on a pause
                UnPauseResetEvent.Set();
                KeepRunning = false;
                System.Threading.Thread.Sleep(0);
            }
        }

        private void ReStart()
        {
            while (isStopped)
            {
                // make sure it isnt stuck on a pause
                UnPauseResetEvent.Set();
                KeepRunning = true;
                MachineRunningResetEvent.Set();
                System.Threading.Thread.Sleep(0);
            }
        }

        public void ThreadStep()
        {
            ForceStop();
            task = MachineTasks.RunOneStep;
            MachineRunningResetEvent.Set();
        }

        public void ThreadFrame()
        {
            ForceStop();
            task = MachineTasks.RunOneFrame;
            MachineRunningResetEvent.Set();
            
        }

        public void ThreadRuntendo()
        {
            KeepRunning = true;
            task = MachineTasks.RUNFORRESTRUN;
            ReStart();
        }

        public void ThreadStoptendo()
        {
            ForceStop();
        }

        private void Work()
        {
            taskResult = MachineTaskResults.RunCompletedOK;
            if (task == MachineTasks.Stoppit) return;             

            switch (task)
            {
                case MachineTasks.RunOneStep:
                    this.Step();
                    break;
                case MachineTasks.RunOneFrame:
                    this.RunFrame();
                    break;
                case MachineTasks.RUNFORRESTRUN:
                    this.Runtendo();
                    break;
                case MachineTasks.DummyRun:
                default:
                    break;
            }

        }

        private void WorkDebug()
        {
            taskResult = MachineTaskResults.RunCompletedOK;
            if (task == MachineTasks.Stoppit) return;

            switch (task)
            {
                case MachineTasks.RunOneStep:
                    this.Step();
                    break;
                case MachineTasks.RunOneFrame:
                    this.RunFrame();
                    break;
                case MachineTasks.RUNFORRESTRUN:
                    this.Runtendo();
                    break;
                case MachineTasks.DummyRun:
                default:
                    break;
            }
            if (isDebugging)
            {
                CreateNewDebugInformation();
            }
            if (breakpointHit)
            {
                keepRunning = false;
                breakpointHit = true;
                BreakpointHit(this, new BreakEventArgs());
                // myTimer.Stop();

            }
        }

        public void Runtendo()
        {
            isDebugging = false;
            RunFrame();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (debug != null)
            {
                debug.Flush();
                debug.Dispose();
            }
            if (_cart != null && _cart.CheckSum != null && SRAMWriter != null)
            {
                SRAMWriter(_cart.CheckSum, _cart.SRAM);
            }

            keepRunning = false;


            Thread.Sleep(100);
            _sharedWave.Dispose();
            soundThreader.Dispose();
            
        }

        #endregion
    }
}
