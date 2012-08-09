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

namespace Gibbed.Avalanche.ModelViewer2
{
    /// <summary>
    /// A general purpose 6DoF (six degrees of freedom) quaternion based
    /// camera. This camera class supports 4 different behaviors: first
    /// person mode, spectator mode, flight mode, and orbit mode. First
    /// person mode only allows 5DOF (x axis movement, y axis movement,
    /// z axis movement, yaw, and pitch) and movement is always parallel
    /// to the world x-z (ground) plane. Spectator mode is similar to first
    /// person mode only movement is along the direction the camera is
    /// pointing. Flight mode supports 6DoF. This is the camera class'
    /// default behavior. Orbit mode rotates the camera around a target
    /// position. This mode can be used to simulate a third person camera.
    /// </summary>
    internal class BasicCamera : ICamera
    {
        public enum Behavior
        {
            FirstPerson,
            Spectator,
            Flight,
            Orbit
        };

        public const float DEFAULT_FOVX = 90.0f;
        public const float DEFAULT_ZNEAR = 0.1f;
        public const float DEFAULT_ZFAR = 1000.0f;

        public const float DEFAULT_ORBIT_MIN_ZOOM = DEFAULT_ZNEAR + 1.0f;
        public const float DEFAULT_ORBIT_MAX_ZOOM = DEFAULT_ZFAR * 0.5f;

        public const float DEFAULT_ORBIT_OFFSET_LENGTH = DEFAULT_ORBIT_MIN_ZOOM +
            (DEFAULT_ORBIT_MAX_ZOOM - DEFAULT_ORBIT_MIN_ZOOM) * 0.25f;

        private static Vector3 WORLD_X_AXIS = new Vector3(1.0f, 0.0f, 0.0f);
        private static Vector3 WORLD_Y_AXIS = new Vector3(0.0f, 1.0f, 0.0f);
        private static Vector3 WORLD_Z_AXIS = new Vector3(0.0f, 0.0f, 1.0f);

        private Behavior behavior;
        private bool preferTargetYAxisOrbiting;

        private float fovx;
        private float aspectRatio;
        private float znear;
        private float zfar;
        private float accumPitchDegrees;
        private float orbitMinZoom;
        private float orbitMaxZoom;
        private float orbitOffsetLength;
        private float firstPersonYOffset;

        private Vector3 eye;
        private Vector3 target;
        private Vector3 targetYAxis;
        private Vector3 xAxis;
        private Vector3 yAxis;
        private Vector3 zAxis;
        private Vector3 viewDir;

        private Quaternion orientation;
        private Matrix viewMatrix;
        private Matrix projMatrix;

        private Quaternion savedOrientation;
        private Vector3 savedEye;
        private float savedAccumPitchDegrees;

        #region Public Methods

        /// <summary>
        /// Constructs a new instance of the camera class. The camera will
        /// have a flight behavior, and will be initially positioned at the
        /// world origin looking down the world negative z axis.
        /// </summary>
        public BasicCamera()
        {
            behavior = Behavior.Flight;
            preferTargetYAxisOrbiting = true;

            fovx = DEFAULT_FOVX;
            znear = DEFAULT_ZNEAR;
            zfar = DEFAULT_ZFAR;

            accumPitchDegrees = 0.0f;
            orbitMinZoom = DEFAULT_ORBIT_MIN_ZOOM;
            orbitMaxZoom = DEFAULT_ORBIT_MAX_ZOOM;
            orbitOffsetLength = DEFAULT_ORBIT_OFFSET_LENGTH;
            firstPersonYOffset = 0.0f;

            eye = Vector3.Zero;
            target = Vector3.Zero;
            targetYAxis = Vector3.UnitY;
            xAxis = Vector3.UnitX;
            yAxis = Vector3.UnitY;
            zAxis = Vector3.UnitZ;

            orientation = Quaternion.Identity;
            viewMatrix = Matrix.Identity;

            savedEye = eye;
            savedOrientation = orientation;
            savedAccumPitchDegrees = 0.0f;
        }

        /// <summary>
        /// Builds a look at style viewing matrix.
        /// </summary>
        /// <param name="target">The target position to look at.</param>
        public void LookAt(Vector3 target)
        {
            LookAt(eye, target, yAxis);
        }

        /// <summary>
        /// Builds a look at style viewing matrix.
        /// </summary>
        /// <param name="eye">The camera position.</param>
        /// <param name="target">The target position to look at.</param>
        /// <param name="up">The up direction.</param>
        public void LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            this.eye = eye;
            this.target = target;

            zAxis = eye - target;
            zAxis.Normalize();

            viewDir.X = -zAxis.X;
            viewDir.Y = -zAxis.Y;
            viewDir.Z = -zAxis.Z;

            Vector3.Cross(ref up, ref zAxis, out xAxis);
            xAxis.Normalize();

            Vector3.Cross(ref zAxis, ref xAxis, out yAxis);
            yAxis.Normalize();
            xAxis.Normalize();

            viewMatrix.M11 = xAxis.X;
            viewMatrix.M21 = xAxis.Y;
            viewMatrix.M31 = xAxis.Z;
            viewMatrix.M41 = -Vector3.Dot(xAxis, eye);

            viewMatrix.M12 = yAxis.X;
            viewMatrix.M22 = yAxis.Y;
            viewMatrix.M32 = yAxis.Z;
            viewMatrix.M42 = -Vector3.Dot(yAxis, eye);

            viewMatrix.M13 = zAxis.X;
            viewMatrix.M23 = zAxis.Y;
            viewMatrix.M33 = zAxis.Z;
            viewMatrix.M43 = -Vector3.Dot(zAxis, eye);

            viewMatrix.M14 = 0.0f;
            viewMatrix.M24 = 0.0f;
            viewMatrix.M34 = 0.0f;
            viewMatrix.M44 = 1.0f;

            accumPitchDegrees = MathHelpers.ToDegrees((float)Math.Asin(viewMatrix.M23));
            Quaternion.RotationMatrix(ref viewMatrix, out orientation);
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
            if (behavior == Behavior.Orbit)
            {
                // Orbiting camera is always positioned relative to the target
                // position. See UpdateViewMatrix().
                return;
            }

            Vector3 forwards;

            if (behavior == Behavior.FirstPerson)
            {
                // Calculate the forwards direction. Can't just use the
                // camera's view direction as doing so will cause the camera to
                // move more slowly as the camera's view approaches 90 degrees
                // straight up and down.

                forwards = Vector3.Normalize(Vector3.Cross(WORLD_Y_AXIS, xAxis));
            }
            else
            {
                forwards = viewDir;
            }

            eye += xAxis * dx;
            eye += WORLD_Y_AXIS * dy;
            eye += forwards * dz;

            Position = eye;
        }

        /// <summary>
        /// Moves the camera the specified distance in the specified direction.
        /// </summary>
        /// <param name="direction">Direction to move.</param>
        /// <param name="distance">How far to move.</param>
        public void Move(Vector3 direction, Vector3 distance)
        {
            if (behavior == Behavior.Orbit)
            {
                // Orbiting camera is always positioned relative to the target
                // position. See UpdateViewMatrix().
                return;
            }

            eye.X += direction.X * distance.X;
            eye.Y += direction.Y * distance.Y;
            eye.Z += direction.Z * distance.Z;

            UpdateViewMatrix();
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
            this.fovx = fovx;
            this.aspectRatio = aspect;
            this.znear = znear;
            this.zfar = zfar;

            float aspectInv = 1.0f / aspect;
            float e = 1.0f / (float)Math.Tan(MathHelpers.ToRadians(fovx) / 2.0f);
            float fovy = 2.0f * (float)Math.Atan(aspectInv / e);
            float xScale = 1.0f / (float)Math.Tan(0.5f * fovy);
            float yScale = xScale / aspectInv;

            projMatrix.M11 = xScale;
            projMatrix.M12 = 0.0f;
            projMatrix.M13 = 0.0f;
            projMatrix.M14 = 0.0f;

            projMatrix.M21 = 0.0f;
            projMatrix.M22 = yScale;
            projMatrix.M23 = 0.0f;
            projMatrix.M24 = 0.0f;

            projMatrix.M31 = 0.0f;
            projMatrix.M32 = 0.0f;
            projMatrix.M33 = (zfar + znear) / (znear - zfar);
            projMatrix.M34 = -1.0f;

            projMatrix.M41 = 0.0f;
            projMatrix.M42 = 0.0f;
            projMatrix.M43 = (2.0f * zfar * znear) / (znear - zfar);
            projMatrix.M44 = 0.0f;
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
            headingDegrees = -headingDegrees;
            pitchDegrees = -pitchDegrees;
            rollDegrees = -rollDegrees;

            switch (behavior)
            {
                case Behavior.FirstPerson:
                case Behavior.Spectator:
                {
                    RotateFirstPerson(headingDegrees, pitchDegrees);
                    break;
                }

                case Behavior.Flight:
                {
                    RotateFlight(headingDegrees, pitchDegrees, rollDegrees);
                    break;
                }

                case Behavior.Orbit:
                {
                    RotateOrbit(headingDegrees, pitchDegrees, rollDegrees);
                    break;
                }

                default:
                {
                    break;
                }
            }

            UpdateViewMatrix();
        }

        /// <summary>
        /// Undo any camera rolling by leveling the camera. When the camera is
        /// orbiting this method will cause the camera to become level with the
        /// orbit target.
        /// </summary>
        public void UndoRoll()
        {
            if (behavior == Behavior.Orbit)
                LookAt(eye, target, targetYAxis);
            else
                LookAt(eye, eye + ViewDirection, WORLD_Y_AXIS);
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
            if (behavior == Behavior.Orbit)
            {
                Vector3 offset = eye - target;

                orbitOffsetLength = offset.Length();
                offset.Normalize();
                orbitOffsetLength += zoom;
                orbitOffsetLength = Math.Min(Math.Max(orbitOffsetLength, minZoom), maxZoom);
                offset *= orbitOffsetLength;
                eye = offset + target;
                UpdateViewMatrix();
            }
            else
            {
                zoom = Math.Min(Math.Max(zoom, minZoom), maxZoom);
                Perspective(zoom, aspectRatio, znear, zfar);
            }
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Change to a new camera behavior.
        /// </summary>
        /// <param name="newBehavior">The new camera behavior.</param>
        private void ChangeBehavior(Behavior newBehavior)
        {
            Behavior prevBehavior = behavior;

            if (prevBehavior == newBehavior)
            {
                return;
            }

            behavior = newBehavior;

            switch (newBehavior)
            {
                case Behavior.FirstPerson:
                {
                    switch (prevBehavior)
                    {
                        case Behavior.Flight:
                        case Behavior.Spectator:
                        eye.Y = firstPersonYOffset;
                        UpdateViewMatrix();
                        break;

                        case Behavior.Orbit:
                        eye.X = savedEye.X;
                        eye.Z = savedEye.Z;
                        eye.Y = firstPersonYOffset;
                        orientation = savedOrientation;
                        accumPitchDegrees = savedAccumPitchDegrees;
                        UpdateViewMatrix();
                        break;

                        default:
                        break;
                    }

                    this.UndoRoll();
                    break;
                }

                case Behavior.Spectator:
                {
                    switch (prevBehavior)
                    {
                        case Behavior.Flight:
                        {
                            this.UpdateViewMatrix();
                            break;
                        }

                        case Behavior.Orbit:
                        {
                            eye = savedEye;
                            orientation = savedOrientation;
                            accumPitchDegrees = savedAccumPitchDegrees;
                            UpdateViewMatrix();
                            break;
                        }

                        default:
                        {
                            break;
                        }
                    }

                    this.UndoRoll();
                    break;
                }

                case Behavior.Flight:
                {
                    if (prevBehavior == Behavior.Orbit)
                    {
                        eye = savedEye;
                        orientation = savedOrientation;
                        accumPitchDegrees = savedAccumPitchDegrees;
                        UpdateViewMatrix();
                    }
                    else
                    {
                        savedEye = eye;
                        UpdateViewMatrix();
                    }
                    break;
                }

                case Behavior.Orbit:
                {
                    if (prevBehavior == Behavior.FirstPerson)
                    {
                        firstPersonYOffset = eye.Y;
                    }

                    savedEye = eye;
                    savedOrientation = orientation;
                    savedAccumPitchDegrees = accumPitchDegrees;

                    targetYAxis = yAxis;

                    Vector3 newEye = eye + zAxis * orbitOffsetLength;

                    this.LookAt(newEye, eye, targetYAxis);
                    break;
                }

                default:
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Sets a new camera orientation.
        /// </summary>
        /// <param name="newOrientation">The new orientation.</param>
        private void ChangeOrientation(Quaternion newOrientation)
        {
            Matrix m = Matrix.RotationQuaternion(newOrientation);

            // Store the pitch for this new orientation.
            // First person and spectator behaviors limit pitching to
            // 90 degrees straight up and down.

            float pitch = (float)Math.Asin(m.M23);

            accumPitchDegrees = MathHelpers.ToDegrees(pitch);

            // First person and spectator behaviors don't allow rolling.
            // Negate any rolling that might be encoded in the new orientation.

            orientation = newOrientation;

            if (behavior == Behavior.FirstPerson || behavior == Behavior.Spectator)
                LookAt(eye, eye + Vector3.Negate(zAxis), WORLD_Y_AXIS);

            UpdateViewMatrix();
        }

        /// <summary>
        /// Rotates the camera for first person and spectator behaviors.
        /// Pitching is limited to 90 degrees straight up and down.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation angle.</param>
        /// <param name="pitchDegrees">X axis rotation angle.</param>
        private void RotateFirstPerson(float headingDegrees, float pitchDegrees)
        {
            accumPitchDegrees += pitchDegrees;

            if (accumPitchDegrees > 90.0f)
            {
                pitchDegrees = 90.0f - (accumPitchDegrees - pitchDegrees);
                accumPitchDegrees = 90.0f;
            }

            if (accumPitchDegrees < -90.0f)
            {
                pitchDegrees = -90.0f - (accumPitchDegrees - pitchDegrees);
                accumPitchDegrees = -90.0f;
            }

            float heading = MathHelpers.ToRadians(headingDegrees);
            float pitch = MathHelpers.ToRadians(pitchDegrees);
            Quaternion rotation = Quaternion.Identity;

            // Rotate the camera about the world Y axis.
            if (heading != 0.0f)
            {
                Quaternion.RotationAxis(ref WORLD_Y_AXIS, heading, out rotation);
                Quaternion.Multiply(ref rotation, ref orientation, out orientation);
            }

            // Rotate the camera about its local X axis.
            if (pitch != 0.0f)
            {
                Quaternion.RotationAxis(ref WORLD_X_AXIS, pitch, out rotation);
                Quaternion.Multiply(ref orientation, ref rotation, out orientation);
            }
        }

        /// <summary>
        /// Rotates the camera for flight behavior.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation angle.</param>
        /// <param name="pitchDegrees">X axis rotation angle.</param>
        /// <param name="rollDegrees">Z axis rotation angle.</param>
        private void RotateFlight(float headingDegrees, float pitchDegrees, float rollDegrees)
        {
            accumPitchDegrees += pitchDegrees;

            if (accumPitchDegrees > 360.0f)
                accumPitchDegrees -= 360.0f;

            if (accumPitchDegrees < -360.0f)
                accumPitchDegrees += 360.0f;

            float heading = MathHelpers.ToRadians(headingDegrees);
            float pitch = MathHelpers.ToRadians(pitchDegrees);
            float roll = MathHelpers.ToRadians(rollDegrees);

            Quaternion rotation = Quaternion.RotationYawPitchRoll(heading, pitch, roll);
            Quaternion.Multiply(ref orientation, ref rotation, out orientation);
        }

        /// <summary>
        /// Rotates the camera for orbit behavior. Rotations are either about
        /// the camera's local y axis or the orbit target's y axis. The property
        /// PreferTargetYAxisOrbiting controls which rotation method to use.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation angle.</param>
        /// <param name="pitchDegrees">X axis rotation angle.</param>
        /// <param name="rollDegrees">Z axis rotation angle.</param>
        private void RotateOrbit(float headingDegrees, float pitchDegrees, float rollDegrees)
        {
            float heading = MathHelpers.ToRadians(headingDegrees);
            float pitch = MathHelpers.ToRadians(pitchDegrees);

            if (preferTargetYAxisOrbiting)
            {
                Quaternion rotation = Quaternion.Identity;

                if (heading != 0.0f)
                {
                    Quaternion.RotationAxis(ref targetYAxis, heading, out rotation);
                    Quaternion.Multiply(ref rotation, ref orientation, out orientation);
                }

                if (pitch != 0.0f)
                {
                    Quaternion.RotationAxis(ref WORLD_X_AXIS, pitch, out rotation);
                    Quaternion.Multiply(ref orientation, ref rotation, out orientation);
                }
            }
            else
            {
                float roll = MathHelpers.ToRadians(rollDegrees);
                Quaternion rotation = Quaternion.RotationYawPitchRoll(heading, pitch, roll);
                Quaternion.Multiply(ref orientation, ref rotation, out orientation);
            }
        }

        /// <summary>
        /// Rebuild the view matrix.
        /// </summary>
        private void UpdateViewMatrix()
        {
            Matrix.RotationQuaternion(ref orientation, out viewMatrix);

            xAxis.X = viewMatrix.M11;
            xAxis.Y = viewMatrix.M21;
            xAxis.Z = viewMatrix.M31;

            yAxis.X = viewMatrix.M12;
            yAxis.Y = viewMatrix.M22;
            yAxis.Z = viewMatrix.M32;

            zAxis.X = viewMatrix.M13;
            zAxis.Y = viewMatrix.M23;
            zAxis.Z = viewMatrix.M33;

            if (behavior == Behavior.Orbit)
            {
                // Calculate the new camera position based on the current
                // orientation. The camera must always maintain the same
                // distance from the target. Use the current offset vector
                // to determine the correct distance from the target.

                eye = target + zAxis * orbitOffsetLength;
            }

            viewMatrix.M41 = -Vector3.Dot(xAxis, eye);
            viewMatrix.M42 = -Vector3.Dot(yAxis, eye);
            viewMatrix.M43 = -Vector3.Dot(zAxis, eye);

            viewDir.X = -zAxis.X;
            viewDir.Y = -zAxis.Y;
            viewDir.Z = -zAxis.Z;
        }

        #endregion
        #region Properties

        /// <summary>
        /// Property to get and set the camera's behavior.
        /// </summary>
        public Behavior CurrentBehavior
        {
            get { return behavior; }
            set { ChangeBehavior(value); }
        }

        /// <summary>
        /// Property to get and set the max orbit zoom distance.
        /// </summary>
        public float OrbitMaxZoom
        {
            get { return orbitMaxZoom; }
            set { orbitMaxZoom = value; }
        }

        /// <summary>
        /// Property to get and set the min orbit zoom distance.
        /// </summary>
        public float OrbitMinZoom
        {
            get { return orbitMinZoom; }
            set { orbitMinZoom = value; }
        }

        /// <summary>
        /// Property to get and set the distance from the target when orbiting.
        /// </summary>
        public float OrbitOffsetDistance
        {
            get { return orbitOffsetLength; }
            set { orbitOffsetLength = value; }
        }

        /// <summary>
        /// Property to get and set the camera orbit target position.
        /// </summary>
        public Vector3 OrbitTarget
        {
            get { return target; }
            set { target = value; }
        }

        /// <summary>
        /// Property to get and set the camera orientation.
        /// </summary>
        public Quaternion Orientation
        {
            get { return orientation; }
            set { ChangeOrientation(value); }
        }

        /// <summary>
        /// Property to get and set the camera position.
        /// </summary>
        public Vector3 Position
        {
            get { return eye; }

            set
            {
                eye = value;
                UpdateViewMatrix();
            }
        }

        /// <summary>
        /// Property to get and set the flag to force the camera
        /// to orbit around the orbit target's Y axis rather than the camera's
        /// local Y axis.
        /// </summary>
        public bool PreferTargetYAxisOrbiting
        {
            get { return preferTargetYAxisOrbiting; }

            set
            {
                preferTargetYAxisOrbiting = value;

                if (preferTargetYAxisOrbiting)
                    UndoRoll();
            }
        }

        /// <summary>
        /// Property to get the perspective projection matrix.
        /// </summary>
        public Matrix ProjectionMatrix
        {
            get { return projMatrix; }
        }

        /// <summary>
        /// Property to get the viewing direction vector.
        /// </summary>
        public Vector3 ViewDirection
        {
            get { return viewDir; }
        }

        /// <summary>
        /// Property to get the view matrix.
        /// </summary>
        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }

        /// <summary>
        /// Property to get the concatenated view-projection matrix.
        /// </summary>
        public Matrix ViewProjectionMatrix
        {
            get { return viewMatrix * projMatrix; }
        }

        /// <summary>
        /// Property to get the camera's local X axis.
        /// </summary>
        public Vector3 XAxis
        {
            get { return xAxis; }
        }

        /// <summary>
        /// Property to get the camera's local Y axis.
        /// </summary>
        public Vector3 YAxis
        {
            get { return yAxis; }
        }

        /// <summary>
        /// Property to get the camera's local Z axis.
        /// </summary>
        public Vector3 ZAxis
        {
            get { return zAxis; }
        }

        #endregion
    }
}
