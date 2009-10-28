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
        private Texture _nesRenderSurface;

        Effect effectC;
        Effect postEffect;

        VertexBuffer vertices;

        Surface surf ;
        RenderToSurface rSurf ;
        Viewport vp ;

        Mesh mesh;
        Mesh mesh2;

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

        

        public virtual void Render()
        {

            rSurf.Device.Clear(ClearFlags.Target, new Color4(System.Drawing.Color.Black), 0, 0);
            rSurf.Device.Clear(ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            rSurf.BeginScene(surf, vp);
            //rSurf.Device.SetRenderState<System.Drawing.Color>(RenderState.Ambient, System.Drawing.Color.White);
            rSurf.Device.SetRenderState<Cull>(RenderState.CullMode, Cull.None);
            rSurf.Device.SetRenderState(RenderState.Lighting, false);

            UpdateNESTextures();

            effectC.Begin();
            effectC.BeginPass(0);
            mesh2.DrawSubset(0);
            effectC.EndPass();
            effectC.End();

            rSurf.EndScene(Filter.Point);
            
            // now render the texture created above onto the actual screen

            panel.Device.Clear(ClearFlags.Target, new Color4(System.Drawing.Color.Black), 0, 0);
            panel.Device.Clear(ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            panel.Device.BeginScene();

            postEffect.Begin();
            postEffect.BeginPass(0);
            mesh.DrawSubset(0);
            postEffect.EndPass();
            postEffect.End();

            panel.Device.EndScene();
        }

        void UpdateNESTextures()
        {
            var rext = _texture.LockRectangle(0, LockFlags.Discard);
            rext.Data.WriteRange<int>(nes.PPU.OutBuffer);
            _texture.UnlockRectangle(0);
            _texture.AddDirtyRectangle(new System.Drawing.Rectangle(0, 0, 256, 256));

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

        static TransformedColoredTexturedVertex[] GetFSQuad()
        {
            return new TransformedColoredTexturedVertex[4]
            {
                new TransformedColoredTexturedVertex(new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 
                    System.Drawing.Color.White.ToArgb(), 
                    new Vector2(1.0f, 0.0f)),
                new TransformedColoredTexturedVertex(new Vector4(1.0f, -1.0f, 1.0f, 1.0f), 
                    System.Drawing.Color.White.ToArgb(), 
                    new Vector2(1.0f, 1.0f)),
                new TransformedColoredTexturedVertex(new Vector4(-1.0f, 1.0f, 1.0f, 1.0f), 
                    System.Drawing.Color.White.ToArgb(), 
                    new Vector2(0.0f, 0.0f)),
                new TransformedColoredTexturedVertex(new Vector4(-1.0f, -1.0f, 1.0f, 1.0f), 
                    System.Drawing.Color.White.ToArgb(), 
                    new Vector2(0.0f, 1.0f))
            };

        }


        //protected VertexBuffer CreateFullScreenQuad(VertexDeclaration vertexDeclaration)
        //{
        //    // Create a vertex buffer for the quad, and fill it in
        //    VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, typeof(QuadVertex), vertexDeclaration.GetVertexStrideSize(0) * 4, BufferUsage.None);
        //    QuadVertex[] vbData = new QuadVertex[4];

        //    // Upper right
        //    vbData[0].position = new Vector3(1, 1, 1);
        //    vbData[0].texCoordAndCornerIndex = new Vector3(1, 0, 1);

        //    // Lower right
        //    vbData[1].position = new Vector3(1, -1, 1);
        //    vbData[1].texCoordAndCornerIndex = new Vector3(1, 1, 2);

        //    // Upper left
        //    vbData[2].position = new Vector3(-1, 1, 1);
        //    vbData[2].texCoordAndCornerIndex = new Vector3(0, 0, 0);

        //    // Lower left
        //    vbData[3].position = new Vector3(-1, -1, 1);
        //    vbData[3].texCoordAndCornerIndex = new Vector3(0, 1, 3);


        //    vertexBuffer.SetData(vbData);
        //    return vertexBuffer;
        //}


        protected void LoadContent()
        {

            CleanupContent();

            // setup render target texture
            _nesRenderSurface = new Texture(panel.Device, 512, 512, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            surf = _nesRenderSurface.GetSurfaceLevel(0);
            rSurf = new RenderToSurface(panel.Device, 512, 512, Format.A8R8G8B8);
            vp = new Viewport(0, 0, 512, 512, 0f, 1.0f);

            //vertices = new VertexBuffer(panel.Device, 4 * TransformedColoredTexturedVertex.SizeInBytes, Usage.Dynamic, VertexFormat.None, Pool.Default);
            //DataStream stream = vertices.Lock(0, 0, LockFlags.None);
            //stream.WriteRange(GetFSQuad());
            //vertices.Unlock();

            effectC = Effect.FromStream(rSurf.Device, 
                Assembly.GetExecutingAssembly().GetManifestResourceStream("SlimDXBindings.Viewer.IndexedRasterize.fx"), 
                ShaderFlags.None);

            postEffect = Effect.FromStream(panel.Device,
                Assembly.GetExecutingAssembly().GetManifestResourceStream("SlimDXBindings.Viewer.Rasterize.fx"),
                ShaderFlags.None);

            mesh = Mesh.CreateBox(panel.Device, 2, 2, 2);
            //mesh = Mesh.CreateSphere(panel.Device, 1, 32, 16);
            mesh.ComputeNormals();
            //ComputeTexCoords(panel.Device, ref mesh, true);
            ComputeBoxTextureCoords(panel.Device, ref mesh, true);

            mesh2 = Mesh.CreateBox(rSurf.Device, 2, 2, 2);
            mesh2.ComputeNormals();
            ComputeBoxTextureCoords(rSurf.Device, ref mesh2, true);

            _texture = new Texture(rSurf.Device, 256, 256, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
            byte[] pal = new byte[256 * 4];
            int[] iPal = PixelWhizzler.GetPalABGR();
            Buffer.BlockCopy(iPal,0,pal,0,1024);
            _paletteTexture = new Texture(rSurf.Device, 256, 1, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);

            _nesVidRAMTexture = new Texture(rSurf.Device, 0x1000, 1, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);

            var rext = _paletteTexture.LockRectangle(0, LockFlags.Discard);
            rext.Data.WriteRange<int>(PixelWhizzler.GetPalABGR(), 0, 256);

            _paletteTexture.UnlockRectangle(0);
            _paletteTexture.AddDirtyRectangle(new System.Drawing.Rectangle(0, 0, 256, 1));





            InitializeScene();

        }

        private void CleanupContent()
        {
            if (effectC != null) effectC.Dispose();
            if (postEffect != null) postEffect.Dispose();
            if (surf != null) surf.Dispose();
            if (rSurf != null) rSurf.Dispose();
            if (vertices != null) vertices.Dispose();
            if (mesh != null) mesh.Dispose();
            if (_texture != null) _texture.Dispose();
            if (_nesVidRAMTexture != null) _nesVidRAMTexture.Dispose();
            if (_paletteTexture != null) _paletteTexture.Dispose();
            if (_nesRenderSurface != null) _nesRenderSurface.Dispose();

        //            private Texture _nesRenderSurface;

        //Effect effectC;
        //Effect postEffect;

        //VertexBuffer vertices;

        //Surface surf ;
        //RenderToSurface rSurf ;
        //Viewport vp ;
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
            camera.Location = new Vector3(0.0f, 0.0f, 3.5f);
            camera.Target = Vector3.Zero;

            effectC.Technique = "TVertexShaderOnly";
            Matrix wvp = Matrix.Multiply(Matrix.RotationZ((float)Math.PI), camera.ViewMatrix);
            wvp = Matrix.Multiply(wvp, camera.ProjectionMatrix);


            effectC.SetValue("matWorldViewProj", wvp);
            effectC.SetValue("matWorld", Matrix.RotationZ((float)Math.PI));
            effectC.SetTexture("nesTexture", _texture);
            effectC.SetTexture("nesPalette", _paletteTexture);

            postEffect.SetValue("matWorldViewProj", wvp);
            postEffect.SetValue("matWorld", Matrix.RotationZ((float)Math.PI));
            postEffect.SetTexture("nesTexture", _nesRenderSurface);

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
