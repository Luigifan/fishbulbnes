using System;
using Tao.Sdl;
using NES.CPU.nitenedo;
using System.Runtime.InteropServices;

namespace GtkNes.SDL
{

	unsafe class Video
	{
		IntPtr sdlBuffer;	 //to be displayed buffer
		IntPtr surfacePtr;
		
		int[] myBuffer = new int[256*256];
		
		int	bpp = 32;
		int	width = 256;
		int	height = 256;
		int buffSize = 256 * 240;
		int resultFlip;
		
		public unsafe void FillBuffer(int pixelColor)
		{
			fixed (int* pbufStart = &myBuffer[0])
			{
				int* pbuf = pbufStart;
				int* pbufEnd = pbuf + buffSize;
				
				while (pbuf++ < pbufEnd)
				{
					*(pbuf) = pixelColor;
				}
			}
		}
		
		public void BlitScreen()
		{
		
//			fixed (int* pbufStart = machine.PPU.VideoBuffer)
//			{
//			int *p = (int * ) sdlBuffer;
//				int *pbuf = pbufStart;
//				int *pbufEnd = pbuf + (256 * 240);
//				
//				while (pbuf < pbufEnd)
//				{
//					*(p++) = *(pbuf++);
//				}
//			}
			resultFlip = Sdl.SDL_Flip(surfacePtr);
			
			// Marshal.AllocHGlobal
			//FillBuffer(j % 255);
		
		}
		
		NESMachine machine;
		
		public Video(NESMachine machine)
		{
			this.machine = machine;
			
			//Initialize the SDL frontend
			int init, flags, j;
			
			init = Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO);
			flags = (Sdl.SDL_HWSURFACE
			         |Sdl.SDL_DOUBLEBUF|Sdl.SDL_ANYFORMAT);
			surfacePtr = Sdl.SDL_SetVideoMode(
				width, 
				height, 
				bpp, 
				flags);
					
			Sdl.SDL_Surface surface = 
				(Sdl.SDL_Surface)Marshal.PtrToStructure(surfacePtr, typeof(Sdl.SDL_Surface));
	
			sdlBuffer = surface.pixels;
			
			machine.PPU.SetVideoBuffer(sdlBuffer);
			//END Initialize the SDL frontend
			
			//Initialize the offscreen buffer
	//		pixelBuffer = new int [320*240];
			// FillBuffer(255);
			//END Initialize the offscreen buffer
		}
		
		~Video()
		{
	 		Sdl.SDL_FreeSurface(surfacePtr);
		
		}
	}
}