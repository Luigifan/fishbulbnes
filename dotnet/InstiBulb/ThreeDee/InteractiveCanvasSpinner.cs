using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;

namespace InstiBulb.ThreeDee
{
    public class InteractiveCanvasSpinnerFactory : FrameworkElement
    {

        public InteractiveCanvasSpinnerFactory(ContainerUIElement3D container, List<UIElement3D> icons, double radius, double rotateZ)
        {
            this.container = container;
            this.radius = radius;
            BuildSpinner(container, icons);
            this.RotateZ(rotateZ);
        }
        double currentAngle = 0;
        double currentZAngle = 0;
        double radius = 2;

        public void Next()
        {
            currPanel++;
            if (currPanel >= angleLocks.Count)
                currPanel = 0;

            RotateTo(angleLocks[currPanel], true);
            SelectActiveIcon();
        }

        public void Previous()
        {
            currPanel--;
            if (currPanel < 0)
                currPanel = angleLocks.Count - 1;

            RotateTo(angleLocks[currPanel], false);
            SelectActiveIcon();
        }

        public void Up()
        {
            RotateZTo(currentZAngle + 90, true);
            SelectActiveIcon();
        }

        public void Down()
        {
            RotateZTo(currentZAngle + 90, false);
            SelectActiveIcon();
        }

        public void JumpTo(int Panel)
        {
            if (Panel > 0 && Panel < angleLocks.Count)
            {
                RotateTo(angleLocks[Panel], false);
                currPanel = Panel;
            }
            SelectActiveIcon();
        }

        void SelectActiveIcon()
        {
            int activeIcon = (currPanel == 0)? 0 :  icons.Count - currPanel;

            for (int i = 0; i < icons.Count; ++i )
            {
                var icon = icons[i] as Icon3D;
                if (icon != null)
                {
                    icon.SetValue(Icon3D.IsActivatableProperty, (i == activeIcon));
                }
            }
            container.InvalidateModel();

            
            
        }

        public void OrderBackToFront()
        {
            container.Children.Clear();
            
            foreach (var child in icons)
            {
                container.Children.Add(child);
            }
        }

        List<double> angleLocks;

        int currPanel = 0;

        public void RotateZ(double angle)
        {
            containerRotation2.Angle = angle;
            currentZAngle = angle;
        }

        void RotateTo(double angle, bool clockWise)
        {

            DoubleAnimation angleAnimation = new DoubleAnimation(0.0, 0.0, new Duration(TimeSpan.FromSeconds(0.5)), FillBehavior.HoldEnd);

            if (clockWise)
            {
                angleAnimation.From = currentAngle;
                if (angle < currentAngle)
                {
                    angle += 360;
                }
                angleAnimation.To = angle;
            }
            else
            {
                angleAnimation.From = currentAngle;
                if (angle > currentAngle)
                {
                    angle -= 360;
                }
                angleAnimation.To = angle;

            }

            angleAnimation.Completed += angleAnimation_Completed;
            containerRotatation.BeginAnimation(AxisAngleRotation3D.AngleProperty, angleAnimation);

            currentAngle = angle;
            while (currentAngle > 360) currentAngle -= 360;
            while (currentAngle < 0) currentAngle += 360;
            
        }

        public void RotateZTo(double angle, bool clockWise)
        {

            DoubleAnimation angleAnimation = new DoubleAnimation(0.0, 0.0, new Duration(TimeSpan.FromSeconds(0.5)), FillBehavior.HoldEnd);

            if (clockWise)
            {
                angleAnimation.From = currentZAngle;
                if (angle < currentZAngle)
                {
                    angle += 360;
                }
                angleAnimation.To = angle;
            }
            else
            {
                angleAnimation.From = currentZAngle;
                if (angle > currentZAngle)
                {
                    angle -= 360;
                }
                angleAnimation.To = angle;

            }

            angleAnimation.Completed += new EventHandler(angleAnimation_Completed);
            angleAnimation.RemoveRequested += new EventHandler(angleAnimation_RemoveRequested);
            containerRotation2.BeginAnimation(AxisAngleRotation3D.AngleProperty, angleAnimation);
            currentZAngle = angle;
            while (currentZAngle > 360) currentZAngle -= 360;
            while (currentZAngle < 0) currentZAngle += 360;
        }

        void angleAnimation_RemoveRequested(object sender, EventArgs e)
        {
            RotateZ(currentZAngle);
        }

        void angleAnimation_Completed(object sender, EventArgs e)
        {

        }

        public double Radius
        {
            get { return radius; }
            set { radius = value; }
        }
        
        ContainerUIElement3D container;
        AxisAngleRotation3D containerRotatation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
        AxisAngleRotation3D containerRotation2 = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
        
        List<UIElement3D> icons = new List<UIElement3D>();
        internal void BuildSpinner(ContainerUIElement3D container, List<UIElement3D> Icons)
        {
            this.icons = Icons;
            int panelCount = icons.Count;
            double angle = 360 / icons.Count;
            angleLocks = new List<double>();
            for (int i = 0; i < panelCount; i++)
            {
                double t = i * angle;
                if (t != 360)
                    angleLocks.Add(t);

                var newContainer = new ContainerUIElement3D();
                Transform3DGroup tGroup = new Transform3DGroup();
                Matrix3D m = Matrix3D.Identity;
                m.Transform(new Vector3D(0, 0, radius));
                Matrix3D m2 = Matrix3D.Identity;
                m2.Rotate(new Quaternion(new Vector3D(0, 1, 0), angle * i));
                //tGroup.Children.Add(new MatrixTransform3D(Matrix3D.Multiply(m, m2)));
                
                tGroup.Children.Add(new TranslateTransform3D(new Vector3D(0, 0, radius)));
                tGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), angle * i)));
                tGroup.Children.Add(new RotateTransform3D(containerRotatation));
                tGroup.Children.Add(new RotateTransform3D(containerRotation2));

                newContainer.Transform = tGroup;

                newContainer.Children.Add(icons[i]);

                container.Children.Add(newContainer);

            }

        }

        public double Angle
        {
            get { return 360 / icons.Count; }
        }


        
    }
}
