using OpenTK.Mathematics;
using GameEngine.Core.Graphics;

namespace GameEngine.Core.Scene.Components
{
    public class MeshRenderer : Component
    {
        private Mesh? _mesh;
        private Material? _material;

        public Mesh? Mesh
        {
            get => _mesh;
            set => SetField(ref _mesh, value);
        }

        public Material? Material
        {
            get => _material;
            set => SetField(ref _material, value);
        }

        public void Render(Camera camera)
        {
            if (_mesh == null || _material == null || !Enabled)
                return;

            _material.Shader.Use();
            _material.Shader.SetMatrix4("model", Transform.WorldMatrix);
            _material.Shader.SetMatrix4("view", camera.ViewMatrix);
            _material.Shader.SetMatrix4("projection", camera.ProjectionMatrix);

            // Apply all material properties
            _material.Apply();

            _mesh.Draw();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _mesh?.Dispose();
            _material?.Dispose();
        }
    }
} 