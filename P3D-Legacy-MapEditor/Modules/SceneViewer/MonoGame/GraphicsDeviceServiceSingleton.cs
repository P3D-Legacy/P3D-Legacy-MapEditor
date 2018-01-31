using System;

using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Modules.SceneViewer.MonoGame
{
    public abstract class GraphicsDeviceServiceSingleton : IGraphicsDeviceService
    {
        public virtual GraphicsDevice GraphicsDevice { get; protected set; }

        public abstract event EventHandler<EventArgs> DeviceCreated;
        public abstract event EventHandler<EventArgs> DeviceDisposing;
        public abstract event EventHandler<EventArgs> DeviceReset;
        public abstract event EventHandler<EventArgs> DeviceResetting;

        public abstract void Release(bool disposing);
    }
}
