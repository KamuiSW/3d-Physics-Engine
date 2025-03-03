using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace GameEngine
{
    public class Engine
    {
        protected readonly Stopwatch gameTimer;
        private readonly List<Entity> entities;
        private readonly Window window;
        private const float TARGET_FPS = 60.0f;
        private const float FRAME_TIME = 1.0f / TARGET_FPS;

        public Engine(int width = 800, int height = 600, string title = "Game Engine")
        {
            gameTimer = new Stopwatch();
            entities = new List<Entity>();
            window = new Window(width, height, title);
            window.Load += OnLoad;
            window.UpdateFrame += OnUpdateFrame;
            window.RenderFrame += OnRenderFrame;
        }

        public void Start()
        {
            gameTimer.Start();
            window.Run();
        }

        protected virtual void OnLoad()
        {
            // Override in derived class
        }

        private void OnUpdateFrame(FrameEventArgs args)
        {
            float deltaTime = (float)args.Time;
            
            foreach (var entity in entities)
            {
                foreach (var component in entity.GetComponents())
                {
                    component.Update(deltaTime);
                }
            }
        }

        private void OnRenderFrame(FrameEventArgs args)
        {
            window.Render(entities);
        }

        public Entity CreateEntity()
        {
            var entity = new Entity();
            entities.Add(entity);
            return entity;
        }
    }
} 