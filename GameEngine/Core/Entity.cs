using System;
using System.Collections.Generic;

namespace GameEngine
{
    public class Entity
    {
        private static int nextId = 0;
        public readonly int Id;
        private readonly Dictionary<Type, Component> components;

        public Entity()
        {
            Id = nextId++;
            components = new Dictionary<Type, Component>();
        }

        public T AddComponent<T>(T component) where T : Component
        {
            components[typeof(T)] = component;
            component.Entity = this;
            return component;
        }

        public T GetComponent<T>() where T : Component
        {
            return components.TryGetValue(typeof(T), out var component) ? (T)component : null;
        }

        public IEnumerable<Component> GetComponents()
        {
            return components.Values;
        }
    }
} 