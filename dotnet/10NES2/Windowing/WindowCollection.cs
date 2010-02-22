using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace InstiBulb.Windowing
{
    public class PopupWindowCollection : Dictionary<string, Window>
    {
        
        public void RegisterWindow (string s, Window w)
        {
            base.Add(s, w);
            w.Name = s;
            w.Closed += new EventHandler(w_Closed);
        }

        void w_Closed(object sender, EventArgs e)
        {
            var win = sender as Window;
            if (win != null)
                Remove(win.Name);
        }

    }
}
