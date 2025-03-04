using OpenTK.Mathematics;
using System;

namespace GameEngine.Core.Scene.Components
{
    public class Camera : Component
    {
        private float _fieldOfView = 60.0f;
        private float _nearPlane = 0.1f;
        private float _farPlane = 1000.0f;
        private float _aspectRatio = 16.0f / 9.0f;
        private bool _orthographic;
        private float _orthographicSize = 5.0f;
        private Matrix4 _projectionMatrix;
        private Matrix4 _viewMatrix;
        private bool _isDirty = true;

        public float FieldOfView
        {
            get => _fieldOfView;
            set
            {
                value = MathHelper.Clamp(value, 1.0f, 179.0f);
                if (_fieldOfView != value)
                {
                    _fieldOfView = value;
                    _isDirty = true;
                    OnPropertyChanged();
                }
            }
        }

        public float NearPlane
        {
            get => _nearPlane;
            set
            {
                if (value > 0 && _nearPlane != value)
                {
                    _nearPlane = value;
                    _isDirty = true;
                    OnPropertyChanged();
                }
            }
        }

        public float FarPlane
        {
            get => _farPlane;
            set
            {
                if (value > _nearPlane && _farPlane != value)
                {
                    _farPlane = value;
                    _isDirty = true;
                    OnPropertyChanged();
                }
            }
        }

        public float AspectRatio
        {
            get => _aspectRatio;
            set
            {
                if (value > 0 && _aspectRatio != value)
                {
                    _aspectRatio = value;
                    _isDirty = true;
                    OnPropertyChanged();
                }
            }
        }

        public bool Orthographic
        {
            get => _orthographic;
            set
            {
                if (_orthographic != value)
                {
                    _orthographic = value;
                    _isDirty = true;
                    OnPropertyChanged();
                }
            }
        }

        public float OrthographicSize
        {
            get => _orthographicSize;
            set
            {
                if (value > 0 && _orthographicSize != value)
                {
                    _orthographicSize = value;
                    _isDirty = true;
                    OnPropertyChanged();
                }
            }
        }

        public Matrix4 ProjectionMatrix
        {
            get
            {
                if (_isDirty)
                {
                    RecalculateMatrices();
                }
                return _projectionMatrix;
            }
        }

        public Matrix4 ViewMatrix
        {
            get
            {
                if (_isDirty)
                {
                    RecalculateMatrices();
                }
                return _viewMatrix;
            }
        }

        private void RecalculateMatrices()
        {
            if (_orthographic)
            {
                float height = _orthographicSize * 2.0f;
                float width = height * _aspectRatio;
                _projectionMatrix = Matrix4.CreateOrthographic(width, height, _nearPlane, _farPlane);
            }
            else
            {
                _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                    MathHelper.DegreesToRadians(_fieldOfView),
                    _aspectRatio,
                    _nearPlane,
                    _farPlane
                );
            }

            _viewMatrix = Transform.WorldMatrix.Inverted();
            _isDirty = false;
        }

        public Ray ScreenPointToRay(Vector2 screenPoint, float screenWidth, float screenHeight)
        {
            Vector2 normalizedPoint = new Vector2(
                (2.0f * screenPoint.X) / screenWidth - 1.0f,
                1.0f - (2.0f * screenPoint.Y) / screenHeight
            );

            Vector4 rayClip = new Vector4(normalizedPoint.X, normalizedPoint.Y, -1.0f, 1.0f);
            Vector4 rayEye = ProjectionMatrix.Inverted() * rayClip;
            rayEye = new Vector4(rayEye.X, rayEye.Y, -1.0f, 0.0f);

            Vector4 rayWorld = Transform.WorldMatrix * rayEye;
            Vector3 rayDirection = new Vector3(rayWorld.X, rayWorld.Y, rayWorld.Z).Normalized();

            return new Ray(Transform.Position, rayDirection);
        }

        public Vector3 WorldToScreenPoint(Vector3 worldPoint, float screenWidth, float screenHeight)
        {
            // Transform world point to clip space
            Vector4 worldPos = new Vector4(worldPoint, 1.0f);
            Vector4 viewPos = TransformVector4(worldPos, ViewMatrix);
            Vector4 clipPos = TransformVector4(viewPos, ProjectionMatrix);

            // Perspective divide
            Vector3 ndc = new Vector3(
                clipPos.X / clipPos.W,
                clipPos.Y / clipPos.W,
                clipPos.Z / clipPos.W
            );

            // Convert to screen space
            return new Vector3(
                (ndc.X + 1.0f) * screenWidth * 0.5f,
                (1.0f - ndc.Y) * screenHeight * 0.5f,
                ndc.Z
            );
        }

        public Frustum GetFrustum()
        {
            return new Frustum(ViewMatrix * ProjectionMatrix);
        }

        private Vector4 TransformVector4(Vector4 vector, Matrix4 matrix)
        {
            return new Vector4(
                Vector4.Dot(new Vector4(matrix.Row0), vector),
                Vector4.Dot(new Vector4(matrix.Row1), vector),
                Vector4.Dot(new Vector4(matrix.Row2), vector),
                Vector4.Dot(new Vector4(matrix.Row3), vector)
            );
        }
    }

    public enum ProjectionType
    {
        Perspective,
        Orthographic
    }

    public struct Ray
    {
        public Vector3 Origin { get; }
        public Vector3 Direction { get; }

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction.Normalized();
        }

        public Vector3 GetPoint(float distance)
        {
            return Origin + Direction * distance;
        }
    }

    public struct Frustum
    {
        public Plane[] Planes { get; }

        public Frustum(Matrix4 viewProjection)
        {
            Planes = new Plane[6];

            // Left plane
            Planes[0] = new Plane(
                viewProjection.M14 + viewProjection.M11,
                viewProjection.M24 + viewProjection.M21,
                viewProjection.M34 + viewProjection.M31,
                viewProjection.M44 + viewProjection.M41
            );

            // Right plane
            Planes[1] = new Plane(
                viewProjection.M14 - viewProjection.M11,
                viewProjection.M24 - viewProjection.M21,
                viewProjection.M34 - viewProjection.M31,
                viewProjection.M44 - viewProjection.M41
            );

            // Bottom plane
            Planes[2] = new Plane(
                viewProjection.M14 + viewProjection.M12,
                viewProjection.M24 + viewProjection.M22,
                viewProjection.M34 + viewProjection.M32,
                viewProjection.M44 + viewProjection.M42
            );

            // Top plane
            Planes[3] = new Plane(
                viewProjection.M14 - viewProjection.M12,
                viewProjection.M24 - viewProjection.M22,
                viewProjection.M34 - viewProjection.M32,
                viewProjection.M44 - viewProjection.M42
            );

            // Near plane
            Planes[4] = new Plane(
                viewProjection.M14 + viewProjection.M13,
                viewProjection.M24 + viewProjection.M23,
                viewProjection.M34 + viewProjection.M33,
                viewProjection.M44 + viewProjection.M43
            );

            // Far plane
            Planes[5] = new Plane(
                viewProjection.M14 - viewProjection.M13,
                viewProjection.M24 - viewProjection.M23,
                viewProjection.M34 - viewProjection.M33,
                viewProjection.M44 - viewProjection.M43
            );

            // Normalize all planes
            for (int i = 0; i < 6; i++)
            {
                Planes[i] = Planes[i].Normalized();
            }
        }
    }

    public struct Plane
    {
        public Vector3 Normal { get; }
        public float Distance { get; }

        public Plane(float a, float b, float c, float d)
        {
            Normal = new Vector3(a, b, c);
            float length = Normal.Length;
            Normal /= length;
            Distance = d / length;
        }

        public Plane Normalized()
        {
            float length = Normal.Length;
            return new Plane(
                Normal.X / length,
                Normal.Y / length,
                Normal.Z / length,
                Distance / length
            );
        }

        public float GetSignedDistanceToPoint(Vector3 point)
        {
            return Vector3.Dot(Normal, point) + Distance;
        }
    }
} 