using System.Linq;
using System.Windows.Input;

using Microsoft.Xna.Framework;

using P3D.Legacy.MapEditor.Components.Camera;
using P3D.Legacy.MapEditor.Modules.SceneViewer.Views;
using P3D.Legacy.MapEditor.Renders;

namespace P3D.Legacy.MapEditor.Components.ModelSelector
{
    public class ModelSelectorGemini : BaseModelSelector
    {
        public ModelSelectorGemini(SceneView userControl, BaseCamera camera) : base(camera)
        {
            /*
            userControl.MouseMove += OnMouse;
            userControl.MouseLeftButtonUp += OnMouse;
            userControl.MouseLeftButtonDown += OnMouse;
            userControl.MouseRightButtonUp += OnMouse;
            userControl.MouseRightButtonDown += OnMouse;
            userControl.MouseWheel += OnMouseWheel;
            */
        }

        public override void Initialize()
        {

        }

        /*
        private void OnMouse(object sender, MouseEventArgs e)
        {
            Update();
        }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }
        */

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