using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Caliburn.Micro;

using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Commands;

using Microsoft.Win32;

using P3D_Legacy.MapEditor.Modules.SceneViewer.ViewModels;

namespace P3D_Legacy.MapEditor.Modules.SceneViewer.Commands
{
    [CommandHandler]
    public class OpenFileCommandHandler : CommandHandlerBase<OpenFileCommandDefinition>
    {
        private readonly IShell _shell;
        private readonly SceneViewModel[] _editorProviders;

        [ImportingConstructor]
        public OpenFileCommandHandler(IShell shell, [ImportMany] SceneViewModel[] editorProviders)
        {
            _shell = shell;
            _editorProviders = editorProviders;
        }

        public override async Task Run(Command command)
        {
            var dialog = new OpenFileDialog();

            dialog.Filter = "All Supported Files|" + string.Join(";", _editorProviders
                .SelectMany(x => x.FileTypes).Select(x => "*" + x.FileExtension));

            dialog.Filter += "|" + string.Join("|", _editorProviders
                .SelectMany(x => x.FileTypes)
                .Select(x => x.Name + "|*" + x.FileExtension));

            if (dialog.ShowDialog() == true)
            {
                // Not sure what's going on here
                foreach (var editorProvider in _editorProviders)
                {
                    if (!editorProvider.Handles(dialog.FileName))
                        continue;

                    _shell.OpenDocument(await GetEditor(editorProvider, dialog.FileName));
                }
            }
        }

        internal static Task<SceneViewModel> GetEditor(SceneViewModel editor, string path)
        {
            var viewAware = (IViewAware) editor;
            viewAware.ViewAttached += (sender, e) =>
            {
                var frameworkElement = (FrameworkElement)e.View;

                async void loadedHandler(object sender2, RoutedEventArgs e2)
                {
                    frameworkElement.Loaded -= loadedHandler;
                    await editor.Load(string.Format(path));
                }

                frameworkElement.Loaded += loadedHandler;
            };

            return Task.FromResult(editor);
        }
    }
}
