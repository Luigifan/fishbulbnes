using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D10;
using System.Drawing;
using SlimDXBindings.Viewer10.Helpers;

namespace SlimDXBindings.Viewer10.Filter
{
    public class WPFVisualChainLink : IFilterChainLink
    {
        bool feedsNextStage = true;

        public bool FeedsNextStage
        {
            get { return feedsNextStage; }
            set { feedsNextStage = value; }
        }

        Device device;
        WPFVisualTexture texture;
        EffectBuddy effectBuddy;
        Effect Effect;
        EffectTechnique technique;
        EffectPass effectPass;
        RenderTargetView renderTarget;

        public RenderTargetView RenderTarget
        {
            get { return renderTarget; }
            set { renderTarget = value; }
        }
        FullscreenQuad quad;
        Viewport vp ;

        SlimDX.DXGI.SampleDescription sampleDescription = new SlimDX.DXGI.SampleDescription(1, 0);
        string shaderName;
        string techniqueName;
        readonly internal int width, height;

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

        public WPFVisualChainLink(Device device, string name, int Width, int Height, string shader, string technique, EffectBuddy effectBuddy, WPFVisualTexture texture)
        {
            this.device = device;
            this.width = Width;
            this.height = Height;
            this.shaderName = shader;
            this.techniqueName = technique;
            this.filterName = name;
            this.effectBuddy = effectBuddy;
            this.texture = texture;
                        
            vp = new Viewport(0, 0, width, height, 0.0f, 1.0f);
            SetupFilter();
        }

        void SetupFilter()
        {

            Effect = effectBuddy.GetEffect(shaderName);

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
        Dictionary<string, ShaderResourceView> shaderResources = new Dictionary<string, ShaderResourceView>();
        public void SetShaderResource(string variableName, Resource resource)
        {
            if (!shaderResources.ContainsKey(variableName) && resource != null)
            {
                EffectResourceVariable variable = Effect.GetVariableByName(variableName).AsResource();
                var shaderRes = new ShaderResourceView(device, resource);
                shaderResources.Add(variableName, shaderRes);
                variable.SetResource(shaderRes);
            }
        }

        public virtual void ProcessEffect()
        {
            technique = Effect.GetTechniqueByName(techniqueName);
            effectPass = technique.GetPassByIndex(0);
            device.Rasterizer.SetViewports(vp);
            device.OutputMerger.SetTargets(renderTarget);
            device.ClearRenderTargetView(renderTarget, Color.Black);

            if (texture.IsDirty)
            {
                texture.UpdateVisual();
            }

            quad.SetupDraw();
            for (int pass = 0; pass < technique.Description.PassCount; ++pass)
            {
                technique.GetPassByIndex(pass).Apply();
                //effectPass.Apply();
                quad.Draw();
            }

        }


        public IFilterChainLink SetScalar(string variableName, float constant) 
        {
            if (boundScalars.Contains(variableName))
            {
                EffectScalarVariable variable = Effect.GetVariableByName(variableName).AsScalar();
                variable.Set(constant);
            }
            return this;
        }

        public IFilterChainLink SetScalar(string variableName, int[] constant)
        {
            if (boundScalars.Contains(variableName))
            {
                EffectScalarVariable variable = Effect.GetVariableByName(variableName).AsScalar();
                variable.Set(constant);
            }
            return this;
        }

        public IFilterChainLink SetScalar<T>(string variableName, T constant) 
        {
            if (boundScalars.Contains(variableName))
            {
                EffectScalarVariable variable = Effect.GetVariableByName(variableName).AsScalar();
                if (typeof(T) == typeof(bool))
                    variable.Set((constant as bool?).GetValueOrDefault(false));
                if (typeof(T) == typeof(float))
                    variable.Set((constant as float?).Value);
                if (typeof(T) == typeof(int[]))
                    variable.Set((constant as int[]));
                
            }
            return this;
        }

        #region IDisposable Members

        public void Dispose()
        {

            texture.Dispose();
            Effect.Dispose();
            renderTarget.Dispose();
            quad.Dispose();

            foreach (var shaderRes in shaderResources.Values)
            {
                shaderRes.Dispose();
            }
        }

        #endregion

        public IFilterChainLink DontFeedNextStage()
        {
            this.feedsNextStage = false;
            return this;
        }

        public IFilterChainLink ClearNeededResources()
        {
            this.neededResources.Clear();
            return this;
        }

        public IFilterChainLink AddNeededResource(string name, string bindsTo)
        {
            this.neededResources.Add(name, bindsTo);
            return this;
        }

        public IFilterChainLink BindScalar(string name)
        {
            boundScalars.Add(name);
            return this;
        }

        public IFilterChainLink SetStaticResource(string name, Resource res)
        {

            if (!shaderResources.ContainsKey(name))
            {
                EffectResourceVariable variable = Effect.GetVariableByName(name).AsResource();
                if (variable != null)
                {
                    var shaderRes = new ShaderResourceView(device, res);
                    shaderResources.Add(name, shaderRes);
                    variable.SetResource(shaderRes);
                }
            }
           
            return this;
        }

        public IFilterChainLink SetMatrix(string variableName, Matrix matrix)
        {
            EffectMatrixVariable varible = Effect.GetVariableByName(variableName).AsMatrix();
            varible.SetMatrix(matrix);
            return this;
        }

        public IFilterChainLink SetViewport(Viewport viewport)
        {
            this.vp = viewport;
            return this;
        }


        #region IFilterChainLink Members


        public void RenderToTexture(Texture2D texture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
