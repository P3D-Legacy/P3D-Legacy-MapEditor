using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using P3D.Legacy.MapEditor.Modules.SceneViewer.Views;

namespace P3D.Legacy.MapEditor.Components
{
    public class Camera3DGemini : BaseCamera
    {
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetCursorPos(int X, int Y);


        private SceneView UserControl { get; }
        private FrameworkElement GraphicsControl => UserControl.GraphicsControl;

        private bool _skipHandleMouseMove;

        public Camera3DGemini(SceneView userControl) : base(userControl.GraphicsDevice)
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
            var state = new MouseState(
                (int) position.X, (int) position.Y, 0,
                mouse.LeftButton == MouseButtonState.Pressed ? ButtonState.Pressed : ButtonState.Released,
                mouse.MiddleButton == MouseButtonState.Pressed ? ButtonState.Pressed : ButtonState.Released,
                mouse.RightButton == MouseButtonState.Pressed ? ButtonState.Pressed : ButtonState.Released,
                mouse.XButton1 == MouseButtonState.Pressed ? ButtonState.Pressed : ButtonState.Released,
                mouse.XButton2 == MouseButtonState.Pressed ? ButtonState.Pressed : ButtonState.Released);

            if (_skipHandleMouseMove)
            {
                _previousMouseState = _currentMouseState;
                _currentMouseState = state;
                _skipHandleMouseMove = false;
                return false;
            }

            UpdateMouse(state);

            return true;
        }

        private bool HandleKeyboard(KeyboardDevice keyboard)
        {
            _velocity = keyboard.IsKeyDown(Key.LeftShift) ? VelocityFast : VelocityStandard;

            var direction = Vector3.Zero;

            if (keyboard.IsKeyDown(Key.W) || keyboard.IsKeyDown(Key.Up))
            {
                if (!_forwardsPressed)
                {
                    _forwardsPressed = true;
                    _currentVelocity.Z = 0.0f;
                }

                direction.Z += 1.0f;
            }
            else
                _forwardsPressed = false;

            if (keyboard.IsKeyDown(Key.S) || keyboard.IsKeyDown(Key.Down))
            {
                if (!_backwardsPressed)
                {
                    _backwardsPressed = true;
                    _currentVelocity.Z = 0.0f;
                }

                direction.Z -= 1.0f;
            }
            else
                _backwardsPressed = false;

            if (keyboard.IsKeyDown(Key.D) || keyboard.IsKeyDown(Key.Right))
            {
                if (!_strafeRightPressed)
                {
                    _strafeRightPressed = true;
                    _currentVelocity.X = 0.0f;
                }

                direction.X += 1.0f;
            }
            else
                _strafeRightPressed = false;

            if (keyboard.IsKeyDown(Key.A) || keyboard.IsKeyDown(Key.Left))
            {
                if (!_strafeLeftPressed)
                {
                    _strafeLeftPressed = true;
                    _currentVelocity.X = 0.0f;
                }

                direction.X -= 1.0f;
            }
            else
                _strafeLeftPressed = false;

            if (keyboard.IsKeyDown(Key.Q) || keyboard.IsKeyDown(Key.Space))
            {
                if (!_lshiftPressed)
                {
                    _lshiftPressed = true;
                    _currentVelocity.Y = 0.0f;
                }

                direction.Y += 1.0f;
            }
            else
                _lshiftPressed = false;

            if (keyboard.IsKeyDown(Key.Z) || keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (!_spacePressed)
                {
                    _spacePressed = true;
                    _currentVelocity.Y = 0.0f;
                }

                direction.Y -= 1.0f;
            }
            else
                _spacePressed = false;

            UpdatePosition(ref direction, 0.025f);

            return true;
        }

        protected override void SetMousePosition(int x, int y)
        {
            var pos = GraphicsControl.PointToScreen(new System.Windows.Point(x, y));

            SetCursorPos((int) pos.X, (int) pos.Y);
            _skipHandleMouseMove = true;
        }

        protected override void SetMouseCursorVisible(bool visible)
        {
            System.Windows.Input.Mouse.OverrideCursor = visible ? Cursors.Arrow : Cursors.None;
        }

        protected override Microsoft.Xna.Framework.Point GetScreenCenter()
        {
            var relativeCenter = GraphicsControl.TranslatePoint(
                new System.Windows.Point(GraphicsControl.ActualWidth * 0.5D, GraphicsControl.ActualHeight * 0.5D), GraphicsControl);
            return new Microsoft.Xna.Framework.Point((int) relativeCenter.X, (int) relativeCenter.Y);
        }
    }
}