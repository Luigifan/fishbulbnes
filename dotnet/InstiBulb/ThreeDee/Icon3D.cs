using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.IO;

namespace InstiBulb.ThreeDee
{
    public class IconPressedEventArgs : EventArgs 
    {
        public IconPressedEventArgs(Type t, string name)
        {
            DisplayNameRequested = name;
            DisplayTypeRequested = t;
        }

        public Type DisplayTypeRequested
        {
            get;
            private set;
        }

        public string DisplayNameRequested
        {
            get;
            private set;
        }
    }

    public class Icon3D : UIElement3D 
    {

        public static DependencyProperty BillboardProperty = 
            DependencyProperty.Register("BillboardText", typeof(UIElement), typeof(Icon3D), 
            new PropertyMetadata(null, new PropertyChangedCallback(BillboardChanged)));

        public static DependencyProperty DesiredRadiusProperty = DependencyProperty.Register(
            "DesiredRadius", typeof(double), typeof(Icon3D), 
            new PropertyMetadata(1.0, new PropertyChangedCallback(DesiredRadiusChanged))
            );

        static void DesiredRadiusChanged(object o, DependencyPropertyChangedEventArgs args)
        {
            var p = o as Icon3D;
            if (p == null) return;
                p.ResizeTo((double)args.NewValue);
        }

        static void BillboardChanged(object o, DependencyPropertyChangedEventArgs args)
        {
            var p = o as Icon3D;
            if (p == null) return;
            p.CreateBillboard(args.NewValue as UIElement);
        }

        GeometryModel3D billboard = new GeometryModel3D();
        Visual visual;
        Brush billboardBrush;

        internal void CreateBillboard(UIElement elem)
        {
            UniformGrid g = new UniformGrid();
            g.Rows = 4;
            g.Columns = 4;
            for (int i = 0; i < 4; ++i)
            {
                g.Children.Add(new Canvas());
            }
            g.Children.Add(elem);
            g.Opacity = 0.5;
            visual = g;

            //Rebuild();

        }

        public Stream MakePNG(int width, int height, UIElement canvas, int dpi)
        {
            Size size = new Size(width,height);
            canvas.Measure(size);
            //canvas.Arrange(new Rect(size)); 

            var rtb = new RenderTargetBitmap(
                width, //width 
                height, //height 
                dpi, //dpi x 
                dpi, //dpi y 
                PixelFormats.Pbgra32 // pixelformat 
                );
            rtb.Render(canvas);

            var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
            enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(rtb));

            var stm = new MemoryStream();
            enc.Save(stm);
            return stm;
        }

        internal static GeometryModel3D CreateBillboardModel()
        {
            GeometryModel3D model = new GeometryModel3D();
            
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(new Point3D(-1, 1, 0));
            mesh.Positions.Add(new Point3D(-1, 0, 0));
            mesh.Positions.Add(new Point3D(1, 0, 0));
            mesh.Positions.Add(new Point3D(1, 1, 0));

            mesh.TextureCoordinates.Add(new System.Windows.Point(0, 0));
            mesh.TextureCoordinates.Add(new System.Windows.Point(0, 1));
            mesh.TextureCoordinates.Add(new System.Windows.Point(1, 1));
            mesh.TextureCoordinates.Add(new System.Windows.Point(1, 0));

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            model.Geometry = mesh;
            
            mesh.Freeze();
            model.Transform = new TranslateTransform3D(new Vector3D(0, 0.2, -1));
            //model.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 90));
            return model;

        }

        public UIElement Billboard
        {
            get { return (UIElement)GetValue(Icon3D.BillboardProperty); }
            set { SetValue(Icon3D.BillboardProperty, value); }
        }

        public double DesiredRadius
        {
            get { return (double)GetValue(Icon3D.DesiredRadiusProperty); }
            set { SetValue(Icon3D.DesiredRadiusProperty, value); }
        }

        ScaleTransform3D modelScale = new ScaleTransform3D();
        TranslateTransform3D modelTranslate = new TranslateTransform3D();

        private void MakeSphere()
        {
            double radius = this.DesiredRadius;

            if (radius <= 0)
            {
                radius = 1;
            }
            
            // find out the largest "radius" of the bounding rectangle
            double bigR = model.Bounds.SizeX;
            if (model.Bounds.SizeY > bigR)
                bigR = model.Bounds.SizeY;
            if (model.Bounds.SizeZ > bigR)
                bigR = model.Bounds.SizeZ;

            bigR /= 2;

            // the model will fit in a bounding sphere 3/4 the size of the outer sphere's radius
            //   this is the factor by which to scale the model to make it fit
            double scaleFactor = ( radius * 0.75) / bigR;
            
            // find the rough center of the model, ie; the position of its origin, plus half its bounding size
            Point3D ctr = model.Bounds.Location;
            ctr.X = ctr.X + (model.Bounds.SizeX / 2);
            ctr.Y = ctr.Y + (model.Bounds.SizeY / 2);
            ctr.Z = ctr.Z + (model.Bounds.SizeZ / 2);
            
            // if we create a vector from origin to center, this is how to translate the model into the bubble
            Vector3D translateFactor = Point3D.Subtract(new Point3D(0,0,0), ctr);
            
            modelScale = new ScaleTransform3D(new Vector3D(scaleFactor, scaleFactor, scaleFactor), ctr);
            modelTranslate = new TranslateTransform3D(translateFactor);

            //Make a bubble
            GeometryModel3D sphereModel = new GeometryModel3D();

            SphereMeshGenerator gen2 = new SphereMeshGenerator();
            gen2.Center = new Point3D(0, 0, 0);
            gen2.Radius = radius;
            sphereModel.Geometry = gen2.Geometry;

            var matGrp = new MaterialGroup();

            if (visual != null)
            {
                billboardBrush = new VisualBrush(visual);
                RenderOptions.SetCachingHint(billboardBrush, CachingHint.Cache);
                matGrp.Children.Add(new DiffuseMaterial(billboardBrush));
            }
            else
            {
                var specMat = new EmissiveMaterial(new SolidColorBrush(Color.FromArgb(15, 0, 0, 255)));
                matGrp.Children.Add(specMat);
            }
            sphereModel.Material = matGrp;
            sphereModel.BackMaterial = matGrp;

            // put some trouble in the bubble
            RotateTransform3D rotSphere = new RotateTransform3D();
            var angle = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            rotSphere.Rotation = angle;
            DoubleAnimation sphereSpin = new DoubleAnimation(360, new Duration(TimeSpan.FromSeconds(6)));
            sphereSpin.RepeatBehavior = RepeatBehavior.Forever;
            sphereSpin.AutoReverse = false;

            angle.BeginAnimation(AxisAngleRotation3D.AngleProperty, sphereSpin);

            sphereModel.Transform = rotSphere;

            this.billboard = sphereModel;
        }

        private List<Point3D> GetAllPositions(Model3D model)
        {
            List<Point3D> pts = new List<Point3D>();
            if (model is Model3DGroup)
            {
                foreach (var p in ((Model3DGroup)model).Children)
                {
                    pts.AddRange(GetAllPositions(p));
                }
            }
            else
            {
                if (model is GeometryModel3D)
                {
                    var p = (MeshGeometry3D)model.GetValue(GeometryModel3D.GeometryProperty);
                    var modelPts = (Point3DCollection)p.GetValue(MeshGeometry3D.PositionsProperty);
                    pts.AddRange(modelPts);
                }
            }

            return pts;
        }


        internal void ResizeTo(double radius)
        {
            // Rebuild();
        }

        public event EventHandler<IconPressedEventArgs> IconPressedEvent;
        Type type;
        public Icon3D(Type t)
            : base()
        {
            type = t;
            anim.RepeatBehavior = RepeatBehavior.Forever;
            anim.FillBehavior = FillBehavior.HoldEnd;
            anim.AutoReverse = true;
            animX.RepeatBehavior = RepeatBehavior.Forever;
            animX.FillBehavior = FillBehavior.HoldEnd;
            animX.AutoReverse = true;
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IconPressedEvent != null)
                IconPressedEvent(this, new IconPressedEventArgs(type, null));
            
            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            // Dance();
            if (visual != null)
                visual.SetValue(Label.BackgroundProperty, new SolidColorBrush(Color.FromArgb(76, 128, 128, 225)));

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            if (visual != null)
                visual.SetValue(Label.BackgroundProperty, new SolidColorBrush(Color.FromArgb(50, 128, 128, 128)));
            // UnDance();
            base.OnMouseLeave(e);
        }

        TranslateTransform3D modelTranslation = new TranslateTransform3D(0, 0, 0);
        Storyboard danceSb ;

        public void UnDance()
        {
           //if (danceSb != null)
          ////  danceSb.Stop();
            modelTranslation.BeginAnimation(TranslateTransform3D.OffsetXProperty, null);
            modelTranslation.BeginAnimation(TranslateTransform3D.OffsetYProperty, null);

        }

        DoubleAnimation anim = new DoubleAnimation(-0.05, 0.05, new Duration(TimeSpan.FromSeconds(0.3)));
        DoubleAnimation animX = new DoubleAnimation(0.05, -0.05, new Duration(TimeSpan.FromSeconds(0.2)));

        public void Dance()
        {
            
            modelTranslation.BeginAnimation(TranslateTransform3D.OffsetXProperty, anim);
            modelTranslation.BeginAnimation(TranslateTransform3D.OffsetYProperty, animX);
            
        }

        private Model3DGroup _models = new Model3DGroup();
        private Model3D model;
        public Model3D Model
        {
            get { return Visual3DModel; }
            set {
                model = value;
                // Rebuild();
            }
        }

        public void Rebuild()
        {
            MakeSphere();

            _models.Children.Clear();

            Transform3DGroup group = new Transform3DGroup();
            // preserve any pre-transform transforms (in the model itself)
            if (model.Transform != null)
                group.Children.Add(model.Transform);

            // first scale the model back down to size
            group.Children.Add(modelScale);
            // translate the model back to the origin
            group.Children.Add(modelTranslate);

            // this one makes it wiggle
            group.Children.Add(modelTranslation);
            model.Transform = group;
            _models.Children.Add(model);
            _models.Children.Add(billboard);

            base.Visual3DModel = _models; 
        }
        

    }
}
