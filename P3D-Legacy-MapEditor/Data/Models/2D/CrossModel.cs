using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Data.Models._2D
{
    public class CrossModel : BaseModel<CrossModel>
    {
        public override int ID => 13;

        public CrossModel(EntityInfo entity, GraphicsDevice graphicsDevice) : base(entity, graphicsDevice)
        {
            if (Vertices.Count > 0)
                return;

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0), Vector3.Backward, new Vector2(0, 1)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), Vector3.Backward, new Vector2(0, 0)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), Vector3.Backward, new Vector2(1, 0)));

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), Vector3.Backward, new Vector2(1, 0)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0), Vector3.Backward, new Vector2(1, 1)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0), Vector3.Backward, new Vector2(0, 1)));

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0, 0.5f, -0.5f), Vector3.Backward, new Vector2(0, 0)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0, -0.5f, -0.5f), Vector3.Backward, new Vector2(0, 1)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0, -0.5f, 0), Vector3.Backward, new Vector2(1, 1)));

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0, -0.5f, 0), Vector3.Backward, new Vector2(1, 1)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0, 0.5f, 0), Vector3.Backward, new Vector2(1, 0)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0, 0.5f, -0.5f), Vector3.Backward, new Vector2(0, 0)));

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0, 0.5f, 0), Vector3.Backward, new Vector2(0, 0)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0, -0.5f, 0), Vector3.Backward, new Vector2(0, 1)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0, -0.5f, 0.5f), Vector3.Backward, new Vector2(1, 1)));

            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0, -0.5f, 0.5f), Vector3.Backward, new Vector2(1, 1)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0, 0.5f, 0.5f), Vector3.Backward, new Vector2(1, 0)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0, 0.5f, 0), Vector3.Backward, new Vector2(0, 0)));

            Setup();
        }

    }
}