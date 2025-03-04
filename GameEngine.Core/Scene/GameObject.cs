using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace GameEngine.Core.Scene
{
    public class GameObject : INotifyPropertyChanged
    {
        private string _name = "GameObject";
        private bool _isActive = true;
        private GameObject? _parent;
        private ObservableCollection<GameObject> _children = new();
        private ObservableCollection<Component> _components = new();

        [JsonProperty]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        [JsonProperty]
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        [JsonIgnore]
        public GameObject? Parent
        {
            get => _parent;
            private set => SetProperty(ref _parent, value);
        }

        [JsonProperty]
        public ObservableCollection<GameObject> Children => _children;

        [JsonProperty]
        public ObservableCollection<Component> Components => _components;

        public event PropertyChangedEventHandler? PropertyChanged;

        public GameObject(string name = "GameObject")
        {
            Name = name;
        }

        public void AddChild(GameObject child)
        {
            if (child.Parent != null)
            {
                child.Parent.Children.Remove(child);
            }
            child.Parent = this;
            Children.Add(child);
        }

        public void RemoveChild(GameObject child)
        {
            if (Children.Remove(child))
            {
                child.Parent = null;
            }
        }

        public T AddComponent<T>() where T : Component, new()
        {
            var component = new T { GameObject = this };
            Components.Add(component);
            return component;
        }

        public T? GetComponent<T>() where T : Component
        {
            foreach (var component in Components)
            {
                if (component is T typedComponent)
                {
                    return typedComponent;
                }
            }
            return null;
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
} 