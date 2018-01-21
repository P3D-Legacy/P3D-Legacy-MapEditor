namespace P3D.Legacy.MapEditor.World
{
    public static class Extensions
    {
        public static int Clamp(this int d, int min, int max)
        {
            if (d > max)
            {
                d = max;
            }

            if (d < min)
            {
                d = min;
            }

            return d;
        }
    }
}