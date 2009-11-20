using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D10;
using System.Drawing;
using SlimDX;
using SlimDXBindings.Viewer10.Helpers;

namespace SlimDXBindings.Viewer10.Filter
{
    public class ToolStrip : IFilterChainLink
    {

        XamlModel3DMeshFactory myMesh;

        bool feedsNextStage = true;

        public bool FeedsNextStage
        {
            get { return feedsNextStage; }
            set { feedsNextStage = value; }
        }

        Device device;
        Texture2D texture;
        TextureBuddy textureBuddy;
        RenderTargetView renderTarget;

        int spriteCount;
        
        Sprite spr;
        SpriteInstance[] sprites = new SpriteInstance[1];
        Texture2D[] spriteTex = new Texture2D[1];

        DepthBuffer dBuffer;

        public RenderTargetView RenderTarget
        {
            get { return renderTarget; }
            set { renderTarget = value; }
        }
        FullscreenQuad quad;
        Viewport vp;

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


        public ToolStrip(Device device, string name, int Width, int Height, TextureBuddy textureBuddy, int itemCount)
        {
            this.device = device;
            this.width = Width;
            this.height = Height;
            this.filterName = name;
            this.textureBuddy = textureBuddy;
            spriteCount = itemCount;

            vp = new Viewport(0, 0, width, height, 0.0f, 1.0f);
            SetupFilter();
        }

        void SetupFilter()
        {

            texture = new Texture2D(device, GetTextureDescription());

            renderTarget = new RenderTargetView(device, texture);

            spr = new Sprite(device, spriteCount);

            sprites =new SpriteInstance[spriteCount];

            //sprites[0] = new SpriteInstance(new ShaderResourceView(device, spriteTex[0]), new Vector2(0, 0), new Vector2(1.0f, 1.0f));

        }

        public void AddTextures(Texture2D[] texArray)
        {
            float scaleFactor = 1.75f / (float)(texArray.Length);
            float offsetFactor = 2.0f / (float)(texArray.Length );

            for (int i = 0; i < texArray.Length; ++i)
            {
                sprites[i] = new SpriteInstance(new ShaderResourceView(device, texArray[i]), new Vector2(0, 0), new Vector2(1.0f, 1.0f));
                //sprites[i].Transform = Matrix.Transformation2D(new Vector2(0, 0), 0, new Vector2(1, 1.1f), // scaling
                //        new Vector2(0, 0), 0, // rotation
                //        new Vector2(-0.5f + (offsetFactor * (i)), 0.0f));

                sprites[i].Transform = Matrix.Scaling(scaleFactor, 1.75f, 1) * Matrix.Translation(-1.0f + (offsetFactor / 2) + (offsetFactor * i),0,0);

                // translation
            }
            // screen from -1 to 1, sprites default size 1

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

        internal virtual Texture2DDescription GetDepthDescription()
        {
            Texture2DDescription desc = new Texture2DDescription();
            desc.Usage = ResourceUsage.Default;
            desc.Format = SlimDX.DXGI.Format.D16_UNorm;
            desc.ArraySize = 1;
            desc.MipLevels = 1;
            desc.Width = width;
            desc.Height = height;
            desc.BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource | BindFlags.RenderTarget;
            desc.SampleDescription = sampleDescription;
            return desc;
        }

        public Texture2D results
        {
            get { return texture; }
        }
        public void SetShaderResource(string variableName, Resource resource)
        {
        }

        public virtual void ProcessEffect()
        {

            //SetMatrix("WorldViewProj", camera.ViewMatrix * camera.ProjectionMatrix);

            //technique = Effect.GetTechniqueByName(techniqueName);
            //effectPass = technique.GetPassByIndex(0);
            device.Rasterizer.SetViewports(vp);
            device.OutputMerger.SetTargets(renderTarget);
            device.ClearRenderTargetView(renderTarget, Color.Wheat);

            spr.Begin(SpriteFlags.None);
            spr.DrawBuffered(sprites);
            spr.Flush();
            spr.End();
        }


        public IFilterChainLink SetScalar(string variableName, float constant)
        {
            return this;
        }

        public IFilterChainLink SetScalar(string variableName, int[] constant)
        {

            return this;
        }

        public IFilterChainLink SetMatrix(string variableName, Matrix matrix)
        {
            return this;
        }

        public IFilterChainLink SetScalar<T>(string variableName, T constant)
        {

            return this;
        }

        #region IDisposable Members

        public void Dispose()
        {

            texture.Dispose();
            renderTarget.Dispose();
            //quad.Dispose();
            //myMesh.Dispose();

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

        #region IFilterChainLink Members


        public IFilterChainLink SetScalar(string variableName, float[] constant)
        {
            return this;
        }

        #endregion
    }
}
