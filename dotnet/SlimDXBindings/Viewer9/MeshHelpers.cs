using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using SlimDX;

namespace SlimDXBindings.Viewer
{
    public static class MeshHelpers
    {
        // This code uses the methods described in:
        // www.mvps.org/directx/articles/spheremap.htm
        public static void ComputeTexCoords(this Device device, ref Mesh mesh,  bool useNormals)
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

        public static void ComputeBoxTextureCoords(this Device device, ref Mesh mesh, bool useNormals)
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

                float u = (vec.X - bbox.Minimum.X) / (bbox.Maximum.X - bbox.Minimum.X);
                float v = (vec.Y - bbox.Minimum.Y) / (bbox.Maximum.Y - bbox.Minimum.Y);

                ds.Position += elems[texCoordsElem].Offset;
                ds.Write<float>(u);
                ds.Write<float>(v);

                ds.Position = oldPos + mesh.BytesPerVertex;
            }

            mesh.UnlockVertexBuffer();
        }

        public static BoundingSphere ComputeBoundingSphere(this Mesh mesh)
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

        public static BoundingBox ComputeBoundingBox(this Mesh mesh)
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

    }
}
