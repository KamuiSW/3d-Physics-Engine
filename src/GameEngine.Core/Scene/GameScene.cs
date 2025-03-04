using System.Collections.Generic;

namespace GameEngine.Core.Scene
{
    public class GameScene
    {
        public List<SceneObject> RootObjects { get; } = new();

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
    }
} 