using System.Windows.Input;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Modules.SceneViewer.Renders
{
    public interface IRender
    {
        void Initialize(GraphicsDevice graphicsDevice);

        void Draw(GraphicsDevice graphicsDevice);
    }
}
