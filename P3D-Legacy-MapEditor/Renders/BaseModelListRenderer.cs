using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Data.Models;
using P3D.Legacy.MapEditor.Data.Vertices;
using P3D.Legacy.MapEditor.Utils;
using P3D.Legacy.MapEditor.World;

namespace P3D.Legacy.MapEditor.Renders
{
    public abstract class BaseModelListRenderer
    {
        public static List<BaseModel> TotalModels { get; } = new List<BaseModel>();

        protected List<BaseModel> Models { get; } = new List<BaseModel>();

        public virtual void AddModel(BaseModel model)
        {
            Models.Add(model);
            TotalModels.Add(model);
        }
        public virtual void AddModels(List<BaseModel> models)
        {
            Models.AddRange(models);
            TotalModels.AddRange(models);
        }

        public abstract void Setup(GraphicsDevice graphicsDevice);
        public abstract void Draw(Level level, BasicEffect basicEffect, AlphaTestEffect alphaTestEffect);

        private class TrianglesWithCroppedTexture
        {
            public List<BaseModel.ModelTriangle> ModelTriangles { get; } = new List<BaseModel.ModelTriangle>();

            public Texture2D CroppedTexture; // Cropped texture from the original texture
            public bool HasTransparentPixels; // Cropped texture has transparent pixels
            public Rectangle AtlasRectangle;
        }
        public Texture2D SetupTextureAtlas(GraphicsDevice graphicsDevice)
        {
            var triangles = Models.SelectMany(m => m.ModelTriangles).ToList();
            var dict = new Dictionary<KeyValuePair<string, Rectangle>, TrianglesWithCroppedTexture>();
            foreach (var triangle in triangles)
            {
                if (!dict.ContainsKey(triangle.TextureKey))
                {
                    if (!TextureHandler.HasCroppedTexture(triangle.TextureKey))
                        TextureHandler.AddCroppedTexture(triangle.TextureKey, TextureHandler.CropTexture(triangle.OriginalTexture, triangle.OriginalTextureRectangle));

                    dict.Add(triangle.TextureKey, new TrianglesWithCroppedTexture()
                    {
                        CroppedTexture = TextureHandler.GetCroppedTexture(triangle.TextureKey).Key,
                        HasTransparentPixels = TextureHandler.GetCroppedTexture(triangle.TextureKey).Value
                    });
                }

                dict[triangle.TextureKey].ModelTriangles.Add(triangle);
            }

            var packer = new RectanglePacker(4096, 4096);
            foreach (var pair in dict)
            {
                var rect = pair.Value.CroppedTexture.Bounds;
                if (!packer.Pack(rect.Width, rect.Height, out rect.X, out rect.Y))
                    throw new Exception("Uh oh, we couldn't pack the rectangles");
                pair.Value.AtlasRectangle = rect;
            }

            foreach (var pair in dict)
                foreach (var triangle in pair.Value.ModelTriangles)
                {
                    triangle.AtlasTextureRectangle = pair.Value.AtlasRectangle;
                    triangle.HasTransparentPixels = pair.Value.HasTransparentPixels;
                }

            var atlas = new Texture2D(graphicsDevice, 4096, 4096);
            foreach (var pair in dict)
            {
                var data = new Color[pair.Value.CroppedTexture.Width * pair.Value.CroppedTexture.Height];
                pair.Value.CroppedTexture.GetData(0, pair.Value.CroppedTexture.Bounds, data, 0, data.Length);
                atlas.SetData(0, pair.Value.AtlasRectangle, data, 0, data.Length);
                pair.Value.CroppedTexture = null;
            }

            return atlas;
        }
    }

    public class StaticModelListRenderer : BaseModelListRenderer
    {
        private OpaqueVertexRenderer StaticOpaqueRenderer { get; set; }
        private TransparentVertexRenderer StaticTransparentRenderer { get; set; }

        public override void Setup(GraphicsDevice graphicsDevice)
        {
            var atlas = SetupTextureAtlas(graphicsDevice);

            var opaqueVertices = new List<VertexPositionNormalColorTexture>();
            var transparentVertices = new List<VertexPositionNormalColorTexture>();
            foreach (var staticModel in Models)
            {
                foreach (var triangle in staticModel.ModelTriangles)
                {
                    var rec = triangle.AtlasTextureRectangle;
                    var maxT = new Vector2(atlas.Width, atlas.Height);

                    var vertices = triangle.OriginalVertices;
                    var indices = triangle.OriginalIndices;
                    for (int i = 0; i < 3; i++)
                    {
                        var vertex = vertices[i];
                        var index = indices[i];

                        var position = Vector3.Transform(vertex.Position, staticModel.WorldMatrix);
                        var normal = Vector3.TransformNormal(vertex.Normal, staticModel.WorldMatrix);
                        var color = staticModel.Entity.Shader;
                        color.A *= (byte) staticModel.Entity.Opacity;
                        var textCoord = new Vector2(
                            vertex.TextureCoordinate.X == 0
                                ? rec.X / maxT.X
                                : (rec.X + vertex.TextureCoordinate.X * rec.Width) / maxT.X,
                            vertex.TextureCoordinate.Y == 0
                                ? rec.Y / maxT.Y
                                : (rec.Y + vertex.TextureCoordinate.Y * rec.Height) / maxT.Y);

                        var vertexNew = new VertexPositionNormalColorTexture(position, normal, color, textCoord);

                        if (staticModel.HasTransparentPixels)
                            transparentVertices.Add(vertexNew);
                        else
                            opaqueVertices.Add(vertexNew);
                    }
                }
            }

            if (opaqueVertices.Any())
            {
                StaticOpaqueRenderer = new OpaqueVertexRenderer(opaqueVertices, new List<int>(), atlas);
                StaticOpaqueRenderer.Setup(graphicsDevice);
            }

            if (transparentVertices.Any())
            {
                StaticTransparentRenderer = new TransparentVertexRenderer(transparentVertices, new List<int>(), atlas);
                StaticTransparentRenderer.Setup(graphicsDevice);
            }
        }

        public override void Draw(Level level, BasicEffect basicEffect, AlphaTestEffect alphaTestEffect)
        {
            var basicEffectDiffuseColor = basicEffect.DiffuseColor;

            if (level.IsDark)
                basicEffect.DiffuseColor *= new Vector3(0.5f, 0.5f, 0.5f);

            StaticOpaqueRenderer?.Draw(level, basicEffect, CullMode.CullClockwiseFace);
            StaticOpaqueRenderer?.Draw(level, basicEffect, CullMode.CullCounterClockwiseFace);
            StaticTransparentRenderer?.Draw(level, basicEffect, alphaTestEffect, CullMode.CullClockwiseFace);
            StaticTransparentRenderer?.Draw(level, basicEffect, alphaTestEffect, CullMode.CullCounterClockwiseFace);

            /*
            // render solid part (CW+CCW)
            // render back-side transparent part(CCW)
            // render solid part(CW + CCW)
            // render front-side transparent part(CW)
            StaticOpaqueRenderer.Draw(level, basicEffect, CullMode.CullClockwiseFace);
            StaticOpaqueRenderer.Draw(level, basicEffect, CullMode.CullCounterClockwiseFace);
            StaticTransparentRenderer.Draw(level, basicEffect, alphaTestEffect, CullMode.CullCounterClockwiseFace);
            StaticOpaqueRenderer.Draw(level, basicEffect, CullMode.CullClockwiseFace);
            StaticOpaqueRenderer.Draw(level, basicEffect, CullMode.CullCounterClockwiseFace);
            StaticTransparentRenderer.Draw(level, basicEffect, alphaTestEffect, CullMode.CullClockwiseFace);
            */

            basicEffect.DiffuseColor = basicEffectDiffuseColor;
        }
    }

    public class DynamicModelListRenderer : BaseModelListRenderer
    {
        private List<OpaqueVertexRenderer> DynamicOpaqueRenderers { get; } = new List<OpaqueVertexRenderer>();
        private List<TransparentVertexRenderer> DynamicTransparentRenderers { get; } = new List<TransparentVertexRenderer>();

        public override void Setup(GraphicsDevice graphicsDevice)
        {

        }

        public override void Draw(Level level, BasicEffect basicEffect, AlphaTestEffect alphaTestEffect)
        {
            foreach (var opaqueRenderer in DynamicOpaqueRenderers)
                opaqueRenderer.Draw(level, basicEffect, CullMode.CullClockwiseFace);
            foreach (var opaqueRenderer in DynamicOpaqueRenderers)
                opaqueRenderer.Draw(level, basicEffect, CullMode.CullCounterClockwiseFace);

            foreach (var transparentRenderer in DynamicTransparentRenderers)
                transparentRenderer.Draw(level, basicEffect, alphaTestEffect, CullMode.CullClockwiseFace);
            foreach (var transparentRenderer in DynamicTransparentRenderers)
                transparentRenderer.Draw(level, basicEffect, alphaTestEffect, CullMode.CullCounterClockwiseFace);
        }
    }
}