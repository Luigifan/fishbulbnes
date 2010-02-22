using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace InstibulbWpfUI.ToolStrip
{
    public class MenuItemVM
    {

        public MenuItemVM()
        {
            visualIcon = new Border();
        }

        private UIElement visualIcon;

        public string ItemName
        {
            get;
            set;
        }

        public UIElement VisualIcon
        {
            get
            {
                return visualIcon;
            }
        }


        
    }
}
