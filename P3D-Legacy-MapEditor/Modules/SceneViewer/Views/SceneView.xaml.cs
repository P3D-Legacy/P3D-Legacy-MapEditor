using System;
using System.Windows;
using System.Windows.Input;
using Gemini.Modules.D3D.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.Legacy.MapEditor.Components;
using P3D.Legacy.MapEditor.Modules.SceneViewer.MonoGame;
using P3D.Legacy.MapEditor.Modules.SceneViewer.ViewModels;
using P3D.Legacy.MapEditor.Renders;
using P3D.Legacy.MapEditor.Utils;

namespace P3D.Legacy.MapEditor.Modules.SceneViewer.Views
{
    /// <summary>
    /// Interaction logic for SceneView.xaml
    /// </summary>
    public partial class SceneView : IDisposable
    {
        public GraphicsDevice GraphicsDevice { get; private set; }
        private SceneViewModel SceneViewModel => (SceneViewModel) DataContext;
        private Render CurrentRender { get; set; }
        private Camera3DGemini Camera { get; set; }
        private ModelSelectorGemini ModelSelector { get; set; }


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
        }

        /// <summary>
        /// Invoked after either control has created its graphics device.
        /// </summary>
        private void GraphicsControl_OnLoadContent(object sender, GraphicsDeviceEventArgs e)
        {
            GraphicsDevice = e.GraphicsDevice;

            GraphicsControl.AlwaysRefresh = true;

            Camera = new Camera3DGemini(this);
            Camera.UpdateProjectionMatrix(45f, GraphicsDevice.Viewport.AspectRatio, 0.01f, 1000f);

            ModelSelector = new ModelSelectorGemini(this, Camera);

            CurrentRender = new Render(GraphicsDevice, Camera, ModelSelector, SceneViewModel.LevelInfo);
            CurrentRender.Initialize();
        }

        /// <summary>
        /// Invoked when our second control is ready to render.
        /// </summary>
        private void GraphicsControl_OnDraw(object sender, DrawEventArgs e)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            CurrentRender.Draw();
        }

        private void GraphicsControl_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
            => GraphicsControl.Focus();

        private void GraphicsControl_OnViewportChanged(object sender, SizeChangedInfo e)
        {
            Camera.UpdateProjectionMatrix(45f, GraphicsDevice.Viewport.AspectRatio, 0.01f, 1000f);
            CurrentRender.ViewportChanged();
        }
    }
}
