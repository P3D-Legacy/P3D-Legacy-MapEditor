using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace P3D.Legacy.MapEditor.Components
{
    public class Camera3DMonoGame : BaseCamera, IGameComponent, IUpdateable, IComparable<GameComponent>
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

        private Game Game { get; }

        private KeyboardState _currentKeyboardState;

        public Camera3DMonoGame(Game game) : base(game.GraphicsDevice) => Game = game;

        public void Initialize()
        {
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();
        }

        public void Update(GameTime gameTime)
        {
            UpdateMouse(Mouse.GetState());
            UpdateKeyboard(Keyboard.GetState(), (float) gameTime.ElapsedGameTime.TotalSeconds * 2.5f);
        }

        private void UpdateKeyboard(KeyboardState keyboardState, float elapsed)
        {
            _currentKeyboardState = keyboardState;

            _velocity = _currentKeyboardState.IsKeyDown(Keys.LeftShift) ? VelocityFast : VelocityStandard;

            var direction = Vector3.Zero;

            if (_currentKeyboardState.IsKeyDown(Keys.W) || _currentKeyboardState.IsKeyDown(Keys.Up))
            {
                if (!_forwardsPressed)
                {
                    _forwardsPressed = true;
                    _currentVelocity.Z = 0.0f;
                }

                direction.Z += 1.0f;
            }
            else
                _forwardsPressed = false;

            if (_currentKeyboardState.IsKeyDown(Keys.S) || _currentKeyboardState.IsKeyDown(Keys.Down))
            {
                if (!_backwardsPressed)
                {
                    _backwardsPressed = true;
                    _currentVelocity.Z = 0.0f;
                }

                direction.Z -= 1.0f;
            }
            else
                _backwardsPressed = false;

            if (_currentKeyboardState.IsKeyDown(Keys.D) || _currentKeyboardState.IsKeyDown(Keys.Right))
            {
                if (!_strafeRightPressed)
                {
                    _strafeRightPressed = true;
                    _currentVelocity.X = 0.0f;
                }

                direction.X += 1.0f;
            }
            else
                _strafeRightPressed = false;

            if (_currentKeyboardState.IsKeyDown(Keys.A) || _currentKeyboardState.IsKeyDown(Keys.Left))
            {
                if (!_strafeLeftPressed)
                {
                    _strafeLeftPressed = true;
                    _currentVelocity.X = 0.0f;
                }

                direction.X -= 1.0f;
            }
            else
                _strafeLeftPressed = false;

            if (_currentKeyboardState.IsKeyDown(Keys.Q) || _currentKeyboardState.IsKeyDown(Keys.Space))
            {
                if (!_lshiftPressed)
                {
                    _lshiftPressed = true;
                    _currentVelocity.Y = 0.0f;
                }

                direction.Y += 1.0f;
            }
            else
                _lshiftPressed = false;

            if (_currentKeyboardState.IsKeyDown(Keys.Z) || _currentKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                if (!_spacePressed)
                {
                    _spacePressed = true;
                    _currentVelocity.Y = 0.0f;
                }

                direction.Y -= 1.0f;
            }
            else
                _spacePressed = false;

            UpdatePosition(ref direction, elapsed);
        }

        protected override void SetMousePosition(int x, int y)
        {
            Mouse.SetPosition(x, y);
        }

        protected override void SetMouseCursorVisible(bool visible)
        {
            Game.IsMouseVisible = visible;
        }

        protected override Point GetScreenCenter()
        {
            return new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        }

        protected virtual void OnUpdateOrderChanged(object sender, EventArgs args)
            => UpdateOrderChanged?.Invoke(sender, args);

        protected virtual void OnEnabledChanged(object sender, EventArgs args) 
            => EnabledChanged?.Invoke(sender, args);

        public int CompareTo(GameComponent other) => other.UpdateOrder - UpdateOrder;
    }
}