/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using SlimDX;
using Control = System.Windows.Forms.Control;

namespace Gibbed.Avalanche.ModelViewer2
{
    internal class InputCamera : ICamera
    {
        private const float _DefaultAccelerationX = 8.0f;
        private const float _DefaultAccelerationY = 8.0f;
        private const float _DefaultAccelerationZ = 8.0f;
        private const float _DefaultMouseSmoothingSensitivity = 0.5f;
        private const float _DefaultSpeedFlightYaw = 100.0f;
        private const float _DefaultSpeedMouseWheel = 1.0f;
        private const float _DefaultSpeedOrbitRoll = 100.0f;
        private const float _DefaultSpeedRotation = 0.3f;
        private const float _DefaultVelocityX = 1.0f;
        private const float _DefaultVelocityY = 1.0f;
        private const float _DefaultVelocityZ = 1.0f;
        private const int _MouseSmoothingCacheSize = 10;

        private readonly Control _Control;
        private readonly BasicCamera _Camera;
        private readonly Clock _Clock;

        private bool _MovingAlongPosX;
        private bool _MovingAlongNegX;
        private bool _MovingAlongPosY;
        private bool _MovingAlongNegY;
        private bool _MovingAlongPosZ;
        private bool _MovingAlongNegZ;
        private int _SavedMousePosX;
        private int _SavedMousePosY;
        private int _MouseIndex;
        private float _RotationSpeed;
        private float _OrbitRollSpeed;
        private float _FlightYawSpeed;
        private float _MouseSmoothingSensitivity;
        private float _MouseWheelSpeed;
        private float _MouseWheelDelta;
        private Vector3 _Acceleration;
        private Vector3 _CurrentVelocity;
        private Vector3 _Velocity;
        private readonly Vector2[] _MouseMovement;
        private readonly Vector2[] _MouseSmoothingCache;
        private Vector2 _SmoothedMouseMovement;

        #region Public Methods

        /// <summary>
        /// Constructs a new instance of the CameraComponent class. The
        /// camera will have a spectator behavior, and will be initially
        /// positioned at the world origin looking down the world negative
        /// z axis. An initial perspective projection matrix is created
        /// as well as setting up initial key bindings to the actions.
        /// </summary>
        public InputCamera(Control control)
        {
            this._Control = control;
            this._Clock = new Clock();
            this._Clock.Start();

            this._Camera = new BasicCamera();
            this._Camera.CurrentBehavior = BasicCamera.Behavior.Spectator;

            this._MovingAlongPosX = false;
            this._MovingAlongNegX = false;
            this._MovingAlongPosY = false;
            this._MovingAlongNegY = false;
            this._MovingAlongPosZ = false;
            this._MovingAlongNegZ = false;

            this._SavedMousePosX = -1;
            this._SavedMousePosY = -1;

            this._RotationSpeed = _DefaultSpeedRotation;
            this._OrbitRollSpeed = _DefaultSpeedOrbitRoll;
            this._FlightYawSpeed = _DefaultSpeedFlightYaw;
            this._MouseWheelSpeed = _DefaultSpeedMouseWheel;
            this._MouseSmoothingSensitivity = _DefaultMouseSmoothingSensitivity;
            this._Acceleration = new Vector3(_DefaultAccelerationX, _DefaultAccelerationY, _DefaultAccelerationZ);
            this._Velocity = new Vector3(_DefaultVelocityX, _DefaultVelocityY, _DefaultVelocityZ);
            this._MouseSmoothingCache = new Vector2[_MouseSmoothingCacheSize];

            this._MouseIndex = 0;
            this._MouseMovement = new Vector2[2];
            this._MouseMovement[0].X = 0.0f;
            this._MouseMovement[0].Y = 0.0f;
            this._MouseMovement[1].X = 0.0f;
            this._MouseMovement[1].Y = 0.0f;

            this.Resized();
        }

        public void Resized()
        {
            float aspect = (float)this._Control.Width / this._Control.Height;

            this.Perspective(BasicCamera.DEFAULT_FOVX,
                aspect,
                0.01f,/*Camera.DEFAULT_ZNEAR,*/
                BasicCamera.DEFAULT_ZFAR);
        }

        public void Stop()
        {
            this._SmoothedMouseMovement.X = 0;
            this._SmoothedMouseMovement.Y = 0;
            this._MovingAlongPosX = false;
            this._MovingAlongNegX = false;
            this._MovingAlongPosY = false;
            this._MovingAlongNegY = false;
            this._MovingAlongPosZ = false;
            this._MovingAlongNegZ = false;
        }

        /// <summary>
        /// Builds a look at style viewing matrix.
        /// </summary>
        /// <param name="target">The target position to look at.</param>
        public void LookAt(Vector3 target)
        {
            this._Camera.LookAt(target);
        }

        /// <summary>
        /// Builds a look at style viewing matrix.
        /// </summary>
        /// <param name="eye">The camera position.</param>
        /// <param name="target">The target position to look at.</param>
        /// <param name="up">The up direction.</param>
        public void LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            this._Camera.LookAt(eye, target, up);
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
            this._Camera.Move(dx, dy, dz);
        }

        /// <summary>
        /// Moves the camera the specified distance in the specified direction.
        /// </summary>
        /// <param name="direction">Direction to move.</param>
        /// <param name="distance">How far to move.</param>
        public void Move(Vector3 direction, Vector3 distance)
        {
            this._Camera.Move(direction, distance);
        }

        /// <summary>
        /// Builds a perspective projection matrix based on a horizontal field
        /// of view.
        /// </summary>
        /// <param name="fovx">Horizontal field of view in degrees.</param>
        /// <param name="aspect">The viewport's aspect ratio.</param>
        /// <param name="znear">The distance to the near clip plane.</param>
        /// <param name="zfar">The distance to the far clip plane.</param>
        public void Perspective(float fovx, float aspect, float znear, float zfar)
        {
            this._Camera.Perspective(fovx, aspect, znear, zfar);
        }

        /// <summary>
        /// Rotates the camera. Positive angles specify counter clockwise
        /// rotations when looking down the axis of rotation towards the
        /// origin.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation in degrees.</param>
        /// <param name="pitchDegrees">X axis rotation in degrees.</param>
        /// <param name="rollDegrees">Z axis rotation in degrees.</param>
        public void Rotate(float headingDegrees, float pitchDegrees, float rollDegrees)
        {
            this._Camera.Rotate(headingDegrees, pitchDegrees, rollDegrees);
        }

        /// <summary>
        /// Updates the state of the CameraComponent class.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update.</param>
        public void Update(float elapsedTime, bool handleInput)
        {
            this.UpdateCamera(elapsedTime);
        }

        /// <summary>
        /// Undo any camera rolling by leveling the camera. When the camera is
        /// orbiting this method will cause the camera to become level with the
        /// orbit target.
        /// </summary>
        public void UndoRoll()
        {
            this._Camera.UndoRoll();
        }

        /// <summary>
        /// Zooms the camera. This method functions differently depending on
        /// the camera's current behavior. When the camera is orbiting this
        /// method will move the camera closer to or further away from the
        /// orbit target. For the other camera behaviors this method will
        /// change the camera's horizontal field of view.
        /// </summary>
        ///
        /// <param name="zoom">
        /// When orbiting this parameter is how far to move the camera.
        /// For the other behaviors this parameter is the new horizontal
        /// field of view.
        /// </param>
        /// 
        /// <param name="minZoom">
        /// When orbiting this parameter is the min allowed zoom distance to
        /// the orbit target. For the other behaviors this parameter is the
        /// min allowed horizontal field of view.
        /// </param>
        /// 
        /// <param name="maxZoom">
        /// When orbiting this parameter is the max allowed zoom distance to
        /// the orbit target. For the other behaviors this parameter is the max
        /// allowed horizontal field of view.
        /// </param>
        public void Zoom(float zoom, float minZoom, float maxZoom)
        {
            this._Camera.Zoom(zoom, minZoom, maxZoom);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines which way to move the camera based on player input.
        /// The returned values are in the range [-1,1].
        /// </summary>
        /// <param name="direction">The movement direction.</param>
        private void GetMovementDirection(out Vector3 direction)
        {
            direction.X = this.LateralInput * this.SpeedFactor;
            direction.Y = 0.0f;
            direction.Z = this.ForwardInput * this.SpeedFactor;
        }

        /// <summary>
        /// Determines which way the mouse wheel has been rolled.
        /// The returned value is in the range [-1,1].
        /// </summary>
        /// <returns>
        /// A positive value indicates that the mouse wheel has been rolled
        /// towards the player. A negative value indicates that the mouse
        /// wheel has been rolled away from the player.
        /// </returns>
        private float GetMouseWheelDirection()
        {
            return this._MouseWheelDelta;

            /*
            int currentWheelValue = currentMouseState.ScrollWheelValue;
            int previousWheelValue = previousMouseState.ScrollWheelValue;

            if (currentWheelValue > previousWheelValue)
                return -1.0f;
            else if (currentWheelValue < previousWheelValue)
                return 1.0f;
            else
                return 0.0f;
            */
        }

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
        /// <param name="y">Vertical mouse distance from window center.</param>
        private void PerformMouseFiltering(float x, float y)
        {
            // Shuffle all the entries in the cache.
            // Newer entries at the front. Older entries towards the back.
            for (int i = this._MouseSmoothingCache.Length - 1; i > 0; --i)
            {
                this._MouseSmoothingCache[i].X = this._MouseSmoothingCache[i - 1].X;
                this._MouseSmoothingCache[i].Y = this._MouseSmoothingCache[i - 1].Y;
            }

            // Store the current mouse movement entry at the front of cache.
            this._MouseSmoothingCache[0].X = x;
            this._MouseSmoothingCache[0].Y = y;

            float averageX = 0.0f;
            float averageY = 0.0f;
            float averageTotal = 0.0f;
            float currentWeight = 1.0f;

            // Filter the mouse movement with the rest of the cache entries.
            // Use a weighted average where newer entries have more effect than
            // older entries (towards the back of the cache).
            for (int i = 0; i < this._MouseSmoothingCache.Length; ++i)
            {
                averageX += this._MouseSmoothingCache[i].X * currentWeight;
                averageY += this._MouseSmoothingCache[i].Y * currentWeight;
                averageTotal += 1.0f * currentWeight;
                currentWeight *= this._MouseSmoothingSensitivity;
            }

            // Calculate the new smoothed mouse movement.
            this._SmoothedMouseMovement.X = averageX / averageTotal;
            this._SmoothedMouseMovement.Y = averageY / averageTotal;
        }

        /// <summary>
        /// Averages the mouse movement over a couple of frames to smooth out
        /// the mouse movement.
        /// </summary>
        /// <param name="x">Horizontal mouse distance from window center.</param>
        /// <param name="y">Vertical mouse distance from window center.</param>
        private void PerformMouseSmoothing(float x, float y)
        {
            this._MouseMovement[this._MouseIndex].X = x;
            this._MouseMovement[this._MouseIndex].Y = y;

            this._SmoothedMouseMovement.X = (this._MouseMovement[0].X + this._MouseMovement[1].X) * 0.5f;
            this._SmoothedMouseMovement.Y = (this._MouseMovement[0].Y + this._MouseMovement[1].Y) * 0.5f;

            this._MouseIndex ^= 1;
            this._MouseMovement[this._MouseIndex].X = 0.0f;
            this._MouseMovement[this._MouseIndex].Y = 0.0f;
        }

        /// <summary>
        /// Dampens the rotation by applying the rotation speed to it.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation in degrees.</param>
        /// <param name="pitchDegrees">X axis rotation in degrees.</param>
        /// <param name="rollDegrees">Z axis rotation in degrees.</param>
        private void RotateSmoothly(float headingDegrees, float pitchDegrees, float rollDegrees)
        {
            headingDegrees *= this._RotationSpeed;
            pitchDegrees *= this._RotationSpeed;
            rollDegrees *= this._RotationSpeed;

            Rotate(headingDegrees, pitchDegrees, rollDegrees);
        }

        /// <summary>
        /// Gathers and updates input from all supported input devices for use
        /// by the CameraComponent class.
        /// </summary>
        private void UpdateInput()
        {
            /*
            // TODO: FIX ME
            this.currentKeyboardState = new KeyboardState(); //Keyboard.GetState();

            // TODO: FIX ME

            this.previousMouseState = this.currentMouseState;
            this.currentMouseState = new MouseState();//Mouse.GetState();
            this._MouseWheelDelta = 0.0f;

            if (currentMouseState.IsPressed(0) == true)
            {
                var position = this._Control.PointToScreen(this._Control.Location);
                var rectangle = new Rectangle(0, 0, this._Control.Width, this._Control.Height);

                if (rectangle.Contains(currentMouseState.X, currentMouseState.Y) == true)
                {
                    int centerX = (this._Control.Width / 2);
                    int centerY = (this._Control.Height / 2);
                    // TODO: FIX ME
                    //Mouse.SetPosition(centerX, centerY);

                    if (previousMouseState.IsPressed(0) == true)
                    {
                        int deltaX = centerX - currentMouseState.X;
                        int deltaY = centerY - currentMouseState.Y;

                        PerformMouseFiltering((float)deltaX, (float)deltaY);
                        PerformMouseSmoothing(this._SmoothedMouseMovement.X, this._SmoothedMouseMovement.Y);

                        // TODO: FIXME
                        /x
                        if (currentMouseState.ScrollWheelValue != previousMouseState.ScrollWheelValue)
                        {
                            this.mouseWheelDelta = 
                                currentMouseState.ScrollWheelValue >
                                previousMouseState.ScrollWheelValue ?
                                -1.0f : 1.0f;
                        }
                        x/
                    }
                }
            }
            */
        }

        /// <summary>
        /// Updates the camera's velocity based on the supplied movement
        /// direction and the elapsed time (since this method was last
        /// called). The movement direction is the in the range [-1,1].
        /// </summary>
        /// <param name="direction">Direction moved.</param>
        /// <param name="elapsedTimeSec">Elapsed game time.</param>
        private void UpdateVelocity(ref Vector3 direction, float elapsedTimeSec)
        {
            if (direction.X != 0.0f)
            {
                // Camera is moving along the x axis.
                // Linearly accelerate up to the camera's max speed.

                this._CurrentVelocity.X += direction.X * this._Acceleration.X * elapsedTimeSec;

                if (this._CurrentVelocity.X > this._Velocity.X)
                    this._CurrentVelocity.X = this._Velocity.X;
                else if (this._CurrentVelocity.X < -this._Velocity.X)
                    this._CurrentVelocity.X = -this._Velocity.X;
            }
            else
            {
                // Camera is no longer moving along the x axis.
                // Linearly decelerate back to stationary state.

                if (this._CurrentVelocity.X > 0.0f)
                {
                    if ((this._CurrentVelocity.X -= this._Acceleration.X * elapsedTimeSec) < 0.0f)
                        this._CurrentVelocity.X = 0.0f;
                }
                else
                {
                    if ((this._CurrentVelocity.X += this._Acceleration.X * elapsedTimeSec) > 0.0f)
                        this._CurrentVelocity.X = 0.0f;
                }
            }

            if (direction.Y != 0.0f)
            {
                // Camera is moving along the y axis.
                // Linearly accelerate up to the camera's max speed.

                this._CurrentVelocity.Y += direction.Y * this._Acceleration.Y * elapsedTimeSec;

                if (this._CurrentVelocity.Y > this._Velocity.Y)
                    this._CurrentVelocity.Y = this._Velocity.Y;
                else if (this._CurrentVelocity.Y < -this._Velocity.Y)
                    this._CurrentVelocity.Y = -this._Velocity.Y;
            }
            else
            {
                // Camera is no longer moving along the y axis.
                // Linearly decelerate back to stationary state.

                if (this._CurrentVelocity.Y > 0.0f)
                {
                    if ((this._CurrentVelocity.Y -= this._Acceleration.Y * elapsedTimeSec) < 0.0f)
                        this._CurrentVelocity.Y = 0.0f;
                }
                else
                {
                    if ((this._CurrentVelocity.Y += this._Acceleration.Y * elapsedTimeSec) > 0.0f)
                        this._CurrentVelocity.Y = 0.0f;
                }
            }

            if (direction.Z != 0.0f)
            {
                // Camera is moving along the z axis.
                // Linearly accelerate up to the camera's max speed.

                this._CurrentVelocity.Z += direction.Z * this._Acceleration.Z * elapsedTimeSec;

                if (this._CurrentVelocity.Z > this._Velocity.Z)
                    this._CurrentVelocity.Z = this._Velocity.Z;
                else if (this._CurrentVelocity.Z < -this._Velocity.Z)
                    this._CurrentVelocity.Z = -this._Velocity.Z;
            }
            else
            {
                // Camera is no longer moving along the z axis.
                // Linearly decelerate back to stationary state.

                if (this._CurrentVelocity.Z > 0.0f)
                {
                    if ((this._CurrentVelocity.Z -= this._Acceleration.Z * elapsedTimeSec) < 0.0f)
                        this._CurrentVelocity.Z = 0.0f;
                }
                else
                {
                    if ((this._CurrentVelocity.Z += this._Acceleration.Z * elapsedTimeSec) > 0.0f)
                        this._CurrentVelocity.Z = 0.0f;
                }
            }
        }

        /// <summary>
        /// Moves the camera based on player input.
        /// </summary>
        /// <param name="direction">Direction moved.</param>
        /// <param name="elapsedTimeSec">Elapsed game time.</param>
        private void UpdatePosition(ref Vector3 direction, float elapsedTimeSec)
        {
            if (this._CurrentVelocity.LengthSquared() != 0.0f)
            {
                // Only move the camera if the velocity vector is not of zero
                // length. Doing this guards against the camera slowly creeping
                // around due to floating point rounding errors.

                Vector3 displacement = (this._CurrentVelocity * elapsedTimeSec) +
                    (0.5f * this._Acceleration * elapsedTimeSec * elapsedTimeSec);

                // Floating point rounding errors will slowly accumulate and
                // cause the camera to move along each axis. To prevent any
                // unintended movement the displacement vector is clamped to
                // zero for each direction that the camera isn't moving in.
                // Note that the UpdateVelocity() method will slowly decelerate
                // the camera's velocity back to a stationary state when the
                // camera is no longer moving along that direction. To account
                // for this the camera's current velocity is also checked.

                if (direction.X == 0.0f && (float)Math.Abs(this._CurrentVelocity.X) < 1e-6f)
                    displacement.X = 0.0f;

                if (direction.Y == 0.0f && (float)Math.Abs(this._CurrentVelocity.Y) < 1e-6f)
                    displacement.Y = 0.0f;

                if (direction.Z == 0.0f && (float)Math.Abs(this._CurrentVelocity.Z) < 1e-6f)
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
        /// Updates the state of the camera based on player input.
        /// </summary>
        /// <param name="gameTime">Elapsed game time.</param>
        private void UpdateCamera(float elapsedTime)
        {
            Vector3 direction;
            this.GetMovementDirection(out direction);

            float dx, dy, dz;

            switch (this._Camera.CurrentBehavior)
            {
                case BasicCamera.Behavior.FirstPerson:
                case BasicCamera.Behavior.Spectator:
                {
                    dx = this._SmoothedMouseMovement.X;
                    dy = this._SmoothedMouseMovement.Y;

                    RotateSmoothly(dx, dy, 0.0f);
                    UpdatePosition(ref direction, elapsedTime);
                    break;
                }

                case BasicCamera.Behavior.Flight:
                {
                    dy = -this._SmoothedMouseMovement.Y;
                    dz = this._SmoothedMouseMovement.X;

                    RotateSmoothly(0.0f, dy, dz);

                    if ((dx = direction.X * this._FlightYawSpeed * elapsedTime) != 0.0f)
                        this._Camera.Rotate(dx, 0.0f, 0.0f);

                    direction.X = 0.0f; // ignore yaw motion when updating camera's velocity
                    UpdatePosition(ref direction, elapsedTime);
                    break;
                }

                case BasicCamera.Behavior.Orbit:
                {
                    dx = -this._SmoothedMouseMovement.X;
                    dy = -this._SmoothedMouseMovement.Y;

                    RotateSmoothly(dx, dy, 0.0f);

                    if (this._Camera.PreferTargetYAxisOrbiting == false)
                    {
                        if ((dz = direction.X * this._OrbitRollSpeed * elapsedTime) != 0.0f)
                            this._Camera.Rotate(0.0f, 0.0f, dz);
                    }

                    if ((dz = GetMouseWheelDirection() * this._MouseWheelSpeed) != 0.0f)
                        this._Camera.Zoom(dz, this._Camera.OrbitMinZoom, this._Camera.OrbitMaxZoom);

                    break;
                }

                default:
                {
                    break;
                }
            }
        }

        #endregion

        #region Properties

        public float ForwardInput
        {
            get;
            set;
        }

        public float LateralInput
        {
            get;
            set;
        }

        public float SpeedFactor
        {
            get;
            set;
        }

        /// <summary>
        /// Property to get and set the camera's acceleration.
        /// </summary>
        public Vector3 Acceleration
        {
            get { return this._Acceleration; }
            set { this._Acceleration = value; }
        }

        /// <summary>
        /// Property to get and set the camera's behavior.
        /// </summary>
        public BasicCamera.Behavior CurrentBehavior
        {
            get { return this._Camera.CurrentBehavior; }
            set { this._Camera.CurrentBehavior = value; }
        }

        /// <summary>
        /// Property to get the camera's current velocity.
        /// </summary>
        public Vector3 CurrentVelocity
        {
            get { return this._CurrentVelocity; }
        }

        /// <summary>
        /// Property to get and set the flight behavior's yaw speed.
        /// </summary>
        public float FlightYawSpeed
        {
            get { return this._FlightYawSpeed; }
            set { this._FlightYawSpeed = value; }
        }

        /// <summary>
        /// Property to get and set the sensitivity value used to smooth
        /// mouse movement.
        /// </summary>
        public float MouseSmoothingSensitivity
        {
            get { return this._MouseSmoothingSensitivity; }
            set { this._MouseSmoothingSensitivity = value; }
        }

        /// <summary>
        /// Property to get and set the speed of the mouse wheel.
        /// This is used to zoom in and out when the camera is orbiting.
        /// </summary>
        public float MouseWheelSpeed
        {
            get { return this._MouseWheelSpeed; }
            set { this._MouseWheelSpeed = value; }
        }

        /// <summary>
        /// Property to get and set the max orbit zoom distance.
        /// </summary>
        public float OrbitMaxZoom
        {
            get { return this._Camera.OrbitMaxZoom; }
            set { this._Camera.OrbitMaxZoom = value; }
        }

        /// <summary>
        /// Property to get and set the min orbit zoom distance.
        /// </summary>
        public float OrbitMinZoom
        {
            get { return this._Camera.OrbitMinZoom; }
            set { this._Camera.OrbitMinZoom = value; }
        }

        /// <summary>
        /// Property to get and set the distance from the target when orbiting.
        /// </summary>
        public float OrbitOffsetDistance
        {
            get { return this._Camera.OrbitOffsetDistance; }
            set { this._Camera.OrbitOffsetDistance = value; }
        }

        /// <summary>
        /// Property to get and set the orbit behavior's rolling speed.
        /// This only applies when PreferTargetYAxisOrbiting is set to false.
        /// Orbiting with PreferTargetYAxisOrbiting set to true will ignore
        /// any camera rolling.
        /// </summary>
        public float OrbitRollSpeed
        {
            get { return this._OrbitRollSpeed; }
            set { this._OrbitRollSpeed = value; }
        }

        /// <summary>
        /// Property to get and set the camera orbit target position.
        /// </summary>
        public Vector3 OrbitTarget
        {
            get { return this._Camera.OrbitTarget; }
            set { this._Camera.OrbitTarget = value; }
        }

        /// <summary>
        /// Property to get and set the camera orientation.
        /// </summary>
        public Quaternion Orientation
        {
            get { return this._Camera.Orientation; }
            set { this._Camera.Orientation = value; }
        }

        /// <summary>
        /// Property to get and set the camera position.
        /// </summary>
        public Vector3 Position
        {
            get { return this._Camera.Position; }
            set { this._Camera.Position = value; }
        }

        /// <summary>
        /// Property to get and set the flag to force the camera
        /// to orbit around the orbit target's Y axis rather than the camera's
        /// local Y axis.
        /// </summary>
        public bool PreferTargetYAxisOrbiting
        {
            get { return this._Camera.PreferTargetYAxisOrbiting; }
            set { this._Camera.PreferTargetYAxisOrbiting = value; }
        }

        /// <summary>
        /// Property to get the perspective projection matrix.
        /// </summary>
        public Matrix ProjectionMatrix
        {
            get { return this._Camera.ProjectionMatrix; }
        }

        /// <summary>
        /// Property to get and set the mouse rotation speed.
        /// </summary>
        public float RotationSpeed
        {
            get { return this._RotationSpeed; }
            set { this._RotationSpeed = value; }
        }

        /// <summary>
        /// Property to get and set the camera's velocity.
        /// </summary>
        public Vector3 Velocity
        {
            get { return this._Velocity; }
            set { this._Velocity = value; }
        }

        /// <summary>
        /// Property to get the viewing direction vector.
        /// </summary>
        public Vector3 ViewDirection
        {
            get { return this._Camera.ViewDirection; }
        }

        /// <summary>
        /// Property to get the view matrix.
        /// </summary>
        public Matrix ViewMatrix
        {
            get { return this._Camera.ViewMatrix; }
        }

        /// <summary>
        /// Property to get the concatenated view-projection matrix.
        /// </summary>
        public Matrix ViewProjectionMatrix
        {
            get { return this._Camera.ViewProjectionMatrix; }
        }

        /// <summary>
        /// Property to get the camera's local X axis.
        /// </summary>
        public Vector3 XAxis
        {
            get { return this._Camera.XAxis; }
        }

        /// <summary>
        /// Property to get the camera's local Y axis.
        /// </summary>
        public Vector3 YAxis
        {
            get { return this._Camera.YAxis; }
        }

        /// <summary>
        /// Property to get the camera's local Z axis.
        /// </summary>
        public Vector3 ZAxis
        {
            get { return this._Camera.ZAxis; }
        }

        #endregion
    }
}
