using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using SlimDX;
using SlimDX.Direct3D9;
using System.IO;
using SampleFramework;

namespace SlimDXNESViewer
{
    /// <summary>
    /// Description of ControlWPF.
    /// </summary>
    public class D3DPanel : Border, IDisposable
    {
        // we use it for 3D
        Direct3D direct3D;
        Direct3DEx direct3DEx;
        Device device;
        DeviceEx deviceEx;
        PresentParameters pp;
        VertexBuffer vertices;
        Sprite sprite;
        Texture tex;
        System.Drawing.Rectangle rect;

        IntPtr _scene;

        // this one is our only child
        System.Windows.Controls.Image image;
        D3DImage d3dimage;
        public bool StartThread = false;
        bool sizeChanged = false;

        // some public properties
        public bool useDeviceEx
        {
            get;
            private set;
        }

        byte[] pixBytes = new byte[256 * 256 * 4];
        bool texWritten = false;
        
        public void UpdatePixBuf(int[] pixels)
        {
            texWritten = true;
            Buffer.BlockCopy(pixels, 0, pixBytes, 0, pixBytes.Length);
        }

        public Direct3D Direct3D
        {
            get
            {
                if (useDeviceEx)
                    return direct3DEx;
                else
                    return direct3D;
            }
        }

        public Device Device
        {
            get
            {
                if (useDeviceEx)
                    return deviceEx;
                else
                    return device;
            }
        }

        #region Events

        /// <summary>
        /// Occurs once per iteration of the main loop.
        /// </summary>
        public event EventHandler MainLoop;

        /// <summary>
        /// Occurs when the device is created.
        /// </summary>
        public event EventHandler DeviceCreated;

        /// <summary>
        /// Occurs when the device is destroyed.
        /// </summary>
        public event EventHandler DeviceDestroyed;

        /// <summary>
        /// Occurs when the device is lost.
        /// </summary>
        public event EventHandler DeviceLost;

        /// <summary>
        /// Occurs when the device is reset.
        /// </summary>
        public event EventHandler DeviceReset;

        /// <summary>
        /// Raises the OnInitialize event.
        /// </summary>
        protected virtual void OnInitialize()
        {
        }

        /// <summary>
        /// Raises the <see cref="E:MainLoop"/> event.
        /// </summary>
        protected virtual void OnMainLoop(EventArgs e)
        {
            if (MainLoop != null)
                MainLoop(this, e);
        }

        /// <summary>
        /// Raises the DeviceCreated event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceCreated(EventArgs e)
        {
            if (DeviceCreated != null)
                DeviceCreated(this, e);
        }

        /// <summary>
        /// Raises the DeviceDestroyed event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceDestroyed(EventArgs e)
        {
            if (DeviceDestroyed != null)
                DeviceDestroyed(this, e);
        }

        /// <summary>
        /// Raises the DeviceLost event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceLost(EventArgs e)
        {
            if (DeviceLost != null)
                DeviceLost(this, e);
        }

        /// <summary>
        /// Raises the DeviceReset event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceReset(EventArgs e)
        {
            if (DeviceReset != null)
                DeviceReset(this, e);
        }


        #endregion

        public D3DPanel()
        {
            d3dimage = new D3DImage(256, 256);
            d3dimage.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;
            
            this.Background = new ImageBrush( d3dimage);
            BeginRenderingScene();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            InitializeDirect3D();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            sizeChanged = true;
        }

        void InitializeDirect3D()
        {
            try
            {
                direct3DEx = new Direct3DEx();
                useDeviceEx = true;
            }
            catch
            {
                direct3D = new Direct3D();
                useDeviceEx = false;
            }
        }

        /// <summary>
        /// Initializes the various Direct3D objects we'll be using.
        /// </summary>
        public bool SetupDisplay()
        {
            try
            {

                ReleaseD3D();
                HwndSource hwnd = new HwndSource(0, 0, 0, 0, 0, "test", IntPtr.Zero);

                pp = new PresentParameters();
                pp.SwapEffect = SwapEffect.Discard;
                pp.DeviceWindowHandle = hwnd.Handle;
                pp.Windowed = true;
                pp.BackBufferWidth = (int)this.ActualWidth;
                pp.BackBufferHeight = (int)this.ActualHeight;
                pp.BackBufferFormat = Format.X8R8G8B8;

                if (useDeviceEx)
                {
                    deviceEx = new DeviceEx((Direct3DEx)Direct3D, 0,
                                        DeviceType.Hardware,
                                        hwnd.Handle,
                                        CreateFlags.HardwareVertexProcessing,
                                        pp);
                }
                else
                {
                    device = new Device(Direct3D, 0,
                                        DeviceType.Hardware,
                                        hwnd.Handle,
                                        CreateFlags.HardwareVertexProcessing,
                                        pp);
                }

                // call the users one
                OnDeviceCreated(EventArgs.Empty);
                OnDeviceReset(EventArgs.Empty);

                sprite = new Sprite(Device);
                
                //tex = new Texture(Device, 256, 256, 0, Usage.Dynamic, Format.X8R8G8B8, Pool.Default);

                tex = Texture.FromFile(Device, @"C:\Users\Public\Pictures\Sample Pictures\Koala.JPG", Usage.Dynamic, Pool.Default);
                
                vertices = new VertexBuffer(Device, 4 * TransformedColoredVertex.SizeInBytes, Usage.WriteOnly, VertexFormat.None, Pool.Default);
                DataStream stream = vertices.Lock(0, 0, LockFlags.None);
                stream.WriteRange(BuildVertexData());
                vertices.Unlock();
                // only if startThread is true
                _scene = Device.GetBackBuffer(0, 0).ComPointer;
                return true;
            }
            catch
            {

                _scene = IntPtr.Zero;
                return false;
            }
        }
        static TransformedColoredVertex[] BuildVertexData()
        {
            return new TransformedColoredVertex[4] {
                new TransformedColoredVertex(new Vector4(150.0f, 100.0f, 0.5f, 1.0f), System.Drawing.Color.White.ToArgb()),
                new TransformedColoredVertex(new Vector4(650.0f, 100.0f, 0.5f, 1.0f), System.Drawing.Color.White.ToArgb()),
                new TransformedColoredVertex(new Vector4(650.0f, 500.0f, 0.5f, 1.0f), System.Drawing.Color.White.ToArgb()),
                new TransformedColoredVertex(new Vector4(150.0f, 500.0f, 0.5f, 1.0f), System.Drawing.Color.White.ToArgb()) };
        }
        public void ReleaseD3D()
        {
            if (device != null)
            {
                if (!device.Disposed)
                {
                    device.Dispose();
                    device = null;
                }
            }
            d3dimage.Lock();
            d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
            d3dimage.Unlock();
        }


        public void DrawTexture()
        {

            try
            {
                if (tex != null)
                {
                    var rext = tex.LockRectangle(0, LockFlags.Discard);
                    
                    rext.Data.Write(pixBytes,0,pixBytes.Length);
                    tex.UnlockRectangle(0);
                    tex.AddDirtyRectangle(new System.Drawing.Rectangle(0, 0, 256, 256));
                }
            }
            catch
            {
            //   Console.WriteLine("");
            }
        }

        private void DrawScene()
        {
            Result result;

            try
            {

                if (d3dimage.IsFrontBufferAvailable)
                {
                    result = Device.TestCooperativeLevel();
                    if (result.IsFailure)
                    {
                       throw new Direct3D9Exception();
                    }

                    Device.Clear(ClearFlags.Target, new Color4(System.Drawing.Color.Yellow), 0, 0);
                    Device.BeginScene();
                    // call the users method
                    // OnMainLoop(EventArgs.Empty);
                    //DrawTexture();
                    

                    Device.SetStreamSource(0, vertices, 0, TransformedColoredVertex.SizeInBytes);
                    Device.VertexFormat = TransformedColoredVertex.Format;
                    Device.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                    
                    sprite.Begin(SpriteFlags.Billboard);
                    rect = new System.Drawing.Rectangle(0, 0, 720, 480);

                    sprite.Draw(tex, Vector3.Zero, new Vector3(0,0,0), System.Drawing.Color.White);
                    sprite.End();


                    Device.EndScene();
                    Device.Present(Present.DoNotWait);

                    
                }
            }
            catch (Direct3D9Exception ex)
            {
                string msg = ex.Message;
                Console.WriteLine(msg);
                SetupDisplay();
            }
            sizeChanged = false;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (Device == null)
                SetupDisplay();

            if (sizeChanged)
            {
                pp.BackBufferWidth = (int)ActualWidth;
                pp.BackBufferHeight = (int)ActualHeight;
                Device.Reset(pp);
                OnDeviceReset(EventArgs.Empty);
            }

            UpdateScene();
        }

        void UpdateScene()
        {
            if (d3dimage.IsFrontBufferAvailable && _scene != IntPtr.Zero)
            {
                d3dimage.Lock();
                DrawScene();
                d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _scene);
                d3dimage.AddDirtyRect(new Int32Rect(0, 0, d3dimage.PixelWidth, d3dimage.PixelHeight));
                d3dimage.Unlock();
            }
        }

        void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (d3dimage.IsFrontBufferAvailable)
            {
                BeginRenderingScene();
                //DrawScene();
            }
            else
            {
                EndRenderingScene();
            }
        }

        private void BeginRenderingScene()
        {
            if (d3dimage.IsFrontBufferAvailable)
            {
                // create a custom D3D scene and get a pointer to its surface
                // (this is a call into our custom unmanaged library)
                SetupDisplay();

                // set the back buffer using the new scene
                d3dimage.Lock();
                d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _scene);
                d3dimage.Unlock();

                // leverage the Rendering event of WPF's composition target to
                // update the custom D3D scene
                CompositionTarget.Rendering += OnRendering;
            }
        }

        private void EndRenderingScene()
        {
            CompositionTarget.Rendering -= OnRendering;
            ReleaseD3D();
            _scene = IntPtr.Zero;
        }


        #region IDisposable Members

        public void Dispose()
        {
            ReleaseD3D();
        }

        #endregion
    }
}
