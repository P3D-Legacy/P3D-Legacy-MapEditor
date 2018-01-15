using Microsoft.Xna.Framework;

namespace P3D_Legacy.MapEditor.World
{
    public class Entity
    {
        public static Vector3 GetRotationFromInteger(int i)
        {
            switch (i)
            {
                case 0:
                    return new Vector3(0, 0, 0);
                case 1:
                    return new Vector3(0, MathHelper.PiOver2, 0);
                case 2:
                    return new Vector3(0, MathHelper.Pi, 0);
                case 3:
                    return new Vector3(0, MathHelper.Pi * 1.5f, 0);
            }

            return Vector3.Zero;
        }
    }
}
