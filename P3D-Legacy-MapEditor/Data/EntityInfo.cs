using Microsoft.Xna.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace P3D_Legacy.MapEditor.Data
{
    public class EntityInfo
    {
        public string EntityID { get; set; }
        public int ID { get; set; } = -1;
        public int ModelID { get; set; }

        public Vector3 Position { get; set; }

        public string TexturePath { get; set; }
        public Rectangle[] TextureRectangles { get; set; }
        public int[] TextureIndexList { get; set; }

        public Vector3 Scale { get; set; } = Vector3.One;
        public bool Collision { get; set; }
        public int ActionValue { get; set; }
        public string AdditionalValue { get; set; } = "";
        public Vector3 Rotation { get; set; }
        public bool Visible { get; set; } = true;
        public Vector3 Shader { get; set; } = Vector3.One;
        public string SeasonTexture { get; set; } = "";
        public string SeasonToggle { get; set; } = "";
        public float Opacity { get; set; } = 1f;


        public override string ToString() => EntityID;
    }
}