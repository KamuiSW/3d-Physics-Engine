using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace GameEngine.Core.Graphics
{
    public class Mesh : IDisposable
    {
        private readonly int _vao;
        private readonly int _vbo;
        private readonly int _ebo;
        private readonly int _indexCount;
        private bool _disposed;

        public Vector3 Bounds { get; }
        public Vector3 Center { get; }

        public Mesh(float[] vertices, uint[] indices, VertexAttribute[] attributes)
        {
            _indexCount = indices.Length;

            // Calculate bounds
            CalculateBounds(vertices, out Vector3 min, out Vector3 max);
            Bounds = max - min;
            Center = (max + min) * 0.5f;

            // Create buffers
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            // Upload vertex data
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Upload index data
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // Set up vertex attributes
            int stride = 0;
            foreach (var attribute in attributes)
            {
                stride += attribute.Size * sizeof(float);
            }

            int offset = 0;
            for (int i = 0; i < attributes.Length; i++)
            {
                var attribute = attributes[i];
                GL.EnableVertexAttribArray(i);
                GL.VertexAttribPointer(
                    i,
                    attribute.Size,
                    VertexAttribPointerType.Float,
                    false,
                    stride,
                    offset
                );
                offset += attribute.Size * sizeof(float);
            }

            GL.BindVertexArray(0);
        }

        private void CalculateBounds(float[] vertices, out Vector3 min, out Vector3 max)
        {
            min = new Vector3(float.MaxValue);
            max = new Vector3(float.MinValue);

            for (int i = 0; i < vertices.Length; i += 3)
            {
                var vertex = new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]);
                min = Vector3.ComponentMin(min, vertex);
                max = Vector3.ComponentMax(max, vertex);
            }
        }

        public void Draw()
        {
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    GL.DeleteBuffer(_vbo);
                    GL.DeleteBuffer(_ebo);
                    GL.DeleteVertexArray(_vao);
                }
                _disposed = true;
            }
        }

        ~Mesh()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public struct VertexAttribute
    {
        public int Size { get; }
        public string Name { get; }

        public VertexAttribute(int size, string name)
        {
            Size = size;
            Name = name;
        }

        public static readonly VertexAttribute Position = new VertexAttribute(3, "Position");
        public static readonly VertexAttribute Normal = new VertexAttribute(3, "Normal");
        public static readonly VertexAttribute UV = new VertexAttribute(2, "UV");
        public static readonly VertexAttribute Color = new VertexAttribute(4, "Color");
        public static readonly VertexAttribute Tangent = new VertexAttribute(4, "Tangent");
    }

    public static class PrimitiveMeshes
    {
        public static Mesh CreateCube()
        {
            float[] vertices = {
                // Front face
                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f, // Bottom-left
                 0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f, // Bottom-right
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f, // Top-right
                -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f, // Top-left

                // Back face
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

                // Top face
                -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,

                // Bottom face
                -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

                // Right face
                 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,

                // Left face
                -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
                -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
                -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
                -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f
            };

            uint[] indices = {
                0,  1,  2,  2,  3,  0,  // Front
                4,  5,  6,  6,  7,  4,  // Back
                8,  9,  10, 10, 11, 8,  // Top
                12, 13, 14, 14, 15, 12, // Bottom
                16, 17, 18, 18, 19, 16, // Right
                20, 21, 22, 22, 23, 20  // Left
            };

            var attributes = new[]
            {
                VertexAttribute.Position,
                VertexAttribute.Normal,
                VertexAttribute.UV
            };

            return new Mesh(vertices, indices, attributes);
        }

        public static Mesh CreateSphere(int segments = 32)
        {
            var vertices = new List<float>();
            var indices = new List<uint>();

            for (int y = 0; y <= segments; y++)
            {
                float ySegment = (float)y / segments;
                float yPos = (float)Math.Cos(ySegment * Math.PI);
                float yRadius = (float)Math.Sin(ySegment * Math.PI);

                for (int x = 0; x <= segments; x++)
                {
                    float xSegment = (float)x / segments;
                    float xPos = (float)Math.Cos(xSegment * 2 * Math.PI) * yRadius;
                    float zPos = (float)Math.Sin(xSegment * 2 * Math.PI) * yRadius;

                    // Position
                    vertices.Add(xPos * 0.5f);
                    vertices.Add(yPos * 0.5f);
                    vertices.Add(zPos * 0.5f);

                    // Normal
                    vertices.Add(xPos);
                    vertices.Add(yPos);
                    vertices.Add(zPos);

                    // UV
                    vertices.Add(xSegment);
                    vertices.Add(ySegment);
                }
            }

            for (int y = 0; y < segments; y++)
            {
                for (int x = 0; x < segments; x++)
                {
                    uint current = (uint)(y * (segments + 1) + x);
                    uint next = current + 1;
                    uint bottom = (uint)((y + 1) * (segments + 1) + x);
                    uint bottomNext = bottom + 1;

                    indices.Add(current);
                    indices.Add(bottom);
                    indices.Add(next);

                    indices.Add(next);
                    indices.Add(bottom);
                    indices.Add(bottomNext);
                }
            }

            var attributes = new[]
            {
                VertexAttribute.Position,
                VertexAttribute.Normal,
                VertexAttribute.UV
            };

            return new Mesh(vertices.ToArray(), indices.ToArray(), attributes);
        }
    }
} 