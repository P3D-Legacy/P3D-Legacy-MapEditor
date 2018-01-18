using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Utils
{
    public abstract class Camera : ICamera
    {
        protected float NearPlane { get; set; } = 0.01f;
        protected float FarPlane { get; set; } = 1000f;

        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public float FOV { get; set; } = 45f;

        protected virtual void CreateView()
        {
            var rotationMatrix = Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y);
            var lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            var cameraLookAt = Position + lookAtOffset;

            View = Matrix.CreateLookAt(Position, cameraLookAt, Vector3.Up);
        }

        protected virtual void CreateProjection(GraphicsDevice graphicsDevice)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV), graphicsDevice.Viewport.AspectRatio, NearPlane, FarPlane);
        }

        public abstract bool HandleMouse(MouseDevice mouse);

        public abstract bool HandleKeyboard(KeyboardDevice keyboard);

        public virtual void BeforeDraw(GraphicsDevice graphicsDevice)
        {
            CreateView();
            CreateProjection(graphicsDevice);
        }
    }
}