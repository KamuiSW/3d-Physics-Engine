using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameEngine.Core.Scene
{
    public class SceneObject : INotifyPropertyChanged
    {
        private string _name = "GameObject";
        private bool _isActive = true;
        private SceneObject? _parent;
        private readonly List<SceneObject> _children = new List<SceneObject>();
        private readonly List<Component> _components = new List<Component>();
        private Transform _transform;

        public SceneObject()
        {
            _transform = AddComponent<Transform>();
        }

        public SceneObject(string name) : this()
        {
            Name = name;
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged();
                    
                    if (_isActive)
                        OnEnable();
                    else
                        OnDisable();
                }
            }
        }

        public SceneObject? Parent
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    OnPropertyChanged();
                }
            }
        }

        public Transform Transform => _transform;

        public IReadOnlyList<SceneObject> Children => _children;
        public IReadOnlyList<Component> Components => _components;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public T AddComponent<T>() where T : Component, new()
        {
            var component = new T();
            component.GameObject = this;
            _components.Add(component);
            
            if (IsActive && component.Enabled)
                component.Start();
                
            return component;
        }

        public void RemoveComponent(Component component)
        {
            _components.Remove(component);
        }

        public T? GetComponent<T>() where T : Component
        {
            return (T?)_components.Find(c => c is T);
        }

        public void AddChild(SceneObject child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));
                
            if (child.Parent != null)
                child.Parent._children.Remove(child);
                
            child.Parent = this;
            _children.Add(child);
        }

        public void RemoveChild(SceneObject child)
        {
            _children.Remove(child);
            child.Parent = null;
        }

        protected virtual void OnEnable()
        {
            foreach (var component in _components)
            {
                if (component.Enabled)
                    component.OnEnable();
            }
            
            foreach (var child in _children)
            {
                if (child.IsActive)
                    child.OnEnable();
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var component in _components)
            {
                if (component.Enabled)
                    component.OnDisable();
            }
            
            foreach (var child in _children)
            {
                if (child.IsActive)
                    child.OnDisable();
            }
        }
    }
} 