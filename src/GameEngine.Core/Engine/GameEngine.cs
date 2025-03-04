using System;
using GameEngine.Core.Scene;

namespace GameEngine.Core
{
    public class GameEngine
    {
        private bool _isRunning;
        private DateTime _lastUpdateTime;
        
        public GameScene Scene { get; } = new GameScene();

        public void Start()
        {
            _isRunning = true;
            _lastUpdateTime = DateTime.Now;
            
            foreach (var obj in Scene.RootObjects)
            {
                InitializeObject(obj);
            }
            
            RunMainLoop();
        }

        private void InitializeObject(SceneObject obj)
        {
            foreach (var component in obj.Components)
            {
                component.Start();
            }
            
            foreach (var child in obj.Children)
            {
                InitializeObject(child);
            }
        }

        private void RunMainLoop()
        {
            while (_isRunning)
            {
                var deltaTime = (DateTime.Now - _lastUpdateTime).TotalSeconds;
                _lastUpdateTime = DateTime.Now;
                
                UpdateScene(deltaTime);
                RenderScene();
            }
        }

        private void UpdateScene(double deltaTime)
        {
            foreach (var obj in Scene.RootObjects)
            {
                UpdateObject(obj, deltaTime);
            }
        }

        private void UpdateObject(SceneObject obj, double deltaTime)
        {
            if (!obj.IsActive) return;
            
            foreach (var component in obj.Components)
            {
                if (component.Enabled)
                {
                    component.Update((float)deltaTime);
                }
            }

            foreach (var child in obj.Children)
            {
                UpdateObject(child, deltaTime);
            }
        }

        private void RenderScene()
        {
            // Rendering logic will be implemented in Phase 5
        }
    }
} 