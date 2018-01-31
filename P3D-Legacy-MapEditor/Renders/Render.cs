using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Components;
using P3D.Legacy.MapEditor.Data;
using P3D.Legacy.MapEditor.Effect;
using P3D.Legacy.MapEditor.World;

namespace P3D.Legacy.MapEditor.Renders
{
    public class Render : IGameComponent
    {
        private GraphicsDevice GraphicsDevice { get; }
        private SpriteBatch SpriteBatch { get; }
        private RenderTarget2D RenderTarget { get; set; }


        private BasicEffect BasicEffect { get; set; }
        private AlphaTestEffect AlphaTestEffect { get; set; }
        private FxaaEffect FxaaEffect { get; set; }
        private bool FxaaEnabled { get; set; } = true;
        private const float FxaaQualitySubpix = 0.75f;
        private const float FxaaQualityEdgeThreshold = 0.166f;
        private const float FxaaQualityEdgeThresholdMin = 0.0833f;

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
            RenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            /*
            FxaaEffect = new FxaaEffect(GraphicsDevice)
            {
                SubpixelQuality = FxaaQualitySubpix,
                EdgeThreshold = FxaaQualityEdgeThreshold,
                EdgeThresholdMin = FxaaQualityEdgeThresholdMin,

                InverseDimensions = new Vector2(1f / GraphicsDevice.Viewport.Width, 1f / GraphicsDevice.Viewport.Height),
                //InvViewportWidth = 1f / GraphicsDevice.Viewport.Width,
                //InvViewportHeight = 1f / GraphicsDevice.Viewport.Height,

                RenderTarget = RenderTarget
            };
            */
            FxaaEffect = new FxaaEffect(GraphicsDevice)
            {
                InverseDimensions = new Vector2(1f / GraphicsDevice.Viewport.Width, 1f / GraphicsDevice.Viewport.Height),
                RenderTarget = RenderTarget
            };
            FxaaEffect.SetHightQuality();
            //FxaaEffect.CurrentTechnique = FxaaEffect.PCTechnique;

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
            RenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            FxaaEffect.InverseDimensions = new Vector2(1f / GraphicsDevice.Viewport.Width, 1f / GraphicsDevice.Viewport.Height);
            FxaaEffect.RenderTarget = RenderTarget;
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
            
            if (FxaaEnabled)
            {
                SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, FxaaEffect);
                SpriteBatch.Draw(RenderTarget, Vector2.Zero, Color.White);
                SpriteBatch.End();
            }
            else
            {
                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp);
                SpriteBatch.Draw(RenderTarget, Vector2.Zero, Color.White);
                SpriteBatch.End();
            }
        }
    }
}
