using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GameEngine
{
    public class Mesh : Component
    {
        private int vao;
        private int vbo;
        private int ebo;
        private int indexCount;

        public Shader Shader { get; set; }

        public Mesh(float[] vertices, uint[] indices, Shader shader)
        {
            Shader = shader;
            indexCount = indices.Length;

            // Create and bind VAO
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            // Create and bind VBO
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Create and bind EBO
            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // Set vertex attributes
            // Position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            // Normal
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            // UV
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);
        }

        public override void Render()
        {
            if (!Entity.HasComponent<TransformComponent>())
                return;

            Shader.Use();
            
            var transform = Entity.GetComponent<TransformComponent>();
            Shader.SetMatrix4("model", transform.GetModelMatrix());

            if (Entity.HasComponent<Camera>())
            {
                var camera = Entity.GetComponent<Camera>();
                Shader.SetMatrix4("view", camera.GetViewMatrix());
                Shader.SetMatrix4("projection", camera.GetProjectionMatrix(16.0f/9.0f));
            }

            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
            GL.DeleteVertexArray(vao);
        }
    }
} 