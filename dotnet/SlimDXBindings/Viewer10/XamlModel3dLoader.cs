using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D10;
using DXGI = SlimDX.DXGI;
using SlimDX;

namespace SlimDXBindings.Viewer10
{
    public class XamlModel3DMeshFactory 
    {
        Device device;

        public XamlModel3DMeshFactory(Device device)
        {
            this.device = device;
        }

        public Mesh BuildMesh(string Positions, string Normals, string TexCoords, string Indices)
        {

            InputElement[] inputElements = new SlimDX.Direct3D10.InputElement[]
			{
				new InputElement("POSITION",0,DXGI.Format.R32G32B32A32_Float,0,0),
				new InputElement("COLOR",0,DXGI.Format.R32G32B32A32_Float,16,0),
                new InputElement("TEXCOORD", 0, SlimDX.DXGI.Format.R32G32_Float, 32, 0)
			};

            List<Vector3> vertexPositions = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> textureCoords = new List<Vector2>();
            List<int> indices = new List<int>();

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
            
            

            Mesh mesh = new Mesh(device, inputElements, "POSITION", vertexPositions.Count / 4, indices.Count / 3, MeshFlags.None);

            using (DataStream testBuf = new DataStream(48 * (vertexPositions.Count ), true, true))
            {

                for (int i = 0; i < vertexPositions.Count; ++i)
                {
                    testBuf.Write<Vector3>(vertexPositions[i]);
                    testBuf.Write<Vector3>(normals[i]);
                    testBuf.Write<Vector2>(textureCoords[i]);
                }
                // note: be kind, please rewind!
                testBuf.Position = 0;
                mesh.SetPointRepresentationData(testBuf);
            }

            mesh.Commit();

            return mesh;
        }
    }
}
