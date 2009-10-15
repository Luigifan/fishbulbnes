using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using _3DTools;
using System.Windows.Media.Media3D;
using System.Windows;

namespace InstiBulb.ThreeDee
{
    public class InteractiveCylinder : InteractiveVisual3D
    {

        List<double> angleLocks;

        public InteractiveCylinder()
        {
            angleLocks = new List<double>();
            Geometry = Tessellate(2, 4, 64, angleLocks);
        }

        public void RebuildGeometry(int segments, int slicesPerSegment, int ySlices)
        {
            List<double> angles = new List<double>();
            Geometry = Tessellate(slicesPerSegment, segments * slicesPerSegment, ySlices, angles);
            angles.Remove(360);
            double angle = 180 / segments;
            angleLocks = new List<double>();
            foreach (double d in angles)
            {
                angleLocks.Add(d + angle);
            }
        }

        internal static Point3D GetPosition(double t, double y)
        {
            double x = Math.Cos(t);
            double z = Math.Sin(t);

            return new Point3D(x , y , z );
        }
        
        private static Vector3D GetNormal(double t, double y)
        {
            double x = Math.Cos(t);
            double z = Math.Sin(t);

            return new Vector3D(x, 0, z);
        }

        internal static double DegToRad(double degrees)
        {
            return (degrees / 180.0) * Math.PI;
        }

        private static Point GetTextureCoordinate(double t, double y)
        {
            return new Point(1.0 - t * 1 / (2 * Math.PI), y * -0.5 + 0.5);
        }

        internal static MeshGeometry3D Tessellate(int divider, int tDiv, int yDiv, List<double> angleLocks)
        {
            double maxTheta = DegToRad(360.0);
            double minY = -1.0;
            double maxY = 1.0;

            double dt = maxTheta / tDiv;
            double dy = (maxY - minY) / yDiv;


            for (int ti = 0; ti <= tDiv; ti++)
            {
                double t = ti * dt;

                if (ti % divider == 0)
                    angleLocks.Add(t / Math.PI * 180);
            }

            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int yi = 0; yi <= yDiv; yi++)
            {
                double y = minY + yi * dy;

                for (int ti = 0; ti <= tDiv; ti++)
                {
                    double t = ti * dt;

                    mesh.Positions.Add(GetPosition(t, y));
                    mesh.Normals.Add(GetNormal(t, y));
                    mesh.TextureCoordinates.Add(GetTextureCoordinate(t, y));
                }
            }

            for (int yi = 0; yi < yDiv; yi++)
            {
                for (int ti = 0; ti < tDiv; ti++)
                {
                    int x0 = ti;
                    int x1 = (ti + 1);
                    int y0 = yi * (tDiv + 1);
                    int y1 = (yi + 1) * (tDiv + 1);

                    mesh.TriangleIndices.Add(x0 + y0);
                    mesh.TriangleIndices.Add(x0 + y1);
                    mesh.TriangleIndices.Add(x1 + y0);

                    mesh.TriangleIndices.Add(x1 + y0);
                    mesh.TriangleIndices.Add(x0 + y1);
                    mesh.TriangleIndices.Add(x1 + y1);
                }
            }

            mesh.Freeze();
            return mesh;
        }
    }
}
