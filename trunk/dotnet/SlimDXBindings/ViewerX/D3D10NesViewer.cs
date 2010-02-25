﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Controls;
using NES.CPU.nitenedo.Interaction;
using NES.CPU.nitenedo;
using System.Windows.Controls;
using System.ComponentModel;
using SlimDXBindings.ViewerX.PropertyPanel;
using Fishbulb.Common.UI;
using InstiBulb.Commands;
using InstiBulb;

namespace SlimDXBindings.Viewer10
{
    public class ShmuckHost : DirectHost
    {
        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return IntPtr.Zero;
        }
    }

    public class D3D10NesViewer : Border, IDisplayContext, IDisposable, INotifyPropertyChanged
    {
        DirectHost dhost;
        D3D10Host host;
        DelegateCommand dumpFilesCommand;

        public DelegateCommand DumpSurfacesCommand
        {
            get { return dumpFilesCommand; }
        }

        PlatformDelegates winDelegates;

        public D3D10NesViewer(PlatformDelegates delegates)
        {
            winDelegates = delegates;
            dhost = new ShmuckHost();
            dumpFilesCommand = new DelegateCommand(
                o => DumpFiles(),
                o => CanDumpFiles());
            this.Child = dhost;
            this.SizeChanged += new System.Windows.SizeChangedEventHandler(D3D10NesViewer_SizeChanged);
        }

        void DumpFiles()
        {
            host.RequestDump(winDelegates);
        }

        bool CanDumpFiles()
        {
            return true;
        }

        bool initialized = false;

        void D3D10NesViewer_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {

            if (initialized)
            {
                host.RequestResize((int)this.ActualHeight, (int)this.ActualWidth);
                return;
            }

            try
            {

                host = new D3D10Host(machine);
                host.QuadUp(dhost);
                NotifyPropertyChanged("Host");
            }
            catch(Exception ex)
            {
                throw new Exception("Display Failed", ex);
            }
            initialized = true;
        }

        void D3D10NesViewer_Initialized(object sender, EventArgs e)
        {
            
        }


        NESMachine machine;
        public NES.CPU.nitenedo.NESMachine AttachedMachine
        {
            get
            {
                return machine;
            }
            set
            {
                machine = value;


            }
        }

        public CallbackType DesiredCallback
        {
            get { return CallbackType.NoArgs; }
        }

        public int PixelWidth
        {
            get { return 32; }
        }

        public NESPixelFormats PixelFormat
        {
            get
            {
                return NESPixelFormats.BGR;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CreateDisplay()
        {
        }

        public void TearDownDisplay()
        {
            
        }

        public void UpdateNESScreen()
        {
            host.DrawScreen();
        }

        public void UpdateNESScreen(int[] pixels)
        {
            host.DrawScreen();
        }

        public void UpdateNESScreen(IntPtr pixelData)
        {
            host.DrawScreen();
        }

        public void DrawDefaultDisplay()
        {
        }

        public void SetPausedState(bool state)
        {
            host.Idling = state;
        }

        public object UIControl
        {
            get { return this; }
        }


        public object PropertiesPanel
        {
            get { return new PropertyPanel(); }
        }

        public string DisplayName
        {
            get { return "Direct3D 10 Viewer"; }
        }

        public D3D10Host Host
        {
            get
            {
                return host;
            }
        }

        void NotifyPropertyChanged(string s)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(s));
        }



        public void Dispose()
        {
            if (host != null)
                host.Dispose();

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}