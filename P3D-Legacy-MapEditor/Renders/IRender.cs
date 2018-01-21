using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Renders
{
    public interface IRender
    {
        void Initialize(GraphicsDevice graphicsDevice);

        void Draw(GraphicsDevice graphicsDevice);
    }
}
