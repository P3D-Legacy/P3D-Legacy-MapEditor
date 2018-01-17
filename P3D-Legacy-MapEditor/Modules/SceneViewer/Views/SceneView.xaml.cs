using System;
using System.Windows.Controls;
using System.Windows.Input;

using Caliburn.Micro;

using Gemini.Modules.MonoGame.Controls;
using Gemini.Modules.Output;

using Microsoft.Xna.Framework;

using P3D_Legacy.MapEditor.Modules.SceneViewer.Renders;


namespace P3D_Legacy.MapEditor.Modules.SceneViewer.Views
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

        private RenderMode RenderMode { get; set; }
        private IRender CurrentRender
        {
            get
            {
                switch (RenderMode)
                {
                    case RenderMode.Mode2D:
                        return Render2D;

                    case RenderMode.Mode3D:
                        return Render3D;

                    default:
                        return Render3D;
                }
            }
        }
        
        private IRender Render2D { get; }
        private IRender Render3D { get; }


        public SceneView()
        {
            InitializeComponent();

            _output = IoC.Get<IOutput>();

            Render2D = new Render2D(this);
            Render3D = new Render3D(this);
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
            CurrentRender.Initialize(e.GraphicsDevice);
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
            if (CurrentRender.HandleMouse(e.MouseDevice))
                GraphicsControl.Invalidate();
        }

        // We use the left mouse button to do exclusive capture of the mouse so we can drag and drag
        // to rotate the cube without ever leaving the control
        private void OnGraphicsControlHwndLButtonDown(object sender, MouseEventArgs e)
        {
            if (CurrentRender.HandleMouse(e.MouseDevice))
                GraphicsControl.Invalidate();

            _output.AppendLine("Mouse left button down");
            GraphicsControl.CaptureMouse();
            GraphicsControl.Focus();
        }

        private void OnGraphicsControlHwndLButtonUp(object sender, MouseEventArgs e)
        {
            if (CurrentRender.HandleMouse(e.MouseDevice))
                GraphicsControl.Invalidate();

            _output.AppendLine("Mouse left button up");
            GraphicsControl.ReleaseMouseCapture();
        }

        private void OnGraphicsControlKeyDown(object sender, KeyEventArgs e)
        {
            if(CurrentRender.HandleKeyboard(e.KeyboardDevice))
                GraphicsControl.Invalidate();

            _output.AppendLine("Key down: " + e.Key);
        }

        private void OnGraphicsControlKeyUp(object sender, KeyEventArgs e)
        {
            if (CurrentRender.HandleKeyboard(e.KeyboardDevice))
                GraphicsControl.Invalidate();

            _output.AppendLine("Key up: " + e.Key);
        }

        private void OnGraphicsControlHwndMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (CurrentRender.HandleMouse(e.MouseDevice))
                GraphicsControl.Invalidate();

            _output.AppendLine("Mouse wheel: " + e.Delta);
        }
    }
}
