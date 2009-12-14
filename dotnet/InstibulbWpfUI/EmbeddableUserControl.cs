using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Automation;

namespace InstibulbWpfUI
{
    public class EmbeddableUserControl : UserControl
    {
        public event EventHandler<RedrawEventArgs> RedrawRequested;
        private delegate void NoArgDelegate();
        Window window;

        public EmbeddableUserControl()
        {
            //window = new Window();
            ////window.Height = 1;
            ////window.Width = 1;
            //window.Content = this;
            //window.Opacity = 25;
            //window.WindowStyle = WindowStyle.None;
            //window.WindowState = WindowState.Minimized;
            ////window.ShowInTaskbar = false;
            //window.Show();
            this.SetValue(UserControl.BackgroundProperty, new SolidColorBrush(Color.FromArgb(50, 205, 120, 100)));
        }

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

        public void HandleEvent(object o, FakeEventArgs ev)
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
                    HandleKeyPress(ev.X, ev.Y, ev.EventData);
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
                if (item != null && item is UIElement)
                {
                    var ev = new MouseEventArgs(Mouse.PrimaryDevice, 0);
                    ev.RoutedEvent = eventType;
                    ev.Source = item;
                    (item as UIElement).RaiseEvent(ev);
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
                    Console.WriteLine("Button Event " + eventType.ToString());
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
                ButtonBase element = FindCommandBoundElement(item);
                if (element != null)
                {
                    ButtonBase b = (ButtonBase)element ;
                    //if (b.Command != null && b.Command.CanExecute(b.CommandParameter))
                    //{
                    //    b.Command.Execute(b.CommandParameter);
                    //    RequestRedraw(0,0,0,0);
                    //    return;
                    //}

                    //if (b is CheckBox)
                    //{
                    //    var c = (CheckBox)b;
                    //    bool? check = (bool?)b.GetValue(CheckBox.IsCheckedProperty);
                    //    if (check.GetValueOrDefault(false))
                    //    {
                    //        b.SetValue(CheckBox.IsCheckedProperty, false);
                    //    }
                    //    else
                    //    {
                    //        b.SetValue(CheckBox.IsCheckedProperty, true);
                    //    }
                    //    return;
                    //}
                    
                    var k = item as FrameworkElement;
                    var tr = k.TransformToVisual(this);
                    var res = tr.Transform(new Point(0, 0));
                    element.SetValue(ButtonBase.ClickModeProperty, ClickMode.Press);
                    element.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, this));
                    Console.WriteLine("Button Click");
                    
                    RequestRedraw((int)res.X, (int)res.Y, (int)k.ActualWidth, (int)k.ActualHeight);
                    
                }

            }), System.Windows.Threading.DispatcherPriority.Render, null);

        }

        private static ButtonBase FindCommandBoundElement(FrameworkElement item)
        {
            FrameworkElement element = item as FrameworkElement;
            while (element != null && !(element is ButtonBase))
            {
                element = element.TemplatedParent as FrameworkElement;
            }
            return element as ButtonBase;
        }

        public void HandleKeyPress(double x, double y, int keyChar)
        {
            //x *= this.ActualWidth;
            //y *= this.ActualHeight;

            //HitTestResult result = null;
            //Dispatcher.Invoke(new NoArgDelegate(delegate
            //{
            //    result = VisualTreeHelper.HitTest(this, new System.Windows.Point(x, y));
            //    if (result == null)
            //        return;
            //    var item = result.VisualHit as FrameworkElement;
            //    // lets try and find the button, if there is one
            //    if (item != null)
            //    {
                    
            //        var k = item as FrameworkElement;
            //        var tr = k.TransformToVisual(this);
            //        var res = tr.Transform(new Point(0, 0));
            //        var f = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(window), 0, Key.A);
            //        f.RoutedEvent = Keyboard.PreviewKeyDownEvent;
            //        f.Source = this;

            //        item.RaiseEvent(f);

            //        RequestRedraw((int)res.X, (int)res.Y, (int)k.ActualWidth, (int)k.ActualHeight);
            //    }

            //}), System.Windows.Threading.DispatcherPriority.Render, null);

        }
    }
}
