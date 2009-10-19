using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using SlimDX;
using NES.CPU.nitenedo;
using NES.CPU.nitenedo.Interaction;
using SampleFramework;
using System.Windows;

namespace SlimDXBindings.Viewer
{
    public class SlimDXRenderer : IDisposable, SlimDXBindings.Viewer.ISlimDXRenderer
    {
        SlimDXControl panel;
        NESMachine nes;

        private Sprite _sprite;
        private Texture _texture;


        public SlimDXRenderer(SlimDXControl control, NESMachine nes)
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
            panel.Device.Clear(ClearFlags.Target, new Color4(System.Drawing.Color.Black), 0, 0);
            panel.Device.BeginScene();

            var rext = _texture.LockRectangle(0, LockFlags.Discard);
            rext.Data.WriteRange<int>(nes.PPU.VideoBuffer);

            _texture.UnlockRectangle(0);
            _texture.AddDirtyRectangle(new System.Drawing.Rectangle(0, 0, 256, 256));

            _sprite.Begin(SpriteFlags.AlphaBlend);
            _sprite.Draw(_texture, Vector3.Zero, Vector3.Zero, new Color4(System.Drawing.Color.White));
            _sprite.End();

            panel.Device.EndScene();
        }

        private void panel_MainLoop(object sender, EventArgs e)
        {
            Render();

            panel.AllowRendering = false;

        }

        private void panel_DeviceCreated(object sender, EventArgs e)
        {
        }

        private void panel_DeviceDestroyed(object sender, EventArgs e)
        {
            if (_sprite != null && !_sprite.Disposed)
            {
                _sprite.Dispose();
            }
            if (_texture != null && !_texture.Disposed)
            {
                _texture.Dispose();
            }
            UnloadContent();
        }

        private void panel_DeviceLost(object sender, EventArgs e)
        {
        }

        private void panel_DeviceReset(object sender, EventArgs e)
        {
            SlimDXControl control = sender as SlimDXControl;
            if (control != null)
            {
                if (_sprite != null)
                {
                    _sprite.Dispose();
                }
                _sprite = new Sprite(control.Device);
                _sprite.Transform = SlimDX.Matrix.Transformation2D(new Vector2(0, 0), 0, scaleVector, new Vector2(0, 0), 0, new Vector2(0, 0));

                if (_texture != null)
                {
                    _texture.Dispose();
                }
                //_texture = Texture.FromFile(control.Device, "test.png", Usage.None, Pool.Default);
                _texture = new Texture(control.Device, 256, 256, 0, Usage.Dynamic, Format.X8R8G8B8, Pool.Default);

                LoadContent();
            }
        }

        Vector2 scaleVector = new Vector2(1, 1);

        void panel_BackBufferSizeChanged(object sender, EventArgs e)
        {
            scaleVector = new Vector2(panel.BackBufferWidth / 256, panel.BackBufferHeight / 240);
            if (_sprite != null)
                _sprite.Transform = SlimDX.Matrix.Transformation2D(new Vector2(0, 0), 0, scaleVector, new Vector2(0, 0), 0, new Vector2(0, 0));
        }

        void panel_Loaded(object sender, RoutedEventArgs e)
        {
            panel.Initialize(true);
            if (panel.UseDeviceEx == false)
            {
                throw new InvalidDisplayContextException("You cannot create a Direct3D9Ex device.  You die and you go to hell.");
            }
            InitializeScene();
        }


        static TransformedColoredVertex[] BuildVertexData()
        {
            return new TransformedColoredVertex[4] {
                new TransformedColoredVertex(new Vector4(600.0f, 100.0f, 0.5f, 1.0f), System.Drawing.Color.Red.ToArgb()),
                new TransformedColoredVertex(new Vector4(600.0f, 500.0f, 0.5f, 1.0f), System.Drawing.Color.Blue.ToArgb()),
                new TransformedColoredVertex(new Vector4(150.0f, 500.0f, 0.5f, 1.0f), System.Drawing.Color.Green.ToArgb()), 
                new TransformedColoredVertex(new Vector4(150.0f, 100.0f, 0.5f, 1.0f), System.Drawing.Color.Purple.ToArgb()), 
            };
        }

        VertexBuffer vertices;
        protected void LoadContent()
        {
            //vertices = new VertexBuffer(panel.Device, 4 * TransformedColoredVertex.SizeInBytes, Usage.Dynamic, VertexFormat.None, Pool.Default);
            //DataStream stream = vertices.Lock(0, 0, LockFlags.None);
            //stream.WriteRange(BuildVertexData());
            //vertices.Unlock();
        }

        protected void UnloadContent()
        {
            //if (vertices != null)
            //    vertices.Dispose();
            //vertices = null;
        }


        //Camera camera = new Camera();
        bool pointLight = false;

        public void InitializeScene()
        {
            //CreateLight();

            //camera.FieldOfView = (float)(Math.PI / 4);
            //camera.NearPlane = 0.0f;
            //camera.FarPlane = 40.0f;
            //camera.Location = new Vector3(0.0f, 7.0f, 20.0f);
            //camera.Target = Vector3.Zero;
        }

        Light light;
        void CreateLight()
        {
            // yes yes, icky fixed-function pipeline stuff
            light = new Light();
            light.Type = LightType.Directional;
            light.Diffuse = System.Drawing.Color.ForestGreen;
            light.Ambient = System.Drawing.Color.GhostWhite;
            light.Direction = new Vector3(0.0f, 0.0f, 1.0f);

            if (pointLight)
            {
                light.Type = LightType.Point;
                light.Range = 100.0f;
                light.Attenuation0 = 0.05f;
                light.Attenuation1 = 0.05f;
                light.Attenuation2 = 0.03f;
                light.Diffuse = System.Drawing.Color.ForestGreen;
                light.Ambient = System.Drawing.Color.GhostWhite;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _sprite.Dispose();
            _texture.Dispose();

        }

        #endregion
    }
}
