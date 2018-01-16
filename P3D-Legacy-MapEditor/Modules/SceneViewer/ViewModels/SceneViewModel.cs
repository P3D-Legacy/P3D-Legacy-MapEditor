using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

using Microsoft.Xna.Framework;

using P3D_Legacy.MapEditor.Data;
using P3D_Legacy.MapEditor.Modules.SceneViewer.Views;
using P3D_Legacy.MapEditor.Properties;
using P3D_Legacy.MapEditor.World;

namespace P3D_Legacy.MapEditor.Modules.SceneViewer.ViewModels
{
    [Export(typeof(SceneViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
	public class SceneViewModel : PersistedDocument
    {
        private ISceneView _sceneView;

        public override bool ShouldReopenOnStart => true;

        private Vector3 _position;
	    public Vector3 Position
	    {
            get => _position;
	        set
            {
                _position = value;
                NotifyOfPropertyChange(() => Position);

                _sceneView?.Invalidate();
            }
	    }

        public IEnumerable<EditorFileType> FileTypes
        {
            get { yield return new EditorFileType(Resources.MapFile, ".dat"); }
        }
        public bool Handles(string path) => Path.GetExtension(path) == ".dat";

        protected override void OnViewLoaded(object view)
        {
            _sceneView = view as ISceneView;
            base.OnViewLoaded(view);
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                if (GetView() is IDisposable view)
                    view.Dispose();
            }

            base.OnDeactivate(close);
        }

        public override void SaveState(BinaryWriter writer) => writer.Write(FilePath);
        public override void LoadState(BinaryReader reader) => Load(reader.ReadString());


        private LevelInfo _levelInfo;
        private string _originalText;
        protected override Task DoNew()
        {
            _originalText = string.Empty;
            LoadMap();
            return TaskUtility.Completed;
        }

        protected override Task DoLoad(string filePath)
        {
            _originalText = File.ReadAllText(filePath);
            LoadMap();
            return TaskUtility.Completed;
        }
        protected override Task DoSave(string filePath)
        {
            var newText = "";
            File.WriteAllText(filePath, newText);
            _originalText = newText;
            return TaskUtility.Completed;
        }
        private void LoadMap()
        {
            DisplayName = FileName;

            if(!string.IsNullOrEmpty(_originalText))
                _levelInfo = LevelLoader.Load(_originalText);
        }
    }
}