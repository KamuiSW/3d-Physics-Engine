using OpenTK.Mathematics;
using System;
using GameEngine.Core.Scene;

namespace GameEngine.Core.Scene
{
    public class Transform : Component
    {
        private Vector3 _position = Vector3.Zero;
        private Quaternion _rotation = Quaternion.Identity;
        private Vector3 _scale = Vector3.One;
        private Matrix4 _worldMatrix;
        private bool _isDirty = true;

        public Vector3 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    SetDirty();
                    OnPropertyChanged();
                }
            }
        }

        public Quaternion Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    SetDirty();
                    OnPropertyChanged();
                }
            }
        }

        public Vector3 Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    SetDirty();
                    OnPropertyChanged();
                }
            }
        }

        public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, Rotation);
        public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);
        public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);

        public Matrix4 WorldMatrix
        {
            get
            {
                if (_isDirty)
                {
                    RecalculateMatrix();
                }
                return _worldMatrix;
            }
        }

        private void RecalculateMatrix()
        {
            Matrix4 translationMatrix = Matrix4.CreateTranslation(_position);
            Matrix4 rotationMatrix = Matrix4.CreateFromQuaternion(_rotation);
            Matrix4 scaleMatrix = Matrix4.CreateScale(_scale);

            _worldMatrix = scaleMatrix * rotationMatrix * translationMatrix;

            if (GameObject?.Parent != null)
            {
                Transform parentTransform = GameObject.Parent.Transform;
                _worldMatrix *= parentTransform.WorldMatrix;
            }

            _isDirty = false;
        }

        private void SetDirty()
        {
            _isDirty = true;
            
            if (GameObject != null)
            {
                foreach (var child in GameObject.Children)
                {
                    child.Transform.SetDirty();
                }
            }
        }

        public void Translate(Vector3 translation)
        {
            Position += translation;
        }

        public void Rotate(Vector3 eulerAngles)
        {
            Quaternion rotation = Quaternion.FromEulerAngles(
                MathHelper.DegreesToRadians(eulerAngles.X),
                MathHelper.DegreesToRadians(eulerAngles.Y),
                MathHelper.DegreesToRadians(eulerAngles.Z));
                
            Rotation = Rotation * rotation;
        }

        public void LookAt(Vector3 target)
        {
            Vector3 direction = (target - Position).Normalized();
            Vector3 right = Vector3.Cross(Vector3.UnitY, direction).Normalized();
            Vector3 up = Vector3.Cross(direction, right);

            Matrix3 rotationMatrix = new Matrix3(
                right.X, up.X, direction.X,
                right.Y, up.Y, direction.Y,
                right.Z, up.Z, direction.Z
            );

            Rotation = Quaternion.FromMatrix(rotationMatrix);
        }
    }
} 