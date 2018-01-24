using Microsoft.Xna.Framework;

namespace P3D.Legacy.MapEditor.World
{
    public class Entity
    {
        public static Vector3 GetRotationFromInteger(int rotation)
        {
            switch (rotation)
            {
                case 0:
                    return new Vector3(0, 0, 0);
                case 1:
                    return new Vector3(0, MathHelper.PiOver2, 0);
                case 2:
                    return new Vector3(0, MathHelper.Pi, 0);
                case 3:
                    return new Vector3(0, MathHelper.Pi * 1.5f, 0);
                default:
                    return Vector3.Zero;
            }
        }
        public static int GetRotationFromVector(Vector3 vector)
        {
            switch (vector.Y)
            {
                case 0:
                    return 0;
                case MathHelper.PiOver2:
                    return 1;
                case MathHelper.Pi:
                    return 2;
                case MathHelper.Pi * 1.5f:
                    return 3;
                default:
                    return 0;
            }
        }
    }
}
