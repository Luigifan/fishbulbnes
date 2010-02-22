using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Controls;
using NES.CPU.nitenedo.Interaction;
using NES.CPU.nitenedo;
using System.Windows.Controls;

namespace SlimDXBindings.Viewer10
{
    public class D3D10NesViewer : Border, IDisplayContext
    {
        DirectHost dhost;
        D3D10Host host;
        public D3D10NesViewer()
        {
            dhost = new DirectHost();
            dhost.Render = new RenderHandler(Render);
            this.Child = dhost;
            this.SizeChanged += new System.Windows.SizeChangedEventHandler(D3D10NesViewer_SizeChanged);
        }

        bool initialized = false;
        void D3D10NesViewer_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {

            if (initialized)
            {
                host.RequestResize((int)this.ActualHeight, (int)this.ActualWidth);
                return;
            }

            host = new D3D10Host(machine);
            host.QuadUp(dhost);
            initialized = true;
        }

        void D3D10NesViewer_Initialized(object sender, EventArgs e)
        {
            
        }

        void Render()
        {
            if (host != null)
            {
                host.DrawFrame();
            }
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
            get { return CallbackType.Array; }
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
        }

        public void UpdateNESScreen(int[] pixels)
        {
        }

        public void UpdateNESScreen(IntPtr pixelData)
        {
        }

        public void DrawDefaultDisplay()
        {
        }

        public void SetPausedState(bool state)
        {
        }

        public object UIControl
        {
            get { return this; }
        }

        public object PropertiesPanel
        {
            get { return null; }
        }

        public string DisplayName
        {
            get { return "Direct3D 10 Viewer"; }
        }

    }
}
