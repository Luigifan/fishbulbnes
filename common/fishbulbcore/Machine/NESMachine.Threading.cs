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

        public enum MachineTasks
        {
            RunOneStep = 0,
            RunOneFrame = 1,
            RunOneScanline = 2,
            RunContinuously = 3,
            Stoppit = 99
        }

        public enum MachineTaskResults
        {
            RunCompletedOK =0,
            BreakpointHit = 1,
            OopsICrappedMyPants = 99,
        }

        public volatile MachineTaskResults taskResult;

        public class MachineWorkItem
        {
            public MachineTasks Task
            {
                get;
                set;
            }
            public MachineTaskResults Result
            {
                get;
                set;
            }
        }

        private Queue<MachineWorkItem> machineWorkQueue = new Queue<MachineWorkItem>();

        public MachineTaskResults TaskResult
        {
            get { return taskResult; }
            set { taskResult = value; }
        }

        private volatile MachineTasks task;
        private volatile bool paused = false;

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
                    runState = NES.Machine.ControlPanel.RunningStatuses.Running;
                    UnPauseResetEvent.Set();
                }
                else
                {
                    runState = NES.Machine.ControlPanel.RunningStatuses.Paused;
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
                
                if (paused)
                {
                    if (RunStatusChangedEvent != null)
                        RunStatusChangedEvent(this, new EventArgs());
                    UnPauseResetEvent.WaitOne();
                    UnPauseResetEvent.Reset();
                    if (RunStatusChangedEvent != null)
                        RunStatusChangedEvent(this, new EventArgs());
                }

                Work();
            }
        }


        private void ForceStop()
        {
            if (_cart != null && _cart.CheckSum != null && SRAMWriter != null)
            {
                SRAMWriter(_cart.CheckSum, _cart.SRAM);
            }

            machineWorkQueue.Enqueue(new MachineWorkItem() { Task = MachineTasks.Stoppit });
            //while (machineWorkQueue.Count > 0)
            //{
            //    // make sure it isnt stuck on a pause
            //    UnPauseResetEvent.Set();
            //    System.Threading.Thread.Sleep(0);
            //}
        }

        private void ReStart()
        {
            Paused = false;
            while (isStopped)
            {
                // make sure it isnt stuck on a pause
                UnPauseResetEvent.Set();
                MachineRunningResetEvent.Set();
                System.Threading.Thread.Sleep(0);
            }
        }

        public void ThreadStep()
        {
            ForceStop();
            
            machineWorkQueue.Enqueue(new MachineWorkItem() { Task = MachineTasks.RunOneStep, Result = MachineTaskResults.RunCompletedOK });
            MachineRunningResetEvent.Set();
        }

        public void ThreadFrame()
        {
            ForceStop();
            
            machineWorkQueue.Enqueue(new MachineWorkItem() { Task = MachineTasks.RunOneFrame, Result = MachineTaskResults.RunCompletedOK });
            MachineRunningResetEvent.Set();
            
        }

        public void ThreadRuntendo()
        {
            Paused = false;
            machineWorkQueue.Enqueue(new MachineWorkItem() { Task = MachineTasks.RunContinuously, Result = MachineTaskResults.RunCompletedOK });
            ReStart();
        }

        public void ThreadStoptendo()
        {

            ForceStop();
        }

        MachineWorkItem currentWorkItem;
        private void Work()
        {

            if (machineWorkQueue.Count > 0)
            {
                currentWorkItem = machineWorkQueue.Dequeue();
            }
            else
            {
                // if the current work item is run, and there are no queued work items, leave it alone, or else stop 
                if (currentWorkItem == null || currentWorkItem.Task != MachineTasks.RunContinuously)
                    currentWorkItem = new MachineWorkItem() { Task = MachineTasks.Stoppit };
                
            }

            currentWorkItem.Result = MachineTaskResults.RunCompletedOK;
            
            task = currentWorkItem.Task;

            // if (task == MachineTasks.Stoppit) return;

            switch (task)
            {
                case MachineTasks.RunOneStep:
                    this.Step();
                    break;
                case MachineTasks.RunOneFrame:
                    this.RunFrame();
                    break;
                case MachineTasks.RunContinuously:
                    this.Runtendo();
                    break;
                case MachineTasks.Stoppit:
                    isStopped = true;
                    MachineRunningResetEvent.WaitOne();
                    isStopped = false;
                    break;
                default:
                    break;
            }
            if (isDebugging)
            {
                CreateNewDebugInformation();
            }
            if (breakpointHit)
            {
                ForceStop();
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



            Thread.Sleep(100);
            _sharedWave.Dispose();
            soundThreader.Dispose();
            
        }

        #endregion
    }
}
