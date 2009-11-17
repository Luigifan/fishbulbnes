using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Windows;
using D3D10 = SlimDX.Direct3D10;
using DXGI = SlimDX.DXGI;
using SlimDX.Direct3D10;
using NES.CPU.nitenedo;
using NES.CPU.PPUClasses;
using SlimDXBindings.Viewer10.Filter;
using System.Collections.Generic;
using SlimDXBindings.Viewer10.Helpers;
using Microsoft.Practices.Unity;

namespace SlimDXBindings.Viewer10 
{
     public class D3D10Host 
    {
         int fullScreenWidth = 1280;
         int fullScreenHeight = 1024;

         NESMachine nes;
         FakeEventMapper mapper;
         public D3D10Host(NESMachine nes)
         {
             this.nes = nes;
         }

         public System.Windows.Threading.Dispatcher Dispatcher
         {
             get;
             set;
         }

         public IUnityContainer Container
         {
             get;
             set;
         }

         TextureBuddy textureBuddy;

         Texture2D nesPalTexture;
         Texture2D chrRomTex;
         Texture2D nesOut2;
         Texture2D texture;
         Texture2D targetTexture;
         Texture2D bankSwitchCache;

         ApplicationContext context;
         Form RenderForm;
         Viewport ViewArea;
         DXGI.SwapChain SwapChain;
         D3D10.Device Device;
         D3D10.Effect Effect;
         D3D10.EffectTechnique Technique;
         D3D10.EffectPass Pass;
         D3D10.InputLayout Layout;
         D3D10.Buffer Vertices;
         D3D10.RenderTargetView RenderTarget;
         
         ShaderResourceView textureView;


         FullscreenQuad quad;
         FilterChain tileFilters;
        const int vertexSize = 40;
        const int vertexCount = 4;

         Vector4 backgroundColor = new Vector4(0.1f, 0.5f, 1.5f, 1.0f);

         List<IDisposable> disposables = new List<IDisposable>();

         public IntPtr WindowHandler
         {
             get { 
                if (RenderForm != null && !RenderForm.IsDisposed) return RenderForm.Handle;
                return IntPtr.Zero;
             }
         }


        D3D10.Texture2D resource;

        DXGI.SwapChainDescription swapChainDescription = new SlimDX.DXGI.SwapChainDescription();
        DXGI.ModeDescription modeDescription = new DXGI.ModeDescription();
        DXGI.SampleDescription sampleDescription = new DXGI.SampleDescription();

        Vector3[] YIQtoRGBMatrix = new Vector3[3];

        static float[] default_decoder = new float[6] { 0.956f, 0.621f, -0.272f, -0.647f, -1.105f, 1.702f };

        void makeYiqToRgbMatrix()
        {   
            int burst_count = 3;
            int std_decoder_hue = -15;
            int ext_decoder_hue = std_decoder_hue + 15;

		    float hue = (float)(1.0 * Math.PI + Math.PI / 180.0 * ext_decoder_hue);
		    float sat = 1;

            float[] outBuf = new float[burst_count * 6];
			float s = (float) Math.Sin( hue ) * sat;
			float c = (float) Math.Cos( hue ) * sat;
			//float* out = impl->to_rgb;
            int outPos = 0;
			
			for (int n1 = 0; n1 < burst_count; ++n1)
			{
                int inBuf =0;
				for (int n = 0; n < 3; ++n)
				{
					float i = default_decoder[inBuf++];
					float q = default_decoder[inBuf++];
					outBuf[outPos++] = i * c - q * s;
					outBuf[outPos++] = i * s + q * c;
				}

				if ( burst_count <= 1 )
					break;
				ROTATE_IQ( ref s, ref c, 0.866025f, -0.5f ); /* +120 degrees */
			}

        }

        void ROTATE_IQ( ref float i, ref float q, float sin_b, float cos_b ) {
	        float t;
	        t = i * cos_b - q * sin_b;
	        q = i * sin_b + q * cos_b;
	        i = t;
        }

        public  void QuadUp()
        {
            makeYiqToRgbMatrix();

            RenderForm = new Form();
            RenderForm.ClientSize = new Size(800, 600);
            RenderForm.Text = "InstiBulb - DirectX 10";

            RenderForm.KeyDown += new KeyEventHandler(RenderForm_KeyDown);
            //RenderForm.Mouse
            modeDescription.Format = DXGI.Format.R8G8B8A8_UNorm;
            modeDescription.RefreshRate = new Rational(60, 1);
            modeDescription.Scaling = DXGI.DisplayModeScaling.Unspecified;
            modeDescription.ScanlineOrdering = DXGI.DisplayModeScanlineOrdering.Unspecified;
            modeDescription.Width = RenderForm.ClientRectangle.Width;
            modeDescription.Height = RenderForm.ClientRectangle.Height;

            sampleDescription.Count = 1;
            sampleDescription.Quality = 0;

            swapChainDescription.ModeDescription = modeDescription;
            swapChainDescription.SampleDescription = sampleDescription;
            swapChainDescription.BufferCount = 1;
            swapChainDescription.Flags = DXGI.SwapChainFlags.None;
            swapChainDescription.IsWindowed = true;
            swapChainDescription.OutputHandle = RenderForm.Handle;
            swapChainDescription.SwapEffect = DXGI.SwapEffect.Discard;
            swapChainDescription.Usage = DXGI.Usage.RenderTargetOutput;

            D3D10.Device.CreateWithSwapChain(null, D3D10.DriverType.Hardware, D3D10.DeviceCreationFlags.Debug, swapChainDescription, out Device, out SwapChain);

            textureBuddy = new TextureBuddy(Device);

            resource = SwapChain.GetBuffer<D3D10.Texture2D>(0);
            RenderTarget = new D3D10.RenderTargetView(Device, resource);

            ViewArea = new Viewport();
            ViewArea.X = 0;
            ViewArea.Y = 0;
            ViewArea.Width = RenderForm.ClientRectangle.Width;
            ViewArea.Height = RenderForm.ClientRectangle.Height;
            ViewArea.MinZ = 0.0f;
            ViewArea.MaxZ = 1.0f;

            Device.Rasterizer.SetViewports(ViewArea);
            Device.OutputMerger.SetTargets(RenderTarget);

            Effect = D3D10.Effect.FromStream(Device, 
                System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SlimDXBindings.Viewer10.RenderNesOutput.fx"), 
                "fx_4_0", D3D10.ShaderFlags.None, D3D10.EffectFlags.None, null, null);

            Technique = Effect.GetTechniqueByName("Render");
            Pass = Technique.GetPassByIndex(0);

            Texture2DDescription desc = new Texture2DDescription();
            desc.Usage = ResourceUsage.Dynamic;
            desc.Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm;
            desc.ArraySize= 1;
            desc.MipLevels = 1;
            desc.Width = 256;
            desc.Height = 256;
            desc.BindFlags = BindFlags.ShaderResource;
            desc.CpuAccessFlags = CpuAccessFlags.Write;
            desc.SampleDescription = sampleDescription;


            texture = textureBuddy.SetupTexture2D("nesOutput", desc);

            Texture2DDescription chrRomDesc = new Texture2DDescription();
            chrRomDesc.Usage = ResourceUsage.Dynamic;
            chrRomDesc.Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm;
            chrRomDesc.ArraySize = 1;
            chrRomDesc.MipLevels = 1;
            chrRomDesc.Width = 256;
            // (8 megabytes of mega chr rom) (67 108 864 bits / 128 bits per pixel) / 8 bits per pixel
            chrRomDesc.Height = 1024;
            chrRomDesc.BindFlags = BindFlags.ShaderResource;
            chrRomDesc.CpuAccessFlags = CpuAccessFlags.Write;
            chrRomDesc.SampleDescription = sampleDescription;

            chrRomTex = textureBuddy.SetupTexture2D("chrRAM", chrRomDesc);

            nesOut2 = textureBuddy.SetupTexture2D("nesOutput2", new Texture2DDescription()
                {
                    Usage = ResourceUsage.Dynamic,
                    Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm,
                    ArraySize = 1,
                    MipLevels = 1,
                    Width = 256,
                    Height = 256,
                    BindFlags = BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    SampleDescription = sampleDescription,
                });

            bankSwitchCache = textureBuddy.SetupTexture2D("bankSwitches", new Texture2DDescription()
            {
                Usage = ResourceUsage.Dynamic,
                Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm,
                ArraySize = 1,
                MipLevels = 1,
                // 16 * 4 = 64 bytes wide (8 rgba texels)
                // 256 * 256 high 
                Width = 16 ,
                Height = 256,
                BindFlags = BindFlags.ShaderResource ,
                CpuAccessFlags = CpuAccessFlags.Write,
                SampleDescription = sampleDescription,
            });


            EffectResourceVariable shaderTexture = Effect.GetVariableByName("texture2d").AsResource();
            textureView = new ShaderResourceView(Device, texture);
            shaderTexture.SetResource(textureView);

            Texture2DDescription palDesc = new Texture2DDescription();
            palDesc.Usage = ResourceUsage.Dynamic;
            palDesc.Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm;
            palDesc.ArraySize = 1;
            palDesc.MipLevels = 1;
            palDesc.Width = 8;
            palDesc.Height = 256;
            palDesc.BindFlags = BindFlags.ShaderResource;
            palDesc.CpuAccessFlags = CpuAccessFlags.Write;
            palDesc.SampleDescription = sampleDescription;


            nesPalTexture = new Texture2D(Device, palDesc);


            Texture2DDescription targetDesc = new Texture2DDescription();
            targetDesc.Usage = ResourceUsage.Default;
            targetDesc.Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm;
            targetDesc.ArraySize = 1;
            targetDesc.MipLevels = 1;
            targetDesc.Width = 256;
            targetDesc.Height = 256;
            targetDesc.BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget;
            targetDesc.SampleDescription = sampleDescription;

            targetTexture = new Texture2D(Device, targetDesc);
            disposables.Add(targetTexture);

            quad = new FullscreenQuad(Device, Pass.Description.Signature);
            Texture2D noise = textureBuddy.CreateNoiseMap2D(128);

            FilterChainLoader loader = null;
            Dispatcher.Invoke(new NoArgDelegate(delegate { 
                loader = new FilterChainLoader(Device, Container);
            tileFilters = (FilterChain)loader.ReadResource(@"SlimDXBindings.Viewer10.Filter.BasicFilterChain.xml");
            } ));

            // TODO: make this array match the elements in the new filterchains Input collection

            
            tileFilters[tileFilters.Count - 1].RenderToTexture(resource) ;
            tileFilters[tileFilters.Count - 1].RenderTarget = RenderTarget;


            mapper = new FakeEventMapper(RenderForm, tileFilters);
            
            disposables.Add(resource);
            disposables.Add(Effect);
            disposables.Add(quad);
            disposables.Add(Layout);
            disposables.Add(textureView);
            disposables.Add(texture);
            disposables.Add(tileFilters);
            disposables.Add(nesPalTexture);
            disposables.Add(textureBuddy);
            
            context = new ApplicationContext(RenderForm);

            texArrayForRender[0] = texture;
            texArrayForRender[1] = nesPalTexture;
            texArrayForRender[2] = chrRomTex;
            texArrayForRender[3] = nesOut2;
            texArrayForRender[4] = bankSwitchCache;
            
            context.ThreadExit += new EventHandler(context_ThreadExit);
            Application.Idle += new EventHandler(Application_Idle);
            
            Application.Run( context);
        }

        int oldHeight;
        int oldWidth;
        void RenderForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                oldWidth = RenderForm.ClientRectangle.Width;
                oldHeight = RenderForm.ClientRectangle.Height;


                //modeDescription.Format = DXGI.Format.R8G8B8A8_UNorm;
                //modeDescription.RefreshRate = new Rational(0, 0);
                //SwapChain.ResizeTarget(modeDescription);
                SwapChain.SetFullScreenState(true, null);
            }
            else if (e.KeyCode == Keys.F12)
            {
                modeDescription.Format = DXGI.Format.R8G8B8A8_UNorm;
                modeDescription.RefreshRate = new Rational(60, 1);
                modeDescription.Scaling = DXGI.DisplayModeScaling.Unspecified;
                modeDescription.ScanlineOrdering = DXGI.DisplayModeScanlineOrdering.Unspecified;
                modeDescription.Width = oldWidth;
                modeDescription.Height = oldHeight;
                
                SwapChain.SetFullScreenState(false, null);
                SwapChain.ResizeTarget(modeDescription);
            }
            else if (e.KeyCode == Keys.D)
            {
                if (tileFilters != null && nes != null && nes.IsRunning)
                    tileFilters.DumpFiles = true;
            }
            else if (e.KeyCode == Keys.F4)
            {
                if (controlVisibility == 0)
                {
                    controlVisibilityOffset = 0.05f;
                }
                else if (controlVisibility == 1)
                {
                    controlVisibilityOffset = -0.05f;
                }
            }
        }
        float controlVisibility = 0;
        float controlVisibilityOffset = 0;

        internal  bool IsRunning
        {
            get {
                return context != null;
            }
        }

        void context_ThreadExit(object sender, EventArgs e)
        {
        }

        float hue = 25.0f;

        public float Hue
        {
            get { return hue; }
            set { hue = value; }
        }

        public float Brightness
        {
            get;
            set;
        }

        public float Contrast
        {
            get;
            set;
        }



        float timer = 0.0f;
        ShaderResourceView texView;

        public void UpdateTextures()
        {
            // System.Buffer.BlockCopy(nes.PPU.OutBuffer, 255 * 256 * 4, spriteRam, 0, 256);

            if (tileFilters.DumpFiles)
            {
                //for (int i = 0; i < 64; ++i)
                //{
                //    Console.WriteLine("{0}, ", spriteRam[i]);
                //}

                Console.WriteLine("SpriteLines");
                for (int i = 0; i < 512; ++i)
                {
                    Console.WriteLine("{0}, ", nes.PPU.SpritesOnLine[i]);
                }
            }

            DataRectangle d = texture.Map(0, MapMode.WriteDiscard, MapFlags.None);
            d.Data.WriteRange<int>(this.nes.PPU.OutBuffer);
            texture.Unmap(0);

            DataRectangle d2 = nesOut2.Map(0, MapMode.WriteDiscard, MapFlags.None);
            d2.Data.WriteRange<int>(this.nes.PPU.VideoBuffer);
            nesOut2.Unmap(0);
            

            DataRectangle palD = nesPalTexture.Map(0, MapMode.WriteDiscard, MapFlags.None);
            for (int i = 0; i < nes.PPU.CurrentPalette + 1; ++i)
                palD.Data.WriteRange<byte>(this.nes.PPU.PalCache[i]);
            nesPalTexture.Unmap(0);

            DataRectangle ramRect = chrRomTex.Map(0, MapMode.WriteDiscard, MapFlags.None);
            ramRect.Data.WriteRange<byte>(this.nes.Cart.ChrRom, 0, this.nes.Cart.ChrRom.Length);
            //ramRect.Data.WriteRange<byte>(new byte[1048576 - nes.Cart.ChrRom.Length]);
            chrRomTex.Unmap(0);

            DataRectangle bankRect = bankSwitchCache.Map(0, MapMode.WriteDiscard, MapFlags.None);
            bankRect.Data.Position = 0;
            for (int i = 0; i <= nes.Cart.CurrentBank; ++i )
                bankRect.Data.WriteRange<int>(nes.Cart.BankStartCache[i]);
            bankSwitchCache.Unmap(0);

            if (nes.Cart.CurrentBank > biggestBSCount)
            {
                biggestBSCount = (int)nes.Cart.CurrentBank;
                Console.WriteLine(string.Format("Biggest bs {0}", biggestBSCount));
            }



        }


        int biggestBSCount = 0;
        Texture2D[] texArrayForRender = new Texture2D[5];

        public  void DrawFrame()
        {

            controlVisibility += controlVisibilityOffset;
            if (controlVisibility >= 1.0f)
            {
                controlVisibility = 1.0f;
                controlVisibilityOffset = 0;
            } else if (controlVisibility <= 0)
            {
                controlVisibility = 0.0f;
                controlVisibilityOffset = 0.0f;
            }
            tileFilters.SetVariable("controlVisibility", controlVisibility);
            tileFilters.SetVariable("timer", timer);
            tileFilters.SetVariable("hue", hue);
            tileFilters.SetVariable("contrast", Contrast);
            tileFilters.SetVariable("brightness", Brightness);
            tileFilters.SetVariable("chrramstart", nes.Cart.ChrRamStart);
            //tileFilters.SetVariable("ppuBankStarts", nes.Cart.PPUBankStarts);
           // tileFilters.SetVariable("spriteRam", spriteRam);
            //tileFilters.SetVariable("spritesOnLine", nes.PPU.SpritesOnLine);            

            timer += 0.1f;

            //texArrayForRender[3] = bankSwitchTex;

            Device.Rasterizer.SetViewports(ViewArea);
            Device.OutputMerger.SetTargets(RenderTarget);
            Device.ClearRenderTargetView(RenderTarget, Color.Black);

            tileFilters[tileFilters.Count - 1].SetViewport(ViewArea);

            tileFilters.Draw(texArrayForRender);

            // render output of filter

            //EffectResourceVariable shaderTexture2 = Effect.GetVariableByName("texture2d").AsResource();

            //if (texView == null) texView = new ShaderResourceView(Device, tileFilters.Result);
            
            //    shaderTexture2.SetResource(texView);

            //    quad.SetupDraw();
            //    for (int pass = 0; pass < Technique.Description.PassCount; ++pass)
            //    {
            //        Pass.Apply();
            //        quad.Draw();
            //    }

            SwapChain.Present(0, DXGI.PresentFlags.None);
        }

        private delegate void NoArgDelegate();

        public  void Die()
        {
            foreach (var p in disposables)
            {
                if (p != null) p.Dispose();
            }

            if (texView != null) texView.Dispose();

            Device.ClearState();
            RenderTarget.Dispose();
            if (Layout != null) Layout.Dispose();

            if (Device != null)  Device.Dispose();
            if (SwapChain != null) SwapChain.Dispose();

            RenderForm.Invoke( new NoArgDelegate( RenderForm.Close ));
            
            
        }

        void Application_Idle(object sender, EventArgs e)
        {

        }
    }
}