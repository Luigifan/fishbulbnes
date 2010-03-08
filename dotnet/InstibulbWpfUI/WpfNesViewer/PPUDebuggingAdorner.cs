using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using NES.CPU.PPUClasses;
using Fishbulb.Common.UI;
using System.Windows.Media;
using System.Windows;
using NES.CPU.nitenedo;

namespace InstiBulb.WpfNESViewer
{
    public class PPUDebuggingAdorner : Adorner
    {

        private NESMachine machine;
        public NES.CPU.nitenedo.NESMachine AttachedMachine
        {
            get
            {
                return machine;
            }
            set
            {
                machine = value;
                machine.DebugInfoChanged += new EventHandler<NES.CPU.Machine.FastendoDebugging.BreakEventArgs>(machine_DebugInfoChanged);

            }
        }

        public PPUDebuggingAdorner(UIElement adornedElement) : base(adornedElement)
        {
            
        }

        void machine_DebugInfoChanged(object sender, NES.CPU.Machine.FastendoDebugging.BreakEventArgs e)
        {
            shouldIAdorn = false;
            events = (from p in machine.DebugInfo.PPU.FrameWriteEvents where p.ScanlineNum > 20 && p.ScanlinePos < 256 select p).ToList();
            if (events.Count > 0)
                shouldIAdorn = true;
        }

        bool shouldIAdorn = false;
        List<PPUWriteEvent> events;

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            if (shouldIAdorn)
            {
                // Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

                double width = this.ActualWidth;
                double height = this.ActualHeight;
                
                Brush yellowBrush = new SolidColorBrush(Colors.Yellow);
                Brush blackBrush = new SolidColorBrush(Colors.Blue);
                
                
                                             
                foreach (PPUWriteEvent ev in events )
                {
                    double x =  (ev.ScanlinePos  * width) / 256.0;
                    //x += adornedElementRect.Left;
                    double y = ((ev.ScanlineNum - 21) * height) / 256.0;
                    //y += adornedElementRect.Right;
                    // drawingContext.DrawRectangle(yellowBrush, new Pen(blackBrush, 1), new Rect(new Point(x, y), new Size(20, 20)));
                    drawingContext.DrawEllipse(yellowBrush, new Pen(blackBrush, 1), new Point(x, y), width/128, height/128);
                    drawingContext.DrawText(new FormattedText(ev.ToString(), System.Globalization.CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, new Typeface("Courier"), 12, yellowBrush), new Point(x, y));
                }
                //shouldIAdorn = false;
            }
            // base.OnRender(drawingContext);
        }

    }
}
