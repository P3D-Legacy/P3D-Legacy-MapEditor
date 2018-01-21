using System;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RenderTest.BMFont;

namespace RenderTest
{
    public sealed class DebugComponent : DrawableGameComponent
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly FontRenderer _fontRenderer;


        private int _frameCounter;
        private readonly TimeSpan _time = TimeSpan.FromMilliseconds(100d);
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private int _frameRate;



        public DebugComponent(Game game) : base(game)
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _fontRenderer = new FontRenderer(GraphicsDevice, "PixelUnicode", 16, 32, 48);
        }

        public override void Update(GameTime gameTime)
        {
            if (_stopwatch.Elapsed > _time)
            {
                _frameRate = (int)((_frameCounter) / _stopwatch.Elapsed.TotalSeconds);
                _stopwatch.Reset();
                _stopwatch.Start();
                _frameCounter = 0;
            }
        }

        private const int Height = 54;
        public override void Draw(GameTime gameTime)
        {
            var width = (int)(Game.GraphicsDevice.Viewport.Width * 0.5f);

            _frameCounter++;

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            var fps = $"FPS : {_frameRate}";
            var ram = $"RAM : {GC.GetTotalMemory(false) / 1024} (KB)";

            _fontRenderer.DrawText(_spriteBatch, fps, new Rectangle(6, 1, width, Height), Color.Black);
            _fontRenderer.DrawText(_spriteBatch, fps, new Rectangle(5, 0, width, Height), Color.White);

            _fontRenderer.DrawText(_spriteBatch, ram, new Rectangle(width + 1, 1, width, Height), Color.Black);
            _fontRenderer.DrawText(_spriteBatch, ram, new Rectangle(width, 0, width, Height), Color.White);

            _spriteBatch.End();
        }
    }
}