using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D10;
using System.Windows.Media;
using System.Windows.Controls;
using DXGI=SlimDX.DXGI;
using System.Windows.Media.Imaging;
using SlimDX;
using InstibulbWpfUI;
using System.Windows;
using System.Drawing;
using System.Windows.Controls.Primitives;

namespace SlimDXBindings.Viewer10.Helpers
{
    public class WPFVisualTexture : Texture2D
    {
        readonly Device device;
        EmbeddableUserControl control;
        int width;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        int height;

        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        int[] pixelData;

        public void MakeDirty()
        {
            dirtyAt = DateTime.Now;
            isDirty = true;
            DirtyRegions.Add(new Rectangle(0, 0, width, height));
        }

        List<FrameworkElement> swappableControls = new List<FrameworkElement>();

        public List<FrameworkElement> SwappableControls
        {
            get { return swappableControls; }
            set { swappableControls = value; }

        }

        public WPFVisualTexture(Device device, int width, int height, EmbeddableUserControl control)
            : base(device, new Texture2DDescription()
                {
                    Usage = ResourceUsage.Dynamic,
                    Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm,
                    ArraySize = 1,
                    MipLevels = 1,
                    Width = 2048,
                    Height = 2048,
                    BindFlags = BindFlags.ShaderResource ,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    SampleDescription = new DXGI.SampleDescription(1,0)
                })
        {
            this.device = device;
            this.control = control;
            this.width = width;
            this.height = height;
            this.control.RedrawRequested +=  new EventHandler<RedrawEventArgs>(control_RedrawRequested);
            rTarg = new RenderTargetBitmap(2048, 2048, 96, 96, PixelFormats.Pbgra32);
            pixelData = new int[2048 * 2048];
            dirtyAt = DateTime.Now;
            isDirty = true;
            SetupControl(this.control);
            DirtyRegions.Add(new Rectangle(0, 0, 2048, 2048));


        }


        int currentId = 0;

        public void SwapControl(int controlId)
        {
            if (swappableControls.Count > controlId && currentId != controlId)
            {
                control.Content = swappableControls[controlId];
                SetupControl(control);
                currentId = controlId;
                DirtyRegions.Add(new Rectangle(0, 0, width, height));
                // rTarg = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            }
            dirtyAt = DateTime.Now;
            isDirty = true;
        }

        bool isDirty = false;

        public bool IsDirty
        {
            get { return isDirty; }
        }


        DateTime dirtyAt = DateTime.MaxValue;

        public List<Rectangle> DirtyRegions = new List<Rectangle>();

        void control_RedrawRequested(object sender, RedrawEventArgs e)
        {
            dirtyAt = DateTime.Now;
            isDirty = true;
            DirtyRegions.Add(new Rectangle(e.Left, e.Top, e.Width, e.Height));
        }

        void SetupControl(object Current)
        {
            if (Current is UIElement)
            {
                if (Current is ButtonBase)
                {
                    ((ButtonBase)Current).SetValue(ButtonBase.ClickModeProperty, ClickMode.Press);

                }
            }

            DependencyObject dObj = Current as DependencyObject;

            if (dObj != null)
                foreach (object child in LogicalTreeHelper.GetChildren(dObj))
                    SetupControl(child);

        }

        private delegate void NoArgDelegate();
        RenderTargetBitmap rTarg;
        public void UpdateVisual()
        {
            BitmapFrame frame = null;
            DataRectangle rect = null;
            control.Dispatcher.Invoke(new NoArgDelegate(delegate
            {
                control.Measure(new System.Windows.Size(width, height));
                control.Arrange(new System.Windows.Rect(0, 0, width, height));
                rTarg.Clear();
                rTarg.Render(control);
                
                frame = BitmapFrame.Create(rTarg);

                int stride = (2048 * 32 + 7) / 8;

                rect = this.Map(0, MapMode.WriteDiscard, MapFlags.None);
                foreach (Rectangle rec in DirtyRegions)
                {
                    frame.CopyPixels(new Int32Rect(rec.X, rec.Y, rec.Width, rec.Height), pixelData, stride, (rec.Y * 2048) + rec.X);
                }

            }), System.Windows.Threading.DispatcherPriority.Send, null);

            //frame.CopyPixels(pixelData, stride, 0);


            rect.Data.WriteRange<int>(pixelData);
            this.Unmap(0);
            if (dirtyAt < DateTime.Now - TimeSpan.FromSeconds(1))
            {
                dirtyAt = DateTime.MaxValue;
                isDirty = false;
                DirtyRegions.Clear();
            }


        }

        public EmbeddableUserControl EmbeddedControl
        {
            get { return control; }
        }
    }
}
