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

        public ThreeDeeControls()
        {
            rotater = new IntArgDelegate(Rotater);
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
            RotateCamera(stepping / (WheelGrid.Children.Count/ 2));
        }

        double currentAngle = 0;
        double stepping = 45;

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            RotateCamera(currentAngle + stepping);
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            RotateCamera(currentAngle - stepping);

        }


        void RotateCamera(double angle)
        {
            DoubleAnimation angleAnimation = new DoubleAnimation();

            angleAnimation.From = currentAngle;

            angleAnimation.To = angle;

            angleAnimation.Duration = TimeSpan.FromSeconds(0.5);
            angleAnimation.FillBehavior = FillBehavior.HoldEnd;
            AxisAngleRotation3D rotation = new AxisAngleRotation3D();

            rotation.Axis = new Vector3D(0, 1, 0);

            camera.Transform = new RotateTransform3D(rotation);
            HeadLight.Transform = new RotateTransform3D(rotation);

            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, angleAnimation);

            currentAngle = angle;
            while (currentAngle > 360) currentAngle -= 360;
            while (currentAngle < 0) currentAngle += 360;
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

            transFormanimation.Completed += new EventHandler(Whizoff_Completed);
            translate.BeginAnimation(TranslateTransform3D.OffsetZProperty, transFormanimation);
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotateAnimation);


        }

        public void WhizOn()
        {
            this.Visibility = Visibility.Visible;
            this.UpdateLayout();

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

            translate.BeginAnimation(TranslateTransform3D.OffsetZProperty, transFormanimation);
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotateAnimation);


        }

        void Whizoff_Completed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            
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
        bool wheeling;

        IntArgDelegate rotater;

        void Rotater(int i)
        {
            RotateCamera(stepping * i);
        }

        private void OuterGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            
            //Dispatcher.BeginInvoke(rotater, e.Delta/120);
        }


    }
}
