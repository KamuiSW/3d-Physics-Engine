using OpenTK.Mathematics;
using Newtonsoft.Json;

namespace GameEngine.Core.Scene.Components
{
    public class Transform : Component
    {
        private Vector3 _position = Vector3.Zero;
        private Vector3 _rotation = Vector3.Zero;
        private Vector3 _scale = Vector3.One;

        [JsonProperty]
        public Vector3 Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        [JsonProperty]
        public Vector3 Rotation
        {
            get => _rotation;
            set => SetProperty(ref _rotation, value);
        }

        [JsonProperty]
        public Vector3 Scale
        {
            get => _scale;
            set => SetProperty(ref _scale, value);
        }

        [JsonIgnore]
        public Matrix4 LocalMatrix
        {
            get
            {
                var translation = Matrix4.CreateTranslation(Position);
                var rotation = Matrix4.CreateRotationX(Rotation.X) *
                             Matrix4.CreateRotationY(Rotation.Y) *
                             Matrix4.CreateRotationZ(Rotation.Z);
                var scale = Matrix4.CreateScale(Scale);
                return scale * rotation * translation;
            }
        }

        [JsonIgnore]
        public Matrix4 WorldMatrix
        {
            get
            {
                var matrix = LocalMatrix;
                var parent = GameObject?.Parent?.GetComponent<Transform>();
                if (parent != null)
                {
                    matrix *= parent.WorldMatrix;
                }
                return matrix;
            }
        }
    }
} 