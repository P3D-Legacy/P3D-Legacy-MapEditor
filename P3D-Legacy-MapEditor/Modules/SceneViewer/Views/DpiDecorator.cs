using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace P3D.Legacy.MapEditor.Modules.SceneViewer.Views
{
    public class DpiDecorator : Decorator
    {
        // https://www.mesta-automation.com/tecniques-scaling-wpf-application/
        public DpiDecorator()
        {
            Loaded += (s, e) =>
            {
                var m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
                var dpiTransform = new ScaleTransform(1 / m.M11, 1 / m.M22);
                if (dpiTransform.CanFreeze)
                    dpiTransform.Freeze();
                LayoutTransform = dpiTransform;
            };
        }
    }
}
