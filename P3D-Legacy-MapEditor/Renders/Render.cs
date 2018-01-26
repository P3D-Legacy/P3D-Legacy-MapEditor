using System.Collections.Generic;
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

        private BasicEffect BasicEffect { get; set; }
        private AlphaTestEffect AlphaTestEffect { get; set; }
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
        }
        
        public void Initialize(GraphicsDevice graphicsDevice)
        {
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

            graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            Level.UpdateLighting(BasicEffect);
            Level.SetWeather(BasicEffect, Weather.Clear);
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

        public void Draw(GraphicsDevice graphicsDevice)
        {
            BasicEffect.View = Camera.ViewMatrix;
            BasicEffect.Projection = Camera.ProjectionMatrix;
            AlphaTestEffect.View = Camera.ViewMatrix;
            AlphaTestEffect.Projection = Camera.ProjectionMatrix;

            Level?.Draw(BasicEffect, AlphaTestEffect);

            Update();
            if (Min.Value != null)
            {
                _cube.Model = Min.Value;
                _cube.Recalc();
                _cube.Draw(BasicEffect, new Color(Color.LimeGreen, 0.75f));
            }
        }
    }
}
