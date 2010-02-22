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
using System.Runtime.InteropServices;
using System.Globalization;

namespace SlimDXBindings.Viewer
{
    public class VertexBasedRenderer : ISlimDXRenderer
    {
        SlimDXControl panel;
        NESMachine nes;

        private Texture _texture;
        private Texture _paletteTexture;
        private Mesh mesh;

        public VertexBasedRenderer(SlimDXControl control, NESMachine nes)
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
            
            

            panel.Device.Clear(ClearFlags.Target, new Color4(System.Drawing.Color.Black), 0, 0);
            panel.Device.Clear(ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            panel.Device.BeginScene();

            var rext = _texture.LockRectangle(0, LockFlags.Discard);
            rext.Data.WriteRange<int>(nes.PPU.OutBuffer);

            _texture.UnlockRectangle(0);
            _texture.AddDirtyRectangle(new System.Drawing.Rectangle(0, 0, 256, 256));
            
            effectC.Technique = "TVertexShaderOnly";
            Matrix wvp = Matrix.Multiply(Matrix.RotationZ((float)Math.PI), camera.ViewMatrix);
            wvp = Matrix.Multiply(wvp, camera.ProjectionMatrix);
            
            effectC.SetValue("matWorldViewProj", wvp);
            effectC.SetValue("matWorld", Matrix.RotationZ((float)Math.PI));
            effectC.SetTexture("nesTexture", _texture);
            effectC.SetTexture("nesPalette", _paletteTexture);
 
            effectC.Begin();
            effectC.BeginPass(0);

            mesh.DrawSubset(0);
            
            effectC.EndPass();
            effectC.End();
            panel.Device.EndScene();
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


        /// <summary>
        ///  custom vertex type, position, texcoord, color
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct TransformedColoredTexturedVertex : IEquatable<TransformedColoredTexturedVertex>
        {
            /// <summary>
            /// Gets or sets the transformed position of the vertex.
            /// </summary>
            /// <value>The transformed position of the vertex.</value>
            [VertexElement(DeclarationType.Float4, DeclarationUsage.PositionTransformed)]
            public Vector4 Position
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the texture coordinate of the vertex
            /// </summary>
            [VertexElement(DeclarationType.Float2, DeclarationUsage.TextureCoordinate)]
            public Vector2 TexCoord
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the color of the vertex.
            /// </summary>
            /// <value>The color of the vertex.</value>
            [VertexElement(DeclarationType.Color, DeclarationUsage.Color)]
            public int Color
            {
                get;
                set;
            }

 

            /// <summary>
            /// Gets the size in bytes.
            /// </summary>
            /// <value>The size in bytes.</value>
            public static int SizeInBytes
            {
                get { return Marshal.SizeOf(typeof(TransformedColoredTexturedVertex)); }
            }

            /// <summary>
            /// Gets the format.
            /// </summary>
            /// <value>The format.</value>
            public static VertexFormat Format
            {
                get { return VertexFormat.PositionRhw | VertexFormat.Diffuse | VertexFormat.Texture1; }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="TransformedColoredVertex"/> struct.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="color">The color.</param>
            public TransformedColoredTexturedVertex(Vector4 position, Vector2 texturePosition, int color)
                : this()
            {
                Position = position;
                Color = color;
                TexCoord = texturePosition;
            }

            /// <summary>
            /// Implements the operator ==.
            /// </summary>
            /// <param name="left">The left side of the operator.</param>
            /// <param name="right">The right side of the operator.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(TransformedColoredTexturedVertex left, TransformedColoredTexturedVertex right)
            {
                return left.Equals(right);
            }

            /// <summary>
            /// Implements the operator !=.
            /// </summary>
            /// <param name="left">The left side of the operator.</param>
            /// <param name="right">The right side of the operator.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator !=(TransformedColoredTexturedVertex left, TransformedColoredTexturedVertex right)
            {
                return !(left == right);
            }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            /// <returns>
            /// A 32-bit signed integer that is the hash code for this instance.
            /// </returns>
            public override int GetHashCode()
            {
                return Position.GetHashCode() + Color.GetHashCode() + TexCoord.GetHashCode();
            }

            /// <summary>
            /// Indicates whether this instance and a specified object are equal.
            /// </summary>
            /// <param name="obj">Another object to compare to.</param>
            /// <returns>
            /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                if (GetType() != obj.GetType())
                    return false;

                return Equals((TransformedColoredTexturedVertex)obj);
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            public bool Equals(TransformedColoredTexturedVertex other)
            {
                return (Position == other.Position && Color == other.Color);
            }

            /// <summary>
            /// Returns a string representation of the current object.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.String"/> representing the vertex.
            /// </returns>
            public override string ToString()
            {
                return string.Format(CultureInfo.CurrentCulture, "{0} ({1}) ({2})", Position.ToString(), System.Drawing.Color.FromArgb(Color).ToString(), TexCoord.ToString() );
            }
        }

        Effect effectC;

        VertexBuffer vertices;
        IndexBuffer indexBuffer;

        protected void LoadContent()
        {
            InitializeScene();

            if (effectC != null) effectC.Dispose();
            effectC = Effect.FromStream(panel.Device, Assembly.GetExecutingAssembly().GetManifestResourceStream("SlimDXBindings.Viewer.VertexRasterize.fx"), ShaderFlags.None );

            if (mesh != null) mesh.Dispose();
            mesh = Mesh.FromFile(panel.Device, @"D:\Projects\FishBulb2010\dotnet\SlimDXBindings\Viewer\screenMesh.x", MeshFlags.Use32Bit) ;

            panel.Device.SetRenderState(RenderState.CullMode, Cull.None);

            if (_texture != null) _texture.Dispose();
            _texture = new Texture(panel.Device, 256, 256, 1, Usage.Dynamic, Format.X8R8G8B8, Pool.Default);
            if (_paletteTexture != null) _paletteTexture.Dispose();
            _paletteTexture = new Texture(panel.Device, 256, 1, 1, Usage.Dynamic, Format.X8R8G8B8, Pool.Default);

            var rext = _paletteTexture.LockRectangle(0, LockFlags.Discard);
            rext.Data.WriteRange<int>(PixelWhizzler.GetPalABGR(),0,256);

            _paletteTexture.UnlockRectangle(0);
            _paletteTexture.AddDirtyRectangle(new System.Drawing.Rectangle(0, 0, 256, 1));


        }

        static VertexElement[] vertexDecl = new VertexElement[]
            {
                new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                new VertexElement(0, 16, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                new VertexElement(0, 24, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0)
            };


        void CreateVertexPanel()
        {

            int vHeight = 8;
            int vWidth = 8;
            int Height = 7;
            int Width = 7;

            TransformedColoredTexturedVertex[] verts = new TransformedColoredTexturedVertex[vHeight * vWidth];
            Vector4 pos = new Vector4();
            Vector3 minBounds = new Vector3(0,0,0);
            Vector3 maxBounds = new Vector3(1, 1, 1);
            float stepX = (maxBounds.X - minBounds.X) / (float)Height;
            float stepZ = (maxBounds.Z - minBounds.Z) / (float)Width;
            
            int count = 0;
            // Loop across and up
            for (int z = 0; z < vHeight; z++)
            {
                pos.X = minBounds.X;
                for (int x = 0; x < vWidth; x++)
                {
                    // Create the verts
                    verts[count].Position = pos;
                    // Increment x across
                    pos.X += stepX;
                    count++;
                }
                // Increment Z
                pos.Z += stepZ;
            }

            vertices = new VertexBuffer(panel.Device, TransformedColoredTexturedVertex.SizeInBytes * verts.Length, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            DataStream s = vertices.Lock(0, 0, LockFlags.None);
            s.WriteRange<TransformedColoredTexturedVertex>(verts,0,0);
            vertices.Unlock();


            int[] indices = new int[vHeight * vWidth * 6];
            count = 0;
            int vIndex = 0;
            for (int z = 0; z < Height; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // first triangle
                    indices[count++] = vIndex;
                    indices[count++] = vIndex + vWidth;
                    indices[count++] = vIndex + vWidth + 1;

                    // second triangle
                    indices[count++] = vIndex;
                    indices[count++] = vIndex + vWidth + 1;
                    indices[count++] = vIndex + 1;

                    vIndex++;
                }
                vIndex++;
            }

            indexBuffer = new IndexBuffer(panel.Device, sizeof(int) * indices.Length, Usage.WriteOnly, Pool.Default, false);
            DataStream i = indexBuffer.Lock(0, 0, LockFlags.None);
            i.WriteRange<int>(indices, 0, 0);
            indexBuffer.Unlock();

        }


        protected void UnloadContent()
        {

            if (effectC != null)
                effectC.OnLostDevice();
            if (_texture != null)
                _texture.Dispose();
            if (_paletteTexture != null)
                _paletteTexture.Dispose();
        }

        Camera camera = new Camera();
        bool pointLight = false;

        public void InitializeScene()
        {
            CreateLight();
            
            camera.FieldOfView = (float)(Math.PI / 4);
            camera.NearPlane = 0.0f;
            camera.FarPlane = 40.0f;
            camera.Location = new Vector3(0.0f, 12.0f, 0.0f);
            camera.Target = Vector3.Zero;
            
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
            _texture.Dispose();
            _paletteTexture.Dispose();
            effectC.Dispose();
        }

        #endregion

        #region ISlimDXRenderer Members


        NESPixelFormats pizelFormat = NESPixelFormats.BGR; 
        public NESPixelFormats PixelFormat
        {
            get { return pizelFormat; }
        }

        #endregion
    }
}
