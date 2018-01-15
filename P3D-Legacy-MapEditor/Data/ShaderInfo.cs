using System.Drawing;
using Microsoft.Xna.Framework;

namespace P3D_Legacy.MapEditor.Data
{
    public class ShaderInfo
    {
        public Vector3 Size { get; set; }
        public Vector3 Shader { get; set; }
        public bool StopOnContact { get; set; }
        public Vector3 Position { get; set; }
        public Size ObjectSize { get; set; }
        public int[] DayTime { get; set; }
    }
}
