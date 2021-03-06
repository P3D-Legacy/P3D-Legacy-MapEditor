﻿using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Components.Camera;
using P3D.Legacy.MapEditor.Components.ModelSelector;
using P3D.Legacy.MapEditor.Data;
using P3D.Legacy.MapEditor.Effect;
using P3D.Legacy.MapEditor.World;

namespace P3D.Legacy.MapEditor.Components.Render
{
    public enum AntiAliasing
    {
        None,
        FXAA,
        SSAA,
        MSAA
    }

    public class Render : IGameComponent, IDrawable
    {
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


        public int DrawCalls => StaticDrawCalls;
        internal static int StaticDrawCalls;

        private GraphicsDevice GraphicsDevice { get; }
        private SpriteBatch SpriteBatch { get; }
        private RenderTarget2D RenderTarget { get; set; }


        private BasicEffect BasicEffect { get; set; }
        private AlphaTestEffect AlphaTestEffect { get; set; }

        private AntiAliasing AntiAliasing { get; set; } = AntiAliasing.MSAA;
        private FxaaEffect FxaaEffect { get; set; }
        private int Scale { get; set; } = 1;

        private Level Level { get; set; }

        private BaseCamera Camera { get; }
        private BaseModelSelector ModelSelector { get; }



        public Render(GraphicsDevice graphicsDevice, BaseCamera camera, BaseModelSelector modelSelector, LevelInfo levelInfo)
        {
            GraphicsDevice = graphicsDevice;
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Camera = camera;
            ModelSelector = modelSelector;

            if (levelInfo != null)
                Level = new Level(levelInfo, graphicsDevice);  
        }
        
        public void Initialize()
        {
            FxaaEffect = new FxaaEffect(GraphicsDevice);
            FxaaEffect.SetDefaultQualityParameters();

            ViewportChanged();

            BasicEffect = new BasicEffect(GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
                FogEnabled = false
            };

            AlphaTestEffect = new AlphaTestEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
                FogEnabled = false
            };

            //Level.UpdateLighting(BasicEffect);
            //Level.SetWeather(BasicEffect, Weather.Clear);
        }

        public void ViewportChanged()
        {
            RenderTarget?.Dispose();
            RenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width * Scale, GraphicsDevice.Viewport.Height * Scale,
                false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            FxaaEffect.InverseDimensions = new Vector2(1f / GraphicsDevice.Viewport.Width * Scale, 1f / GraphicsDevice.Viewport.Height * Scale);
            FxaaEffect.RenderTarget = RenderTarget;
        }

        public void SetAntiAliasing(AntiAliasing antiAliasing)
        {
            AntiAliasing = antiAliasing;

            Scale = 1;

            switch (AntiAliasing)
            {
                case AntiAliasing.None:
                    break;

                case AntiAliasing.FXAA:
                    break;

                case AntiAliasing.SSAA:
                    Scale = 2;
                    break;

                case AntiAliasing.MSAA:
                    break;
            }
            ViewportChanged();
        }

        public void Draw(GameTime gameTime)
        {
            BasicEffect.View = Camera.ViewMatrix;
            BasicEffect.Projection = Camera.ProjectionMatrix;
            AlphaTestEffect.View = Camera.ViewMatrix;
            AlphaTestEffect.Projection = Camera.ProjectionMatrix;


            var prevRenderTargets = GraphicsDevice.GetRenderTargets();
            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Level?.Draw(BasicEffect, AlphaTestEffect);
            ModelSelector?.Draw(BasicEffect);
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.SetRenderTargets(prevRenderTargets);

            switch (AntiAliasing)
            {
                case AntiAliasing.None:
                case AntiAliasing.MSAA:
                    SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
                    SpriteBatch.Draw(RenderTarget, Vector2.Zero, Color.White);
                    SpriteBatch.End();
                    break;
                case AntiAliasing.FXAA:
                    SpriteBatch.Begin(SpriteSortMode.Immediate, effect: FxaaEffect);
                    SpriteBatch.Draw(RenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f / Scale, SpriteEffects.None, 0f);
                    SpriteBatch.End();
                    break;

                case AntiAliasing.SSAA:
                    SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                    SpriteBatch.Draw(RenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f / Scale, SpriteEffects.None, 0f);
                    SpriteBatch.End();
                    break;
            }
        }

        protected virtual void OnVisibleChanged(object sender, EventArgs args) 
            => VisibleChanged?.Invoke(this, args);

        protected virtual void OnDrawOrderChanged(object sender, EventArgs args)
            => DrawOrderChanged?.Invoke(this, args);
    }
}
