using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D10;
using DXGI = SlimDX.DXGI;
using SlimDX;
using System.Runtime.InteropServices;

namespace SlimDXBindings.Viewer10
{
    public class XamlModel3DMeshFactory : IDisposable
    {
        Device device;
        SlimDX.Direct3D10.Buffer vertices;
        SlimDX.Direct3D10.Buffer indexBuf;
        InputLayout layout;
        ShaderSignature sig;
        int vertexCount;
        int vertexSize;


        public XamlModel3DMeshFactory(Device device, ShaderSignature sig)
        {
            this.device = device;
            this.sig = sig;
        }
        List<Vector3> vertexPositions = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> textureCoords = new List<Vector2>();
        List<int> indices = new List<int>();
        public XamlModel3DMeshFactory Build(string Positions, string Normals, string TexCoords, string Indices)
        {

            InputElement[] inputElements = new SlimDX.Direct3D10.InputElement[]
			{
				new InputElement("POSITION",0,DXGI.Format.R32G32B32A32_Float,0,0),
				new InputElement("COLOR",0,DXGI.Format.R32G32B32A32_Float,16,0),
                new InputElement("TEXCOORD", 0, SlimDX.DXGI.Format.R32G32_Float, 32, 0)
			};



            var p = Positions.Split(new char[] { ' ', '\r', '\n', '\t' }).Where(item => !string.IsNullOrEmpty(item)).ToArray<string>();
            for (int i = 0; i < p.Length; i += 3)
            {
                vertexPositions.Add(new Vector3(float.Parse(p[i]), float.Parse(p[i + 1]), float.Parse(p[i + 2])));
            }
            var t = TexCoords.Split(new char[] { ' ', '\r', '\n', '\t' }).Where(item => !string.IsNullOrEmpty(item)).ToArray<string>();
            for (int i = 0; i < t.Length; i += 2)
            {
                textureCoords.Add(new Vector2(float.Parse(t[i]), float.Parse(t[i + 1])));
            }
            var n = Normals.Split(new char[] { ' ', '\r', '\n', '\t' }).Where(item => !string.IsNullOrEmpty(item)).ToArray<string>();
            for (int i = 0; i < n.Length; i += 3)
            {
                normals.Add(new Vector3(float.Parse(p[i]), float.Parse(p[i + 1]), float.Parse(p[i + 2])));
            }
            var inds = Indices.Split(new char[] { ' ', '\r', '\n', '\t' }).Where(item => !string.IsNullOrEmpty(item)).ToArray<string>();
            for (int i = 0; i < inds.Length; i++)
            {
                indices.Add(int.Parse(inds[i]));
            }

            vertexCount = vertexPositions.Count;
            vertexSize = 40;

            BufferDescription bufferDescription = new BufferDescription();
            bufferDescription.BindFlags = BindFlags.VertexBuffer;
            bufferDescription.CpuAccessFlags = CpuAccessFlags.None;
            bufferDescription.OptionFlags = ResourceOptionFlags.None;
            bufferDescription.SizeInBytes = vertexCount * vertexSize;
            bufferDescription.Usage = ResourceUsage.Default;

            BufferDescription indBufDescription = new BufferDescription();
            indBufDescription.BindFlags = BindFlags.IndexBuffer;
            indBufDescription.CpuAccessFlags = CpuAccessFlags.None;
            indBufDescription.OptionFlags = ResourceOptionFlags.None;
            indBufDescription.SizeInBytes = sizeof(uint) * vertexCount;
            indBufDescription.Usage = ResourceUsage.Default;

            //Mesh mesh = new Mesh(device, inputElements, "POSITION", vertexPositions.Count / 4, indices.Count / 3, MeshFlags.None);

            //using (DataStream vertStream = new DataStream(16 * vertexPositions.Count, true, true))
            //{
            //    vertStream.WriteRange<Vector3>(vertexPositions.ToArray());
            //    vertStream.Position = 0;
            //    mesh.SetVertexData(0, vertStream);
            //}

            //using (DataStream indStream = new DataStream(4 * indices.Count, true, true))
            //{
            //    indStream.WriteRange<int>(indices.ToArray());
            //    indStream.Position = 0;
            //    mesh.SetIndexData( indStream, indices.Count);
            //}


            using (DataStream testBuf = new DataStream(40 * (vertexCount), true, true))
            {
                //testBuf.Write(new Vector4(0.0f, 0.5f, 0.5f, 1.0f));
                //testBuf.Write(new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
                //testBuf.Write(new Vector2(1.0f, 0.0f));

                //testBuf.Write(new Vector4(0.5f, -0.5f, 0.5f, 1.0f));
                //testBuf.Write(new Vector4(0.0f, 1.0f, 0.0f, 1.0f));
                //testBuf.Write(new Vector2(0.0f, 0.0f));

                //testBuf.Write(new Vector4(-0.5f, -0.5f, 0.5f, 1.0f));
                //testBuf.Write(new Vector4(0.0f, 0.0f, 1.0f, 1.0f));
                //testBuf.Write(new Vector2(0.0f, 1.0f));

                for (int i = 0; i < vertexPositions.Count; ++i)
                {
                    testBuf.Write<Vector4>(new Vector4(vertexPositions[i],1.0f));
                    testBuf.Write<Vector4>(new Vector4( normals[i],1.0f));
                    if (textureCoords.Count > i)
                        testBuf.Write<Vector2>(textureCoords[i]);
                    else
                        testBuf.Write<Vector2>(new Vector2(0, 0));
                }
                //// note: be kind, please rewind!
                testBuf.Position = 0;
                vertices = new SlimDX.Direct3D10.Buffer(device, testBuf, bufferDescription);
                testBuf.Close();
            }

            using (DataStream testBuf = new DataStream(sizeof(uint) * (indices.Count), true, true))
            {

                for (int i = 0; i < indices.Count; ++i)
                {
                    testBuf.Write<uint>((uint)indices[i]);
                }
                // note: be kind, please rewind!
                testBuf.Position = 0;
                indexBuf = new SlimDX.Direct3D10.Buffer(device, testBuf, indBufDescription);
            }
            
            
            layout = new InputLayout(device, inputElements, sig);



            return this;
        }

        


        public void DrawSubset(int num)
        {
            //RasterizerState oldState = device.Rasterizer.State;

            //RasterizerState state = RasterizerState.FromDescription(device, new RasterizerStateDescription() 
            //    { CullMode = CullMode.None, FillMode = FillMode.Solid });

            //device.Rasterizer.State = state;            

            device.InputAssembler.SetInputLayout(layout);
            device.InputAssembler.SetPrimitiveTopology(PrimitiveTopology.TriangleList);
            device.InputAssembler.SetVertexBuffers(0, new SlimDX.Direct3D10.VertexBufferBinding(vertices, vertexSize, 0));
            //device.InputAssembler.SetIndexBuffer(indexBuf, SlimDX.DXGI.Format.D32_Float, 0);
            //device.DrawIndexed(indices.Count, 0, 0);

            //device.Rasterizer.State = oldState;
            device.Draw(vertexCount, 0);
        }

        #region IDisposable Members

        public void Dispose()
        {
            vertices.Dispose();
        }

        #endregion
    }
}
