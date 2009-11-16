using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlimDXBindings.Viewer10.Helpers
{
    using System;

    using D3D10 = SlimDX.Direct3D10;
    using DXGI = SlimDX.DXGI;

    public class DepthBuffer : IDisposable
    {
        readonly D3D10.Device device;

        public readonly D3D10.Texture2D Resource;
        public readonly D3D10.DepthStencilView View;

        public DepthBuffer(D3D10.Device device, int width, int height, DXGI.Format format)
        {
            this.device = device;

            D3D10.Texture2DDescription desc = new D3D10.Texture2DDescription();

            desc.Width = width;
            desc.Height = height;

            desc.ArraySize = 1;
            desc.BindFlags = D3D10.BindFlags.DepthStencil;
            desc.CpuAccessFlags = D3D10.CpuAccessFlags.None;
            desc.Format = format;
            desc.MipLevels = 1;
            desc.OptionFlags = D3D10.ResourceOptionFlags.None;
            desc.SampleDescription = new SlimDX.DXGI.SampleDescription(1, 0);
            desc.Usage = D3D10.ResourceUsage.Default;

            Resource = new D3D10.Texture2D(device, desc);
            View = new D3D10.DepthStencilView(device, Resource);
        }

        public void Dispose()
        {
            Resource.Dispose();
            View.Dispose();
        }

        public void Clear(float depth)
        {
            device.ClearDepthStencilView(View, D3D10.DepthStencilClearFlags.Depth, depth, 0);
        }
    }
}
