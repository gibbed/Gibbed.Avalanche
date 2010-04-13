using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Control = System.Windows.Forms.Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
//using GameTime = Microsoft.Xna.Framework.GameTime;

namespace Gibbed.Avalanche.ModelViewer
{
    internal class ModelViewCamera : ICamera
    {
        public enum Actions
        {
            FlightYawLeftPrimary,
            FlightYawLeftAlternate,
            FlightYawRightPrimary,
            FlightYawRightAlternate,

            MoveForwardsPrimary,
            MoveForwardsAlternate,
            MoveBackwardsPrimary,
            MoveBackwardsAlternate,

            MoveDownPrimary,
            MoveDownAlternate,
            MoveUpPrimary,
            MoveUpAlternate,

            OrbitRollLeftPrimary,
            OrbitRollLeftAlternate,
            OrbitRollRightPrimary,
            OrbitRollRightAlternate,

            PitchUpPrimary,
            PitchUpAlternate,
            PitchDownPrimary,
            PitchDownAlternate,

            YawLeftPrimary,
            YawLeftAlternate,
            YawRightPrimary,
            YawRightAlternate,

            RollLeftPrimary,
            RollLeftAlternate,
            RollRightPrimary,
            RollRightAlternate,

            StrafeRightPrimary,
            StrafeRightAlternate,
            StrafeLeftPrimary,
            StrafeLeftAlternate
        };

        private const float DEFAULT_ACCELERATION_X = 8.0f;
        private const float DEFAULT_ACCELERATION_Y = 8.0f;
        private const float DEFAULT_ACCELERATION_Z = 8.0f;
        private const float DEFAULT_MOUSE_SMOOTHING_SENSITIVITY = 0.5f;
        private const float DEFAULT_SPEED_FLIGHT_YAW = 100.0f;
        private const float DEFAULT_SPEED_MOUSE_WHEEL = 1.0f;
        private const float DEFAULT_SPEED_ORBIT_ROLL = 100.0f;
        private const float DEFAULT_SPEED_ROTATION = 0.3f;
        private const float DEFAULT_VELOCITY_X = 1.0f;
        private const float DEFAULT_VELOCITY_Y = 1.0f;
        private const float DEFAULT_VELOCITY_Z = 1.0f;

        private const int MOUSE_SMOOTHING_CACHE_SIZE = 10;

        private Control control;
        private Camera camera;
        private bool movingAlongPosX;
        private bool movingAlongNegX;
        private bool movingAlongPosY;
        private bool movingAlongNegY;
        private bool movingAlongPosZ;
        private bool movingAlongNegZ;
        private int savedMousePosX;
        private int savedMousePosY;
        private int mouseIndex;
        private float rotationSpeed;
        private float orbitRollSpeed;
        private float flightYawSpeed;
        private float mouseSmoothingSensitivity;
        private float mouseWheelSpeed;
        private float mouseWheelDelta;
        private Vector3 acceleration;
        private Vector3 currentVelocity;
        private Vector3 velocity;
        private Vector2[] mouseMovement;
        private Vector2[] mouseSmoothingCache;
        private Vector2 smoothedMouseMovement;
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private KeyboardState currentKeyboardState;
        private Dictionary<Actions, Keys> actionKeys;

        #region Public Methods

        /// <summary>
        /// Constructs a new instance of the CameraComponent class. The
        /// camera will have a spectator behavior, and will be initially
        /// positioned at the world origin looking down the world negative
        /// z axis. An initial perspective projection matrix is created
        /// as well as setting up initial key bindings to the actions.
        /// </summary>
        public ModelViewCamera(Control control)
        {
            this.control = control;

            this.camera = new Camera();
            this.camera.CurrentBehavior = Camera.Behavior.Spectator;

            this.movingAlongPosX = false;
            this.movingAlongNegX = false;
            this.movingAlongPosY = false;
            this.movingAlongNegY = false;
            this.movingAlongPosZ = false;
            this.movingAlongNegZ = false;

            this.savedMousePosX = -1;
            this.savedMousePosY = -1;

            this.rotationSpeed = DEFAULT_SPEED_ROTATION;
            this.orbitRollSpeed = DEFAULT_SPEED_ORBIT_ROLL;
            this.flightYawSpeed = DEFAULT_SPEED_FLIGHT_YAW;
            this.mouseWheelSpeed = DEFAULT_SPEED_MOUSE_WHEEL;
            this.mouseSmoothingSensitivity = DEFAULT_MOUSE_SMOOTHING_SENSITIVITY;
            this.acceleration = new Vector3(DEFAULT_ACCELERATION_X, DEFAULT_ACCELERATION_Y, DEFAULT_ACCELERATION_Z);
            this.velocity = new Vector3(DEFAULT_VELOCITY_X, DEFAULT_VELOCITY_Y, DEFAULT_VELOCITY_Z);
            this.mouseSmoothingCache = new Vector2[MOUSE_SMOOTHING_CACHE_SIZE];

            this.mouseIndex = 0;
            this.mouseMovement = new Vector2[2];
            this.mouseMovement[0].X = 0.0f;
            this.mouseMovement[0].Y = 0.0f;
            this.mouseMovement[1].X = 0.0f;
            this.mouseMovement[1].Y = 0.0f;

            float aspect = (float)this.control.Width / (float)this.control.Height;

            this.Perspective(
                Camera.DEFAULT_FOVX,
                aspect,
                0.01f,/*Camera.DEFAULT_ZNEAR,*/
                Camera.DEFAULT_ZFAR);

            this.actionKeys = new Dictionary<Actions, Keys>();
            this.actionKeys.Add(Actions.FlightYawLeftPrimary, Keys.Left);
            this.actionKeys.Add(Actions.FlightYawLeftAlternate, Keys.A);
            this.actionKeys.Add(Actions.FlightYawRightPrimary, Keys.Right);
            this.actionKeys.Add(Actions.FlightYawRightAlternate, Keys.D);

            this.actionKeys.Add(Actions.MoveForwardsPrimary, Keys.Up);
            this.actionKeys.Add(Actions.MoveForwardsAlternate, Keys.W);
            this.actionKeys.Add(Actions.MoveBackwardsPrimary, Keys.Down);
            this.actionKeys.Add(Actions.MoveBackwardsAlternate, Keys.S);

            this.actionKeys.Add(Actions.MoveDownPrimary, Keys.Q);
            this.actionKeys.Add(Actions.MoveDownAlternate, Keys.PageDown);
            this.actionKeys.Add(Actions.MoveUpPrimary, Keys.E);
            this.actionKeys.Add(Actions.MoveUpAlternate, Keys.PageUp);
            
            this.actionKeys.Add(Actions.OrbitRollLeftPrimary, Keys.Left);
            this.actionKeys.Add(Actions.OrbitRollLeftAlternate, Keys.A);
            this.actionKeys.Add(Actions.OrbitRollRightPrimary, Keys.Right);
            this.actionKeys.Add(Actions.OrbitRollRightAlternate, Keys.D);
            
            this.actionKeys.Add(Actions.StrafeRightPrimary, Keys.Right);
            this.actionKeys.Add(Actions.StrafeRightAlternate, Keys.D);
            this.actionKeys.Add(Actions.StrafeLeftPrimary, Keys.Left);
            this.actionKeys.Add(Actions.StrafeLeftAlternate, Keys.A);
        }

        public void Stop()
        {
            this.smoothedMouseMovement.X = 0;
            this.smoothedMouseMovement.Y = 0;
            this.movingAlongPosX = false;
            this.movingAlongNegX = false;
            this.movingAlongPosY = false;
            this.movingAlongNegY = false;
            this.movingAlongPosZ = false;
            this.movingAlongNegZ = false;
        }

        /// <summary>
        /// Builds a look at style viewing matrix.
        /// </summary>
        /// <param name="target">The target position to look at.</param>
        public void LookAt(Vector3 target)
        {
            this.camera.LookAt(target);
        }

        /// <summary>
        /// Builds a look at style viewing matrix.
        /// </summary>
        /// <param name="eye">The camera position.</param>
        /// <param name="target">The target position to look at.</param>
        /// <param name="up">The up direction.</param>
        public void LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            this.camera.LookAt(eye, target, up);
        }

        /// <summary>
        /// Binds an action to a keyboard key.
        /// </summary>
        /// <param name="action">The action to bind.</param>
        /// <param name="key">The key to map the action to.</param>
        public void MapActionToKey(Actions action, Keys key)
        {
            this.actionKeys[action] = key;
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
            this.camera.Move(dx, dy, dz);
        }

        /// <summary>
        /// Moves the camera the specified distance in the specified direction.
        /// </summary>
        /// <param name="direction">Direction to move.</param>
        /// <param name="distance">How far to move.</param>
        public void Move(Vector3 direction, Vector3 distance)
        {
            this.camera.Move(direction, distance);
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
            this.camera.Perspective(fovx, aspect, znear, zfar);
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
            this.camera.Rotate(headingDegrees, pitchDegrees, rollDegrees);
        }

        /// <summary>
        /// Updates the state of the CameraComponent class.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update.</param>
        public void Update(GameTime gameTime, bool handleInput)
        {
            if (handleInput == true)
            {
                this.UpdateInput();
            }
            else
            {
                this.currentKeyboardState = new KeyboardState();
                this.currentMouseState = this.previousMouseState = new MouseState();
            }

            this.UpdateCamera(gameTime);
        }

        /// <summary>
        /// Undo any camera rolling by leveling the camera. When the camera is
        /// orbiting this method will cause the camera to become level with the
        /// orbit target.
        /// </summary>
        public void UndoRoll()
        {
            this.camera.UndoRoll();
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
            this.camera.Zoom(zoom, minZoom, maxZoom);
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
            direction.X = 0.0f;
            direction.Y = 0.0f;
            direction.Z = 0.0f;

            if (currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveForwardsPrimary]) ||
                currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveForwardsAlternate]))
            {
                if (!movingAlongNegZ)
                {
                    movingAlongNegZ = true;
                    currentVelocity.Z = 0.0f;
                }

                direction.Z += 1.0f;
            }
            else
            {
                movingAlongNegZ = false;
            }

            if (currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveBackwardsPrimary]) ||
                currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveBackwardsAlternate]))
            {
                if (!movingAlongPosZ)
                {
                    movingAlongPosZ = true;
                    currentVelocity.Z = 0.0f;
                }

                direction.Z -= 1.0f;
            }
            else
            {
                movingAlongPosZ = false;
            }

            if (currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveUpPrimary]) ||
                currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveUpAlternate]))
            {
                if (!movingAlongPosY)
                {
                    movingAlongPosY = true;
                    currentVelocity.Y = 0.0f;
                }

                direction.Y += 1.0f;
            }
            else
            {
                movingAlongPosY = false;
            }

            if (currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveDownPrimary]) ||
                currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveDownAlternate]))
            {
                if (!movingAlongNegY)
                {
                    movingAlongNegY = true;
                    currentVelocity.Y = 0.0f;
                }

                direction.Y -= 1.0f;
            }
            else
            {
                movingAlongNegY = false;
            }

            switch (CurrentBehavior)
            {
                case Camera.Behavior.FirstPerson:
                case Camera.Behavior.Spectator:
                {
                    if (currentKeyboardState.IsKeyDown(actionKeys[Actions.StrafeRightPrimary]) ||
                        currentKeyboardState.IsKeyDown(actionKeys[Actions.StrafeRightAlternate]))
                    {
                        if (!movingAlongPosX)
                        {
                            movingAlongPosX = true;
                            currentVelocity.X = 0.0f;
                        }

                        direction.X += 1.0f;
                    }
                    else
                    {
                        movingAlongPosX = false;
                    }

                    if (currentKeyboardState.IsKeyDown(actionKeys[Actions.StrafeLeftPrimary]) ||
                        currentKeyboardState.IsKeyDown(actionKeys[Actions.StrafeLeftAlternate]))
                    {
                        if (!movingAlongNegX)
                        {
                            movingAlongNegX = true;
                            currentVelocity.X = 0.0f;
                        }

                        direction.X -= 1.0f;
                    }
                    else
                    {
                        movingAlongNegX = false;
                    }

                    break;
                }

                case Camera.Behavior.Flight:
                {
                    if (currentKeyboardState.IsKeyDown(actionKeys[Actions.FlightYawLeftPrimary]) ||
                        currentKeyboardState.IsKeyDown(actionKeys[Actions.FlightYawLeftAlternate]))
                    {
                        if (!movingAlongPosX)
                        {
                            movingAlongPosX = true;
                            currentVelocity.X = 0.0f;
                        }

                        direction.X += 1.0f;
                    }
                    else
                    {
                        movingAlongPosX = false;
                    }

                    if (currentKeyboardState.IsKeyDown(actionKeys[Actions.FlightYawRightPrimary]) ||
                        currentKeyboardState.IsKeyDown(actionKeys[Actions.FlightYawRightAlternate]))
                    {
                        if (!movingAlongNegX)
                        {
                            movingAlongNegX = true;
                            currentVelocity.X = 0.0f;
                        }

                        direction.X -= 1.0f;
                    }
                    else
                    {
                        movingAlongNegX = false;
                    }
                    break;
                }

                case Camera.Behavior.Orbit:
                {
                    if (currentKeyboardState.IsKeyDown(actionKeys[Actions.OrbitRollLeftPrimary]) ||
                        currentKeyboardState.IsKeyDown(actionKeys[Actions.OrbitRollLeftAlternate]))
                    {
                        if (!movingAlongPosX)
                        {
                            movingAlongPosX = true;
                            currentVelocity.X = 0.0f;
                        }

                        direction.X += 1.0f;
                    }
                    else
                    {
                        movingAlongPosX = false;
                    }

                    if (currentKeyboardState.IsKeyDown(actionKeys[Actions.OrbitRollRightPrimary]) ||
                        currentKeyboardState.IsKeyDown(actionKeys[Actions.OrbitRollRightAlternate]))
                    {
                        if (!movingAlongNegX)
                        {
                            movingAlongNegX = true;
                            currentVelocity.X = 0.0f;
                        }

                        direction.X -= 1.0f;
                    }
                    else
                    {
                        movingAlongNegX = false;
                    }
                    break;
                }

                default:
                {
                    break;
                }
            }
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
            return this.mouseWheelDelta;

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
        /// Event handler for when the game window acquires input focus.
        /// </summary>
        /// <param name="sender">Ignored.</param>
        /// <param name="e">Ignored.</param>
        private void HandleActivatedEvent(object sender, EventArgs e)
        {
            if (savedMousePosX >= 0 && savedMousePosY >= 0)
                Mouse.SetPosition(savedMousePosX, savedMousePosY);
        }

        /// <summary>
        /// Event hander for when the game window loses input focus.
        /// </summary>
        /// <param name="sender">Ignored.</param>
        /// <param name="e">Ignored.</param>
        private void HandleDeactivatedEvent(object sender, EventArgs e)
        {
            MouseState state = Mouse.GetState();

            savedMousePosX = state.X;
            savedMousePosY = state.Y;
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
            for (int i = mouseSmoothingCache.Length - 1; i > 0; --i)
            {
                mouseSmoothingCache[i].X = mouseSmoothingCache[i - 1].X;
                mouseSmoothingCache[i].Y = mouseSmoothingCache[i - 1].Y;
            }

            // Store the current mouse movement entry at the front of cache.
            mouseSmoothingCache[0].X = x;
            mouseSmoothingCache[0].Y = y;

            float averageX = 0.0f;
            float averageY = 0.0f;
            float averageTotal = 0.0f;
            float currentWeight = 1.0f;

            // Filter the mouse movement with the rest of the cache entries.
            // Use a weighted average where newer entries have more effect than
            // older entries (towards the back of the cache).
            for (int i = 0; i < mouseSmoothingCache.Length; ++i)
            {
                averageX += mouseSmoothingCache[i].X * currentWeight;
                averageY += mouseSmoothingCache[i].Y * currentWeight;
                averageTotal += 1.0f * currentWeight;
                currentWeight *= mouseSmoothingSensitivity;
            }

            // Calculate the new smoothed mouse movement.
            smoothedMouseMovement.X = averageX / averageTotal;
            smoothedMouseMovement.Y = averageY / averageTotal;
        }

        /// <summary>
        /// Averages the mouse movement over a couple of frames to smooth out
        /// the mouse movement.
        /// </summary>
        /// <param name="x">Horizontal mouse distance from window center.</param>
        /// <param name="y">Vertical mouse distance from window center.</param>
        private void PerformMouseSmoothing(float x, float y)
        {
            mouseMovement[mouseIndex].X = x;
            mouseMovement[mouseIndex].Y = y;

            smoothedMouseMovement.X = (mouseMovement[0].X + mouseMovement[1].X) * 0.5f;
            smoothedMouseMovement.Y = (mouseMovement[0].Y + mouseMovement[1].Y) * 0.5f;

            mouseIndex ^= 1;
            mouseMovement[mouseIndex].X = 0.0f;
            mouseMovement[mouseIndex].Y = 0.0f;
        }

        /// <summary>
        /// Dampens the rotation by applying the rotation speed to it.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation in degrees.</param>
        /// <param name="pitchDegrees">X axis rotation in degrees.</param>
        /// <param name="rollDegrees">Z axis rotation in degrees.</param>
        private void RotateSmoothly(float headingDegrees, float pitchDegrees, float rollDegrees)
        {
            headingDegrees *= rotationSpeed;
            pitchDegrees *= rotationSpeed;
            rollDegrees *= rotationSpeed;

            Rotate(headingDegrees, pitchDegrees, rollDegrees);
        }

        /// <summary>
        /// Gathers and updates input from all supported input devices for use
        /// by the CameraComponent class.
        /// </summary>
        private void UpdateInput()
        {
            this.currentKeyboardState = Keyboard.GetState();

            this.previousMouseState = this.currentMouseState;
            this.currentMouseState = Mouse.GetState();
            this.mouseWheelDelta = 0.0f;

            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                var position = this.control.PointToScreen(this.control.Location);
                var rectangle = new Rectangle(0, 0, control.Width, control.Height);

                if (rectangle.Contains(currentMouseState.X, currentMouseState.Y) == true)
                {
                    int centerX = (control.Width / 2);
                    int centerY = (control.Height / 2);
                    Mouse.SetPosition(centerX, centerY);

                    if (previousMouseState.LeftButton == ButtonState.Pressed)
                    {
                        int deltaX = centerX - currentMouseState.X;
                        int deltaY = centerY - currentMouseState.Y;

                        PerformMouseFiltering((float)deltaX, (float)deltaY);
                        PerformMouseSmoothing(smoothedMouseMovement.X, smoothedMouseMovement.Y);

                        if (currentMouseState.ScrollWheelValue != previousMouseState.ScrollWheelValue)
                        {
                            this.mouseWheelDelta = 
                                currentMouseState.ScrollWheelValue >
                                previousMouseState.ScrollWheelValue ?
                                -1.0f : 1.0f;
                        }
                    }
                }
            }
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

                currentVelocity.X += direction.X * acceleration.X * elapsedTimeSec;

                if (currentVelocity.X > velocity.X)
                    currentVelocity.X = velocity.X;
                else if (currentVelocity.X < -velocity.X)
                    currentVelocity.X = -velocity.X;
            }
            else
            {
                // Camera is no longer moving along the x axis.
                // Linearly decelerate back to stationary state.

                if (currentVelocity.X > 0.0f)
                {
                    if ((currentVelocity.X -= acceleration.X * elapsedTimeSec) < 0.0f)
                        currentVelocity.X = 0.0f;
                }
                else
                {
                    if ((currentVelocity.X += acceleration.X * elapsedTimeSec) > 0.0f)
                        currentVelocity.X = 0.0f;
                }
            }

            if (direction.Y != 0.0f)
            {
                // Camera is moving along the y axis.
                // Linearly accelerate up to the camera's max speed.

                currentVelocity.Y += direction.Y * acceleration.Y * elapsedTimeSec;

                if (currentVelocity.Y > velocity.Y)
                    currentVelocity.Y = velocity.Y;
                else if (currentVelocity.Y < -velocity.Y)
                    currentVelocity.Y = -velocity.Y;
            }
            else
            {
                // Camera is no longer moving along the y axis.
                // Linearly decelerate back to stationary state.

                if (currentVelocity.Y > 0.0f)
                {
                    if ((currentVelocity.Y -= acceleration.Y * elapsedTimeSec) < 0.0f)
                        currentVelocity.Y = 0.0f;
                }
                else
                {
                    if ((currentVelocity.Y += acceleration.Y * elapsedTimeSec) > 0.0f)
                        currentVelocity.Y = 0.0f;
                }
            }

            if (direction.Z != 0.0f)
            {
                // Camera is moving along the z axis.
                // Linearly accelerate up to the camera's max speed.

                currentVelocity.Z += direction.Z * acceleration.Z * elapsedTimeSec;

                if (currentVelocity.Z > velocity.Z)
                    currentVelocity.Z = velocity.Z;
                else if (currentVelocity.Z < -velocity.Z)
                    currentVelocity.Z = -velocity.Z;
            }
            else
            {
                // Camera is no longer moving along the z axis.
                // Linearly decelerate back to stationary state.

                if (currentVelocity.Z > 0.0f)
                {
                    if ((currentVelocity.Z -= acceleration.Z * elapsedTimeSec) < 0.0f)
                        currentVelocity.Z = 0.0f;
                }
                else
                {
                    if ((currentVelocity.Z += acceleration.Z * elapsedTimeSec) > 0.0f)
                        currentVelocity.Z = 0.0f;
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
            if (currentVelocity.LengthSquared() != 0.0f)
            {
                // Only move the camera if the velocity vector is not of zero
                // length. Doing this guards against the camera slowly creeping
                // around due to floating point rounding errors.

                Vector3 displacement = (currentVelocity * elapsedTimeSec) +
                    (0.5f * acceleration * elapsedTimeSec * elapsedTimeSec);

                // Floating point rounding errors will slowly accumulate and
                // cause the camera to move along each axis. To prevent any
                // unintended movement the displacement vector is clamped to
                // zero for each direction that the camera isn't moving in.
                // Note that the UpdateVelocity() method will slowly decelerate
                // the camera's velocity back to a stationary state when the
                // camera is no longer moving along that direction. To account
                // for this the camera's current velocity is also checked.

                if (direction.X == 0.0f && (float)Math.Abs(currentVelocity.X) < 1e-6f)
                    displacement.X = 0.0f;

                if (direction.Y == 0.0f && (float)Math.Abs(currentVelocity.Y) < 1e-6f)
                    displacement.Y = 0.0f;

                if (direction.Z == 0.0f && (float)Math.Abs(currentVelocity.Z) < 1e-6f)
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
        private void UpdateCamera(GameTime gameTime)
        {
            float elapsedTimeSec = (float)gameTime.ElapsedRealTime.TotalSeconds;

            Vector3 direction = new Vector3();

            this.GetMovementDirection(out direction);

            float dx = 0.0f;
            float dy = 0.0f;
            float dz = 0.0f;

            switch (camera.CurrentBehavior)
            {
                case Camera.Behavior.FirstPerson:
                case Camera.Behavior.Spectator:
                {
                    dx = smoothedMouseMovement.X;
                    dy = smoothedMouseMovement.Y;

                    RotateSmoothly(dx, dy, 0.0f);
                    UpdatePosition(ref direction, elapsedTimeSec);
                    break;
                }

                case Camera.Behavior.Flight:
                {
                    dy = -smoothedMouseMovement.Y;
                    dz = smoothedMouseMovement.X;

                    RotateSmoothly(0.0f, dy, dz);

                    if ((dx = direction.X * flightYawSpeed * elapsedTimeSec) != 0.0f)
                        camera.Rotate(dx, 0.0f, 0.0f);

                    direction.X = 0.0f; // ignore yaw motion when updating camera's velocity
                    UpdatePosition(ref direction, elapsedTimeSec);
                    break;
                }

                case Camera.Behavior.Orbit:
                {
                    dx = -smoothedMouseMovement.X;
                    dy = -smoothedMouseMovement.Y;

                    RotateSmoothly(dx, dy, 0.0f);

                    if (!camera.PreferTargetYAxisOrbiting)
                    {
                        if ((dz = direction.X * orbitRollSpeed * elapsedTimeSec) != 0.0f)
                            camera.Rotate(0.0f, 0.0f, dz);
                    }

                    if ((dz = GetMouseWheelDirection() * mouseWheelSpeed) != 0.0f)
                        camera.Zoom(dz, camera.OrbitMinZoom, camera.OrbitMaxZoom);

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

        /// <summary>
        /// Property to get and set the camera's acceleration.
        /// </summary>
        public Vector3 Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }

        /// <summary>
        /// Property to get and set the camera's behavior.
        /// </summary>
        public Camera.Behavior CurrentBehavior
        {
            get { return camera.CurrentBehavior; }
            set { camera.CurrentBehavior = value; }
        }

        /// <summary>
        /// Property to get the camera's current velocity.
        /// </summary>
        public Vector3 CurrentVelocity
        {
            get { return currentVelocity; }
        }

        /// <summary>
        /// Property to get and set the flight behavior's yaw speed.
        /// </summary>
        public float FlightYawSpeed
        {
            get { return flightYawSpeed; }
            set { flightYawSpeed = value; }
        }

        /// <summary>
        /// Property to get and set the sensitivity value used to smooth
        /// mouse movement.
        /// </summary>
        public float MouseSmoothingSensitivity
        {
            get { return mouseSmoothingSensitivity; }
            set { mouseSmoothingSensitivity = value; }
        }

        /// <summary>
        /// Property to get and set the speed of the mouse wheel.
        /// This is used to zoom in and out when the camera is orbiting.
        /// </summary>
        public float MouseWheelSpeed
        {
            get { return mouseWheelSpeed; }
            set { mouseWheelSpeed = value; }
        }

        /// <summary>
        /// Property to get and set the max orbit zoom distance.
        /// </summary>
        public float OrbitMaxZoom
        {
            get { return camera.OrbitMaxZoom; }
            set { camera.OrbitMaxZoom = value; }
        }

        /// <summary>
        /// Property to get and set the min orbit zoom distance.
        /// </summary>
        public float OrbitMinZoom
        {
            get { return camera.OrbitMinZoom; }
            set { camera.OrbitMinZoom = value; }
        }

        /// <summary>
        /// Property to get and set the distance from the target when orbiting.
        /// </summary>
        public float OrbitOffsetDistance
        {
            get { return camera.OrbitOffsetDistance; }
            set { camera.OrbitOffsetDistance = value; }
        }

        /// <summary>
        /// Property to get and set the orbit behavior's rolling speed.
        /// This only applies when PreferTargetYAxisOrbiting is set to false.
        /// Orbiting with PreferTargetYAxisOrbiting set to true will ignore
        /// any camera rolling.
        /// </summary>
        public float OrbitRollSpeed
        {
            get { return orbitRollSpeed; }
            set { orbitRollSpeed = value; }
        }

        /// <summary>
        /// Property to get and set the camera orbit target position.
        /// </summary>
        public Vector3 OrbitTarget
        {
            get { return camera.OrbitTarget; }
            set { camera.OrbitTarget = value; }
        }

        /// <summary>
        /// Property to get and set the camera orientation.
        /// </summary>
        public Quaternion Orientation
        {
            get { return camera.Orientation; }
            set { camera.Orientation = value; }
        }

        /// <summary>
        /// Property to get and set the camera position.
        /// </summary>
        public Vector3 Position
        {
            get { return camera.Position; }
            set { camera.Position = value; }
        }

        /// <summary>
        /// Property to get and set the flag to force the camera
        /// to orbit around the orbit target's Y axis rather than the camera's
        /// local Y axis.
        /// </summary>
        public bool PreferTargetYAxisOrbiting
        {
            get { return camera.PreferTargetYAxisOrbiting; }
            set { camera.PreferTargetYAxisOrbiting = value; }
        }

        /// <summary>
        /// Property to get the perspective projection matrix.
        /// </summary>
        public Matrix ProjectionMatrix
        {
            get { return camera.ProjectionMatrix; }
        }

        /// <summary>
        /// Property to get and set the mouse rotation speed.
        /// </summary>
        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }

        /// <summary>
        /// Property to get and set the camera's velocity.
        /// </summary>
        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        /// <summary>
        /// Property to get the viewing direction vector.
        /// </summary>
        public Vector3 ViewDirection
        {
            get { return camera.ViewDirection; }
        }

        /// <summary>
        /// Property to get the view matrix.
        /// </summary>
        public Matrix ViewMatrix
        {
            get { return camera.ViewMatrix; }
        }

        /// <summary>
        /// Property to get the concatenated view-projection matrix.
        /// </summary>
        public Matrix ViewProjectionMatrix
        {
            get { return camera.ViewProjectionMatrix; }
        }

        /// <summary>
        /// Property to get the camera's local X axis.
        /// </summary>
        public Vector3 XAxis
        {
            get { return camera.XAxis; }
        }

        /// <summary>
        /// Property to get the camera's local Y axis.
        /// </summary>
        public Vector3 YAxis
        {
            get { return camera.YAxis; }
        }

        /// <summary>
        /// Property to get the camera's local Z axis.
        /// </summary>
        public Vector3 ZAxis
        {
            get { return camera.ZAxis; }
        }

        #endregion
    }
}
