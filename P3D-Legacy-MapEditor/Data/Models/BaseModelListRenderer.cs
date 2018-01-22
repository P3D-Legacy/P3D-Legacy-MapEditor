using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Data.Vertices;
using P3D.Legacy.MapEditor.Utils;
using P3D.Legacy.MapEditor.World;

namespace P3D.Legacy.MapEditor.Data.Models
{
    public abstract class BaseModelListRenderer
    {
        public List<BaseModel> Models { get; } = new List<BaseModel>();


        private class TextureInfo
        {
            public List<BaseModel.ModelTriangle> ModelTriangles { get; } = new List<BaseModel.ModelTriangle>();

            public Texture2D CroppedTexture; // Cropped texture from the original texture
            public bool HasTransparentPixels; // Cropped texture has transparent pixels
            public Rectangle AtlasRectangle;
        }
        public Texture2D SetupTextureAtlas(GraphicsDevice graphicsDevice)
        {
            var triangles = Models.SelectMany(m => m.ModelTriangles).ToList();
            var dict = new Dictionary<KeyValuePair<string, Rectangle>, TextureInfo>();
            foreach (var triangle in triangles)
            {
                if (!dict.ContainsKey(triangle.TextureKey))
                {
                    if (!TextureHandler.HasCroppedTexture(triangle.TextureKey))
                        TextureHandler.AddCroppedTexture(triangle.TextureKey, TextureHandler.CropTexture(triangle.OriginalTexture, triangle.OriginalTextureRectangle));

                    dict.Add(triangle.TextureKey, new TextureInfo()
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

        public void Setup(GraphicsDevice graphicsDevice)
        {
            var atlas = SetupTextureAtlas(graphicsDevice);
            //var stream = File.OpenWrite("C://GitHub//Test.png");
            //atlas.SaveAsPng(stream, atlas.Width, atlas.Height);
            //stream.Dispose();
            StaticOpaqueRenderer = new OpaqueVertexRenderer() { Atlas = atlas };
            StaticTransparentRenderer = new TransparentVertexRenderer() { Atlas = atlas };

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
                        //var index = indices[i];

                        var position = Vector3.Transform(vertex.Position, staticModel.WorldMatrix);
                        var normal = Vector3.Transform(vertex.Normal, staticModel.RotationMatrix);
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
                            StaticTransparentRenderer.Vertices.Add(vertexNew);
                        else
                            StaticOpaqueRenderer.Vertices.Add(vertexNew);
                    }
                }
            }

            StaticOpaqueRenderer.Setup(graphicsDevice);
            StaticTransparentRenderer.Setup(graphicsDevice);
        }

        public void Draw(Level level, BasicEffect basicEffect, AlphaTestEffect alphaTestEffect)
        {
            var basicEffectDiffuseColor = basicEffect.DiffuseColor;
            var alphaTestEffectDiffuseColor = alphaTestEffect.DiffuseColor;

            // Alpha should be moved to shader
            basicEffect.Alpha = 1f;
            alphaTestEffect.Alpha = 1f;

            if (level.IsDark)
            {
                basicEffect.DiffuseColor *= new Vector3(0.5f, 0.5f, 0.5f);
                alphaTestEffect.DiffuseColor *= new Vector3(0.5f, 0.5f, 0.5f);
            }

            StaticOpaqueRenderer.Draw(level, basicEffect, CullMode.CullClockwiseFace);
            StaticOpaqueRenderer.Draw(level, basicEffect, CullMode.CullCounterClockwiseFace);
            StaticTransparentRenderer.Draw(level, basicEffect, alphaTestEffect, CullMode.CullClockwiseFace);
            StaticTransparentRenderer.Draw(level, basicEffect, alphaTestEffect, CullMode.CullCounterClockwiseFace);

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
            alphaTestEffect.DiffuseColor = alphaTestEffectDiffuseColor;
        }
    }

    public class DynamicModelListRenderer
    {
        public List<BaseModel> TotalDynamicModels { get; } = new List<BaseModel>();

        private List<OpaqueVertexRenderer> DynamicOpaqueRenderers { get; } = new List<OpaqueVertexRenderer>();
        private List<TransparentVertexRenderer> DynamicTransparentRenderers { get; } = new List<TransparentVertexRenderer>();

        public void Draw(Level level, BasicEffect basicEffect, AlphaTestEffect alphaTestEffect)
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