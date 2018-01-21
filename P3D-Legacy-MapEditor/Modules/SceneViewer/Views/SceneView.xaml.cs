using System;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Gemini.Modules.MonoGame.Controls;
using Gemini.Modules.Output;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.Legacy.MapEditor.Modules.SceneViewer.Renders;
using P3D.Legacy.MapEditor.Modules.SceneViewer.ViewModels;
using P3D.Legacy.MapEditor.Utils;

namespace P3D.Legacy.MapEditor.Modules.SceneViewer.Views
{
    public enum RenderMode
    {
        None,
        Mode2D,
        Mode3D
    }

    /// <summary>
    /// Interaction logic for SceneView.xaml
    /// </summary>
    public partial class SceneView : UserControl, ISceneView, IDisposable
    {
        private readonly IOutput _output;

        public GraphicsDevice GraphicsDevice { get; private set; }
        private SceneViewModel SceneViewModel => (SceneViewModel) DataContext;
        private IRender CurrentRender { get; set; }
        private Camera Camera { get; set; }


        public SceneView()
        {
            InitializeComponent();

            _output = IoC.Get<IOutput>();
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
        private void OnGraphicsControlLoadContent(object sender, GraphicsDeviceEventArgs e)
        {
            GraphicsDevice = e.GraphicsDevice;

            Camera = new Camera3DGemini(this);
            CurrentRender = new Render(GraphicsDevice, Camera, SceneViewModel.LevelInfo);
            CurrentRender.Initialize(GraphicsDevice);
        }

        /// <summary>
        /// Invoked when our second control is ready to render.
        /// </summary>
        private void OnGraphicsControlDraw(object sender, DrawEventArgs e)
        {
            e.GraphicsDevice.Clear(Color.CornflowerBlue);

            CurrentRender.Draw(e.GraphicsDevice);
        }

        // Invoked when the mouse moves over the second viewport
        private void OnGraphicsControlMouseMove(object sender, MouseEventArgs e)
        {
            //if (CurrentRender.HandleMouse(e.MouseDevice))
            //    GraphicsControl.Invalidate();
        }

        // We use the left mouse button to do exclusive capture of the mouse so we can drag and drag
        // to rotate the cube without ever leaving the control
        private void OnGraphicsControlHwndLButtonDown(object sender, MouseEventArgs e)
        {
            //if (CurrentRender.HandleMouse(e.MouseDevice))
            //    GraphicsControl.Invalidate();

            _output.AppendLine("Mouse left button down");
        }

        private void OnGraphicsControlHwndLButtonUp(object sender, MouseEventArgs e)
        {
            //if (CurrentRender.HandleMouse(e.MouseDevice))
            //    GraphicsControl.Invalidate();

            _output.AppendLine("Mouse left button up");
        }

        private void OnGraphicsControlHwndRButtonDown(object sender, MouseEventArgs e)
        {
            //if (CurrentRender.HandleMouse(e.MouseDevice))
            //    GraphicsControl.Invalidate();

            _output.AppendLine("Mouse right button down");
            //GraphicsControl.CaptureMouse();
            GraphicsControl.Focus();
        }

        private void OnGraphicsControlHwndRButtonUp(object sender, MouseEventArgs e)
        {
            //if (CurrentRender.HandleMouse(e.MouseDevice))
            //    GraphicsControl.Invalidate();

            _output.AppendLine("Mouse right button up");
            //GraphicsControl.ReleaseMouseCapture();
        }

        private void OnGraphicsControlKeyDown(object sender, KeyEventArgs e)
        {
            //if(CurrentRender.HandleKeyboard(e.KeyboardDevice))
            //    GraphicsControl.Invalidate();

            _output.AppendLine("Key down: " + e.Key);
        }

        private void OnGraphicsControlKeyUp(object sender, KeyEventArgs e)
        {
            //if (CurrentRender.HandleKeyboard(e.KeyboardDevice))
            //    GraphicsControl.Invalidate();

            _output.AppendLine("Key up: " + e.Key);
        }

        private void OnGraphicsControlHwndMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //if (CurrentRender.HandleMouse(e.MouseDevice))
            //    GraphicsControl.Invalidate();

            _output.AppendLine("Mouse wheel: " + e.Delta);
        }
    }
}
