using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Data.Models;
using P3D.Legacy.MapEditor.Primitives;

namespace P3D.Legacy.MapEditor.Components
{
    public class BaseModelSelector
    {
        public BaseModel SelectedModel { get; protected set; }
        protected float SelectedModelDistance { get; set; }

        protected BaseCamera Camera { get; }

        protected CubePrimitive Cube { get; }

        public BaseModelSelector(BaseCamera camera)
        {
            Camera = camera;
            Cube = new CubePrimitive();
        }

        public virtual void Draw(BasicEffect basicEffect)
        {
            if (SelectedModel != null)
            {
                Cube.Model = SelectedModel;
                Cube.Recalc();
                Cube.Draw(basicEffect, new Color(Color.LimeGreen, 0.75f));
            }
        }
    }
}