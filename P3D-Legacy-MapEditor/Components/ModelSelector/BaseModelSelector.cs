using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Components.Camera;
using P3D.Legacy.MapEditor.Data.Models;
using P3D.Legacy.MapEditor.Primitives;

namespace P3D.Legacy.MapEditor.Components.ModelSelector
{
    public abstract class BaseModelSelector : IGameComponent, IUpdateable
    {
        private bool _enabled = true;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnEnabledChanged(this, EventArgs.Empty);
                }
            }
        }

        private int _updateOrder;
        public int UpdateOrder
        {
            get => _updateOrder;
            set
            {
                if (_updateOrder != value)
                {
                    _updateOrder = value;
                    OnUpdateOrderChanged(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;


        public BaseModel SelectedModel { get; protected set; }
        protected float SelectedModelDistance { get; set; }

        protected BaseCamera Camera { get; }

        protected CubePrimitive Cube { get; }

        protected BaseModelSelector(BaseCamera camera)
        {
            Camera = camera;
            Cube = new CubePrimitive();
        }

        public abstract void Initialize();

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(BasicEffect basicEffect)
        {
            if (SelectedModel != null)
            {
                Cube.Model = SelectedModel;
                Cube.Recalc();
                Cube.Draw(basicEffect, new Color(Color.LimeGreen, 0.75f));
            }
        }

        protected virtual void OnUpdateOrderChanged(object sender, EventArgs args)
            => UpdateOrderChanged?.Invoke(sender, args);

        protected virtual void OnEnabledChanged(object sender, EventArgs args)
            => EnabledChanged?.Invoke(sender, args);
    }
}