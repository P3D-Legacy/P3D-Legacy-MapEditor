using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.Legacy.MapEditor.Data.Models.Blocks;
using P3D.Legacy.MapEditor.Data.Models.Cliffs;
using P3D.Legacy.MapEditor.Data.Models.Other;
using P3D.Legacy.MapEditor.Data.Models.Steps;
using P3D.Legacy.MapEditor.Data.Models._2D;
using P3D.Legacy.MapEditor.World;

namespace P3D.Legacy.MapEditor.Data.Models
{
    public abstract class BaseModel
    {
        protected class ModelTypeStaticInfo
        {
            public Type ModulType;
            public MethodInfo SetupMethodInfo;
            public MethodInfo DrawMethodInfo;
        }
        protected static List<ModelTypeStaticInfo> ModelTypes { get; } = new List<ModelTypeStaticInfo>();

        protected class ModelTriangle
        {
            public List<VertexPositionNormalTexture> Vertices;
            public int TextureIndex;
            public Rectangle TextureRectangle;
        }
        protected GraphicsDevice GraphicsDevice { get; }
        public Texture2D Atlas { get; set; }
        protected List<ModelTriangle> ModelParts { get; } = new List<ModelTriangle>();

        protected List<VertexPositionNormalTexture> Vertices { get; } = new List<VertexPositionNormalTexture>();


        public abstract int ID { get; }
        protected EntityInfo Entity { get; }

        protected BaseModel(EntityInfo entity, GraphicsDevice graphicsDevice)
        {
            Entity = entity;
            GraphicsDevice = graphicsDevice;
        }

        protected virtual void Setup()
        {
            if (Entity.TextureIndexList == null)
                Entity.TextureIndexList = new[] { 0 };
            if (Entity.TextureRectangles == null)
                Entity.TextureRectangles = new[] { new Rectangle(0, 0, 0, 0) }; // Get rectangle from texture

            var partCount = Vertices.Count / 3;

            // if TextureIndex list is smaller than the triangle count, fill them with the first index value
            if (Entity.TextureIndexList.Length < partCount)
            {
                var newTextureIndexList = new int[partCount];
                for (var i = 0; i < Entity.TextureIndexList.Length; i++)
                    newTextureIndexList[i] = Entity.TextureIndexList[i];
                for (var i = Entity.TextureIndexList.Length; i < newTextureIndexList.Length; i++)
                    newTextureIndexList[i] = newTextureIndexList[0];
                Entity.TextureIndexList = newTextureIndexList;
            }
            // same for TextureRectangles
            if (Entity.TextureRectangles.Length < partCount)
            {
                var newTextureRectangles = new Rectangle[partCount];
                for (var i = 0; i < Entity.TextureRectangles.Length; i++)
                    newTextureRectangles[i] = Entity.TextureRectangles[i];
                for (var i = Entity.TextureRectangles.Length; i < newTextureRectangles.Length; i++)
                    newTextureRectangles[i] = newTextureRectangles[0];
                Entity.TextureRectangles = newTextureRectangles;
            }

            for (var i = 0; i < partCount; i++)
            {
                ModelParts.Add(new ModelTriangle()
                {
                    TextureIndex = Entity.TextureIndexList[i],
                    TextureRectangle = Entity.TextureRectangles[i],
                    Vertices = Vertices.Skip(i * 3).Take(3).ToList()
                });
            }
        }
        public static void SetupStatic(GraphicsDevice graphicsDevice)
        {
            // Ensure we build static vertex buffer only once per model type
            foreach (var modelType in ModelTypes)
                modelType.SetupMethodInfo.Invoke(null, new object[] { graphicsDevice });
        }

        public virtual void Draw(Level level, Effect effect) { }
        public static void DrawStatic(Level level, Effect effect)
        {
            foreach (var modelType in ModelTypes)
                modelType.DrawMethodInfo.Invoke(null, new object[] { level, effect });
        }

        public static BaseModel GetModelByEntityInfo(EntityInfo entity, GraphicsDevice graphicsDevice)
        {
            var model = default(BaseModel);
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
    }

    //Static variables will be per model type
    public abstract class BaseModel<T> : BaseModel where T : BaseModel
    {
        static BaseModel()
        {
            ModelTypes.Add(new ModelTypeStaticInfo()
            {
                ModulType = typeof(BaseModel<T>),
                SetupMethodInfo = typeof(BaseModel<T>).GetMethod("SetupStaticPerType", BindingFlags.Public | BindingFlags.Static),
                DrawMethodInfo = typeof(BaseModel<T>).GetMethod("DrawStaticPerType", BindingFlags.Public | BindingFlags.Static)
            });
        }

        protected class ModelPartStaticVertexBuffer
        {
            public ModelTriangle Info;

            public List<VertexWorldNormalTexture> Vertices { get; } = new List<VertexWorldNormalTexture>();
            public VertexBuffer StaticVertexBuffer;

            public void Setup(GraphicsDevice graphicsDevice)
            {
                StaticVertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexWorldNormalTexture), Vertices.Count, BufferUsage.WriteOnly);
                StaticVertexBuffer.SetData(Vertices.ToArray());
            }

            public void Draw(Level level, Effect effect)
            {
                effect.GraphicsDevice.SetVertexBuffer(StaticVertexBuffer);

                foreach (var effectPass in effect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();
                    effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, StaticVertexBuffer.VertexCount);
                }
            }
        }
        protected static Dictionary<int, ModelPartStaticVertexBuffer> PartsStaticVertexBuffer { get; } = new Dictionary<int, ModelPartStaticVertexBuffer>();

        protected static List<BaseModel<T>> Models { get; } = new List<BaseModel<T>>();

        protected BaseModel(EntityInfo entity, GraphicsDevice graphicsDevice) : base(entity, graphicsDevice)
        {
            Models.Add(this);
        }

        public static void SetupStaticPerType(GraphicsDevice graphicsDevice)
        {
            foreach (var model in Models)
            {
                if (!PartsStaticVertexBuffer.Any())
                    for (var i = 0; i < model.ModelParts.Count; i++)
                        PartsStaticVertexBuffer.Add(i, new ModelPartStaticVertexBuffer());

                for (var i = 0; i < model.ModelParts.Count; i++)
                {
                    var vertices = model.ModelParts[i].Vertices;
                    foreach (var vertex in vertices)
                    {
                        PartsStaticVertexBuffer[i].Vertices.Add(new VertexWorldNormalTexture(
                            Matrix.CreateScale(model.Entity.Scale) *
                            Matrix.CreateFromYawPitchRoll(model.Entity.Rotation.Y, model.Entity.Rotation.X, model.Entity.Rotation.Z) *
                            Matrix.Transpose(Matrix.CreateTranslation(vertex.Position + model.Entity.Position)),
                            vertex.Normal,
                            vertex.TextureCoordinate));
                    }
                }
            }

            for (var i = 0; i < PartsStaticVertexBuffer.Count; i++)
                PartsStaticVertexBuffer[i].Setup(graphicsDevice);
        }

        private static Random r = new Random();
        public static void DrawStaticPerType(Level level, Effect effect)
        {
            var effectDiffuseColor = effect.Parameters["DiffuseColor"].GetValueVector3();
            //effect.Alpha = entity.Opacity;
            effect.Parameters["Alpha"].SetValue(1f);

            effect.Parameters["DiffuseColor"].SetValue(
                effectDiffuseColor * new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble()));

            if (level.IsDark)
                effect.Parameters["DiffuseColor"].SetValue(
                    effect.Parameters["DiffuseColor"].GetValueVector3() * new Vector3(0.5f, 0.5f, 0.5f));

            for (var i = 0; i < PartsStaticVertexBuffer.Count; i++)
                PartsStaticVertexBuffer[i].Draw(level, effect);

            effect.Parameters["DiffuseColor"].SetValue(effectDiffuseColor);
        }
    }
}