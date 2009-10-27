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
        }

        public void Previous()
        {
            currPanel--;
            if (currPanel < 0)
                currPanel = angleLocks.Count - 1;

            RotateTo(angleLocks[currPanel], false);
        }

        public void Up()
        {
            RotateZTo(currentZAngle + 90, true);
        }

        public void Down()
        {
            RotateZTo(currentZAngle + 90, false);
        }

        public void JumpTo(int Panel)
        {
            if (Panel > 0 && Panel < angleLocks.Count)
            {
                RotateTo(angleLocks[Panel], false);
                currPanel = Panel;
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
            foreach (var p in icons)
            {
                if (p is Icon3D)
                    (p as Icon3D).Dance();

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
            foreach (var p in icons)
            {
                if (p is Icon3D)
                    (p as Icon3D).UnDance();

            }
        }

        public double Radius
        {
            get { return radius; }
            set { radius = value; }
        }
        
        ContainerUIElement3D container;
        AxisAngleRotation3D containerRotatation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
        AxisAngleRotation3D containerRotation2 = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
        
        List<InteractiveCanvas> iCanvas;
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

                tGroup.Children.Add(new TranslateTransform3D(new Vector3D(0, 0, radius)));
                tGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), angle * i)));
                tGroup.Children.Add(new RotateTransform3D(containerRotatation));
                tGroup.Children.Add(new RotateTransform3D(containerRotation2));

                newContainer.Transform = tGroup;

                newContainer.Children.Add(icons[i]);
                //var light = new PointLight(Colors.AntiqueWhite, new Point3D(0, 0, radius * 2));
                //light.Range = radius * 1.5;
                ////light.LinearAttenuation = 0.1;
                //var model = new ModelVisual3D();
                //model.Content = light;
                //newContainer.Children.Add(model);

                container.Children.Add(newContainer);

            }

        }

        public void UpdateVisual(int index, Visual newVisual)
        {
            if (null != iCanvas && iCanvas.Count > index)
            {
                iCanvas[index].Visual = null;
                iCanvas[index].Visual = newVisual;
            }
            
        }
        
    }
}
