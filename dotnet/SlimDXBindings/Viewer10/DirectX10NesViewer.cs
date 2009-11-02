using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NES.CPU.nitenedo.Interaction;
using System.Threading;
using NES.CPU.nitenedo;

namespace SlimDXBindings.Viewer10
{
    public class DirectX10NesViewer : IDisplayContext
    {
        NESMachine nes;
        public DirectX10NesViewer(NESMachine nes)
        {
            myQuad = new D3D10Host(nes);
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
            myQuad.DrawFrame();
        }

        void run(object o)

        {
            myQuad.QuadUp(); 
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
            get { return new System.Windows.Controls.Canvas(); }
        }

        public object PropertiesPanel
        {
            get { return null; }
        }

        public string DisplayName
        {
            get { return "DirectX10Viewer"; }
        }

        #endregion
    }
}
