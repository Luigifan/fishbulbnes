using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace SlimDXBindings.Viewer
{
    /// <summary>
    /// Description of SlimDXControl.
    /// </summary>
    public class SlimDXControl : Canvas
    {
        // we use it for 3D
        private Direct3D _direct3D;
        private Direct3DEx _direct3DEx;
        private Device _device;
        private DeviceEx _deviceEx;
        private Surface _backBufferSurface;

        private PresentParameters _pp;

        // this one is our only child
        private D3DImage _d3dimage;
        private Image image;
        private bool _startThread = false;
        private bool _sizeChanged = false;

        // some public properties
        public bool UseDeviceEx
        {
            get;
            private set;
        }

        public Direct3D Direct3D
        {
            get
            {
                if (UseDeviceEx)
                    return _direct3DEx;
                else
                    return _direct3D;
            }
        }

        public Device Device
        {
            get
            {
                if (UseDeviceEx)
                    return _deviceEx;
                else
                    return _device;
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
        /// Occurs when the PresentationParameters backbuffer size is reset
        /// </summary>
        public event EventHandler BackBufferSizeChanged;

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

        public SlimDXControl()
        {

            //Background = new ImageBrush(_d3dimage);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            _d3dimage = new D3DImage();
            image = new Image();
            image.Source = _d3dimage;
            image.Stretch = Stretch.Uniform;
            this.Children.Add(image);
            InitializeDirect3D();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            _sizeChanged = true;
        }

        void InitializeDirect3D()
        {
            try
            {
                _direct3DEx = new Direct3DEx();
                UseDeviceEx = true;
            }
            catch
            {
                _direct3D = new Direct3D();
                UseDeviceEx = false;
            }
        }

        /// <summary>
        /// Initializes the various Direct3D objects we'll be using.
        /// </summary>
        public bool Initialize(bool startThread)
        {
            try
            {
                _startThread = startThread;

                ReleaseDevice();
                HwndSource hwnd = new HwndSource(0, 0, 0, 0, 0, "test", IntPtr.Zero);

                _pp = new PresentParameters();
                _pp.SwapEffect = SwapEffect.Discard;
                _pp.DeviceWindowHandle = hwnd.Handle;
                _pp.Windowed = true;
                _pp.BackBufferWidth = (int)BackBufferWidth;
                _pp.BackBufferHeight = (int)BackBufferHeight;
                _pp.BackBufferFormat = Format.X8R8G8B8;
                BackBufferSizeChanged(this, EventArgs.Empty);
                if (UseDeviceEx)
                {
                    _deviceEx = new DeviceEx((Direct3DEx)Direct3D, 0,
                                        DeviceType.Hardware,
                                        hwnd.Handle,
                                        CreateFlags.HardwareVertexProcessing,
                                        _pp);
                }
                else
                {
                    _device = new Device(Direct3D, 0,
                                        DeviceType.Hardware,
                                        hwnd.Handle,
                                        CreateFlags.HardwareVertexProcessing,
                                        _pp);
                }

                // call the users one
                OnDeviceCreated(EventArgs.Empty);
                OnDeviceReset(EventArgs.Empty);

                // only if startThread is true
                if (_startThread)
                {
                    CompositionTarget.Rendering += OnRendering;
                    _d3dimage.IsFrontBufferAvailableChanged += new DependencyPropertyChangedEventHandler(OnIsFrontBufferAvailableChanged);
                }
                _d3dimage.Lock();
                _backBufferSurface = Device.GetBackBuffer(0, 0);
                _d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _backBufferSurface.ComPointer);
                _d3dimage.Unlock();
                //this.Background = new ImageBrush(_d3dimage);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ReleaseDevice()
        {
            if (_device != null)
            {
                if (!_device.Disposed)
                {
                    _device.Dispose();
                    _device = null;
                    OnDeviceDestroyed(EventArgs.Empty);
                }
            }

            if (_deviceEx != null)
            {
                if (!_deviceEx.Disposed)
                {
                    _deviceEx.Dispose();
                    _device = null;
                    OnDeviceDestroyed(EventArgs.Empty);
                }
            }

            _d3dimage.Lock();
            _d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
            _d3dimage.Unlock();

            ReleaseBackBuffer();
        }

        public void ReleaseDirect3D()
        {
            if (_direct3D != null && !_direct3D.Disposed)
            {
                _direct3D.Dispose();
                _direct3D = null;
            }

            if (_direct3DEx != null && !_direct3DEx.Disposed)
            {
                _direct3DEx.Dispose();
                _direct3DEx = null;
            }
        }

        public bool AllowRendering
        {
            get;
            set;
        }

        public int BackBufferWidth
        {
            get { return (int)ActualWidth; }
        }

        public int BackBufferHeight { get { return (int)ActualHeight; } }
        private void OnRendering(object sender, EventArgs e)
        {
            Result result;

            if (!AllowRendering) return;

            try
            {
                if (Device == null)
                    Initialize(_startThread);

                if (_sizeChanged)
                {
                    _pp.BackBufferWidth = (int)BackBufferWidth;
                    _pp.BackBufferHeight = (int)BackBufferHeight;
                    ReleaseBackBuffer();
                    Device.Reset(_pp);
                    OnDeviceReset(EventArgs.Empty);
                    BackBufferSizeChanged(this, EventArgs.Empty);
                }

                if (_d3dimage.IsFrontBufferAvailable)
                {
                    result = Device.TestCooperativeLevel();
                    if (result.IsFailure)
                    {
                        throw new Direct3D9Exception();
                    }
                    _d3dimage.Lock();
                    OnMainLoop(EventArgs.Empty);
                    Device.Present();

                    _backBufferSurface = Device.GetBackBuffer(0, 0);
                    _d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _backBufferSurface.ComPointer);
                    _d3dimage.AddDirtyRect(new Int32Rect(0, 0, _d3dimage.PixelWidth, _d3dimage.PixelHeight));
                    _d3dimage.Unlock();
                }
            }
            catch (Direct3D9Exception ex)
            {
                string msg = ex.Message;
                Initialize(_startThread);
            }
            _sizeChanged = false;
        }

        private void ReleaseBackBuffer()
        {
            if (_backBufferSurface != null && !_backBufferSurface.Disposed)
            {
                _backBufferSurface.Dispose();
                _backBufferSurface = null;
            }
        }

        void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_d3dimage.IsFrontBufferAvailable)
            {
                Initialize(_startThread);
            }
            else
            {
                CompositionTarget.Rendering -= OnRendering;
            }
        }


        #region IDisposable Members

        #endregion
    }


}
