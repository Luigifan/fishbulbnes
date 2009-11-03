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
using InstiBulb.WinViewModels;

namespace InstiBulb.Views
{
    /// <summary>
    /// Interaction logic for TileEditor.xaml
    /// </summary>
    public partial class TileEditor : UserControl
    {
        Label[,] grid = new Label[8, 8];
        TextBlock[] labels = new TextBlock[8];

        public TileEditor()
        {

            InitializeComponent();

            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    grid[i, j] = new Label();
                    grid[i, j].SetValue(Grid.RowProperty, i);
                    grid[i, j].SetValue(Grid.ColumnProperty, j);
                    grid[i, j].SetValue(Label.BackgroundProperty, new SolidColorBrush(Colors.DarkRed));
                    grid[i, j].SetValue(Label.BorderBrushProperty, new SolidColorBrush(Colors.Black));
                    grid[i, j].SetValue(Label.BorderThicknessProperty, new Thickness(1.0));
                    grid[i, j].SetValue(Label.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                    grid[i, j].SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
                    grid[i, j].SetValue(Label.VerticalAlignmentProperty, VerticalAlignment.Stretch);
                    grid[i, j].SetValue(Label.VerticalContentAlignmentProperty, VerticalAlignment.Center);
                    
                    TileLayout.Children.Add(grid[i, j]);
                }

                labels[i] = new TextBlock();
                labels[i].SetValue(Grid.RowProperty, i);
                labels[i].SetValue(Grid.ColumnProperty, 8);
                TileLayout.Children.Add(labels[i]);

            }
        }

        private void Image_SourceUpdated(object sender, DataTransferEventArgs e)
        {
        }

        private void Image_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            // the new datacontext should be a new TileInformation object, so we can refill our Border with it
            var tileInfo = e.NewValue as TileInformation;
            if (tileInfo != null)
            {
                for (int i = 0; i < 8; ++i)
                {
                    for (int j = 7; j >= 0; --j)
                    {
                        switch (tileInfo.TileData[i * 8 + j])
                        {
                            case 0:
                                grid[i, 7 - j].SetValue(Border.BackgroundProperty, new SolidColorBrush(Colors.Black));
                                break;
                            case 1:
                                grid[i, 7 - j].SetValue(Border.BackgroundProperty, new SolidColorBrush(Colors.Red));
                                break;
                            case 2:
                                grid[i, 7 - j].SetValue(Border.BackgroundProperty, new SolidColorBrush(Colors.Blue));
                                break;
                            case 3:
                                grid[i, 7 - j].SetValue(Border.BackgroundProperty, new SolidColorBrush(Colors.White));
                                break;
                        }
                        ComboBox t = new ComboBox();
                        t.Items.Add("0");
                        t.Items.Add("1");
                        t.Items.Add("2");
                        t.Items.Add("3");
                        t.Items.Add("4");
                        t.SelectedIndex = 0;
                        t.Background = new SolidColorBrush(Colors.Yellow);
                        t.Foreground = new SolidColorBrush(Colors.Black);
                        grid[i, j].Content = t;
                    }
                    labels[i].Text = string.Format("{0:x4}", tileInfo.LineAddresses[i]);
                }

            }
        }
    }
}
