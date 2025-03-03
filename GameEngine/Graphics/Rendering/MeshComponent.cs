namespace GameEngine.Graphics.Rendering
{
    public class MeshComponent : Component
    {
        private int VAO, VBO;
        private readonly float[] vertices;
        private readonly Shader shader;

        public MeshComponent(float[] vertices, Shader shader)
        {
            this.vertices = vertices;
            this.shader = shader;
            Initialize();
        }

        private void Initialize()
        {
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        public void Render()
        {
            shader.Use();
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }
    }
} 