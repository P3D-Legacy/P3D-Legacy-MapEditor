using System;
using System.ComponentModel.Composition;
using Gemini.Framework;

using Microsoft.Xna.Framework;

using P3D_Legacy.MapEditor.Modules.SceneViewer3D.Views;

namespace P3D_Legacy.MapEditor.Modules.SceneViewer3D.ViewModels
{
    [Export(typeof(SceneViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
	public class SceneViewModel : Document
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

        public SceneViewModel()
        {
            DisplayName = "3D Scene";
        }

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
	}
}