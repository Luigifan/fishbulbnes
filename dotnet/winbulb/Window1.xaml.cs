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
using System.Windows.Threading;
using System.IO;
using System.Windows.Interop;
using NES.CPU;
using NES.CPU.nitenedo;
using System.Runtime.InteropServices;
using WPFamicom.ControlPanelMVVM;
using WPFamicom.FileLoader;
using WPFamicom.Sound;
using WPFamicom.Input;
using WPFamicom.ControlPanelMVVM.CheatControls;


namespace WPFamicom
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public unsafe partial class Window1 : Window
    {
        private double top = 0, left = 0;
        
        NESMachine nes;
        ControlPanelVM controlPanel;
        DebuggerVM dvm;
        SoundThreader sndThread;
        SlimDXKeyboardControlPad keyboard;
        IntPtr vidBuffer;

        public Window1()
        {
            this.Loaded += new RoutedEventHandler(Window1_Loaded);
            InitializeComponent();

            nes = new NESMachine();

            //nesHost = nesDisplayer.Context;
            nesDisplayer.Target = nes;

            sndThread = new SoundThreader(nes);

            dvm = new DebuggerVM();
            dvm.DebugTarget = nes;

            whizzlerDebugger.DataContext = dvm;
            this.Closed += Window1_Closed;
            keyboard = new SlimDXKeyboardControlPad();
            nes.PadOne = keyboard;

            vidBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * 256 * 256);
            //nes.PPU.SetVideoBuffer(vidBuffer);
            nes.Drawscreen += nes_Drawscreen;


            Keyboard.AddPreviewKeyDownHandler(this, new KeyEventHandler(Window1_KeyDown));
            Keyboard.AddPreviewKeyUpHandler(this, new KeyEventHandler(Window1_KeyUp));
        }


        void nes_Drawscreen(object sender, EventArgs e)
        {
        }


        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            controlPanel = new ControlPanelVM(
                new ControlPanelModel(nes, sndThread.WavePlayer),
                nesDisplayer,
                new StateLoader(),
                new StateSaver(),
                new CheatControlVM(nes),
                theControlPanel
            );
            theControlPanel.DataContext = controlPanel;

        }

        void Window1_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = !nes.Paused;
            
        }

        void Window1_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !nes.Paused;
        }

        private BitmapPalette SetupNESPalette()
        {
            List<Color> colors = new List<Color>();

            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WPFamicom.bnes.pal"))
            {
                for (int n = 0; n < 64; ++n)
                {
                    byte r = (byte)stream.ReadByte();
                    byte g = (byte)stream.ReadByte();
                    byte b = (byte)stream.ReadByte();
                    colors.Add(Color.FromRgb(r, g, b));
                }
            }
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WPFamicom.bnes.pal"))
            {
                for (int n = 0; n < 64; ++n)
                {
                    byte r = (byte)stream.ReadByte();
                    byte g = (byte)stream.ReadByte();
                    byte b = (byte)stream.ReadByte();
                    colors.Add(Color.FromRgb(r, g, b));
                }
            }
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WPFamicom.bnes.pal"))
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
        
        void Window1_Closed(object sender, EventArgs e)
        {
            try
            {
                Marshal.FreeHGlobal(vidBuffer);
                Dispatcher.ExitAllFrames();
                controlPanel.Dispose();
                nes.Dispose();
                sndThread.Dispose();
                keyboard.Dispose();
                nesDisplayer.Dispose();
            }
            catch
            {
                Console.WriteLine("Cant close window");
            }
            finally
                {
                    
                }
            //nesHost.Dispose();
        }

        private delegate void NoArgDelegate();
        
        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MoveWindow(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            left += e.HorizontalChange;
             top += e.VerticalChange;
            this.Top = top;
            this.Left = left;
            e.Handled = true;
        }

        private void SetPosition(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            top = (double)this.Top;
            left = (double)this.Left;
        }

        private void ResizeThumbMoved(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.ActualWidth + e.HorizontalChange > 0)
                this.Width = this.ActualWidth + e.HorizontalChange;
            if (this.ActualHeight + e.VerticalChange > 0)
                this.Height = this.ActualHeight + e.VerticalChange;
        }

        private void ShowControlPanel(object sender, MouseEventArgs e)
        {
            theControlPanel.Visibility = Visibility.Visible;
        }

        private void ControlPanelResize(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width <= 10)
                (sender as ControlPanel).Visibility = Visibility.Hidden;
            else
                (sender as ControlPanel).Visibility = Visibility.Visible;
        }

    }
}
