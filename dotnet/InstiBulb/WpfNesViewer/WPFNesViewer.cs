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

namespace WpfNESViewer
{
    [NESDisplayPluginAttribute]
    public class WPFNesViewer : Canvas, IDisplayContext
    {

        WriteableBitmap bitmap;
        #region IDisplayContext Members

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
            bitmap = new WriteableBitmap(256, 240, 96, 96, PixelFormats.Pbgra32,null);
            this.Background = new ImageBrush(bitmap);
            this.SnapsToDevicePixels = true;
        }

        public void TearDownDisplay()
        {
        }

        int stride = (256 * 32 + 7) / 8;

        byte[] pixArray = new byte[256 * 240];
        public void UpdateNESScreen(int[] pixels)
        {
            
            bitmap.WritePixels(new Int32Rect(0, 8, 256, 240), pixels, stride, 0, 0);
            if (isDefault)
            {
                this.Background = new ImageBrush(bitmap);
                isDefault = false;
            }
        }
        bool isDefault = true;

        public void DrawDefaultDisplay()
        {
            isDefault = true;
            this.Background = new SolidColorBrush(Color.FromArgb(0,0, 0, 0));
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


        string properties;
        public string PropertiesPanel
        {
            get
            {
                if (properties == null)
                {
                    //XamlReader reader = new XamlReader();
                    TextReader reader = new StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WpfNESViewer.WpfControls.xaml"));
                    properties = reader.ReadToEnd();
                }
                return properties;
            }
        }


        public void UpdateNESScreen(int[] pixels, int[] palette)
        {
            throw new NotImplementedException();
        }


        public string DisplayName
        {
            get { return "WPF WriteableBitmap"; }
        }

        #endregion

        #region IDisplayContext Members


        public void UpdateNESScreen(IntPtr pixelData)
        {
           
            bitmap.WritePixels(new Int32Rect(0, 8, 256, 240), pixelData, 256*240*8 , stride, 0, 0);
            
        }

        #endregion
    }
}
