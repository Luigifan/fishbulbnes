using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D10;
using System.Drawing;
using DXGI = SlimDX.DXGI;
using SlimDX;


namespace SlimDXBindings.Viewer10.Filter
{
    public class BasicPostProcessingFilter :  IDisposable, SlimDXBindings.Viewer10.Filter.IFilterChainLink
    {

        bool feedsNextStage = true;

        public bool FeedsNextStage
        {
            get { return feedsNextStage; }
            set { feedsNextStage = value; }
        }

        Device device;
        Texture2D texture;
        Effect Effect;
        EffectTechnique technique;
        EffectPass effectPass;
        RenderTargetView renderTarget;
        FullscreenQuad quad;
        Viewport vp ;

        SlimDX.DXGI.SampleDescription sampleDescription = new SlimDX.DXGI.SampleDescription(1, 0);
        string shaderName;
        string techniqueName;
        readonly int width, height;

        private List<string> boundScalars = new List<string>();

        public List<string> BoundScalars
        {
            get { return boundScalars; }
            set { boundScalars = value; }
        }

        string filterName = "none";

        public string FilterName
        {
            get { return filterName; }
            set { filterName = value; }
        }

        Dictionary<string, string> neededResources = new Dictionary<string, string>();

        // key = resourceName (name of  previous filter)
        // value = map to
        public Dictionary<string, string> NeededResources
        {
            get { return neededResources; }
            set { neededResources = value; }
        }

        public BasicPostProcessingFilter(Device device, string name, int Width, int Height, string shader, string technique)
        {
            this.device = device;
            this.width = Width;
            this.height = Height;
            this.shaderName = shader;
            this.techniqueName = technique;
            this.filterName = name;
            
            this.neededResources.Add("input", "texture2d");
            
            vp = new Viewport(0, 0, width, height, 0.0f, 1.0f);
            SetupFilter();
        }

        static System.IO.Stream GetShaderStreamFromResource(string name)
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(
                string.Format("SlimDXBindings.Viewer10.Filter.Shaders.{0}.fx", name)
                );
        }

        void SetupFilter()
        {

            Effect = Effect.FromStream(device,
                GetShaderStreamFromResource(shaderName),
                "fx_4_0", ShaderFlags.None, EffectFlags.None, null, null);

            texture = new Texture2D(device, GetTextureDescription());

            renderTarget = new RenderTargetView(device, texture);

            technique = Effect.GetTechniqueByName(techniqueName);
            effectPass = technique.GetPassByIndex(0);

            quad = new FullscreenQuad(device, effectPass.Description.Signature);
        }

        internal virtual Texture2DDescription GetTextureDescription()
        {
            Texture2DDescription desc = new Texture2DDescription();
            desc.Usage = ResourceUsage.Default;
            desc.Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm;
            desc.ArraySize = 1;
            desc.MipLevels = 1;
            desc.Width = width;
            desc.Height = height;
            desc.BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget;
            desc.SampleDescription = sampleDescription;
            return desc;
        }

        public Texture2D results
        {
            get { return texture; }
        }

        public void SetShaderResource(string variableName, Resource resource)
        {
            EffectResourceVariable variable = Effect.GetVariableByName(variableName).AsResource();
            variable.SetResource( new ShaderResourceView(device, resource));
        }

        public virtual void ProcessEffect()
        {
            device.Rasterizer.SetViewports(vp);
            device.OutputMerger.SetTargets(renderTarget);

            device.ClearRenderTargetView(renderTarget, Color.Black);

            quad.SetupDraw();
            for (int pass = 0; pass < technique.Description.PassCount; ++pass)
            {
                technique.GetPassByIndex(pass).Apply();
                //effectPass.Apply();
                quad.Draw();
            }

        }


        public void SetScalar(string variableName, float constant) 
        {
            if (boundScalars.Contains(variableName))
            {
                EffectScalarVariable variable = Effect.GetVariableByName(variableName).AsScalar();
                variable.Set(constant);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            quad.Dispose();
        }

        #endregion

        public BasicPostProcessingFilter DontFeedNextStage()
        {
            this.feedsNextStage = false;
            return this;
        }

        public BasicPostProcessingFilter ClearNeededResources()
        {
            this.neededResources.Clear();
            return this;
        }

        public BasicPostProcessingFilter AddNeededResource(string name, string bindsTo)
        {
            this.neededResources.Add(name, bindsTo);
            return this;
        }

        public BasicPostProcessingFilter BindScalar(string name)
        {
            boundScalars.Add(name);
            return this;
        }

        public BasicPostProcessingFilter SetStaticResource(string name, Resource res)
        {
            EffectResourceVariable variable = Effect.GetVariableByName(name).AsResource();
            variable.SetResource(new ShaderResourceView(device, res));
            
            return this;
        }
    }
}
