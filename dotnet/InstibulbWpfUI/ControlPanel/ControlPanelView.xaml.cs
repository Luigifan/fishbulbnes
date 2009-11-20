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
using InstibulbWpfUI;

namespace InstibulbWpfUI
{
    /// <summary>
    /// Interaction logic for ControlPanelView.xaml
    /// </summary>
    public partial class ControlPanelView : EmbeddableUserControl
    {



        public ControlPanelView()
        {
            InitializeComponent();
        }

        public event EventHandler<EventArgs> UpdateKeyhandlingEvent;



        public bool SuppressKeystrokes
        {
            get;
            set;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
