using System.ComponentModel.Composition;
using System.Drawing;
using System.Threading;
using Gemini.Framework;
using P3D.Legacy.MapEditor.Modules.SceneViewer.ViewModels;
using P3D.Legacy.MapEditor.Properties;

namespace P3D.Legacy.MapEditor.Modules.Startup
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        //[Export]
        //public static MenuItemGroupDefinition SceneViewer3D = new MenuItemGroupDefinition(
        //    Gemini.Modules.MainMenu.MenuDefinitions.ViewMenu, 0);

        public override void Initialize()
        {
            MainWindow.Title = Resources.Title;

            // No idea how to use Gemini's resource manager
            var icon = (Icon) new System.Resources.ResourceManager(typeof(Resources)).GetObject("P3D_Legacy");
            MainWindow.Icon = icon.ToImageSource();
            icon?.Dispose();
        }
    }
}