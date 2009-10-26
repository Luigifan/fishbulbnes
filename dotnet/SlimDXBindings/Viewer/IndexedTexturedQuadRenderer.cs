using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SlimDX.Direct3D9;
using SlimDX;
using SampleFramework;
using NES.CPU.nitenedo.Interaction;
using System.Drawing;
using NES.CPU.nitenedo;
using System.Windows;
using System.Reflection;
using NES.CPU.PPUClasses;

namespace SlimDXBindings.Viewer
{
    public class IndexedTexturedQuadRenderer : ISlimDXRenderer
    {
        SlimDXControl panel;
        NESMachine nes;

        private Texture _texture;
        private Texture _paletteTexture;
        private Texture _nesVidRAMTexture;

        public IndexedTexturedQuadRenderer(SlimDXControl control, NESMachine nes)
        {
            panel = control;
            this.nes = nes;
            panel.DeviceCreated += new EventHandler(panel_DeviceCreated);
            panel.DeviceDestroyed += new EventHandler(panel_DeviceDestroyed);
            panel.DeviceLost += new EventHandler(panel_DeviceLost);
            panel.DeviceReset += new EventHandler(panel_DeviceReset);
            panel.MainLoop += new EventHandler(panel_MainLoop);
            panel.BackBufferSizeChanged += new EventHandler(panel_BackBufferSizeChanged);

        }

        Mesh mesh;

        public virtual void Render()
        {
            
            

            panel.Device.Clear(ClearFlags.Target, new Color4(System.Drawing.Color.Black), 0, 0);
            panel.Device.Clear(ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            panel.Device.BeginScene();

            UpdateNESTextures();
            

 
            effectC.Begin();
            effectC.BeginPass(0);

            panel.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);

            mesh.DrawSubset(0);
            effectC.EndPass();
            effectC.End();
            panel.Device.EndScene();
        }

        void UpdateNESTextures()
        {
            var rext = _texture.LockRectangle(0, LockFlags.Discard);
            rext.Data.WriteRange<int>(nes.PPU.OutBuffer);
            _texture.UnlockRectangle(0);
            _texture.AddDirtyRectangle(new System.Drawing.Rectangle(0, 0, 256, 256));

            //int[] ram = new int[0x1000];
            //Buffer.BlockCopy(nes.PPU.VidRAM, 0, ram, 0, 0x4000);

            //var ramRect = _nesVidRAMTexture.LockRectangle(0, LockFlags.Discard);
            //ramRect.Data.WriteRange<byte>(nes.PPU.VidRAM);
            //_nesVidRAMTexture.UnlockRectangle(0);
            //_nesVidRAMTexture.AddDirtyRectangle(new Rectangle(0, 0, 0x1000, 1));

            //if (ramRect == null)
            //{
            //    Texture.ToFile(_nesVidRAMTexture, @"d:\nesVidRam.dds", ImageFileFormat.Dds);
            //}

        }


        public static BoundingSphere ComputeBoundingSphere(Mesh mesh)
        {
            List<Vector3> verts = new List<Vector3>(mesh.VertexCount);
            DataStream ds = mesh.LockVertexBuffer(LockFlags.ReadOnly);

            while (ds.Position < ds.Length)
            {
                long oldPos = ds.Position;

                Vector3 pos = ds.Read<Vector3>();
                verts.Add(pos);

                ds.Position = oldPos + mesh.BytesPerVertex;
            }

            mesh.UnlockVertexBuffer();

            return BoundingSphere.FromPoints(verts.ToArray());
        }

        public static BoundingBox ComputeBoundingBox(Mesh mesh)
        {
            List<Vector3> verts = new List<Vector3>(mesh.VertexCount);
            DataStream ds = mesh.LockVertexBuffer(LockFlags.ReadOnly);

            while (ds.Position < ds.Length)
            {
                long oldPos = ds.Position;

                Vector3 pos = ds.Read<Vector3>();
                verts.Add(pos);

                ds.Position = oldPos + mesh.BytesPerVertex;
            }

            mesh.UnlockVertexBuffer();

            return BoundingBox.FromPoints(verts.ToArray());
        }

        public static int FindElementIndex(VertexElement[] elems, DeclarationUsage usage)
        {
            for (int i = 0; i < elems.Length; ++i)
            {
                if (elems[i].Usage == usage)
                    return i;
            }

            return -1;
        }


        // This code uses the methods described in:
        // www.mvps.org/directx/articles/spheremap.htm
        public static void ComputeTexCoords(Device device, ref Mesh mesh, bool useNormals)
        {
            if (useNormals)
            {
                if ((mesh.VertexFormat & VertexFormat.Normal) == 0)
                {
                    mesh.ComputeNormals();
                }
            }

            // Make room for texture coordinates
            // The 'newMesh' variable is not needed anymore. The extra {} ensure that it is not used accidentally
            {
                Mesh newMesh = mesh.Clone(device, mesh.CreationOptions, mesh.VertexFormat | VertexFormat.Texture1);
                mesh.Dispose();
                mesh = newMesh;
            }

            VertexElement[] elems = mesh.GetDeclaration();
            int posElem = FindElementIndex(elems, DeclarationUsage.Position);
            int normalElem = FindElementIndex(elems, DeclarationUsage.Normal);
            int texCoordsElem = FindElementIndex(elems, DeclarationUsage.TextureCoordinate);

            // Needed for positional spherical mapping
            BoundingBox bbox = ComputeBoundingBox(mesh);

            DataStream ds = mesh.LockVertexBuffer(LockFlags.None);

            while (ds.Position < ds.Length)
            {
                long oldPos = ds.Position;

                Vector3 vec;
                if (useNormals)     // Spherical mapping with normals
                {
                    ds.Position += elems[normalElem].Offset;
                    vec = ds.Read<Vector3>();
                    ds.Position = oldPos;
                }
                else                // Spherical mapping with positions
                {
                    ds.Position += elems[posElem].Offset;
                    Vector3 pos = ds.Read<Vector3>();
                    ds.Position = oldPos;

                    Vector3 center = (bbox.Minimum + bbox.Maximum) * 0.5f;
                    vec = Vector3.Normalize(pos - center);
                }

                float u = (float)Math.Asin(vec.X) / (float)Math.PI + 0.5f;
                float v = (float)Math.Asin(vec.Y) / (float)Math.PI + 0.5f;

                ds.Position += elems[texCoordsElem].Offset;
                ds.Write<float>(u);
                ds.Write<float>(v);

                ds.Position = oldPos + mesh.BytesPerVertex;
            }

            mesh.UnlockVertexBuffer();
        }

        public static void ComputeBoxTextureCoords(Device device, ref Mesh mesh, bool useNormals)
        {
            //if (useNormals)
            //{
            //    if ((mesh.VertexFormat & VertexFormat.Normal) == 0)
            //    {
            //        mesh.ComputeNormals();
            //    }
            //}

            // Make room for texture coordinates
            // The 'newMesh' variable is not needed anymore. The extra {} ensure that it is not used accidentally
            {
                Mesh newMesh = mesh.Clone(device, mesh.CreationOptions, mesh.VertexFormat | VertexFormat.Texture1);
                mesh.Dispose();
                mesh = newMesh;
            }

            VertexElement[] elems = mesh.GetDeclaration();
            int posElem = FindElementIndex(elems, DeclarationUsage.Position);
            int normalElem = FindElementIndex(elems, DeclarationUsage.Normal);
            int texCoordsElem = FindElementIndex(elems, DeclarationUsage.TextureCoordinate);

            // Needed for positional spherical mapping
            BoundingBox bbox = ComputeBoundingBox(mesh);

            DataStream ds = mesh.LockVertexBuffer(LockFlags.None);

            while (ds.Position < ds.Length)
            {
                long oldPos = ds.Position;

                Vector3 vec;
                ds.Position += elems[posElem].Offset;
                vec = ds.Read<Vector3>();
                ds.Position = oldPos;

                float u = (vec.X - bbox.Minimum.X) / (bbox.Maximum.X - bbox.Minimum.X) ; 
                float v = (vec.Y - bbox.Minimum.Y) / (bbox.Maximum.Y - bbox.Minimum.Y);  
                
                ds.Position += elems[texCoordsElem].Offset;
                ds.Write<float>(u);
                ds.Write<float>(v);

                ds.Position = oldPos + mesh.BytesPerVertex;
            }

            mesh.UnlockVertexBuffer();
        }


        private void panel_MainLoop(object sender, EventArgs e)
        {
            Render();

            panel.AllowRendering = false;

        }

        private void panel_DeviceCreated(object sender, EventArgs e)
        {
            LoadContent();
        }

        private void panel_DeviceDestroyed(object sender, EventArgs e)
        {

            UnloadContent();
        }

        private void panel_DeviceLost(object sender, EventArgs e)
        {
            UnloadContent();
        }

        private void panel_DeviceReset(object sender, EventArgs e)
        {
            LoadContent();
        }

        Vector2 scaleVector = new Vector2(1, 1);

        void panel_BackBufferSizeChanged(object sender, EventArgs e)
        {
            if (panel.BackBufferHeight == 0)
            {
                camera.AspectRatio = 4 / 3;
            }
            else
            {
                camera.AspectRatio = panel.BackBufferWidth / panel.BackBufferHeight;
            }
        }

        void InitializeD3D()
        {
            panel.Initialize(true);
            if (panel.UseDeviceEx == false)
            {
                throw new InvalidDisplayContextException("You cannot create a Direct3D9Ex device.  You die and you go to hell.");
            }
            InitializeScene();
        }

        void panel_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeD3D();
        }


        static TransformedColoredVertex[] BuildVertexData()
        {
            return new TransformedColoredVertex[4] {
                new TransformedColoredVertex(new Vector4(600.0f, 100.0f, 0.5f, 1.0f), System.Drawing.Color.Red.ToArgb()),
                new TransformedColoredVertex(new Vector4(600.0f, 500.0f, 0.5f, 1.0f), System.Drawing.Color.Blue.ToArgb()),
                new TransformedColoredVertex(new Vector4(150.0f, 500.0f, 0.5f, 1.0f), System.Drawing.Color.Green.ToArgb()), 
                new TransformedColoredVertex(new Vector4(150.0f, 100.0f, 0.5f, 1.0f), System.Drawing.Color.Purple.ToArgb()), 
            };
        }

        Effect effectC;

        VertexBuffer vertices;
        
        protected void LoadContent()
        {

            CleanupContent();

            effectC = Effect.FromStream(panel.Device, 
                Assembly.GetExecutingAssembly().GetManifestResourceStream("SlimDXBindings.Viewer.IndexedRasterize.fx"), 
                ShaderFlags.None);

            mesh = Mesh.CreateBox(panel.Device, 6, 6, 6);
            mesh.ComputeNormals();

            ComputeBoxTextureCoords(panel.Device, ref mesh, true);


            _texture = new Texture(panel.Device, 256, 256, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
            byte[] pal = new byte[256 * 4];
            int[] iPal = PixelWhizzler.GetPalABGR();
            Buffer.BlockCopy(iPal,0,pal,0,1024);
            _paletteTexture = new Texture(panel.Device, 256, 1, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);

            _nesVidRAMTexture = new Texture(panel.Device, 0x1000, 1, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);

            var rext = _paletteTexture.LockRectangle(0, LockFlags.Discard);
            rext.Data.WriteRange<int>(PixelWhizzler.GetPalABGR(), 0, 256);

            _paletteTexture.UnlockRectangle(0);
            _paletteTexture.AddDirtyRectangle(new System.Drawing.Rectangle(0, 0, 256, 1));


            InitializeScene();

        }

        private void CleanupContent()
        {
            if (effectC != null) effectC.Dispose();
            if (mesh != null) mesh.Dispose();
            if (_texture != null) _texture.Dispose();
            if (_nesVidRAMTexture != null) _nesVidRAMTexture.Dispose();
            if (_paletteTexture != null) _paletteTexture.Dispose();
        }

        protected void UnloadContent()
        {
            CleanupContent();
        }

        Camera camera = new Camera();
        bool pointLight = false;

        public void InitializeScene()
        {
            CreateLight();
            
            camera.FieldOfView = (float)(Math.PI / 4);
            camera.NearPlane = 0.0f;
            camera.FarPlane = 40.0f;
            camera.Location = new Vector3(0.0f, 0.0f, 10.5f);
            camera.Target = Vector3.Zero;

            effectC.Technique = "TVertexShaderOnly";
            Matrix wvp = Matrix.Multiply(Matrix.RotationZ((float)Math.PI), camera.ViewMatrix);
            wvp = Matrix.Multiply(wvp, camera.ProjectionMatrix);
            effectC.SetValue("matWorldViewProj", wvp);
            effectC.SetValue("matWorld", Matrix.RotationZ((float)Math.PI));
            effectC.SetTexture("nesTexture", _texture);
            effectC.SetTexture("nesPalette", _paletteTexture);
        }

        Light light;
        void CreateLight()
        {
            light = new Light();
            light.Type = LightType.Directional;
            light.Diffuse = System.Drawing.Color.White;
            light.Ambient = System.Drawing.Color.White;
            light.Direction = new Vector3(0.0f, -7.0f, -20.0f);
        }

        #region IDisposable Members

        public void Dispose()
        {
            CleanupContent();
        }

        #endregion

        #region ISlimDXRenderer Members


        NESPixelFormats pizelFormat = NESPixelFormats.Indexed; 
        public NESPixelFormats PixelFormat
        {
            get { return pizelFormat; }
        }

        #endregion
    }
}
