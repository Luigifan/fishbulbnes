﻿using System;
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
         NESMachine nes;

         public D3D10Host(NESMachine nes)
         {
             this.nes = nes;
         }

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
         
         FullscreenQuad quad;
         FilterChain tileFilters;
         FilterChain spriteFilters;

        const int vertexSize = 40;
        const int vertexCount = 4;

         Vector4 backgroundColor = new Vector4(0.1f, 0.5f, 1.5f, 1.0f);

         List<IDisposable> disposables = new List<IDisposable>();

        private Texture2D CreateNoiseMap(int resolution)
         {
             Random rand = new Random();
             Vector4[] noisyColors = new Vector4[resolution * resolution];
             for (int x = 0; x < resolution; x++)
                 for (int y = 0; y < resolution; y++)
                     noisyColors[x + y * resolution] = new Vector4((float)rand.Next(1000) / 1000.0f, (float)rand.Next(1000) / 1000.0f, (float)rand.Next(1000) / 1000.0f, (float)rand.Next(1000) / 1000.0f);


             Texture2DDescription desc = new Texture2DDescription();
             desc.Usage = ResourceUsage.Dynamic;
             desc.Format = SlimDX.DXGI.Format.R32G32B32A32_Float;
             desc.ArraySize = 1;
             desc.MipLevels = 1;
             desc.Width = resolution;
             desc.Height = resolution;
             desc.BindFlags = BindFlags.ShaderResource;
             desc.CpuAccessFlags = CpuAccessFlags.Write;
             desc.SampleDescription = new SlimDX.DXGI.SampleDescription(1, 0);

             Texture2D noiseImage = new Texture2D(Device, desc);
             DataRectangle r = noiseImage.Map(0, MapMode.WriteDiscard, MapFlags.None);
             r.Data.WriteRange<Vector4>(noisyColors);
             noiseImage.Unmap(0);

             disposables.Add(noiseImage);
             return noiseImage;
         }

        public  void QuadUp()
        {
            RenderForm = new Form();
            RenderForm.ClientSize = new Size(800, 600);
            RenderForm.Text = "InstiBulb - DirectX 10";

            DXGI.SwapChainDescription swapChainDescription = new SlimDX.DXGI.SwapChainDescription();
            DXGI.ModeDescription modeDescription = new DXGI.ModeDescription();
            DXGI.SampleDescription sampleDescription = new DXGI.SampleDescription();

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

            using (D3D10.Texture2D resource = SwapChain.GetBuffer<D3D10.Texture2D>(0))
            {
                RenderTarget = new D3D10.RenderTargetView(Device, resource);
            }

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
            ShaderResourceView textureView = new ShaderResourceView(Device, texture);
            shaderTexture.SetResource(textureView);

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

            quad = new FullscreenQuad(Device, Pass.Description.Signature);

            tileFilters = new FilterChain();
            tileFilters.Add( 
                new BasicPostProcessingFilter(Device, "tiles", 256, 256, "RenderNesTiles", "Render")
                .DontFeedNextStage());

            tileFilters.Add(
                new BasicPostProcessingFilter(Device, "wavyTiles", 512, 512, "blue", "Wave")
                .ClearNeededResources()
                .AddNeededResource("tiles", "texture2d")
                .BindScalar("timer")
                .SetStaticResource("noiseTex", this.CreateNoiseMap(128))
                .DontFeedNextStage()
                );

            tileFilters.Add(
                new BasicPostProcessingFilter(Device, "sprites", 256, 256, "RenderNesSprites", "Render")
                .DontFeedNextStage());
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
                );
            tileFilters.Add( 
                    new BasicPostProcessingFilter(Device, "finalBlur", RenderForm.ClientRectangle.Width, RenderForm.ClientRectangle.Height, "simpleBlur", "Render")
                    .ClearNeededResources()
                    .AddNeededResource("combined", "texture2d")
                    );

            context = new ApplicationContext(RenderForm);
            
            context.ThreadExit += new EventHandler(context_ThreadExit);
            Application.Idle += new EventHandler(Application_Idle);
            
            Application.Run( context);
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
        public  void DrawFrame()
        {
            // update nes texture

            DataRectangle d =  texture.Map(0, MapMode.WriteDiscard, MapFlags.None);
            d.Data.WriteRange<int>(this.nes.PPU.OutBuffer);
            texture.Unmap(0);

            tileFilters.SetVariable("timer", timer);
            timer += 0.1f;
            tileFilters.Draw(texture);


            // render output of filter
            Device.Rasterizer.SetViewports(ViewArea);
            Device.OutputMerger.SetTargets(RenderTarget);

            Device.ClearRenderTargetView(RenderTarget, Color.Black);

            EffectResourceVariable shaderTexture = Effect.GetVariableByName("texture2d").AsResource();
            ShaderResourceView textureView = new ShaderResourceView(Device, tileFilters.Result);
            shaderTexture.SetResource(textureView);

            quad.SetupDraw();
            for (int pass = 0; pass < Technique.Description.PassCount; ++pass)
            {
                Pass.Apply();
                quad.Draw();
            }

            SwapChain.Present(0, DXGI.PresentFlags.None);
        }

        public  void Die()
        {
            Device.ClearState();
            RenderTarget.Dispose();
            Effect.Dispose();
            quad.Dispose();
            Layout.Dispose();
            Device.Dispose();
            SwapChain.Dispose();
            RenderForm.Dispose();
           // RenderForm.Close();
            //context.Dispose();
        }

         void Application_Idle(object sender, EventArgs e)
        {

        }
    }
}