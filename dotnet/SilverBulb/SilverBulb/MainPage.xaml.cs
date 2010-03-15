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
using SilverlightBindings.ViewModels;
using System.ComponentModel;
using System.Windows.Browser;
using SilverlightCommonUI.ScriptViews;

namespace SilverBulb
{
    public partial class MainPage : UserControl, INotifyPropertyChanged
    {
        IUnityContainer container;

        NESMachine nes;

        System.Windows.Interop.Settings settings;
        System.Windows.Interop.SilverlightHost host;

        ScriptControlPanelVM scriptView;
            
        public MainPage()
        {

            host = Application.Current.Host;

            settings = host.Settings;



            InitializeComponent();
        }

        public int FPS
        {
            get
            {
                return settings.MaxFrameRate;
            }
            set
            {
                settings.MaxFrameRate = value;
                NotifyPropertyChanged("FPS");
                
            }
        }

        WriteableBitmap bmp = new WriteableBitmap(256, 240);

        volatile bool nesUpdated = false;

        ControlPanelVM controlVM;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            NES.CPU.FastendoDebugging.DisassemblyExtensions.SetupOpcodes();

            //ImageBrush brush = new ImageBrush();
            //brush.ImageSource = bmp;
            NesOut.Source = bmp;
            // DrawArea.Background = brush;
            container = new UnityContainer();
            new UnityRegistration().RegisterNesTypes(container, "soft");


            this.ToolBar.DataContext = new ToolstripViewModel(container, this.Dispatcher);

            scriptView = container.Resolve<IViewModel>("ScriptControlPanel") as ScriptControlPanelVM;

            nes = container.Resolve<NESMachine>();

            var src = container.Resolve<IWavStreamer>() as SilverlightWavStreamer;
            if (src != null)
            {
                src.MediaHost = NintendoSound;
                // src.MediaSource.TargetMachine = nes;
                NintendoSound.SetSource(src.MediaSource);
            }

            var pad = container.Resolve<IControlPad>("padone") as SilverlightControlPad;
            if (pad != null)
            {
                pad.BoundControl = this;
            }

            nes.PPU.FillRGB = false;
            // nes.PPU.LoadPalRGBA();
            nes.PPU.SetVideoBuffer(bmp.Pixels);

            nes.RunStatusChangedEvent += new EventHandler<EventArgs>(nes_RunStatusChangedEvent);
            nes.Drawscreen += new EventHandler(nes_Drawscreen);

            //WebClient client = new WebClient();
            ////client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            //client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            //UriKind kind = UriKind.Relative;
            //string resName = null;

            if (host.InitParams.ContainsKey("ShowControls"))
            {
                string val = host.InitParams["ShowControls"];
                bool showControls = true;
                if (bool.TryParse(val, out showControls))
                {
                    
                }

                if (!showControls)
                {
                    ToolBar.Visibility = Visibility.Collapsed;
                    NesBorder.SetValue(Grid.ColumnSpanProperty, 2);
                }
            }

            //if (host.InitParams.ContainsKey("Cart"))
            //{
            //    resName = host.InitParams["Cart"];
            //}

            //if (resName != null)
            //    client.OpenReadAsync(new Uri(resName, kind));


            //nes.SilverlightStartDemo();
        }

//        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
//        {
//            if (e.Error == null)
//            {
//                nes.LoadCart(e.Result);
//            }
//            else
//            {

////                e.Error.Message;
//            }
//        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                
            }

            //throw new NotImplementedException();
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


        double vol = 1.0;
        void Pause()
        {
            NintendoSound.Volume = 0;
            CompositionTarget.Rendering -= new EventHandler(CompositionTarget_Rendering);
        }

        void Play()
        {
            NintendoSound.Volume = vol;
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }

        void CompositionTarget_Rendering(object o, EventArgs e)
        {
            if (nesUpdated)
            {
                bmp.Invalidate();
            }
        }


        void nes_Drawscreen(object sender, EventArgs e)
        {
            nesUpdated = true;
        }

        private void DrawArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ToolBar.Visibility == System.Windows.Visibility.Visible)
            {
                ToolBar.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                ToolBar.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void EnableControls_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void FullScreen_Checked(object sender, RoutedEventArgs e)
        {
        }

        void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
