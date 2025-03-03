using OpenTK.Mathematics;
using GameEngine.Graphics.Camera;

namespace GameEngine.Examples
{
    public class TestScene : Engine
    {
        private Entity triangle;

        protected override void OnLoad()
        {
            // Create a triangle entity
            triangle = CreateEntity();
            var transform = triangle.AddComponent(new TransformComponent());
            
            // Create and setup camera
            var cameraEntity = CreateEntity();
            var camera = cameraEntity.AddComponent(new Camera(new Vector3(0, 0, 3)));
            
            // Set up basic triangle data
            float[] vertices = {
                -0.5f, -0.5f, 0.0f,  // Bottom-left vertex
                 0.5f, -0.5f, 0.0f,  // Bottom-right vertex
                 0.0f,  0.5f, 0.0f   // Top vertex
            };

            // Create a basic shader
            var shader = new Shader("Basic");
            
            // Add mesh component
            triangle.AddComponent(new MeshComponent(vertices, shader));
        }
    }
} 