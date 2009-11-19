using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace InstibulbWpfUI
{

    public enum FakedEventTypes
    {
        MOUSECLICK,
        MOUSEDOUBLECLICK,
        MOUSEDOWN,
        MOUSEUP,
        MOUSEMOVE,
        MOUSEENTER,
        MOUSELEAVE,
        KEYPRESS
    }

    public class FakeEventArgs : EventArgs
    {
        public FakedEventTypes EventType
        {
            get; set;
        }
        



        /// <summary>
        /// For mouse events, 1 = left mouse button, 2 = right mouse button, 4 = middle mouse button, 8 = extended 1, 16 = extended 2
        /// For keyboard events, the int corresponding to the key event based Wpf Keys enum
        /// </summary>
        public int EventData
        {
            get;
            set;
        }

        public double X
        {
            get;
            set;
        }

        public double Y
        {
            get;
            set;
        }
    }

    public class FakeEventMapper
    {

        float[] mousePosition = new float[2];
        Form form;
        bool allowEvents = false;

        public event EventHandler<FakeEventArgs> FakeThisEvent;

        public FakeEventMapper(Form form)
        {
            this.form = form;
            form.MouseMove += form_MouseMove;

        }

        public float[] MousePosition
        {
            get { return mousePosition; }
        }


        public bool AllowEvents
        {
            get { return allowEvents; }
            set { allowEvents = value;
                form.MouseClick -= form_MouseClick;
                form.MouseDoubleClick -= form_MouseDoubleClick;
                form.MouseDown -= form_MouseDown;
                form.MouseUp -= form_MouseUp; 
                if (allowEvents)
                {
                    form.MouseClick += form_MouseClick;
                    form.MouseDoubleClick += form_MouseDoubleClick;
                    form.MouseDown += form_MouseDown;
                    form.MouseUp += form_MouseUp;
                }
            }
        }

        void form_MouseUp(object sender, MouseEventArgs e)
        {
            if (FakeThisEvent != null)
            {
                var p = new FakeEventArgs()
                {
                    EventType = FakedEventTypes.MOUSEDOWN,
                    X = ((double)e.X) / ((double)form.Width),
                    Y = ((double)e.Y) / ((double)form.Height),
                };
                FakeThisEvent(this, p);
            }
            
        }

        void form_MouseDown(object sender, MouseEventArgs e)
        {
            if (FakeThisEvent != null)
            {
                var p = new FakeEventArgs()
                {
                    EventType = FakedEventTypes.MOUSEDOWN,
                    X = ((double)e.X) / ((double)form.Width),
                    Y = ((double)e.Y) / ((double)form.Height),
                };
                FakeThisEvent(this, p);
            }
        }

        void form_MouseMove(object sender, MouseEventArgs e)
        {
            if (FakeThisEvent != null)
            {
                mousePosition[0] = ((float)e.X) / ((float)form.Width);
                mousePosition[1] = ((float)e.Y) / ((float)form.Height);
                if (allowEvents)
                {
                    var p = new FakeEventArgs()
                    {
                        EventType = FakedEventTypes.MOUSEMOVE,
                        X = ((double)e.X) / ((double)form.Width),
                        Y = ((double)e.Y) / ((double)form.Height),
                    };
                    FakeThisEvent(this, p);
                }
            }
        }

        //void form_MouseLeave(object sender, EventArgs e)
        //{
        //    var p = new FakeEvent()
        //    {
        //        EventType = FakedEventTypes.MOUSELEAVE,
        //        X = 0,
        //        Y = 0,
        //    };
        //    chain.ProcessEvent(p);
        //}

        //void form_MouseEnter(object sender, EventArgs e)
        //{
        //    var p = new FakeEvent()
        //    {
        //        EventType = FakedEventTypes.MOUSEENTER,
        //        X = 0,
        //        Y = 0,
        //    };
        //    chain.ProcessEvent(p);
        //}

        void form_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (FakeThisEvent != null)
            {
                var p = new FakeEventArgs()
                {
                    EventType = FakedEventTypes.MOUSEDOUBLECLICK,
                    X = ((double)e.X) / ((double)form.Width),
                    Y = ((double)e.Y) / ((double)form.Height),
                };
                FakeThisEvent(this, p);
            }
        }

        void form_MouseClick(object sender, MouseEventArgs e)
        {
            if (FakeThisEvent != null)
            {
                var p = new FakeEventArgs()
                {
                    EventType = FakedEventTypes.MOUSECLICK,
                    X = ((double)e.X) / ((double)form.Width),
                    Y = ((double)e.Y) / ((double)form.Height),
                };
                FakeThisEvent(this, p);
            }
        }
    }
}
