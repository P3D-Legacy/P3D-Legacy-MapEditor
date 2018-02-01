using System;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.BMFont;

namespace P3D.Legacy.MapEditor.Components
{
    public sealed class DebugComponent : IGameComponent, IUpdateable, IDrawable
    {
        private bool _enabled = true;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnEnabledChanged(this, EventArgs.Empty);
                }
            }
        }

        private int _updateOrder;
        public int UpdateOrder
        {
            get => _updateOrder;
            set
            {
                if (_updateOrder != value)
                {
                    _updateOrder = value;
                    OnUpdateOrderChanged(this, EventArgs.Empty);
                }
            }
        }

        private int _drawOrder;
        public int DrawOrder
        {
            get => _drawOrder;
            set
            {
                if (_drawOrder != value)
                {
                    _drawOrder = value;
                    OnDrawOrderChanged(this, EventArgs.Empty);
                }
            }
        }

        private bool _visible = true;
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    OnVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;


        private readonly GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private FontRenderer _fontRenderer;


        private int _frameCounter;
        private readonly TimeSpan _time = TimeSpan.FromMilliseconds(100d);
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private int _frameRate;

        public DebugComponent(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void Initialize()
        {
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _fontRenderer = new FontRenderer(_graphicsDevice, "PixelUnicode", 16, 32, 48);
        }

        public void Update(GameTime gameTime)
        {
            if (_stopwatch.Elapsed > _time)
            {
                _frameRate = (int)(_frameCounter / _stopwatch.Elapsed.TotalSeconds);
                _stopwatch.Reset();
                _stopwatch.Start();
                _frameCounter = 0;
            }
        }

        private const int Height = 54;
        public void Draw(GameTime gameTime)
        {
            var width = (int)(_graphicsDevice.Viewport.Width * 0.5f);

            _frameCounter++;

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            var fps = $"FPS : {_frameRate}";
            var ram = $"RAM : {GC.GetTotalMemory(false) / 1024} (KB)";
            //var fxa = $"FXAA : {Game.useFXAA}";

            _fontRenderer.DrawText(_spriteBatch, fps, new Rectangle(6, 1, width, Height), Color.Black);
            _fontRenderer.DrawText(_spriteBatch, fps, new Rectangle(5, 0, width, Height), Color.White);

            //_fontRenderer.DrawText(_spriteBatch, ram, new Rectangle(width + 1, 1, width, Height), Color.Black);
            //_fontRenderer.DrawText(_spriteBatch, ram, new Rectangle(width, 0, width, Height), Color.White);

            _fontRenderer.DrawText(_spriteBatch, ram, new Rectangle(6, 41, width, Height), Color.Black);
            _fontRenderer.DrawText(_spriteBatch, ram, new Rectangle(5, 40, width, Height), Color.White);

            //_fontRenderer.DrawText(_spriteBatch, fxa, new Rectangle(6, 81, width, Height), Color.Black);
            //_fontRenderer.DrawText(_spriteBatch, fxa, new Rectangle(5, 80, width, Height), Color.White);

            _spriteBatch.End();
        }

        private void OnUpdateOrderChanged(object sender, EventArgs args)
            => UpdateOrderChanged?.Invoke(sender, args);

        private void OnEnabledChanged(object sender, EventArgs args)
            => EnabledChanged?.Invoke(sender, args);

        private void OnVisibleChanged(object sender, EventArgs args)
            => VisibleChanged?.Invoke(this, args);

        private void OnDrawOrderChanged(object sender, EventArgs args)
            => DrawOrderChanged?.Invoke(this, args);
    }
}