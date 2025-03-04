using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;

namespace GameEngine.Editor.Viewport
{
    public class SceneViewport : NativeControlHost
    {
        private IPlatformHandle? _platformHandle;
        private IOpenGLContext? _glContext;
        private bool _initialized;
        private Vector3 _cameraPosition = new Vector3(0, 5, 10);
        private Vector3 _cameraTarget = Vector3.Zero;
        private float _rotation;

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            _platformHandle = base.CreateNativeControlCore(parent);
            InitializeOpenGL();
            return _platformHandle;
        }

        private void InitializeOpenGL()
        {
            if (_initialized || _platformHandle == null) return;

            var glInterface = AvaloniaLocator.Current.GetService<IWindowingPlatformGlFeature>();
            if (glInterface == null) throw new Exception("OpenGL is not supported on this platform");

            var platformHandle = _platformHandle;
            _glContext = glInterface.CreateOpenGlContext(platformHandle);

            MakeCurrent();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            _initialized = true;
            StartRenderLoop();
        }

        private void MakeCurrent()
        {
            _glContext?.MakeCurrent();
        }

        private void StartRenderLoop()
        {
            var timer = new global::Avalonia.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
            };

            timer.Tick += (sender, e) =>
            {
                if (!_initialized) return;
                Render();
            };

            timer.Start();
        }

        private void Render()
        {
            if (!_initialized) return;

            MakeCurrent();

            var width = Bounds.Width;
            var height = Bounds.Height;
            GL.Viewport(0, 0, (int)width, (int)height);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Create view matrix
            var view = Matrix4.LookAt(_cameraPosition, _cameraTarget, Vector3.UnitY);

            // Create projection matrix
            var projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45.0f),
                (float)width / (float)height,
                0.1f,
                100.0f);

            // Create model matrix
            _rotation += 0.01f;
            var model = Matrix4.CreateRotationY(_rotation);

            // Draw a simple coordinate system
            DrawCoordinateSystem(model, view, projection);

            // Draw a sample cube
            DrawCube(model, view, projection);

            _glContext?.SwapBuffers();
        }

        private void DrawCoordinateSystem(Matrix4 model, Matrix4 view, Matrix4 projection)
        {
            GL.Begin(PrimitiveType.Lines);

            // X axis (red)
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.Vertex3(5.0f, 0.0f, 0.0f);

            // Y axis (green)
            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 5.0f, 0.0f);

            // Z axis (blue)
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 5.0f);

            GL.End();
        }

        private void DrawCube(Matrix4 model, Matrix4 view, Matrix4 projection)
        {
            GL.Begin(PrimitiveType.Quads);

            // Front face (red)
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);

            // Back face (green)
            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);

            // Top face (blue)
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);

            // Bottom face (yellow)
            GL.Color3(1.0f, 1.0f, 0.0f);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);

            // Right face (magenta)
            GL.Color3(1.0f, 0.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);

            // Left face (cyan)
            GL.Color3(0.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);

            GL.End();
        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            _initialized = false;
            _glContext?.Dispose();
            _glContext = null;
            base.DestroyNativeControlCore(control);
        }
    }
} 