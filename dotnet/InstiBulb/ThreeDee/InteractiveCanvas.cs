using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _3DTools;
using System.Windows.Media.Media3D;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
namespace InstiBulb.ThreeDee
{
    public class InteractiveCanvas : InteractiveVisual3D
    {
        //     <MeshGeometry3D x:Key="PlaneMesh" Positions="-1,1,0 -1,-1,0 1,-1,0 1,1,0" TextureCoordinates="0,0 0,1 1,1 1,0" TriangleIndices="0 1 2 0 2 3"/>

        public void JumpTo() { }
        public void Previous() { }
        public void Next() { }

        public InteractiveCanvas()
        {
            Geometry = CreateCanvas();
        }

        internal static MeshGeometry3D CreateCanvas()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(new Point3D(-1, 1, 0));
            mesh.Positions.Add(new Point3D(-1, -1, 0));
            mesh.Positions.Add(new Point3D(1, -1, 0));
            mesh.Positions.Add(new Point3D(1, 1, 0));

            mesh.TextureCoordinates.Add(new System.Windows.Point(0, 0));
            mesh.TextureCoordinates.Add(new System.Windows.Point(0, 1));
            mesh.TextureCoordinates.Add(new System.Windows.Point(1, 1));
            mesh.TextureCoordinates.Add(new System.Windows.Point(1, 0));

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            mesh.Freeze();
            return mesh;

        }
    }
}
