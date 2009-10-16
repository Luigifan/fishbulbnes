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

namespace SlimDXNESViewer
{
    [NESDisplayPluginAttribute]
    public class SlimDXNesViewer : Border, IDisplayContext
    {
        private D3DPanel panel;

        #region IDisplayContext Members


        public SlimDXNesViewer()
        {

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
            panel = new D3DPanel();
            this.Child = panel;
            panel.StartThread = true;
            panel.SetupDisplay();
        }

        private BitmapPalette SetupNESPalette()
        {
            List<Color> colors = new List<Color>();

            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WpfNESViewer.bnes.pal"))
            {
                for (int n = 0; n < 64; ++n)
                {
                    byte r = (byte)stream.ReadByte();
                    byte g = (byte)stream.ReadByte();
                    byte b = (byte)stream.ReadByte();
                    colors.Add(Color.FromRgb(r, g, b));
                }
            }
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WpfNESViewer.bnes.pal"))
            {
                for (int n = 0; n < 64; ++n)
                {
                    byte r = (byte)stream.ReadByte();
                    byte g = (byte)stream.ReadByte();
                    byte b = (byte)stream.ReadByte();
                    colors.Add(Color.FromRgb(r, g, b));
                }
            }
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WpfNESViewer.bnes.pal"))
            {
                for (int n = 0; n < 64; ++n)
                {
                    byte r = (byte)stream.ReadByte();
                    byte g = (byte)stream.ReadByte();
                    byte b = (byte)stream.ReadByte();
                    colors.Add(Color.FromRgb(r, g, b));
                }
            }
            return new BitmapPalette(colors);
        }

        public void TearDownDisplay()
        {
            panel.ReleaseD3D();
            panel = null;
            this.Child = null;
        }

        int stride = (256 * 8 + 7) / 8;

        byte[] pixArray = new byte[256 * 256];
        public void UpdateNESScreen(int[] pixels)
        {
            panel.UpdatePixBuf(pixels);
            panel.Draw();
            
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

        #endregion

        #region IDisplayContext Members


        public int PixelWidth
        {
            get { return 8; }
        }

        #endregion

        #region IDisplayContext Members


        string properties;
        public string PropertiesPanel
        {
            get
            {
                //if (properties == null)
                //{
                //    //XamlReader reader = new XamlReader();
                //    TextReader reader = new StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WpfNESViewer.WpfControls.xaml"));
                //    properties = reader.ReadToEnd();
                //}
                return properties;
            }
        }
        #endregion

        #region IDisplayContext Members


        public void UpdateNESScreen(int[] pixels, int[] palette)
        {
            
        }

        public string DisplayName
        {
            get { return "D3DImage(SlimDX)"; }
        }

        #endregion


        public void UpdateNESScreen(IntPtr pixelData)
        {
            
        }
    }
}
