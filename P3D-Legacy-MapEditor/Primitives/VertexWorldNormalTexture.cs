using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Data
{
    public struct VertexWorldNormalTexture : IVertexType
    {
        public Matrix World;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;

        public VertexWorldNormalTexture(Matrix world, Vector3 normal, Vector2 textureCoordinate)
        {
            World = world;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
        }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(00, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3),
            new VertexElement(64, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(76, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }
}