using Microsoft.Xna.Framework;

namespace P3D.Legacy.MapEditor.World
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
        public static int GetRotationFromVector(Vector3 v)
        {
            if (v.Y == 0)
                return 0;
            if (v.Y == MathHelper.PiOver2)
                return 1;
            if (v.Y == MathHelper.Pi)
                return 2;
            if (v.Y == MathHelper.Pi * 1.5f)
                return 3;

            return 0;
        }

        public Matrix World { get; set; }
        public float Opacity { get; set; }
        public Vector3 Shader { get; set; }
    }
}
