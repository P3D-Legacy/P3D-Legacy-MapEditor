using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace P3D_Legacy.MapEditor.Modules.SceneViewer.Renders
{
    public interface IRender
    {
        void PlaceObject(Vector3 position, object obj);
    }
}
