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
                _cpu.Debugging = value;
                _ppu.IsDebugging = value;
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
                    if (value)
                    {
                        if (currentWorkItem.Task == MachineTasks.RunContinuously)
                            paused = true;
                    } else
                    {
                        paused = false;
                        
                    }

                }

                if (!paused)
                {
                    //runState = NES.Machine.ControlPanel.RunningStatuses.Running;
                    MachineRunningResetEvent.Set();
                    machineWorkQueue.Enqueue(new MachineWorkItem() { Task = MachineTasks.RunContinuously });
                }


            }
        }

        private volatile int framesRendered;

        private AutoResetEvent MachineRunningResetEvent = new AutoResetEvent(false);

        public int FramesRendered
        {
            get { return framesRendered; }
        }

        

        private void SetupTimer()
        {

            //ThreadPool.QueueUserWorkItem(NESThreadStarter, null);
#if SILVERLIGHT
            System.Threading.Thread nesThread =
                new System.Threading.Thread(NESThreadStarter);
            
            nesThread.Name = "NESThread";
            nesThread.IsBackground = true;
            nesThread.Start(null);
#else
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(NESThreadStarter, System.Threading.Tasks.TaskCreationOptions.LongRunning);
            task.Start();
            //nesThread.Priority = ThreadPriority.AboveNormal;
#endif

        }

        volatile bool isStopped = false;

        
        void NESThreadStarter(object o)
        {
            while (true)
            {
                
                //if (paused)
                //{
                //    RunState = NES.Machine.ControlPanel.RunningStatuses.Paused;
                //    MachineRunningResetEvent.WaitOne();
                //    RunState = NES.Machine.ControlPanel.RunningStatuses.Running;
                //}

                Work();
            }
        }


        private void ForceStop()
        {


            if (currentWorkItem.Task != MachineTasks.Stoppit)
            {
                machineWorkQueue.Clear();
                machineWorkQueue.Enqueue(new MachineWorkItem() { Task = MachineTasks.Stoppit });

                while (currentWorkItem.Task != MachineTasks.Stoppit) 
                {
                    //    // make sure it isnt stuck on a pause

                    System.Threading.Thread.Sleep(1);
                }
            }
        }

        private void ReStart()
        {
            Paused = false;
        }

        public void ThreadStep()
        {
            //ForceStop();
            doDraw = true;

            machineWorkQueue.Enqueue(new MachineWorkItem() { Task = MachineTasks.RunOneStep, Result = MachineTaskResults.RunCompletedOK });
            MachineRunningResetEvent.Set();
            doDraw = false; 

        }

        public void ThreadFrame()
        {
            //ForceStop();
            doDraw = true;

            machineWorkQueue.Enqueue(new MachineWorkItem() { Task = MachineTasks.RunOneFrame, Result = MachineTaskResults.RunCompletedOK });
            MachineRunningResetEvent.Set();
            doDraw = false;
            
        }

        public void ThreadRuntendo()
        {

            doDraw = true;
            machineWorkQueue.Enqueue(new MachineWorkItem() { Task = MachineTasks.RunContinuously, Result = MachineTaskResults.RunCompletedOK });
            MachineRunningResetEvent.Set();
            RunState = NES.Machine.ControlPanel.RunningStatuses.Running;
            // doDraw = false;
        }

        public void ThreadStoptendo()
        {
            doDraw = false;

            ForceStop();
            RunState = NES.Machine.ControlPanel.RunningStatuses.Off;
        }

        MachineWorkItem currentWorkItem = new MachineWorkItem(){ Task = MachineTasks.Stoppit };
        private void Work()
        {

            if (machineWorkQueue.Count > 0)
            {
                currentWorkItem = machineWorkQueue.Dequeue();
            }
            else
            {
                // if the current work item is run, and there are no queued work items, leave it alone, or else stop 
                if (currentWorkItem.Task != MachineTasks.RunContinuously)
                    currentWorkItem = new MachineWorkItem() { Task = MachineTasks.Stoppit };
                
            }

            currentWorkItem.Result = MachineTaskResults.RunCompletedOK;
            
            task = currentWorkItem.Task;

            // if (task == MachineTasks.Stoppit) return;

            switch (task)
            {
                case MachineTasks.RunContinuously:
                    while (machineWorkQueue.Count == 0)
                    {
                        if (breakpointHit || paused)
                            break;

                        this.Runtendo();
                    }
                    StopMachine();
                    //RunState = NES.Machine.ControlPanel.RunningStatuses.Running;
                    break;

                case MachineTasks.RunOneStep:
                    this.Step();
                    RunState = NES.Machine.ControlPanel.RunningStatuses.Frozen;
                    StopMachine();
                    break;

                case MachineTasks.RunOneFrame:
                    this.RunFrame();
                    RunState = NES.Machine.ControlPanel.RunningStatuses.Frozen;
                    StopMachine();
                    break;

                case MachineTasks.Stoppit:
                    RunState = NES.Machine.ControlPanel.RunningStatuses.Off;
                    StopMachine();
                    break;

                default:
                    break;
            }
            if (breakpointHit)
            {
                ForceStop();
                breakpointHit = true;
                BreakpointHit(this, new BreakEventArgs());
                // myTimer.Stop();

            }

        }

        private void StopMachine()
        {
            
            
            // RunState = NES.Machine.ControlPanel.RunningStatuses.Off;
            if (isDebugging)
            {
                CreateNewDebugInformation();
            }

            while (paused || machineWorkQueue.Count == 0)
            {

                if (paused)
                    RunState = NES.Machine.ControlPanel.RunningStatuses.Paused;
                isStopped = true;
                MachineRunningResetEvent.WaitOne();
            }
            //MachineRunningResetEvent.Reset();
            isStopped = false;
            RunState = NES.Machine.ControlPanel.RunningStatuses.Running;
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
