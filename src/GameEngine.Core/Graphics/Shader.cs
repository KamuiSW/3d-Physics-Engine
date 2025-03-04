using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameEngine.Core.Graphics
{
    public class Shader : IDisposable
    {
        private readonly int _handle;
        private readonly Dictionary<string, int> _uniformLocations;
        private bool _disposed;

        public Shader(string vertexPath, string fragmentPath)
        {
            // Load shader source
            string vertexSource = File.ReadAllText(vertexPath);
            string fragmentSource = File.ReadAllText(fragmentPath);

            // Create vertex shader
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            CompileShader(vertexShader);

            // Create fragment shader
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            CompileShader(fragmentShader);

            // Link program
            _handle = GL.CreateProgram();
            GL.AttachShader(_handle, vertexShader);
            GL.AttachShader(_handle, fragmentShader);
            LinkProgram(_handle);

            // Cleanup
            GL.DetachShader(_handle, vertexShader);
            GL.DetachShader(_handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            // Cache uniform locations
            GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out var uniformCount);
            _uniformLocations = new Dictionary<string, int>();

            for (var i = 0; i < uniformCount; i++)
            {
                var key = GL.GetActiveUniform(_handle, i, out _, out _);
                var location = GL.GetUniformLocation(_handle, key);
                _uniformLocations.Add(key, location);
            }
        }

        public Shader() : this(
            File.Exists("default.vert") ? "default.vert" : throw new FileNotFoundException("Missing default.vert"),
            File.Exists("default.frag") ? "default.frag" : throw new FileNotFoundException("Missing default.frag"))
        {
        }

        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetProgramInfoLog(program);
                throw new Exception($"Error occurred whilst linking Program({program}).\n\n{infoLog}");
            }
        }

        public void Use()
        {
            GL.UseProgram(_handle);
        }

        public void SetInt(string name, int value)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
            {
                GL.Uniform1(location, value);
            }
        }

        public void SetFloat(string name, float value)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
            {
                GL.Uniform1(location, value);
            }
        }

        public void SetVector2(string name, Vector2 value)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
            {
                GL.Uniform2(location, value);
            }
        }

        public void SetVector3(string name, Vector3 value)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
            {
                GL.Uniform3(location, value);
            }
        }

        public void SetVector4(string name, Vector4 value)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
            {
                GL.Uniform4(location, value);
            }
        }

        public void SetMatrix4(string name, Matrix4 value)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
            {
                GL.UniformMatrix4(location, false, ref value);
            }
        }

        public void SetColor(string name, Color4 value)
        {
            if (_uniformLocations.TryGetValue(name, out int location))
            {
                GL.Uniform4(location, value);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    GL.DeleteProgram(_handle);
                }
                _disposed = true;
            }
        }

        ~Shader()
        {
            GL.DeleteProgram(_handle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
} 