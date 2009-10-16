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
using _3DTools;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;

namespace InstiBulb.ThreeDee
{
    /// <summary>
    /// Interaction logic for ThreeDeeControls.xaml
    /// </summary>
    public partial class ThreeDeeControls : UserControl
    {
        public delegate void IntArgDelegate(int i);
        public delegate void NoArgDelegate();

        public NoArgDelegate WhizOnHandler;
        public NoArgDelegate WhizOffHandler;

        public static DependencyProperty IsAnimatingProperty = DependencyProperty.Register("IsAnimating", typeof(bool), typeof(ThreeDeeControls), new PropertyMetadata(false));


        public bool IsAnimating
        {
            get { return (bool)GetValue(ThreeDeeControls.IsAnimatingProperty); }
            set { SetValue(ThreeDeeControls.IsAnimatingProperty, value); }
        }

        public ThreeDeeControls()
        {
            WhizOnHandler = new NoArgDelegate(WhizOn);
            WhizOffHandler = new NoArgDelegate(WhizOff);

            InitializeComponent();
            ControlPanel.UpdateKeyhandlingEvent += new EventHandler<EventArgs>(ControlPanel_UpdateKeyhandlingEvent);

            WheelGrid.Rows = 1;
            WheelGrid.Columns = WheelGrid.Children.Count;
            Wheel.RebuildGeometry(WheelGrid.Children.Count, 8, 2);
            
            if (WheelGrid.Columns > 0)
                stepping = 360 / WheelGrid.Columns;
            else
                stepping = 15;
            //WheelGrid.UpdateLayout();
            myAngleRotation.SetValue(AxisAngleRotation3D.AngleProperty, stepping);
            WheelGrid.UpdateLayout();
            //RotateCamera(stepping / (WheelGrid.Children.Count/ 2));
            Wheel.JumpTo(0);
        }

        double currentAngle = 0;
        double stepping = 45;

        private void Left_Click(object sender, RoutedEventArgs e)
        {
         //   RotateCamera(currentAngle + stepping);
            Wheel.Previous();
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            //RotateCamera(currentAngle - stepping);
            Wheel.Next();
        }


        private void ControlPanel_UpdateKeyhandlingEvent(object sender, EventArgs e)
        {
            DependencyObject parent = GetTopParent();

            if (parent == null)
            {
                return;
            }

            if (ControlPanel.SuppressKeystrokes)
            {
                Keyboard.AddPreviewKeyDownHandler(parent, KeySuppressor);
                Keyboard.AddPreviewKeyDownHandler(parent, KeySuppressor);
                Dispatcher.BeginInvoke(WhizOffHandler, System.Windows.Threading.DispatcherPriority.Render, null);
            }
            else {
                Keyboard.RemovePreviewKeyDownHandler(parent, KeySuppressor);
                Keyboard.RemovePreviewKeyDownHandler(parent, KeySuppressor);

            }

        }

        private void WhizOff()
        {
            this.IsAnimating = true;
            DoubleAnimation transFormanimation = new DoubleAnimation();
            transFormanimation.From = 0;
            transFormanimation.To = 20;
            transFormanimation.Duration = TimeSpan.FromSeconds(1.5);
            transFormanimation.FillBehavior = FillBehavior.Stop;
            
            DoubleAnimation rotateAnimation = new DoubleAnimation();
            rotateAnimation.From = 0;
            rotateAnimation.To = 5000;
            rotateAnimation.Duration = TimeSpan.FromSeconds(1.5);
            rotateAnimation.FillBehavior = FillBehavior.Stop;

            TranslateTransform3D translate = new TranslateTransform3D();
            AxisAngleRotation3D rotation = new AxisAngleRotation3D();
            rotation.Axis = new Vector3D(0, 1, 0);

            Transform3DGroup group = new Transform3DGroup();
            group.Children.Add(translate);
            group.Children.Add(new RotateTransform3D(rotation));

            camera.Transform = group;
            HeadLight.Transform = group;

            transFormanimation.Completed += Whizoff_Completed;
            translate.BeginAnimation(TranslateTransform3D.OffsetXProperty, transFormanimation);
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotateAnimation);


        }

        public void WhizOn()
        {
            this.IsAnimating = true;
            this.Visibility = Visibility.Visible;
            //this.UpdateLayout();

            DoubleAnimation transFormanimation = new DoubleAnimation();
            transFormanimation.From = 20;
            transFormanimation.To = 0;
            transFormanimation.Duration = TimeSpan.FromSeconds(1.5);
            transFormanimation.FillBehavior = FillBehavior.Stop;

            DoubleAnimation rotateAnimation = new DoubleAnimation();
            rotateAnimation.From = -5000;
            rotateAnimation.To = currentAngle;
            rotateAnimation.Duration = TimeSpan.FromSeconds(1.5);
            rotateAnimation.FillBehavior = FillBehavior.HoldEnd;

            TranslateTransform3D translate = new TranslateTransform3D();
            AxisAngleRotation3D rotation = new AxisAngleRotation3D();
            rotation.Axis = new Vector3D(0, 1, 0);


            Transform3DGroup group = new Transform3DGroup();
            group.Children.Add(translate);
            group.Children.Add(new RotateTransform3D(rotation));

            camera.Transform = group;
            HeadLight.Transform = group;

            transFormanimation.Completed += new EventHandler(EndAnimation);
            translate.BeginAnimation(TranslateTransform3D.OffsetXProperty, transFormanimation);
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotateAnimation);


        }

        void Whizoff_Completed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            this.IsAnimating = false;
            
        }

        void EndAnimation(object sender, EventArgs e)
        {

            this.IsAnimating = false;
        }

        void KeySuppressor(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }


        private DependencyObject GetTopParent()   
       {   
           DependencyObject dpParent = this.Parent;   
           do   
           {   
               dpParent = LogicalTreeHelper.GetParent(dpParent);
           } while (dpParent.GetType().BaseType != typeof(Window) && dpParent.GetType().BaseType != typeof(UserControl));   
           return dpParent;   
       }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            ColorAnimation animation = new ColorAnimation(Color.FromArgb(75, 128, 201, 128), new Duration(new TimeSpan(0, 0, 0, 0, 250)));
            animation.FillBehavior = FillBehavior.HoldEnd;
            var p = sender as Label;
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(0, 128, 128, 128));
            p.SetValue(Label.BackgroundProperty, brush);

            brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }


        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            ColorAnimation animation = new ColorAnimation(Color.FromArgb(0, 128, 201, 128), new Duration(new TimeSpan(0, 0, 0, 0, 250)));
            animation.FillBehavior = FillBehavior.HoldEnd;
            var p = sender as Label;
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(75, 128, 201, 128));
            p.SetValue(Label.BackgroundProperty, brush);

            brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

    }
}
