using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Controls;
using NES.CPU.nitenedo.Interaction;
using NES.CPU.nitenedo;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace SlimDXBindings.Viewer10
{

    public class D3D10EmbeddableNesViewer : Border, IDisplayContext, IDisposable
    {
        ShmuckHost dhost = new ShmuckHost();
        D3D10Host host;
        D3DImageEx image;
        public D3D10EmbeddableNesViewer()
        {
            image = new D3DImageEx();
            this.Child = dhost;
            this.Background  =
                new ImageBrush(
                image);
            
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
            //this.Child = null;
            image.SetBackBufferEx(D3DResourceTypeEx.ID3D10Texture2D, host.RenderTargetHandle);

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
                this.InvalidateVisual();
                //dhost.InvalidateVisual();
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
            get { return null; }
        }

        public string DisplayName
        {
            get { return "Direct3D 10 Viewer"; }
        }


        public void Dispose()
        {
            if (host != null)
                host.Dispose();

        }


        public void ToggleFullScreen()
        {
            throw new NotImplementedException();
        }
    }
}
