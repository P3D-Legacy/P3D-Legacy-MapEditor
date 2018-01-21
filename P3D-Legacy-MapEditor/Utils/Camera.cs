using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Data.Models;

namespace P3D.Legacy.MapEditor.Utils
{
    public abstract class Camera// : ICamera
    {
        protected float NearPlane { get; set; } = 0.01f;
        protected float FarPlane { get; set; } = 1000f;

        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public float FOV { get; set; } = 45f;

        public virtual void UpdateView()
        {
            var rotationMatrix = Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y);
            var lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            var cameraLookAt = Position + lookAtOffset;

            View = Matrix.CreateLookAt(Position, cameraLookAt, Vector3.Up);
        }

        public virtual void UpdateProjection(GraphicsDevice graphicsDevice)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV), graphicsDevice.Viewport.AspectRatio, NearPlane, FarPlane);
        }

        public virtual void BeforeDraw(GraphicsDevice graphicsDevice)
        {
            UpdateView();
            UpdateProjection(graphicsDevice);
        }

        public abstract Vector2 GetMouse();
        public abstract Vector3 GetMouseInWorld();

        public Ray CalculateMouseRay(Matrix world, Matrix view, Matrix projection, Viewport viewport)
        {
            var mouse = GetMouse();
            var nearPoint = viewport.Unproject(new Vector3(mouse.X, mouse.Y, 0.0f), projection, view, world);
            var farPoint = viewport.Unproject(new Vector3(mouse.X, mouse.Y, 1.0f), projection, view, world);

            var direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }
        public float? MouseIntersectDistance(BoundingBox box, Matrix world, Matrix view, Matrix projection, Viewport viewport)
        {
            return CalculateMouseRay(world, view, projection, viewport).Intersects(box);
        }
        public bool MouseIntersects(BaseModel model, Matrix world, Matrix view, Matrix projection, Viewport viewport)
        {
            return MouseIntersectDistance(model.BoundingBox, world, view, projection, viewport) != null;
        }
    }
}