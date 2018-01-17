using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D_Legacy.MapEditor.Data;
using P3D_Legacy.MapEditor.Modules.SceneViewer.ViewModels;
using P3D_Legacy.MapEditor.Primitives;

namespace P3D_Legacy.MapEditor.Modules.SceneViewer.Renders
{
    public class Render3D : IRender
    {
        private UserControl UserControl { get; }
        private SceneViewModel SceneViewModel => UserControl.DataContext as SceneViewModel;
        private LevelInfo LevelInfo => SceneViewModel.LevelInfo;

        System.Windows.Point _previousMousePosition;
        float leftrightRot = MathHelper.PiOver2;
        float updownRot = -MathHelper.Pi / 10.0f;
        const float rotationSpeed = 0.3f;

        private readonly CubePrimitive _cube;

        public Render3D(UserControl userControl)
        {
            UserControl = userControl;

            _cube = new CubePrimitive();
        }
        
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            // Create our 3D cube object
            _cube.Initialize(graphicsDevice);
        }

        public bool HandleMouse(MouseDevice mouse)
        {
            var amount = 0.1f;
            var position = mouse.GetPosition(UserControl);

            if (mouse.RightButton == MouseButtonState.Pressed)
            {
                var xDifference = position.X - _previousMousePosition.X;
                var yDifference = position.Y - _previousMousePosition.Y;
                leftrightRot -= (float)(rotationSpeed * xDifference * amount);
                updownRot -= (float)(rotationSpeed * yDifference * amount);

                _previousMousePosition = position;
                return true;
            }

            _previousMousePosition = position;
            return false;
        }

        public bool HandleKeyboard(KeyboardDevice keyboard)
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
            if (keyboard.IsKeyDown(Key.Q))
                moveVector += new Vector3(0, 1, 0);
            if (keyboard.IsKeyDown(Key.Z))
                moveVector += new Vector3(0, -1, 0);

            var cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            var rotatedVector = Vector3.Transform(moveVector * 0.01f, cameraRotation);
            SceneViewModel.CameraPosition += SceneViewModel.CameraMoveSpeed * rotatedVector;

            return true;
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            var cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);

            var cameraOriginalTarget = new Vector3(0, 0, -1);
            var cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            var cameraFinalTarget = SceneViewModel.CameraPosition + cameraRotatedTarget;

            var cameraOriginalUpVector = new Vector3(0, 1, 0);
            var cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            var world = Matrix.Identity;
            var view = Matrix.CreateLookAt(SceneViewModel.CameraPosition, cameraFinalTarget, cameraRotatedUpVector);
            var projection = Matrix.CreatePerspectiveFieldOfView(1, graphicsDevice.Viewport.AspectRatio, 1, 10);

            _cube.Draw(world, view, projection, Color.LightGreen);
        }
    }
}
