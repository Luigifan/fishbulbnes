using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDXBindings.Viewer10.Filter;

namespace SlimDXBindings.Viewer10.Helpers
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

    public class FakeEvent
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

        Form form;
        FilterChain chain;
        bool allowEvents = false;


        public FakeEventMapper(Form form, FilterChain chain)
        {
            this.form = form;
            this.chain = chain;

        }

        public bool AllowEvents
        {
            get { return allowEvents; }
            set { allowEvents = value;
                form.MouseClick -= form_MouseClick;
                form.MouseDoubleClick -= form_MouseDoubleClick;
                form.MouseMove -= form_MouseMove;
                form.MouseDown -= form_MouseDown;
                form.MouseUp -= form_MouseUp; 
                if (allowEvents)
                {
                    form.MouseClick += form_MouseClick;
                    form.MouseDoubleClick += form_MouseDoubleClick;
                    form.MouseMove += form_MouseMove;
                    form.MouseDown += form_MouseDown;
                    form.MouseUp += form_MouseUp;
                }
            }
        }

        void form_MouseUp(object sender, MouseEventArgs e)
        {
            var p = new FakeEvent()
            {
                EventType = FakedEventTypes.MOUSEDOWN,
                X = ((double)e.X) / ((double)form.Width),
                Y = ((double)e.Y) / ((double)form.Height),
            };
            chain.ProcessEvent(p);
        }

        void form_MouseDown(object sender, MouseEventArgs e)
        {

            var p = new FakeEvent()
            {
                EventType = FakedEventTypes.MOUSEDOWN,
                X = ((double)e.X) / ((double)form.Width),
                Y = ((double)e.Y) / ((double)form.Height),
            };
            chain.ProcessEvent(p);
        }

        void form_MouseMove(object sender, MouseEventArgs e)
        {
            var p = new FakeEvent()
            {
                EventType = FakedEventTypes.MOUSEMOVE,
                X = ((double)e.X) / ((double)form.Width),
                Y = ((double)e.Y) / ((double)form.Height),
            };
            chain.ProcessEvent(p);
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
            var p = new FakeEvent()
            {
                EventType = FakedEventTypes.MOUSEDOUBLECLICK,
                X = ((double)e.X) / ((double)form.Width),
                Y = ((double)e.Y) / ((double)form.Height),
            };
            chain.ProcessEvent(p);
        }

        void form_MouseClick(object sender, MouseEventArgs e)
        {

            var p = new FakeEvent()
            {
                EventType = FakedEventTypes.MOUSECLICK,
                X = ((double)e.X) / ((double)form.Width),
                Y = ((double)e.Y) / ((double)form.Height),
            };
            chain.ProcessEvent(p);
        }
    }
}
