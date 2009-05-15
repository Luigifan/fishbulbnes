using System;
using System.Collections.Generic;
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

namespace WPFamicom
{
	/// <summary>
	/// Interaction logic for PPUWriteViewer.xaml
	/// </summary>
	public partial class PPUWriteViewer
	{
		public PPUWriteViewer()
		{
			this.InitializeComponent();
		}

        // draws a line with an arrow from the pixel x, y in this gris col 0 (scaled onto 341x262) 
        // 
        public void ArrowToPixel(int x, int y, int listIndex)
        {
            Line line = new Line();

        }
	}
}