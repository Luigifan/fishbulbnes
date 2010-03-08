using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D10;
using System.Drawing;
using DXGI = SlimDX.DXGI;
using SlimDX;
using SlimDXBindings.Viewer10.Helpers;


namespace SlimDXBindings.Viewer10.Filter
{

    public class ScalarBindingBase
    {
        bool isChanged;
        internal Type type;
        public Type ScalarType
        {
            get
            {
                return type;
            }
        }

        EffectScalarVariable variable;

        public EffectScalarVariable Variable
        {
            get { return variable; }
            set { variable = value; }
        }

        public bool IsChanged
        {
            get { return isChanged; }
            set { isChanged = value; }
        }
    }

    public class ScalarBinding<T> : ScalarBindingBase
    {
        T newValue;

        public ScalarBinding()
        {
            type = typeof(T);
        }

        public T NewValue
        {
            get { return newValue; }
            set { newValue = value; }
        }



    }


    public class BasicPostProcessingFilter :  IDisposable, SlimDXBindings.Viewer10.Filter.IFilterChainLink
    {

        bool feedsNextStage = true;

        internal bool isUpdated = true;

        bool hasShaderResources = false;
        bool scalarsUpdated = true;

        public virtual void Update()
        {
            isUpdated = hasShaderResources;

            foreach (var p in GetScalarList<bool>())
            {
                p.Value.Variable.Set(p.Value.NewValue);
                isUpdated = true;
                p.Value.IsChanged = false;
            }

            foreach (var p in GetScalarList<bool[]>())
            {
                p.Value.Variable.Set(p.Value.NewValue);
                isUpdated = true;
                p.Value.IsChanged = false;
            }


            foreach (var p in GetScalarList<float>())
            {
                p.Value.Variable.Set(p.Value.NewValue);
                isUpdated = true;
                p.Value.IsChanged = false;
            }

            foreach (var p in GetScalarList<int>())
            {
                p.Value.Variable.Set(p.Value.NewValue);
                isUpdated = true;
                p.Value.IsChanged = false;
            }

            foreach (var p in GetScalarList<int[]>())
            {
                p.Value.Variable.Set(p.Value.NewValue);
                isUpdated = true;
                p.Value.IsChanged = false;
            }

            foreach (var p in GetScalarList<float[]>())
            {
                p.Value.Variable.Set(p.Value.NewValue);
                isUpdated = true;
                p.Value.IsChanged = false;
            }
        }

        IEnumerable<KeyValuePair<string, ScalarBinding<T> >> GetScalarList<T>()
        {
            var list = (from sc in scalars where sc.Value.ScalarType == typeof(T) && sc.Value.IsChanged 
                        select new KeyValuePair<string, ScalarBinding<T>>(sc.Key, sc.Value as ScalarBinding<T>) ).ToList();
            return list ;
        }


        public virtual void AfterDraw()
        {
        }

        internal bool isShown = true;

        public bool FeedsNextStage
        {
            get { return feedsNextStage; }
            set { feedsNextStage = value; }
        }

        Device device;
        Texture2D texture;
        EffectBuddy effectBuddy;
        Effect Effect;
        EffectTechnique technique;
        EffectPass effectPass;
        RenderTargetView renderTarget;

        public RenderTargetView RenderTarget
        {
            get { return renderTarget; }
            set {
                if (renderTarget != null) renderTarget.Dispose();
                renderTarget = value; 
            }
        }
        FullscreenQuad quad;
        Viewport vp ;

        SlimDX.DXGI.SampleDescription sampleDescription = new SlimDX.DXGI.SampleDescription(1, 0);
        string shaderName;
        string techniqueName;
        internal int width, height;

        //Dictionary<string, ScalarBinding<bool>> scalarBools = new Dictionary<string, ScalarBinding<bool>>();
        //Dictionary<string, ScalarBinding<float>> scalarFloats = new Dictionary<string, ScalarBinding<float>>();
        //Dictionary<string, ScalarBinding<int>> scalarInts = new Dictionary<string, ScalarBinding<int>>();
        //Dictionary<string, ScalarBinding<int[]>> scalarIntArrays = new Dictionary<string, ScalarBinding<int[]>>();

        Dictionary<string, ScalarBindingBase> scalars = new Dictionary<string, ScalarBindingBase>();

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

        public BasicPostProcessingFilter(Device device, string name, int Width, int Height, string shader, string technique, EffectBuddy effectBuddy)
        {
            this.device = device;
            this.width = Width;
            this.height = Height;
            this.shaderName = shader;
            this.techniqueName = technique;
            this.filterName = name;
            this.effectBuddy = effectBuddy;
                        
            vp = new Viewport(0, 0, width, height, 0.0f, 1.0f);
            SetupFilter();
        }

        bool origTexture = true;
        void SetupFilter()
        {

            

            Effect = effectBuddy.GetEffect(shaderName);

            texture = new Texture2D(device, GetTextureDescription());

            renderTarget = new RenderTargetView(device, texture);

            technique = Effect.GetTechniqueByName(techniqueName);
            effectPass = technique.GetPassByIndex(0);

            quad = new FullscreenQuad(device, effectPass.Description.Signature);

            var p = Effect.GetVariableBySemantic("WORLDVIEWPROJECTION");
            if (p != null && p.AsMatrix() != null)
                p.AsMatrix().SetMatrix(Matrix.Identity);
        }

        public void BuildQuad(Vector4 edgePositions, Vector4 texCoords)
        {
            quad.Dispose();
            quad = new FullscreenQuad(device, effectPass.Description.Signature, edgePositions, texCoords);
            isUpdated = true;
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
            desc.BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget ;
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
                hasShaderResources = true;
                variable.SetResource(shaderRes);
            }
        }

        public void UpdateSize(int width, int height)
        {
            texture.Dispose();
            this.width = width;
            this.height = height;
            texture = new Texture2D(device, GetTextureDescription());
        }

        public virtual void ProcessEffect()
        {
            Update();
            if (isUpdated)
            {
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
            }
            AfterDraw();

        }


        public IFilterChainLink SetScalar<T>(string variableName, T constant) 
        {
            if (scalars.ContainsKey(variableName) )
            {
                if (scalars[variableName] == null)
                {
                    scalars[variableName] = new ScalarBinding<T>();
                    scalars[variableName].Variable = Effect.GetVariableByName(variableName).AsScalar();
                }

                ScalarBinding<T> bind = scalars[variableName] as ScalarBinding<T>;
                if (bind != null)
                {
                    if (!constant.Equals(bind.NewValue))
                    {
                        bind.IsChanged = true;
                        bind.NewValue = constant;
                    } 
                }
            }
            
            return this;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!notMyTexture)
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
            scalars.Add(name, null);
            return this;
        }

        public IFilterChainLink BindScalar<T>(string name)
        {
            var bind = new ScalarBinding<T>();
            bind.Variable = Effect.GetVariableByName(name).AsScalar();
            scalars.Add(name, bind );
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

        bool notMyTexture = false;
        public void RenderToTexture(Texture2D texture)
        {
            if (origTexture && this.texture!= null)
                this.texture.Dispose();
            //if (this.texture != null && !this.texture.Disposed)
            //{
            //    this.texture.Dispose();
            //}
            //notMyTexture = true;
            this.texture = texture;
            origTexture = false;
        }

        #endregion

    }
}
