using System;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Xna.Framework.Graphics;

using P3D_Legacy.MapEditor.Data;
using P3D_Legacy.MapEditor.Modules.SceneViewer.ViewModels;

namespace P3D_Legacy.MapEditor.Modules.SceneViewer.Renders
{
    public class Render2D : IRender
    {
        private UserControl UserControl { get; }
        private SceneViewModel SceneViewModel => UserControl.DataContext as SceneViewModel;
        private LevelInfo LevelInfo => SceneViewModel.LevelInfo;

        public Render2D(UserControl userControl)
        {
            UserControl = userControl;
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            throw new NotImplementedException();
        }

        public bool HandleMouse(MouseDevice mouse)
        {
            throw new NotImplementedException();
        }

        public bool HandleKeyboard(KeyboardDevice keyboard)
        {
            throw new NotImplementedException();
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            throw new NotImplementedException();
        }
    }
}
