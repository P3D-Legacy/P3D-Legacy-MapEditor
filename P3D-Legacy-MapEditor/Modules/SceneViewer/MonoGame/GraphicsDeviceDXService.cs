#region File Description
//-----------------------------------------------------------------------------
// Copyright 2011, Nick Gravelyn.
// Licensed under the terms of the Ms-PL: 
// http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
//-----------------------------------------------------------------------------
#endregion

using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

using Caliburn.Micro;

using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Modules.SceneViewer.MonoGame
{
    /// <summary>
    /// Helper class responsible for creating and managing the GraphicsDevice.
    /// All GraphicsDeviceControl instances share the same GraphicsDeviceService,
    /// so even though there can be many controls, there will only ever be a single
    /// underlying GraphicsDevice. This implements the standard IGraphicsDeviceService
    /// interface, which provides notification events for when the device is reset
    /// or disposed.
    /// </summary>
    [Export(typeof(IGraphicsDeviceService))]
    [Export(typeof(GraphicsDeviceDXService))]
    public class GraphicsDeviceDXService : GraphicsDeviceServiceSingleton
    {
        // Keep track of how many controls are sharing the singletonInstance.
        private static int _referenceCount;

        private GraphicsDevice _graphicsDevice;

        // Store the current device settings.
        private PresentationParameters _parameters;

        /// <summary>
        /// Gets the current graphics device.
        /// </summary>
        public override GraphicsDevice GraphicsDevice
        {
            get
            {
                EnsureGraphicsDevice();
                return _graphicsDevice;
            }
            protected set => _graphicsDevice = value;
        }

        // IGraphicsDeviceService events.
        public override event EventHandler<EventArgs> DeviceCreated;
        public override event EventHandler<EventArgs> DeviceDisposing;
        public override event EventHandler<EventArgs> DeviceReset;
        public override event EventHandler<EventArgs> DeviceResetting;

        /// <summary>
        /// Constructor is private, because this is a singleton class:
        /// client controls should use the public AddRef method instead.
        /// </summary>
        [Obsolete("This constructor shouldn't be called directly. Instead, you should get the (singleton) instance from the IoC container.")]
        public GraphicsDeviceDXService() { }

        private void EnsureGraphicsDevice(int width = 1, int height = 1)
        {
            if (_graphicsDevice != null)
                return;

            // Create the device using the main window handle, and a placeholder size (1,1).
            // The actual size doesn't matter because whenever we render using this GraphicsDevice,
            // we will make sure the back buffer is large enough for the window we're rendering into.
            // Also, the handle doesn't matter because we call GraphicsDevice.Present(...) with the
            // actual window handle to render into.
            CreateDevice(new WindowInteropHelper(Application.Current.MainWindow).Handle, width, height);
        }

        private void CreateDevice(IntPtr windowHandle, int width, int height)
        {
            _parameters = new PresentationParameters
            {
                BackBufferWidth = Math.Max(width, 1),
                BackBufferHeight = Math.Max(height, 1),
                BackBufferFormat = SurfaceFormat.Color,
                DepthStencilFormat = DepthFormat.Depth24Stencil8,
                DeviceWindowHandle = windowHandle,
                PresentationInterval = PresentInterval.Immediate,
                IsFullScreen = false,
                //MultiSampleCount = 32
            };
            _graphicsDevice = new GraphicsDevice(
                GraphicsAdapter.DefaultAdapter,
                GraphicsProfile.HiDef,
                _parameters);

            DeviceCreated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets a reference to the singleton instance.
        /// </summary>
        public static GraphicsDeviceDXService AddRef(int width, int height)
        {
            var singletonInstance = IoC.Get<GraphicsDeviceDXService>();

            // Increment the "how many controls sharing the device" reference count.
            if (Interlocked.Increment(ref _referenceCount) == 1)
            {
                // If this is the first control to start using the
                // device, we must create the device.
                singletonInstance.EnsureGraphicsDevice(width, height);
            }

            return singletonInstance;
        }

        /// <summary>
        /// Releases a reference to the singleton instance.
        /// </summary>
        public override void Release(bool disposing)
        {
            // Decrement the "how many controls sharing the device" reference count.
            if (Interlocked.Decrement(ref _referenceCount) == 0)
            {
                // If this is the last control to finish using the
                // device, we should dispose the singleton instance.
                if (disposing)
                {
                    DeviceDisposing?.Invoke(this, EventArgs.Empty);

                    _graphicsDevice.Dispose();
                }

                _graphicsDevice = null;
            }
        }
        
        /// <summary>
        /// Resets the graphics device to whichever is bigger out of the specified
        /// resolution or its current size. This behavior means the device will
        /// demand-grow to the largest of all its GraphicsDeviceControl clients.
        /// </summary>
        public void ResetDevice(int width, int height)
        {
            var newWidth = Math.Max(_parameters.BackBufferWidth, width);
            var newHeight = Math.Max(_parameters.BackBufferHeight, height);

            if (newWidth != _parameters.BackBufferWidth || newHeight != _parameters.BackBufferHeight)
            {
                DeviceResetting?.Invoke(this, EventArgs.Empty);

                _parameters.BackBufferWidth = newWidth;
                _parameters.BackBufferHeight = newHeight;

                // TODO
                //_graphicsDevice.Reset(_parameters);

                DeviceReset?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}