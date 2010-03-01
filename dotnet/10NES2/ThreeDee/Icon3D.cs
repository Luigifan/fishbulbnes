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
using System.Windows.Input;
using System.ComponentModel;

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

    public class Icon3D : UIElement3D, IMenuIconItem, ICommandSource
    {

        public static DependencyProperty IsActivatableProperty =
            DependencyProperty.Register("IsActivatable", typeof(bool), typeof(Icon3D),
            new PropertyMetadata(false, new PropertyChangedCallback(IsActivatableChanged)));

        static Brush defaultBubbleBrush = new SolidColorBrush(Color.FromArgb(15, 0, 0, 255));

        public static DependencyProperty BillboardProperty =
            DependencyProperty.Register("Billboard", typeof(Brush), typeof(Icon3D),
            new PropertyMetadata(defaultBubbleBrush, new PropertyChangedCallback(BillboardChanged)));

        public static DependencyProperty DesiredRadiusProperty = DependencyProperty.Register(
            "DesiredRadius", typeof(double), typeof(Icon3D),
            new PropertyMetadata(1.0, new PropertyChangedCallback(DesiredRadiusChanged))
            );

        public static DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(Icon3D),
            new PropertyMetadata(null, new PropertyChangedCallback(OnCommandChanged))
            );
        public static DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof(object), typeof(Icon3D),
            new PropertyMetadata(null, new PropertyChangedCallback(OnCommandParameterChanged))
            );

        public static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(Icon3D),
            new PropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged))
            );


        public Icon3D()
        {
            
        }




        static void OnTitleChanged(object o, DependencyPropertyChangedEventArgs args)
        {
            var p = o as Icon3D;
            p.CreateTitleBrush(args.NewValue as string);
            //p.Rebuild();
        }

        static void OnCommandChanged(object o, DependencyPropertyChangedEventArgs args)
        {
        }

        static void OnCommandParameterChanged(object o, DependencyPropertyChangedEventArgs args)
        {
        }

        static void DesiredRadiusChanged(object o, DependencyPropertyChangedEventArgs args)
        {
            var p = o as Icon3D;
            if (p == null) return;
            p.ResizeTo((double)args.NewValue);
        }

        static void IsActivatableChanged(object o, DependencyPropertyChangedEventArgs args)
        {
            var p = o as Icon3D;
            if (p == null) return;
            if ((bool)args.NewValue)
            {
                p.Activate();
            }
            else
            {
                p.DeActivate();
            }

        }

        static void BillboardChanged(object o, DependencyPropertyChangedEventArgs args)
        {
            var p = o as Icon3D;
            if (p == null) return;
            if (args.NewValue is Brush)
                p.Billboard = args.NewValue as Brush;
            else
                p.Billboard = defaultBubbleBrush;
        }

        GeometryModel3D billboardModel = new GeometryModel3D();
        GeometryModel3D titleModel = null;
        
        public bool IsActivatable
        {
            get { return (bool)GetValue(Icon3D.IsActivatableProperty); }
            set { SetValue(Icon3D.IsActivatableProperty, value); }
        }

        public Stream MakePNG(int width, int height, UIElement canvas, int dpi)
        {
            Size size = new Size(width, height);
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

        public string Title
        {
            get
            {
                return (string)GetValue(Icon3D.TitleProperty); 
            }
            set
            {
                SetValue(Icon3D.TitleProperty, value);
            }
        }

        Brush _titleBrush;

        void CreateTitleBrush(string Title)
        {
            TextBlock l = new TextBlock();
            l.Text = Title;
            l.Background = new SolidColorBrush(Colors.Transparent);
            l.Foreground = new SolidColorBrush(Colors.Yellow);
            l.Margin = new Thickness(0, 16, 0, 0);
            l.VerticalAlignment = VerticalAlignment.Center;
            UniformGrid g = new UniformGrid();
            g.Rows = 4;
            g.Columns = 4;
            for (int i = 0; i < 4; ++i)
            {
                g.Children.Add(new TextBlock());
            }
            g.Children.Add(l);
            g.Background = new SolidColorBrush(Colors.Transparent);

            _titleBrush = new VisualBrush(g);
            
        }

        public Brush Billboard
        {
            get { return (Brush)GetValue(Icon3D.BillboardProperty); }
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
            double scaleFactor = (radius * 0.75) / bigR;

            // find the rough center of the model, ie; the position of its origin, plus half its bounding size
            Point3D ctr = model.Bounds.Location;
            ctr.X = ctr.X + (model.Bounds.SizeX / 2);
            ctr.Y = ctr.Y + (model.Bounds.SizeY / 2);
            ctr.Z = ctr.Z + (model.Bounds.SizeZ / 2);

            // if we create a vector from origin to center, this is how to translate the model into the bubble
            Vector3D translateFactor = Point3D.Subtract(new Point3D(0, 0, 0), ctr);

            modelScale = new ScaleTransform3D(new Vector3D(scaleFactor, scaleFactor, scaleFactor), ctr);
            modelTranslate = new TranslateTransform3D(translateFactor);

            //Make a bubble
            GeometryModel3D sphereModel = new GeometryModel3D();

            GeometryModel3D titleModel = new GeometryModel3D();

            SphereMeshGenerator gen2 = new SphereMeshGenerator();
            gen2.Slices = 64;
            gen2.Stacks = 32;
            gen2.Center = new Point3D(0, 0, 0);
            gen2.Radius = radius;
            sphereModel.Geometry = gen2.Geometry;

            var matGrp = new MaterialGroup();
            matGrp.Children.Add(new DiffuseMaterial(Billboard));
            matGrp.Children.Add(new SpecularMaterial(new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)), 0.8));
            sphereModel.Material = matGrp;
            sphereModel.BackMaterial = matGrp;

            if (_titleBrush != null)
            {

                gen2.Slices = 64;
                gen2.Stacks = 32;
                gen2.Center = new Point3D(0, 0, 0);
                gen2.Radius = radius ;
                titleModel.Geometry = gen2.Geometry;

                var titleMatGrp = new MaterialGroup();
                titleMatGrp.Children.Add(new DiffuseMaterial(_titleBrush));
                titleModel.Material = titleMatGrp;
                titleModel.BackMaterial = titleMatGrp;
                this.titleModel = titleModel;
                Transform3DGroup titleTrans = new Transform3DGroup();
                titleTrans.Children.Add(new RotateTransform3D(titleRotation, new Point3D(0, 0, 0)));
                titleTrans.Children.Add(new ScaleTransform3D(1.5,1.0,1.1));

                
                titleModel.Transform = titleTrans;
            }


            // put some trouble in the bubble
            RotateTransform3D rotSphere = new RotateTransform3D();
            var angle = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 135);
            rotSphere.Rotation = angle;
            sphereModel.Transform = rotSphere;

            this.billboardModel = sphereModel;
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

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsActivatable)
            {
                if (Command != null && Command.CanExecute(CommandParameter))
                {
                    Command.Execute(CommandParameter);
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            hasMouse = true;
            UpdateColors();

        }


        DoubleAnimation titleAnimation = new DoubleAnimation(0, -360, new Duration(new TimeSpan(0, 0, 3)), FillBehavior.HoldEnd);
        public void Activate()
        {
            titleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            titleAnimation.By = -5.0f;

            titleRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, titleAnimation);

            UpdateColors();
        }

        public void DeActivate()
        {
            titleRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, null);
            
            UpdateColors();
        }

        public void ReActivate()
        {
            if (IsActivatable) 
                Activate(); 
            else 
                DeActivate();
        }


        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            hasMouse = false;
            UpdateColors();
        }

        bool hasMouse = false;
        void UpdateColors()
        {
            //if (visual != null)
            //{
            //    if (IsActivatable)
            //    {
            //        if (hasMouse && visual is FrameworkElement)
            //            (visual as FrameworkElement).Focus();
            //            //visual.SetValue(Panel.BackgroundProperty, new SolidColorBrush(Color.FromArgb(100, 255, 75, 75)));
            //        else
            //            visual.SetValue(Panel.BackgroundProperty, new SolidColorBrush(Color.FromArgb(50, 255, 75, 75)));
            //    }
            //    else
            //    {
            //        visual.SetValue(Panel.BackgroundProperty, new SolidColorBrush(Color.FromArgb(50, 128, 128, 128)));
            //    }
            //}
        }

        TranslateTransform3D modelTranslation = new TranslateTransform3D(0, 0, 0);
        RotateTransform3D rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -90), new Point3D(0, 0, 0));

        AxisAngleRotation3D titleRotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 90);

        

        private Model3DGroup _models = new Model3DGroup();
        private Model3D model;
        public Model3D Model
        {
            get { return Visual3DModel; }
            set
            {
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
            _models.Children.Add(billboardModel);

            if (titleModel != null)
                _models.Children.Add(titleModel);

            // rotate the whole thing if vertical
            _models.Transform = rotateTransform;

            base.Visual3DModel = _models;
        }



        [Localizability(LocalizationCategory.NeverLocalize)]
        [Bindable(true)]
        [Category("Action")]
        public ICommand Command 
        { 
                get 
                { 
                    return (ICommand)GetValue(Icon3D.CommandProperty); 
                }
                set 
                { 
                    SetValue(Icon3D.CommandProperty, value); 
                } 
        }
        //
        // Summary:
        //     Gets or sets the parameter to pass to the System.Windows.Controls.Primitives.ButtonBase.Command
        //     property.
        //
        // Returns:
        //     Parameter to pass to the System.Windows.Controls.Primitives.ButtonBase.Command
        //     property.
        [Bindable(true)]
        [Localizability(LocalizationCategory.NeverLocalize)]
        [Category("Action")]
        public object CommandParameter
        {
            get
            {
                return GetValue(Icon3D.CommandParameterProperty);
            }
            set
            {
                SetValue(Icon3D.CommandParameterProperty, value);
            }
        }
        //
        // Summary:
        //     Gets or sets the element on which to raise the specified command.
        //
        // Returns:
        //     Element on which to raise a command.
        [Bindable(true)]
        [Category("Action")]
        public IInputElement CommandTarget { get; set; }
    }
}
