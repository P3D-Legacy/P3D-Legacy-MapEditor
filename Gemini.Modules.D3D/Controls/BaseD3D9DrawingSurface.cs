using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Gemini.Modules.D3D.Services;

using SharpDX.Direct3D9;

namespace Gemini.Modules.D3D.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseD3D9DrawingSurface : Image, IDrawingSurface, IDisposable
    {
        static BaseD3D9DrawingSurface()
        {
            StretchProperty.OverrideMetadata(typeof(BaseD3D9DrawingSurface), new FrameworkPropertyMetadata(Stretch.Fill));
            FocusableProperty.OverrideMetadata(typeof(BaseD3D9DrawingSurface), new FrameworkPropertyMetadata(true));
        }

        /// <summary>
        /// Occurs when the control changed it's size.
        /// </summary>
        public event EventHandler<SizeChangedInfo> ViewportChanged;

        /// <summary>
        /// Occurs when the DrawingSurface has been invalidated.
        /// </summary>
        public event EventHandler<DrawEventArgs> Draw;

        private D3D9ImageSource _d3DSurface;

        private TimeSpan _last = TimeSpan.Zero;

        protected IntPtr RenderTargetPtr;
        protected Int32Rect RenderTargetRectangle;

        protected bool ContentNeedsRefresh;

        /// <summary>
        /// Gets or sets a value indicating whether this control will redraw every time the CompositionTarget.Rendering event is fired.
        /// Defaults to false.
        /// </summary>
        public bool AlwaysRefresh { get; set; }

        //protected override bool HasEffectiveKeyboardFocus => true;

        protected BaseD3D9DrawingSurface()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            RemoveBackBufferReference();
            ContentNeedsRefresh = true;

            base.OnRenderSizeChanged(sizeInfo);

            RaiseViewportChanged(sizeInfo);
            EnsureRenderTarget();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _d3DSurface = new D3D9ImageSource(Window.GetWindow(this));
            _d3DSurface.IsFrontBufferAvailableChanged += OnD3DImageIsFrontBufferAvailableChanged;

            //EnsureRenderTarget();

            Source = _d3DSurface;

            CompositionTarget.Rendering += OnCompositionTargetRendering;

            ContentNeedsRefresh = true;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            RemoveBackBufferReference();

            Source = null;

            CompositionTarget.Rendering -= OnCompositionTargetRendering;
        }

        /// <summary>
        /// If we didn't do this, D3DImage would keep an reference to the backbuffer that causes the device reset below to fail.
        /// </summary>
        protected virtual void RemoveBackBufferReference()
        {
            RenderTargetPtr = IntPtr.Zero;

            _d3DSurface.SetRenderTarget(null, true);
        }

        protected virtual void EnsureRenderTarget()
        {
            if (!_d3DSurface.HasRenderTarget)
            {
                if(RenderTargetPtr == IntPtr.Zero)
                    throw new ArgumentException("RenderTarget pointer is not set!");

                _d3DSurface.SetRenderTarget(new Texture(DeviceService.D3DDevice,
                    RenderTargetRectangle.Width, RenderTargetRectangle.Height, 1, Usage.RenderTarget,
                    Format.A8R8G8B8, Pool.Default, ref RenderTargetPtr), true);
            }
        }

        // this fires when the screensaver kicks in, the machine goes into sleep or hibernate
        // and any other catastrophic losses of the d3d device from WPF's point of view
        private void OnD3DImageIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_d3DSurface.IsFrontBufferAvailable)
                ContentNeedsRefresh = true;
        }

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            var args = (RenderingEventArgs) e;
            if (args.RenderingTime == _last)
                return;
            
            _last = args.RenderingTime;



            if ((ContentNeedsRefresh || AlwaysRefresh) && BeginDraw())
            {
                ContentNeedsRefresh = false;

                EnsureRenderTarget();

                RaiseDraw(new DrawEventArgs(this));

                _d3DSurface.InvalidateD3DImage();
            }
        }

        protected virtual void RaiseDraw(DrawEventArgs args)
            => Draw?.Invoke(this, args);

        protected virtual void RaiseViewportChanged(SizeChangedInfo args)
            => ViewportChanged?.Invoke(this, args);

        private bool BeginDraw()
        {
            // If we have no graphics device, we must be running in the designer.
            if (RenderTargetPtr == IntPtr.Zero)
                return false;

            if (!_d3DSurface.IsFrontBufferAvailable)
                return false;

            return true;
        }

        public void Invalidate() => ContentNeedsRefresh = true;

        #region IDisposable

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                _d3DSurface?.Dispose();
                IsDisposed = true;
            }
        }

        ~BaseD3D9DrawingSurface()
        {
            Dispose(false);
        }

        #endregion
    }
}