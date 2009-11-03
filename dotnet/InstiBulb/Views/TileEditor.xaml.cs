using System;
using System.Collections.Generic;
using System.Linq;
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

namespace InstiBulb.Views
{
    /// <summary>
    /// Interaction logic for TileEditor.xaml
    /// </summary>
    public partial class TileEditor : UserControl
    {
        Border[,] grid = new Border[8,8];

        public TileEditor()
        {

            InitializeComponent();

            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    grid[i, j] = new Border();
                    grid[i, j].SetValue(Grid.RowProperty, i);
                    grid[i, j].SetValue(Grid.ColumnProperty, j);
                    grid[i, j].SetValue(Border.BackgroundProperty, new SolidColorBrush(Colors.DarkRed));
                    grid[i, j].SetValue(Border.BorderBrushProperty, new SolidColorBrush(Colors.Black));
                    grid[i, j].SetValue(Border.BorderThicknessProperty, new Thickness(1.0));
                    TileLayout.Children.Add(grid[i, j]);
                }
            }
        }
    }
}
