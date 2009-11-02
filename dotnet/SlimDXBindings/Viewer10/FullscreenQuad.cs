using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D10;
using SlimDX;
using DXGI = SlimDX.DXGI;
namespace SlimDXBindings.Viewer10
{
    public class FullscreenQuad : IDisposable
    {
        Device device;
        Vector4 backgroundColor = new Vector4(0.1f, 0.5f, 1.5f, 1.0f);
        InputLayout Layout;
        SlimDX.Direct3D10.Buffer Vertices;
        const int vertexSize = 40;
        const int vertexCount = 4;
        public FullscreenQuad(Device device, ShaderSignature sig)
        {
            this.device = device;
            CreateQuad(sig);
        }


        void CreateQuad(ShaderSignature sig)
        {

            InputElement[] inputElements = new SlimDX.Direct3D10.InputElement[]
			{
				new InputElement("POSITION",0,DXGI.Format.R32G32B32A32_Float,0,0),
				new InputElement("COLOR",0,DXGI.Format.R32G32B32A32_Float,16,0),
                new InputElement("TEXCOORD", 0, SlimDX.DXGI.Format.R32G32_Float, 32, 0)
			};


            Layout = new InputLayout(device, inputElements, sig);

            DataStream stream = new DataStream(vertexCount * vertexSize, true, true);
            stream.Write(new Vector4(-1.0f, -1.0f, 1.0f, 1.0f));
            stream.Write(backgroundColor);
            stream.Write(new Vector2(0.0f, 1.0f));

            stream.Write(new Vector4(-1.0f, 1.0f, 1.0f, 1.0f));
            stream.Write(backgroundColor);
            stream.Write(new Vector2(0.0f, 0.0f));

            stream.Write(new Vector4(1.0f, -1.0f, 1.0f, 1.0f));
            stream.Write(backgroundColor);
            stream.Write(new Vector2(1.0f, 1.0f));

            stream.Write(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            stream.Write(backgroundColor);
            stream.Write(new Vector2(1.0f, 0.0f));


            stream.Position = 0;

            BufferDescription bufferDescription = new BufferDescription();
            bufferDescription.BindFlags = BindFlags.VertexBuffer;
            bufferDescription.CpuAccessFlags = CpuAccessFlags.None;
            bufferDescription.OptionFlags = ResourceOptionFlags.None;
            bufferDescription.SizeInBytes = vertexCount * vertexSize;
            bufferDescription.Usage = ResourceUsage.Default;

            Vertices = new SlimDX.Direct3D10.Buffer(device, stream, bufferDescription);
            stream.Close();
        }

        public void SetupDraw()
        {
            device.InputAssembler.SetInputLayout(Layout);
            device.InputAssembler.SetPrimitiveTopology(PrimitiveTopology.TriangleStrip);
            device.InputAssembler.SetVertexBuffers(0, new SlimDX.Direct3D10.VertexBufferBinding(Vertices, vertexSize, 0));
        }

        public void Draw()
        {
            device.Draw(4, 0);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Vertices.Dispose();
            Layout.Dispose();
        }

        #endregion
    }
}
