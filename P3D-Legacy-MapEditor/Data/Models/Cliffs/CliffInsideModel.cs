using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Data.Models.Cliffs
{
    public class CliffInsideModel : BaseModel<CliffInsideModel>
    {
        public override int ID => 10;

        public CliffInsideModel(EntityInfo entity, GraphicsDevice graphicsDevice) : base(entity, graphicsDevice)
        {
            if (Vertices.Count > 0)
                return;

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(0, 0)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)));

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), Vector3.Up, new Vector2(1, 0)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(1, 1)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), Vector3.Up, new Vector2(0, 1)));

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.25f), Vector3.Forward, new Vector2(0.0f, 1.0f))); //h
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), Vector3.Forward, new Vector2(0.0f, 0.0f))); //e
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.25f), Vector3.Forward, new Vector2(1.0f, 1.0f))); //c

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.25f), Vector3.Forward, new Vector2(1.0f, 1.0f))); //c
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), Vector3.Forward, new Vector2(0.0f, 0.0f))); //e
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), Vector3.Forward, new Vector2(1.0f, 0.0f))); //d

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 0.0f))); //e
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.25f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 1.0f))); //h
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.25f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 1.0f))); //a

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.25f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 1.0f))); //a
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 0.0f))); //f
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 0.0f))); //e

            Setup();
        }

    }
}
