using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameEngine
{
    public class Window : GameWindow
    {
        public Window(int width = 800, int height = 600, string title = "Game Engine") 
            : base(GameWindowSettings.Default, 
                  new NativeWindowSettings
                  {
                      Size = new Vector2i(width, height),
                      Title = title,
                      APIVersion = new Version(4, 1),
                      Profile = ContextProfile.Core
                  })
        {
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            base.OnLoad();
        }

        protected override void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnResize(OpenTK.Windowing.Common.ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            InputManager.Update(KeyboardState, MouseState);
            base.OnUpdateFrame(args);
        }
    }
} 