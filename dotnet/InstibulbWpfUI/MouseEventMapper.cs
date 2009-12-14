using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;


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


    public class FakeEventThrower 
    {
        public event EventHandler<FakeEventArgs> FakeThisEvent;

        double[] drawArea = new double[4];
        public double[] DrawArea
        {
            get { return drawArea; }
            set { drawArea = value; }
        }

        public bool AllowingEvents
        {
            get;
            set;
        }

        List<FakedEventTypes> thrownTypes = new List<FakedEventTypes>();

        public List<FakedEventTypes> ThrownTypes
        {
            get { return thrownTypes; }
            set { thrownTypes = value; }
        }

        public void ThrowEvent(FakeEventArgs args)
        {
            if (AllowingEvents && FakeThisEvent != null && thrownTypes.Contains(args.EventType))
                FakeThisEvent(this, args);
        }
    }

    public class FakeEventMapper
    {

        float[] mousePosition = new float[2];
        Form form;
        bool allowEvents = false;

        public FakeEventMapper(Form form)
        {
            this.form = form;
            form.MouseMove += form_MouseMove;

        }


        List<FakeEventThrower> EventThrowers = new List<FakeEventThrower>();

        public void AddEventThrower(FakeEventThrower thrower)
        {
            EventThrowers.Add(thrower);
        }

        void ThrowFakeEvent(FakeEventArgs e)
        {
            Point pt = new Point((int)(e.X * form.ClientSize.Width), (int)(e.Y * form.ClientSize.Height));
            foreach (FakeEventThrower t in EventThrowers)
            {
                Rectangle rect = new Rectangle((int)(t.DrawArea[0] * form.ClientSize.Width), 
                                                (int)(t.DrawArea[1] * form.ClientSize.Height),
                                                (int)(t.DrawArea[2] * form.ClientSize.Width), 
                                                (int)(t.DrawArea[3] * form.ClientSize.Height));

                if (rect.Contains(pt) && t.AllowingEvents && t.ThrownTypes.Contains(e.EventType))
                {
                    // scale pt to e.DrawArea
                    e.X =  (double)(pt.X - rect.Left) / (double)rect.Width;
                    e.Y = (double)(pt.Y - rect.Top) / (double)rect.Height;
                    t.ThrowEvent(e);
                    return;
                }
            }
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
                form.KeyPress -= form_KeyPress;
                
                if (allowEvents)
                {
                    form.MouseClick += form_MouseClick;
                    form.MouseDoubleClick += form_MouseDoubleClick;
                    form.MouseDown += form_MouseDown;
                    form.MouseUp += form_MouseUp;
                    form.KeyPress += form_KeyPress;
                }
            }
        }
        

        void form_KeyPress(object sender, KeyPressEventArgs e)
        {
            ThrowFakeEvent(new FakeEventArgs()
            {
                EventType = FakedEventTypes.KEYPRESS,
                EventData = e.KeyChar,
                X = mousePosition[0],
                Y = mousePosition[1],
            });
        }

        void form_MouseUp(object sender, MouseEventArgs e)
        {
            ThrowFakeEvent( new FakeEventArgs()
            {
                EventType = FakedEventTypes.MOUSEUP,
                X = ((double)e.X) / ((double)form.ClientSize.Width),
                Y = ((double)e.Y) / ((double)form.ClientSize.Height),
            });
        }

        void form_MouseDown(object sender, MouseEventArgs e)
        {
            ThrowFakeEvent(
                new FakeEventArgs()
                {
                    EventType = FakedEventTypes.MOUSEDOWN,
                    X = ((double)e.X) / ((double)form.ClientSize.Width),
                    Y = ((double)e.Y) / ((double)form.ClientSize.Height),
                });
        }

        void form_MouseMove(object sender, MouseEventArgs e)
        {
            mousePosition[0] = ((float)e.X) / ((float)form.ClientSize.Width);
            mousePosition[1] = ((float)e.Y) / ((float)form.ClientSize.Height);
            if (allowEvents)
            {
                ThrowFakeEvent(
                new FakeEventArgs()
                {
                    EventType = FakedEventTypes.MOUSEMOVE,
                    X = ((double)e.X) / ((double)form.ClientSize.Width),
                    Y = ((double)e.Y) / ((double)form.ClientSize.Height),
                });
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
            ThrowFakeEvent( new FakeEventArgs()
                {
                    EventType = FakedEventTypes.MOUSEDOUBLECLICK,
                    X = ((double)e.X) / ((double)form.ClientSize.Width),
                    Y = ((double)e.Y) / ((double)form.ClientSize.Height),
                });
            
        }

        void form_MouseClick(object sender, MouseEventArgs e)
        {
            ThrowFakeEvent(new FakeEventArgs()
                {
                    EventType = FakedEventTypes.MOUSECLICK,
                    X = ((double)e.X) / ((double)form.ClientSize.Width),
                    Y = ((double)e.Y) / ((double)form.ClientSize.Height),
                });
        }
    }
}
