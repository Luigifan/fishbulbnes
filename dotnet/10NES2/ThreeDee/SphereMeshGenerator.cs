using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows;

namespace InstiBulb.ThreeDee
{
    public class SphereMeshGenerator
    {

        // Four private initialized fields.

        int slices = 32;

        int stacks = 16;

        Point3D center = new Point3D();

        double radius = 1;

        // Four public properties allow access to private fields.

        public int Slices
        {

            set { slices = value; }

            get { return slices; }

        }

        public int Stacks
        {

            set { stacks = value; }

            get { return stacks; }

        }

        public Point3D Center
        {

            set { center = value; }

            get { return center; }

        }

        public double Radius
        {

            set { radius = value; }

            get { return radius; }

        }

        // Get-only property generates MeshGeometry3D.

        public MeshGeometry3D Geometry
        {

            get
            {

                // Create a MeshGeometry3D.

                MeshGeometry3D mesh = new MeshGeometry3D();

                // Fill the vertices, normals, and textures collections.

                for (int stack = 0; stack <= Stacks; stack++)
                {

                    double phi = Math.PI / 2 - stack * Math.PI / Stacks;

                    double y = Radius * Math.Sin(phi);

                    double scale = -Radius * Math.Cos(phi);

                    for (int slice = 0; slice <= Slices; slice++)
                    {

                        double theta = slice * 2 * Math.PI / Slices;

                        double x = scale * Math.Sin(theta);

                        double z = scale * Math.Cos(theta);

                        Vector3D normal = new Vector3D(x, y, z);

                        mesh.Normals.Add(normal);

                        mesh.Positions.Add(normal + Center);

                        mesh.TextureCoordinates.Add(

                                    new Point((double)slice / Slices,

                                              (double)stack / Stacks));

                    }

                }

                // Fill the indices collection.

                for (int stack = 0; stack < Stacks; stack++)
                {

                    int top = (stack + 0) * (Slices + 1);

                    int bot = (stack + 1) * (Slices + 1);

                    for (int slice = 0; slice < Slices; slice++)
                    {

                        if (stack != 0)
                        {

                            mesh.TriangleIndices.Add(top + slice);

                            mesh.TriangleIndices.Add(bot + slice);

                            mesh.TriangleIndices.Add(top + slice + 1);

                        }

                        if (stack != Stacks - 1)
                        {

                            mesh.TriangleIndices.Add(top + slice + 1);

                            mesh.TriangleIndices.Add(bot + slice);

                            mesh.TriangleIndices.Add(bot + slice + 1);

                        }

                    }

                }
                mesh.Freeze();

                return mesh;

            }

        }

        public void MakeBoundingSphere(List<Point3D> V)
    {
        Point3D C;                           // Center of ball
        double rad, rad2;                   // radius and radius squared
        double xmin, xmax, ymin, ymax, zmin, zmax;      // bounding box extremes
        int   Pxmin, Pxmax, Pymin, Pymax, Pzmin, Pzmax;  // index of V[] at box extreme
        int n = V.Count;
        if (n == 0) return;
        // find a large diameter to start with
        // first get the bounding box and V[] extreme points for it
        xmin = xmax = V[0].X;
        ymin = ymax = V[0].Y;
        zmin = zmax = V[0].Z;

        Pxmin = Pxmax = Pymin = Pymax = Pzmax = Pzmin = 0;
        for (int i=1; i<n; i++) {
            if (V[i].X < xmin) {
                xmin = V[i].X;
                Pxmin = i;
            }
            else if (V[i].X > xmax) {
                xmax = V[i].X;
                Pxmax = i;
            }
            if (V[i].Y < ymin) {
                ymin = V[i].Y;
                Pymin = i;
            }
            else if (V[i].Y > ymax) {
                ymax = V[i].Y;
                Pymax = i;
            }
            if (V[i].Z < zmin)
            {
                zmin = V[i].Z;
                Pzmin = i;
            }
            else if (V[i].Z > zmax)
            {
                zmax = V[i].Z;
                Pzmax = i;
            }

        }
        // select the largest extent as an initial diameter for the ball
        Vector3D dVx = Point3D.Subtract(V[Pxmax], V[Pxmin]); // diff of Vy max and min
        Vector3D dVy = Point3D.Subtract(V[Pymax], V[Pymin]); // diff of Vy max and min
        Vector3D dVz = Point3D.Subtract(V[Pzmax], V[Pzmin]); // diff of Vy max and min

        double dx2 = dVx.LengthSquared;// (dVx); // Vx diff squared
        double dy2 = dVy.LengthSquared; // Vy diff squared
        double dz2 = dVz.LengthSquared;


        if (dx2 >= dy2 && dx2 >= dz2) {                     // x direction is largest extent
            C = V[Pxmin] + (dVx / 2.0);         // Center = midpoint of extremes
            rad2 = (V[Pxmax] - C).LengthSquared;         // radius squared
        }
        else if (dy2 >= dx2 && dy2 >= dz2)
        {                                // y direction is largest extent
            C = V[Pymin] + (dVy / 2.0);         // Center = midpoint of extremes
            rad2 = (V[Pymax] - C).LengthSquared;         // radius squared
        }
        else
        {
            C = V[Pzmin] + (dVz / 2.0);         // z direction is largest
            rad2 = (V[Pzmax] - C).LengthSquared;         // radius squared
        }
        rad = Math.Sqrt(rad2);

        // now check that all points V[i] are in the ball
        // and if not, expand the ball just enough to include them
        Vector3D dV;
        double dist, dist2;
        for (int i=0; i<n; i++) {
            dV = V[i] - C;
            dist2 = dV.LengthSquared;
            if (dist2 <= rad2)    // V[i] is inside the ball already
                continue;
            // V[i] not in ball, so expand ball to include it
            dist = Math.Sqrt(dist2);
            rad = (rad + dist) / 2.0;         // enlarge radius just enough
            rad2 = rad * rad;
            C = C + ((dist-rad)/dist) * dV;   // shift Center toward V[i]
        }
        this.Center = C;
        this.Radius = rad;
        return;
    }



    }


}
