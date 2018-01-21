using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

using Gemini.Modules.MonoGame.Controls;

using Microsoft.Xna.Framework;

using P3D.Legacy.MapEditor.Modules.SceneViewer.Views;

namespace P3D.Legacy.MapEditor.Utils
{
    public class Camera3DGemini : Camera
    {
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetCursorPos(int X, int Y);


        private SceneView UserControl { get; }
        private DrawingSurface GraphicsControl => UserControl.GraphicsControl;

        private Vector2 _lastMouseLocation;
        private Vector2 _cameraRotationBuffer;
        private bool _skipHandleMouseMove;

        public Camera3DGemini(SceneView userControl)
        {
            UserControl = userControl;
            UserControl.MouseMove += OnMouse;
            UserControl.MouseLeftButtonUp += OnMouse;
            UserControl.MouseLeftButtonDown += OnMouse;
            UserControl.MouseRightButtonUp += OnMouse;
            UserControl.MouseRightButtonDown += OnMouse;
            UserControl.MouseWheel += OnMouseWheel;
            UserControl.KeyUp += OnKeyboard;
            UserControl.KeyDown += OnKeyboard;
            UserControl.PreviewKeyDown += OnKeyboard;

            Position = new Vector3(1, 1, 2f);
            Rotation = new Vector3(0.5f, 3.5f, 0);
        }

        // Invoked when the mouse moves over the second viewport
        private void OnMouse(object sender, MouseEventArgs e)
        {
            if (HandleMouse(e.MouseDevice))
                UserControl.Invalidate();
        }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (HandleMouse(e.MouseDevice))
                UserControl.Invalidate();
        }
        private void OnKeyboard(object sender, KeyEventArgs e)
        {
            if (HandleKeyboard(e.KeyboardDevice))
                UserControl.Invalidate();
        }

        private bool HandleMouse(MouseDevice mouse)
        {
            var position = mouse.GetPosition(GraphicsControl);
            _lastMouseLocation = new Vector2((float) position.X, (float) position.Y);

            if (mouse.RightButton == MouseButtonState.Pressed)
            {
                System.Windows.Input.Mouse.OverrideCursor = Cursors.None;

                if (_skipHandleMouseMove)
                {
                    _skipHandleMouseMove = false;
                    return false;
                }

                var graphicsControlRelativeCenter = GraphicsControl.TranslatePoint(
                    new System.Windows.Point(GraphicsControl.ActualWidth * 0.5D, GraphicsControl.ActualHeight * 0.5D), GraphicsControl);
                var absoluteScreenPosition = GraphicsControl.PointToScreen(graphicsControlRelativeCenter);

                SetCursorPos((int) absoluteScreenPosition.X, (int) absoluteScreenPosition.Y);
                _skipHandleMouseMove = true;

                var delta = position - graphicsControlRelativeCenter;
                if (delta.Length > 100)
                    delta = new Vector(0, 0);
                var deltaVector = new Vector2((float) delta.X, (float) delta.Y);

                _cameraRotationBuffer -= 0.01f * deltaVector;
                if (_cameraRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                    _cameraRotationBuffer.Y = _cameraRotationBuffer.Y - (_cameraRotationBuffer.Y - MathHelper.ToRadians(-75.0f));
                if (_cameraRotationBuffer.Y > MathHelper.ToRadians(75.0f))
                    _cameraRotationBuffer.Y = _cameraRotationBuffer.Y - (_cameraRotationBuffer.Y - MathHelper.ToRadians(75.0f));

                Rotation = new Vector3(-MathHelper.Clamp(_cameraRotationBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)), MathHelper.WrapAngle(_cameraRotationBuffer.X), 0);

                return true;
            }
            System.Windows.Input.Mouse.OverrideCursor = Cursors.Arrow;

            return false;
        }

        private bool HandleKeyboard(KeyboardDevice keyboard)
        {
            var moveVector = Vector3.Zero;
            if (keyboard.IsKeyDown(Key.Up) || keyboard.IsKeyDown(Key.W))
                moveVector += Vector3.Backward;
            if (keyboard.IsKeyDown(Key.Down) || keyboard.IsKeyDown(Key.S))
                moveVector += Vector3.Forward;
            if (keyboard.IsKeyDown(Key.Left) || keyboard.IsKeyDown(Key.A))
                moveVector += Vector3.Right;
            if (keyboard.IsKeyDown(Key.Right) || keyboard.IsKeyDown(Key.D))
                moveVector += Vector3.Left;

            var cameraRotation = Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y);
            var rotatedVector = Vector3.Transform(moveVector, cameraRotation);

            if (keyboard.IsKeyDown(Key.Space) || keyboard.IsKeyDown(Key.Q))
                rotatedVector += Vector3.Up;
            if (keyboard.IsKeyDown(Key.LeftShift) || keyboard.IsKeyDown(Key.Z))
                rotatedVector += Vector3.Down;

            Position += 0.1f * rotatedVector;

            return true;
        }

        public override Vector2 GetMouse() => _lastMouseLocation;

        public override Vector3 GetMouseInWorld()
        {
            return Vector3.Zero;
        }
    }
}