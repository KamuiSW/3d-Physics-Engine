using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameEngine.Graphics.Camera
{
    public class CameraController : Component
    {
        private Camera camera;
        private float moveSpeed = 5.0f;
        private float mouseSensitivity = 0.2f;
        private Vector2 lastMousePos;
        private bool firstMouse = true;

        public override void Initialize()
        {
            camera = Entity.GetComponent<Camera>();
            if (camera == null)
                throw new Exception("CameraController requires a Camera component");
        }

        public override void Update(float deltaTime)
        {
            HandleKeyboardInput(deltaTime);
            HandleMouseInput();
        }

        private void HandleKeyboardInput(float deltaTime)
        {
            if (InputManager.IsKeyDown(Keys.W))
                camera.Position += camera.Front * moveSpeed * deltaTime;
            if (InputManager.IsKeyDown(Keys.S))
                camera.Position -= camera.Front * moveSpeed * deltaTime;
            if (InputManager.IsKeyDown(Keys.A))
                camera.Position -= camera.Right * moveSpeed * deltaTime;
            if (InputManager.IsKeyDown(Keys.D))
                camera.Position += camera.Right * moveSpeed * deltaTime;
            if (InputManager.IsKeyDown(Keys.Space))
                camera.Position += Vector3.UnitY * moveSpeed * deltaTime;
            if (InputManager.IsKeyDown(Keys.LeftShift))
                camera.Position -= Vector3.UnitY * moveSpeed * deltaTime;
        }

        private void HandleMouseInput()
        {
            var mousePos = InputManager.GetMousePosition();

            if (firstMouse)
            {
                lastMousePos = mousePos;
                firstMouse = false;
                return;
            }

            float xOffset = mousePos.X - lastMousePos.X;
            float yOffset = lastMousePos.Y - mousePos.Y;
            lastMousePos = mousePos;

            xOffset *= mouseSensitivity;
            yOffset *= mouseSensitivity;

            camera.Yaw += xOffset;
            camera.Pitch = MathHelper.Clamp(camera.Pitch + yOffset, -89.0f, 89.0f);
        }
    }
} 