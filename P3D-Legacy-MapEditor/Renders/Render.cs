using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Components;
using P3D.Legacy.MapEditor.Data;
using P3D.Legacy.MapEditor.Effect;
using P3D.Legacy.MapEditor.World;

namespace P3D.Legacy.MapEditor.Renders
{
    public enum AntiAliasing
    {
        None,
        FXAA,
        SSAA,
        MSAA
    }

    public class Render : IGameComponent
    {
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
                FogEnabled = false,
                //AlphaFunction = CompareFunction.Equal,
                //ReferenceAlpha = 0
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

        public void Draw()
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
                    SpriteBatch.Draw(RenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f / (float)Scale, SpriteEffects.None, 0f);
                    SpriteBatch.End();
                    break;
            }
        }
    }
}
