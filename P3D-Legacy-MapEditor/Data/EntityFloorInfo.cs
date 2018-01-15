using System.Drawing;

namespace P3D_Legacy.MapEditor.Data
{
    public class EntityFloorInfo : EntityInfo
    {
        public Size Size { get; set; }
        public bool RemoveFloor { get; set; }
        public bool HasSnow { get; set; } = true;
        public bool HasSand { get; set; } = true;
        public bool HasIce { get; set; }

        public EntityFloorInfo()
        {
            EntityID = "Floor";
        }
    }
}