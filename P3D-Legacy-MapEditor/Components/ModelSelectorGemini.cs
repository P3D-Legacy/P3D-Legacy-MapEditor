using System.Linq;
using System.Windows.Input;

using P3D.Legacy.MapEditor.Modules.SceneViewer.Views;
using P3D.Legacy.MapEditor.Renders;

namespace P3D.Legacy.MapEditor.Components
{
    public class ModelSelectorGemini : BaseModelSelector
    {
        public ModelSelectorGemini(SceneView userControl, BaseCamera camera) : base(camera)
        {
            userControl.MouseMove += OnMouse;
            userControl.MouseLeftButtonUp += OnMouse;
            userControl.MouseLeftButtonDown += OnMouse;
            userControl.MouseRightButtonUp += OnMouse;
            userControl.MouseRightButtonDown += OnMouse;
            userControl.MouseWheel += OnMouseWheel;
        }

        private void OnMouse(object sender, MouseEventArgs e)
        {
            Update();
        }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        public void Update()
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