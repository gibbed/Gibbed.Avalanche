using System;
using System.Windows.Forms;

using Microsoft.Xna.Framework;

namespace Gibbed.Avalanche.ModelViewer
{
    internal class Camera
    {
        private Control Viewport;

        public float MaxLookAngle { get; set; }
        public float MinLookAngle { get; set; }
        public float MaxMoveSpeed { get; set; }
        public float MinMoveSpeed { get; set; }
        public float MaxLookSpeed { get; set; }
        public float MinLookSpeed { get; set; }
        public float MaxMoveAccel { get; set; }
        public float MinMoveAccel { get; set; }
        public float MaxLookAccel { get; set; }
        public float MinLookAccel { get; set; }
        public float MinFieldOfView { get; set; }
        public float MaxFieldOfView { get; set; }

        // orientation
        public readonly Matrix World = Matrix.Identity;
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }

        // need to track velocities
        public bool HasChanged { get; private set; }

        private Vector3 Position;   // Point3
        private Vector3 Velocity;

        public Vector3 ForwardDirection;
        public Vector3 VerticalDirection;
        public Vector3 HorizontalDirection;
        public Vector3 LookAt;

        private Vector2 LookAngle;
        private Vector2 LookVelocity;

        private float FieldOfView;
        private float SpeedScale;
        private float LookSpeedScale;
        private float MoveAcceleration;
        private float LookAcceleration;
        private float ZoomVelocity;
        private float ViewDistance;

        public Camera(Control viewport)
        {
            this.Viewport = viewport;

            this.SpeedScale = 0.05f;
            this.LookSpeedScale = 0.35f;
            this.MoveAcceleration = 0.4f;    // lower the value, the slower the acceleration (1 = instantaneous movement)
            this.LookAcceleration = 0.5f;
            this.ZoomVelocity = 0;
            this.ViewDistance = 1000;
            this.MaxLookAngle = 1.57f;
            this.MinLookAngle = -1.57f;
            this.MaxMoveSpeed = 5.0f;
            this.MinMoveSpeed = 0.01f;
            this.MaxLookSpeed = 5.0f;
            this.MinLookSpeed = 0.01f;
            this.MaxMoveAccel = 1.0f;
            this.MinMoveAccel = 0.01f;
            this.MaxLookAccel = 1.0f;
            this.MinLookAccel = 0.01f;
            this.FieldOfView = DegreesToRadians(90.0f);
            this.MinFieldOfView = DegreesToRadians(0.05f);
            this.MaxFieldOfView = DegreesToRadians(120.0f);
        }

        // immediates
        public void MoveTo(Vector3 position)
        {
            this.Position = position;
            this.HasChanged = true;
        }

        public void LookTo(Vector2 direction)
        {
            this.LookAngle = direction;
            this.HasChanged = true;
        }

        // acceleration
        public void ApplyForce(Vector3 force)
        {
            this.Velocity += force * SpeedScale;
            this.HasChanged = true;
        }

        public void ApplyLookForce(Vector2 force)
        {
            this.LookVelocity = force * LookSpeedScale;
            this.HasChanged = true;
        }
        
        public void ApplyZoomForce(float force)
        {
            this.ZoomVelocity += force;
            this.HasChanged = true;
        }

        public void Update()
        {
            // update velocities and changed status
            this.Velocity -= this.Velocity * this.MoveAcceleration;
            this.LookVelocity -= this.LookVelocity * this.LookAcceleration;
            this.ZoomVelocity -= this.ZoomVelocity * 0.2f;
            this.HasChanged =
                this.Velocity.Length() != 0 ||
                this.LookVelocity.Length() != 0 ||
                this.ZoomVelocity != 0;

            // update position and look angles based on velocities
            this.Position += this.Velocity;
            this.LookAngle += this.LookVelocity;
            this.FieldOfView *= 1.0f + this.ZoomVelocity;

            // clamp look angles and fov
            // [-2PI, 2PI]
            this.LookAngle.X = (float)(((this.LookAngle.X / (Math.PI * 2)) -(int)(this.LookAngle.X / (Math.PI * 2))) * Math.PI * 2);
            this.LookAngle.Y = Clamp(this.LookAngle.Y, this.MinLookAngle, this.MaxLookAngle);

            this.FieldOfView = Clamp(this.FieldOfView, this.MinFieldOfView, this.MaxFieldOfView);

            // do some math...
            float sh = (float)Math.Sin(LookAngle.X);
            float sv = (float)Math.Sin(LookAngle.Y);
            float ch = (float)Math.Cos(LookAngle.X);
            float cv = (float)Math.Cos(LookAngle.Y);
            float cx = (float)Math.Cos(LookAngle.X + 1.57f);
            float sx = (float)Math.Sin(LookAngle.X + 1.57f);

            // calculate new look directions
            this.ForwardDirection = new Vector3(ch * cv, sv, sh * cv);
            this.VerticalDirection = new Vector3(-ch * sv, cv, -sh * sv);
            this.HorizontalDirection = new Vector3(cx, 0, sx); // left
            this.LookAt = this.Position + this.ForwardDirection; // destination

            // calculate transformations
            this.View = Matrix.CreateLookAt(this.Position, this.LookAt, new Vector3(0, 1, 0)); // use this as up, since they have their cars sideways
            this.Projection = Matrix.CreatePerspectiveFieldOfView(this.FieldOfView, (float)this.Viewport.Width / (float)this.Viewport.Height, 0.1f, this.ViewDistance);
        }

        public void Zoom(float scale)
        {
            this.FieldOfView = Clamp(this.FieldOfView * scale, this.MinFieldOfView, this.MaxFieldOfView);
        }

        private static float DegreesToRadians(float degrees)
        {
            return (float)(degrees * (Math.PI / 180));
        }

        private static float RadiansToDegrees(float radians)
        {
            return (float)(radians * (180 / Math.PI));
        }

        private static float Clamp(float value, float lowerBound, float upperBound)
        {
            if (value > upperBound)
            {
                return upperBound;
            }
            else if (value < lowerBound)
            {
                return lowerBound;
            }
            else
            {
                return value;
            }
        }
    }
}
