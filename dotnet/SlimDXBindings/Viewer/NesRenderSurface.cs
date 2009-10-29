using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using SlimDX;
using System.Drawing;
using NES.CPU.nitenedo;
using System.Reflection;
using NES.CPU.PPUClasses;

namespace SlimDXBindings.Viewer
{
    public class NesRenderSurface : IDisposable
    {

        NESMachine nes;
        Surface surf;
        RenderToSurface rSurf;
        Viewport vp;
        Effect effectC;
        Mesh mesh;

        private Texture _texture;
        private Texture _paletteTexture;
        Texture _nesRenderSurface;

        public Texture SurfaceTexture
        {
            get { return _nesRenderSurface; }
        }

        public NesRenderSurface(NESMachine nes, Device device)
        {
            this.nes = nes;
            LoadContent(device);
        }

        void LoadContent(Device device)
        {
            rSurf = new RenderToSurface(device, 256, 256, Format.A8R8G8B8);
            _nesRenderSurface = new Texture(device, 256, 256, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            
            // setup render target texture
            surf = _nesRenderSurface.GetSurfaceLevel(0);
            vp = new Viewport(0, 0, 256, 256, 0f, 1.0f);

            effectC = Effect.FromStream(rSurf.Device,
                Assembly.GetExecutingAssembly().GetManifestResourceStream("SlimDXBindings.Viewer.IndexedRasterize.fx"),
                ShaderFlags.None);

            //_sprite = new Sprite(rSurf.Device);

            mesh = Mesh.CreateBox(rSurf.Device, 2, 2, 2);
            mesh.ComputeNormals();
            MeshHelpers.ComputeBoxTextureCoords(rSurf.Device, ref mesh, true);

            _texture = new Texture(rSurf.Device, 256, 256, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);

            CreatePaletteTexture();

            effectC.Technique = "TVertexShaderOnly";

            SampleFramework.Camera camera = new SampleFramework.Camera();
            camera.FieldOfView = (float)(Math.PI / 4);
            camera.NearPlane = 0.0f;
            camera.FarPlane = 40.0f;
            camera.Location = new Vector3(0.0f, 0.0f, 3.41f);
            camera.Target = Vector3.Zero;
            camera.AspectRatio = 256 / 256;
            
            Matrix wvp = Matrix.Multiply(Matrix.RotationZ((float)Math.PI), camera.ViewMatrix);
            wvp = Matrix.Multiply(wvp, camera.ProjectionMatrix);

            effectC.SetValue("matWorldViewProj", wvp);
            effectC.SetValue("matWorld", Matrix.RotationZ((float)Math.PI));
            effectC.SetTexture("nesTexture", _texture);
            effectC.SetTexture("nesPalette", _paletteTexture);
        }

        private void CreatePaletteTexture()
        {
            _paletteTexture = new Texture(rSurf.Device, 256, 1, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);

            byte[] pal = new byte[256 * 4];
            int[] iPal = PixelWhizzler.GetPalABGR();
            Buffer.BlockCopy(iPal, 0, pal, 0, 1024);

            var rext = _paletteTexture.LockRectangle(0, LockFlags.Discard);
            rext.Data.WriteRange<int>(PixelWhizzler.GetPalABGR(), 0, 256);
            _paletteTexture.UnlockRectangle(0);
            _paletteTexture.AddDirtyRectangle(new System.Drawing.Rectangle(0, 0, 256, 1));
        }

        public void RenderNESToSurface()
        {
            rSurf.Device.Clear(ClearFlags.Target, new Color4(System.Drawing.Color.Yellow), 0, 0);
            rSurf.Device.Clear(ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            rSurf.BeginScene(surf, vp);

            UpdateNESTextures();

            effectC.Begin();
            effectC.BeginPass(0);
            
            mesh.DrawSubset(0);

            effectC.EndPass();
            effectC.End();

            rSurf.EndScene(Filter.Point);
        }

        void UpdateNESTextures()
        {
            var rext = _texture.LockRectangle(0, LockFlags.Discard);
            rext.Data.WriteRange<int>(nes.PPU.OutBuffer);
            _texture.UnlockRectangle(0);
            _texture.AddDirtyRectangle(new System.Drawing.Rectangle(0, 0, 256, 256));

        }

        #region IDisposable Members

        public void Dispose()
        {
            effectC.Dispose();
            //_sprite.Dispose();
            mesh.Dispose();
            _texture.Dispose();
            _paletteTexture.Dispose();
            _nesRenderSurface.Dispose();
            rSurf.Dispose();
            surf.Dispose();

        }

        #endregion
    }
}
