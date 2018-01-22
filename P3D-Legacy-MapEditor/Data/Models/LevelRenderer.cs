using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Utils;
using P3D.Legacy.MapEditor.World;

namespace P3D.Legacy.MapEditor.Data.Models
{
    public class LevelRenderer : IDisposable
    {
        protected StaticModelListRenderer StaticRenderer;
        protected DynamicModelListRenderer DynamicRenderer;

        public void HandleModels(List<BaseModel> models)
        {
            StaticRenderer = new StaticModelListRenderer();
            StaticRenderer.Models.AddRange(models);
        }

        public void Setup(GraphicsDevice graphicsDevice)
        {
            StaticRenderer.Setup(graphicsDevice);
        }

        public void Draw(Level level, BasicEffect basicEffect, AlphaTestEffect alphaTestEffect)
        {
            StaticRenderer.Draw(level, basicEffect, alphaTestEffect);
        }

        public void Dispose()
        {
            TextureHandler.Dispose();
        }
    }
}