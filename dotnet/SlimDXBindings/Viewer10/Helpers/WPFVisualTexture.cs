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

namespace SlimDXBindings.Viewer10.Helpers
{
    public class WPFVisualTexture : Texture2D
    {
        readonly Device device;
        public EmbeddableUserControl control;
        int width; int height;
        int[] pixelData;

        public WPFVisualTexture(Device device, int width, int height, EmbeddableUserControl control)
            : base(device, new Texture2DDescription()
                {
                    Usage = ResourceUsage.Dynamic,
                    Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm,
                    ArraySize = 1,
                    MipLevels = 1,
                    Width = width,
                    Height = height,
                    BindFlags = BindFlags.ShaderResource ,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    SampleDescription = new DXGI.SampleDescription(1,0)
                })
        {
            this.device = device;
            this.control = control;
            this.width = width;
            this.height = height;
            this.control.RedrawRequested += new EventHandler(control_RedrawRequested);
            pixelData = new int[width * height];
            isDirty = true;
            
        }

        bool isDirty = false;

        public bool IsDirty
        {
            get { return isDirty; }
        }

        void control_RedrawRequested(object sender, EventArgs e)
        {
            isDirty = true;
        }

        public void UpdateVisual()
        {
            
            control.Measure(new System.Windows.Size(width, height));
            control.Arrange(new System.Windows.Rect(0, 0, width, height));

            RenderTargetBitmap rTarg = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rTarg.Render(control);

            var frame = BitmapFrame.Create(rTarg);
            int stride = (width * 32 + 7) / 8;
            frame.CopyPixels(pixelData, stride, 0);

            DataRectangle rect = this.Map(0, MapMode.WriteDiscard, MapFlags.None);
            rect.Data.WriteRange<int>(pixelData);
            this.Unmap(0);
            isDirty = false;

        }

        public EmbeddableUserControl EmbeddedControl
        {
            get { return control; }
        }
    }
}
