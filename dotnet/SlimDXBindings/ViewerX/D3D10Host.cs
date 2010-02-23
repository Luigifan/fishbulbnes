using System;
using System.Linq;
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
using InstibulbWpfUI;
using System.ComponentModel;
using Microsoft.WindowsAPICodePack.DirectX.Controls;

namespace SlimDXBindings.Viewer10 
{
    public class D3D10Host : IDisposable
    {
NESMachine nes;
         
         FakeEventMapper mapper;
         Timer tickTimer; 

         private SlimDXZapper zapper;
         public D3D10Host(NESMachine nes)
         {
             this.nes = nes;
             tickTimer = new Timer();

             zapper = this.nes.PadTwo as SlimDXZapper;
         }


         public void DrawScreen()
         {
             if (!idling)
             {
                 this.UpdateTextures();
                 this.DrawFrame();
             }
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

         DirectHost _renderHost;

         public DirectHost RenderHost
         {
             get { return _renderHost; }
         }

         Viewport ViewArea;
         DXGI.SwapChain SwapChain;
         D3D10.Device Device;
         D3D10.RenderTargetView RenderTarget;
         ShaderResourceView textureView;

         FilterChain tileFilters;
         const int vertexSize = 40;
         const int vertexCount = 4;

         Vector4 backgroundColor = new Vector4(0.1f, 0.5f, 1.5f, 1.0f);

         List<IDisposable> disposables = new List<IDisposable>();

         public IntPtr WindowHandler
         {
             get { 
                if (_renderHost != null ) return _renderHost.Handle;
                return IntPtr.Zero;
             }
         }


        D3D10.Texture2D resource;

        DXGI.SwapChainDescription swapChainDescription = new SlimDX.DXGI.SwapChainDescription();
        DXGI.ModeDescription modeDescription = new DXGI.ModeDescription();
        DXGI.SampleDescription sampleDescription = new DXGI.SampleDescription();

        public void QuadUp(DirectHost renderHost)
        {
            _renderHost= renderHost;

            //RenderForm = new Form();
            //RenderForm.ClientSize = new Size(720, 480);
            //RenderForm.Text = "10NES";
            
            //RenderForm.KeyDown += new KeyEventHandler(RenderForm_KeyDown);
            //RenderForm.MouseDown += new MouseEventHandler(RenderForm_MouseDown);
            //RenderForm.MouseUp += new MouseEventHandler(RenderForm_MouseUp);
            //RenderForm.MouseMove += new MouseEventHandler(RenderForm_MouseMove);
            //RenderForm.FormClosed += new FormClosedEventHandler(RenderForm_FormClosed);
            //RenderForm.ResizeEnd += new EventHandler(RenderForm_ResizeEnd);

            //RenderForm.Mouse
            modeDescription.Format = DXGI.Format.R8G8B8A8_UNorm;
            modeDescription.RefreshRate = new Rational(60, 1);
            modeDescription.Scaling = DXGI.DisplayModeScaling.Unspecified;
            modeDescription.ScanlineOrdering = DXGI.DisplayModeScanlineOrdering.Unspecified;
            modeDescription.Width = (int)_renderHost.ActualWidth;
            modeDescription.Height = (int)_renderHost.ActualHeight;

            sampleDescription.Count = 1;
            sampleDescription.Quality = 0;

            swapChainDescription.ModeDescription = modeDescription;
            swapChainDescription.SampleDescription = sampleDescription;
            swapChainDescription.BufferCount = 1;
            swapChainDescription.Flags = DXGI.SwapChainFlags.None;
            swapChainDescription.IsWindowed = true;
            swapChainDescription.OutputHandle = _renderHost.Handle;
            swapChainDescription.SwapEffect = DXGI.SwapEffect.Discard;
            swapChainDescription.Usage = DXGI.Usage.RenderTargetOutput;

            D3D10.Device.CreateWithSwapChain(null, D3D10.DriverType.Hardware, D3D10.DeviceCreationFlags.Debug, swapChainDescription, out Device, out SwapChain);

            textureBuddy = new TextureBuddy(Device);
            
            resource = Texture2D.FromSwapChain<D3D10.Texture2D>(SwapChain,0);
            RenderTarget = new D3D10.RenderTargetView(Device, resource);

            ViewArea = new Viewport();
            ViewArea.X = 0;
            ViewArea.Y = 0;
            ViewArea.Width = (int)_renderHost.ActualWidth;
            ViewArea.Height = (int)_renderHost.ActualHeight;
            ViewArea.MinZ = 0.0f;
            ViewArea.MaxZ = 1.0f;

            Device.Rasterizer.SetViewports(ViewArea);
            Device.OutputMerger.SetTargets(RenderTarget);

            

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

            Texture2D noise = textureBuddy.CreateNoiseMap2D(128);

            FilterChainLoader loader = null;

            //mapper = new FakeEventMapper(RenderForm);

            //mapper.AllowEvents = true;
            
            // the rendering chain is built on the UI thread, since it's hosting Visuals, and possibly other things that require a STA thread
            
            loader = new FilterChainLoader(Device, Container);
            tileFilters = (FilterChain)loader.ReadResource(@"SlimDXBindings.ViewerX.Filter.BasicFilterChain.xml", mapper);
            
            
            tileFilters[tileFilters.Count - 1].RenderToTexture(resource) ;
            tileFilters[tileFilters.Count - 1].RenderTarget = RenderTarget;

            //tileFilters.NotifyScreenSize(720, (480 * 240) / 256);

            if (zapper != null)
            {
                var filter = (from f in tileFilters where f.GetType() == typeof(MouseTestingFilter) select f).FirstOrDefault();
                if (filter != null)
                    (filter as MouseTestingFilter).Zapper = zapper;
            }


            disposables.Add(resource);
            disposables.Add(textureView);
            disposables.Add(texture);
            disposables.Add(tileFilters);
            disposables.Add(nesPalTexture);
            disposables.Add(targetTexture);
            disposables.Add(textureBuddy);
            disposables.Add(RenderTarget);
            

            // TODO: make this array match the elements in the new filterchains Input collection
            texArrayForRender[0] = texture;
            texArrayForRender[1] = nesPalTexture;
            texArrayForRender[2] = chrRomTex;
            texArrayForRender[3] = nesOut2;
            texArrayForRender[4] = bankSwitchCache;
            
            // context.ThreadExit += new EventHandler(context_ThreadExit);
            tickTimer = new Timer();
            tickTimer.Interval = 16;
            tickTimer.Tick += new EventHandler(tickTimer_Tick);
            tickTimer.Start();


            //Application.Run(context);
        }


        public void RequestResize(int height, int width)
        {
            newWidth = width;
            newHeight = height;
            needResizing = true;
        }

        void RenderForm_ResizeEnd(object sender, EventArgs e)
        {

            if (tileFilters != null)
            {
                int height = (int)(_renderHost.ActualHeight * (240.0 / 256.0));
                tileFilters.NotifyScreenSize((int)_renderHost.ActualWidth / 2, height / 2);
            }
        }

        void tickTimer_Tick(object sender, EventArgs e)
        {
            if (!idling)
            {
                tickTimer.Stop();
                return;
            }

            DrawFrame();
        }

        void RenderForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            tickTimer.Stop();
        }




        //TODO: move all the mouse stuff into the zapper (or other appropriate handler, like the event faker)
        void RenderForm_MouseMove(object sender, MouseEventArgs e)
        {

            if (zapper != null)
            {
                var X = (((double)e.X) / (double)(_renderHost.ActualWidth)) * 255;
                var Y = (((double)e.Y) / ((double)_renderHost.ActualHeight)) * 255;

                if (Y > 239)
                {
                    zapper.SetPixel(-1);
                }
                else
                {
                    zapper.SetPixel((int)(X + (256 * Y)));
                }
            }
        }

        void RenderForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (zapper != null)
                zapper.TriggerUp();
        }

        void RenderForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (zapper != null)
            {
                zapper.TriggerDown();
            }
        }

        int oldHeight;
        int oldWidth;
        
        void RenderForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                oldWidth = (int) _renderHost.ActualWidth;
                oldHeight = (int) _renderHost.ActualHeight;


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
            else if (e.KeyCode == Keys.F4 )
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
            //if (tileFilters.DumpFiles)
            //{
            //    //for (int i = 0; i < 64; ++i)
            //    //{
            //    //    Console.WriteLine("{0}, ", spriteRam[i]);
            //    //}

            //    Console.WriteLine("SpriteLines");
            //    for (int i = 0; i < 512; ++i)
            //    {
            //        Console.WriteLine("{0}, ", nes.PPU.SpritesOnLine[i]);
            //    }
            //}

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
                // Console.WriteLine(string.Format("Biggest bs {0}", biggestBSCount));
            }

            updated = true;
            

        }


        int biggestBSCount = 0;
        Texture2D[] texArrayForRender = new Texture2D[5];
        Vector2 mousePos = new Vector2(0,0);

        bool needResizing = false;
        bool updated = false;
        bool fullScreen = false;
        bool changeFullScreenState = false;
        int newWidth, newHeight;
        public  void DrawFrame()
        {

            if (needResizing)
            {
                needResizing = false;

                modeDescription.Format = DXGI.Format.R8G8B8A8_UNorm;
                modeDescription.RefreshRate = new Rational(60, 1);
                modeDescription.Scaling = DXGI.DisplayModeScaling.Unspecified;
                modeDescription.ScanlineOrdering = DXGI.DisplayModeScanlineOrdering.Unspecified;
                modeDescription.Width = newWidth;
                modeDescription.Height = newHeight;
                
                if (changeFullScreenState)
                {
                    SwapChain.SetFullScreenState(fullScreen, null);
                }

                SwapChain.ResizeTarget(modeDescription);
            }

            if (updated)
            {

                //tileFilters.SetVariable("mousePosition", mapper.MousePosition);
                tileFilters.SetVariable("timer", timer);
                tileFilters.SetVariable("hue", hue);
                tileFilters.SetVariable("contrast", Contrast);
                tileFilters.SetVariable("brightness", Brightness);
                
                tileFilters.SetVariable("chrramstart", nes.Cart.ChrRamStart);

                timer += 0.1f;

                Device.Rasterizer.SetViewports(ViewArea);
                Device.OutputMerger.SetTargets(RenderTarget);
                Device.ClearRenderTargetView(RenderTarget, Color.Black);

                tileFilters[tileFilters.Count - 1].SetViewport(ViewArea);

                tileFilters.Draw(texArrayForRender);

                SwapChain.Present(0, DXGI.PresentFlags.None);
                updated = false;
            }
        }

        private delegate void NoArgDelegate();

        public  void Die()
        {
            foreach (IDisposable p in disposables)
            {
                if (p != null) p.Dispose();
            }

            if (texView != null) texView.Dispose();

            Device.ClearState();
            RenderTarget.Dispose();
            if (Device != null)  Device.Dispose();
            if (SwapChain != null) SwapChain.Dispose();

            tileFilters.Dispose();
            
        }

        bool idling = false;

        void Application_Idle(object sender, EventArgs e)
        {
        }

        public void Dispose()
        {

            Die();
            
        }
    }
}