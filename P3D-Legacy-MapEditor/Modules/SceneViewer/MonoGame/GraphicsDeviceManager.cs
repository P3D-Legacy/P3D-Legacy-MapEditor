using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

using Caliburn.Micro;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Modules.SceneViewer.MonoGame
{
    /// <summary>
    /// Used to initialize and control the presentation of the graphics device.
    /// </summary>
    [Export(typeof(IGraphicsDeviceService))]
    [Export(typeof(GraphicsDeviceManager))]
    public partial class GraphicsDeviceManager : GraphicsDeviceServiceSingleton, IDisposable
    {
        // Keep track of how many controls are sharing the singletonInstance.
        private static int _referenceCount;

        /// <summary>
        /// Gets a reference to the singleton instance.
        /// </summary>
        public static GraphicsDeviceManager AddRef(int width, int height)
        {
            var singletonInstance = IoC.Get<GraphicsDeviceManager>();

            // Increment the "how many controls sharing the device" reference count.
            if (Interlocked.Increment(ref _referenceCount) == 1)
            {
                // If this is the first control to start using the
                // device, we must create the device.
                singletonInstance.ApplyChanges();
                //singletonInstance.EnsureGraphicsDevice(width, height);
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

                    GraphicsDevice.Dispose();
                }

                GraphicsDevice = null;
            }
        }




        //private readonly Game _game;
        private bool _initialized;

        private int _preferredBackBufferHeight;
        private int _preferredBackBufferWidth;
        private SurfaceFormat _preferredBackBufferFormat;
        private DepthFormat _preferredDepthStencilFormat;
        private bool _preferMultiSampling;
        private bool _synchronizedWithVerticalRetrace = true;
        private bool _disposed;
        private bool _hardwareModeSwitch = true;
        private bool _wantFullScreen;
        private GraphicsProfile _graphicsProfile;
        // dirty flag for ApplyChanges
        private bool _shouldApplyChanges;

        /// <summary>
        /// The default back buffer width.
        /// </summary>
        public static readonly int DefaultBackBufferWidth = 800;

        /// <summary>
        /// The default back buffer height.
        /// </summary>
        public static readonly int DefaultBackBufferHeight = 480;

        /// <summary>
        /// Optional override for platform specific defaults.
        /// </summary>
        partial void PlatformConstruct();

        /// <summary>
        /// Associates this graphics device manager to a game instances.
        /// </summary>
        /// <param name="game">The game instance to attach.</param>
        //public GraphicsDeviceManager(Game game)
        public GraphicsDeviceManager()
        {
            _preferredBackBufferFormat = SurfaceFormat.Color;
            _preferredDepthStencilFormat = DepthFormat.Depth24;
            _synchronizedWithVerticalRetrace = true;

            // Assume the window client size as the default back 
            // buffer resolution in the landscape orientation.
            //var clientBounds = _game.Window.ClientBounds;
            //var clientBounds = new Rectangle(0, 0, (int) Application.Current.MainWindow.ActualWidth, (int) Application.Current.MainWindow.ActualHeight);
            var clientBounds = new Rectangle(0, 0, 1, 1);
            if (clientBounds.Width >= clientBounds.Height)
            {
                _preferredBackBufferWidth = clientBounds.Width;
                _preferredBackBufferHeight = clientBounds.Height;
            }
            else
            {
                _preferredBackBufferWidth = clientBounds.Height;
                _preferredBackBufferHeight = clientBounds.Width;
            }

            // Default to windowed mode... this is ignored on platforms that don't support it.
            _wantFullScreen = false;

            GraphicsProfile = GraphicsProfile.HiDef;

            // Let the plaform optionally overload construction defaults.
            PlatformConstruct();
        }

        ~GraphicsDeviceManager()
        {
            Dispose(false);
        }

        private void CreateDevice()
        {
            if (GraphicsDevice != null)
                return;

            try
            {
                if (!_initialized)
                    Initialize();

                var gdi = DoPreparingDeviceSettings();
                CreateDevice(gdi);
            }
            catch (NoSuitableGraphicsDeviceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NoSuitableGraphicsDeviceException("Failed to create graphics device!", ex);
            }
        }

        private void CreateDevice(GraphicsDeviceInformation gdi)
        {
            if (GraphicsDevice != null)
                return;

            GraphicsDevice = new GraphicsDevice(gdi.Adapter, gdi.GraphicsProfile, gdi.PresentationParameters);
            _shouldApplyChanges = false;

            // hook up reset events
            GraphicsDevice.DeviceReset += (sender, args) => OnDeviceReset(args);
            GraphicsDevice.DeviceResetting += (sender, args) => OnDeviceResetting(args);

            OnDeviceCreated(EventArgs.Empty);
        }

        #region IGraphicsDeviceService Members

        public override event EventHandler<EventArgs> DeviceCreated;
        public override event EventHandler<EventArgs> DeviceDisposing;
        public override event EventHandler<EventArgs> DeviceReset;
        public override event EventHandler<EventArgs> DeviceResetting;
        public event EventHandler<PreparingDeviceSettingsEventArgs> PreparingDeviceSettings;
        public event EventHandler<EventArgs> Disposed;

        protected void OnDeviceDisposing(EventArgs e) => DeviceDisposing?.Invoke(this, e);

        protected void OnDeviceResetting(EventArgs e) => DeviceResetting?.Invoke(this, e);

        internal void OnDeviceReset(EventArgs e) => DeviceReset?.Invoke(this, e);

        internal void OnDeviceCreated(EventArgs e) => DeviceCreated?.Invoke(this, e);

        /// <summary>
        /// This populates a GraphicsDeviceInformation instance and invokes PreparingDeviceSettings to
        /// allow users to change the settings. Then returns that GraphicsDeviceInformation.
        /// Throws NullReferenceException if users set GraphicsDeviceInformation.PresentationParameters to null.
        /// </summary>
        private GraphicsDeviceInformation DoPreparingDeviceSettings()
        {
            var gdi = new GraphicsDeviceInformation();
            PrepareGraphicsDeviceInformation(gdi);
            var preparingDeviceSettingsHandler = PreparingDeviceSettings;

            if (preparingDeviceSettingsHandler != null)
            {
                // this allows users to overwrite settings through the argument
                var args = new PreparingDeviceSettingsEventArgs(gdi);
                preparingDeviceSettingsHandler(this, args);

                if (gdi.PresentationParameters == null || gdi.Adapter == null)
                    throw new NullReferenceException("Members should not be set to null in PreparingDeviceSettingsEventArgs");
            }

            return gdi;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (GraphicsDevice != null)
                    {
                        GraphicsDevice.Dispose();
                        GraphicsDevice = null;
                    }
                }
                _disposed = true;
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        partial void PlatformApplyChanges();

        partial void PlatformPreparePresentationParameters(PresentationParameters presentationParameters);

        private void PreparePresentationParameters(PresentationParameters presentationParameters)
        {
            presentationParameters.BackBufferFormat = _preferredBackBufferFormat;
            presentationParameters.BackBufferWidth = _preferredBackBufferWidth;
            presentationParameters.BackBufferHeight = _preferredBackBufferHeight;
            presentationParameters.DepthStencilFormat = _preferredDepthStencilFormat;
            presentationParameters.IsFullScreen = _wantFullScreen;
            //presentationParameters.HardwareModeSwitch = _hardwareModeSwitch;
            presentationParameters.PresentationInterval = _synchronizedWithVerticalRetrace ? PresentInterval.One : PresentInterval.Immediate;
            presentationParameters.DisplayOrientation = DisplayOrientation.Default;
            presentationParameters.DeviceWindowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;

            if (_preferMultiSampling)
            {
                // always initialize MultiSampleCount to the maximum, if users want to overwrite
                // this they have to respond to the PreparingDeviceSettingsEvent and modify
                // args.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount
                //presentationParameters.MultiSampleCount = GraphicsDevice != null
                //    ? GraphicsDevice.GraphicsCapabilities.MaxMultiSampleCount
                //    : 32;
            }
            else
            {
                presentationParameters.MultiSampleCount = 0;
            }

            PlatformPreparePresentationParameters(presentationParameters);
        }

        private void PrepareGraphicsDeviceInformation(GraphicsDeviceInformation gdi)
        {
            gdi.Adapter = GraphicsAdapter.DefaultAdapter;
            gdi.GraphicsProfile = GraphicsProfile;
            var pp = new PresentationParameters();
            PreparePresentationParameters(pp);
            gdi.PresentationParameters = pp;
        }

        /// <summary>
        /// Applies any pending property changes to the graphics device.
        /// </summary>
        public void ApplyChanges()
        {
            // If the device hasn't been created then create it now.
            if (GraphicsDevice == null)
                CreateDevice();

            if (!_shouldApplyChanges)
                return;

            _shouldApplyChanges = false;

            // Allow for optional platform specific behavior.
            PlatformApplyChanges();

            // populates a gdi with settings in this gdm and allows users to override them with
            // PrepareDeviceSettings event this information should be applied to the GraphicsDevice
            var gdi = DoPreparingDeviceSettings();

            if (gdi.GraphicsProfile != GraphicsDevice.GraphicsProfile)
            {
                // if the GraphicsProfile changed we need to create a new GraphicsDevice
                DisposeGraphicsDevice();
                CreateDevice(gdi);
                return;
            }

            GraphicsDevice.Reset(gdi.PresentationParameters);
        }

        private void DisposeGraphicsDevice()
        {
            GraphicsDevice.Dispose();
            DeviceDisposing?.Invoke(this, EventArgs.Empty);
            GraphicsDevice = null;
        }

        partial void PlatformInitialize(PresentationParameters presentationParameters);

        private void Initialize()
        {
            var presentationParameters = new PresentationParameters();
            PreparePresentationParameters(presentationParameters);

            // Allow for any per-platform changes to the presentation.
            PlatformInitialize(presentationParameters);

            _initialized = true;
        }

        /// <summary>
        /// Toggles between windowed and fullscreen modes.
        /// </summary>
        /// <remarks>
        /// Note that on platforms that do not support windowed modes this has no affect.
        /// </remarks>
        public void ToggleFullScreen()
        {
            IsFullScreen = !IsFullScreen;
            ApplyChanges();
        }

        /// <summary>
        /// The profile which determines the graphics feature level.
        /// </summary>
        public GraphicsProfile GraphicsProfile
        {
            get => _graphicsProfile;
            set
            {
                _shouldApplyChanges = true;
                _graphicsProfile = value;
            }
        }

        /// <summary>
        /// Indicates the desire to switch into fullscreen mode.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set fullscreen mode during initialization.  If
        /// set after startup you must call ApplyChanges() for the fullscreen mode to be changed.
        /// Note that for some platforms that do not support windowed modes this property has no affect.
        /// </remarks>
        public bool IsFullScreen
        {
            get => _wantFullScreen;
            set
            {
                _shouldApplyChanges = true;
                _wantFullScreen = value;
            }
        }

        /// <summary>
        /// Gets or sets the boolean which defines how window switches from windowed to fullscreen state.
        /// "Hard" mode(true) is slow to switch, but more effecient for performance, while "soft" mode(false) is vice versa.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool HardwareModeSwitch
        {
            get => _hardwareModeSwitch;
            set
            {
                _shouldApplyChanges = true;
                _hardwareModeSwitch = value;
            }
        }

        /// <summary>
        /// Indicates the desire for a multisampled back buffer.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set the MSAA mode during initialization.  If
        /// set after startup you must call ApplyChanges() for the MSAA mode to be changed.
        /// </remarks>
        public bool PreferMultiSampling
        {
            get => _preferMultiSampling;
            set
            {
                _shouldApplyChanges = true;
                _preferMultiSampling = value;
            }
        }

        /// <summary>
        /// Indicates the desired back buffer color format.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set the format during initialization.  If
        /// set after startup you must call ApplyChanges() for the format to be changed.
        /// </remarks>
        public SurfaceFormat PreferredBackBufferFormat
        {
            get => _preferredBackBufferFormat;
            set
            {
                _shouldApplyChanges = true;
                _preferredBackBufferFormat = value;
            }
        }

        /// <summary>
        /// Indicates the desired back buffer height in pixels.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set the height during initialization.  If
        /// set after startup you must call ApplyChanges() for the height to be changed.
        /// </remarks>
        public int PreferredBackBufferHeight
        {
            get => _preferredBackBufferHeight;
            set
            {
                _shouldApplyChanges = true;
                _preferredBackBufferHeight = value;
            }
        }

        /// <summary>
        /// Indicates the desired back buffer width in pixels.
        /// </summary>
        /// <remarks>
        /// When called at startup this will automatically set the width during initialization.  If
        /// set after startup you must call ApplyChanges() for the width to be changed.
        /// </remarks>
        public int PreferredBackBufferWidth
        {
            get => _preferredBackBufferWidth;
            set
            {
                _shouldApplyChanges = true;
                _preferredBackBufferWidth = value;
            }
        }

        /// <summary>
        /// Indicates the desired depth-stencil buffer format.
        /// </summary>
        /// <remarks>
        /// The depth-stencil buffer format defines the scene depth precision and stencil bits available for effects during rendering.
        /// When called at startup this will automatically set the format during initialization.  If
        /// set after startup you must call ApplyChanges() for the format to be changed.
        /// </remarks>
        public DepthFormat PreferredDepthStencilFormat
        {
            get => _preferredDepthStencilFormat;
            set
            {
                _shouldApplyChanges = true;
                _preferredDepthStencilFormat = value;
            }
        }

        /// <summary>
        /// Indicates the desire for vsync when presenting the back buffer.
        /// </summary>
        /// <remarks>
        /// Vsync limits the frame rate of the game to the monitor referesh rate to prevent screen tearing.
        /// When called at startup this will automatically set the vsync mode during initialization.  If
        /// set after startup you must call ApplyChanges() for the vsync mode to be changed.
        /// </remarks>
        public bool SynchronizeWithVerticalRetrace
        {
            get => _synchronizedWithVerticalRetrace;
            set
            {
                _shouldApplyChanges = true;
                _synchronizedWithVerticalRetrace = value;
            }
        }
    }
}
