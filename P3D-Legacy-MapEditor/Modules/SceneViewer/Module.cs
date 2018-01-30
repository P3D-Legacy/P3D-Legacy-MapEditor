using System.ComponentModel.Composition;
using System.Linq;

using Gemini.Framework;
using Gemini.Modules.Inspector;
using Gemini.Modules.Inspector.Conventions;

using P3D.Legacy.MapEditor.Modules.SceneViewer.Inspectors;
using P3D.Legacy.MapEditor.Modules.SceneViewer.ViewModels;

namespace P3D.Legacy.MapEditor.Modules.SceneViewer
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
	    private readonly IInspectorTool _inspectorTool;

	    [ImportingConstructor]
	    public Module(IInspectorTool inspectorTool)
        {
            _inspectorTool = inspectorTool;
        }

	    public override void Initialize()
	    {
	        DefaultPropertyInspectors.InspectorBuilders.Add(
	            new StandardPropertyEditorBuilder<float, FloatEditorViewModel>());
	    }

	    public override void PostInitialize()
	    {
            var sceneViewModel = Shell.Documents.OfType<SceneViewModel>().FirstOrDefault();
	        if (sceneViewModel != null)
	        {
                //_inspectorTool.SelectedObject = new InspectableObjectBuilder()
                //    .WithVector3Editor(sceneViewModel, x => x.CameraPosition)
                //    .WithFloatEditor(sceneViewModel, x => x.CameraMoveSpeed)
                //    .ToInspectableObject();
            }
        }
	}
}