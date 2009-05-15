using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using NES.CPU.nitenedo.Interaction;
using System.Windows.Controls;
using System.Windows.Markup;
using System.IO;

namespace WPFamicom.OpenGLDisplay
{
    public class OpenGLViewWritableBitmap : Canvas, IDisplayContext
    {

        private OpenGLNESViewer viewer;
        public OpenGLViewWritableBitmap()
        {
            viewer = new OpenGLNESViewer(RenderEngines.RGBInputLinearFilter);
            this.PixelFormat = NESPixelFormats.RGB;
            this.Background = new ImageBrush(viewer.NESBitmap);
        }

        private string[] renderShapes = new string[] { "Billboard", "Cube", "Sphere" };
        private string renderOnto ="Billboard";

        public string RenderOnto
        {
            get { return renderOnto; }
            set { renderOnto = value;
                switch (renderOnto)
                {
                    case "Billboard":
                        viewer.RenderOnto = Shapes.Billboard;
                        break;
                    case "Cube":
                        viewer.RenderOnto = Shapes.Cube;
                        break;
                    case "Sphere":
                        viewer.RenderOnto = Shapes.Sphere;
                        break;
                }
            }
        }

        public string[] RenderShapes
        {
            get { return renderShapes; }
        }

        public bool IsRotating
        {
            get { return viewer.IsRotating; }
            set { viewer.IsRotating = value; }
        }



        RenderEngines renderEngine;

        public RenderEngines RenderEngine
        {
            get { return renderEngine; }
            set { renderEngine = value; }
        }

        #region IDisplayContext Members

        public void CreateDisplay()
        {
            viewer.InitializeContexts();
            viewer.SetupDisplay();
        }

        public void TearDownDisplay()
        {
            // viewer.DestroyContexts();
            properties = null;
            viewer.Dispose();

        }

        public void UpdateNESScreen(int[] pixels)
        {
            viewer.UpdateNesScreen(pixels);
            // viewer.Draw();
        }

        public void DrawDefaultDisplay()
        {
            viewer.DrawDefaultDisplay();
        }

        public void SetPausedState(bool state)
        {
            viewer.SetPausedState(state);
        }

        NESPixelFormats pixelFormat;
        public NESPixelFormats PixelFormat
        {
            get
            {
                return pixelFormat;
            }
            set
            {
                pixelFormat = value;
            }
        }

        public System.Windows.UIElement UIControl
        {
            get { return this; }
        }


        public int PixelWidth
        {
            get { return 32; }
        }

        string properties;
        public string PropertiesPanel
        {
            get
            {
                if (properties == null)
                {
                    //XamlReader reader = new XamlReader();
                    TextReader reader = new StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("OpenGLNESViewer.OpenGLControls.xaml"));
                    properties = reader.ReadToEnd();
                }
                return properties;
            }
        }
        #endregion

        #region IDisplayContext Members


        public void UpdateNESScreen(int[] pixels, int[] palette)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisplayContext Members


        public string DisplayName
        {
            get { return "OpenGL"; }
        }

        #endregion
    }
}
