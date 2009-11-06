using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D10;

namespace SlimDXBindings.Viewer10.Filter
{
    public class TextureBuddy : IDisposable
    {
        Device device;
        public TextureBuddy(Device device)
        {
            this.device = device;
        }

        List<IDisposable> disposables = new List<IDisposable>();

        public Texture2D CreateNoiseMap(int resolution)
        {
            Random rand = new Random();
            int[] noisyColors = new int[resolution * resolution];
            for (int x = 0; x < resolution; x++)
                for (int y = 0; y < resolution; y++)
                    noisyColors[x + y * resolution] = rand.Next(int.MaxValue);


            Texture2DDescription desc = new Texture2DDescription();
            desc.Usage = ResourceUsage.Dynamic;
            desc.Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm;
            desc.ArraySize = 1;
            desc.MipLevels = 1;
            desc.Width = resolution;
            desc.Height = resolution;
            desc.BindFlags = BindFlags.ShaderResource;
            desc.CpuAccessFlags = CpuAccessFlags.Write;
            desc.SampleDescription = new SlimDX.DXGI.SampleDescription(1, 0);

            Texture2D noiseImage = new Texture2D(device, desc);
            DataRectangle r = noiseImage.Map(0, MapMode.WriteDiscard, MapFlags.None);
            r.Data.WriteRange<int>(noisyColors);
            noiseImage.Unmap(0);

            disposables.Add(noiseImage);

            return noiseImage;
        }

        public Texture2D LoadFile(string fileName)
        {

            if (System.IO.File.Exists(fileName))
            {
                Texture2D tex = Texture2D.FromFile(device, fileName);
                disposables.Add(tex);
                return tex;
            }
            else
            {
                return CreateNoiseMap(64);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (IDisposable i in disposables)
            {
                i.Dispose();
            }
        }

        #endregion
    }
}
