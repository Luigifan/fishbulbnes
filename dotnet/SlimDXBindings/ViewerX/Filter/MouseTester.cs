using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D10;
using SlimDX;
using System.Drawing;
using SlimDXBindings.Viewer10.Helpers;

namespace SlimDXBindings.Viewer10.Filter
{
    public class MouseTestingFilter : IFilterChainLink
    {
        bool feedsNextStage = true;

        public bool FeedsNextStage
        {
            get { return feedsNextStage; }
            set { feedsNextStage = value; }
        }

        SlimDXZapper zapper;

        public SlimDXZapper Zapper
        {
            get { return zapper; }
            set { zapper = value; }
        }

        Device device;
        Texture2D texture;
        Texture2D stage;
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

        public MouseTestingFilter(Device device, string name, string shader, string technique, EffectBuddy effectBuddy)
        {
            this.device = device;
            this.width = 8;
            this.height = 8;
            this.shaderName = shader;
            this.techniqueName = technique;
            this.filterName = name;
            this.effectBuddy = effectBuddy;
                        
            vp = new Viewport(0, 0, width, height, 0.0f, 1.0f);
            SetupFilter();
        }

        void SetupFilter()
        {

            Effect = effectBuddy.GetEffect(shaderName);

            texture = new Texture2D(device, GetTextureDescription());
            stage = new Texture2D(device, GetStagingTextureDescription());

            renderTarget = new RenderTargetView(device, texture);

            technique = Effect.GetTechniqueByName(techniqueName);
            effectPass = technique.GetPassByIndex(0);

            quad = new FullscreenQuad(device, effectPass.Description.Signature);
        }

        internal virtual Texture2DDescription GetTextureDescription()
        {
            Texture2DDescription desc = new Texture2DDescription();
            desc.Usage = ResourceUsage.Default ;
            desc.Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm;
            desc.ArraySize = 1;
            desc.MipLevels = 1;
            desc.Width = width;
            desc.Height = height;
            desc.BindFlags =  BindFlags.RenderTarget | BindFlags.ShaderResource ;
            desc.SampleDescription = sampleDescription;
            return desc;
        }

        internal virtual Texture2DDescription GetStagingTextureDescription()
        {
            Texture2DDescription desc = new Texture2DDescription();
            desc.Usage = ResourceUsage.Staging;
            desc.CpuAccessFlags = CpuAccessFlags.Read;
            desc.Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm;
            desc.ArraySize = 1;
            desc.MipLevels = 1;
            desc.Width = width;
            desc.Height = height;
            desc.BindFlags = BindFlags.None;
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
            if (zapper == null) return;
            technique = Effect.GetTechniqueByName(techniqueName);
            effectPass = technique.GetPassByIndex(0);
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
            device.Flush();
            device.CopyResource(texture, stage);

            var k = stage.Map(0, MapMode.Read, MapFlags.None);

            byte[] result = k.Data.ReadRange<byte>(64 * 4);
            stage.Unmap(0);

            // Console.WriteLine(string.Format("avg: {0}", result[result.Length/2]));
            Array.Sort<byte>(result);
            byte b = result[result.Length / 2];
            //zapper.SetLuma(b);
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

        public IFilterChainLink SetScalar(string variableName, float[] constant)
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
            stage.Dispose();
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
            if (this.texture != null && !this.texture.Disposed)
            {
                this.texture.Dispose();
            }
            this.texture = texture;
        }

        #endregion

    }
}
