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

namespace SlimDXBindings.Viewer10 
{
     public class D3D10Host 
    {
         int fullScreenWidth = 1280;
         int fullScreenHeight = 1024;

         NESMachine nes;

         public D3D10Host(NESMachine nes)
         {
             this.nes = nes;
         }

         TextureBuddy textureBuddy;

         Texture2D nesPalTexture;

         Texture2D texture;
         Texture2D targetTexture;
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
         FilterChain spriteFilters;

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

        public  void QuadUp()
        {


            RenderForm = new Form();
            RenderForm.ClientSize = new Size(800, 600);
            RenderForm.Text = "InstiBulb - DirectX 10";

            RenderForm.KeyDown += new KeyEventHandler(RenderForm_KeyDown);

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


            texture = new Texture2D(Device, desc);

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
            Texture2D noise = textureBuddy.CreateNoiseMap(128);
            tileFilters = new FilterChain();
            tileFilters.Add( 
                new BasicPostProcessingFilter(Device, "tiles", 256, 256, "RenderNesTiles", "Render")
                .DontFeedNextStage()
                .SetStaticResource("nesPal", nesPalTexture)
                );



            tileFilters.Add(
                new BasicPostProcessingFilter(Device, "sprites", 256, 256, "RenderNesSprites", "Render")
                .SetStaticResource("nesPal", nesPalTexture)
                .DontFeedNextStage());

            tileFilters.Add(
                new BasicPostProcessingFilter(Device, "spriteMask", 256, 256, "RenderNesSprites", "SpriteMask")
                .SetStaticResource("nesPal", nesPalTexture)
                .DontFeedNextStage());

            tileFilters.Add(
                new BasicPostProcessingFilter(Device, "tileMask", 256, 256, "RenderNesSprites", "TileMask")
                .SetStaticResource("nesPal", nesPalTexture)
                .DontFeedNextStage());

            tileFilters.Add(
                new BasicPostProcessingFilter(Device, "wavyTiles", 1024, 512, "blue", "Wave")
                .ClearNeededResources()
                .AddNeededResource("tiles", "texture2d")
                .BindScalar("timer")
                .SetStaticResource("noiseTex", noise)
                .DontFeedNextStage()
                );

            tileFilters.Add(
                new BasicPostProcessingFilter(Device, "wavyTileMask", 1024, 512, "blue", "Wave")
                .ClearNeededResources()
                .AddNeededResource("tileMask", "texture2d")
                .BindScalar("timer")
                .SetStaticResource("noiseTex", noise)
                .DontFeedNextStage()
                );



            tileFilters.Add(
                    new BasicPostProcessingFilter(Device, "blurredSprites", 1024, 1024, "simpleBlur", "Render")
                        .ClearNeededResources()
                        .AddNeededResource("sprites", "texture2d").DontFeedNextStage()
                        );

            tileFilters.Add(
                new BasicPostProcessingFilter(Device, "combined", RenderForm.ClientRectangle.Width, RenderForm.ClientRectangle.Height, "CombineNesOutput", "Render")
                .ClearNeededResources()
                .AddNeededResource("wavyTiles", "screenOne")
                .AddNeededResource("blurredSprites", "screenTwo")
                .AddNeededResource("spriteMask", "spriteMask")
                .AddNeededResource("wavyTileMask", "tileMask")
                .SetStaticResource("nesTexture", texture)
                .SetStaticResource("backgroundPic",  textureBuddy.LoadFile(@"C:\Users\strat\Pictures\water.jpg"))
                );
            //tileFilters.Add( 
            //        new BasicPostProcessingFilter(Device, "finalBlur", RenderForm.ClientRectangle.Width, RenderForm.ClientRectangle.Height, "simpleBlur", "Render")
            //        .ClearNeededResources()
            //        .AddNeededResource("combined", "texture2d")
            //        );

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

                modeDescription.Format = DXGI.Format.R8G8B8A8_UNorm;
                modeDescription.RefreshRate = new Rational(75, 1);
                modeDescription.Scaling = DXGI.DisplayModeScaling.Unspecified;
                modeDescription.ScanlineOrdering = DXGI.DisplayModeScanlineOrdering.Progressive;
                modeDescription.Width = 1280;
                modeDescription.Height = 1024;
                SwapChain.ResizeTarget(modeDescription);
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
        }

        internal  bool IsRunning
        {
            get {
                return context != null;
            }
        }

        void context_ThreadExit(object sender, EventArgs e)
        {
        }

        float timer = 0.0f;
        ShaderResourceView texView;

        public void UpdateTextures()
        {
            DataRectangle d = texture.Map(0, MapMode.WriteDiscard, MapFlags.None);
            d.Data.WriteRange<uint>(this.nes.PPU.OutBuffer);
            texture.Unmap(0);

            DataRectangle palD = nesPalTexture.Map(0, MapMode.WriteDiscard, MapFlags.None);
            for (int i = 0; i < nes.PPU.CurrentPalette + 1; ++i)
                palD.Data.WriteRange<byte>(this.nes.PPU.PalCache[i]);

            nesPalTexture.Unmap(0);
        }

        public  void DrawFrame()
        {

            tileFilters.SetVariable("timer", timer);
            timer += 0.1f;
            tileFilters.Draw(texture);


            // render output of filter
            Device.Rasterizer.SetViewports(ViewArea);
            Device.OutputMerger.SetTargets(RenderTarget);

            Device.ClearRenderTargetView(RenderTarget, Color.Black);

            EffectResourceVariable shaderTexture2 = Effect.GetVariableByName("texture2d").AsResource();

            if (texView == null) texView = new ShaderResourceView(Device, tileFilters.Result);
            
                shaderTexture2.SetResource(texView);

                quad.SetupDraw();
                for (int pass = 0; pass < Technique.Description.PassCount; ++pass)
                {
                    Pass.Apply();
                    quad.Draw();
                }
            
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