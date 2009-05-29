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

namespace WPFamicom.GameDisplay
{
    public class NESDisplay : Border, IDisposable
    {
        private delegate void NoArgDelegate();

        private IDisplayContext displayContext;

        public NESDisplay()
            : base()
        {
            doTheDraw = new NoArgDelegate(DrawScreen);
        }

        public void DestroyContext()
        {
            if (displayContext != null)
            {
                displayContext.TearDownDisplay();
                displayContext = null;
                target.Drawscreen -= target_Drawscreen;
            }
        }

        private NESMachine target;
        public NESMachine Target
        {
            get { return target; }
            set { target = value;
            }
        }

        void target_Drawscreen(object sender, EventArgs e)
        {

            Dispatcher.BeginInvoke(doTheDraw, DispatcherPriority.Normal, null);

        }

        Delegate doTheDraw;

        void DrawScreen()
        {
            //if (displayContext.PixelWidth ==32)
                displayContext.UpdateNESScreen(target.PPU.VideoDataPtr);
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
                target.Drawscreen += target_Drawscreen;
            }

        }

        public IDisplayContext Context
        {
            get { return displayContext; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (displayContext != null)
                displayContext.TearDownDisplay();
        }

        #endregion
    }
}
