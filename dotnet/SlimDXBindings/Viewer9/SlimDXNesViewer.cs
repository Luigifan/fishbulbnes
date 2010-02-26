using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NES.CPU.nitenedo.Interaction;
using System.IO;
using System.Windows.Markup;
using System.Windows.Interop;
using SlimDXBindings.Viewer;
using SlimDX.Direct3D9;
using SlimDX;
using SampleFramework;
using System.Runtime.InteropServices;
using NES.CPU.nitenedo;

namespace SlimDXNESViewer
{
    [NESDisplayPluginAttribute]
    public class SlimDXNesViewer : Border, IDisplayContext, IDisposable
    {
        private SlimDXControl panel;

        NESMachine nes;

        PropertyPanel propertiesPanel = new PropertyPanel();

        ISlimDXRenderer currentRenderer;

        public SlimDXNesViewer()
        {
        }

        public SlimDXNesViewer(NESMachine nes)
        {
            this.nes = nes;

            SetupViewer(nes);
        }

        private void SetupViewer(NESMachine nes)
        {
            panel = new SlimDXControl();

            this.Child = panel;
            currentRenderer = new IndexedTexturedQuadRenderer(panel, nes);
            //CreateDisplay();
            propertiesPanel.DataContext = this;
        }

        void SlimDXNesViewer_Loaded(object sender, RoutedEventArgs e)
        {
            panel.Initialize(true);
        }

        public NESPixelFormats PixelFormat
        {
            get
            {
                return currentRenderer.PixelFormat;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CreateDisplay()
        {

            //panel.InvalidateMeasure();
            panel.Initialize(true);
            if (panel.UseDeviceEx == false)
            {
                throw new InvalidDisplayContextException("You cannot create a Direct3D9Ex device.  You die and you go to hell.");
            }
            currentRenderer.InitializeScene();


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            panel.Initialize(true);
        }

        public void TearDownDisplay()
        {
            panel.ReleaseDevice();
            panel.ReleaseDirect3D();
        }

        public void UpdateNESScreen(int[] pixels)
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

        public int PixelWidth
        {
            get { return 8; }
        }

        
        public object PropertiesPanel
        {
            get
            {
                return propertiesPanel;
            }
        }


        public string DisplayName
        {
            get { return "D3DImage(SlimDX)"; }
        }


        public void UpdateNESScreen(IntPtr pixelData)
        {
            
        }

        public CallbackType DesiredCallback
        {
            get
            {
                return CallbackType.NoArgs;
            }

        }

        public void UpdateNESScreen()
        {
            currentRenderer.Render();
            panel.InvalidateVisual();
            panel.AllowRendering = true;
        }

        #region IDisposable Members

        public void Dispose()
        {
            currentRenderer.Dispose();
            //vertices.Dispose();
            panel.ReleaseDevice();
            panel.ReleaseDirect3D();
        }

        #endregion



        public NESMachine AttachedMachine
        {
            get
            {
                return nes;
            }
            set
            {
                nes = value;
                SetupViewer(nes);
            }
        }


        public void ToggleFullScreen()
        {
            throw new NotImplementedException();
        }
    }
}
