using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Primitives
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionNormalColorTexture : IVertexType
    {
        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(00, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), 
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), 
            new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0), 
            new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));

        public Vector3 Position;
        public Vector3 Normal;
        public Color Color;
        public Vector2 TextureCoordinate;
        
        public VertexPositionNormalColorTexture(Vector3 position, Vector3 normal, Color color, Vector2 textureCoordinate)
        {
            Position = position;
            Normal = normal;
            Color = color;
            TextureCoordinate = textureCoordinate;
        }

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Normal.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ TextureCoordinate.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() => 
            $"{{{{Position:{Position} Normal:{Normal} Color:{Color} TextureCoordinate:{TextureCoordinate}}}}}";

        public static bool operator ==(VertexPositionNormalColorTexture left, VertexPositionNormalColorTexture right) =>
            left.Position == right.Position && left.Normal == right.Normal && left.Color == right.Color && left.TextureCoordinate == right.TextureCoordinate;

        public static bool operator !=(VertexPositionNormalColorTexture left, VertexPositionNormalColorTexture right) =>
            !(left == right);

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            return this == (VertexPositionNormalColorTexture) obj;
        }
    }
}
