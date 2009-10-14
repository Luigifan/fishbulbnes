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

        public ThreeDeeControls()
        {
            InitializeComponent();
            WheelGrid.Rows = 1;
            WheelGrid.Columns = WheelGrid.Children.Count;
            if (WheelGrid.Columns > 0)
                stepping = 360 / WheelGrid.Columns;
            else
                stepping = 15;
            //WheelGrid.UpdateLayout();
            myAngleRotation.SetValue(AxisAngleRotation3D.AngleProperty, stepping);
            WheelGrid.UpdateLayout();
        }

        double currentAngle = 0;
        double stepping = 45;

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation angleAnimation = new DoubleAnimation();

            angleAnimation.From = currentAngle;

            angleAnimation.To = currentAngle + stepping;

            angleAnimation.Duration = TimeSpan.FromSeconds(0.5);

            AxisAngleRotation3D rotation  = new AxisAngleRotation3D();

            rotation.Axis = new Vector3D(0, 1, 0);

            camera.Transform = new RotateTransform3D(rotation);
            HeadLight.Transform = new RotateTransform3D(rotation);

            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, angleAnimation);

            currentAngle += stepping;
            if (currentAngle > 360) currentAngle -= 360;
        }


        private void Right_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation angleAnimation = new DoubleAnimation();

            angleAnimation.From = currentAngle;

            angleAnimation.To = currentAngle - stepping;

            angleAnimation.Duration = TimeSpan.FromSeconds(0.5);

            AxisAngleRotation3D rotation = new AxisAngleRotation3D();

            rotation.Axis = new Vector3D(0, 1, 0);

            camera.Transform = new RotateTransform3D(rotation);
            HeadLight.Transform = new RotateTransform3D(rotation);

            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, angleAnimation);

            currentAngle -= stepping;
            if (currentAngle < 0) currentAngle += 360;
        }




    }
}
