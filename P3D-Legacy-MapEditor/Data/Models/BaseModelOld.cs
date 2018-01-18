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
using P3D.Legacy.MapEditor.Primitives;
using P3D.Legacy.MapEditor.World;

namespace P3D.Legacy.MapEditor.Data.Models
{
    public abstract class BaseModel1
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

        protected BaseModel1(EntityInfo entity, GraphicsDevice graphicsDevice)
        {
            Entity = entity;
            GraphicsDevice = graphicsDevice;
        }

        protected virtual void Setup()
        {
            if (Entity.TextureIndexList == null)
                Entity.TextureIndexList = new[] { 0 };
            if (Entity.TextureRectangles == null)
                Entity.TextureRectangles = new []{ new Rectangle(0, 0, 0, 0) }; // Get rectangle from texture

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
    public abstract class BaseModel1<T> : BaseModel where T: BaseModel
    {
        static BaseModel1()
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

            public List<VertexPositionNormalTexture> Vertices { get; } = new List<VertexPositionNormalTexture>();
            public VertexBuffer StaticVertexBuffer;

            public void Setup(GraphicsDevice graphicsDevice)
            {
                StaticVertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), Vertices.Count, BufferUsage.WriteOnly);
                StaticVertexBuffer.SetData(Vertices.ToArray());
            }

            public void Draw(Level level, Effect effect)
            {
                effect.GraphicsDevice.SetVertexBuffer(StaticVertexBuffer);

                foreach (var effectPass in effect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();
                    effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, StaticVertexBuffer.VertexCount / 3);
                }
            }
        }
        protected static Dictionary<int, ModelPartStaticVertexBuffer> PartsStaticVertexBuffer { get; } = new Dictionary<int, ModelPartStaticVertexBuffer>();

        protected static List<BaseModel1<T>> Models { get; } = new List<BaseModel1<T>>();

        protected BaseModel1(EntityInfo entity, GraphicsDevice graphicsDevice) : base(entity, graphicsDevice)
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
                        var editableVertex = vertex;
                        editableVertex.Position += model.Entity.Position;
                        PartsStaticVertexBuffer[i].Vertices.Add(editableVertex);
                    }
                }
            }

            for (var i = 0; i < PartsStaticVertexBuffer.Count; i++)
                PartsStaticVertexBuffer[i].Setup(graphicsDevice);
        }

        private static Random r = new Random();
        public static void DrawStaticPerType1(Level level, Effect effect)
        {
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(255.0f));

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
        }
        public static void DrawStaticPerType(Level level, BasicEffect effect)
        {
            //if (typeof(T) == typeof(FloorModel))
            //    return;

            var effectDiffuseColor = effect.DiffuseColor;

            //effect.World = entity.World;
            //effect.World = Matrix.Identity;//Matrix.CreateScale(entity.Scale) * Matrix.CreateFromYawPitchRoll(entity.Rotation.Y, entity.Rotation.X, entity.Rotation.Z) * Matrix.CreateTranslation(entity.Position);
            effect.TextureEnabled = false;
            //effect.Alpha = entity.Opacity;
            effect.Alpha = 1f;

            effect.DiffuseColor = effectDiffuseColor * new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());// * entity.Shader;

            if (level.IsDark)
                effect.DiffuseColor *= new Vector3(0.5f, 0.5f, 0.5f);


            //effect.Parameters["DiffuseColor"].SetValue(new Vector4(
            //    Color.White.ToVector3() * new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble())
            //    , 1.0f));

            for (var i = 0; i < PartsStaticVertexBuffer.Count; i++)
                PartsStaticVertexBuffer[i].Draw(level, effect);

            effect.DiffuseColor = effectDiffuseColor;
        }
    }


    /*
    public abstract class BaseModel1
    {
        public static void DrawStatic(Level level, Effect effect, EntityInfo entity, Texture2D[] textures)
        {

        }
        
        public abstract int ID { get; }
        public virtual List<VertexPositionNormalTexture> Vertices { get; } = new List<VertexPositionNormalTexture>();
        public virtual List<VertexWorldNormalTexture> NewVertices { get; } = new List<VertexWorldNormalTexture>();
        public abstract VertexBuffer VertexBuffer { get; }

        public Texture2D Texture { get; set; }
        public int[] TextureIndexes { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public Matrix World => Matrix.CreateScale(Scale) *
                               Matrix.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);

        protected bool IsFloor = false;

        //public BaseModel() { }

        public abstract void Init();

        public abstract void Draw(Level level, Effect effect, Entity entity, Texture2D[] textures);



        public static BaseModel GetModelByID(int id, bool isAllside, GraphicsDevice graphicsDevice)
        {
            var model = default(BaseModel);
            switch (id)
            {
                case 0:
                    model = new FloorModel(graphicsDevice);
                    break;
                case 1:
                    model = new BlockModel(graphicsDevice);
                    break;
                case 2:
                    model = new SlideModel(graphicsDevice);
                    break;
                case 3:
                    model = new BillModel(graphicsDevice);
                    break;
                case 4:
                    model = new SignModel(graphicsDevice);
                    break;
                case 5:
                    model = new CornerModel(graphicsDevice);
                    break;
                case 6:
                    model = new InsideCornerModel(graphicsDevice);
                    break;
                case 7:
                    model = new StepModel(graphicsDevice);
                    break;
                case 8:
                    model = new InsideStepModel(graphicsDevice);
                    break;
                case 9:
                    model = new CliffModel(graphicsDevice);
                    break;
                case 10:
                    model = new CliffInsideModel(graphicsDevice);
                    break;
                case 11:
                    model = new CliffCornerModel(graphicsDevice);
                    break;
                case 12:
                    model = new CubeModel(graphicsDevice);
                    break;
                case 13:
                    model = new CrossModel(graphicsDevice);
                    break;
                case 14:
                    model = new DoubleFloorModel(graphicsDevice);
                    break;
                case 15:
                    model = new PyramidModel(graphicsDevice);
                    break;
                case 16:
                    model = new StairsModel(graphicsDevice);
                    break;
                default:
                    model = new BlockModel(graphicsDevice);
                    break;
            }

            model.IsFloor = isAllside;

            return model;
        }
    }
    public abstract class BaseModel1<T> : BaseModel
    {
        #region Shit
        public struct Pair
        {
            public int Index;       // 0 = 0-1,  1 = 2-3,  3 = 4-5,  4 = 6-7
            public int IndexValue;  // Works only if both values are equal
        }
        private static Dictionary<Pair, List<VertexWorldNormalTexture>> VerticesInPairs { get; } = new Dictionary<Pair, List<VertexWorldNormalTexture>>();
        private static List<VertexBuffer> PairsBuffers { get; } = new List<VertexBuffer>();
        private static bool AfterInit = false;

        private static void GetPairs(BaseModel model, int[] textureIndexes)
        {
            // 2 пары  вырезаем 4 пары  2 пары
            //[ 0, 0,  -1,-1,   2, 2,   3, 3]
            //[ 2, 2,  -1,-1,   2, 2,   1, 1]
            //[ 0, 0,  -1,-1,   2, 2,   2, 2]
            //[ 3, 3,  -1,-1,   2, 2,   3, 3]

            for (int i = 0; i < textureIndexes.Length; i++)
            {
                var pair = new Pair() { Index = i, IndexValue = textureIndexes[i] };
                var vertices = model.NewVertices.Skip(i * 3).Take(3).ToList();
                //var vertices = model.Vertices.Skip(i * 3).Take(3).ToList();

                if (!VerticesInPairs.ContainsKey(pair))
                {
                    if (vertices.Count > 0)
                        VerticesInPairs.Add(pair, vertices);
                }
                else
                    VerticesInPairs[pair].AddRange(vertices);
            }
            //for (int i = 0, j = 0; i < textureIndexes.Length / 2; i += 2, j++)
            //{
            //    if (textureIndexes[i] != textureIndexes[i + 1])
            //        continue;
            //
            //    var pair = new Pair() { Index = j, IndexValue = textureIndexes[i] };
            //    var vertices = model.Vertices.Skip(j * 3).Take(3).ToList();
            //
            //    if(!VerticesInPairs.ContainsKey(pair))
            //        VerticesInPairs.Add(pair, vertices);
            //    else
            //        VerticesInPairs[pair].AddRange(vertices);
            //}
        }
        private static void Aft(GraphicsDevice graphicsDevice)
        {
            foreach (var pair in VerticesInPairs)
            {
                if(pair.Value.Count == 0)
                    continue;

                var buffer = new VertexBuffer(graphicsDevice, typeof(VertexWorldNormalTexture), pair.Value.Count, BufferUsage.WriteOnly);
                buffer.SetData(pair.Value.ToArray());
                PairsBuffers.Add(buffer);
            }
        }
        #endregion


        protected static List<BaseModel<T>> Models { get; } = new List<BaseModel<T>>();
        
        private static Random r = new Random();

        protected GraphicsDevice GraphicsDevice { get; }
        protected BaseModel(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;

            Models.Add(this);
        }

        public override void Init()
        {
            for (var i = 0; i < Vertices.Count; i++)
            {
                NewVertices.Add(new VertexWorldNormalTexture(
                    Matrix.Transpose(Matrix.CreateTranslation(Vertices[i].Position + Position)),
                    Vertices[i].Normal,
                    Vertices[i].TextureCoordinate));
                Vertices[i] = new VertexPositionNormalTexture(
                    Vertices[i].Position + Position,
                    Vertices[i].Normal,
                    Vertices[i].TextureCoordinate);
            }

            if(TextureIndexes == null)
                return;

            GetPairs(this, TextureIndexes);

            NVertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexWorldNormalTexture), NewVertices.Count, BufferUsage.WriteOnly);
            NVertexBuffer.SetData(NewVertices.ToArray());
        }

        //public List<VertexPositionNormalTexture> VertexData { get; } = new List<VertexPositionNormalTexture>();
        private static VertexBuffer SVertexBuffer { get; set; }
        private static VertexBuffer NVertexBuffer { get; set; }

        protected static List<int> IndexData { get; } = new List<int>();
        private static IndexBuffer SIndexBuffer { get; set; }

        protected void SetupVb()
        {
            SVertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionNormalTexture), Vertices.Count, BufferUsage.WriteOnly);
            SVertexBuffer.SetData(Vertices.ToArray());
            //VertexData.Clear();
        }
        protected void SetupIb()
        {
            SIndexBuffer = new IndexBuffer(GraphicsDevice, typeof(int), IndexData.Count, BufferUsage.WriteOnly);
            SIndexBuffer.SetData(IndexData.ToArray());
            //VertexData.Clear();
        }

        public sealed override VertexBuffer VertexBuffer => SVertexBuffer;


        // Вершины разделены на пары по три. Текстурные индексы маркируют эти пары. -1 означает,
        // что пара не используется. Следовательно, мы может обтро
        //

        public new static void DrawStatic(Level level, Effect effect, EntityInfo entity, Texture2D[] textures)
        {
            if (!AfterInit)
            {
                AfterInit = true;
                Aft(effect.GraphicsDevice);
            }

            effect.Parameters["DiffuseColor"].SetValue(new Vector3(255.0f));

            var effectDiffuseColor = effect.Parameters["DiffuseColor"].GetValueVector3();
            //effect.Alpha = entity.Opacity;
            effect.Parameters["Alpha"].SetValue(1f);

            effect.Parameters["DiffuseColor"].SetValue(
                effectDiffuseColor * new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble()));

            if (level.IsDark)
                effect.Parameters["DiffuseColor"].SetValue(
                    effect.Parameters["DiffuseColor"].GetValueVector3() * new Vector3(0.5f, 0.5f, 0.5f));


            foreach (var vertexBuffer in PairsBuffers)
            {
                effect.GraphicsDevice.SetVertexBuffer(vertexBuffer);

                foreach (var effectPass in effect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();

                    effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount);
                }
            }
        }


        public new static void DrawStatic(Level level, BasicEffect effect, EntityInfo entity, Texture2D[] textures)
        {
            if (!AfterInit)
            {
                AfterInit = true;
                Aft(effect.GraphicsDevice);
            }

            var effectDiffuseColor = effect.DiffuseColor;

            //effect.World = entity.World;
            effect.World = Matrix.CreateScale(entity.Scale) * Matrix.CreateFromYawPitchRoll(entity.Rotation.Y, entity.Rotation.X, entity.Rotation.Z) * Matrix.CreateTranslation(entity.Position);
            effect.TextureEnabled = false;
            //effect.Alpha = entity.Opacity;
            effect.Alpha = 1f;

            effect.DiffuseColor = effectDiffuseColor * new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());// * entity.Shader;

            if (level.IsDark)
                effect.DiffuseColor *= new Vector3(0.5f, 0.5f, 0.5f);


            foreach (var vertexBuffer in PairsBuffers)
            {
                effect.GraphicsDevice.SetVertexBuffer(vertexBuffer);
                effect.Techniques[0].Passes[0].Apply();
                effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount / 3);
            }
            return;
        }

        public override void Draw(Level level, Effect effect, Entity entity, Texture2D[] textures)
        {
            if (!AfterInit)
            {
                AfterInit = true;
                Aft(GraphicsDevice);
            }

            //*
            Vector3 effectDiffuseColor = effect.DiffuseColor;

            //effect.World = entity.World;
            //effect.World = Matrix.CreateScale(Scale) * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) * Matrix.CreateTranslation(Position);
            //effect.TextureEnabled = false;
            //effect.Alpha = entity.Opacity;
            effect.Alpha = 1f;

            effect.DiffuseColor = effectDiffuseColor * RandColor;// * entity.Shader;

            if (level.IsDark)
                effect.DiffuseColor *= new Vector3(0.5f, 0.5f, 0.5f);
            ///*

            effect.Parameters["DiffuseColor"].SetValue(new Vector3(255.0f));

            var effectDiffuseColor = effect.Parameters["DiffuseColor"].GetValueVector3();
            //effect.Alpha = entity.Opacity;
            effect.Parameters["Alpha"].SetValue(1f);

            effect.Parameters["DiffuseColor"].SetValue(
                effectDiffuseColor * new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble()));

            if (level.IsDark)
                effect.Parameters["DiffuseColor"].SetValue(
                    effect.Parameters["DiffuseColor"].GetValueVector3() * new Vector3(0.5f, 0.5f, 0.5f));


            /*
            foreach (var vertexBuffer in PairsBuffers)
            {
                GraphicsDevice.SetVertexBuffer(vertexBuffer);
                effect.Techniques[0].Passes[0].Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount / 3);
            }
            return;
            //*
            GraphicsDevice.SetVertexBuffer(NVertexBuffer);
            effect.Techniques[0].Passes[0].Apply();

            //if (IsFloor)
            //{
            //    GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            //    GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, NVertexBuffer.VertexCount);
            //}

            //GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, NVertexBuffer.VertexCount);


            /*
            if (VertexBuffer.VertexCount / 3 > entity.TextureIndex.Length)
            {
                int[] newTextureIndex = new int[VertexBuffer.VertexCount / 3 + 1];
                for (var i = 0; i <= VertexBuffer.VertexCount / 3; i++)
                {
                    if (entity.TextureIndex.Length - 1 >= i)
                    {
                        newTextureIndex[i] = entity.TextureIndex[i];
                    }
                    else
                    {
                        newTextureIndex[i] = 0;
                    }
                }
                entity.TextureIndex = newTextureIndex;
            }

            bool isEqual = true;
            if (entity.HasEqualTextures == -1)
            {
                entity.HasEqualTextures = 1;
                int contains = entity.TextureIndex[0];
                for (var index = 1; index <= entity.TextureIndex.Length - 1; index++)
                {
                    if (contains != entity.TextureIndex[index])
                    {
                        entity.HasEqualTextures = 0;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }
            if (entity.HasEqualTextures == 0)
            {
                isEqual = false;
            }

            if (isEqual)
            {
                if (entity.TextureIndex[0] > -1)
                {
                    ApplyTexture(effect, textures[entity.TextureIndex[0]]);

                    Core.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, VertexBuffer.VertexCount / 3);

                    RenderTracker.DrawnVertices += VertexBuffer.VertexCount / 3;
                }
            }
            else
            {
                for (var i = 0; i <= VertexBuffer.VertexCount - 1; i += 3)
                {
                    if (entity.TextureIndex[i / 3] > -1)
                    {
                        ApplyTexture(effect, textures[entity.TextureIndex[i / 3]]);

                        Core.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, i, 1);
                        RenderTracker.DrawnVertices += 1;
                    }
                }
            }
            //*

            effect.Parameters["DiffuseColor"].SetValue(effectDiffuseColor);
            //effect.DiffuseColor = effectDiffuseColor;
        }

        protected void ApplyTexture(BasicEffect effect, Texture2D texture)
        {
            effect.Texture = texture;
            effect.CurrentTechnique.Passes[0].Apply();
        }
    }
    */
}
