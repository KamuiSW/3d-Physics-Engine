using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using GameEngine.Core.Graphics;

namespace GameEngine.Core.Graphics
{
    public class Material : IDisposable
    {
        private readonly Shader _shader;
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();
        private bool _disposed;
        private string _name = "New Material";

        public string Name
        {
            get => _name;
            set => _name = value ?? throw new ArgumentNullException(nameof(value));
        }

        public RenderQueue RenderQueue { get; set; } = RenderQueue.Geometry;
        public BlendMode BlendMode { get; set; } = BlendMode.Opaque;
        public CullMode CullMode { get; set; } = CullMode.Back;
        public bool DepthTest { get; set; } = true;
        public bool DepthWrite { get; set; } = true;
        public Shader Shader => _shader;

        public IReadOnlyDictionary<string, object> Properties => _properties;

        public Material(Shader shader)
        {
            _shader = shader ?? throw new ArgumentNullException(nameof(shader));
        }

        public void SetFloat(string name, float value)
        {
            _properties[name] = value;
        }

        public void SetInt(string name, int value)
        {
            _properties[name] = value;
        }

        public void SetVector2(string name, Vector2 value)
        {
            _properties[name] = value;
        }

        public void SetVector3(string name, Vector3 value)
        {
            _properties[name] = value;
        }

        public void SetVector4(string name, Vector4 value)
        {
            _properties[name] = value;
        }

        public void SetColor(string name, Color4 value)
        {
            _properties[name] = value;
        }

        public void SetMatrix4(string name, Matrix4 value)
        {
            _properties[name] = value;
        }

        public void SetTexture(string name, Texture value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
                
            _properties[name] = value;
        }

        public T? GetProperty<T>(string name)
        {
            if (_properties.TryGetValue(name, out object? value) && value is T typedValue)
            {
                return typedValue;
            }
            return default;
        }

        public void Apply()
        {
            _shader.Use();

            int textureUnit = 0;
            foreach (var property in _properties)
            {
                switch (property.Value)
                {
                    case float f:
                        _shader.SetFloat(property.Key, f);
                        break;
                    case int i:
                        _shader.SetInt(property.Key, i);
                        break;
                    case Vector2 v2:
                        _shader.SetVector2(property.Key, v2);
                        break;
                    case Vector3 v3:
                        _shader.SetVector3(property.Key, v3);
                        break;
                    case Vector4 v4:
                        _shader.SetVector4(property.Key, v4);
                        break;
                    case Color4 c:
                        _shader.SetColor(property.Key, c);
                        break;
                    case Matrix4 m:
                        _shader.SetMatrix4(property.Key, m);
                        break;
                    case Texture t:
                        t.Use(textureUnit);
                        _shader.SetInt(property.Key, textureUnit);
                        textureUnit++;
                        break;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose textures
                    foreach (var property in _properties.Values)
                    {
                        if (property is Texture texture)
                        {
                            texture.Dispose();
                        }
                    }
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public enum RenderQueue
    {
        Background = 1000,
        Geometry = 2000,
        AlphaTest = 2450,
        Transparent = 3000,
        Overlay = 4000
    }

    public enum BlendMode
    {
        Opaque,
        Transparent,
        Additive,
        Multiply
    }

    public enum CullMode
    {
        None,
        Front,
        Back
    }
} 