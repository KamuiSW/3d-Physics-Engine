using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using GameEngine.Core.Scene;

namespace GameEngine.Core.Scene
{
    public class Scene
    {
        private string _name;
        private readonly List<SceneObject> _rootObjects = new List<SceneObject>();
        private readonly List<Component> _startQueue = new();
        private readonly List<Component> _updateList = new();
        private readonly List<Component> _lateUpdateList = new();
        private bool _isPlaying;
        private bool _isPaused;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public IReadOnlyList<SceneObject> RootObjects => _rootObjects;
        public bool IsPlaying => _isPlaying;
        public bool IsPaused => _isPaused;

        public Scene(string name = "New Scene")
        {
            _name = name;
        }

        public void AddRootObject(SceneObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (obj.Parent != null)
                throw new InvalidOperationException("Cannot add an object with a parent as a root object.");

            _rootObjects.Add(obj);
        }

        public void RemoveRootObject(SceneObject obj)
        {
            _rootObjects.Remove(obj);
        }

        public void Play()
        {
            if (_isPlaying)
                return;

            _isPlaying = true;
            _isPaused = false;

            foreach (var obj in _rootObjects)
            {
                InitializeObject(obj);
            }
        }

        private void InitializeObject(SceneObject obj)
        {
            if (!obj.IsActive)
                return;

            foreach (var component in obj.Components)
            {
                if (component.Enabled)  // Changed from IsEnabled to Enabled
                {
                    component.Start();
                }
            }

            foreach (var child in obj.Children)
            {
                InitializeObject(child);
            }
        }

        public void Pause()
        {
            if (!_isPlaying)
                return;

            _isPaused = !_isPaused;
        }

        public void Stop()
        {
            _isPlaying = false;
            _isPaused = false;
        }

        public void Update(float deltaTime)
        {
            if (!_isPlaying || _isPaused)
                return;

            foreach (var obj in _rootObjects)
            {
                UpdateObject(obj, deltaTime);  // Added deltaTime parameter
            }
        }

        private void UpdateObject(SceneObject obj, float deltaTime)  // Added deltaTime parameter
        {
            if (!obj.IsActive)
                return;

            foreach (var component in obj.Components)
            {
                if (component.Enabled)  // Changed from IsEnabled to Enabled
                {
                    component.Update(deltaTime);  // Added deltaTime parameter
                }
            }

            foreach (var child in obj.Children)
            {
                UpdateObject(child, deltaTime);  // Added deltaTime parameter
            }
        }

        public void LateUpdate(float deltaTime)
        {
            if (!_isPlaying || _isPaused)
                return;

            foreach (var obj in _rootObjects)
            {
                LateUpdateObject(obj, deltaTime);  // Added deltaTime parameter
            }
        }

        private void LateUpdateObject(SceneObject obj, float deltaTime)  // Added deltaTime parameter
        {
            if (!obj.IsActive)
                return;

            foreach (var component in obj.Components)
            {
                if (component.Enabled)  // Changed from IsEnabled to Enabled
                {
                    component.LateUpdate(deltaTime);  // Added deltaTime parameter
                }
            }

            foreach (var child in obj.Children)
            {
                LateUpdateObject(child, deltaTime);  // Added deltaTime parameter
            }
        }

        public void FixedUpdate(float deltaTime)
        {
            if (!_isPlaying || _isPaused)
                return;

            foreach (var obj in _rootObjects)
            {
                FixedUpdateObject(obj, deltaTime);  // Added deltaTime parameter
            }
        }

        private void FixedUpdateObject(SceneObject obj, float deltaTime)  // Added deltaTime parameter
        {
            if (!obj.IsActive)
                return;

            foreach (var component in obj.Components)
            {
                if (component.Enabled)  // Changed from IsEnabled to Enabled
                {
                    component.FixedUpdate(deltaTime);  // Added deltaTime parameter
                }
            }

            foreach (var child in obj.Children)
            {
                FixedUpdateObject(child, deltaTime);  // Added deltaTime parameter
            }
        }

        public T? FindObjectOfType<T>() where T : Component
        {
            return FindObjectsOfType<T>().FirstOrDefault();
        }

        public IEnumerable<T> FindObjectsOfType<T>() where T : Component
        {
            return _rootObjects
                .SelectMany(GetAllChildrenRecursive)
                .SelectMany(go => go.Components)
                .OfType<T>();
        }

        private IEnumerable<SceneObject> GetAllChildrenRecursive(SceneObject gameObject)
        {
            yield return gameObject;
            foreach (var child in gameObject.Children)
            {
                foreach (var descendant in GetAllChildrenRecursive(child))
                {
                    yield return descendant;
                }
            }
        }
    }
} 