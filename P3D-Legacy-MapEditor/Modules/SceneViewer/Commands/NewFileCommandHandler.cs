using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;

using Caliburn.Micro;

using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using Gemini.Modules.Shell.Commands;

using P3D.Legacy.MapEditor.Modules.SceneViewer.ViewModels;
using P3D.Legacy.MapEditor.Properties;

namespace P3D.Legacy.MapEditor.Modules.SceneViewer.Commands
{
    [CommandHandler]
    public class NewFileCommandHandler : ICommandListHandler<NewFileCommandListDefinition>
    {
        private class NewFileTag
        {
            public SceneViewModel EditorProvider { get; set; }
            public EditorFileType FileType { get; set; }
        }

        private int _newFileCounter = 1;

        private readonly IShell _shell;
        private readonly SceneViewModel[] _editorProviders;

        [ImportingConstructor]
        public NewFileCommandHandler(IShell shell, [ImportMany] SceneViewModel[] editorProviders)
        {
            _shell = shell;
            _editorProviders = editorProviders;
        }

        public void Populate(Command command, List<Command> commands)
        {
            foreach (var editorProvider in _editorProviders)
            foreach (var fileType in editorProvider.FileTypes)
                commands.Add(new Command(command.CommandDefinition)
                {
                    Text = fileType.Name,
                    Tag = new NewFileTag
                    {
                        EditorProvider = editorProvider,
                        FileType = fileType
                    }
                });
        }

        public Task Run(Command command)
        {
            var tag = (NewFileTag) command.Tag;
            var editor = tag.EditorProvider;

            var viewAware = (IViewAware) editor;
            viewAware.ViewAttached += (sender, e) =>
            {
                var frameworkElement = (FrameworkElement) e.View;

                async void LoadedHandler(object sender2, RoutedEventArgs e2)
                {
                    frameworkElement.Loaded -= LoadedHandler;
                    // TODO: Need to reference Gemini.dll resource file instead
                    await tag.EditorProvider.New(string.Format(Resources.FileNewUntitled, (_newFileCounter++) + tag.FileType.FileExtension));
                }

                frameworkElement.Loaded += LoadedHandler;
            };

            _shell.OpenDocument(editor);

            return TaskUtility.Completed;
        }
    }
}
