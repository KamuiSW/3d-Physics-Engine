using OpenTK.Mathematics;
using System;

namespace GameEngine
{
    public class Camera : Component
    {
        public Vector3 Position { get; set; }
        public Vector3 Front { get; private set; }
        public Vector3 Up { get; private set; }
        public Vector3 Right { get; private set; }

        public float Yaw { get; set; } = -90.0f;
        public float Pitch { get; set; } = 0.0f;
        
        private float fov = 45.0f;
        public float Fov
        {
            get => fov;
            set => fov = MathHelper.Clamp(value, 1.0f, 90.0f);
        }

        public Camera(Vector3 position)
        {
            Position = position;
            Up = Vector3.UnitY;
            UpdateVectors();
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + Front, Up);
        }

        public Matrix4 GetProjectionMatrix(float aspectRatio)
        {
            return Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(Fov),
                aspectRatio,
                0.1f,
                100.0f);
        }

        private void UpdateVectors()
        {
            Front = new Vector3(
                MathF.Cos(MathHelper.DegreesToRadians(Pitch)) * MathF.Cos(MathHelper.DegreesToRadians(Yaw)),
                MathF.Sin(MathHelper.DegreesToRadians(Pitch)),
                MathF.Cos(MathHelper.DegreesToRadians(Pitch)) * MathF.Sin(MathHelper.DegreesToRadians(Yaw))
            ).Normalized();

            Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }

        public override void Update(float deltaTime)
        {
            UpdateVectors();
        }
    }
} 