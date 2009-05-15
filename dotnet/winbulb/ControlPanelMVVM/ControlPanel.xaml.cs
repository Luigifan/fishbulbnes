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
using WPFamicom.ControlPanelMVVM;
using System.Windows.Media.Animation;

namespace WPFamicom
{
	/// <summary>
	/// Interaction logic for ControlPanel.xaml
	/// </summary>
	public partial class ControlPanel
	{

        public static readonly DependencyProperty DebuggerIsVisibleProperty = DependencyProperty.Register("DebuggerIsVisible", typeof(Boolean), typeof(ControlPanel), new PropertyMetadata(false));
        public static readonly DependencyProperty PPUDebuggerIsVisibleProperty = DependencyProperty.Register("PPUDebuggerIsVisible", typeof(Boolean), typeof(ControlPanel), new PropertyMetadata(false));
        public static readonly DependencyProperty BreakpointPendingProperty = DependencyProperty.Register("BreakpointPending", typeof(Boolean), typeof(ControlPanel), new PropertyMetadata(false));



        public Boolean BreakpointPending
        {
            get { return (Boolean)this.GetValue(BreakpointPendingProperty); }
            set { 
                this.SetValue(BreakpointPendingProperty, value);
                if (value)
                {
                    sbBreakpointPending.Begin(this);
                }
                else
                {
                    sbBreakpointPending.Remove();
                }
            }
        }

        public Boolean DebuggerIsVisible
        {
            get { return (Boolean)this.GetValue(DebuggerIsVisibleProperty); }
            set { this.SetValue(DebuggerIsVisibleProperty, value); }
        }

        public Boolean PPUDebuggerIsVisible
        {
            get { return (Boolean)this.GetValue(PPUDebuggerIsVisibleProperty); }
            set 
            {
                this.SetValue(PPUDebuggerIsVisibleProperty, value); 
            }
        }

        Storyboard sbBreakpointPending;
 	    public ControlPanel()
		{
			this.InitializeComponent();
            sbBreakpointPending = (Storyboard)FindResource("BreakpointPendingStoryboard");
		}

        private void ShowDebugger(object sender, RoutedEventArgs e)
        {
            this.DebuggerIsVisible = true; 
        }

        private void HIdeDebugger(object sender, RoutedEventArgs e)
        {
            this.DebuggerIsVisible = false;
        }

        private void HideControlPanel(object sender, RoutedEventArgs e)
        {
            this.ControlStack.Visibility = Visibility.Hidden;
        }

        private void ShowControlPanel(object sender, RoutedEventArgs e)
        {
            this.ControlStack.Visibility = Visibility.Visible;
        }

        private void IgnoreKey(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

	}
}