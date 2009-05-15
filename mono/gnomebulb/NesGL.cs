
using System;
using Tao.OpenGl;
using System.IO;
using System.Reflection;

namespace testproject
{
	public enum RenderStyles
	{
		NESPaletted,
		RGB
	}
	
    public enum Shapes
    {
        Billboard,
        Cube,
        Sphere, 
        SphereInCube
    }

	
	public class NesGL
	{
		

        private int[] textureHandle = new int[2];
        private int nesPaletteTex;
        string[] fragShader = new string[1];
        string[] vertShader = new string[1];

        int vShaderIndex, fShaderIndex;

        Shapes renderOnto = Shapes.Billboard;

        public Shapes RenderOnto
        {
            get { return renderOnto; }
            set { renderOnto = value; }
        }

        RenderStyles engine;

        public NesGL() : this(RenderStyles.RGB)
        {
        }

        public NesGL(RenderStyles engine)
        {
            this.engine = engine;

            SetupShaders();
        }

        private void SetupShaders()
        {
			foreach (string s in Assembly.GetExecutingAssembly().GetManifestResourceNames())
			{
				Console.WriteLine(s);
			}
			
			
            using (TextReader r = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Shaders.basicVert.vert")))
            {
                vertShader[0] = r.ReadToEnd();
				Console.WriteLine("vert shader:" + vertShader[0]);
            }


            using (TextReader r = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Shaders.paletteShader.frag")))
            {
                fragShader[0] = r.ReadToEnd();
				Console.WriteLine("frag shader:" + fragShader[0]);
            }
        }

        private byte[] pixels;
        int program;

        int passOneFrameBuffer;
        int passOneRenderBuffer;
        int passOneFBTex;


        int nesCubeDisplayList;
        int fragShaderPassNumber;

        public void SetupDisplay()
        {
            //BitmapImage bitmap = new BitmapImage(new Uri(@"D:\Projects\Fastendu\GLFamicom\Artwork\MMYouGot.JPG"));

 			           
            Gl.ReloadFunctions();
            //base.AutoSwapBuffers = false;
            Gl.glClearColor(0, 0, 0, 0.5f);
            Gl.glClearDepth(1.0f);
            //Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_TEXTURE_1D);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LEQUAL);
            
            // Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);

            pixels = new byte[256 * 256 * 4];
            
            // Create a texture for pass one
            Gl.glGenTextures(2, textureHandle);

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureHandle[0]);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            switch (engine)
            {
                case RenderStyles.RGB:
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
                    break;
                case RenderStyles.NESPaletted:
                    frameBufferWidth = 256;
                    frameBufferHeight = 256;
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
                    break;
            }
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, 3, 256, 256, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, null);

            SetupPaletteTexture();

            // create and link the shader
            program = Gl.glCreateProgram();

            vShaderIndex = Gl.glCreateShader(Gl.GL_VERTEX_SHADER);
            Gl.glShaderSource(vShaderIndex, 1, vertShader, new int[1] { vertShader[0].Length });
            Gl.glCompileShader(vShaderIndex);
            Gl.glAttachShader(program, vShaderIndex);

            fShaderIndex = Gl.glCreateShader(Gl.GL_FRAGMENT_SHADER);
            Gl.glShaderSource(fShaderIndex, 1, fragShader, new int[1] { fragShader[0].Length });
            Gl.glCompileShader(fShaderIndex);
            Gl.glAttachShader(program, fShaderIndex);

            Gl.glLinkProgram(program);
            Gl.glUseProgram(program);

            // get sampler uniform locations
            //int loc = Gl.glGetUniformLocation(program, "myTexture");
            
            //// bind texture to texture unit 
            //Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0);
            //Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureHandle[0]);
            //Gl.glUniform1i(loc, 0);
            fragShaderPassNumber = Gl.glGetUniformLocation(program, "passNumber");
            Gl.glUniform1i(fragShaderPassNumber, 1);

            if (engine == RenderStyles.NESPaletted)
            {
                Gl.glUniform1i(fragShaderPassNumber, 0);
                int loc = Gl.glGetUniformLocation(program, "myPalette");
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1);
                Gl.glBindTexture(Gl.GL_TEXTURE_1D, textureHandle[1]);
                Gl.glUniform1i(loc, 1);
            }
            

            // create the framebuffer needed to store the results of the first pass
            // SetupFrameBuffer();

            nesCubeDisplayList = Gl.glGenLists(1);
            
            Gl.glNewList(nesCubeDisplayList, Gl.GL_COMPILE);
            DrawNesCube();
            Gl.glEndList();

            // prepare the sphere
            quad = Glu.gluNewQuadric();
            Glu.gluQuadricDrawStyle(quad, Glu.GLU_FILL);
            Glu.gluQuadricNormals(quad, Glu.GLU_SMOOTH);
            Glu.gluQuadricTexture(quad, Gl.GL_TRUE);

            SetupFrameBuffer();

			earth = new Planetoid() { Diameter = 1, SpinSpeed = 1.0f, Tilt = 3 };
        	moon = new Planetoid() { Diameter = 0.3f, SpinSpeed = -0.2f, Tilt = 0 };

        }

        private void SetupPaletteTexture()
        {

            // creae the texture which holds the nes's palette
            Gl.glBindTexture(Gl.GL_TEXTURE_1D, textureHandle[1]);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_DECAL);
            Gl.glTexParameteri(Gl.GL_TEXTURE_1D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
            Gl.glTexParameteri(Gl.GL_TEXTURE_1D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
            Gl.glTexParameteri(Gl.GL_TEXTURE_1D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
            Gl.glTexParameteri(Gl.GL_TEXTURE_1D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
            uint[] pal = new uint[256];
//            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("OpenGLNESViewer.Art.bnes.pal"))
//            {
//                for (int n = 0; n < 64; ++n)
//                {
//                    byte r = (byte)stream.ReadByte();
//                    byte g = (byte)stream.ReadByte();
//                    byte b = (byte)stream.ReadByte();
//                    //pal[n] = (uint)( (r << 24) | (g << 16) | (b << 8) | 0xFF);
//                    pal[n] = (uint)((0xFF << 24) | (b << 16) | (g << 8) | r);
//                    pal[n + 64] = pal[n];
//                    pal[n + 128] = pal[n];
//                    pal[n + 192] = pal[n];
//                }
//            }
            Gl.glTexImage1D(Gl.GL_TEXTURE_1D, 0, 3, 256, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pal);
        }

        public void ResizeGLWindow (int Height, int Width)
        {
            if (Height == 0) return;
            Gl.glViewport(0, 0, Width, Height);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(45.0f, Width / Height, 0.1f, 100.0f);
            //Gl.glOrtho(-2, 2, -2, 2, 0, 100);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
        }


        int frameBufferWidth = 1024;
        int frameBufferHeight = 768;

        void SetupFrameBuffer()
        {
            Gl.glGenFramebuffersEXT(1, out passOneFrameBuffer);
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, passOneFrameBuffer);

            // create a render buffer and assign it (depth)
            Gl.glGenRenderbuffersEXT(1, out passOneRenderBuffer);
            Gl.glBindRenderbufferEXT(Gl.GL_RENDERBUFFER_EXT, passOneRenderBuffer);
            Gl.glRenderbufferStorageEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_DEPTH_COMPONENT24, frameBufferWidth, frameBufferHeight);
            Gl.glFramebufferRenderbufferEXT(Gl.GL_FRAMEBUFFER_EXT,
                Gl.GL_DEPTH_ATTACHMENT_EXT,
                Gl.GL_RENDERBUFFER_EXT, passOneRenderBuffer);


            // create the texture to render to 
            Gl.glGenTextures(1, out passOneFBTex);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, passOneFBTex);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, frameBufferWidth, frameBufferHeight, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, null);
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, passOneFBTex, 0);

            /// Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            //            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

            // set the framebuffer to render to the texture
            
            if (Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT) != Gl.GL_FRAMEBUFFER_COMPLETE_EXT)
            {
				Console.WriteLine("Framebuffer " + Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT));
                // throw new NotImplementedException("Frame Buffer IS NOT A FRAME BUFFER!");
            }
            // shut off our framebuffer
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0);

        }
        int[] nesPalette = new int[256];
        public void UpdateNESScreen(int[] pixels) 
        {
            //Array.Copy(pixels, 255 * 256, nesPalette, 0, 32);

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT);
            UpdateNESTexture(pixels);

            // Gl.glCallList(nesCubeDisplayList);
            Gl.glLoadIdentity();
            Gl.glTranslatef(0f, 0, -3.5f);
            Gl.glUniform1i(fragShaderPassNumber, (int)engine);
            if (isRotating)
            {
                rotation = isRotating ? rotation - 0.25f : 0;

                Gl.glRotatef(rotation, 1.0f, 1.0f, 1.0f);
            }
            switch (renderOnto)
            {
                case Shapes.Billboard:
                    DrawNesScreen();
                    break;
                case Shapes.Cube:
                    Gl.glCallList(nesCubeDisplayList);
                    break;
                case Shapes.Sphere:
                    DrawNesSphere();
                    break;
                case Shapes.SphereInCube:
                    DrawNesSphereInNesCube();
                    break;
            }
        }

        private void UpdateNESTexture(int[] pixels)
        {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureHandle[0]);
            Gl.glTexSubImage2D(Gl.GL_TEXTURE_2D, 0, 0, 0, 256, 256, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);
        }


        public void DrawDefaultDisplay()
        {
            // Gl.glUseProgram(0);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureHandle[0]);

            //DrawNesScreen();
            DrawNesSphere();
            Gl.glFlush();
        }

        private void DrawNesSphereInNesCube()
        {
            FrameBufferOn();
            Gl.glUniform1i(fragShaderPassNumber, (int)engine);

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();

            Gl.glTranslatef(0f, 0, -3.5f);

            if (isRotating)
            {
                Gl.glRotatef(-rotation, 1.0f, 1.0f, 1.0f);
            }
            // Gl.glRotatef(rotation, 1.0f, 1.0f, 1.0f);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureHandle[0]);
            Glu.gluSphere(quad, 1, 60, 60);

            FrameBufferOff();

            Gl.glLoadIdentity();
            Gl.glTranslatef(0f, 0, -3.5f);
            if (isRotating)
            {
                Gl.glRotatef(rotation, 1.0f, 1.0f, 1.0f);
            }
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, passOneFBTex);
            Gl.glActiveTexture(Gl.GL_TEXTURE0 );
            Gl.glUniform1i(fragShaderPassNumber, 1);
            DrawNesCube();
        }


        private static void FrameBufferOff()
        {
            Gl.glPopAttrib();
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0);
        }

        private void FrameBufferOn()
        {
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, passOneFrameBuffer);
            Gl.glPushAttrib(Gl.GL_VIEWPORT_BIT);
            Gl.glViewport(0, 0, frameBufferWidth, frameBufferHeight);
        }

        private void DrawNesScreen()
        {

            if (engine == RenderStyles.NESPaletted)
            {
                FrameBufferOn();
                Gl.glUniform1i(fragShaderPassNumber, 0);
                Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
                Gl.glLoadIdentity();

                Gl.glTranslatef(0f, 0, -3.3f);

                DrawBillboard();

                FrameBufferOff();

                Gl.glUniform1i(fragShaderPassNumber, 1);

                Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
                Gl.glLoadIdentity();

                Gl.glBindTexture(Gl.GL_TEXTURE_2D, passOneFBTex);
                Gl.glActiveTexture(Gl.GL_TEXTURE0);
                
                Gl.glTranslatef(0, 0, -3.5f);
                //Gl.glRotatef(180, 0.0f, 0.0f, -1.0f);
                DrawBillboard2();

            }
            else
            {
                Gl.glLoadIdentity();

                Gl.glTranslatef(0, 0, -3.5f);
                DrawBillboard();
            }
         //   Gl.glPopMatrix();
        }

        private static void DrawBillboard()
        {
            Gl.glBegin(Gl.GL_QUADS);
            // Front Face
            Gl.glTexCoord2f(1.0f, 0.0f);			// bottom right of texture
            Gl.glVertex3f(1.0f, 1.0f, 1.0f);		// top right of quad
            Gl.glTexCoord2f(0.0f, 0.0f);			// bottom left of texture
            Gl.glVertex3f(-1.0f, 1.0f, 1.0f);		// top left of quad
            Gl.glTexCoord2f(0.0f, 1.0f);			// top left of texture
            Gl.glVertex3f(-1.0f, -1.0f, 1.0f);	// bottom left of quad
            Gl.glTexCoord2f(1.0f, 1.0f);			// top right of texture
            Gl.glVertex3f(1.0f, -1.0f, 1.0f);		// bottom right of quad

            Gl.glEnd();
        }
        private static void DrawBillboard2()
        {
            Gl.glBegin(Gl.GL_QUADS);
            // Front Face
            Gl.glTexCoord2f(1.0f, 1.0f);			// top right of texture
            Gl.glVertex3f(1.0f, 1.0f, 1.0f);		// top right of quad
            
            Gl.glTexCoord2f(0.0f, 1.0f);			// top left of texture
            Gl.glVertex3f(-1.0f, 1.0f, 1.0f);		// top left of quad

            Gl.glTexCoord2f(0.0f, 0.0f);			// bottom left of texture
            Gl.glVertex3f(-1.0f, -1.0f, 1.0f);	// bottom left of quad

            Gl.glTexCoord2f(1.0f, 0.0f);			// bottom right of texture
            Gl.glVertex3f(1.0f, -1.0f, 1.0f);		// bottom right of quad

            Gl.glEnd();
        }
        


        float rotation;
        Glu.GLUquadric quad;
		Planetoid earth, moon;
        float moonOrbitRadius = 2, moonOrbitSpeed = 0.2f, moonOrbitLocation;
        private void DrawNesSphere()
        {
			
            Gl.glLoadIdentity();
            Gl.glTranslatef(0, 0, -3.5f);
            earth.Update();
            earth.Draw();
            Gl.glRotatef(moonOrbitLocation, 0, 1, 0);
            moonOrbitLocation += moonOrbitSpeed;
            Gl.glTranslatef(0, 0, moonOrbitRadius);
            moon.Update();
            moon.Draw();
            // 
        }

        private void DrawNesCube()
        {
            Gl.glBegin(Gl.GL_QUADS);
            // Front Face
            Gl.glTexCoord2f(1.0f, 0.0f);			// bottom right of texture
            Gl.glVertex3f(1.0f, 1.0f, 1.0f);		// top right of quad
            Gl.glTexCoord2f(0.0f, 0.0f);			// bottom left of texture
            Gl.glVertex3f(-1.0f, 1.0f, 1.0f);		// top left of quad
            Gl.glTexCoord2f(0.0f, 1.0f);			// top left of texture
            Gl.glVertex3f(-1.0f, -1.0f, 1.0f);	// bottom left of quad
            Gl.glTexCoord2f(1.0f, 1.0f);			// top right of texture
            Gl.glVertex3f(1.0f, -1.0f, 1.0f);		// bottom right of quad

            // Back Face
            Gl.glTexCoord2f(1.0f, 0.0f);			// bottom right of texture
            Gl.glVertex3f(-1.0f, 1.0f, -1.0f);	// top right of quad
            Gl.glTexCoord2f(0.0f, 0.0f);			// bottom left of texture
            Gl.glVertex3f(1.0f, 1.0f, -1.0f);		// top left of quad
            Gl.glTexCoord2f(0.0f, 1.0f);			// top left of texture
            Gl.glVertex3f(1.0f, -1.0f, -1.0f);	// bottom left of quad
            Gl.glTexCoord2f(1.0f, 1.0f);			// top right of texture
            Gl.glVertex3f(-1.0f, -1.0f, -1.0f);	// bottom right of quad

            // Top Face
            Gl.glTexCoord2f(1.0f, 1.0f);			// top right of texture
            Gl.glVertex3f(1.0f, 1.0f, -1.0f);		// top right of quad
            Gl.glTexCoord2f(0.0f, 1.0f);			// top left of texture
            Gl.glVertex3f(-1.0f, 1.0f, -1.0f);	// top left of quad
            Gl.glTexCoord2f(0.0f, 0.0f);			// bottom left of texture
            Gl.glVertex3f(-1.0f, 1.0f, 1.0f);		// bottom left of quad
            Gl.glTexCoord2f(1.0f, 0.0f);			// bottom right of texture
            Gl.glVertex3f(1.0f, 1.0f, 1.0f);		// bottom right of quad

            // Bottom Face
            Gl.glTexCoord2f(1.0f, 1.0f);			// top right of texture
            Gl.glVertex3f(1.0f, -1.0f, 1.0f);		// top right of quad
            Gl.glTexCoord2f(0.0f, 1.0f);			// top left of texture
            Gl.glVertex3f(-1.0f, -1.0f, 1.0f);	// top left of quad
            Gl.glTexCoord2f(0.0f, 0.0f);			// bottom left of texture
            Gl.glVertex3f(-1.0f, -1.0f, -1.0f);	// bottom left of quad
            Gl.glTexCoord2f(1.0f, 0.0f);			// bottom right of texture
            Gl.glVertex3f(1.0f, -1.0f, -1.0f);	// bottom right of quad

            // Right Face
            Gl.glTexCoord2f(1.0f, 1.0f);			// top right of texture
            Gl.glVertex3f(1.0f, 1.0f, -1.0f);		// top right of quad
            Gl.glTexCoord2f(0.0f, 1.0f);			// top left of texture
            Gl.glVertex3f(1.0f, 1.0f, 1.0f);		// top left of quad
            Gl.glTexCoord2f(0.0f, 0.0f);			// bottom left of texture
            Gl.glVertex3f(1.0f, -1.0f, 1.0f);		// bottom left of quad
            Gl.glTexCoord2f(1.0f, 0.0f);			// bottom right of texture
            Gl.glVertex3f(1.0f, -1.0f, -1.0f);	// bottom right of quad

            // Left Face
            Gl.glTexCoord2f(1.0f, 1.0f);			// top right of texture
            Gl.glVertex3f(-1.0f, 1.0f, 1.0f);		// top right of quad
            Gl.glTexCoord2f(0.0f, 1.0f);			// top left of texture
            Gl.glVertex3f(-1.0f, 1.0f, -1.0f);	// top left of quad
            Gl.glTexCoord2f(0.0f, 0.0f);			// bottom left of texture
            Gl.glVertex3f(-1.0f, -1.0f, -1.0f);	// bottom left of quad
            Gl.glTexCoord2f(1.0f, 0.0f);			// bottom right of texture
            Gl.glVertex3f(-1.0f, -1.0f, 1.0f);	// bottom right of quad
            Gl.glEnd();

        }

        bool isRotating;

        public bool IsRotating
        {
            get { return isRotating; }
            set { isRotating = value; }
        }


    }
	
}
