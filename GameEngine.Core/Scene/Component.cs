using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace GameEngine.Core.Scene
{
    public abstract class Component : INotifyPropertyChanged
    {
        private bool _isEnabled = true;
        private GameObject? _gameObject;

        [JsonProperty]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        [JsonIgnore]
        public GameObject? GameObject
        {
            get => _gameObject;
            internal set => SetProperty(ref _gameObject, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnDestroy() { }
    }
} 