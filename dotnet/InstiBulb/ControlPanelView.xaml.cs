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
using Fishbulb.Common.UI;

namespace InstiBulb
{
    /// <summary>
    /// Interaction logic for ControlPanelView.xaml
    /// </summary>
    public partial class ControlPanelView : UserControl
    {



        public ControlPanelView()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;

        }

        public event EventHandler<EventArgs> UpdateKeyhandlingEvent;

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var old = e.OldValue as ControlPanelVM;
            if (old != null)
            {
                old.RunStatusChangedEvent -= OnRunStatusChangedEvent;
            }

            var p = e.NewValue as ControlPanelVM;
            if (p != null)
            {
                p.RunStatusChangedEvent += OnRunStatusChangedEvent;
            }
        }

        void OnRunStatusChangedEvent(object sender, RunStatusChangedEventArgs e)
        {
            SuppressKeystrokes = (e.NewState == RunningStatuses.Running);
            if (UpdateKeyhandlingEvent != null)
                UpdateKeyhandlingEvent(this, EventArgs.Empty);
        }

        public bool SuppressKeystrokes
        {
            get;
            set;
        }
    }
}
