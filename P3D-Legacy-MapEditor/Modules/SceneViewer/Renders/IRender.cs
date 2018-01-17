using System.Windows.Input;

using Microsoft.Xna.Framework.Graphics;

namespace P3D_Legacy.MapEditor.Modules.SceneViewer.Renders
{
    public interface IRender
    {
        void Initialize(GraphicsDevice graphicsDevice);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mouse"></param>
        /// <returns>If true will invalidate GraphicsControl</returns>
        bool HandleMouse(MouseDevice mouse);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyboard"></param>
        /// <returns>If true will invalidate GraphicsControl</returns>
        bool HandleKeyboard(KeyboardDevice keyboard);

        void Draw(GraphicsDevice graphicsDevice);
    }
}
