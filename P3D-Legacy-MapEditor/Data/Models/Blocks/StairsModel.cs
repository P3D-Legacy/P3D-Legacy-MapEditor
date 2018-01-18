using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Data.Models.Blocks
{
    public class StairsModel : BaseModel<StairsModel>
    {
        public override int ID => 16;

        public StairsModel(EntityInfo entity, GraphicsDevice graphicsDevice) : base(entity, graphicsDevice)
        {
            if (Vertices.Count > 0)
                return;

            //Lower stair, front:
            //left vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(0, 0, 1), new Vector2(0.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0.5f), new Vector3(0, 0, 1), new Vector2(1.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0.0f, 1.0f)));
            //right vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0.5f), new Vector3(0, 0, 1), new Vector2(1.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1.0f, 1.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0.0f, 1.0f)));

            //Lower stair, top:
            //Left vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), new Vector3(0, 1, 0), new Vector2(0.0f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(0, 1, 0), new Vector2(1.0f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(0, 1, 0), new Vector2(0.0f, 0.5f)));
            //Right vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(0, 1, 0), new Vector2(1.0f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0.5f), new Vector3(0, 1, 0), new Vector2(1.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(0, 1, 0), new Vector2(0.0f, 0.5f)));

            //Upper stair, front:
            //left vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(0, 0, 1), new Vector2(0.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), new Vector3(0, 0, 1), new Vector2(1.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), new Vector3(0, 0, 1), new Vector2(0.0f, 1.0f)));
            //right vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), new Vector3(0, 0, 1), new Vector2(1.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(0, 0, 1), new Vector2(1.0f, 1.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), new Vector3(0, 0, 1), new Vector2(0.0f, 1.0f)));

            //Upper stair, top:
            //Left vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(0.0f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(1.0f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(0, 1, 0), new Vector2(0.0f, 0.5f)));
            //Right vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(1.0f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), new Vector3(0, 1, 0), new Vector2(1.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(0, 1, 0), new Vector2(0.0f, 0.5f)));

            //Back:
            //Left vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0.0f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1.0f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0.0f, 1.0f)));
            //Right vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1.0f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1.0f, 1.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0.0f, 1.0f)));

            //Left side, lower:
            //Left vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 1.0f)));
            //Right vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 1.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 1.0f)));

            //Left side, upper:
            //Left vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(-1, 0, 0), new Vector2(0.5f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 0.5f)));
            //Right vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0), new Vector3(-1, 0, 0), new Vector2(0.5f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), new Vector3(-1, 0, 0), new Vector2(0.5f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, 0, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 0.5f)));

            //Right side, lower:
            //Left vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0.5f), new Vector3(1, 0, 0), new Vector2(0.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, -0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0.0f, 1.0f)));
            //Right vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, -0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 1.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0.0f, 1.0f)));

            //Right side, upper:
            //Left vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0), new Vector3(1, 0, 0), new Vector2(0.5f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(1, 0, 0), new Vector2(0.5f, 0.5f)));
            //Right vertex:
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 0.0f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, -0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 0.5f)));
            Vertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), new Vector3(1, 0, 0), new Vector2(0.5f, 0.5f)));

            Setup();
        }

    }
}