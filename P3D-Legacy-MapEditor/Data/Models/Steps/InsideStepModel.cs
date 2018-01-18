using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Data.Models.Steps
{
    public class InsideStepModel : BaseModel<InsideStepModel>
    {
        public override int ID => 8;

        public InsideStepModel(EntityInfo entity, GraphicsDevice graphicsDevice) : base(entity, graphicsDevice)
        {
            if (Vertices.Count > 0)
                return;

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0.0f, 1.0f))); //h
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, 1), new Vector2(0.0f, 0.0f))); //e
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1.0f, 1.0f))); //c

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1.0f, 1.0f))); //c
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, 1), new Vector2(0.0f, 0.0f))); //e
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), new Vector3(0, 0, 1), new Vector2(1.0f, 0.0f))); //d

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 0.0f))); //e
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 1.0f))); //h
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 1.0f))); //a

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 1.0f))); //a
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 0.0f))); //f
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 0.0f))); //e

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 1.0f))); //c
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 0.0f))); //d
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(0.0f, 1.0f))); //b

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, -0.5f), new Vector3(0, 0, -1), new Vector2(0.0f, 0.0f))); //f
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0.0f, 1.0f))); //a
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1.0f, 1.0f))); //b

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0.0f, 1.0f))); //b
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), new Vector3(0, 0, -1), new Vector2(1.0f, 1.0f))); //d
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, -1), new Vector2(1.0f, 0.0f))); //e

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(0.0f, 1.0f))); //b
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 0.0f))); //e
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, -0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 1.0f))); //f

            Setup();
        }

    }
}