using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace GameEngine
{
    public class Window : GameWindow
    {
        public Window(int width, int height, string title) 
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

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);
            base.OnResize(e);
        }

        public void Render(List<Entity> entities)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var entity in entities)
            {
                var mesh = entity.GetComponent<MeshComponent>();
                if (mesh != null)
                {
                    mesh.Render();
                }
            }

            SwapBuffers();
        }
    }
} 