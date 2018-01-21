using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Data.Models.Blocks
{
    public class StairsModel : BaseModel
    {
        public override int ID => 16;

        public StairsModel(EntityInfo entity, GraphicsDevice graphicsDevice) : base(entity, graphicsDevice)
        {
            if (ModelVertices.Count > 0)
                return;

            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(0, 0, 1), new Vector2(0.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0.5f), new Vector3(0, 0, 1), new Vector2(1.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0.5f), new Vector3(0, 0, 1), new Vector2(1.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), new Vector3(0, 1, 0), new Vector2(0.0f, 0.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(0, 1, 0), new Vector2(1.0f, 0.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(0, 1, 0), new Vector2(0.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(0, 1, 0), new Vector2(1.0f, 0.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0.5f), new Vector3(0, 1, 0), new Vector2(1.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(0, 1, 0), new Vector2(0.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(0, 0, 1), new Vector2(0.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), new Vector3(0, 0, 1), new Vector2(1.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), new Vector3(0, 0, 1), new Vector2(0.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), new Vector3(0, 0, 1), new Vector2(1.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(0, 0, 1), new Vector2(1.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), new Vector3(0, 0, 1), new Vector2(0.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(0.0f, 0.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(1.0f, 0.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(0, 1, 0), new Vector2(0.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(1.0f, 0.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), new Vector3(0, 1, 0), new Vector2(1.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(0, 1, 0), new Vector2(0.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0.0f, 0.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1.0f, 0.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 0.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(-1, 0, 0), new Vector2(0.5f, 0.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(-1, 0, 0), new Vector2(0.5f, 0.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), new Vector3(-1, 0, 0), new Vector2(0.5f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0.5f), new Vector3(1, 0, 0), new Vector2(0.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, -0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, -0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0.0f, 1.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), new Vector3(1, 0, 0), new Vector2(0.5f, 0.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 0.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(1, 0, 0), new Vector2(0.5f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 0.0f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, -0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 0.5f)));
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(1, 0, 0), new Vector2(0.5f, 0.5f)));

            Setup();
        }

    }
}