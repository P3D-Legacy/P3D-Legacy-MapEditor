using System;
using System.Linq;

using Microsoft.Xna.Framework;

using P3D.Legacy.MapEditor.Renders;

namespace P3D.Legacy.MapEditor.Components
{
    public class ModelSelectorMonoGame : BaseModelSelector, IGameComponent, IUpdateable, IComparable<GameComponent>
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
        
        public ModelSelectorMonoGame(BaseCamera camera) : base(camera) { }

        public void Initialize() { }

        public void Update(GameTime gameTime)
        {
            SelectedModel = null;
            SelectedModelDistance = float.MaxValue;

            foreach (var baseModel in BaseModelListRenderer.TotalModels.Where(m => m.Entity.Visible))
            {
                var distance = baseModel.BoundingBox.Intersects(Camera.GetMouseRay());
                if (distance.HasValue && distance.Value < SelectedModelDistance)
                {
                    SelectedModel = baseModel;
                    SelectedModelDistance = distance.Value;
                }
            }
        }


        protected virtual void OnUpdateOrderChanged(object sender, EventArgs args)
            => UpdateOrderChanged?.Invoke(sender, args);

        protected virtual void OnEnabledChanged(object sender, EventArgs args)
            => EnabledChanged?.Invoke(sender, args);

        public int CompareTo(GameComponent other) => other.UpdateOrder - UpdateOrder;
    }
}