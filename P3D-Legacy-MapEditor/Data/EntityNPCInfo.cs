using Microsoft.Xna.Framework;

namespace P3D.Legacy.MapEditor.Data
{
    public class EntityNPCInfo : EntityInfo
    {
        public string Name { get; set; }
        public string TextureID { get; set; }
        public string Movement { get; set; }
        public Rectangle[] MoveRectangles { get; set; }
        public bool AnimateIdle { get; set; }
        public int FaceRotation { get; set; }

        public override string ToString() => $"{base.ToString()} {Name}";
    }
}