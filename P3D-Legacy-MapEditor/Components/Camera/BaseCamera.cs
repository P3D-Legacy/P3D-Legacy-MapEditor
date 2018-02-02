using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace P3D.Legacy.MapEditor.Components.Camera
{
    // TODO: remove velocity and keep it simple
    // http://www.dhpoware.com/demos/xnaFirstPersonCamera.html
    public abstract class BaseCamera : IGameComponent
    {
        #region Default Values

        public const float DEFAULT_FOVX = 45.0f;
        public const float DEFAULT_ZNEAR = 0.1f;
        public const float DEFAULT_ZFAR = 1000.0f;

        protected static Vector3 WORLD_X_AXIS = Vector3.UnitX;
        protected static Vector3 WORLD_Y_AXIS = Vector3.UnitY;
        protected static Vector3 WORLD_Z_AXIS = Vector3.UnitZ;

        protected const float DEFAULT_ACCELERATION_X = 8.0f;
        protected const float DEFAULT_ACCELERATION_Y = 8.0f;
        protected const float DEFAULT_ACCELERATION_Z = 8.0f;
        protected const float DEFAULT_VELOCITY_X = 2.0f;
        protected const float DEFAULT_VELOCITY_Y = 2.0f;
        protected const float DEFAULT_VELOCITY_Z = 2.0f;
        protected const float DEFAULT_FAST_MULTIPLIER = 2.0f;
        protected const float DEFAULT_MOUSE_SMOOTHING_SENSITIVITY = 0.5f;
        protected const float DEFAULT_SPEED_ROTATION = 0.2f;
        protected const int MOUSE_SMOOTHING_CACHE_SIZE = 10;

        #endregion

        protected float _fovx = DEFAULT_FOVX;
        protected float _aspectRatio;
        protected float _znear = DEFAULT_ZNEAR;
        protected float _zfar = DEFAULT_ZFAR;

        protected Vector3 _eye = Vector3.Zero;
        protected Vector3 _target = Vector3.Zero;
        protected Vector3 _targetYAxis = Vector3.UnitY;

        protected Vector3 _velocity;

        protected bool _cameraLocked;
        protected bool _forwardsPressed;
        protected bool _backwardsPressed;
        protected bool _lshiftPressed;
        protected bool _spacePressed;
        protected bool _strafeRightPressed;
        protected bool _strafeLeftPressed;
        protected int _mouseIndex;
        protected float _mouseSmoothingSensitivity = DEFAULT_MOUSE_SMOOTHING_SENSITIVITY;
        protected Vector2[] _mouseMovement = { new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f) };
        protected Vector2[] _mouseSmoothingCache = new Vector2[MOUSE_SMOOTHING_CACHE_SIZE];
        protected Vector2 _smoothedMouseMovement;
        protected MouseState _currentMouseState;
        protected MouseState _previousMouseState;

        #region Properties

        public Vector3 Acceleration { get; set; } = new Vector3(DEFAULT_ACCELERATION_X, DEFAULT_ACCELERATION_Y, DEFAULT_ACCELERATION_Z);

        protected Vector3 _currentVelocity;
        public Vector3 CurrentVelocity => _currentVelocity;

        public bool EnableMouseSmoothing { get; set; } = true;

        protected float _accumHeadingDegrees;
        public float HeadingDegrees => -_accumHeadingDegrees;

        protected float _accumPitchDegrees;
        public float PitchDegrees => -_accumPitchDegrees;

        protected Quaternion _orientation = Quaternion.Identity;
        public Quaternion Orientation => _orientation;

        public Vector3 Position
        {
            get => _eye;

            set
            {
                _eye = value;
                UpdateViewMatrix();
            }
        }

        public Matrix ProjectionMatrix { get; protected set; }

        public float RotationSpeed { get; set; } = DEFAULT_SPEED_ROTATION;

        public Vector3 VelocityStandard { get; set; } = new Vector3(DEFAULT_VELOCITY_X, DEFAULT_VELOCITY_Y, DEFAULT_VELOCITY_Z);

        public Vector3 VelocityFast { get; set; } = new Vector3(DEFAULT_VELOCITY_X, DEFAULT_VELOCITY_Y, DEFAULT_VELOCITY_Z) * DEFAULT_FAST_MULTIPLIER;

        protected Vector3 _viewDir = Vector3.Forward;
        public Vector3 ViewDirection => _viewDir;

        protected Matrix _viewMatrix = Matrix.Identity;
        public Matrix ViewMatrix => _viewMatrix;

        protected Vector3 _xAxis = Vector3.UnitX;
        public Vector3 XAxis => _xAxis;

        protected Vector3 _yAxis = Vector3.UnitY;
        public Vector3 YAxis => _yAxis;

        protected Vector3 _zAxis = Vector3.UnitZ;
        public Vector3 ZAxis => _zAxis;

        #endregion

        #region Public Methods

        protected GraphicsDevice GraphicsDevice;

        protected BaseCamera(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;

            // Initialize camera state.
            _velocity = VelocityStandard;

            // Setup perspective projection matrix.
            _aspectRatio = graphicsDevice.Viewport.AspectRatio;
            UpdateProjectionMatrix(_fovx, _aspectRatio, _znear, _zfar);
        }

        public abstract void Initialize();

        public void LookAt(Vector3 target) => LookAt(_eye, target, _yAxis);
        public void LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            _eye = eye;
            _target = target;

            _zAxis = eye - target;
            _zAxis.Normalize();

            _viewDir.X = -_zAxis.X;
            _viewDir.Y = -_zAxis.Y;
            _viewDir.Z = -_zAxis.Z;

            Vector3.Cross(ref up, ref _zAxis, out _xAxis);
            _xAxis.Normalize();

            Vector3.Cross(ref _zAxis, ref _xAxis, out _yAxis);
            _yAxis.Normalize();
            _xAxis.Normalize();

            _viewMatrix.M11 = _xAxis.X;
            _viewMatrix.M21 = _xAxis.Y;
            _viewMatrix.M31 = _xAxis.Z;
            Vector3.Dot(ref _xAxis, ref eye, out _viewMatrix.M41);
            _viewMatrix.M41 = -_viewMatrix.M41;

            _viewMatrix.M12 = _yAxis.X;
            _viewMatrix.M22 = _yAxis.Y;
            _viewMatrix.M32 = _yAxis.Z;
            Vector3.Dot(ref _yAxis, ref eye, out _viewMatrix.M42);
            _viewMatrix.M42 = -_viewMatrix.M42;

            _viewMatrix.M13 = _zAxis.X;
            _viewMatrix.M23 = _zAxis.Y;
            _viewMatrix.M33 = _zAxis.Z;
            Vector3.Dot(ref _zAxis, ref eye, out _viewMatrix.M43);
            _viewMatrix.M43 = -_viewMatrix.M43;

            _viewMatrix.M14 = 0.0f;
            _viewMatrix.M24 = 0.0f;
            _viewMatrix.M34 = 0.0f;
            _viewMatrix.M44 = 1.0f;

            _accumPitchDegrees = MathHelper.ToDegrees((float)Math.Asin(_viewMatrix.M23));
            _accumHeadingDegrees = MathHelper.ToDegrees((float)Math.Atan2(_viewMatrix.M13, _viewMatrix.M33));

            Quaternion.CreateFromRotationMatrix(ref _viewMatrix, out _orientation);
        }

        /// <summary>
        /// Moves the camera by dx world units to the left or right; dy
        /// world units upwards or downwards; and dz world units forwards
        /// or backwards.
        /// </summary>
        /// <param name="dx">Distance to move left or right.</param>
        /// <param name="dy">Distance to move up or down.</param>
        /// <param name="dz">Distance to move forwards or backwards.</param>
        public void Move(float dx, float dy, float dz)
        {
            // Calculate the forwards direction. Can't just use the
            // camera's view direction as doing so will cause the camera to
            // move more slowly as the camera's view approaches 90 degrees
            // straight up and down.

            var forwards = Vector3.Normalize(Vector3.Cross(WORLD_Y_AXIS, _xAxis));

            _eye += _xAxis * dx;
            _eye += WORLD_Y_AXIS * dy;
            _eye += forwards * dz;

            Position = _eye;
        }

        public void UpdateProjectionMatrix(float fovx, float aspect, float znear, float zfar)
        {
            _fovx = fovx;
            _aspectRatio = aspect;
            _znear = znear;
            _zfar = zfar;

            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_fovx), _aspectRatio, _znear, _zfar);
        }

        public void Rotate(float headingDegrees, float pitchDegrees)
        {
            headingDegrees = -headingDegrees;
            pitchDegrees = -pitchDegrees;

            _accumPitchDegrees += pitchDegrees;

            if (_accumPitchDegrees > 90.0f)
            {
                pitchDegrees = 90.0f - (_accumPitchDegrees - pitchDegrees);
                _accumPitchDegrees = 90.0f;
            }

            if (_accumPitchDegrees < -90.0f)
            {
                pitchDegrees = -90.0f - (_accumPitchDegrees - pitchDegrees);
                _accumPitchDegrees = -90.0f;
            }

            _accumHeadingDegrees += headingDegrees;

            if (_accumHeadingDegrees > 360.0f)
                _accumHeadingDegrees -= 360.0f;

            if (_accumHeadingDegrees < -360.0f)
                _accumHeadingDegrees += 360.0f;

            var heading = MathHelper.ToRadians(headingDegrees);
            var pitch = MathHelper.ToRadians(pitchDegrees);

            // Rotate the camera about the world Y axis.
            if (heading != 0.0f)
            {
                Quaternion.CreateFromAxisAngle(ref WORLD_Y_AXIS, heading, out var rotation);
                Quaternion.Concatenate(ref rotation, ref _orientation, out _orientation);
            }

            // Rotate the camera about its local X axis.
            if (pitch != 0.0f)
            {
                Quaternion.CreateFromAxisAngle(ref WORLD_X_AXIS, pitch, out var rotation);
                Quaternion.Concatenate(ref _orientation, ref rotation, out _orientation);
            }

            UpdateViewMatrix();
        }

        public Ray GetMouseRay()
        {
            var mouse = _currentMouseState;

            var nearPoint = new Vector3(mouse.Position.ToVector2(), 0);
            var farPoint = new Vector3(mouse.Position.ToVector2(), 1);

            nearPoint = GraphicsDevice.Viewport.Unproject(nearPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);
            farPoint = GraphicsDevice.Viewport.Unproject(farPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);

            var direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        #endregion

        protected abstract void SetMousePosition(int x, int y);
        protected abstract void SetMouseCursorVisible(bool visible);
        protected abstract Point GetScreenCenter();

        #region Private Methods

        /// <summary>
        /// Filters the mouse movement based on a weighted sum of mouse
        /// movement from previous frames.
        /// <para>
        /// For further details see:
        ///  Nettle, Paul "Smooth Mouse Filtering", flipCode's Ask Midnight column.
        ///  http://www.flipcode.com/cgi-bin/fcarticles.cgi?show=64462
        /// </para>
        /// </summary>
        /// <param name="x">Horizontal mouse distance from window center.</param>
        /// <param name="y">Vertice mouse distance from window center.</param>
        protected void PerformMouseFiltering(Vector2 mouse)
        {
            // Shuffle all the entries in the cache.
            // Newer entries at the front. Older entries towards the back.
            for (var i = _mouseSmoothingCache.Length - 1; i > 0; --i)
            {
                _mouseSmoothingCache[i].X = _mouseSmoothingCache[i - 1].X;
                _mouseSmoothingCache[i].Y = _mouseSmoothingCache[i - 1].Y;
            }

            // Store the current mouse movement entry at the front of cache.
            _mouseSmoothingCache[0].X = mouse.X;
            _mouseSmoothingCache[0].Y = mouse.Y;

            var averageX = 0.0f;
            var averageY = 0.0f;
            var averageTotal = 0.0f;
            var currentWeight = 1.0f;

            // Filter the mouse movement with the rest of the cache entries.
            // Use a weighted average where newer entries have more effect than
            // older entries (towards the back of the cache).
            for (var i = 0; i < _mouseSmoothingCache.Length; ++i)
            {
                averageX += _mouseSmoothingCache[i].X * currentWeight;
                averageY += _mouseSmoothingCache[i].Y * currentWeight;
                averageTotal += 1.0f * currentWeight;
                currentWeight *= _mouseSmoothingSensitivity;
            }

            // Calculate the new smoothed mouse movement.
            _smoothedMouseMovement.X = averageX / averageTotal;
            _smoothedMouseMovement.Y = averageY / averageTotal;
        }

        /// <summary>
        /// Averages the mouse movement over a couple of frames to smooth out
        /// the mouse movement.
        /// </summary>
        /// <param name="x">Horizontal mouse distance from window center.</param>
        /// <param name="y">Vertice mouse distance from window center.</param>
        protected void PerformMouseSmoothing(Vector2 mouse)
        {
            _mouseMovement[_mouseIndex].X = mouse.X;
            _mouseMovement[_mouseIndex].Y = mouse.Y;

            _smoothedMouseMovement.X = (_mouseMovement[0].X + _mouseMovement[1].X) * 0.5f;
            _smoothedMouseMovement.Y = (_mouseMovement[0].Y + _mouseMovement[1].Y) * 0.5f;

            _mouseIndex ^= 1;
            _mouseMovement[_mouseIndex].X = 0.0f;
            _mouseMovement[_mouseIndex].Y = 0.0f;
        }

        /// <summary>
        /// Dampens the rotation by applying the rotation speed to it.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation in degrees.</param>
        /// <param name="pitchDegrees">X axis rotation in degrees.</param>
        protected void RotateSmoothly(float headingDegrees, float pitchDegrees)
        {
            headingDegrees *= RotationSpeed;
            pitchDegrees *= RotationSpeed;

            Rotate(headingDegrees, pitchDegrees);
        }

        protected void UpdateMouse(MouseState mouseState)
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = mouseState;

            if (_currentMouseState.RightButton == ButtonState.Pressed)
            {
                SetMouseCursorVisible(false);

                var center = GetScreenCenter();
                var delta = new Vector2(center.X - _currentMouseState.X, center.Y - _currentMouseState.Y);

                SetMousePosition(center.X, center.Y);

                if (!_cameraLocked)
                    _cameraLocked = true;
                else
                {
                    if (EnableMouseSmoothing)
                    {
                        PerformMouseFiltering(delta);
                        PerformMouseSmoothing(_smoothedMouseMovement);
                    }
                    else
                        _smoothedMouseMovement = delta;
                }
            }
            else
            {
                SetMouseCursorVisible(true);
                _cameraLocked = false;
                _smoothedMouseMovement = Vector2.Zero;
            }

            RotateSmoothly(_smoothedMouseMovement.X, _smoothedMouseMovement.Y);
        }

        /// <summary>
        /// Moves the camera based on player input.
        /// </summary>
        /// <param name="direction">Direction moved.</param>
        /// <param name="elapsedTimeSec">Elapsed game time.</param>
        protected void UpdatePosition(ref Vector3 direction, float elapsedTimeSec)
        {
            if (_currentVelocity.LengthSquared() != 0.0f)
            {
                // Only move the camera if the velocity vector is not of zero
                // length. Doing this guards against the camera slowly creeping
                // around due to floating point rounding errors.

                var displacement = _currentVelocity * elapsedTimeSec + 0.5f * Acceleration * elapsedTimeSec * elapsedTimeSec;

                // Floating point rounding errors will slowly accumulate and
                // cause the camera to move along each axis. To prevent any
                // unintended movement the displacement vector is clamped to
                // zero for each direction that the camera isn't moving in.
                // Note that the UpdateVelocity() method will slowly decelerate
                // the camera's velocity back to a stationary state when the
                // camera is no longer moving along that direction. To account
                // for this the camera's current velocity is also checked.

                if (direction.X == 0.0f && Math.Abs(_currentVelocity.X) < 1e-6f)
                    displacement.X = 0.0f;

                if (direction.Y == 0.0f && Math.Abs(_currentVelocity.Y) < 1e-6f)
                    displacement.Y = 0.0f;

                if (direction.Z == 0.0f && Math.Abs(_currentVelocity.Z) < 1e-6f)
                    displacement.Z = 0.0f;

                Move(displacement.X, displacement.Y, displacement.Z);
            }

            // Continuously update the camera's velocity vector even if the
            // camera hasn't moved during this call. When the camera is no
            // longer being moved the camera is decelerating back to its
            // stationary state.

            UpdateVelocity(ref direction, elapsedTimeSec);
        }

        /// <summary>
        /// Updates the camera's velocity based on the supplied movement
        /// direction and the elapsed time (since this method was last
        /// called). The movement direction is the in the range [-1,1].
        /// </summary>
        /// <param name="direction">Direction moved.</param>
        /// <param name="elapsedTimeSec">Elapsed game time.</param>
        protected void UpdateVelocity(ref Vector3 direction, float elapsedTimeSec)
        {
            if (direction.X != 0.0f)
            {
                // Camera is moving along the x axis.
                // Linearly accelerate up to the camera's max speed.

                _currentVelocity.X += direction.X * Acceleration.X * elapsedTimeSec;

                if (_currentVelocity.X > _velocity.X)
                    _currentVelocity.X = _velocity.X;
                else if (_currentVelocity.X < -_velocity.X)
                    _currentVelocity.X = -_velocity.X;
            }
            else
            {
                // Camera is no longer moving along the x axis.
                // Linearly decelerate back to stationary state.

                if (_currentVelocity.X > 0.0f)
                {
                    if ((_currentVelocity.X -= Acceleration.X * elapsedTimeSec) < 0.0f)
                        _currentVelocity.X = 0.0f;
                }
                else
                {
                    if ((_currentVelocity.X += Acceleration.X * elapsedTimeSec) > 0.0f)
                        _currentVelocity.X = 0.0f;
                }
            }

            if (direction.Y != 0.0f)
            {
                // Camera is moving along the y axis. There are two cases here:
                // jumping and crouching. When jumping we're always applying a
                // negative acceleration to simulate the force of gravity.
                // However when crouching we apply a positive acceleration and
                // rely more on the direction.

                _currentVelocity.Y += direction.Y * Acceleration.Y * elapsedTimeSec;

                if (_currentVelocity.Y > _velocity.Y)
                    _currentVelocity.Y = _velocity.Y;
                else if (_currentVelocity.Y < -_velocity.Y)
                    _currentVelocity.Y = -_velocity.Y;
            }
            else
            {
                // Camera is no longer moving along the y axis.
                // Linearly decelerate back to stationary state.

                if (_currentVelocity.Y > 0.0f)
                {
                    if ((_currentVelocity.Y -= Acceleration.Y * elapsedTimeSec) < 0.0f)
                        _currentVelocity.Y = 0.0f;
                }
                else
                {
                    if ((_currentVelocity.Y += Acceleration.Y * elapsedTimeSec) > 0.0f)
                        _currentVelocity.Y = 0.0f;
                }
            }

            if (direction.Z != 0.0f)
            {
                // Camera is moving along the z axis.
                // Linearly accelerate up to the camera's max speed.

                _currentVelocity.Z += direction.Z * Acceleration.Z * elapsedTimeSec;

                if (_currentVelocity.Z > _velocity.Z)
                    _currentVelocity.Z = _velocity.Z;
                else if (_currentVelocity.Z < -_velocity.Z)
                    _currentVelocity.Z = -_velocity.Z;
            }
            else
            {
                // Camera is no longer moving along the z axis.
                // Linearly decelerate back to stationary state.

                if (_currentVelocity.Z > 0.0f)
                {
                    if ((_currentVelocity.Z -= Acceleration.Z * elapsedTimeSec) < 0.0f)
                        _currentVelocity.Z = 0.0f;
                }
                else
                {
                    if ((_currentVelocity.Z += Acceleration.Z * elapsedTimeSec) > 0.0f)
                        _currentVelocity.Z = 0.0f;
                }
            }
        }

        protected void UpdateViewMatrix()
        {
            Matrix.CreateFromQuaternion(ref _orientation, out _viewMatrix);

            _xAxis.X = _viewMatrix.M11;
            _xAxis.Y = _viewMatrix.M21;
            _xAxis.Z = _viewMatrix.M31;

            _yAxis.X = _viewMatrix.M12;
            _yAxis.Y = _viewMatrix.M22;
            _yAxis.Z = _viewMatrix.M32;

            _zAxis.X = _viewMatrix.M13;
            _zAxis.Y = _viewMatrix.M23;
            _zAxis.Z = _viewMatrix.M33;

            _viewMatrix.M41 = -Vector3.Dot(_xAxis, _eye);
            _viewMatrix.M42 = -Vector3.Dot(_yAxis, _eye);
            _viewMatrix.M43 = -Vector3.Dot(_zAxis, _eye);

            _viewDir.X = -_zAxis.X;
            _viewDir.Y = -_zAxis.Y;
            _viewDir.Z = -_zAxis.Z;
        }

        #endregion
    }
}
