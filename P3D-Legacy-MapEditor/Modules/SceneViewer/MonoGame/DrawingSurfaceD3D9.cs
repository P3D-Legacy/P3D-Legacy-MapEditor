using System;
using System.Reflection;
using System.Windows;

using Gemini.Modules.D3D.Controls;

using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Modules.SceneViewer.MonoGame
{
    public class DrawingSurfaceD3D9 : BaseD3D9DrawingSurface
    {
        /// <summary>
        /// Occurs when the control has initialized the GraphicsDevice.
        /// </summary>
        public event EventHandler<GraphicsDeviceEventArgs> LoadContent;

        /// <summary>
        /// Occurs when the control is unloading the GraphicsDevice.
        /// </summary>
        public event EventHandler<GraphicsDeviceEventArgs> UnloadContent;

        private GraphicsDeviceServiceSingleton _graphicsDeviceService;
        private RenderTarget2D _renderTarget;

        public GraphicsDevice GraphicsDevice => _graphicsDeviceService.GraphicsDevice;

        public DrawingSurfaceD3D9()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_graphicsDeviceService == null)
            {
                // We use a render target, so the back buffer dimensions don't matter.
                _graphicsDeviceService = GraphicsDeviceServiceDX.AddRef((int) ActualWidth, (int) ActualHeight);
                //_graphicsDeviceService = GraphicsDeviceService.AddRef((int) ActualWidth, (int) ActualHeight);
                _graphicsDeviceService.DeviceResetting += OnGraphicsDeviceServiceDeviceResetting;

                SetViewport();

                // Invoke the LoadContent event
                RaiseLoadContent(new GraphicsDeviceEventArgs(_graphicsDeviceService.GraphicsDevice));

                EnsureRenderTarget();
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_graphicsDeviceService != null)
            {
                //RaiseUnloadContent(new GraphicsDeviceEventArgs(GraphicsDevice));

                _graphicsDeviceService.DeviceResetting -= OnGraphicsDeviceServiceDeviceResetting;
                _graphicsDeviceService = null;
            }
        }

        private void OnGraphicsDeviceServiceDeviceResetting(object sender, EventArgs e)
        {
            RemoveBackBufferReference();
            ContentNeedsRefresh = true;
        }

        protected override void RemoveBackBufferReference()
        {
            if (_renderTarget != null)
            {
                _renderTarget.Dispose();
                _renderTarget = null;
            }

            base.RemoveBackBufferReference();
        }

        protected override void EnsureRenderTarget()
        {
            if (_renderTarget == null)
            {
                var width = Math.Max((int)ActualWidth, 100);
                var height = Math.Max((int)ActualHeight, 100);

                _renderTarget = new RenderTarget2D(GraphicsDevice, width, height,
                    false, SurfaceFormat.Bgra32, DepthFormat.Depth24Stencil8, 1, RenderTargetUsage.PlatformContents, true);

                RenderTargetPtr = GetRenderTargetPtr();
                RenderTargetRectangle = new Int32Rect(0, 0, _renderTarget.Width, _renderTarget.Height);
            }

            base.EnsureRenderTarget();
        }

        protected virtual void RaiseLoadContent(GraphicsDeviceEventArgs args)
            => LoadContent?.Invoke(this, args);

        protected virtual void RaiseUnloadContent(GraphicsDeviceEventArgs args)
            => UnloadContent?.Invoke(this, args);

        protected override void RaiseViewportChanged(SizeChangedInfo args)
        {
            SetViewport();

            base.RaiseViewportChanged(args);
        }

        protected override void RaiseDraw(DrawEventArgs args)
        {
            GraphicsDevice.SetRenderTarget(_renderTarget);
            SetViewport();
            base.RaiseDraw(args);
            _graphicsDeviceService.GraphicsDevice.Flush();
            GraphicsDevice.SetRenderTarget(null);
        }

        private void SetViewport()
        {
            // Many GraphicsDeviceControl instances can be sharing the same
            // GraphicsDevice. The device backbuffer will be resized to fit the
            // largest of these controls. But what if we are currently drawing
            // a smaller control? To avoid unwanted stretching, we set the
            // viewport to only use the top left portion of the full backbuffer.
            _graphicsDeviceService.GraphicsDevice.Viewport = new Viewport(
                0, 0, Math.Max(1, (int) ActualWidth), Math.Max(1, (int) ActualHeight));
        }

        private IntPtr GetRenderTargetPtr()
        {
            var glInfo = typeof(RenderTarget2D).GetField("glTexture", BindingFlags.Instance | BindingFlags.NonPublic);
            var glTexture = (int) (glInfo?.GetValue(_renderTarget) ?? 0);
            var glPtr = new IntPtr(glTexture);

            var dxInfo = typeof(RenderTarget2D).GetMethod("GetSharedHandle", BindingFlags.Instance | BindingFlags.Public);
            var dxPtr = (IntPtr) (dxInfo?.Invoke(_renderTarget, null) ?? IntPtr.Zero);

            if (glPtr != IntPtr.Zero)
                return glPtr;
            else if (dxPtr != IntPtr.Zero)
                return dxPtr;
            else
                throw new Exception();
        }


        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                _renderTarget?.Dispose();
                _graphicsDeviceService?.Release(disposing);
            }

            base.Dispose(disposing);
        }

        ~DrawingSurfaceD3D9()
        {
            Dispose(false);
        }

        #endregion
    }
}
