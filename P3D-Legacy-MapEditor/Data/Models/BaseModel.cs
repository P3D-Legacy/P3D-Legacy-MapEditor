using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Data.Models.Blocks;
using P3D.Legacy.MapEditor.Data.Models.Cliffs;
using P3D.Legacy.MapEditor.Data.Models.Other;
using P3D.Legacy.MapEditor.Data.Models.Steps;
using P3D.Legacy.MapEditor.Data.Models._2D;
using P3D.Legacy.MapEditor.Data.Vertices;
using P3D.Legacy.MapEditor.Utils;
using P3D.Legacy.MapEditor.World;

namespace P3D.Legacy.MapEditor.Data.Models
{
    public abstract class BaseModel
    {
        public static List<BaseModel> TotalStaticModels { get; } = new List<BaseModel>();
        
        protected static ModelRenderer StaticRendererOpaque;
        protected static ModelRenderer StaticRendererTransparent;


        protected List<VertexPositionNormalTexture> ModelVertices { get; } = new List<VertexPositionNormalTexture>();
        protected List<int> ModelIndices { get; } = new List<int>();

        protected class ModelTriangle : IDisposable
        {
            public BaseModel Model { get; }

            public List<VertexPositionNormalTexture> OriginalVertices => Model.ModelVertices.Skip(VertexOffset).Take(3).ToList();
            public List<int> OriginalIndices => Model.ModelIndices.Skip(VertexOffset).Take(3).ToList();
            public int VertexOffset;

            public string OriginalTexturePath { get; } // Original texture path
            public Texture2D OriginalTexture { get; } // Original texture path
            public Rectangle OriginalTextureRectangle { get; } // Original texture rectangle
            public KeyValuePair<string, Rectangle> TextureKey => new KeyValuePair<string, Rectangle>(OriginalTexturePath, OriginalTextureRectangle);
            // OriginalTexturePath and OriginalTextureRectangle are used to identity triangles for AtlasTexture creation
            
            public Rectangle AtlasTextureRectangle; // Rectangle to use when draawing with atlas

            public bool HasTransparentPixels { get; set; } // Cropped texture has transparent pixels

            public ModelTriangle(BaseModel model, string texturePath, Texture2D texture, Rectangle textureRectangle)
            {
                Model = model;
                OriginalTexturePath = texturePath;
                OriginalTexture = texture;
                OriginalTextureRectangle = textureRectangle;
            }


            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    OriginalTexture?.Dispose();
                }
            }
        }
        protected List<ModelTriangle> ModelTriangles { get; } = new List<ModelTriangle>();


        public abstract int ID { get; }
        public EntityInfo Entity { get; }

        public GraphicsDevice GraphicsDevice { get; }

        public bool HasTransparentPixels => ModelTriangles.Any(t => t.HasTransparentPixels);
        
        public Matrix ScaleMatrix => Matrix.CreateScale(Entity.Scale);
        public Matrix RotationMatrix => Matrix.CreateFromYawPitchRoll(Entity.Rotation.Y, Entity.Rotation.X, Entity.Rotation.Z);
        public Matrix TranslationMatrix => Matrix.CreateTranslation(Entity.Position);
        public Matrix WorldMatrix => ScaleMatrix * RotationMatrix * TranslationMatrix;

        public BoundingBox BoundingBox { get; set; }

        public bool IsStatic { get; private set; } = true;

        protected BaseModel(EntityInfo entity, GraphicsDevice graphicsDevice)
        {
            Entity = entity;
            GraphicsDevice = graphicsDevice;

            if(IsStatic)
                TotalStaticModels.Add(this);
        }

        public static BaseModel GetModelByEntityInfo(EntityInfo entity, GraphicsDevice graphicsDevice)
        {
            BaseModel model;
            switch (entity.ModelID)
            {
                case 0:
                    model = new FloorModel(entity, graphicsDevice);
                    break;
                case 1:
                    model = new BlockModel(entity, graphicsDevice);
                    break;
                case 2:
                    model = new SlideModel(entity, graphicsDevice);
                    break;
                case 3:
                    model = new BillModel(entity, graphicsDevice);
                    break;
                case 4:
                    model = new SignModel(entity, graphicsDevice);
                    break;
                case 5:
                    model = new CornerModel(entity, graphicsDevice);
                    break;
                case 6:
                    model = new InsideCornerModel(entity, graphicsDevice);
                    break;
                case 7:
                    model = new StepModel(entity, graphicsDevice);
                    break;
                case 8:
                    model = new InsideStepModel(entity, graphicsDevice);
                    break;
                case 9:
                    model = new CliffModel(entity, graphicsDevice);
                    break;
                case 10:
                    model = new CliffInsideModel(entity, graphicsDevice);
                    break;
                case 11:
                    model = new CliffCornerModel(entity, graphicsDevice);
                    break;
                case 12:
                    model = new CubeModel(entity, graphicsDevice);
                    break;
                case 13:
                    model = new CrossModel(entity, graphicsDevice);
                    break;
                case 14:
                    model = new DoubleFloorModel(entity, graphicsDevice);
                    break;
                case 15:
                    model = new PyramidModel(entity, graphicsDevice);
                    break;
                case 16:
                    model = new StairsModel(entity, graphicsDevice);
                    break;
                default:
                    model = new BlockModel(entity, graphicsDevice);
                    break;
            }

            return model;
        }

        protected void Setup()
        {
            var geometry = new Geometry<VertexPositionNormalTexture>();
            geometry.AddVertices(ModelVertices.ToArray());
            ModelIndices.AddRange(geometry.Indices);

            if (Entity is EntityFloorInfo && Entity.TextureIndexList == null)
                Entity.TextureIndexList = new[] { 0, 0 };

            var partCount = ModelVertices.Count / 3;

            // if TextureIndex list is smaller than the triangle count, fill them with 0
            if (Entity.TextureIndexList?.Length < partCount)
            {
                var newTextureIndexList = new int[partCount];
                for (var i = 0; i < Entity.TextureIndexList.Length; i++)
                    newTextureIndexList[i] = Entity.TextureIndexList[i];
                for (var i = Entity.TextureIndexList.Length; i < newTextureIndexList.Length; i++)
                    newTextureIndexList[i] = 0;
                Entity.TextureIndexList = newTextureIndexList;
            }

            for (var i = 0; i < partCount; i++)
            {
                var isNonVisibleModel =
                    (string.Equals(Entity.EntityID, "ScriptBlock", StringComparison.OrdinalIgnoreCase) && Entity.ActionValue == 0);

                var indexValue = Entity.TextureIndexList[i];
                if(indexValue == -1 || !Entity.Visible || isNonVisibleModel)
                    continue;

                ModelTriangles.Add(new ModelTriangle(this, Entity.TexturePath,
                    TextureHandler.LoadTexture(GraphicsDevice, Entity), Entity.TextureRectangles[indexValue])
                {
                    VertexOffset = i * 3
                });
            }

            BuildBoundingBox();
        }
        public static void SetupStatic(GraphicsDevice graphicsDevice)
        {
            var atlas = SetupTextureAtlas(graphicsDevice);
            StaticRendererOpaque = new ModelRenderer() { Atlas = atlas };
            StaticRendererTransparent = new ModelRenderer() { Atlas = atlas };

            foreach (var staticModel in TotalStaticModels)
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
                        {
                            StaticRendererTransparent.Vertices.Add(vertexNew);
                        }
                        else
                        {
                            StaticRendererOpaque.Vertices.Add(vertexNew);
                        }
                    }
                }
            }

            StaticRendererOpaque.Setup(graphicsDevice);
            StaticRendererTransparent.Setup(graphicsDevice);
        }

        private void BuildBoundingBox()
        {
            // Create initial variables to hold min and max xyz values for the mesh
            var meshMax = new Vector3(float.MinValue);
            var meshMin = new Vector3(float.MaxValue);

            foreach (var triangle in ModelTriangles)
            foreach (var vertex in triangle.OriginalVertices)
            {
                // update our values from this vertex
                meshMin = Vector3.Min(meshMin, vertex.Position);
                meshMax = Vector3.Max(meshMax, vertex.Position);
            }

            // transform by mesh bone matrix
            meshMin = Vector3.Transform(meshMin, WorldMatrix);
            meshMax = Vector3.Transform(meshMax, WorldMatrix);

            // Create the bounding box
            BoundingBox = new BoundingBox(meshMin, meshMax);
        }

        private class TextureInfo
        {
            public List<ModelTriangle> ModelTriangles { get; } = new List<ModelTriangle>();

            public Texture2D CroppedTexture; // Cropped texture from the original texture
            public bool HasTransparentPixels; // Cropped texture has transparent pixels
            public Rectangle AtlasRectangle;
        }
        public static Texture2D SetupTextureAtlas(GraphicsDevice graphicsDevice)
        {
            var triangles = TotalStaticModels.SelectMany(m => m.ModelTriangles);
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

        public void Draw(Level level, Effect effect) { }
        public static void DrawStatic(Level level, BasicEffect effect)
        {
            var effectDiffuseColor = effect.DiffuseColor;

            // Alpha should be moved to shader
            effect.Alpha = 1f;

            if (level.IsDark)
                effect.DiffuseColor *= new Vector3(0.5f, 0.5f, 0.5f);

            StaticRendererOpaque.Draw(level, effect);
            StaticRendererTransparent.Draw(level, effect);

            effect.DiffuseColor = effectDiffuseColor;
        }

        public void Dispose()
        {

        }
        public static void DisposeStatic()
        {
            foreach (var staticModel in TotalStaticModels)
                staticModel.Dispose();
            TotalStaticModels.Clear();

            TextureHandler.Dispose();
        }
    }
}