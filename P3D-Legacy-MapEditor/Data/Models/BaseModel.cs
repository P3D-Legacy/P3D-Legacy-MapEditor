using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Utils;

namespace P3D.Legacy.MapEditor.Data.Models
{
    public abstract class BaseModel
    {
        protected static Dictionary<int, List<VertexPositionNormalTexture>> StaticModelVertices { get; } = new Dictionary<int, List<VertexPositionNormalTexture>>();
        protected static Dictionary<int, List<int>> StaticModelIndices { get; } = new Dictionary<int, List<int>>();

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


        public abstract int ID { get; }
        public EntityInfo Entity { get; }

        protected List<VertexPositionNormalTexture> ModelVertices => StaticModelVertices[ID];
        protected List<int> ModelIndices => StaticModelIndices[ID];

        public class ModelTriangle
        {
            public BaseModel Model { get; }

            public List<VertexPositionNormalTexture> OriginalVertices => Model.ModelVertices.Skip(VertexOffset).Take(3).ToList();
            public List<int> OriginalIndices => Model.ModelIndices.Skip(VertexOffset).Take(3).ToList();

            public int VertexOffset;

            public Texture2D OriginalTexture => TextureHandler.LoadTexture(Model.GraphicsDevice, Model.Entity);
            public int OriginalTextureIndex => Model.Entity.TextureIndexList[VertexOffset / 3];
            public Rectangle OriginalTextureRectangle => Model.Entity.TextureRectangles[OriginalTextureIndex];

            public KeyValuePair<string, Rectangle> TextureKey => new KeyValuePair<string, Rectangle>(Model.Entity.TexturePath, OriginalTextureRectangle);
            // Entity.TexturePath and OriginalTextureRectangle are used to identity triangles for AtlasTexture creation

            public Rectangle AtlasTextureRectangle; // Rectangle to use when draawing with atlas

            public bool HasTransparentPixels { get; set; } // Cropped texture has transparent pixels

            public ModelTriangle(BaseModel model, int vertexOffset)
            {
                Model = model;
                VertexOffset = vertexOffset;
            }
        }
        public List<ModelTriangle> ModelTriangles { get; } = new List<ModelTriangle>();
        
        public GraphicsDevice GraphicsDevice { get; }

        public bool HasTransparentPixels => ModelTriangles.Any(t => t.HasTransparentPixels);
        
        public Matrix ScaleMatrix => Matrix.CreateScale(Entity.Scale);
        public Matrix RotationMatrix => Matrix.CreateFromYawPitchRoll(Entity.Rotation.Y, Entity.Rotation.X, Entity.Rotation.Z);
        public Matrix TranslationMatrix => Matrix.CreateTranslation(Entity.Position);
        public Matrix WorldMatrix => ScaleMatrix * RotationMatrix * TranslationMatrix;

        public BoundingBox BoundingBox { get; set; }

        protected BaseModel(EntityInfo entity, GraphicsDevice graphicsDevice)
        {
            Entity = entity;
            GraphicsDevice = graphicsDevice;

            if (!StaticModelVertices.ContainsKey(ID))
            {
                StaticModelVertices.Add(ID, new List<VertexPositionNormalTexture>());
                StaticModelIndices.Add(ID, new List<int>());
            }
        }

        protected void Setup()
        {
            if (!ModelIndices.Any())
                BuildIndices();

            if (Entity is EntityFloorInfo && Entity.TextureIndexList == null)
                Entity.TextureIndexList = new[] { 0, 0 };
            else if(Entity.TextureIndexList == null)
                return;


            var triangles = ModelVertices.Count / 3;

            // if TextureIndex list is smaller than the triangle count, fill them with 0
            if (Entity.TextureIndexList.Length < triangles)
            {
                var newTextureIndexList = new int[triangles];
                for (var i = 0; i < Entity.TextureIndexList.Length; i++)
                    newTextureIndexList[i] = Entity.TextureIndexList[i];
                for (var i = Entity.TextureIndexList.Length; i < newTextureIndexList.Length; i++)
                    newTextureIndexList[i] = 0;
                Entity.TextureIndexList = newTextureIndexList;
            }

            for (var i = 0; i < triangles; i++)
            {
                var indexValue = Entity.TextureIndexList[i];
                if(indexValue == -1 || !Entity.Visible )
                    continue;

                ModelTriangles.Add(new ModelTriangle(this, i * 3));
            }

            BuildBoundingBox();
        }
        private void BuildIndices()
        {
            var geometry = new Geometry<VertexPositionNormalTexture>();
            geometry.AddVertices(ModelVertices.ToArray());
            ModelIndices.AddRange(geometry.Indices);
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
    }
}