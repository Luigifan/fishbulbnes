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
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using Microsoft.Practices.Unity;
using Fishbulb.Common.UI;
using System.Windows.Markup;
using InstiBulb.Views;
using System.IO;
using InstiBulb;
using InstiBulb.Integration;
using System.Windows.Controls.Primitives;
using InstibulbWpfUI;

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

        InteractiveCanvasSpinnerFactory menuSpinner;
        InteractiveCanvasSpinnerFactory debugSpinner;

        InteractiveCanvasSpinnerFactory activeSpinner;
        bool menuActive = true;

        public static DependencyProperty IsAnimatingProperty = DependencyProperty.Register("IsAnimating", typeof(bool), typeof(ThreeDeeControls), new PropertyMetadata(false));


        public bool IsAnimating
        {
            get { return (bool)GetValue(ThreeDeeControls.IsAnimatingProperty); }
            set { SetValue(ThreeDeeControls.IsAnimatingProperty, value); }
        }

        IUnityContainer container;
        public ThreeDeeControls()
        {
            container = (IUnityContainer)FindResource("Container");

            watchedDisplay = container.Resolve<NESDisplay>();
            watchedDisplay.ContextChanged += new EventHandler(watchedDisplay_ContextChanged);

            WhizOnHandler = new NoArgDelegate(WhizOn);
            WhizOffHandler = new NoArgDelegate(WhizOff);
            this.Initialized += new EventHandler(ThreeDeeControls_Initialized);
            InitializeComponent();
            OuterGrid.Background = new SolidColorBrush(Color.FromArgb(25, 0, 0, 240));
            
        }

        NESDisplay watchedDisplay;
        
        //Visual[] vList = new Visual[6];

        void ThreeDeeControls_Initialized(object sender, EventArgs e)
        {
            
            List<UIElement3D> menuIcons = new List<UIElement3D>();
            double radius = GetSegmentLength(5, 360 / 5) * 0.30;
            menuIcons.Add(MakeIcon<ControlPanelView>(TryFindResource("nes") as Model3DGroup, icon_IconPressedEvent, "Control Panel", radius));
            menuIcons.Add(MakeIcon<SoundPanelView>(TryFindResource("Headphones") as Model3DGroup, icon_IconPressedEvent, "Sound Panel", radius));
            menuIcons.Add(MakeIcon<ControllerConfig>(TryFindResource("controlPad") as Model3DGroup, icon_IconPressedEvent, "Control Pad", radius));
            menuIcons.Add(MakeIcon<CheatControl>(TryFindResource("Sword") as Model3DGroup, icon_IconPressedEvent, "Cheat Control", radius));
            menuIcons.Add(MakeIcon<CartInfoPanel>(TryFindResource("Heart") as Model3DGroup, icon_IconPressedEvent, "Cart Info", radius));
            menuIcons.Add(MakeIcon<SaveStateView>(TryFindResource("Heart") as Model3DGroup, icon_IconPressedEvent, "Save States", radius));

            menuSpinner = new InteractiveCanvasSpinnerFactory(spinnerContainer, menuIcons, 5, 0);
            menuSpinner.JumpTo(0);

            MakeDebugSpinner();
            activeSpinner = menuSpinner;
        }

        void  MakeDebugSpinner()
        {
            List<UIElement3D> icons = new List<UIElement3D>();

            double radius = GetSegmentLength(4, 360/4) * 0.3;
            icons.Add(MakeIcon<MachineStatus>(TryFindResource("nes") as Model3DGroup, icon_IconPressedEvent, "Machine Status", radius));
            icons.Add(MakeIcon<NameTableViewerControl>(TryFindResource("nes") as Model3DGroup, icon_IconPressedEvent, "Name Tables", radius));
            //icons.Add(MakeIcon<PatternViewerControl>(TryFindResource("nes") as Model3DGroup, icon_IconPressedEvent, "Pattern Tables", radius));
            icons.Add(MakeIcon<InstructionRolloutControl>(TryFindResource("nes") as Model3DGroup, icon_IconPressedEvent,"Future Instructions", radius));
            icons.Add(MakeIcon<InstructionHistoryControl>(TryFindResource("nes") as Model3DGroup, icon_IconPressedEvent, "Past Instructions", radius));
            icons.Add(MakeIcon<TileEditor>(TryFindResource("nes") as Model3DGroup, icon_IconPressedEvent, "Tile Editor", radius));

            debugSpinner = new InteractiveCanvasSpinnerFactory(debugContainer, icons, 4, 90);

        }

        public double GetSegmentLength(double radius, double angle)
        {
            return Math.Sqrt((2 * Math.Pow(radius, 2)) * (1 - Math.Cos( ToRadians( angle ) )));
        }

        double ToRadians(double degrees)
        {
            return  (Math.PI * degrees)/180;
        }
        
        public Icon3D MakeIcon<T>(Model3DGroup model, EventHandler<IconPressedEventArgs> handler, string billboardText, double radius) where T : Visual
        {
            Icon3D icon = new Icon3D();
            
            TextBlock l = new TextBlock();
            l.Text = billboardText;
            l.Background = new SolidColorBrush(Colors.Transparent);
            l.Foreground = new SolidColorBrush(Colors.Yellow);

            UniformGrid g = new UniformGrid();
            g.Rows = 4;
            g.Columns = 4;
            for (int i = 0; i < 4; ++i)
            {
                g.Children.Add(new TextBlock());
            }
            g.Children.Add(l);
            g.Background = new SolidColorBrush(Colors.Transparent);

            icon.Model = model;
            icon.Billboard = g;
            icon.DesiredRadius = radius;
            icon.Rebuild();
            return icon;
        }



        FrameworkElement popup;
        void icon_IconPressedEvent(object sender, IconPressedEventArgs e)
        {
            popup = (FrameworkElement)container.Resolve(e.DisplayTypeRequested);
            popup.SetValue(Grid.RowSpanProperty, 2);
            PopupPropertiesGrid.Child = popup;
            PopupPropertiesGrid.Visibility = Visibility.Visible;
        }


        void watchedDisplay_ContextChanged(object sender, EventArgs e)
        {
           // menuSpinner.UpdateVisual(5, (UIElement)watchedDisplay.Context.PropertiesPanel); 
        }

        private static UIElement MakeRedBox( )
        {
            Canvas b = new Canvas();
            b.Height = 255;
            b.Width = 255;
            b.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            Viewbox v = new Viewbox();
            v.Child = b;
            return v ;
        }

        public Model3D LoadModel(System.IO.Stream fileStream)
        {
            return (Model3D)XamlReader.Load(fileStream);

        }

        double currentAngle = 0;

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            activeSpinner.Previous();
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            activeSpinner.Next();
        }


        private void ControlPanel_UpdateKeyhandlingEvent(object sender, EventArgs e)
        {
            DependencyObject parent = GetTopParent();

            if (parent == null)
            {
                return;
            }

            //if (ControlPanel.SuppressKeystrokes)
            //{
            //    Keyboard.AddPreviewKeyDownHandler(parent, KeySuppressor);
            //    Keyboard.AddPreviewKeyDownHandler(parent, KeySuppressor);
            //    Dispatcher.BeginInvoke(WhizOffHandler, System.Windows.Threading.DispatcherPriority.Render, null);
            //}
            //else {
            //    Keyboard.RemovePreviewKeyDownHandler(parent, KeySuppressor);
            //    Keyboard.RemovePreviewKeyDownHandler(parent, KeySuppressor);

            //}

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
            rotateAnimation.To = 1080;
            rotateAnimation.Duration = TimeSpan.FromSeconds(1.5);
            rotateAnimation.FillBehavior = FillBehavior.Stop;
            rotateAnimation.By = 30;
            TranslateTransform3D translate = new TranslateTransform3D();
            AxisAngleRotation3D rotation = new AxisAngleRotation3D();
            rotation.Axis = new Vector3D(0, 1, 0);

            Transform3DGroup group = new Transform3DGroup();
            group.Children.Add(translate);
            group.Children.Add(new RotateTransform3D(rotation));

            camera.Transform = group;
            //HeadLight.Transform = group;

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
            rotateAnimation.From = -1080;
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
            //HeadLight.Transform = group;

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

        private void viewer3d_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                activeSpinner.Previous();
            if (e.Delta < 0)
                activeSpinner.Next();

            if (e.MiddleButton == MouseButtonState.Pressed)
                WhizOff();
        }

        private void viewer3d_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
                WhizOff();
        }

        private void Up_Click(object sender, MouseButtonEventArgs e)
        {
            if (!menuActive)
            {
                activeSpinner = menuSpinner;
                menuSpinner.RotateZTo(0, false);
                debugSpinner.RotateZTo(90, true);
                menuActive = true;
            }
            else
            {
                menuSpinner.RotateZTo(90, false);
                debugSpinner.RotateZTo(0, true);
                activeSpinner = debugSpinner;
                menuActive = false;

            }
        }

        private void Down_Click(object sender, MouseButtonEventArgs e)
        {
            //AxisAngleRotation3D rotation = new AxisAngleRotation3D();
            //rotation.Axis = new Vector3D(0, 0, 1);
            if (!menuActive)
            {
                activeSpinner = menuSpinner;
                menuSpinner.RotateZTo(0, true);
                debugSpinner.RotateZTo(90, false);
                menuActive = true;
            }
            else
            {
                menuSpinner.RotateZTo(90, true);
                debugSpinner.RotateZTo(0, false);
                activeSpinner = debugSpinner;
                menuActive = false;

            }
        }
    }
}
