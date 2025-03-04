using Avalonia.Input;
using OpenTK.Mathematics;
using System;

namespace GameEngine.Editor.Viewport
{
    public class CameraController
    {
        private Vector3 _position;
        private Vector3 _target;
        private Vector3 _up = Vector3.UnitY;
        private float _yaw = -90f;
        private float _pitch;
        private float _distance = 10f;
        private bool _isDragging;
        private Point _lastMousePosition;

        public Vector3 Position => _position;
        public Vector3 Target => _target;
        public Vector3 Up => _up;

        public CameraController()
        {
            UpdateCameraPosition();
        }

        public void OnMouseWheel(PointerWheelEventArgs e)
        {
            _distance = Math.Clamp(_distance - e.Delta.Y * 0.5f, 1f, 50f);
            UpdateCameraPosition();
        }

        public void OnMouseDown(PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(null).Properties.IsMiddleButtonPressed)
            {
                _isDragging = true;
                _lastMousePosition = e.GetPosition(null);
            }
        }

        public void OnMouseUp(PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton == MouseButton.Middle)
            {
                _isDragging = false;
            }
        }

        public void OnMouseMove(PointerEventArgs e)
        {
            if (!_isDragging) return;

            var currentPosition = e.GetPosition(null);
            var delta = currentPosition - _lastMousePosition;

            if (e.KeyModifiers.HasFlag(KeyModifiers.Alt))
            {
                // Orbit
                _yaw += delta.X * 0.5f;
                _pitch = Math.Clamp(_pitch - delta.Y * 0.5f, -89f, 89f);
            }
            else
            {
                // Pan
                var right = Vector3.Cross(Vector3.UnitY, (_position - _target).Normalized());
                var up = Vector3.Cross((_position - _target).Normalized(), right);

                _target -= right * (float)delta.X * 0.01f * _distance;
                _target += up * (float)delta.Y * 0.01f * _distance;
            }

            UpdateCameraPosition();
            _lastMousePosition = currentPosition;
        }

        private void UpdateCameraPosition()
        {
            var direction = new Vector3(
                (float)(Math.Cos(MathHelper.DegreesToRadians(_yaw)) * Math.Cos(MathHelper.DegreesToRadians(_pitch))),
                (float)Math.Sin(MathHelper.DegreesToRadians(_pitch)),
                (float)(Math.Sin(MathHelper.DegreesToRadians(_yaw)) * Math.Cos(MathHelper.DegreesToRadians(_pitch)))
            );

            _position = _target + direction.Normalized() * _distance;
        }
    }
} 