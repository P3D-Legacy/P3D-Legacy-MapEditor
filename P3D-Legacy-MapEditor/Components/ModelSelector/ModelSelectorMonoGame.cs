using System.Linq;

using Microsoft.Xna.Framework;

using P3D.Legacy.MapEditor.Components.Camera;
using P3D.Legacy.MapEditor.Renders;

namespace P3D.Legacy.MapEditor.Components.ModelSelector
{
    public class ModelSelectorMonoGame : BaseModelSelector, IUpdateable
    {
       public ModelSelectorMonoGame(BaseCamera camera) : base(camera) { }

        public override void Initialize() { }

        public override void Update(GameTime gameTime)
        {
            SelectedModel = null;
            SelectedModelDistance = float.MaxValue;

            foreach (var baseModel in BaseModelListRenderer.TotalModels.Where(m => m.Entity.Visible))
            {
                var distance = baseModel.BoundingBox.Intersects(Camera.GetMouseRay());
                if (distance.HasValue && distance.Value < SelectedModelDistance)
                {
                    SelectedModel = baseModel;
                    SelectedModelDistance = distance.Value;
                }
            }
        }
    }
}