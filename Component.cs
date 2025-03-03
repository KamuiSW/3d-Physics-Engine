namespace GameEngine
{
    public abstract class Component
    {
        public Entity Entity { get; internal set; }
        
        public virtual void Initialize() { }
        public virtual void Update(float deltaTime) { }
        public virtual void FixedUpdate(float fixedDeltaTime) { }
    }
} 