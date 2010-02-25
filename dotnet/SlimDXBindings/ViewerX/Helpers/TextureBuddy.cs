using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D10;
using System.Windows.Media;
using InstibulbWpfUI;

namespace SlimDXBindings.Viewer10.Helpers
{
    public class EffectBuddy : IDisposable
    {
        Device device;
        Dictionary<string, Effect> createdEffects = new Dictionary<string, Effect>();

        public EffectBuddy(Device device)
        {
            this.device = device;
        }

        static System.IO.Stream GetShaderStreamFromResource(string name)
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(
                string.Format("SlimDXBindings.ViewerX.Filter.Shaders.{0}.fx", name)
                );
        }

        public Effect GetEffect( string resourceName)
        {
            if (createdEffects.ContainsKey(resourceName))
                return createdEffects[resourceName];

            Effect e = Effect.FromStream(device,
                GetShaderStreamFromResource(resourceName),
                "fx_4_0", ShaderFlags.None, EffectFlags.None, null, null);


            createdEffects.Add(resourceName, e);
            return e;
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (var p in createdEffects)
            {
                p.Value.Dispose();
            }
        }

        #endregion
    }

    public class TextureBuddy : IDisposable
    {
        Device device;
        public TextureBuddy(Device device)
        {
            this.device = device;
        }
        Dictionary<string,  Texture2D> createdTextures = new Dictionary<string, Texture2D>();
        List<IDisposable> disposables = new List<IDisposable>();
        List<WPFVisualTexture> wpfTextures = new List<WPFVisualTexture>();


        public Texture2D SetupTexture2D(string name, Texture2DDescription description)
        {
            Texture2D newTex;
            try
            {
                newTex = new Texture2D(device, description);

                if (description.Usage == ResourceUsage.Dynamic)
                {
                    DataRectangle r = newTex.Map(0, MapMode.WriteDiscard, MapFlags.None);
                    r.Data.WriteRange<int>(new int[description.Width * description.Height]);
                    newTex.Unmap(0);
                }
            }
            catch
            {
                return null;
            }
            createdTextures.Add(name, newTex);
            return newTex;
        }

        /// <summary>
        /// Fetches a texture out of the dictionary by name
        /// </summary>
        /// <typeparam name="T">SlimDX.Direct3D10.Texture2D, or a subclass thereof</typeparam>
        /// <param name="name">The name of the texture</param>
        /// <returns>The texture if found, else null</returns>
        public T GetTexture<T>(string name) where T: Texture2D
        {
            if (createdTextures.ContainsKey(name))
            {
                return createdTextures[name] as T;
            }
            return null;
        }

        /// <summary>
        /// Creates a 2D texture of the specified resolution full of Perlin noise 
        /// 
        /// The new texture is tracked in the textureBuddys disposables collection, and is managed by the TextureBuddy
        /// the new texture is not available in the dictionary
        /// </summary>
        /// <param name="resolution">The resolution of the final texture (both height and width)</param>
        /// <returns>The new texture</returns>
        public Texture2D CreateNoiseMap2D(int resolution)
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

        /// <summary>
        /// Creates a 2D texture of the specified resolution full of Perlin noise 
        /// 
        /// The new texture is tracked in the textureBuddys disposables collection, and is managed by the TextureBuddy
        /// The new texture is available in the dictionary
        /// </summary>
        /// <param name="name">The name of the texture to add to the dictionary</param>
        /// <param name="resolution">the resolution of the new texture</param>
        /// <returns>the new texture</returns>
        public Texture2D CreateNoiseMap2D(string name,int resolution)
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

            // disposables.Add(noiseImage);
            createdTextures.Add(name, noiseImage);
            return noiseImage;
        }


        public Texture2D FromFile(string fileName)
        {
            if (createdTextures.ContainsKey(fileName))
            {
                return (Texture2D)createdTextures[fileName];
            }

            Texture2D tex;
            if (System.IO.File.Exists(fileName))
            {
                tex = Texture2D.FromFile(device, fileName );
                disposables.Add(tex);
                return tex;
            }
            else
            {
                tex = CreateNoiseMap2D(64);
            }
            createdTextures.Add(fileName, tex);
            return tex;
        }

        public WPFVisualTexture CreateVisualTexture(EmbeddableUserControl visual, int width, int height)
        {
            var tex = new WPFVisualTexture(device, width, height, visual);
            wpfTextures.Add(tex);
            return tex;
        }

        public void RefreshVisualTextures()
        {
            foreach (var tex in wpfTextures)
            {
                if (tex.IsDirty)
                    tex.UpdateVisual();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (IDisposable i in disposables)
            {
                i.Dispose();
            }

            foreach (var p in createdTextures)
            {
                p.Value.Dispose();
            }
        }

        #endregion
    }
}
