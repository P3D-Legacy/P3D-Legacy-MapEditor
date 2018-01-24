using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Data.Models
{
    public class CliffInsideModel : BaseModel
    {
        public override int ID => 10;

        public CliffInsideModel(EntityInfo entity, GraphicsDevice graphicsDevice) : base(entity, graphicsDevice)
        {
            if (ModelVertices.Count > 0)
            {
                Setup();
                return;
            }

            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(0, 0)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)));

            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(1, 1)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)));

            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.25f), Vector3.Forward, new Vector2(0.0f, 1.0f))); //h
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), Vector3.Forward, new Vector2(0.0f, 0.0f))); //e
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.25f), Vector3.Forward, new Vector2(1.0f, 1.0f))); //c

            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.25f), Vector3.Forward, new Vector2(1.0f, 1.0f))); //c
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), Vector3.Forward, new Vector2(0.0f, 0.0f))); //e
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), Vector3.Forward, new Vector2(1.0f, 0.0f))); //d

            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 0.0f))); //e
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.25f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 1.0f))); //h
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.25f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 1.0f))); //a

            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.25f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 1.0f))); //a
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 0.0f))); //f
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 0.0f))); //e

            Setup();
        }

    }
}
