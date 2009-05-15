using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using Tao.FreeGlut;
namespace testproject
{
    public class Planetoid
    {
        private float tilt = 0;

        Glu.GLUquadric quad = new Glu.GLUquadric();
        public Planetoid()
        {
            quad = Glu.gluNewQuadric();
            Glu.gluQuadricDrawStyle(quad, Glu.GLU_FILL);
            Glu.gluQuadricNormals(quad, Glu.GLU_SMOOTH);
            Glu.gluQuadricTexture(quad, Gl.GL_TRUE);
        }

        /// <summary>all 
        /// Degrees of tilt of the axis
        /// </summary>
        public float Tilt
        {
            get { return tilt; }
            set { tilt = value; }
        }

        private float spinSpeed;

        /// <summary>
        /// Speed of CW rotation in degrees per frame, negative rotations cause a CCW spin
        /// </summary>
        public float SpinSpeed
        {
            get { return spinSpeed; }
            set { spinSpeed = value; }
        }

        private float diameter;

        public float Diameter
        {
            get { return diameter; }
            set { diameter = value; }
        }

        float rotation;

        float orbitRadius = 2, orbitSpeed = 0.2f, orbitLocation;

        public float OrbitLocation
        {
            get { return orbitLocation; }
            set { orbitLocation = value; }
        }

        public float OrbitSpeed
        {
            get { return orbitSpeed; }
            set { orbitSpeed = value; }
        }

        public float OrbitRadius
        {
            get { return orbitRadius; }
            set { orbitRadius = value; }
        }


        public void Update()
        {
            rotation += spinSpeed;
        }

        public void Draw()
        {
            Gl.glPushMatrix();
            Gl.glRotatef(rotation, 0, 1, 0);
            Gl.glRotatef(tilt, 0, 0, 1);
            Glu.gluSphere(quad, diameter / 2, 60, 60);
            Gl.glPopMatrix();
        }

        List<Planetoid> satellites = new List<Planetoid>();

        public List<Planetoid> Satellites
        {
            get { return satellites; }
            set { satellites = value; }
        }
    }
}
