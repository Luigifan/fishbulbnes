using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.nitenedo.Interaction;
using System.Threading;
using NES.CPU.nitenedo;
using SlimDXBindings.Viewer10.ControlPanel;
//using SlimDXBindings.Viewer10.ControlPanel;

namespace SlimDXBindings.Viewer10
{
    public class DirectX10NesViewer : IDisplayContext
    {
        NESMachine nes;
        D3D10ControlPanel panel;
        public DirectX10NesViewer(NESMachine nes)
        {
            this.nes = nes;
            myQuad = new D3D10Host(nes);
            panel = new D3D10ControlPanel();

            myQuad.Dispatcher = panel.Dispatcher;
        }

        D3D10Host myQuad;

        #region IDisplayContext Members

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
                return NESPixelFormats.Indexed;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        
        public void CreateDisplay()
        {
            if (!myQuad.IsRunning)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(run));
            }

            
        }

        void tick(object o)
        {
            myQuad.UpdateTextures();
            myQuad.DrawFrame();
        }

        void run(object o)
        {
            myQuad.QuadUp();

            //var pad = nes.PadOne as SlimDXKeyboardControlPad;
            //if (pad != null)
            //    pad.Viewer = this;
        }


        public void TearDownDisplay()
        {
            myQuad.Die();
        }

        public void UpdateNESScreen()
        {

            tick(null);
        }


        public void UpdateNESScreen(int[] pixels)
        {
            throw new NotImplementedException();
        }

        public void UpdateNESScreen(IntPtr pixelData)
        {
            throw new NotImplementedException();
        }

        public void DrawDefaultDisplay()
        {
            
        }

        public void SetPausedState(bool state)
        {
            
        }

        public object UIControl
        {

            get {


                panel.DataContext = this;
                return panel;
            }
        }

        public float Hue
        {
            get
            {
                return myQuad.Hue;
            }
            set
            {
                myQuad.Hue = value;
            }
        }

        public float Brightness
        {
            get
            {
                return myQuad.Brightness;
            }
            set
            {
                myQuad.Brightness = value;
            }
        }

        public float Contrast
        {
            get
            {
                return myQuad.Contrast;
            }
            set
            {
                myQuad.Contrast = value;
            }
        }

        public object PropertiesPanel
        {
            get { return null; }
        }

        public string DisplayName
        {
            get { return "DirectX10Viewer"; }
        }

        public IntPtr WindowHandle()
        {
            return myQuad.WindowHandler;
        }

        #endregion
    }
}
