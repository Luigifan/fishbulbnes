using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D10;

namespace SlimDXBindings.Viewer10.Filter
{
    public class BasicHDRFilter : BasicPostProcessingFilter
    {
        SlimDX.DXGI.SampleDescription sampleDescription = new SlimDX.DXGI.SampleDescription(1, 0);
        internal override SlimDX.Direct3D10.Texture2DDescription GetTextureDescription()
        {
            Texture2DDescription desc = new Texture2DDescription();
            desc.Usage = ResourceUsage.Default;
            desc.Format = SlimDX.DXGI.Format.R32G32B32A32_Float;
            desc.ArraySize = 1;
            desc.MipLevels = 1;
            desc.Width = width;
            desc.Height = height;
            desc.BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget;
            desc.SampleDescription = sampleDescription;
            return desc;
        }
    }
}
