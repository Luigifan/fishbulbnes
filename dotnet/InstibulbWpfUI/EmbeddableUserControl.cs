using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace InstibulbWpfUI
{
    public class EmbeddableUserControl : UserControl
    {
        public event EventHandler<RedrawEventArgs> RedrawRequested;
        private delegate void NoArgDelegate();

        internal void RequestRedraw(int x, int y, int width, int height)
        {
            if (RedrawRequested != null) RedrawRequested(this, new RedrawEventArgs()
                                            {
                                                Left = x,
                                                Top = y,
                                                Width = width,
                                                Height = height
                                            }
                );
        }

        public void HandleEvent(FakeEventArgs ev)
        {
            switch (ev.EventType)
            {
                case FakedEventTypes.MOUSEMOVE:
                    HandleMouseEvent(ev.X, ev.Y, Mouse.MouseMoveEvent);
                    break;
                case FakedEventTypes.MOUSELEAVE:
                    HandleMouseEvent(ev.X, ev.Y, Mouse.MouseLeaveEvent);
                    break;
                case FakedEventTypes.MOUSEENTER:
                    HandleMouseEvent(ev.X, ev.Y, Mouse.MouseEnterEvent);
                    break;
                case FakedEventTypes.MOUSECLICK:
                    HandleMouseClick(ev.X, ev.Y);
                    break;
                case FakedEventTypes.MOUSEDOWN:
                    HandleMouseButtonEvent(ev.X, ev.Y, Mouse.MouseDownEvent);
                    break;
                case FakedEventTypes.MOUSEUP:
                    HandleMouseButtonEvent(ev.X, ev.Y, Mouse.MouseUpEvent);
                    break;
                    
                case FakedEventTypes.MOUSEDOUBLECLICK:
                case FakedEventTypes.KEYPRESS:
                    break;
            }
        }

        public void HandleMouseEvent(double x, double y, RoutedEvent eventType)
        {
            x *= this.ActualWidth;
            y *= this.ActualHeight;

            HitTestResult result = null;
            Dispatcher.Invoke(new NoArgDelegate(delegate
            {
                result = VisualTreeHelper.HitTest(this, new System.Windows.Point(x, y));
                if (result == null)
                    return;
                var item = result.VisualHit;
                if (item != null && item is FrameworkElement)
                {
                    var ev = new MouseEventArgs(Mouse.PrimaryDevice, 0);
                    ev.RoutedEvent = eventType;
                    ev.Source = item;
                    (item as FrameworkElement).RaiseEvent(ev);
                    var k = item as FrameworkElement;
                    var tr = k.TransformToVisual(this);
                    var res = tr.Transform(new Point(0,0));
                    RequestRedraw((int)res.X, (int)res.Y, (int)k.ActualWidth, (int)k.ActualHeight);
                }

            }), System.Windows.Threading.DispatcherPriority.Render, null);

        }


        public void HandleMouseButtonEvent(double x, double y, RoutedEvent eventType)
        {
            x *= this.ActualWidth;
            y *= this.ActualHeight;

            HitTestResult result = null;
            Dispatcher.Invoke(new NoArgDelegate(delegate
            {
                result = VisualTreeHelper.HitTest(this, new System.Windows.Point(x, y));
                if (result == null)
                    return;
                var item = result.VisualHit;
                if (item != null && item is FrameworkElement)
                {
                    var ev = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
                    ev.RoutedEvent = eventType;
                    ev.Source = item;
                    (item as FrameworkElement).RaiseEvent(ev);
                    var k = item as FrameworkElement;
                    var tr = k.TransformToVisual(this);
                    var res = tr.Transform(new Point(0, 0));
                    RequestRedraw((int)res.X, (int)res.Y, (int)k.ActualWidth, (int)k.ActualHeight);
                }

            }), System.Windows.Threading.DispatcherPriority.Render, null);

        }


        public void HandleMouseClick(double x, double y)
        {
            x *= this.ActualWidth;
            y *= this.ActualHeight;

            HitTestResult result = null;
            Dispatcher.Invoke(new NoArgDelegate(delegate
            {
                result = VisualTreeHelper.HitTest(this, new System.Windows.Point(x, y));
                if (result == null)
                    return;
                var item = result.VisualHit as FrameworkElement;
                // lets try and find the button, if there is one
                FrameworkElement element = item as FrameworkElement;
                while (element != null && !(element is Button))
                {
                    element = element.TemplatedParent as FrameworkElement;
                }
                if (element != null)
                {
                    Button b = (Button)element ;
                    if (b.Command != null && b.Command.CanExecute(b.CommandParameter))
                    {
                        b.Command.Execute(b.CommandParameter);
                        RequestRedraw(0,0,0,0);
                        return;
                    }                
                }
                
                if (item != null)
                {
                    var k = item as FrameworkElement;
                    var tr = k.TransformToVisual(this);
                    var res = tr.Transform(new Point(0, 0));
                    //if (item.Command != null && item.Command.CanExecute(item.CommandParameter))
                    //{
                    //    item.Command.Execute(item.CommandParameter);
                    //}
                    //else
                    //{
                        item.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, this));
                    //}
                    RequestRedraw((int)res.X, (int)res.Y, (int)k.ActualWidth, (int)k.ActualHeight);

                }

            }), System.Windows.Threading.DispatcherPriority.Render, null);

        }

        public void HandleKeyPress(Key key)
        {

        }
    }
}
