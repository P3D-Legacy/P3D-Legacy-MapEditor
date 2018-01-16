using System.ComponentModel.Composition;

using Gemini.Framework.Menus;

using P3D_Legacy.MapEditor.Modules.SceneViewer.Commands;

namespace P3D_Legacy.MapEditor.Modules.SceneViewer
{
    public static class MenuDefinitions
    {
        // We don't need a menu entry if we will open it via file creation
        //[Export]
        //public static MenuItemDefinition ViewSceneViewerMenuItem = new CommandMenuItemDefinition<ViewSceneViewerCommandDefinition>(
        //    Startup.Module.SceneViewer3D, 1);
    }
}