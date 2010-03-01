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
using NES.CPU.Machine;
using Fishbulb.Common.UI;

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

        bool nesUpdated = false;

        ControlPanelVM controlVM;

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

            var pad = container.Resolve<IControlPad>("padone") as SilverlightControlPad;
            if (pad != null)
            {
                pad.BoundControl = this;
            }

            nes.PPU.FillRGB = false;
            nes.PPU.SetVideoBuffer( bmp.Pixels);

            nes.RunStatusChangedEvent += new EventHandler<EventArgs>(nes_RunStatusChangedEvent);
            nes.Drawscreen += new EventHandler(nes_Drawscreen);

            //CompositionTarget.Rendering += CompositionTarget_Rendering;
            controlVM = new ControlPanelVM(new PlatformDelegates().BrowseForFile);
            controlVM.TargetMachine = nes;
            controlVM.Dispatcher = this.Dispatcher;
            this.ControlPanel.DataContext = controlVM;
        }

        void nes_RunStatusChangedEvent(object sender, EventArgs e)
        {

            if (nes.RunState != NES.Machine.ControlPanel.RunningStatuses.Running)
            {
                Dispatcher.BeginInvoke(NintendoSound.Pause);
            }
            else
            {
                Dispatcher.BeginInvoke( NintendoSound.Play);
            }

        }

        void nes_Drawscreen(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(bmp.Invalidate);    
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            nes.SilverlightStart();
        }

    }
}
