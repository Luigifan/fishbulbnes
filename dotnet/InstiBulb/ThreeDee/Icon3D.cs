using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;

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

        static void BillboardChanged(object o, DependencyPropertyChangedEventArgs args)
        {
            var p = o as Icon3D;
            if (p == null) return;
            p.CreateBillboard(args.NewValue as UIElement);
        }

        GeometryModel3D billboard = new GeometryModel3D();
        Visual visual;
        VisualBrush billboardBrush;

        internal void CreateBillboard(UIElement elem)
        {
            billboard = CreateBillboardModel();
            visual = elem;
            billboardBrush = new VisualBrush(elem);

            RenderOptions.SetCachingHint(billboardBrush, CachingHint.Cache);
            billboardBrush.ViewportUnits = BrushMappingMode.Absolute;
            billboardBrush.TileMode = TileMode.None;

            billboard.Material = new DiffuseMaterial(billboardBrush);
            Rebuild();
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

                Rebuild();
            }
        }

        void Rebuild()
        {
            _models.Children.Clear();
            _models.Children.Add(billboard);

            Transform3DGroup group = new Transform3DGroup();
            if (model.Transform != null)
                group.Children.Add(model.Transform);
            group.Children.Add(modelTranslation);
            model.Transform = group;
            _models.Children.Add(model);

            base.Visual3DModel = _models; 
        }
        

    }
}
