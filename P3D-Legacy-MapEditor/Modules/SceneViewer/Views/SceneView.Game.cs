using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Modules.SceneViewer.Views
{
    public partial class SceneView
    {
        private readonly SortingFilteringCollection<IDrawable> _drawables =
            new SortingFilteringCollection<IDrawable>(
                d => d.Visible,
                (d, handler) => d.VisibleChanged += handler,
                (d, handler) => d.VisibleChanged -= handler,
                (d1, d2) => Comparer<int>.Default.Compare(d1.DrawOrder, d2.DrawOrder),
                (d, handler) => d.DrawOrderChanged += handler,
                (d, handler) => d.DrawOrderChanged -= handler);

        private readonly SortingFilteringCollection<IUpdateable> _updateables =
            new SortingFilteringCollection<IUpdateable>(
                u => u.Enabled,
                (u, handler) => u.EnabledChanged += handler,
                (u, handler) => u.EnabledChanged -= handler,
                (u1, u2) => Comparer<int>.Default.Compare(u1.UpdateOrder, u2.UpdateOrder),
                (u, handler) => u.UpdateOrderChanged += handler,
                (u, handler) => u.UpdateOrderChanged -= handler);


        private bool _initialized;

        private TimeSpan _targetElapsedTime = TimeSpan.FromTicks(166667); // 60fps

        private TimeSpan _maxElapsedTime = TimeSpan.FromMilliseconds(500);

        private bool _suppressDraw;

        ~SceneView()
        {
            Dispose(false);
        }

        #region IDisposable Implementation

        private bool _isDisposed;
        public void GameDispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // Dispose loaded game components
                    for (var i = 0; i < Components.Count; i++)
                    {
                        if (Components[i] is IDisposable disposable)
                            disposable.Dispose();
                    }
                    Components = null;
                }
                _isDisposed = true;
            }
        }

        [DebuggerNonUserCode]
        private void AssertNotDisposed()
        {
            if (_isDisposed)
            {
                var name = GetType().Name;
                throw new ObjectDisposedException(
                    name, $"The {name} object was used after being Disposed.");
            }
        }

        #endregion IDisposable Implementation

        #region Properties

        public GameComponentCollection Components { get; private set; } = new GameComponentCollection();

        /// <summary>
        /// The maximum amount of time we will frameskip over and only perform Update calls with no Draw calls.
        /// MonoGame extension.
        /// </summary>
        public TimeSpan MaxElapsedTime
        {
            get => _maxElapsedTime;
            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("The time must be positive.", default(Exception));
                if (value < _targetElapsedTime)
                    throw new ArgumentOutOfRangeException("The time must be at least TargetElapsedTime", default(Exception));

                _maxElapsedTime = value;
            }
        }

        public TimeSpan TargetElapsedTime
        {
            get => _targetElapsedTime;
            set
            {
                if (value <= TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("The time must be positive and non-zero.", default(Exception));

                if (value != _targetElapsedTime)
                    _targetElapsedTime = value;
            }
        }

        public bool IsFixedTimeStep { get; set; } = true;

        public GraphicsDevice GraphicsDevice { get; private set; }

        #endregion Properties

        #region Public Methods

        public void ResetElapsedTime()
        {
            _gameTimer.Reset();
            _gameTimer.Start();
            _accumulatedElapsedTime = TimeSpan.Zero;
            _gameTime.ElapsedGameTime = TimeSpan.Zero;
            _previousTicks = 0L;
        }

        public void SuppressDraw() => _suppressDraw = true;

        public void Run()
        {
            if (!_initialized)
            {
                DoInitialize();
                _gameTimer = Stopwatch.StartNew();
                _initialized = true;
            }
        }

        private TimeSpan _accumulatedElapsedTime;
        private readonly GameTime _gameTime = new GameTime();
        private Stopwatch _gameTimer;
        private long _previousTicks;
        private int _updateFrameLag;

        public void Tick()
        {
            // NOTE: This code is very sensitive and can break very badly
            // with even what looks like a safe change.  Be sure to test 
            // any change fully in both the fixed and variable timestep 
            // modes across multiple devices and platforms.

            RetryTick:

            // Advance the accumulated elapsed time.
            var currentTicks = _gameTimer.Elapsed.Ticks;
            _accumulatedElapsedTime += TimeSpan.FromTicks(currentTicks - _previousTicks);
            _previousTicks = currentTicks;

            // If we're in the fixed timestep mode and not enough time has elapsed
            // to perform an update we sleep off the the remaining time to save battery
            // life and/or release CPU time to other threads and processes.
            if (IsFixedTimeStep && _accumulatedElapsedTime < TargetElapsedTime)
            {
                var sleepTime = (int) (TargetElapsedTime - _accumulatedElapsedTime).TotalMilliseconds;

                // NOTE: While sleep can be inaccurate in general it is 
                // accurate enough for frame limiting purposes if some
                // fluctuation is an acceptable result.
                Thread.Sleep(sleepTime);

                goto RetryTick;
            }

            // Do not allow any update to take longer than our maximum.
            if (_accumulatedElapsedTime > _maxElapsedTime)
                _accumulatedElapsedTime = _maxElapsedTime;

            if (IsFixedTimeStep)
            {
                _gameTime.ElapsedGameTime = TargetElapsedTime;
                var stepCount = 0;

                // Perform as many full fixed length time steps as we can.
                while (_accumulatedElapsedTime >= TargetElapsedTime)
                {
                    _gameTime.TotalGameTime += TargetElapsedTime;
                    _accumulatedElapsedTime -= TargetElapsedTime;
                    ++stepCount;

                    DoUpdate(_gameTime);
                }

                //Every update after the first accumulates lag
                _updateFrameLag += Math.Max(0, stepCount - 1);

                //If we think we are running slowly, wait until the lag clears before resetting it
                if (_gameTime.IsRunningSlowly)
                {
                    if (_updateFrameLag == 0)
                        _gameTime.IsRunningSlowly = false;
                }
                else if (_updateFrameLag >= 5)
                {
                    //If we lag more than 5 frames, start thinking we are running slowly
                    _gameTime.IsRunningSlowly = true;
                }

                //Every time we just do one update and one draw, then we are not running slowly, so decrease the lag
                if (stepCount == 1 && _updateFrameLag > 0)
                    _updateFrameLag--;

                // Draw needs to know the total elapsed time
                // that occured for the fixed length updates.
                _gameTime.ElapsedGameTime = TimeSpan.FromTicks(TargetElapsedTime.Ticks * stepCount);
            }
            else
            {
                // Perform a single variable length update.
                _gameTime.ElapsedGameTime = _accumulatedElapsedTime;
                _gameTime.TotalGameTime += _accumulatedElapsedTime;
                _accumulatedElapsedTime = TimeSpan.Zero;

                DoUpdate(_gameTime);
            }

            // Draw unless the update suppressed it.
            if (_suppressDraw)
                _suppressDraw = false;
            else
                DoDraw(_gameTime);
        }

        #endregion

        #region Protected Methods

        protected virtual void Initialize()
        {
            // According to the information given on MSDN (see link below), all
            // GameComponents in Components at the time Initialize() is called
            // are initialized.
            // http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.game.initialize.aspx
            // Initialize all existing components
            InitializeExistingComponents();
        }

        private static readonly Action<IDrawable, GameTime> DrawAction =
            (drawable, gameTime) => drawable.Draw(gameTime);

        protected virtual void Draw(GameTime gameTime)
        {
            SceneViewDraw(gameTime);
            _drawables.ForEachFilteredItem(DrawAction, gameTime);
        }

        private static readonly Action<IUpdateable, GameTime> UpdateAction =
            (updateable, gameTime) => updateable.Update(gameTime);

        protected virtual void Update(GameTime gameTime)
        {
            _updateables.ForEachFilteredItem(UpdateAction, gameTime);
            SceneViewUpdate(gameTime);
        }

        #endregion Protected Methods

        #region Event Handlers

        private void Components_ComponentAdded(object sender, GameComponentCollectionEventArgs e)
        {
            // Since we only subscribe to ComponentAdded after the graphics
            // devices are set up, it is safe to just blindly call Initialize.
            e.GameComponent.Initialize();
            CategorizeComponent(e.GameComponent);
        }

        private void Components_ComponentRemoved(object sender, GameComponentCollectionEventArgs e)
        {
            DecategorizeComponent(e.GameComponent);
        }

        #endregion Event Handlers

        #region Internal Methods

        internal void DoUpdate(GameTime gameTime)
        {
            Update(gameTime);
        }

        internal void DoDraw(GameTime gameTime)
        {
            Draw(gameTime);
        }

        internal void DoInitialize()
        {
            AssertNotDisposed();

            Initialize();

            // We need to do this after virtual Initialize(...) is called.
            // 1. Categorize components into IUpdateable and IDrawable lists.
            // 2. Subscribe to Added/Removed events to keep the categorized
            //    lists synced and to Initialize future components as they are
            //    added.            
            CategorizeComponents();
            Components.ComponentAdded += Components_ComponentAdded;
            Components.ComponentRemoved += Components_ComponentRemoved;
        }

        #endregion Internal Methods

        // NOTE: InitializeExistingComponents really should only be called once.
        //       Game.Initialize is the only method in a position to guarantee
        //       that no component will get a duplicate Initialize call.
        //       Further calls to Initialize occur immediately in response to
        //       Components.ComponentAdded.
        private void InitializeExistingComponents()
        {
            for (var i = 0; i < Components.Count; ++i)
                Components[i].Initialize();
        }

        private void CategorizeComponents()
        {
            DecategorizeComponents();
            for (var i = 0; i < Components.Count; ++i)
                CategorizeComponent(Components[i]);
        }

        // FIXME: I am open to a better name for this method.  It does the
        //        opposite of CategorizeComponents.
        private void DecategorizeComponents()
        {
            _updateables.Clear();
            _drawables.Clear();
        }

        private void CategorizeComponent(IGameComponent component)
        {
            if (component is IUpdateable updateable)
                _updateables.Add(updateable);
            if (component is IDrawable drawable)
                _drawables.Add(drawable);
        }

        // FIXME: I am open to a better name for this method.  It does the
        //        opposite of CategorizeComponent.
        private void DecategorizeComponent(IGameComponent component)
        {
            if (component is IUpdateable updateable)
                _updateables.Remove(updateable);
            if (component is IDrawable drawable)
                _drawables.Remove(drawable);
        }
    }
}
