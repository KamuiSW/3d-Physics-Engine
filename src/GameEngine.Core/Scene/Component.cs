using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameEngine.Core.Scene
{
    public abstract class Component : INotifyPropertyChanged
    {
        private bool _enabled = true;
        private SceneObject? _gameObject;

        public SceneObject GameObject
        {
            get => _gameObject ?? throw new InvalidOperationException("GameObject is not set");
            internal set
            {
                if (_gameObject != value)
                {
                    _gameObject = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged();
                    if (GameObject != null && GameObject.IsActive)
                    {
                        if (_enabled)
                            OnEnable();
                        else
                            OnDisable();
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Transform Transform => GameObject.Transform;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update(float deltaTime) { }
        public virtual void LateUpdate(float deltaTime) { }
        public virtual void FixedUpdate(float deltaTime) { }
        protected internal virtual void OnEnable() { }
        protected internal virtual void OnDisable() { }
        public virtual void OnDestroy() { }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
} 