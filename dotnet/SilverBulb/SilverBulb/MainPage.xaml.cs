using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using NES.CPU.nitenedo;
using Microsoft.Practices.Unity;
using NES.Sound;
using SilverlightBindings;

namespace SilverBulb
{
    public partial class MainPage : UserControl
    {
        IUnityContainer container;

        NESMachine nes;

        public MainPage()
        {
            InitializeComponent();
        }

        WriteableBitmap bmp = new WriteableBitmap(256, 256);

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = bmp;
            DrawArea.Background = brush;
            container = new UnityContainer();
            new UnityRegistration().RegisterNesTypes(container, "soft");
            
            nes = container.Resolve<NESMachine>();
            var src = container.Resolve<IWavStreamer>() as SilverlightWavStreamer;
            if (src == null)
            {
                return;
            }
            NintendoSound.SetSource( src.MediaSource);

            nes.PPU.FillRGB = true;
            
            nes.Drawscreen += new EventHandler(nes_Drawscreen);
            nes.StartDemo();
            

        }

        void nes_Drawscreen(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(Draw));
        }

        void Draw()
        {
            for (int i = 0; i < nes.PPU.VideoBuffer.Length; ++i)
            {
                bmp.Pixels[i] = nes.PPU.VideoBuffer[i];
            }

            bmp.Invalidate();

        }

    }
}
