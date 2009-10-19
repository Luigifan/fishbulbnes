using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using NES.CPU.nitenedo;
using NES.CPU.nitenedo.Interaction;
using System.Windows.Threading;
using WpfNESViewer;

namespace InstiBulb.Integration
{
    public class NESDisplay : Border, IDisposable
    {

        public static DependencyProperty DisplayContextProperty = DependencyProperty.Register("Context", typeof(IDisplayContext), typeof(NESDisplay), new PropertyMetadata(null, new PropertyChangedCallback(DisplayContextChanged)));
        public static DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(NESMachine), typeof(NESDisplay), new PropertyMetadata(null, new PropertyChangedCallback(TargetChanged)));

        public static DependencyProperty SuspendNESDisplayProperty = DependencyProperty.Register("SuspendNESDisplay", typeof(bool), typeof(NESDisplay), new PropertyMetadata(false, new PropertyChangedCallback(SuspendNESDisplayChanged)));

        private delegate void NoArgDelegate();

        private IDisplayContext displayContext;

        public NESDisplay()
            : base()
        {
            doTheDraw = new NoArgDelegate(DrawScreen);
        }

        bool _suspendNESDisplay = false;

        internal void StopDisplaying()
        {
            if (Target != null)
                Target.Drawscreen -= target_Drawscreen;
        }

        internal void StartDisplaying()
        {
            if (Target != null)
                Target.Drawscreen += target_Drawscreen;
        }

        public bool SuspendNESDisplay
        {
            get { return (bool)GetValue( NESDisplay.SuspendNESDisplayProperty); }
            set { SetValue(NESDisplay.SuspendNESDisplayProperty, value); }
        }

        static void SuspendNESDisplayChanged(DependencyObject o, DependencyPropertyChangedEventArgs arg)
        {
            if ((bool)arg.NewValue)
                ((NESDisplay)o).StopDisplaying();
            else
                ((NESDisplay)o).StartDisplaying();

        }

        static void DisplayContextChanged(DependencyObject o, DependencyPropertyChangedEventArgs arg)
        {
            (o as NESDisplay).SetupRenderer(arg.NewValue as IDisplayContext);
        }

        static void TargetChanged(DependencyObject o, DependencyPropertyChangedEventArgs arg)
        {
            var p = o as NESDisplay;
            if (p == null) return;
            p.UnhookTarget(arg.OldValue as NESMachine);
            p.UpdateTarget(arg.NewValue as NESMachine);
            if (p.Context != null)
            {
                p.SetupRenderer(p.Context);
            }
            
        }

        public void DestroyContext()
        {
            if (displayContext != null)
            {
                displayContext.TearDownDisplay();
                displayContext = null;
                Target.Drawscreen -= target_Drawscreen;
            }
        }

        public event EventHandler ContextChanged;

        internal void UpdateContext()
        {
            if (ContextChanged != null) ContextChanged(this, EventArgs.Empty);
        }

        void target_Drawscreen(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(doTheDraw, DispatcherPriority.Normal, null);
        }

        public NESMachine Target
        {
            get { return (NESMachine)GetValue(NESDisplay.TargetProperty); }
            set {
                SetValue(NESDisplay.TargetProperty, value);
                
            }
        }

        internal void UpdateTarget(NESMachine target)
        {
            if (target != null)
            target.Drawscreen += target_Drawscreen;
        }

        internal void UnhookTarget(NESMachine target)
        {
            //if (target != null)
            //target.Drawscreen -= target_Drawscreen;
        }

        Delegate doTheDraw;

        void DrawScreenArray()
        {
            //if (displayContext.PixelWidth ==32)
                displayContext.UpdateNESScreen(Target.PPU.VideoBuffer);
        }

        void DrawScreen()
        {
            //if (displayContext.PixelWidth ==32)
            displayContext.UpdateNESScreen();
        }

        void DrawScreenPtr()
        {
            //if (displayContext.PixelWidth ==32)
            displayContext.UpdateNESScreen(new IntPtr(Target.PPU.VideoBuffer[0]) );
        }


        public void SetupRenderer(IDisplayContext displayContext)
        {
            if (displayContext.UIControl as UIElement != null)
            {
                this.Child = displayContext.UIControl as UIElement;

                this.Child.SetValue(UIElement.IsEnabledProperty, true);
                this.Child.SetValue(UIElement.VisibilityProperty, Visibility.Visible);

                this.displayContext = displayContext;
                this.displayContext.CreateDisplay();
                this.displayContext.DrawDefaultDisplay();
                UpdateContext();
                if (Target != null)
                {
                    Target.PPU.PixelWidth = displayContext.PixelWidth;
                    Target.PPU.PixelWidth = displayContext.PixelWidth;

                    Target.PPU.LoadPalABGR();
                    if (displayContext.PixelFormat == NES.CPU.nitenedo.Interaction.NESPixelFormats.RGB)
                    {
                        Target.PPU.LoadPalRGBA();

                    }

                    if (displayContext.PixelFormat == NESPixelFormats.Indexed)
                    {
                        Target.PPU.FillRGB = false;
                    }
                    else
                    {
                        Target.PPU.FillRGB = true;
                    }
                }

                switch (displayContext.DesiredCallback)
                {
                    case CallbackType.None:
                        break;
                    case CallbackType.NoArgs:
                        doTheDraw = new NoArgDelegate(DrawScreen);
                        break;
                    case CallbackType.Array:
                        doTheDraw = new NoArgDelegate( DrawScreenArray);
                        break;
                    case CallbackType.IntPtr:
                        doTheDraw = new NoArgDelegate(DrawScreenPtr);
                        break;
                }
            }
        }

        public IDisplayContext Context
        {
            get { return (IDisplayContext)GetValue(NESDisplay.DisplayContextProperty); }
            set 
            { 
                SetValue(NESDisplay.DisplayContextProperty, value);
            }
        }



        public void Dispose()
        {
            if (displayContext != null)
                displayContext.TearDownDisplay();
        }
    }
}
