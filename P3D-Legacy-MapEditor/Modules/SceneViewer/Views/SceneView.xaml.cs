using System;
using System.Windows;
using System.Windows.Input;
using Gemini.Modules.D3D.Controls;

using Microsoft.Xna.Framework;

using P3D.Legacy.MapEditor.Components;
using P3D.Legacy.MapEditor.Components.Camera;
using P3D.Legacy.MapEditor.Components.ModelSelector;
using P3D.Legacy.MapEditor.Modules.SceneViewer.MonoGame;
using P3D.Legacy.MapEditor.Modules.SceneViewer.ViewModels;
using P3D.Legacy.MapEditor.Renders;

namespace P3D.Legacy.MapEditor.Modules.SceneViewer.Views
{
    /// <summary>
    /// Interaction logic for SceneView.xaml
    /// </summary>
    public partial class SceneView : IDisposable
    {
        private SceneViewModel SceneViewModel => (SceneViewModel) DataContext;
        private Render Render { get; set; }
        private Camera3DGemini Camera { get; set; }

        public SceneView()
        {
            InitializeComponent();
        }

        public void Invalidate()
        {
            GraphicsControl.Invalidate();
        }

        public void Dispose()
        {
            GraphicsControl.Dispose();
            GameDispose();
        }

        /// <summary>
        /// Invoked after either control has created its graphics device.
        /// </summary>
        private void GraphicsControl_OnLoadContent(object sender, GraphicsDeviceEventArgs e)
        {
            GraphicsDevice = e.GraphicsDevice;

            Camera = new Camera3DGemini(this);
            Camera.UpdateProjectionMatrix(45f, GraphicsDevice.Viewport.AspectRatio, 0.01f, 1000f);
            Components.Add(Camera);

            var modelSelector = new ModelSelectorMonoGame(Camera);
            Components.Add(modelSelector);

            Render = new Render(GraphicsDevice, Camera, modelSelector, SceneViewModel.LevelInfo);
            Components.Add(Render);

            Components.Add(new DebugComponent(GraphicsDevice));

            Run();
            GraphicsControl.AlwaysRefresh = true;
            IsFixedTimeStep = false;
        }

        /// <summary>
        /// Invoked when our second control is ready to render.
        /// </summary>
        private void GraphicsControl_OnDraw(object sender, DrawEventArgs e)
        {
            Tick();
        }

        private void GraphicsControl_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
            => GraphicsControl.Focus();

        private void GraphicsControl_OnViewportChanged(object sender, SizeChangedInfo e)
        {
            Camera.UpdateProjectionMatrix(45f, GraphicsDevice.Viewport.AspectRatio, 0.01f, 1000f);
            Render.ViewportChanged();
        }

        private void SceneViewDraw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
        }

        private void SceneViewUpdate(GameTime gameTime)
        {

        }
    }
}
