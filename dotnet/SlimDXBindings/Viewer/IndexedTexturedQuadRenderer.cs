using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SlimDX.Direct3D9;
using SlimDX;
using SampleFramework;
using NES.CPU.nitenedo.Interaction;
using System.Drawing;
using NES.CPU.nitenedo;
using System.Windows;
using System.Reflection;
using NES.CPU.PPUClasses;

namespace SlimDXBindings.Viewer
{
    public class IndexedTexturedQuadRenderer : ISlimDXRenderer
    {
        SlimDXControl panel;
        NESMachine nes;
        NesRenderSurface nesSurfaceDrawer;

        Effect postEffect;

        Mesh mesh;

        public IndexedTexturedQuadRenderer(SlimDXControl control, NESMachine nes)
        {
            panel = control;
            this.nes = nes;
            panel.DeviceCreated += new EventHandler(panel_DeviceCreated);
            panel.DeviceDestroyed += new EventHandler(panel_DeviceDestroyed);
            panel.DeviceLost += new EventHandler(panel_DeviceLost);
            panel.DeviceReset += new EventHandler(panel_DeviceReset);
            panel.MainLoop += new EventHandler(panel_MainLoop);
            panel.BackBufferSizeChanged += new EventHandler(panel_BackBufferSizeChanged);

        }

        public virtual void Render()
        {

            // RenderNESToSurface();
            nesSurfaceDrawer.RenderNESToSurface();
            
            // now render the texture created above onto the actual screen

            RenderScene();

        }

        float timer = 0;

        public void UpdateTime()
        {
            timer += 0.01f;
        }
        EffectHandle timerHandle = new EffectHandle("timer");
        public void RenderScene()
        {
            panel.Device.Clear(ClearFlags.Target, new Color4(System.Drawing.Color.Pink), 0, 0);
            panel.Device.Clear(ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            panel.Device.BeginScene();

            postEffect.Technique = "TVertexShaderOnly";
            postEffect.SetValue(timerHandle, timer);
            postEffect.Begin();
            postEffect.BeginPass(0);
            mesh.DrawSubset(0);
            postEffect.EndPass();
            postEffect.End();

            panel.Device.EndScene();
        }

        private void panel_MainLoop(object sender, EventArgs e)
        {
            Render();

            panel.AllowRendering = false;

        }

        private void panel_DeviceCreated(object sender, EventArgs e)
        {
            LoadContent();
        }

        private void panel_DeviceDestroyed(object sender, EventArgs e)
        {

            UnloadContent();
        }

        private void panel_DeviceLost(object sender, EventArgs e)
        {
            UnloadContent();
        }

        private void panel_DeviceReset(object sender, EventArgs e)
        {
            LoadContent();
        }

        Vector2 scaleVector = new Vector2(1, 1);

        void panel_BackBufferSizeChanged(object sender, EventArgs e)
        {
            if (panel.BackBufferHeight == 0)
            {
                camera.AspectRatio = 4 / 3;
            }
            else
            {
                camera.AspectRatio = panel.BackBufferWidth / panel.BackBufferHeight;
            }
        }

        void InitializeD3D()
        {
            panel.Initialize(true);
            if (panel.UseDeviceEx == false)
            {
                throw new InvalidDisplayContextException("You cannot create a Direct3D9Ex device.  You die and you go to hell.");
            }
            InitializeScene();
        }

        void panel_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeD3D();
        }

        static TransformedColoredTexturedVertex[] GetFSQuad()
        {
            return new TransformedColoredTexturedVertex[4]
            {
                new TransformedColoredTexturedVertex(new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 
                    System.Drawing.Color.White.ToArgb(), 
                    new Vector2(1.0f, 0.0f)),
                new TransformedColoredTexturedVertex(new Vector4(1.0f, -1.0f, 1.0f, 1.0f), 
                    System.Drawing.Color.White.ToArgb(), 
                    new Vector2(1.0f, 1.0f)),
                new TransformedColoredTexturedVertex(new Vector4(-1.0f, 1.0f, 1.0f, 1.0f), 
                    System.Drawing.Color.White.ToArgb(), 
                    new Vector2(0.0f, 0.0f)),
                new TransformedColoredTexturedVertex(new Vector4(-1.0f, -1.0f, 1.0f, 1.0f), 
                    System.Drawing.Color.White.ToArgb(), 
                    new Vector2(0.0f, 1.0f))
            };

        }

        protected void LoadContent()
        {

            CleanupContent();

            nesSurfaceDrawer = new NesRenderSurface(nes, panel.Device);

            postEffect = Effect.FromStream(panel.Device,
                Assembly.GetExecutingAssembly().GetManifestResourceStream("SlimDXBindings.Viewer.PostProcess.fx"),
                ShaderFlags.None);

            mesh = Mesh.CreateBox(panel.Device, 2, 2, 2);
            mesh.ComputeNormals();
            panel.Device.ComputeBoxTextureCoords(ref mesh, true);

            InitializeScene();

        }

        private void CleanupContent()
        {
            if (nesSurfaceDrawer != null) nesSurfaceDrawer.Dispose();
            if (postEffect != null) postEffect.Dispose();
            if (mesh != null) mesh.Dispose();


        }

        protected void UnloadContent()
        {
            CleanupContent();
        }

        Camera camera = new Camera();

        public void InitializeScene()
        {
            CreateLight();
            
            camera.FieldOfView = (float)(Math.PI / 4);
            camera.NearPlane = 0.0f;
            camera.FarPlane = 40.0f;
            camera.Location = new Vector3(0.0f, 0.0f, 3.3f);
            camera.Target = Vector3.Zero;

            postEffect.Technique = "TVertexShaderOnly";
            Matrix wvp = Matrix.Multiply(Matrix.RotationZ((float)Math.PI), camera.ViewMatrix);
            wvp = Matrix.Multiply(wvp, camera.ProjectionMatrix);
            postEffect.SetValue("matWorldViewProj", wvp);
            postEffect.SetTexture("nesTexture", nesSurfaceDrawer.SurfaceTexture); 

        }

        Light light;
        void CreateLight()
        {
            light = new Light();
            light.Type = LightType.Directional;
            light.Diffuse = System.Drawing.Color.White;
            light.Ambient = System.Drawing.Color.White;
            light.Direction = new Vector3(0.0f, -7.0f, -20.0f);
        }

        #region IDisposable Members

        public void Dispose()
        {
            CleanupContent();
            timerHandle.Dispose();
        }

        #endregion

        #region ISlimDXRenderer Members


        NESPixelFormats pizelFormat = NESPixelFormats.Indexed; 
        public NESPixelFormats PixelFormat
        {
            get { return pizelFormat; }
        }

        #endregion
    }
}
