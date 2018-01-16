using System.ComponentModel.Composition;

using Gemini.Framework;
using Gemini.Framework.Menus;

namespace P3D_Legacy.MapEditor.Modules.Startup
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
	    [Export]
	    public static MenuItemGroupDefinition SceneViewer3D = new MenuItemGroupDefinition(
	        Gemini.Modules.MainMenu.MenuDefinitions.ViewMenu, 0);

		public override void Initialize()
		{
			MainWindow.Title = "P3D-Legacy Map Editor";
		}
	}
}