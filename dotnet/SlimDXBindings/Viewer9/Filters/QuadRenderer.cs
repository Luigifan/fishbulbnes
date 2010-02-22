using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using System.Runtime.InteropServices;
using SlimDX;

namespace SlimDXBindings.Viewer.Filters
{
    public class QuadRenderer  : IDisposable
    {
        readonly Device device;
        VertexDeclaration vd;
        SlimDX.Vector2 v1; SlimDX.Vector2 v2;
        VertexPositionTexture[] verts = BasicQuad;


        public QuadRenderer(Device device, SlimDX.Vector2 v1, SlimDX.Vector2 v2)
        {
            this.device = device;
            vd = new VertexDeclaration(device, vertexDecl);
            this.v1 = v1;
            this.v2 = v2;

            //VertexBufferDescription description = new VertexBufferDescription();
            //description.FVF = (VertexFormat.PositionW | VertexFormat.Texture0);
            //description.Pool = Pool.Default;
            //description.SizeInBytes = Marshal.SizeOf(typeof(VertexPositionTexture)) * 4;
            //description.Type = ResourceType.VertexBuffer;
            //description.Usage = Usage.Dynamic;

            //VertexBuffer buffer = new VertexBuffer(device, Marshal.SizeOf(typeof(VertexPositionTexture)) * 4,
            //    Usage.Dynamic, (VertexFormat.PositionW | VertexFormat.Texture0), Pool.Default);

            //DataStream stream = buffer.Lock(0, Marshal.SizeOf(typeof(VertexPositionTexture)) * 4, LockFlags.Discard);
            //stream.WriteRange<VertexPositionTexture>(BasicQuad);
            //stream.Position = 0;
            //buffer.Unlock();
            
            //verts[0].Position.X = v2.X; //-1
            //verts[0].Position.Y = v1.Y; //1

            //verts[1].Position.X = v1.X; //1
            //verts[1].Position.Y = v1.Y; //1

            //verts[2].Position.X = v1.X; //1
            //verts[2].Position.Y = v2.Y; //-1

            //verts[3].Position.X = v2.X; //-1
            //verts[3].Position.Y = v2.Y; //-1

        }

        [StructLayout(LayoutKind.Sequential)]
        private struct VertexPositionTexture
        {
            public SlimDX.Vector4 Position;
            public SlimDX.Vector2 Texture;

            public VertexPositionTexture(SlimDX.Vector4 Position, SlimDX.Vector2 Texture)
            {
                this.Position = Position;
                this.Texture = Texture;
            }
        }

        static VertexElement[] vertexDecl = new VertexElement[]
            {
                new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
                new VertexElement(0, 16, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                VertexElement.VertexDeclarationEnd
            };


            //        vertexPositions.Add(new Vector4(-0.5f, -0.5f, 0.5f, 1.0f));
            //vertexPositions.Add(new Vector4(-0.5f, 0.5f, 0.5f, 1.0f));
            //vertexPositions.Add(new Vector4(0.5f, -0.5f, 0.5f, 1.0f));
            //vertexPositions.Add(new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
            
            //textureCoords.Add(new Vector2(0.0f, 1.0f));
            //textureCoords.Add(new Vector2(0.0f, 0.0f));
            //textureCoords.Add(new Vector2(1.0f, 1.0f));
            //textureCoords.Add(new Vector2(1.0f, 0.0f));

        static VertexPositionTexture[] BasicQuad = new VertexPositionTexture[]
            {
                new VertexPositionTexture(
                    new Vector4(-0.5f, -0.5f, 0.5f, 1.0f),
                    new Vector2(0.0f, 1.0f)),
                new VertexPositionTexture(
                    new Vector4(-0.5f, 0.5f, 0.5f, 1.0f),
                    new Vector2(0.0f, 0.0f)),
                new VertexPositionTexture(
                    new Vector4(0.5f, -0.5f, 0.5f, 1.0f),
                    new Vector2(1.0f, 1.0f)),
                new VertexPositionTexture(
                    new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
                    new Vector2(1.0f, 0.0f))
            };

        static short[] ib = new short[] { 0, 1, 2, 2, 3, 0 };

        public void Draw()
        {
            device.VertexFormat = (VertexFormat.PositionW | VertexFormat.Texture1 );
            device.VertexDeclaration = vd;

            device.DrawIndexedUserPrimitives<short, VertexPositionTexture>
                (PrimitiveType.TriangleStrip, 0, 4, 2, ib, Format.Index16, verts, Marshal.SizeOf(typeof(VertexPositionTexture)));

        }

        VertexBuffer _vb;
        int _totalNumberOfVertices;



        #region IDisposable Members

        public void Dispose()
        {
            vd.Dispose();
        }

        #endregion
    }


}
