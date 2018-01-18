using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Utils
{
    public interface ICamera
    {
        Matrix View { get; }
        Matrix Projection { get; }

        Vector3 Position { get; set; }

        bool HandleMouse(MouseDevice mouse);
        bool HandleKeyboard(KeyboardDevice keyboard);

        void BeforeDraw(GraphicsDevice graphicsDevice);
    }

    //https://github.com/nilllzz/Game-Dev-Common/blob/5d3e220b7fe20b51d6046ebb7fc95e0a07ab4b9d/src/GameDevCommon/Rendering/Camera.cs
    public abstract class CameraNilllzz : ICamera
    {
        protected float NearPlane { get; set; } = 0.01f;
        protected float FarPlane { get; set; } = 1000f;

        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }

        public Vector3 Position { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }

        public float FOV { get; set; } = 45f;

        protected virtual void CreateView()
        {
            var up = Vector3.Up;
            var forward = Vector3.Forward;

            // yaw:
            {
                forward.Normalize();
                forward = Vector3.Transform(forward, Matrix.CreateFromAxisAngle(up, Yaw));
            }

            // pitch:
            {
                forward.Normalize();
                var left = Vector3.Cross(up, forward);
                left.Normalize();

                forward = Vector3.Transform(forward, Matrix.CreateFromAxisAngle(left, -Pitch));
                up = Vector3.Transform(up, Matrix.CreateFromAxisAngle(left, -Pitch));
            }

            // roll:
            {
                up.Normalize();
                var left = Vector3.Cross(up, forward);
                left.Normalize();
                up = Vector3.Transform(up, Matrix.CreateFromAxisAngle(forward, Roll));
            }

            View = Matrix.CreateLookAt(Position, forward + Position, up);
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