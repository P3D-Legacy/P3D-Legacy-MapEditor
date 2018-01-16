using System.ComponentModel.Composition;
using System.Threading.Tasks;

using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

using P3D_Legacy.MapEditor.Modules.SceneViewer3D.ViewModels;

namespace P3D_Legacy.MapEditor.Modules.SceneViewer3D.Commands
{
    [CommandHandler]
    public class ViewSceneViewerCommandHandler : CommandHandlerBase<ViewSceneViewerCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewSceneViewerCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.OpenDocument(new SceneViewModel());
            return TaskUtility.Completed;
        }
    }
}