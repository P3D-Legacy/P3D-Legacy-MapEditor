using System.ComponentModel.Composition;

using Gemini.Framework.Menus;

using P3D_Legacy.MapEditor.Modules.SceneViewer3D.Commands;

namespace P3D_Legacy.MapEditor.Modules.SceneViewer3D
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition ViewSceneViewerMenuItem = new CommandMenuItemDefinition<ViewSceneViewerCommandDefinition>(
            Startup.Module.SceneViewer3D, 1);
    }
}