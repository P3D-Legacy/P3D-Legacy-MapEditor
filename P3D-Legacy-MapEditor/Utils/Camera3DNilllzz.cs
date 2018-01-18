using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xna.Framework;

namespace P3D.Legacy.MapEditor.Utils
{
    public class Camera3DNilllzz : CameraNilllzz
    {
        private UserControl UserControl { get; }

        private System.Windows.Point _previousMousePosition;

        public Camera3DNilllzz(UserControl userControl)
        {
            UserControl = userControl;

            Position = new Vector3(1, 1, 2f);
            Yaw = 0.5f;
            Pitch = -0.5f;
        }

        public override bool HandleMouse(MouseDevice mouse)
        {
            var position = mouse.GetPosition(UserControl);

            if (mouse.RightButton == MouseButtonState.Pressed)
            {
                Yaw -= (float)(position.X - _previousMousePosition.X) * 0.01f;
                Pitch -= (float)(position.Y - _previousMousePosition.Y) * 0.01f;

                _previousMousePosition = position;

                return true;
            }

            _previousMousePosition = position;
            return false;
        }

        public override bool HandleKeyboard(KeyboardDevice keyboard)
        {
            var moveVector = new Vector3(0, 0, 0);
            if (keyboard.IsKeyDown(Key.Up) || keyboard.IsKeyDown(Key.W))
                moveVector += new Vector3(0, 0, -1);
            if (keyboard.IsKeyDown(Key.Down) || keyboard.IsKeyDown(Key.S))
                moveVector += new Vector3(0, 0, 1);
            if (keyboard.IsKeyDown(Key.Left) || keyboard.IsKeyDown(Key.A))
                moveVector += new Vector3(-1, 0, 0);
            if (keyboard.IsKeyDown(Key.Right) || keyboard.IsKeyDown(Key.D))
                moveVector += new Vector3(1, 0, 0);
            if (keyboard.IsKeyDown(Key.Space) || keyboard.IsKeyDown(Key.Q))
                moveVector += new Vector3(0, 1, 0);
            if (keyboard.IsKeyDown(Key.LeftShift) || keyboard.IsKeyDown(Key.Z))
                moveVector += new Vector3(0, -1, 0);

            var cameraRotation = Matrix.CreateFromYawPitchRoll(Yaw, Pitch, Roll);
            var rotatedVector = Vector3.Transform(moveVector, cameraRotation);
            Position += 0.1f * rotatedVector;

            return true;
        }
    }
}