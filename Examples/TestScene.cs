using OpenTK.Mathematics;
using GameEngine.Graphics.Camera;
using GameEngine.Graphics.Rendering;

namespace GameEngine.Examples
{
    public class TestScene : Engine
    {
        private Entity cube;
        private Entity mainCamera;
        private Entity light;

        protected override void OnLoad()
        {
            // Create and setup camera
            mainCamera = CreateEntity();
            var camera = mainCamera.AddComponent(new Camera(new Vector3(0, 0, 5)));
            mainCamera.AddComponent(new CameraController());

            // Create a cube
            cube = CreateEntity();
            var transform = cube.AddComponent(new TransformComponent());
            
            // Create cube mesh data
            float[] vertices = {
                // Front face
                -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f, // Bottom-left
                 0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f, // Bottom-right
                 0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f, // Top-right
                -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f  // Top-left
                // Add other faces...
            };

            uint[] indices = {
                0, 1, 2,
                2, 3, 0
                // Add other face indices...
            };

            // Create material
            var material = new Material
            {
                Shader = ResourceManager.LoadShader(
                    "Graphics/Shaders/Programs/PBR/pbr.vert",
                    "Graphics/Shaders/Programs/PBR/pbr.frag",
                    "pbr"),
                Albedo = new Vector3(1.0f, 0.5f, 0.31f),
                Metallic = 0.5f,
                Roughness = 0.5f
            };

            // Add mesh with material
            var mesh = cube.AddComponent(new Mesh(vertices, indices, material.Shader));

            // Create a light
            light = CreateEntity();
            var lightTransform = light.AddComponent<TransformComponent>();
            lightTransform.Position = new Vector3(2.0f, 2.0f, 2.0f);
        }

        protected override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Rotate the cube
            if (cube != null)
            {
                var transform = cube.GetComponent<TransformComponent>();
                transform.Rotation += new Vector3(0, 45.0f * deltaTime, 0); // 45 degrees per second
            }

            // Update light position for demonstration
            if (light != null)
            {
                var transform = light.GetComponent<TransformComponent>();
                float time = (float)gameTimer.Elapsed.TotalSeconds;
                transform.Position = new Vector3(
                    MathF.Cos(time) * 3.0f,
                    2.0f,
                    MathF.Sin(time) * 3.0f
                );
            }
        }
    }
} 