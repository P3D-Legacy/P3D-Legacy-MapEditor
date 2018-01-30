using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Data;
using P3D.Legacy.MapEditor.Data.Models;
using P3D.Legacy.MapEditor.Data.World;
using P3D.Legacy.MapEditor.Primitives;
using P3D.Legacy.MapEditor.Utils;
using P3D.Legacy.MapEditor.World;

namespace P3D.Legacy.MapEditor.Renders
{
    public class Render
    {
        private GraphicsDevice GraphicsDevice { get; }
        private SpriteBatch SpriteBatch { get; set; }
        private RenderTarget2D RenderTarget { get; set; }


        private BasicEffect BasicEffect { get; set; }
        private AlphaTestEffect AlphaTestEffect { get; set; }
        private Effect FXAAEffect { get; set; }
        private bool FXAAEnabled { get; set; } = false;
        private float FXAAQualitySubpix = 0.75f;
        private float FXAAQualityEdgeThreshold = 0.166f;
        private float FXAAQualityEdgeThresholdMin = 0.0833f;

        private Level Level { get; set; }

        private BaseCamera Camera { get; }

        private readonly CubePrimitive _cube;

        public Render(GraphicsDevice graphicsDevice, BaseCamera camera, LevelInfo levelInfo)
        {
            GraphicsDevice = graphicsDevice;
            Camera = camera;

            if (levelInfo != null)
                Level = new Level(levelInfo, graphicsDevice);

            _cube = new CubePrimitive();

            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }
        
        public void Initialize()
        {       
            RenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            FXAAEffect = new Effect(GraphicsDevice, File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "FXAA3.11.mgfx")));
            FXAAEffect.CurrentTechnique = FXAAEffect.Techniques["ppfxaa_PC"];
            FXAAEffect.Parameters["fxaaQualitySubpix"].SetValue(FXAAQualitySubpix);
            FXAAEffect.Parameters["fxaaQualityEdgeThreshold"].SetValue(FXAAQualityEdgeThreshold);
            FXAAEffect.Parameters["fxaaQualityEdgeThresholdMin"].SetValue(FXAAQualityEdgeThresholdMin);

            FXAAEffect.Parameters["invViewportWidth"].SetValue(1f / RenderTarget.Width);
            FXAAEffect.Parameters["invViewportHeight"].SetValue(1f / RenderTarget.Height);
            FXAAEffect.Parameters["texScreen"].SetValue(RenderTarget);


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

            //graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            Level.UpdateLighting(BasicEffect);
            Level.SetWeather(BasicEffect, Weather.Clear);
        }

        public void ViewportChanged()
        {
            RenderTarget?.Dispose();
            RenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        }

        KeyValuePair<float, BaseModel> Min;
        public void Update()
        {
            Min = new KeyValuePair<float, BaseModel>(float.MaxValue, null);
            foreach (var baseModel in BaseModelListRenderer.TotalModels.Where(m => m.Entity.Visible))
            {
                var value = baseModel.BoundingBox.Intersects(Camera.GetMouseRay());
                if (value.HasValue && value.Value < Min.Key)
                    Min = new KeyValuePair<float, BaseModel>(value.Value, baseModel);
            } 
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
            GraphicsDevice.Clear(ClearOptions.Stencil, Color.Transparent, 0, 0);
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Level?.Draw(BasicEffect, AlphaTestEffect);
            //Update();
            if (Min.Value != null)
            {
                _cube.Model = Min.Value;
                _cube.Recalc();
                _cube.Draw(BasicEffect, new Color(Color.LimeGreen, 0.75f));
            }
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.SetRenderTargets(prevRenderTargets);
            
            if (FXAAEnabled)
            {
                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                    DepthStencilState.Default, RasterizerState.CullNone, FXAAEffect);
                SpriteBatch.Draw(RenderTarget, new Rectangle(0, 0, RenderTarget.Width, RenderTarget.Height), Color.White);
                SpriteBatch.End();
            }
            else
            {
                SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                SpriteBatch.Draw(RenderTarget, Vector2.Zero, Color.White);
                SpriteBatch.End();
            }
        }
    }
}
