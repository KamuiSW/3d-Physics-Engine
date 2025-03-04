using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameEngine.Core.Scene;
using OpenTK.Mathematics;

namespace GameEngine.Editor.ViewModels
{
    public class TransformViewModel : INotifyPropertyChanged
    {
        private readonly Transform _transform;

        public TransformViewModel(Transform transform)
        {
            _transform = transform;
            _transform.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
        }

        public Vector3 Position
        {
            get => _transform.Position;
            set => _transform.Position = value;
        }

        public Vector3 Scale
        {
            get => _transform.Scale;
            set => _transform.Scale = value;
        }

        // Convert Quaternion to Euler angles for the UI
        public Vector3 Rotation
        {
            get
            {
                var rotation = _transform.Rotation;
                var euler = rotation.ToEulerAngles();
                return new Vector3(
                    MathHelper.RadiansToDegrees(euler.X),
                    MathHelper.RadiansToDegrees(euler.Y),
                    MathHelper.RadiansToDegrees(euler.Z)
                );
            }
            set
            {
                var radians = new Vector3(
                    MathHelper.DegreesToRadians(value.X),
                    MathHelper.DegreesToRadians(value.Y),
                    MathHelper.DegreesToRadians(value.Z)
                );
                _transform.Rotation = Quaternion.FromEulerAngles(radians);
            }
        }

        public string Name => "Transform";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 