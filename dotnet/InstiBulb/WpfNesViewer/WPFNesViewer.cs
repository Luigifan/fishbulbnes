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
using InstiBulb.Effects;
using NES.CPU.nitenedo;
using NES.CPU.PPUClasses;

namespace WpfNESViewer
{
    [NESDisplayPluginAttribute]
    public class WPFNesViewer : Canvas, IDisplayContext
    {
        public WPFNesViewer()
        {
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            UpdateNESScreen();
        }

        

        private NESMachine nes;
        public WPFNesViewer(NESMachine nes)
        {
            this.nes = nes;
        }

        WriteableBitmap bitmap;
        WriteableBitmap palette;
        WriteableBitmap nesPalette;

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

        //RasterizeEffect rasterize = new RasterizeEffect();

        ImageBrush bmpBrush;
        public void CreateDisplay()
        {
            palette = new WriteableBitmap(256, 1, 96, 96, PixelFormats.Pbgra32, null);
            palette.WritePixels(new Int32Rect(0,0,256,1), nes.PPU.LoadPalABGR(), stride, 0);
            bitmap = new WriteableBitmap(256, 256, 96, 96, PixelFormats.Pbgra32,null);

            nesPalette = new WriteableBitmap(32, 1, 96, 96, PixelFormats.Pbgra32, null);
            bmpBrush = new ImageBrush(bitmap);
            this.Background = bmpBrush;

        }

        public void TearDownDisplay()
        {
        }

        int stride = (256 * 32 + 7) / 8;

        byte[] pixArray = new byte[256 * 240];
        public void UpdateNESScreen(int[] pixels)
        {
            bitmap.WritePixels(new Int32Rect(0, 0, 256, 256), pixels, stride, 0, 0);
            ////  nesPalette.WritePixels(new Int32Rect(0, 0, 32, 1), nes.PPU.Palette, (32 * 32 + 7) / 8, 0); 
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
        public object PropertiesPanel
        {
            get
            {
                //if (properties == null)
                //{
                //    //XamlReader reader = new XamlReader();
                //    TextReader reader = new StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WpfNESViewer.WpfControls.xaml"));
                //    properties = reader.ReadToEnd();
                //}
                return null;
            }
        }


        public string DisplayName
        {
            get { return "WPF WriteableBitmap"; }
        }

        public void UpdateNESScreen(IntPtr pixelData)
        {
           
            bitmap.WritePixels(new Int32Rect(0, 8, 256, 240), pixelData, 256*240*8 , stride, 0, 0);
            
        }


        #region IDisplayContext Members

        public CallbackType DesiredCallback
        {
            get { return CallbackType.Array; }
        }

        public void UpdateNESScreen()
        {
            bitmap.WritePixels(new Int32Rect(0, 0, 256, 256), nes.PPU.VideoBuffer, stride, 0, 0);
            //  nesPalette.WritePixels(new Int32Rect(0, 0, 32, 1), nes.PPU.Palette, (32 * 32 + 7) / 8, 0); 
            if (isDefault)
            {
                this.Background = new ImageBrush(bitmap);
                isDefault = false;
            }
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
            }
        }
    }
}
