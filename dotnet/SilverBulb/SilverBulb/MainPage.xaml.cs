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

        WriteableBitmap bmp = new WriteableBitmap(256, 240);

        volatile bool nesUpdated = false;

        ControlPanelVM controlVM;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            NES.CPU.FastendoDebugging.DisassemblyExtensions.SetupOpcodes();
            
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
            
            // src.MediaSource.TargetMachine = nes;
            
            NintendoSound.SetSource( src.MediaSource);

            var pad = container.Resolve<IControlPad>("padone") as SilverlightControlPad;
            if (pad != null)
            {
                pad.BoundControl = this;
            }

            nes.PPU.FillRGB = false;
            // nes.PPU.LoadPalRGBA();
            nes.PPU.SetVideoBuffer( bmp.Pixels);

            nes.RunStatusChangedEvent += new EventHandler<EventArgs>(nes_RunStatusChangedEvent);
            nes.Drawscreen += new EventHandler(nes_Drawscreen);

            controlVM = new ControlPanelVM(new PlatformDelegates().BrowseForFile);
            controlVM.TargetMachine = nes;
            controlVM.Dispatcher = this.Dispatcher;
            this.ControlPanel.DataContext = controlVM;

            nes.SilverlightStartDemo();
        }

        void nes_RunStatusChangedEvent(object sender, EventArgs e)
        {

            if (nes.RunState != NES.Machine.ControlPanel.RunningStatuses.Running)
            {
                Dispatcher.BeginInvoke(Pause);
            }
            else
            {
                Dispatcher.BeginInvoke(Play);
            }

        }

        void Pause()
        {
            NintendoSound.Pause();
            CompositionTarget.Rendering -= new EventHandler(CompositionTarget_Rendering);
        }

        void Play()
        {
            NintendoSound.Play();
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }


        void CompositionTarget_Rendering(object o, EventArgs e)
        {
            if (nesUpdated)
                bmp.Invalidate();
        }


        void nes_Drawscreen(object sender, EventArgs e)
        {
            nesUpdated = true;
        }

        private void DrawArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ControlPane.Visibility == System.Windows.Visibility.Visible)
            {
                ControlPane.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                ControlPane.Visibility = System.Windows.Visibility.Visible;
            }
        }


    }
}
