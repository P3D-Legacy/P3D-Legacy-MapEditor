using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D.Legacy.MapEditor.Utils
{
    public class Camera3DMonoGame : Camera
    {
        public GraphicsDevice GraphicsDevice { get; }

        private Vector2 _lastMouseLocation;
        private Vector2 _cameraRotationBuffer;

        public Camera3DMonoGame(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;

            Position = new Vector3(1, 1, 2f);
            Rotation = new Vector3(0.5f, 3.5f, 0);
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds * 20F;

            var moveVector = Vector3.Zero;
            if (InputManager.IsCurrentKeyPressed(Keys.Up) || InputManager.IsCurrentKeyPressed(Keys.W))
                moveVector += new Vector3(0, 0, 1);
            if (InputManager.IsCurrentKeyPressed(Keys.Down) || InputManager.IsCurrentKeyPressed(Keys.S))
                moveVector += new Vector3(0, 0, -1);
            if (InputManager.IsCurrentKeyPressed(Keys.Left) || InputManager.IsCurrentKeyPressed(Keys.A))
                moveVector += new Vector3(1, 0, 0);
            if (InputManager.IsCurrentKeyPressed(Keys.Right) || InputManager.IsCurrentKeyPressed(Keys.D))
                moveVector += new Vector3(-1, 0, 0);

            var cameraRotation = Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y);
            var rotatedVector = Vector3.Transform(moveVector, cameraRotation);

            if (InputManager.IsCurrentKeyPressed(Keys.Space) || InputManager.IsCurrentKeyPressed(Keys.Q))
                rotatedVector += Vector3.Up;
            if (InputManager.IsCurrentKeyPressed(Keys.LeftShift) || InputManager.IsCurrentKeyPressed(Keys.Z))
                rotatedVector += Vector3.Down;

            Position += dt * rotatedVector;


            if (InputManager.CurrentMouseState.RightButton == ButtonState.Pressed)
            {
                var center = new Vector2(GraphicsDevice.Viewport.Width * 0.5f, GraphicsDevice.Viewport.Height * 0.5f);
                var mouseDelta = InputManager.MousePosition.ToVector2() - center;


                // Android set mouse to (0, 0)
                if (center != -mouseDelta)
                {
                    Microsoft.Xna.Framework.Input.Mouse.SetPosition((int)center.X, (int)center.Y);

                    _cameraRotationBuffer -= 0.1f * mouseDelta * dt;
                }

                if (_cameraRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                    _cameraRotationBuffer.Y = _cameraRotationBuffer.Y - (_cameraRotationBuffer.Y - MathHelper.ToRadians(-75.0f));
                if (_cameraRotationBuffer.Y > MathHelper.ToRadians(75.0f))
                    _cameraRotationBuffer.Y = _cameraRotationBuffer.Y - (_cameraRotationBuffer.Y - MathHelper.ToRadians(75.0f));

                Rotation = new Vector3(-MathHelper.Clamp(_cameraRotationBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)), MathHelper.WrapAngle(_cameraRotationBuffer.X), 0);
                //Position = new Vector3(-MathHelper.Clamp(_cameraPositionBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)), MathHelper.WrapAngle(_cameraPositionBuffer.X), 0);
            }

            _lastMouseLocation = InputManager.MousePosition.ToVector2();

            if (InputManager.IsCurrentKeyPressed(Keys.G))
            {
                var t = GetMouseInWorld();
            }
        }

        public void MoveTo(Vector3 pos, Vector3 rot)
        {
            Position = pos;
            Rotation = rot;
        }

        private Vector3 PreviewMove(Vector3 amount)
        {
            var rotate = Matrix.CreateRotationY(Position.Y);

            var movement = new Vector3(amount.X, amount.Y, amount.Z);
            movement = Vector3.Transform(movement, rotate);

            return Position + movement;
        }

        private void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);
        }


        public override Vector2 GetMouse() => _lastMouseLocation;

        public override Vector3 GetMouseInWorld()
        {
            return GetMouseWorldPosition();

            var mouse = InputManager.MousePosition;
            var inverseView = Matrix.Invert(View * Projection);
            return new Vector3(Vector2.Transform(new Vector2(mouse.X, mouse.Y), inverseView), 0);
        }
        public Vector3 GetMouseWorldPosition()
        {
            var mouse = InputManager.MousePosition;

            var nearSource = new Vector3(mouse.X, mouse.Y, 0f);
            var farSource = new Vector3(mouse.X, mouse.Y, 1f);
            var nearPoint = GraphicsDevice.Viewport.Unproject(nearSource, Projection, View, Matrix.Identity);
            var farPoint = GraphicsDevice.Viewport.Unproject(farSource, Projection, View, Matrix.Identity);

            // Create a ray from the near clip plane to the far clip plane.
            var direction = farPoint - nearPoint;
            direction.Normalize();

            // Create a ray.
            var ray = new Ray(nearPoint, direction);

            // Calculate the ray-plane intersection point.
            var n = new Vector3(0f, 1f, 0f);
            var p = new Plane(n, 0f);

            // Calculate distance of intersection point from r.origin.
            var denominator = Vector3.Dot(p.Normal, ray.Direction);
            var numerator = Vector3.Dot(p.Normal, ray.Position) + p.D;
            var t = -(numerator / denominator);

            // Calculate the picked position on the y = 0 plane.
            var pickedPosition = nearPoint + direction * t;
            pickedPosition.Y = -direction.Y * t;

            return pickedPosition;
        }
    }
}