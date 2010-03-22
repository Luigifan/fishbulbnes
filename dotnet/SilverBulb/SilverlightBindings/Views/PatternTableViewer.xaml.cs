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
using System.ComponentModel;
using SilverlightCommonUI.ControlPanel;
using System.Windows.Media.Imaging;

namespace SilverlightBindings.Views
{
    public partial class PatternTableViewer : CommandingUserControl
    {
        public PatternTableViewer()
        {
            InitializeComponent();
            BindButtons(this.LayoutRoot as Panel);
        }

        protected override void OnCommandSent(string name, object param)
        {
            var p = this.DataContext as PatternTablesViewModel;
            if (p != null)
            {
                switch (name)
                {
                    case "RefreshPatternTable":
                        WritePatternTables(p);
                        break;
                    case "RefreshNameTable":
                        WriteNameTables(p);
                        break;

                }
            }
        }

        private void WritePatternTables(PatternTablesViewModel p)
        {
            WriteableBitmap bmp1 = new WriteableBitmap(128, 128);
            WriteableBitmap bmp2 = new WriteableBitmap(128, 128);
            Array.Copy(p.PatternTables[0], bmp1.Pixels, 128 * 128);
            Array.Copy(p.PatternTables[1], bmp2.Pixels, 128 * 128);
            PatternTableZero.Source = bmp1;
            PatternTableOne.Source = bmp2;
        }

        private void WriteNameTables(PatternTablesViewModel p)
        {
            WriteableBitmap bmp1 = new WriteableBitmap(256, 256);
            WriteableBitmap bmp2 = new WriteableBitmap(256, 256);
            WriteableBitmap bmp3 = new WriteableBitmap(256, 256);
            WriteableBitmap bmp4 = new WriteableBitmap(256, 256);

            Array.Copy(p.NameTables[0], bmp1.Pixels, 256 * 240);
            Array.Copy(p.NameTables[1], bmp2.Pixels, 256 * 240);
            Array.Copy(p.NameTables[2], bmp3.Pixels, 256 * 240);
            Array.Copy(p.NameTables[3], bmp4.Pixels, 256 * 240);
            NameTableZero.Source = bmp1;
            NameTableOne.Source = bmp2;
            NameTableTwo.Source = bmp3;
            NameTableThree.Source = bmp4;
        }
    }
}
