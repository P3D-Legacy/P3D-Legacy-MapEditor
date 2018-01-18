using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xna.Framework;

namespace P3D.Legacy.MapEditor.Utils
{
    public class Camera3D : Camera
    {
        private UserControl UserControl { get; }

        private System.Windows.Point _previousMousePosition;

        public Camera3D(UserControl userControl)
        {
            UserControl = userControl;

            Position = new Vector3(1, 1, 2f);
            Rotation = new Vector3(0.5f, 3.5f, 0);
        }

        public override bool HandleMouse(MouseDevice mouse)
        {
            var position = mouse.GetPosition(UserControl);

            if (mouse.RightButton == MouseButtonState.Pressed)
            {
                /*
                var delta = position - _previousMousePosition;
                var deltaVector = new Vector2((float) delta.X, (float) delta.Y);

                _cameraRotationBuffer -= 0.01f * deltaVector;
                if (_cameraRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                    _cameraRotationBuffer.Y = _cameraRotationBuffer.Y - (_cameraRotationBuffer.Y - MathHelper.ToRadians(-75.0f));
                if (_cameraRotationBuffer.Y > MathHelper.ToRadians(75.0f))
                    _cameraRotationBuffer.Y = _cameraRotationBuffer.Y - (_cameraRotationBuffer.Y - MathHelper.ToRadians(75.0f));

                Rotation = new Vector3(-MathHelper.Clamp(_cameraRotationBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)), MathHelper.WrapAngle(_cameraRotationBuffer.X), 0);
                */

                var xDifference = position.X - _previousMousePosition.X;
                var yDifference = position.Y - _previousMousePosition.Y;

                Rotation -= new Vector3(-(float)(yDifference * 0.01f), (float)(xDifference * 0.01f), 0);

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
                moveVector += new Vector3(0, 0, 1);
            if (keyboard.IsKeyDown(Key.Down) || keyboard.IsKeyDown(Key.S))
                moveVector += new Vector3(0, 0, -1);
            if (keyboard.IsKeyDown(Key.Left) || keyboard.IsKeyDown(Key.A))
                moveVector += new Vector3(1, 0, 0);
            if (keyboard.IsKeyDown(Key.Right) || keyboard.IsKeyDown(Key.D))
                moveVector += new Vector3(-1, 0, 0);
            if (keyboard.IsKeyDown(Key.Space) || keyboard.IsKeyDown(Key.Q))
                moveVector += new Vector3(0, 1, 0);
            if (keyboard.IsKeyDown(Key.LeftShift) || keyboard.IsKeyDown(Key.Z))
                moveVector += new Vector3(0, -1, 0);

            var cameraRotation = Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y);
            var rotatedVector = Vector3.Transform(moveVector, cameraRotation);
            Position += 0.1f * rotatedVector;

            return true;
        }
    }
}