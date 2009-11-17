using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SlimDXBindings.Viewer10.Helpers
{
    public class EmbeddableUserControl : UserControl
    {
        public event EventHandler RedrawRequested;
        private delegate void NoArgDelegate();

        internal void RequestRedraw()
        {
            if (RedrawRequested != null) RedrawRequested(this, EventArgs.Empty);
        }

        public void HandleMouseMove(double x, double y)
        {
            x *= 512;
            y *= 512;

            HitTestResult result = null;
            Dispatcher.BeginInvoke(new NoArgDelegate(delegate
            {
                result = VisualTreeHelper.HitTest(this, new System.Windows.Point(x, y));
                if (result == null)
                    return;
                var item = result.VisualHit;
                if (item != null && item is UIElement)
                {
                    //var ev = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
                    //ev.RoutedEvent = UIElement.MouseDownEvent;
                    //ev.Source = item;
                    //ev.LeftButton = MouseButtonState.Pressed;
                    var tev = new RoutedEventArgs(ButtonBase.ClickEvent, item);
                    (item as UIElement).RaiseEvent(tev);

                }
            }), System.Windows.Threading.DispatcherPriority.Render, null);

        }

        public void HandleKeyPress(Key key)
        {

        }
    }
}
