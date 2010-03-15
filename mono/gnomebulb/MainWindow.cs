using System;
using Gtk;
using Tao.OpenGl;
using System.Timers;
using NES.CPU.nitenedo;
using NES.CPU.Machine.BeepsBoops;
using WPFamicom;
using WPFamicom.Sound;
using testproject;
using Microsoft.Practices.Unity;
using TestGtkInstigation;
using Fishbulb.Common.UI;
using System.Collections.Generic;
using GtkNes;
using System.Runtime.InteropServices;
using NES.Sound;

public partial class MainWindow: Gtk.Window
{	
	
	NESMachine machine;
	SoundThreader sndThread;
    IUnityContainer container;

	public MainWindow (IUnityContainer container): base (Gtk.WindowType.Toplevel)
	{
        this.container = container;

 		machine = container.Resolve<NESMachine>();       
		machine.PPU.LoadPalRGBA();
        Build ();
		
        List<IViewModel> viewModels = new List<IViewModel>();
		
		viewModels.Add(container.Resolve<IViewModel>("ControlPanel") );
		
		container.RegisterType<Widget, FrontPanel>("FrontPanel");
		
        GTKInstigator instigator = new GTKInstigator(container);
        
		
//        viewModels.Add(new SoundViewModel(machine, sndThread.WavePlayer));
//        viewModels.Add(new CheatPanelVM(machine));
//		viewModels.Add(new CPUStatusVM(machine));
//		viewModels.Add(new DebuggerVM(machine));
//        viewModels.Add(new FutureInstructions(machine));

		
        instigator.Bootstrap(this.notebook1, viewModels);

		this.KeyPressEvent += HandleKeyPressEvent;
		this.KeyReleaseEvent += HandleKeyReleaseEvent;
		
		

		
		for(int i = 0; i < 2; ++i)
		{
			vidBuffers[i] = new int[256*256];
		}
		SwapNesBuffer();
		vidBuffer = vidBuffers[curBuffer];
		
		glwidget2.ExposeEvent += Render;
		
		glwidget2.SizeAllocated += HandleSizeAllocated;
		// sdlVideo = new Video(machine);
		
		machine.Drawscreen += HandleDrawscreen;	

		
		

	}


	int[] vidBuffer;
	int[][] vidBuffers = new int[2][];
	int curBuffer=0;
	//  int[] vidBuffer = new int[256*256];

	void SwapNesBuffer()
	{
		curBuffer++;
		curBuffer %=2;
		machine.PPU.SetVideoBuffer(vidBuffers[curBuffer]);

	}
	void HandleSizeAllocated(object o, SizeAllocatedArgs args)
	{
		if (inititalized) GLResize();	
	}


	void HandleKeyReleaseEvent(object o, KeyReleaseEventArgs args)
	{
		
		switch (args.Event.Key)
		{
		case Gdk.Key.Up:
		case Gdk.Key.i:
			padOneState = padOneState & ~16;
			break;
		case Gdk.Key.Down:
		case Gdk.Key.k:
			padOneState = padOneState & ~32;
			break;
		case Gdk.Key.Left:
		case Gdk.Key.j:
			padOneState = padOneState & ~64;
			break;
		case Gdk.Key.Right:
		case Gdk.Key.l:
			padOneState = padOneState & ~128;
			break;
		case Gdk.Key.z:
			padOneState = padOneState & ~2;
			break;
		case Gdk.Key.x:
			padOneState = padOneState & ~1;
			break;
		case Gdk.Key.w:
			padOneState = padOneState & ~4;
			break;
		case Gdk.Key.q:
			padOneState = padOneState & ~8;
			break;
			
		}
		args.RetVal = true;
	}

	
	int padOneState = 0;
	volatile bool isFullscreen = false;
	void HandleKeyPressEvent(object o, KeyPressEventArgs args)
	{
		
		
		switch (args.Event.Key)
		{
		case Gdk.Key.F10:
			if (isFullscreen)
			{
				this.Unfullscreen();
				isFullscreen = false;
			} else 
			{
				this.Fullscreen();
				isFullscreen = true;
			}
			break;
		case Gdk.Key.Up:
		case Gdk.Key.i:
			padOneState = padOneState | 16;
			padOneState = padOneState & ~32;
			break;
		case Gdk.Key.Down:
		case Gdk.Key.k:
			padOneState = padOneState | 32;
			padOneState = padOneState & ~16;
			break;
		case Gdk.Key.Left:
		case Gdk.Key.j:
			padOneState = padOneState | 64;
			padOneState = padOneState & ~128;
			break;
		case Gdk.Key.Right:
		case Gdk.Key.l:
			padOneState = padOneState | 128;
			padOneState = padOneState & ~64;
			break;
		case Gdk.Key.z:
			padOneState |=2;
			break;
		case Gdk.Key.x:
			padOneState |=1;
			break;
		case Gdk.Key.w:
			padOneState |=4;
			break;
		case Gdk.Key.q:
			padOneState |=8;
			break;
			
		}
		args.RetVal =true;
	}

	void HandleDrawscreen(object sender, EventArgs e)
	{

		
		// machine.PadOne.SetNextControlByte(padOneState);
		vidBuffer = vidBuffers[curBuffer];
		SwapNesBuffer();
 		
		Gtk.Application.Invoke(delegate {RefreshGLWidgets(); });
	}


	void RefreshGLWidgets()
	{
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureHandle[0]);
        Gl.glTexSubImage2D(Gl.GL_TEXTURE_2D, 0, 0, 0, 256, 256, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, vidBuffer);

		glwidget2.QueueDraw();
		//sdlVideo.BlitScreen();
        
	}
	
	byte[] pixels = new byte[256 * 256 * 4];
	int[] textureHandle = new int[2];
	
    public void SetupDisplay()
    {

		for (int i = 0; i < pixels.Length; ++i)
		{
			pixels[i]=255;
		}
		
        
        Gl.glClearColor(0, 0, 0, 0.5f);
        Gl.glClearDepth(1.0f);
        //Gl.glShadeModel(Gl.GL_SMOOTH);
        Gl.glEnable(Gl.GL_TEXTURE_2D);
        //Gl.glEnable(Gl.GL_TEXTURE_1D);
        //Gl.glEnable(Gl.GL_DEPTH_TEST);
        //Gl.glDepthFunc(Gl.GL_LEQUAL);
        
        // Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);

        //pixels = new byte[256 * 256 * 4];
        
        // Create a texture for pass one
        Gl.glGenTextures(2, textureHandle);

		Console.WriteLine(string.Format("TexID {0}", textureHandle[0]));
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureHandle[0]);
        
		Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
        Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, 3, 256, 256, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);

    }

	void GLResize()
	{
		    int width;
		    int height;
			glwidget2.GdkWindow.GetSize(out width, out height);
	
			Gl.glViewport(0, 0, width, height);
			Gl.glClearColor(0.0f,0.0f,0.0f,0);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Gl.glOrtho(-1, 1, -1, 1, -1, 1);
			
			Gl.glMatrixMode(Gl.GL_MODELVIEW);

	}
	
	bool inititalized =false;
	unsafe void  Render(object sender, EventArgs e)
	{
		if (!inititalized)
		{
				inititalized= true;
		SetupDisplay();
		GLResize();
		}

        
		Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

		Gl.glLoadIdentity();
		
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureHandle[0]);
        //Gl.glTexSubImage2D(Gl.GL_TEXTURE_2D, 0, 0, 0, 256, 256, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, vidBuffer);
		DrawBillboard();
		Gl.glFlush();
	}
	

    private static void DrawBillboard()
    {
        Gl.glBegin(Gl.GL_QUADS);

		Gl.glTexCoord2f(1.0f, 0.0f);			// bottom right of texture
        Gl.glVertex2f(1.0f, 1.0f);		// top right of quad
		
        Gl.glTexCoord2f(0.0f, 0.0f);			// bottom left of texture
        Gl.glVertex2f(-1.0f, 1.0f);		// top left of quad
		
        Gl.glTexCoord2f(0.0f, 1.0f);			// top left of texture
        Gl.glVertex2f(-1.0f, -1.0f);	// bottom left of quad
		
        Gl.glTexCoord2f(1.0f, 1.0f);			// top right of texture
        Gl.glVertex2f(1.0f, -1.0f);		// bottom right of quad

        Gl.glEnd();
    }
	
    private static void DrawBillboard2()
    {
        Gl.glBegin(Gl.GL_QUADS);
        // Front Face
        Gl.glTexCoord2f(1.0f, 1.0f);			// top right of texture
        Gl.glVertex2f(1.0f, 1.0f);		// top right of quad
        
        Gl.glTexCoord2f(0.0f, 1.0f);			// top left of texture
        Gl.glVertex2f(-1.0f, 1.0f);		// top left of quad

        Gl.glTexCoord2f(0.0f, 0.0f);			// bottom left of texture
        Gl.glVertex2f(-1.0f, -1.0f);	// bottom left of quad

        Gl.glTexCoord2f(1.0f, 0.0f);			// bottom right of texture
        Gl.glVertex2f(1.0f, -1.0f);		// bottom right of quad

        Gl.glEnd();
    }

	
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		container.Dispose();

	}	


	
}
