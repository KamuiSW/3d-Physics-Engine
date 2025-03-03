using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace GameEngine
{
    public class Engine
    {
        private bool isRunning;
        private readonly Stopwatch gameTimer;
        private const float TARGET_FPS = 60.0f;
        private const float FRAME_TIME = 1.0f / TARGET_FPS;
        private float accumulatedTime = 0.0f;
        private float deltaTime = 0.0f;
        private readonly Window window;
        private readonly List<Entity> entities;
        private readonly List<Entity> entitiesToAdd;
        private readonly List<Entity> entitiesToRemove;

        public Engine(int width = 800, int height = 600, string title = "Game Engine")
        {
            gameTimer = new Stopwatch();
            window = new Window(width, height, title);
            entities = new List<Entity>();
            entitiesToAdd = new List<Entity>();
            entitiesToRemove = new List<Entity>();
        }

        public void Start()
        {
            isRunning = true;
            gameTimer.Start();
            window.Run();  // This will handle the game loop for us
        }

        private void GameLoop()
        {
            float previousTime = (float)gameTimer.Elapsed.TotalSeconds;

            while (isRunning)
            {
                float currentTime = (float)gameTimer.Elapsed.TotalSeconds;
                deltaTime = currentTime - previousTime;
                previousTime = currentTime;

                accumulatedTime += deltaTime;

                // Fixed timestep updates
                while (accumulatedTime >= FRAME_TIME)
                {
                    FixedUpdate(FRAME_TIME);
                    accumulatedTime -= FRAME_TIME;
                }

                // Variable timestep update
                Update(deltaTime);

                // Render
                Render();
            }
        }

        protected virtual void FixedUpdate(float fixedDeltaTime)
        {
            // Will be used for physics updates
        }

        protected virtual void Update(float deltaTime)
        {
            // Add new entities
            if (entitiesToAdd.Count > 0)
            {
                entities.AddRange(entitiesToAdd);
                entitiesToAdd.Clear();
            }

            // Update all entities
            foreach (var entity in entities)
            {
                foreach (var component in entity.GetComponents())
                {
                    component.Update(deltaTime);
                }
            }

            // Remove marked entities
            if (entitiesToRemove.Count > 0)
            {
                foreach (var entity in entitiesToRemove)
                {
                    entities.Remove(entity);
                }
                entitiesToRemove.Clear();
            }
        }

        protected virtual void Render()
        {
            // Will be used for rendering
        }

        public void Stop()
        {
            isRunning = false;
            gameTimer.Stop();
        }

        public Entity CreateEntity()
        {
            var entity = new Entity();
            entitiesToAdd.Add(entity);
            return entity;
        }

        public void DestroyEntity(Entity entity)
        {
            entitiesToRemove.Add(entity);
        }
    }
} 