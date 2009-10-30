using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using System.Runtime.InteropServices;

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

            verts[0].Position.X = v2.X; //-1
            verts[0].Position.Y = v1.Y; //1

            verts[1].Position.X = v1.X; //1
            verts[1].Position.Y = v1.Y; //1

            verts[2].Position.X = v1.X; //1
            verts[2].Position.Y = v2.Y; //-1

            verts[3].Position.X = v2.X; //-1
            verts[3].Position.Y = v2.Y; //-1

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

        static VertexPositionTexture[] BasicQuad = new VertexPositionTexture[]
            {
                new VertexPositionTexture(
                    new SlimDX.Vector4(0,0,1,0),
                    new SlimDX.Vector2(1,1)),
                new VertexPositionTexture(
                    new SlimDX.Vector4(0,0,1,0),
                    new SlimDX.Vector2(0,1)),
                new VertexPositionTexture(
                    new SlimDX.Vector4(0,0,1,0),
                    new SlimDX.Vector2(0,0)),
                new VertexPositionTexture(
                    new SlimDX.Vector4(0,0,1,0),
                    new SlimDX.Vector2(1,0))
            };

        static short[] ib = new short[] { 0, 1, 2, 2, 3, 0 };

        public void Draw()
        {
            device.VertexFormat = (VertexFormat.PositionW | VertexFormat.Texture0 );
            device.VertexDeclaration = vd;

            device.DrawIndexedUserPrimitives<short, VertexPositionTexture>
                (PrimitiveType.TriangleList, 0, 4, 2, ib, Format.Index16, verts, Marshal.SizeOf(typeof(VertexPositionTexture)));

        }

        #region IDisposable Members

        public void Dispose()
        {
            vd.Dispose();
        }

        #endregion
    }


}
